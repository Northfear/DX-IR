using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/3D Toggle Button")]
public class UIStateToggleBtn3D : ControlBase
{
	protected int curStateIndex;

	public int defaultState;

	[HideInInspector]
	public string[] states = new string[2] { "Unnamed", "Disabled" };

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[2]
	{
		new EZTransitionList(new EZTransition[1]
		{
			new EZTransition("From Prev")
		}),
		new EZTransitionList(new EZTransition[1]
		{
			new EZTransition("From State")
		})
	};

	private EZTransition prevTransition;

	[HideInInspector]
	public string[] stateLabels = new string[4] { "[\"]", "[\"]", "[\"]", "[\"]" };

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
				SetToggleState(curStateIndex);
			}
		}
	}

	public int StateNum
	{
		get
		{
			return curStateIndex;
		}
	}

	public string StateName
	{
		get
		{
			return states[curStateIndex];
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
		if (index == curStateIndex)
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
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ToggleState();
			if (soundToPlay != null)
			{
				soundToPlay.PlayOneShot(soundToPlay.clip);
			}
		}
		if (ptr.evt == whenToInvoke && scriptWithMethodToInvoke != null)
		{
			scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}
		base.OnInput(ref ptr);
	}

	protected override void Awake()
	{
		base.Awake();
		curStateIndex = defaultState;
	}

	public override void Start()
	{
		base.Start();
		if (!Application.isPlaying)
		{
			return;
		}
		for (int i = 0; i < states.Length; i++)
		{
			transitions[i].list[0].MainSubject = base.gameObject;
			if (spriteText != null)
			{
				transitions[i].list[0].AddSubSubject(spriteText.gameObject);
			}
		}
		if (base.collider == null)
		{
			AddCollider();
		}
	}

	public override void Copy(IControl c)
	{
		Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		base.Copy(c);
		if (!(c is UIStateToggleBtn3D))
		{
			return;
		}
		UIStateToggleBtn3D uIStateToggleBtn3D = (UIStateToggleBtn3D)c;
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			defaultState = uIStateToggleBtn3D.defaultState;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = uIStateToggleBtn3D.prevTransition;
			if (Application.isPlaying)
			{
				SetToggleState(uIStateToggleBtn3D.StateNum);
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = uIStateToggleBtn3D.scriptWithMethodToInvoke;
			methodToInvoke = uIStateToggleBtn3D.methodToInvoke;
			whenToInvoke = uIStateToggleBtn3D.whenToInvoke;
			delay = uIStateToggleBtn3D.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundToPlay = uIStateToggleBtn3D.soundToPlay;
		}
	}

	public int ToggleState()
	{
		SetToggleState(curStateIndex + 1);
		if (changeDelegate != null)
		{
			changeDelegate(this);
		}
		return curStateIndex;
	}

	public virtual void SetToggleState(int s)
	{
		curStateIndex = s % (states.Length - 1);
		UseStateLabel(curStateIndex);
		if (prevTransition != null)
		{
			prevTransition.StopSafe();
		}
		transitions[curStateIndex].list[0].Start();
		prevTransition = transitions[curStateIndex].list[0];
	}

	public virtual void SetToggleState(string stateName)
	{
		for (int i = 0; i < states.Length; i++)
		{
			if (states[i] == stateName)
			{
				SetToggleState(i);
				break;
			}
		}
	}

	protected void DisableMe()
	{
		UseStateLabel(states.Length - 1);
		if (prevTransition != null)
		{
			prevTransition.StopSafe();
		}
		transitions[states.Length - 1].list[0].Start();
		prevTransition = transitions[states.Length - 1].list[0];
	}

	public override int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));
		if (GUILayout.Button((!inspector) ? "Add State" : "+", (!inspector) ? "Button" : "ToolbarButton"))
		{
			List<string> list = new List<string>();
			list.AddRange(states);
			list.Insert(states.Length - 1, "State " + (states.Length - 1));
			states = list.ToArray();
			List<EZTransitionList> list2 = new List<EZTransitionList>();
			list2.AddRange(transitions);
			list2.Insert(transitions.Length - 1, new EZTransitionList(new EZTransition[1]
			{
				new EZTransition("From Prev")
			}));
			transitions = list2.ToArray();
			List<string> list3 = new List<string>();
			list3.AddRange(stateLabels);
			list3.Insert(stateLabels.Length - 1, "[\"]");
			stateLabels = list3.ToArray();
		}
		if (states.Length > 2 && selState != states.Length - 1)
		{
			if (GUILayout.Button((!inspector) ? "Delete State" : "-", (!inspector) ? "Button" : "ToolbarButton"))
			{
				List<string> list4 = new List<string>();
				list4.AddRange(states);
				list4.RemoveAt(selState);
				states = list4.ToArray();
				List<EZTransitionList> list5 = new List<EZTransitionList>();
				list5.AddRange(transitions);
				list5.RemoveAt(selState);
				transitions = list5.ToArray();
				List<string> list6 = new List<string>();
				list6.AddRange(stateLabels);
				list6.RemoveAt(selState);
				stateLabels = list6.ToArray();
			}
			defaultState %= states.Length;
		}
		if (inspector)
		{
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
		return 14;
	}

	public override int DrawPostStateSelectGUI(int selState)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));
		GUILayout.Space(20f);
		GUILayout.Label("State Name:");
		if (selState < states.Length - 1)
		{
			states[selState] = GUILayout.TextField(states[selState]);
		}
		else
		{
			GUILayout.TextField(states[selState]);
		}
		GUILayout.EndHorizontal();
		return 28;
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}

	public static UIStateToggleBtn3D Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIStateToggleBtn3D)gameObject.AddComponent(typeof(UIStateToggleBtn3D));
	}

	public static UIStateToggleBtn3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIStateToggleBtn3D)gameObject.AddComponent(typeof(UIStateToggleBtn3D));
	}
}
