using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Panel Tab")]
public class UIPanelTab : UIRadioBtn
{
	public bool toggle;

	public UIPanelManager panelManager;

	public UIPanelBase panel;

	public bool panelShowingAtStart = true;

	protected bool panelIsShowing = true;

	protected bool internalCall;

	public override bool Value
	{
		get
		{
			return base.Value;
		}
		set
		{
			base.Value = value;
			if (!toggle)
			{
				if (panelIsShowing == value)
				{
					return;
				}
			}
			else if (internalCall)
			{
				return;
			}
			if (panelManager != null)
			{
				if (value)
				{
					panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
				}
				else if (panelManager.CurrentPanel == panel)
				{
					panelManager.Dismiss();
				}
			}
			else if (value)
			{
				panel.BringIn();
			}
			else
			{
				panel.Dismiss();
			}
			panelIsShowing = value;
		}
	}

	public override void Start()
	{
		if (!m_started)
		{
			base.Start();
			if (Application.isPlaying)
			{
				panelIsShowing = panelShowingAtStart;
				Value = panelShowingAtStart;
			}
			if (managed && m_hidden)
			{
				Hide(true);
			}
		}
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
		{
			return;
		}
		internalCall = true;
		base.OnInput(ref ptr);
		if (m_controlIsEnabled && !IsHidden() && !(panel == null))
		{
			if (ptr.evt == whenToInvoke)
			{
				DoPanelStuff();
			}
			internalCall = false;
		}
	}

	protected void DoPanelStuff()
	{
		if (toggle)
		{
			if (panelManager != null)
			{
				if (panelManager.CurrentPanel == panel)
				{
					panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
					panelIsShowing = false;
				}
				else
				{
					panelManager.BringIn(panel);
					panelIsShowing = true;
				}
			}
			else
			{
				if (panelIsShowing)
				{
					panel.Dismiss();
				}
				else
				{
					panel.BringIn();
				}
				panelIsShowing = !panelIsShowing;
			}
			base.Value = panelIsShowing;
		}
		else if (panelManager != null)
		{
			panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else
		{
			panel.BringIn();
		}
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (s is UIPanelTab)
		{
			UIPanelTab uIPanelTab = (UIPanelTab)s;
			if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
			{
				toggle = uIPanelTab.toggle;
				panelManager = uIPanelTab.panelManager;
				panel = uIPanelTab.panel;
				panelShowingAtStart = uIPanelTab.panelShowingAtStart;
			}
			if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
			{
				Value = uIPanelTab.Value;
			}
		}
	}

	public new static UIPanelTab Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIPanelTab)gameObject.AddComponent(typeof(UIPanelTab));
	}

	public new static UIPanelTab Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIPanelTab)gameObject.AddComponent(typeof(UIPanelTab));
	}
}
