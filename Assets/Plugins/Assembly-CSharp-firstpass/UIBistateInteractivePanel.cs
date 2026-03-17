using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("EZ GUI/Panels/Bi-State Interactive Panel")]
public class UIBistateInteractivePanel : UIPanelBase
{
	public enum STATE
	{
		SHOWING = 0,
		HIDDEN = 1
	}

	protected STATE m_panelState;

	[HideInInspector]
	public EZTransitionList transitions = new EZTransitionList(new EZTransition[4]
	{
		new EZTransition("Bring In Forward"),
		new EZTransition("Bring In Back"),
		new EZTransition("Dismiss Forward"),
		new EZTransition("Dismiss Back")
	});

	public bool requireTap = true;

	public bool alwaysShowOnClick = true;

	public bool dismissOnOutsideClick = true;

	public bool dismissOnPeerClick;

	public bool dismissOnChildClick;

	public bool dismissOnMoveOff;

	public bool showOnChildClick = true;

	public STATE initialState = STATE.HIDDEN;

	protected int lastActionID = -1;

	protected POINTER_INFO.POINTER_TYPE lastPtrType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;

	protected POINTER_INFO.POINTER_TYPE lastListenerType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;

	public STATE State
	{
		get
		{
			return m_panelState;
		}
		set
		{
			SetPanelState(value);
		}
	}

	public bool IsShowing
	{
		get
		{
			return m_panelState == STATE.SHOWING;
		}
	}

	public override EZTransitionList Transitions
	{
		get
		{
			return transitions;
		}
	}

	public override void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
		{
			return;
		}
		if (inputDelegate != null)
		{
			inputDelegate(ref ptr);
		}
		lastPtrType = ptr.type;
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.PRESS:
			if (!requireTap)
			{
				PanelClicked(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.TAP:
			PanelClicked(ptr);
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
		{
			if (!(base.collider != null))
			{
				break;
			}
			RaycastHit hitInfo;
			if (base.collider.Raycast(ptr.ray, out hitInfo, ptr.rayDepth))
			{
				if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
				{
					ptr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
				}
				else
				{
					ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
				}
			}
			else if (dismissOnMoveOff && m_panelState == STATE.SHOWING)
			{
				SetPanelState(STATE.HIDDEN);
			}
			break;
		}
		}
		base.OnInput(ptr);
	}

	protected void PanelClicked(POINTER_INFO ptr)
	{
		if (ptr.actionID == lastActionID)
		{
			return;
		}
		lastActionID = ptr.actionID;
		if (ptr.callerIsControl)
		{
			if (m_panelState == STATE.HIDDEN && showOnChildClick)
			{
				SetPanelState(STATE.SHOWING);
			}
			else if (m_panelState == STATE.SHOWING && dismissOnChildClick)
			{
				SetPanelState(STATE.HIDDEN);
			}
		}
		else if (alwaysShowOnClick)
		{
			SetPanelState(STATE.SHOWING);
		}
		else
		{
			ToggleState();
		}
	}

	public void Awake()
	{
		m_panelState = initialState;
	}

	public void ToggleState()
	{
		if (m_panelState == STATE.HIDDEN)
		{
			SetPanelState(STATE.SHOWING);
		}
		else
		{
			SetPanelState(STATE.HIDDEN);
		}
	}

	protected void SetPanelState(STATE s)
	{
		if (m_panelState == s)
		{
			return;
		}
		m_panelState = s;
		if (dismissOnPeerClick || dismissOnOutsideClick)
		{
			if (m_panelState == STATE.SHOWING)
			{
				if ((lastPtrType & POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD) == lastPtrType)
				{
					UIManager.instance.AddMouseTouchPtrListener(ClickListener);
					lastListenerType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;
				}
				else
				{
					UIManager.instance.AddRayPtrListener(ClickListener);
					lastListenerType = POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD;
				}
			}
			else if ((lastListenerType & POINTER_INFO.POINTER_TYPE.MOUSE_TOUCHPAD) == lastListenerType)
			{
				UIManager.instance.RemoveMouseTouchPtrListener(ClickListener);
			}
			else
			{
				UIManager.instance.RemoveRayPtrListener(ClickListener);
			}
		}
		if (m_panelState == STATE.SHOWING)
		{
			base.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		}
		else
		{
			base.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		}
	}

	public override void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		if (mode == UIPanelManager.SHOW_MODE.BringInBack || mode == UIPanelManager.SHOW_MODE.BringInForward)
		{
			SetPanelState(STATE.SHOWING);
		}
		else
		{
			SetPanelState(STATE.HIDDEN);
		}
	}

	protected void ClickListener(POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			if (ptr.targetObj == null && dismissOnOutsideClick)
			{
				SetPanelState(STATE.HIDDEN);
			}
			else if (dismissOnPeerClick && (!(ptr.targetObj is Component) || !((Component)ptr.targetObj).transform.IsChildOf(base.transform)) && dismissOnPeerClick)
			{
				SetPanelState(STATE.HIDDEN);
			}
		}
	}

	public void Hide()
	{
		StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	public void Reveal()
	{
		StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}

	public static UIBistateInteractivePanel Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIBistateInteractivePanel)gameObject.AddComponent(typeof(UIBistateInteractivePanel));
	}

	public static UIBistateInteractivePanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIBistateInteractivePanel)gameObject.AddComponent(typeof(UIBistateInteractivePanel));
	}
}
