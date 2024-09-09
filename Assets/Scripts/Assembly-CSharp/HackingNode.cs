using System;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

[Serializable]
public class HackingNode : MonoBehaviour
{
	public HackingNode_Base m_node;

	public List<HackingNode> m_connections;

	public SpriteText m_ratingSprite;

	public PackedSprite m_iconSprite;

	public SpriteText m_captureRateSprite;

	public PackedSprite m_captureRateBackground;

	private bool m_playerTraversingTo;

	private bool m_playerCapturing;

	private bool m_playerCaptured;

	private bool m_subRoutineTraversingTo;

	private bool m_subRoutineCapturing;

	private bool m_subRoutineCaptured;

	private bool m_locked;

	private float m_playerTimeLeftToCapture;

	private float m_subRoutineTimeLeftToCapture;

	private HackingNodeState m_nodeState;

	private List<Hacking_Circuit> m_circuits = new List<Hacking_Circuit>();

	private bool m_ShortCircuiting;

	public bool IsLocked()
	{
		return m_locked;
	}

	public HackingNodeState GetNodeState()
	{
		return m_nodeState;
	}

	public bool IsPlayerTraversingTo()
	{
		return m_playerTraversingTo;
	}

	public bool IsCapturedByPlayer()
	{
		return m_playerCaptured;
	}

	public void StartPlayerTraversing()
	{
		m_playerTraversingTo = true;
	}

	public void StartSubRoutineTraversing()
	{
		m_subRoutineTraversingTo = true;
	}

	public HackingNode_Base SetNewHackingNode(HackingNodeType type)
	{
		int rating = -1;
		if ((bool)m_node)
		{
			rating = m_node.m_rating;
			UnityEngine.Object.DestroyImmediate(m_node);
		}
		switch (type)
		{
		case HackingNodeType.None:
		case HackingNodeType.Directory:
			m_node = base.gameObject.AddComponent<HackingNode_Directory>();
			break;
		case HackingNodeType.Registry:
			m_node = base.gameObject.AddComponent<HackingNode_Registry>();
			break;
		case HackingNodeType.IOPort:
			m_node = base.gameObject.AddComponent<HackingNode_IOPort>();
			break;
		case HackingNodeType.DataStore:
			m_node = base.gameObject.AddComponent<HackingNode_DataStore>();
			break;
		case HackingNodeType.API:
			m_node = base.gameObject.AddComponent<HackingNode_API>();
			break;
		case HackingNodeType.DiagnosticSubroutine:
			m_node = base.gameObject.AddComponent<HackingNode_DiagnosticSubroutine>();
			break;
		}
		m_node.m_rating = rating;
		m_node.Setup();
		return m_node;
	}

	public void OpenNodePanel()
	{
		HackingSystem.m_this.OpenNodeOptionsPanel(this);
	}

	public void LockNode()
	{
		m_locked = true;
		m_nodeState = HackingNodeState.Inaccessible;
		m_playerCaptured = false;
		m_iconSprite.Color = Color.gray;
		m_ratingSprite.Color = Color.gray;
	}

	public void UnlockNode()
	{
		if (m_nodeState != HackingNodeState.BeingCaptured)
		{
			m_locked = false;
			m_nodeState = HackingNodeState.Idle;
			m_iconSprite.Color = Color.white;
			m_ratingSprite.Color = Color.white;
		}
	}

	public int DetectionChance(int captureProgramRating, int slealthProgramRating)
	{
		int value = 30 + m_node.m_rating * 15 - captureProgramRating * 10 - slealthProgramRating * 5;
		return Mathf.Clamp(value, 5, 100);
	}

	public int TimeToCapture(int captureProgramRating)
	{
		int num = m_node.m_rating * 2 - captureProgramRating;
		return Mathf.Clamp(num, 1, num);
	}

	public void BeginCapturing(bool player)
	{
		if (player)
		{
			m_playerTraversingTo = false;
			m_playerCapturing = true;
			m_playerTimeLeftToCapture = TimeToCapture(HackingSystem.m_this.GetPlayerCaptureProgramRating());
		}
		else
		{
			m_subRoutineTraversingTo = false;
			m_subRoutineCapturing = true;
			m_subRoutineTimeLeftToCapture = TimeToCapture(HackingSystem.m_this.m_SubRoutineCaptureProgramRating);
		}
		m_nodeState = HackingNodeState.BeingCaptured;
	}

	public void NukeCapture()
	{
		CaptureComplete(true, true);
	}

	public void CaptureComplete(bool player, bool nuked = false)
	{
		m_iconSprite.DoAnim(m_node.m_type.ToString() + "_Captured");
		if (player)
		{
			m_playerCaptured = true;
			for (int i = 0; i < m_connections.Count; i++)
			{
				m_connections[i].UnlockNode();
				EventManager.Instance.PostEvent("Hack_Node_Capture", EventAction.PlaySound, base.gameObject);
			}
			m_node.CaptureNode();
			if (HackingSystem.m_this.GetHackingSystemState() != HackingSystemState.Results && !nuked && UnityEngine.Random.Range(0, 100) < DetectionChance(HackingSystem.m_this.GetPlayerCaptureProgramRating(), HackingSystem.m_this.GetPlayerStealthProgramRating()))
			{
				HackingSystem.m_this.TriggerAlarm();
			}
			GameObject gameObject = ((!nuked) ? (UnityEngine.Object.Instantiate(Globals.m_HackingGlobals.m_CaptureEffectPrefab) as GameObject) : (UnityEngine.Object.Instantiate(Globals.m_HackingGlobals.m_NukingEffectPrefab) as GameObject));
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return;
		}
		if (m_node.m_type == HackingNodeType.IOPort)
		{
			HackingSystem.m_this.CapturedIOPort();
			return;
		}
		m_subRoutineCaptured = true;
		List<HackingBridge> bridges = HackingSystem.m_this.GetBridges();
		for (int j = 0; j < m_connections.Count; j++)
		{
			if (m_connections[j].m_subRoutineCaptured || m_connections[j].m_subRoutineCapturing)
			{
				continue;
			}
			for (int k = 0; k < bridges.Count; k++)
			{
				HackingBridge hackingBridge = bridges[k];
				if (hackingBridge == null)
				{
					continue;
				}
				if (this == bridges[k].m_connectedNode1 && m_connections[j] == hackingBridge.m_connectedNode2)
				{
					if (!hackingBridge.m_connectedNode2.m_subRoutineCaptured && !hackingBridge.m_connectedNode2.m_subRoutineTraversingTo)
					{
						hackingBridge.StartTraversing(false, m_connections[j]);
					}
				}
				else if (m_connections[j] == bridges[k].m_connectedNode1 && this == hackingBridge.m_connectedNode2 && !hackingBridge.m_connectedNode1.m_subRoutineCaptured && !hackingBridge.m_connectedNode1.m_subRoutineTraversingTo)
				{
					hackingBridge.StartTraversing(false, m_connections[j]);
				}
			}
		}
	}

	public void RegisterCircuit(Hacking_Circuit circuit)
	{
		m_circuits.Add(circuit);
	}

	public void CircuitPressed()
	{
		bool flag = true;
		int num = 0;
		for (int i = 0; i < m_circuits.Count; i++)
		{
			if (!m_circuits[i].IsPressed())
			{
				flag = false;
			}
			else
			{
				num++;
			}
		}
		float num2 = 0.1f;
		float num3 = 0.9f + num2 * (float)(num - 1);
		EventManager.Instance.PostEvent("Hack_ShortCircuit_Node", EventAction.SetPitch, num3);
		EventManager.Instance.PostEvent("Hack_ShortCircuit_Node", EventAction.PlaySound, null, base.gameObject);
		if (flag)
		{
			for (int j = 0; j < m_circuits.Count; j++)
			{
				m_circuits[j].BeginShortCircuit();
			}
			m_ShortCircuiting = true;
		}
	}

	public void CircuitReleased()
	{
		for (int i = 0; i < m_circuits.Count; i++)
		{
			m_circuits[i].CancelShortCircuiting();
		}
		m_ShortCircuiting = false;
	}

	public void FinishedShortCircuiting()
	{
		for (int i = 0; i < m_circuits.Count; i++)
		{
			m_circuits[i].FinishedShortCircuiting();
		}
		m_ShortCircuiting = false;
		m_node.m_rating = HackingSystem.m_this.GetShortCircuitChangeRatingTo();
		m_ratingSprite.Text = m_node.m_rating.ToString();
		m_ratingSprite.UpdateMesh();
		EventManager.Instance.PostEvent("Hack_ShortCircuit_Success", EventAction.PlaySound, null, base.gameObject);
		if (HackingSystem.m_this.GetHackingSystemState() != HackingSystemState.Results && UnityEngine.Random.Range(0, 100) < DetectionChance(HackingSystem.m_this.GetPlayerCaptureProgramRating(), HackingSystem.m_this.GetPlayerStealthProgramRating()))
		{
			HackingSystem.m_this.TriggerAlarm();
		}
	}

	private void Awake()
	{
		if (m_node.m_type == HackingNodeType.IOPort)
		{
			m_nodeState = HackingNodeState.Idle;
			m_playerCaptured = true;
		}
		else if (m_node.m_type == HackingNodeType.DiagnosticSubroutine)
		{
			m_subRoutineCaptured = true;
		}
		SetCaptureRateInfo(0f, 0f);
	}

	private void Start()
	{
		if (m_node.m_type != HackingNodeType.DataStore)
		{
			return;
		}
		List<HackingNode> dataStoreList = HackingSystem.m_this.GetDataStoreList();
		bool flag = true;
		for (int i = 0; i < dataStoreList.Count; i++)
		{
			if (dataStoreList[i] == this)
			{
				flag = false;
			}
		}
		if (flag)
		{
			dataStoreList.Add(this);
		}
	}

	private void Update()
	{
		if (HackingSystem.m_this.GetHackingSystemState() == HackingSystemState.Results)
		{
			return;
		}
		switch (m_nodeState)
		{
		case HackingNodeState.Idle:
			if ((bool)m_captureRateSprite && m_captureRateSprite.color.a > 0f)
			{
				SetCaptureRateInfo(100f, Mathf.Max(m_captureRateSprite.color.a - 1f * Time.deltaTime, 0f));
			}
			break;
		case HackingNodeState.BeingCaptured:
			if (m_playerCapturing)
			{
				m_playerTimeLeftToCapture -= Time.deltaTime;
				m_playerTimeLeftToCapture = Mathf.Clamp(m_playerTimeLeftToCapture, 0f, m_playerTimeLeftToCapture);
				float alpha = Mathf.Min(m_captureRateSprite.color.a + 2f * Time.deltaTime, 1f);
				SetCaptureRateInfo((1f - m_playerTimeLeftToCapture / (float)TimeToCapture(HackingSystem.m_this.GetPlayerCaptureProgramRating())) * 100f, alpha);
				if (m_playerTimeLeftToCapture <= 0f)
				{
					m_playerCapturing = false;
					CaptureComplete(true);
				}
			}
			if (m_subRoutineCapturing)
			{
				m_subRoutineTimeLeftToCapture -= Time.deltaTime * HackingSystem.m_this.GetSubRoutineUpdateSpeed();
				if (m_subRoutineTimeLeftToCapture <= 0f)
				{
					m_subRoutineCapturing = false;
					CaptureComplete(false);
				}
			}
			if (!m_playerCapturing && !m_subRoutineCapturing)
			{
				m_nodeState = HackingNodeState.Idle;
			}
			break;
		}
		if (m_ShortCircuiting)
		{
			Globals.m_PlayerController.UseEnergy(HackingSystem.m_this.m_ShortCircuitingDrainRate * Time.deltaTime);
			HackingSystem.m_this.SetCurrentEnergy(Globals.m_PlayerController.GetCurrentEnergy(), true);
			if (Globals.m_PlayerController.GetCurrentEnergy() <= 0f)
			{
				CircuitReleased();
			}
		}
	}

	private void SetCaptureRateInfo(float capturePercent, float alpha)
	{
		if ((bool)m_captureRateSprite)
		{
			Color color = m_captureRateSprite.color;
			color.a = alpha;
			m_captureRateSprite.SetColor(color);
			m_captureRateSprite.Text = Mathf.Ceil(capturePercent) + "%";
		}
		if ((bool)m_captureRateBackground)
		{
			Color color2 = m_captureRateBackground.color;
			color2.a = alpha;
			m_captureRateBackground.SetColor(color2);
		}
	}
}
