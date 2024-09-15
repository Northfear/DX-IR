using System.Reflection;
using Fabric;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Button")]
public class UIButton : AutoSpriteControlBase
{
	public enum CONTROL_STATE
	{
		NORMAL = 0,
		OVER = 1,
		ACTIVE = 2,
		DISABLED = 3
	}

	protected CONTROL_STATE m_ctrlState;

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[4]
	{
		new TextureAnim("Normal"),
		new TextureAnim("Over"),
		new TextureAnim("Active"),
		new TextureAnim("Disabled")
	};

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

	public SpriteRoot[] layers = new SpriteRoot[0];

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public string methodToInvokeOnPress = string.Empty;

	public string methodToInvokeOnRelease = string.Empty;

	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	public float delay;

	public AudioSource soundOnOver;

	public AudioSource soundOnClick;

	public SoundEvent[] eventsOnClick;

	public bool repeat;

	public bool alwaysFinishActiveTransition;

	protected bool transitionQueued;

	protected EZTransition nextTransition;

	protected CONTROL_STATE nextState;

	protected int[,] stateIndices;

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
			bool flag = m_controlIsEnabled;
			m_controlIsEnabled = value;
			if (!value)
			{
				SetControlState(CONTROL_STATE.DISABLED);
			}
			else if (!flag)
			{
				SetControlState(CONTROL_STATE.NORMAL);
			}
		}
	}

	public override TextureAnim[] States
	{
		get
		{
			return states;
		}
		set
		{
			states = value;
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
			if (!flag || !(spriteText != null) || !Application.isPlaying)
			{
				return;
			}
			for (int i = 0; i < transitions.Length; i++)
			{
				for (int j = 0; j < transitions[i].list.Length; j++)
				{
					transitions[i].list[j].AddSubSubject(spriteText.gameObject);
				}
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
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (inputDelegate != null)
		{
			inputDelegate(ref ptr);
		}
		if (!m_controlIsEnabled || IsHidden())
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
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS && scriptWithMethodToInvoke != null && methodToInvokeOnPress != string.Empty)
		{
			MethodInfo method = scriptWithMethodToInvoke.GetType().GetMethod(methodToInvokeOnPress);
			method.Invoke(scriptWithMethodToInvoke, new object[1] { ptr });
		}
		if ((ptr.evt == POINTER_INFO.INPUT_EVENT.TAP || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE) && scriptWithMethodToInvoke != null && methodToInvokeOnRelease != string.Empty)
		{
			MethodInfo method2 = scriptWithMethodToInvoke.GetType().GetMethod(methodToInvokeOnRelease);
			method2.Invoke(scriptWithMethodToInvoke, new object[1] { ptr });
		}
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
		if (ptr.evt == whenToInvoke)
		{
			if (soundOnClick != null)
			{
				soundOnClick.PlayOneShot(soundOnClick.clip);
			}
			if (eventsOnClick != null)
			{
				for (int i = 0; i < eventsOnClick.Length; i++)
				{
					if (eventsOnClick[i] != null)
					{
						EventManager.Instance.PostEvent(eventsOnClick[i].SoundEventName, eventsOnClick[i].SoundEventAction, eventsOnClick[i].SoundEventParameter, base.gameObject);
					}
				}
			}
		}
		if (scriptWithMethodToInvoke != null && methodToInvoke != string.Empty)
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
		base.Start();
		if (Application.isPlaying)
		{
			aggregateLayers = new SpriteRoot[1][];
			aggregateLayers[0] = layers;
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
			stateIndices = new int[layers.Length, 4];
			for (int k = 0; k < layers.Length; k++)
			{
				if (layers[k] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + base.name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}
				stateIndices[k, 0] = layers[k].GetStateIndex("normal");
				stateIndices[k, 1] = layers[k].GetStateIndex("over");
				stateIndices[k, 2] = layers[k].GetStateIndex("active");
				stateIndices[k, 3] = layers[k].GetStateIndex("disabled");
				if (stateIndices[k, 0] != -1)
				{
					transitions[0].list[0].AddSubSubject(layers[k].gameObject);
					transitions[0].list[1].AddSubSubject(layers[k].gameObject);
					transitions[0].list[2].AddSubSubject(layers[k].gameObject);
				}
				if (stateIndices[k, 1] != -1)
				{
					transitions[1].list[0].AddSubSubject(layers[k].gameObject);
					transitions[1].list[1].AddSubSubject(layers[k].gameObject);
				}
				if (stateIndices[k, 2] != -1)
				{
					transitions[2].list[0].AddSubSubject(layers[k].gameObject);
					transitions[2].list[1].AddSubSubject(layers[k].gameObject);
				}
				if (stateIndices[k, 3] != -1)
				{
					transitions[3].list[0].AddSubSubject(layers[k].gameObject);
					transitions[3].list[1].AddSubSubject(layers[k].gameObject);
					transitions[3].list[2].AddSubSubject(layers[k].gameObject);
				}
				if (stateIndices[k, (int)m_ctrlState] != -1)
				{
					layers[k].SetState(stateIndices[k, (int)m_ctrlState]);
				}
				else
				{
					layers[k].Hide(true);
				}
			}
			if (base.collider == null)
			{
				AddCollider();
			}
			SetState((int)m_ctrlState);
		}
		if (managed && m_hidden)
		{
			Hide(true);
		}
	}

	protected override void OnEnable()
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

	protected override void OnDisable()
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

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UIButton))
		{
			return;
		}
		UIButton uIButton = (UIButton)s;
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = uIButton.prevTransition;
			if (Application.isPlaying)
			{
				SetControlState(uIButton.controlState);
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = uIButton.scriptWithMethodToInvoke;
			methodToInvoke = uIButton.methodToInvoke;
			whenToInvoke = uIButton.whenToInvoke;
			delay = uIButton.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundOnOver = uIButton.soundOnOver;
			soundOnClick = uIButton.soundOnClick;
			eventsOnClick = uIButton.eventsOnClick;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			repeat = uIButton.repeat;
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
		if (!alwaysFinishActiveTransition || (prevTransition != transitions[2].list[0] && (prevTransition != transitions[2].list[1] || !prevTransition.IsRunning())))
		{
			int ctrlState = (int)m_ctrlState;
			m_ctrlState = s;
			if (animations[(int)s].GetFrameCount() > 0)
			{
				SetState((int)s);
			}
			UseStateLabel((int)s);
			if (s == CONTROL_STATE.DISABLED)
			{
				m_controlIsEnabled = false;
			}
			else
			{
				m_controlIsEnabled = true;
			}
			UpdateCollider();
			for (int i = 0; i < layers.Length; i++)
			{
				if (stateIndices[i, (int)s] != -1)
				{
					layers[i].Hide(IsHidden());
					layers[i].SetState(stateIndices[i, (int)s]);
				}
				else
				{
					layers[i].Hide(true);
				}
			}
			if (prevTransition != null)
			{
				prevTransition.StopSafe();
			}
			StartTransition((int)s, ctrlState, suppressTransitions);
		}
		else
		{
			QueueTransition((int)s, 2, suppressTransitions);
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

	protected void StartTransition(int newState, int prevState, bool suppressTransition)
	{
		int num = DetermineNextTransition(newState, prevState);
		prevTransition = transitions[newState].list[num];
		if (prevTransition.animationTypes == null || prevTransition.animationTypes.Length < 1)
		{
			prevTransition = null;
			return;
		}
		prevTransition.Start();
		if (suppressTransition)
		{
			prevTransition.End();
		}
	}

	protected void QueueTransition(int newState, int prevState, bool suppressTransition)
	{
		if (deleted)
		{
			return;
		}
		nextTransition = transitions[newState].list[DetermineNextTransition(newState, prevState)];
		nextState = (CONTROL_STATE)newState;
		if (suppressTransition)
		{
			prevTransition.End();
			prevTransition = nextTransition;
			prevTransition.Start();
			prevTransition.End();
		}
		else
		{
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

	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
		{
			frameInfo.Copy(states[0].spriteFrames[0]);
		}
		base.InitUVs();
	}

	public static UIButton Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIButton)gameObject.AddComponent(typeof(UIButton));
	}

	public static UIButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIButton)gameObject.AddComponent(typeof(UIButton));
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}
}
