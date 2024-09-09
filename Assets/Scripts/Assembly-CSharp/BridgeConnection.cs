using System;
using UnityEngine;

[Serializable]
public class BridgeConnection
{
	public Sprite m_bridgeSprite;

	public Sprite m_bridgeProgressSprite;

	[HideInInspector]
	public bool m_bridgeInTransit;

	[HideInInspector]
	public float m_currentTransitTime;

	[HideInInspector]
	public HackingNode m_targetNode;
}
