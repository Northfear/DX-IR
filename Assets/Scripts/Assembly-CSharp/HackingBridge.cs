using UnityEngine;

public class HackingBridge : MonoBehaviour
{
	public HackingNode m_connectedNode1;

	public HackingNode m_connectedNode2;

	public HackingSystem m_hackingSystem;

	public Sprite m_directionalArrow;

	public BridgeConnection m_playerBridge = new BridgeConnection();

	public BridgeConnection m_subRoutineBridge = new BridgeConnection();

	private bool m_nukeCapture;

	public bool IsPlayerInTransit()
	{
		return m_playerBridge.m_bridgeInTransit;
	}

	public bool IsSubRoutineInTransit()
	{
		return m_subRoutineBridge.m_bridgeInTransit;
	}

	public void InitializeBridge(HackingNode node1, HackingNode node2)
	{
		m_connectedNode1 = node1;
		m_connectedNode2 = node2;
		ConstructBridge();
	}

	public void ConstructBridge()
	{
		Vector3 vector = m_connectedNode2.gameObject.transform.position - m_connectedNode1.gameObject.transform.position;
		vector.Normalize();
		int num = ((!(Vector3.Cross(Vector3.right, vector).z < 0f)) ? 1 : (-1));
		float angle = Vector3.Angle(Vector3.right, vector) * (float)num;
		if (!m_playerBridge.m_bridgeSprite && !m_playerBridge.m_bridgeProgressSprite)
		{
			GameObject gameObject = new GameObject("Player Bridge Connection");
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.layer = 12;
			m_playerBridge.m_bridgeSprite = gameObject.AddComponent<Sprite>();
			GameObject gameObject2 = new GameObject("Player Bridge Progress Connection");
			gameObject2.transform.parent = base.gameObject.transform;
			gameObject2.layer = 12;
			m_playerBridge.m_bridgeProgressSprite = gameObject2.AddComponent<Sprite>();
		}
		if (!m_subRoutineBridge.m_bridgeSprite && !m_subRoutineBridge.m_bridgeProgressSprite)
		{
			GameObject gameObject3 = new GameObject("Diagnostic Sub-Routine Bridge Connection");
			gameObject3.transform.parent = base.gameObject.transform;
			gameObject3.layer = 12;
			m_subRoutineBridge.m_bridgeSprite = gameObject3.AddComponent<Sprite>();
			GameObject gameObject4 = new GameObject("Diagnostic Sub-Routine Bridge Progress Connection");
			gameObject4.transform.parent = base.gameObject.transform;
			gameObject4.layer = 12;
			m_subRoutineBridge.m_bridgeProgressSprite = gameObject4.AddComponent<Sprite>();
		}
		SetupBridgeConnection(m_playerBridge, vector, angle, -1f, new Color(0.21f, 0.65f, 0.98f, 1f));
		SetupBridgeConnection(m_subRoutineBridge, vector, angle, 1f, Color.red);
		base.gameObject.transform.position = m_connectedNode1.gameObject.transform.position;
		base.gameObject.transform.parent = m_hackingSystem.m_hackingPanel.transform;
	}

	public void StartTraversing(bool player, HackingNode targetNode, bool nukeCapture = false)
	{
		m_nukeCapture = nukeCapture;
		BridgeSpriteInfo bridgeSpriteInfo = ((!m_connectedNode1.m_connections.Contains(m_connectedNode2) || !m_connectedNode2.m_connections.Contains(m_connectedNode1) || 1 == 0) ? m_hackingSystem.m_bridgeDottedLine : m_hackingSystem.m_bridgeLine);
		HackingNode hackingNode = ((!(targetNode == m_connectedNode1)) ? m_connectedNode1 : m_connectedNode2);
		if (player)
		{
			targetNode.StartPlayerTraversing();
		}
		else
		{
			targetNode.StartSubRoutineTraversing();
		}
		Vector3 vector = m_connectedNode2.gameObject.transform.position - m_connectedNode1.gameObject.transform.position;
		Vector3 vector2 = targetNode.gameObject.transform.position - hackingNode.gameObject.transform.position;
		int num = ((!(Vector3.Cross(Vector3.right, vector).z < 0f)) ? 1 : (-1));
		float angle = Vector3.Angle(Vector3.right, vector) * (float)num + (float)(((!(vector == vector2)) ? 180 : 0) % 360);
		Vector3 newPos = ((!(vector == vector2)) ? m_connectedNode2.gameObject.transform.position : m_connectedNode1.gameObject.transform.position);
		if (player && m_playerBridge.m_targetNode == null)
		{
			RotateIntoPlace(m_playerBridge, targetNode, newPos, angle, 0f - bridgeSpriteInfo.m_spacing);
		}
		else if (!player && m_subRoutineBridge.m_targetNode == null)
		{
			RotateIntoPlace(m_subRoutineBridge, targetNode, newPos, angle, bridgeSpriteInfo.m_spacing);
		}
	}

	private void SetupBridgeConnection(BridgeConnection connection, Vector3 dir, float angle, float offset, Color color)
	{
		BridgeSpriteInfo bridgeSpriteInfo = ((!m_connectedNode1.m_connections.Contains(m_connectedNode2) || !m_connectedNode2.m_connections.Contains(m_connectedNode1) || 1 == 0) ? m_hackingSystem.m_bridgeDottedLine : m_hackingSystem.m_bridgeLine);
		HackingSystem hackingSystem = Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem;
		float num = Vector3.Distance(m_connectedNode1.gameObject.transform.position, m_connectedNode2.gameObject.transform.position);
		num /= bridgeSpriteInfo.m_spriteDimensions.x;
		connection.m_bridgeSprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		connection.m_bridgeSprite.width = bridgeSpriteInfo.m_spriteDimensions.x * num;
		connection.m_bridgeSprite.height = bridgeSpriteInfo.m_spriteDimensions.y;
		connection.m_bridgeSprite.SetPixelDimensions((int)(bridgeSpriteInfo.m_pixelDimensions.x * num), (int)bridgeSpriteInfo.m_pixelDimensions.y);
		connection.m_bridgeSprite.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		connection.m_bridgeSprite.gameObject.renderer.material = bridgeSpriteInfo.m_material;
		connection.m_bridgeSprite.gameObject.layer = 12;
		connection.m_bridgeSprite.RenderCamera = hackingSystem.m_hackingCamera;
		connection.m_bridgeSprite.gameObject.transform.localPosition = new Vector3(bridgeSpriteInfo.m_spacing * offset, 0f, 5f);
		connection.m_bridgeProgressSprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		connection.m_bridgeProgressSprite.width = bridgeSpriteInfo.m_spriteDimensions.x * num;
		connection.m_bridgeProgressSprite.height = bridgeSpriteInfo.m_spriteDimensions.y;
		connection.m_bridgeProgressSprite.SetPixelDimensions((int)(bridgeSpriteInfo.m_pixelDimensions.x * num), (int)bridgeSpriteInfo.m_pixelDimensions.y);
		connection.m_bridgeProgressSprite.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		connection.m_bridgeProgressSprite.gameObject.renderer.material = bridgeSpriteInfo.m_material;
		connection.m_bridgeProgressSprite.gameObject.layer = 12;
		connection.m_bridgeProgressSprite.RenderCamera = hackingSystem.m_hackingCamera;
		connection.m_bridgeProgressSprite.SetColor(color);
		connection.m_bridgeProgressSprite.gameObject.transform.localPosition = new Vector3(bridgeSpriteInfo.m_spacing * offset, 0f, 4f);
	}

	private void Awake()
	{
	}

	private void Start()
	{
		SetBridgeProgress(m_playerBridge, 0f);
		SetBridgeProgress(m_subRoutineBridge, 0f);
	}

	private void Update()
	{
		if (m_playerBridge.m_bridgeInTransit)
		{
			float playerBridgeTime = HackingSystem.m_this.m_playerBridgeTime;
			m_playerBridge.m_currentTransitTime += Time.deltaTime;
			m_playerBridge.m_currentTransitTime = Mathf.Min(m_playerBridge.m_currentTransitTime, playerBridgeTime);
			SetBridgeProgress(m_playerBridge, m_playerBridge.m_currentTransitTime / playerBridgeTime);
			if (m_playerBridge.m_currentTransitTime == playerBridgeTime)
			{
				m_playerBridge.m_bridgeInTransit = false;
				if (!m_nukeCapture)
				{
					m_playerBridge.m_targetNode.BeginCapturing(true);
				}
				else
				{
					m_playerBridge.m_targetNode.NukeCapture();
				}
				m_playerBridge.m_targetNode = null;
			}
		}
		if (m_subRoutineBridge.m_bridgeInTransit)
		{
			float subRoutineBridgeTime = HackingSystem.m_this.m_subRoutineBridgeTime;
			m_subRoutineBridge.m_currentTransitTime += Time.deltaTime * HackingSystem.m_this.GetSubRoutineUpdateSpeed();
			m_subRoutineBridge.m_currentTransitTime = Mathf.Min(m_subRoutineBridge.m_currentTransitTime, subRoutineBridgeTime);
			SetBridgeProgress(m_subRoutineBridge, m_subRoutineBridge.m_currentTransitTime / subRoutineBridgeTime);
			if (m_subRoutineBridge.m_currentTransitTime == subRoutineBridgeTime)
			{
				m_subRoutineBridge.m_bridgeInTransit = false;
				m_subRoutineBridge.m_targetNode.BeginCapturing(false);
				m_subRoutineBridge.m_targetNode = null;
			}
		}
	}

	private void SetBridgeProgress(BridgeConnection connection, float progress)
	{
		if (!(m_connectedNode1 == null) && !(m_connectedNode2 == null))
		{
			BridgeSpriteInfo bridgeSpriteInfo = ((!m_connectedNode1.m_connections.Contains(m_connectedNode2) || !m_connectedNode2.m_connections.Contains(m_connectedNode1) || 1 == 0) ? m_hackingSystem.m_bridgeDottedLine : m_hackingSystem.m_bridgeLine);
			float num = Vector3.Distance(m_connectedNode1.gameObject.transform.position, m_connectedNode2.gameObject.transform.position);
			num /= bridgeSpriteInfo.m_spriteDimensions.x;
			connection.m_bridgeProgressSprite.width = bridgeSpriteInfo.m_spriteDimensions.x * num * progress;
			connection.m_bridgeProgressSprite.SetPixelDimensions((int)(bridgeSpriteInfo.m_pixelDimensions.x * num * progress), (int)bridgeSpriteInfo.m_pixelDimensions.y);
			connection.m_bridgeProgressSprite.SetSize(connection.m_bridgeProgressSprite.width, connection.m_bridgeProgressSprite.height);
		}
	}

	private void RotateIntoPlace(BridgeConnection connection, HackingNode targetNode, Vector3 newPos, float angle, float offset)
	{
		connection.m_bridgeInTransit = true;
		connection.m_currentTransitTime = 0f;
		connection.m_targetNode = targetNode;
		Vector3 localPosition = connection.m_bridgeSprite.gameObject.transform.localPosition;
		bool flag = Mathf.Abs(localPosition.x) > Mathf.Abs(localPosition.y);
		connection.m_bridgeSprite.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		connection.m_bridgeSprite.gameObject.transform.position = newPos;
		if (flag)
		{
			connection.m_bridgeSprite.gameObject.transform.localPosition += new Vector3(offset, 0f, 5f);
		}
		else
		{
			connection.m_bridgeSprite.gameObject.transform.localPosition += new Vector3(0f, offset, 5f);
		}
		connection.m_bridgeProgressSprite.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		connection.m_bridgeProgressSprite.gameObject.transform.position = newPos;
		if (flag)
		{
			connection.m_bridgeProgressSprite.gameObject.transform.localPosition += new Vector3(offset, 0f, 4f);
		}
		else
		{
			connection.m_bridgeProgressSprite.gameObject.transform.localPosition += new Vector3(0f, offset, 4f);
		}
	}
}
