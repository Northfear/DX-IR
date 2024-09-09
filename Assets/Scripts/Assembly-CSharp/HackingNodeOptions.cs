using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class HackingNodeOptions : MonoBehaviour
{
	public static HackingNodeOptions m_this;

	public PackedSprite m_background;

	public HackingNodeOptionsButton m_captureButton;

	public HackingNodeOptionsButton m_nukeButton;

	public HackingNodeOptionsButton m_stopWormButton;

	private HackingNode m_currentlySelectedNode;

	private FadeState m_fadeState;

	private float m_fadeRate = 4f;

	private float m_spritesAlpha;

	public void BringIn()
	{
		m_fadeState = FadeState.BringIn;
		m_currentlySelectedNode = HackingSystem.m_this.GetCurrentlySelectedNode();
	}

	public void Dismiss()
	{
		m_fadeState = FadeState.Dismiss;
	}

	public void CaptureButtonPressed()
	{
		HackingNode currentlySelectedNode = HackingSystem.m_this.GetCurrentlySelectedNode();
		if (currentlySelectedNode == null || currentlySelectedNode.IsPlayerTraversingTo() || currentlySelectedNode.IsCapturedByPlayer())
		{
			return;
		}
		List<HackingBridge> bridges = HackingSystem.m_this.GetBridges();
		bool flag = false;
		for (int i = 0; i < bridges.Count; i++)
		{
			HackingBridge hackingBridge = bridges[i];
			if (hackingBridge.m_connectedNode1 == currentlySelectedNode && hackingBridge.m_connectedNode2.IsCapturedByPlayer() && !hackingBridge.IsPlayerInTransit())
			{
				flag = true;
				hackingBridge.StartTraversing(true, currentlySelectedNode);
				break;
			}
			if (hackingBridge.m_connectedNode2 == currentlySelectedNode && hackingBridge.m_connectedNode1.IsCapturedByPlayer() && !hackingBridge.IsPlayerInTransit())
			{
				flag = true;
				hackingBridge.StartTraversing(true, currentlySelectedNode);
				break;
			}
		}
		EventManager.Instance.PostEvent("Hack_Select", EventAction.PlaySound, base.gameObject);
		HackingSystem.m_this.BeganHacking();
		if (flag)
		{
			HackingSystem.m_this.CloseNodeOptionsPanel();
		}
	}

	public void NukeButtonPressed()
	{
		if (Globals.m_Inventory.m_Nukes == 0 || m_currentlySelectedNode == null || m_currentlySelectedNode.IsPlayerTraversingTo() || m_currentlySelectedNode.IsCapturedByPlayer())
		{
			return;
		}
		List<HackingBridge> bridges = HackingSystem.m_this.GetBridges();
		bool flag = false;
		for (int i = 0; i < bridges.Count; i++)
		{
			HackingBridge hackingBridge = bridges[i];
			if (hackingBridge.m_connectedNode1 == m_currentlySelectedNode && hackingBridge.m_connectedNode2.IsCapturedByPlayer() && !hackingBridge.IsPlayerInTransit())
			{
				flag = true;
				hackingBridge.StartTraversing(true, m_currentlySelectedNode, true);
				break;
			}
			if (hackingBridge.m_connectedNode2 == m_currentlySelectedNode && hackingBridge.m_connectedNode1.IsCapturedByPlayer() && !hackingBridge.IsPlayerInTransit())
			{
				flag = true;
				hackingBridge.StartTraversing(true, m_currentlySelectedNode, true);
				break;
			}
		}
		HackingSystem.m_this.UseNuke();
		EventManager.Instance.PostEvent("Hack_Item_Nuke", EventAction.PlaySound, base.gameObject);
		HackingSystem.m_this.BeganHacking();
		if (flag)
		{
			HackingSystem.m_this.CloseNodeOptionsPanel();
		}
	}

	public void StopButtonPressed()
	{
		if (Globals.m_Inventory.m_StopWorms != 0 && HackingSystem.m_this.IsAlarmTriggered())
		{
			HackingSystem.m_this.UseStopWorm();
			EventManager.Instance.PostEvent("Hack_Item_Stop", EventAction.PlaySound, base.gameObject);
			HackingSystem.m_this.CloseNodeOptionsPanel();
		}
	}

	public void SetupNodeOptions()
	{
		if ((bool)m_currentlySelectedNode)
		{
			Color color = m_background.Color;
			color.a = m_spritesAlpha;
			m_background.SetColor(color);
			SetIconAlpha(m_captureButton.m_buttonIcon, m_captureButton.m_buttonBackground, !m_currentlySelectedNode.IsCapturedByPlayer() && !m_currentlySelectedNode.IsPlayerTraversingTo());
			SetIconAlpha(m_nukeButton.m_buttonIcon, m_nukeButton.m_buttonBackground, Globals.m_Inventory.m_Nukes > 0);
			SetIconAlpha(m_stopWormButton.m_buttonIcon, m_stopWormButton.m_buttonBackground, Globals.m_Inventory.m_StopWorms > 0 && HackingSystem.m_this.IsAlarmTriggered());
		}
	}

	private void SetIconAlpha(PackedSprite icon, PackedSprite background, bool active)
	{
		Color color = icon.Color;
		Color color2 = background.Color;
		color.a = m_spritesAlpha * ((!active) ? 0.25f : 1f);
		color2.a = m_spritesAlpha * ((!active) ? 0.25f : 1f);
		icon.SetColor(color);
		background.SetColor(color2);
	}

	private void Awake()
	{
		if (m_this == null)
		{
			m_this = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		switch (m_fadeState)
		{
		case FadeState.BringIn:
			m_spritesAlpha = Mathf.Min(m_spritesAlpha + Time.deltaTime * m_fadeRate, 1f);
			SetupNodeOptions();
			if (m_spritesAlpha == 1f)
			{
				m_fadeState = FadeState.Idle;
			}
			break;
		case FadeState.Dismiss:
			m_spritesAlpha = Mathf.Max(m_spritesAlpha - Time.deltaTime * m_fadeRate, 0f);
			SetupNodeOptions();
			if (m_spritesAlpha == 0f)
			{
				m_fadeState = FadeState.Idle;
			}
			break;
		}
	}
}
