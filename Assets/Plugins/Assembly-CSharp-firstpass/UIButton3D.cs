using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/3D Button")]
public class UIButton3D : ControlBase
{
	public enum CONTROL_STATE
	{
		NORMAL = 0,
		OVER = 1,
		ACTIVE = 2,
		DISABLED = 3
	}

	protected CONTROL_STATE m_ctrlState;

	protected string[] states = new string[4] { "Normal", "Over", "Active", "Disabled" };

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[4]
	{
		new EZTransitionList(new EZTransition[3]
		{
			new EZTransition("From Over"),
			new EZTransition("From Active"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Active")
		}),
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Over")
		}),
		new EZTransitionList(new EZTransition[3]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Over"),
			new EZTransition("From Active")
		})
	};

	private EZTransition prevTransition;

	[HideInInspector]
	public string[] stateLabels = new string[4] { "[\"]", "[\"]", "[\"]", "[\"]" };

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	public float delay;

	public AudioSource soundOnOver;

	public AudioSource soundOnClick;

	public bool repeat;

	public bool alwaysFinishActiveTransition;

	protected bool transitionQueued;

	protected EZTransition nextTransition;

	protected CONTROL_STATE nextState;

	protected bool m_started;

	public CONTROL_STATE controlState
	{
		get
		{
			return m_ctrlState;
		}
	}

	public override bool controlIsEnabled
	{
		get
		{
			return m_controlIsEnabled;
		}
		set
		{
			m_controlIsEnabled = value;
			if (!value)
			{
				SetControlState(CONTROL_STATE.DISABLED);
			}
			else
			{
				SetControlState(CONTROL_STATE.NORMAL);
			}
		}
	}

	public override string[] States
	{
		get
		{
			return states;
		}
	}

	public override EZTransitionList[] Transitions
	{
		get
		{
			return transitions;
		}
		set
		{
			transitions = value;
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool flag = spriteText == null;
			base.Text = value;
			if (flag && spriteText != null && Application.isPlaying)
			{
				transitions[0].list[0].AddSubSubject(spriteText.gameObject);
				transitions[0].list[1].AddSubSubject(spriteText.gameObject);
				transitions[0].list[2].AddSubSubject(spriteText.gameObject);
				transitions[1].list[0].AddSubSubject(spriteText.gameObject);
				transitions[1].list[1].AddSubSubject(spriteText.gameObject);
				transitions[2].list[0].AddSubSubject(spriteText.gameObject);
				transitions[2].list[1].AddSubSubject(spriteText.gameObject);
				transitions[3].list[0].AddSubSubject(spriteText.gameObject);
				transitions[3].list[1].AddSubSubject(spriteText.gameObject);
				transitions[3].list[2].AddSubSubject(spriteText.gameObject);
			}
		}
	}

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= transitions.Length)
		{
			return null;
		}
		return transitions[index];
	}

	public override string GetStateLabel(int index)
	{
		return stateLabels[index];
	}

	public override void SetStateLabel(int index, string label)
	{
		stateLabels[index] = label;
		if (index == (int)m_ctrlState)
		{
			UseStateLabel(index);
		}
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
		{
			return;
		}
		if (!m_controlIsEnabled)
		{
			base.OnInput(ref ptr);
			return;
		}
		if (inputDelegate != null)
		{
			inputDelegate(ref ptr);
		}
		if (!m_controlIsEnabled)
		{
			base.OnInput(ref ptr);
			return;
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.MOVE:
			if (m_ctrlState != CONTROL_STATE.OVER)
			{
				SetControlState(CONTROL_STATE.OVER);
				if (soundOnOver != null)
				{
					soundOnOver.PlayOneShot(soundOnOver.clip);
				}
			}
			break;
		case POINTER_INFO.INPUT_EVENT.PRESS:
		case POINTER_INFO.INPUT_EVENT.DRAG:
			SetControlState(CONTROL_STATE.ACTIVE);
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
			if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD && ptr.hitInfo.collider == base.collider)
			{
				SetControlState(CONTROL_STATE.OVER);
			}
			else
			{
				SetControlState(CONTROL_STATE.NORMAL);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			SetControlState(CONTROL_STATE.NORMAL);
			break;
		}
		base.OnInput(ref ptr);
		if (repeat)
		{
			if (m_ctrlState != CONTROL_STATE.ACTIVE)
			{
				return;
			}
		}
		else if (ptr.evt != whenToInvoke)
		{
			return;
		}
		if (ptr.evt == whenToInvoke && soundOnClick != null)
		{
			soundOnClick.PlayOneShot(soundOnClick.clip);
		}
		if (scriptWithMethodToInvoke != null)
		{
			scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}
		if (changeDelegate != null)
		{
			changeDelegate(this);
		}
	}

	public override void Start()
	{
		if (m_started)
		{
			return;
		}
		m_started = true;
		base.Start();
		if (!Application.isPlaying)
		{
			return;
		}
		for (int i = 0; i < transitions.Length; i++)
		{
			for (int j = 0; j < transitions[i].list.Length; j++)
			{
				transitions[i].list[j].MainSubject = base.gameObject;
				if (spriteText != null)
				{
					transitions[i].list[j].AddSubSubject(spriteText.gameObject);
				}
			}
		}
		if (base.collider == null)
		{
			AddCollider();
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (Application.isPlaying && m_started)
		{
			m_ctrlState = (CONTROL_STATE)(-1);
			if (controlIsEnabled)
			{
				SetControlState(CONTROL_STATE.NORMAL, true);
			}
			else
			{
				SetControlState(CONTROL_STATE.DISABLED, true);
			}
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (transitionQueued)
		{
			nextTransition.RemoveTransitionEndDelegate(RunFollowupTrans);
			transitionQueued = false;
		}
		if (EZAnimator.Exists() && !deleted)
		{
			bool flag = alwaysFinishActiveTransition;
			alwaysFinishActiveTransition = false;
			if (prevTransition != null && prevTransition.IsRunning())
			{
				prevTransition.End();
			}
			alwaysFinishActiveTransition = flag;
		}
		prevTransition = null;
	}

	public override void Copy(IControl c)
	{
		Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		base.Copy(c, flags);
		if (!(c is UIButton3D))
		{
			return;
		}
		UIButton3D uIButton3D = (UIButton3D)c;
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = uIButton3D.prevTransition;
			if (Application.isPlaying)
			{
				SetControlState(uIButton3D.controlState);
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = uIButton3D.scriptWithMethodToInvoke;
			methodToInvoke = uIButton3D.methodToInvoke;
			whenToInvoke = uIButton3D.whenToInvoke;
			delay = uIButton3D.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundOnOver = uIButton3D.soundOnOver;
			soundOnClick = uIButton3D.soundOnClick;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			repeat = uIButton3D.repeat;
		}
	}

	public virtual void SetControlState(CONTROL_STATE s)
	{
		SetControlState(s, false);
	}

	public virtual void SetControlState(CONTROL_STATE s, bool suppressTransitions)
	{
		if (m_ctrlState == s)
		{
			return;
		}
		if (!alwaysFinishActiveTransition || (prevTransition != transitions[2].list[0] && prevTransition != transitions[2].list[1]))
		{
			int ctrlState = (int)m_ctrlState;
			m_ctrlState = s;
			UseStateLabel((int)s);
			if (s == CONTROL_STATE.DISABLED)
			{
				m_controlIsEnabled = false;
			}
			else
			{
				m_controlIsEnabled = true;
			}
			if (!suppressTransitions)
			{
				if (prevTransition != null)
				{
					prevTransition.StopSafe();
				}
				StartTransition((int)s, ctrlState);
			}
		}
		else if (!suppressTransitions)
		{
			QueueTransition((int)s, 2);
		}
	}

	protected int DetermineNextTransition(int newState, int prevState)
	{
		int result = 0;
		switch (newState)
		{
		case 0:
			switch (prevState)
			{
			case 1:
				result = 0;
				break;
			case 2:
				result = 1;
				break;
			case 3:
				result = 2;
				break;
			}
			break;
		case 1:
			switch (prevState)
			{
			case 0:
				result = 0;
				break;
			case 2:
				result = 1;
				break;
			}
			break;
		case 2:
			switch (prevState)
			{
			case 0:
				result = 0;
				break;
			case 1:
				result = 1;
				break;
			}
			break;
		case 3:
			switch (prevState)
			{
			case 0:
				result = 0;
				break;
			case 1:
				result = 1;
				break;
			case 2:
				result = 2;
				break;
			}
			break;
		}
		return result;
	}

	protected void StartTransition(int newState, int prevState)
	{
		int num = DetermineNextTransition(newState, prevState);
		prevTransition = transitions[newState].list[num];
		prevTransition.Start();
	}

	protected void QueueTransition(int newState, int prevState)
	{
		if (!deleted)
		{
			nextTransition = transitions[newState].list[DetermineNextTransition(newState, prevState)];
			nextState = (CONTROL_STATE)newState;
			if (!transitionQueued)
			{
				prevTransition.AddTransitionEndDelegate(RunFollowupTrans);
			}
			transitionQueued = true;
		}
	}

	protected void RunFollowupTrans(EZTransition trans)
	{
		if (deleted)
		{
			trans.RemoveTransitionEndDelegate(RunFollowupTrans);
			return;
		}
		prevTransition = null;
		nextTransition = null;
		trans.RemoveTransitionEndDelegate(RunFollowupTrans);
		transitionQueued = false;
		SetControlState(nextState);
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}

	public static UIButton3D Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIButton3D)gameObject.AddComponent(typeof(UIButton3D));
	}

	public static UIButton3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIButton3D)gameObject.AddComponent(typeof(UIButton3D));
	}
}
