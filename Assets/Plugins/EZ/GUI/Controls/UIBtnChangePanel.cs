using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Change Panel Button")]
public class UIBtnChangePanel : UIButton
{
	public enum ChangeType
	{
		BringIn = 0,
		BringInForward = 1,
		BringInBack = 2,
		Dismiss = 3,
		DismissCurrent = 4,
		Toggle = 5,
		Forward = 6,
		Back = 7,
		BringInImmediate = 8,
		DismissImmediate = 9
	}

	public UIPanelManager panelManager;

	public ChangeType changeType;

	public bool detargetAllOthers = true;

	public string panel;

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
		if (ptr.evt == whenToInvoke)
		{
			if (panelManager == null)
			{
				if (UIPanelManager.instance == null)
				{
					base.OnInput(ref ptr);
					return;
				}
				panelManager = UIPanelManager.instance;
				if (panelManager == null)
				{
					base.OnInput(ref ptr);
					return;
				}
			}
			if (detargetAllOthers)
			{
				UIManager.instance.DetargetAllExcept(ptr.id);
			}
			switch (changeType)
			{
			case ChangeType.BringIn:
				panelManager.BringIn(panel);
				break;
			case ChangeType.BringInImmediate:
				panelManager.BringInImmediate(panel);
				break;
			case ChangeType.BringInForward:
				panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
				break;
			case ChangeType.BringInBack:
				panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Backwards);
				break;
			case ChangeType.Dismiss:
				if (panelManager.CurrentPanel != null && string.Equals(panelManager.CurrentPanel.name, panel, StringComparison.CurrentCultureIgnoreCase))
				{
					panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
				}
				break;
			case ChangeType.DismissCurrent:
				panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
				break;
			case ChangeType.DismissImmediate:
				panelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
				break;
			case ChangeType.Toggle:
				if (panelManager != null && panelManager.CurrentPanel != null && string.Equals(panelManager.CurrentPanel.name, panel, StringComparison.CurrentCultureIgnoreCase))
				{
					panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
				}
				else
				{
					panelManager.BringIn(panel);
				}
				break;
			case ChangeType.Forward:
				panelManager.MoveForward();
				break;
			case ChangeType.Back:
				panelManager.MoveBack();
				break;
			}
		}
		base.OnInput(ref ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (s is UIBtnChangePanel)
		{
			UIBtnChangePanel uIBtnChangePanel = (UIBtnChangePanel)s;
			if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
			{
				panelManager = uIBtnChangePanel.panelManager;
				changeType = uIBtnChangePanel.changeType;
				panel = uIBtnChangePanel.panel;
			}
		}
	}

	public new static UIBtnChangePanel Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIBtnChangePanel)gameObject.AddComponent(typeof(UIBtnChangePanel));
	}

	public new static UIBtnChangePanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIBtnChangePanel)gameObject.AddComponent(typeof(UIBtnChangePanel));
	}
}
