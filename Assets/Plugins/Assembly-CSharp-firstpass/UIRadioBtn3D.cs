using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/3D Radio Button")]
public class UIRadioBtn3D : ControlBase, IRadioButton
{
	protected enum CONTROL_STATE
	{
		True = 0,
		False = 1,
		Disabled = 2
	}

	private CONTROL_STATE state;

	protected bool btnValue;

	public bool useParentForGrouping = true;

	public int radioGroup;

	protected RadioBtnGroup group;

	public bool defaultValue;

	protected bool stateChangeWhileDeactivated;

	protected string[] states = new string[3] { "True", "False", "Disabled" };

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[3]
	{
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From False"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From True"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From True"),
			new EZTransition("From False")
		})
	};

	private EZTransition prevTransition;

	[HideInInspector]
	public string[] stateLabels = new string[3] { "[\"]", "[\"]", "[\"]" };

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	public float delay;

	public AudioSource soundToPlay;

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
				DisableMe();
			}
			else
			{
				SetButtonState();
			}
		}
	}

	public bool Value
	{
		get
		{
			return btnValue;
		}
		set
		{
			bool flag = btnValue;
			btnValue = value;
			if (btnValue)
			{
				PopOtherButtonsInGroup();
			}
			SetButtonState();
			if (flag != btnValue && changeDelegate != null)
			{
				changeDelegate(this);
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
		if (index == (int)state)
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
		if (ptr.evt == whenToInvoke)
		{
			Value = true;
			if (soundToPlay != null)
			{
				soundToPlay.PlayOneShot(soundToPlay.clip);
			}
			if (scriptWithMethodToInvoke != null)
			{
				scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
			}
		}
		base.OnInput(ref ptr);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (stateChangeWhileDeactivated)
		{
			SetButtonState();
			stateChangeWhileDeactivated = false;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (group != null)
		{
			group.buttons.Remove(this);
			group = null;
		}
	}

	public void SetGroup(GameObject parent)
	{
		SetGroup(parent.transform.GetHashCode());
	}

	public void SetGroup(int groupID)
	{
		if (group != null)
		{
			group.buttons.Remove(this);
			group = null;
		}
		radioGroup = groupID;
		group = RadioBtnGroup.GetGroup(groupID);
		group.buttons.Add(this);
		if (btnValue)
		{
			PopOtherButtonsInGroup();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		btnValue = defaultValue;
	}

	public override void Start()
	{
		base.Start();
		state = ((!controlIsEnabled) ? CONTROL_STATE.Disabled : ((!btnValue) ? CONTROL_STATE.False : CONTROL_STATE.True));
		if (Application.isPlaying)
		{
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
			int num = ((!btnValue) ? 1 : 0);
			num = ((!m_controlIsEnabled) ? 2 : num);
			if (base.collider == null)
			{
				AddCollider();
			}
		}
		Value = btnValue;
		if (useParentForGrouping && base.transform.parent != null)
		{
			SetGroup(base.transform.parent.GetHashCode());
		}
		else
		{
			SetGroup(radioGroup);
		}
	}

	public override void Copy(IControl c)
	{
		Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		if (!(c is UIRadioBtn3D))
		{
			return;
		}
		base.Copy(c);
		UIRadioBtn3D uIRadioBtn3D = (UIRadioBtn3D)c;
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			group = uIRadioBtn3D.group;
			defaultValue = uIRadioBtn3D.defaultValue;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = uIRadioBtn3D.prevTransition;
			if (Application.isPlaying)
			{
				Value = uIRadioBtn3D.Value;
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = uIRadioBtn3D.scriptWithMethodToInvoke;
			methodToInvoke = uIRadioBtn3D.methodToInvoke;
			whenToInvoke = uIRadioBtn3D.whenToInvoke;
			delay = uIRadioBtn3D.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundToPlay = uIRadioBtn3D.soundToPlay;
		}
	}

	protected void PopOtherButtonsInGroup()
	{
		if (group == null)
		{
			return;
		}
		for (int i = 0; i < group.buttons.Count; i++)
		{
			if ((UIRadioBtn3D)group.buttons[i] != this)
			{
				((UIRadioBtn3D)group.buttons[i]).Value = false;
			}
		}
	}

	protected virtual void SetButtonState()
	{
		int prevState = (int)state;
		state = ((!controlIsEnabled) ? CONTROL_STATE.Disabled : ((!btnValue) ? CONTROL_STATE.False : CONTROL_STATE.True));
		int num = (int)state;
		if (!base.gameObject.active)
		{
			stateChangeWhileDeactivated = true;
			return;
		}
		UseStateLabel(num);
		if (prevTransition != null)
		{
			prevTransition.StopSafe();
		}
		StartTransition(num, prevState);
	}

	protected void StartTransition(int newState, int prevState)
	{
		int num = 0;
		switch (newState)
		{
		case 0:
			switch (prevState)
			{
			case 1:
				num = 0;
				break;
			case 2:
				num = 1;
				break;
			}
			break;
		case 1:
			switch (prevState)
			{
			case 0:
				num = 0;
				break;
			case 2:
				num = 1;
				break;
			}
			break;
		case 2:
			switch (prevState)
			{
			case 0:
				num = 0;
				break;
			case 1:
				num = 1;
				break;
			}
			break;
		}
		transitions[newState].list[num].Start();
		prevTransition = transitions[newState].list[num];
	}

	protected void DisableMe()
	{
		UseStateLabel(states.Length - 1);
		if (prevTransition != null)
		{
			prevTransition.StopSafe();
		}
		StartTransition(2, (int)state);
		state = CONTROL_STATE.Disabled;
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}

	public static UIRadioBtn3D Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIRadioBtn3D)gameObject.AddComponent(typeof(UIRadioBtn3D));
	}

	public static UIRadioBtn3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIRadioBtn3D)gameObject.AddComponent(typeof(UIRadioBtn3D));
	}

	virtual string IRadioButton.get_name()
	{
		return base.name;
	}

	virtual void IRadioButton.set_name(string value)
	{
		base.name = value;
	}
}
