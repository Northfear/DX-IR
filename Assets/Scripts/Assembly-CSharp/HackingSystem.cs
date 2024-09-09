using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class HackingSystem : MonoBehaviour
{
	public delegate void CallbackDelegate();

	private const int AlertBackgroundImages = 3;

	private static float m_subRoutineDefaultSpeed = 1f;

	private static float m_subRoutineStopWormSpeed;

	public static HackingSystem m_this;

	public Camera m_hackingCamera;

	public UIPanelManager m_panelManager;

	public UIPanel m_hackingPanel;

	public UIPanel m_hackingNodeOptionsPanel;

	public UIPanel m_hackingVictoryResultPanel;

	public UIPanel m_hackingLossResultPanel;

	public UIPanel m_AlertPanel;

	public UIButton m_autoHackButton;

	public SpriteText m_TraceTimerText;

	public SpriteText m_stopWormCountText;

	public SpriteText m_nukeCountText;

	public BridgeSpriteInfo m_bridgeLine = new BridgeSpriteInfo();

	public BridgeSpriteInfo m_bridgeDottedLine = new BridgeSpriteInfo();

	public List<HackingBridge> m_nodeBridges = new List<HackingBridge>();

	private List<HackingNode> m_subRoutines = new List<HackingNode>();

	private List<float> m_subRoutinesTraceTime = new List<float>();

	public HackingNode m_startingNode;

	public float m_defaultZoom = 0.75f;

	public float m_playerBridgeTime = 1f;

	public float m_subRoutineBridgeTime = 1f;

	public float m_timeToShortCircuit = 1f;

	public int m_shortCircuitChangeRatingTo;

	public float m_ShortCircuitingDrainRate = 2f;

	public int m_SubRoutineCaptureProgramRating = 1;

	private HackingNode m_currentlySelectedNode;

	private int m_numberOfRegistries;

	private int m_numberOfCapturedRegistries;

	private int m_tempNukes;

	private int m_tempStopWorms;

	private float m_subRoutineUpdateSpeed = 1f;

	private float m_stopWormTimer;

	private HackingSystemState m_hackingSystemState;

	private bool m_alarmTriggered;

	private List<HackingNode> m_dataStores = new List<HackingNode>();

	private bool m_giveAllRewards;

	private bool m_beganHacking;

	private float m_CurrentChargeTimeOffset;

	private float m_ChargeTimeCycleDuration = 1.25f;

	private float m_ChargeTimeFlashDuration = 0.5f;

	private PackedSprite[] m_EnergyContainers = new PackedSprite[3];

	private PackedSprite[] m_EnergyBars = new PackedSprite[3];

	private SimpleSprite[] m_AlertBackground = new SimpleSprite[3];

	private Color m_NormalBackgroundColor = new Color(0f, 0f, 0f, 0.9f);

	private Color m_AlertBackgroundColor = new Color(0.5f, 0.09f, 0.03f, 0.9f);

	private SpriteText m_AutoHackCount;

	private CallbackDelegate m_onVictoryCallback;

	private CallbackDelegate m_onLossCallback;

	private CallbackDelegate m_onExitHackingCallback;

	public static void SetOnVictoryCallback(CallbackDelegate del)
	{
		m_this.m_onVictoryCallback = del;
	}

	public static void SetOnLossCallback(CallbackDelegate del)
	{
		m_this.m_onLossCallback = del;
	}

	public static void SetOnExitCallback(CallbackDelegate del)
	{
		m_this.m_onExitHackingCallback = del;
	}

	public UIPanel GetHackingPanel()
	{
		return m_hackingPanel;
	}

	public HackingNode GetCurrentlySelectedNode()
	{
		return m_currentlySelectedNode;
	}

	public List<HackingBridge> GetBridges()
	{
		return m_nodeBridges;
	}

	public HackingSystemState GetHackingSystemState()
	{
		return m_hackingSystemState;
	}

	public List<HackingNode> GetDataStoreList()
	{
		return m_dataStores;
	}

	public bool IsAlarmTriggered()
	{
		return m_alarmTriggered;
	}

	public bool IsGivingAllRewards()
	{
		return m_giveAllRewards;
	}

	public float GetSubRoutineUpdateSpeed()
	{
		return m_subRoutineUpdateSpeed;
	}

	public float GetTimeToShortCircuit()
	{
		return m_timeToShortCircuit;
	}

	public int GetShortCircuitChangeRatingTo()
	{
		return m_shortCircuitChangeRatingTo;
	}

	public int GetPlayerCaptureProgramRating()
	{
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		return augmentation_HackingCapture.GetCaptureProgramRating();
	}

	public int GetPlayerStealthProgramRating()
	{
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		return augmentation_HackingCapture.GetStealthProgramRating();
	}

	public void UseNuke()
	{
		Globals.m_Inventory.m_Nukes--;
		SetItemCountText(m_nukeCountText, Globals.m_Inventory.m_Nukes);
	}

	public void UseStopWorm()
	{
		Globals.m_Inventory.m_StopWorms--;
		SetItemCountText(m_stopWormCountText, Globals.m_Inventory.m_StopWorms);
		m_stopWormTimer = 5f;
		m_subRoutineUpdateSpeed = m_subRoutineStopWormSpeed;
	}

	public void AddNuke()
	{
		m_tempNukes++;
	}

	public void AddStopWorm()
	{
		m_tempStopWorms++;
	}

	private void SetItemCountText(SpriteText text, int count)
	{
		text.Text = string.Format("{0:00}", count);
	}

	public void AutoHackButtonPressed()
	{
		if (!m_beganHacking)
		{
			m_giveAllRewards = true;
			Globals.m_Inventory.m_AutoHacks--;
			m_AutoHackCount.Text = "-" + Globals.m_Inventory.m_AutoHacks + "-";
			m_numberOfCapturedRegistries = m_numberOfRegistries;
			CapturedRegistry();
		}
	}

	public void BeganHacking()
	{
		if (!m_beganHacking)
		{
			m_autoHackButton.SetColor(Color.gray);
			m_AutoHackCount.SetColor(Color.gray);
			m_autoHackButton.gameObject.collider.enabled = false;
			m_AutoHackCount.Text = "-" + Globals.m_Inventory.m_AutoHacks + "-";
		}
		m_beganHacking = true;
	}

	public void OpenNodeOptionsPanel(HackingNode node)
	{
		if (node.GetNodeState() != 0 && m_hackingSystemState == HackingSystemState.Hacking)
		{
			m_currentlySelectedNode = node;
			Vector3 position = node.gameObject.transform.position;
			m_panelManager.BringIn(m_hackingNodeOptionsPanel.index);
			Vector3 position2 = m_panelManager.CurrentPanel.gameObject.transform.position;
			position2.x = position.x;
			position2.y = position.y;
			m_panelManager.CurrentPanel.gameObject.transform.position = position2;
			HackingNodeOptions.m_this.BringIn();
		}
	}

	public void CloseNodeOptionsPanel()
	{
		m_currentlySelectedNode = null;
		m_panelManager.BringIn(m_hackingPanel);
		HackingNodeOptions.m_this.Dismiss();
	}

	public void CapturedRegistry()
	{
		m_numberOfCapturedRegistries++;
		if (m_numberOfCapturedRegistries >= m_numberOfRegistries)
		{
			if (m_onVictoryCallback != null)
			{
				m_onVictoryCallback();
			}
			Globals.m_Inventory.m_Nukes += m_tempNukes;
			m_subRoutineUpdateSpeed = 0f;
			Hacking_VictoryResults.m_this.SetupResults();
			m_hackingVictoryResultPanel.BringIn();
			m_hackingSystemState = HackingSystemState.Results;
		}
		EventManager.Instance.PostEvent("Music_Game", EventAction.SetSwitch, "Level");
		EventManager.Instance.PostEvent("Hack_Success", EventAction.PlaySound, base.gameObject);
		EventManager.Instance.PostEvent("Hack_Access_Granted", EventAction.PlaySound, base.gameObject);
		EventManager.Instance.PostEvent("Hack_Alarm_Timer", EventAction.StopSound, null);
	}

	public void CaptureSubRoutine()
	{
		if (m_onVictoryCallback != null)
		{
			m_onVictoryCallback();
		}
		Globals.m_Inventory.m_Nukes += m_tempNukes;
		m_subRoutineUpdateSpeed = 0f;
		m_giveAllRewards = true;
		Hacking_VictoryResults.m_this.SetupResults();
		m_hackingVictoryResultPanel.BringIn();
		m_hackingSystemState = HackingSystemState.Results;
		EventManager.Instance.PostEvent("Hack_Success", EventAction.PlaySound, base.gameObject);
		EventManager.Instance.PostEvent("Hack_Access_Granted", EventAction.PlaySound, base.gameObject);
	}

	public void CapturedIOPort()
	{
		if (m_onLossCallback != null)
		{
			m_onLossCallback();
		}
		m_hackingLossResultPanel.BringIn();
		m_hackingSystemState = HackingSystemState.Results;
		EventManager.Instance.PostEvent("Music_Game", EventAction.SetSwitch, "Level");
		EventManager.Instance.PostEvent("Hack_Fail", EventAction.PlaySound, base.gameObject);
		EventManager.Instance.PostEvent("Hack_Access_Denied", EventAction.PlaySound, base.gameObject);
		EventManager.Instance.PostEvent("Hack_Alarm_Timer", EventAction.StopSound, null);
	}

	public void TriggerAlarm()
	{
		if (m_alarmTriggered)
		{
			return;
		}
		m_alarmTriggered = true;
		m_nukeCountText.gameObject.transform.parent = m_panelManager.gameObject.transform;
		m_stopWormCountText.gameObject.transform.parent = m_panelManager.gameObject.transform;
		m_panelManager.BringIn(m_AlertPanel.index);
		EventManager.Instance.PostEvent("Music_Hacking", EventAction.SetSwitch, "Hacking_Alert");
		EventManager.Instance.PostEvent("Hack_Subroutine_Activate", EventAction.PlaySound, base.gameObject);
		List<HackingBridge> bridges = m_this.GetBridges();
		for (int i = 0; i < m_subRoutines.Count; i++)
		{
			for (int j = 0; j < m_subRoutines[i].m_connections.Count; j++)
			{
				for (int k = 0; k < bridges.Count; k++)
				{
					HackingBridge hackingBridge = bridges[k];
					if (!(hackingBridge == null))
					{
						if (m_subRoutines[i] == hackingBridge.m_connectedNode1 && m_subRoutines[i].m_connections[j] == hackingBridge.m_connectedNode2)
						{
							hackingBridge.StartTraversing(false, m_subRoutines[i].m_connections[j]);
						}
						else if (m_subRoutines[i].m_connections[j] == hackingBridge.m_connectedNode1 && m_subRoutines[i] == hackingBridge.m_connectedNode2)
						{
							hackingBridge.StartTraversing(false, m_subRoutines[i].m_connections[j]);
						}
					}
				}
			}
			List<HackingNode> previousPath = new List<HackingNode>();
			m_subRoutinesTraceTime[i] = TimeToGetToIOPort(9999f, m_subRoutines[i], previousPath);
			if (m_subRoutinesTraceTime[i] <= 10f)
			{
				EventManager.Instance.PostEvent("Hack_Alarm_Timer", EventAction.PlaySound, base.gameObject);
			}
		}
	}

	private void EnterHacking()
	{
		UIManager.instance.AddCamera(m_hackingCamera, 4096, 100f, 0);
		m_tempNukes = 0;
		SetItemCountText(m_nukeCountText, Globals.m_Inventory.m_Nukes);
		m_tempStopWorms = 0;
		SetItemCountText(m_stopWormCountText, Globals.m_Inventory.m_StopWorms);
		EventManager.Instance.PostEvent("Music_Game", EventAction.SetSwitch, "Hacking");
		EventManager.Instance.PostEvent("Music_Hacking", EventAction.SetSwitch, "Hacking_Ambient");
		EventManager.Instance.PostEvent("DynamicMixer", EventAction.AddPreset, "Hacking");
	}

	public void ExitHacking()
	{
		if (m_onExitHackingCallback != null)
		{
			m_onExitHackingCallback();
		}
		UIManager.instance.RemoveCamera(m_hackingCamera);
		EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "Hacking");
		if (m_hackingSystemState != HackingSystemState.Results)
		{
			EventManager.Instance.PostEvent("Music_Game", EventAction.SetSwitch, "Level");
		}
		EventManager.Instance.PostEvent("Hack_Alarm_Timer", EventAction.StopSound, null, base.gameObject);
		Object.Destroy(base.gameObject);
	}

	public static void AddBridge(HackingNode node1, HackingNode node2)
	{
		HackingBridge bridge = GetBridge(node1, node2);
		if ((bool)bridge)
		{
			bridge.ConstructBridge();
			return;
		}
		GameObject gameObject = new GameObject(node1.gameObject.name + "-" + node2.gameObject.name + " Bridge");
		HackingBridge hackingBridge = gameObject.AddComponent<HackingBridge>();
		HackingSystem hackingSystem = (hackingBridge.m_hackingSystem = Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem);
		hackingBridge.InitializeBridge(node1, node2);
		hackingSystem.m_nodeBridges.Add(hackingBridge);
	}

	public static void RemoveBridge(HackingNode node1, HackingNode node2)
	{
		HackingBridge bridge = GetBridge(node1, node2);
		if ((bool)bridge)
		{
			if (node1.m_connections.Contains(node2) || node2.m_connections.Contains(node1))
			{
				bridge.ConstructBridge();
				return;
			}
			HackingSystem hackingSystem = Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem;
			GameObject obj = bridge.gameObject;
			hackingSystem.m_nodeBridges.Remove(bridge);
			Object.DestroyImmediate(obj);
		}
	}

	public static HackingBridge GetBridge(HackingNode node1, HackingNode node2)
	{
		if (node1 == node2)
		{
			return null;
		}
		HackingSystem hackingSystem = ((!(m_this != null)) ? (Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem) : m_this);
		CleanupBridges();
		for (int i = 0; i < hackingSystem.m_nodeBridges.Count; i++)
		{
			if ((hackingSystem.m_nodeBridges[i].m_connectedNode1 == node1 || hackingSystem.m_nodeBridges[i].m_connectedNode2 == node1) && (hackingSystem.m_nodeBridges[i].m_connectedNode1 == node2 || hackingSystem.m_nodeBridges[i].m_connectedNode2 == node2))
			{
				return hackingSystem.m_nodeBridges[i];
			}
		}
		return null;
	}

	private static void CleanupBridges()
	{
		HackingSystem hackingSystem = ((!(m_this != null)) ? (Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem) : m_this);
		for (int i = 0; i < hackingSystem.m_nodeBridges.Count; i++)
		{
			if (hackingSystem.m_nodeBridges[i] == null)
			{
				hackingSystem.m_nodeBridges.RemoveAt(i);
				i--;
			}
		}
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
		EnterHacking();
		List<HackingNode> list = new List<HackingNode>();
		list.Add(m_startingNode);
		while (list.Count > 0)
		{
			HackingNode hackingNode = list[list.Count - 1];
			if (hackingNode.IsLocked())
			{
				list.Remove(hackingNode);
				continue;
			}
			for (int i = 0; i < hackingNode.m_connections.Count; i++)
			{
				if ((bool)hackingNode.m_connections[i] && !hackingNode.m_connections[i].IsLocked())
				{
					list.Add(hackingNode.m_connections[i]);
				}
			}
			if (hackingNode.m_node.m_type != HackingNodeType.IOPort)
			{
				hackingNode.LockNode();
			}
			if (hackingNode.m_node.m_type == HackingNodeType.Registry)
			{
				m_numberOfRegistries++;
			}
			if (hackingNode.m_node.m_type == HackingNodeType.DiagnosticSubroutine)
			{
				m_subRoutines.Add(hackingNode);
				m_subRoutinesTraceTime.Add(0f);
			}
			list.Remove(hackingNode);
		}
		for (int j = 0; j < m_startingNode.m_connections.Count; j++)
		{
			if ((bool)m_startingNode.m_connections[j])
			{
				m_startingNode.m_connections[j].UnlockNode();
			}
		}
		m_startingNode.UnlockNode();
		m_panelManager.gameObject.transform.localScale = new Vector3(m_defaultZoom, m_defaultZoom, 1f);
		m_hackingSystemState = HackingSystemState.Hacking;
		m_hackingPanel.BringIn();
		SetupEnergyMeter();
		SetupUI();
	}

	private void Update()
	{
		if (m_alarmTriggered)
		{
			float num = 9999f;
			for (int i = 0; i < m_subRoutinesTraceTime.Count; i++)
			{
				float num2 = m_subRoutinesTraceTime[i];
				List<float> subRoutinesTraceTime;
				List<float> list = (subRoutinesTraceTime = m_subRoutinesTraceTime);
				int index;
				int index2 = (index = i);
				float num3 = subRoutinesTraceTime[index];
				list[index2] = num3 - Time.deltaTime * m_subRoutineUpdateSpeed;
				m_subRoutinesTraceTime[i] = Mathf.Max(m_subRoutinesTraceTime[i], 0f);
				if (m_subRoutinesTraceTime[i] <= 10f && num2 > 10f)
				{
					EventManager.Instance.PostEvent("Hack_Alarm_Timer", EventAction.PlaySound, base.gameObject);
				}
				num = ((!(m_subRoutinesTraceTime[i] < num)) ? num : m_subRoutinesTraceTime[i]);
			}
			float num4 = Mathf.Round(num * 100f) / 100f;
			m_TraceTimerText.Text = string.Format("{0:00.00}", num4);
			Color color = Color.Lerp(m_NormalBackgroundColor, m_AlertBackgroundColor, Mathf.PingPong(Time.time, 2f) * 0.5f);
			for (int j = 0; j < 3; j++)
			{
				m_AlertBackground[j].SetColor(color);
			}
		}
		else
		{
			for (int k = 0; k < 3; k++)
			{
				m_AlertBackground[k].SetColor(m_NormalBackgroundColor);
			}
		}
		if (m_stopWormTimer > 0f)
		{
			m_stopWormTimer -= Time.deltaTime;
			if (m_stopWormTimer <= 0f)
			{
				m_subRoutineUpdateSpeed = m_subRoutineDefaultSpeed;
				EventManager.Instance.PostEvent("Hack_Subroutine_Activate", EventAction.PlaySound, base.gameObject);
			}
		}
	}

	private float TimeToGetToIOPort(float lowestTime, HackingNode currentNode, List<HackingNode> previousPath)
	{
		previousPath.Add(currentNode);
		if (currentNode.m_node.m_type != HackingNodeType.IOPort)
		{
			for (int i = 0; i < currentNode.m_connections.Count; i++)
			{
				if (!previousPath.Contains(currentNode.m_connections[i]))
				{
					float num = TimeToGetToIOPort(lowestTime, currentNode.m_connections[i], previousPath);
					lowestTime = ((!(num < lowestTime)) ? lowestTime : num);
				}
			}
		}
		else
		{
			float num2 = 0f;
			for (int j = 0; j < previousPath.Count; j++)
			{
				if (previousPath[j].m_node.m_type != HackingNodeType.DiagnosticSubroutine)
				{
					num2 += (float)previousPath[j].TimeToCapture(m_SubRoutineCaptureProgramRating);
				}
			}
			num2 += (float)(previousPath.Count - 1) * m_subRoutineBridgeTime;
			lowestTime = num2;
		}
		return lowestTime;
	}

	public void SetCurrentEnergy(float TotalEnergy, bool Depleting = false)
	{
		Color color = ((!Depleting) ? Globals.m_HUD.m_EnergyChargingColor : Globals.m_HUD.m_EnergyEmptyColor);
		m_CurrentChargeTimeOffset += Time.deltaTime;
		if (m_CurrentChargeTimeOffset >= m_ChargeTimeCycleDuration)
		{
			m_CurrentChargeTimeOffset = 0f;
		}
		color = ((m_CurrentChargeTimeOffset <= m_ChargeTimeFlashDuration) ? Color.Lerp(Globals.m_HUD.m_EnergyFullColor, color, m_CurrentChargeTimeOffset / m_ChargeTimeFlashDuration) : ((!(m_CurrentChargeTimeOffset <= m_ChargeTimeFlashDuration * 2f)) ? Globals.m_HUD.m_EnergyFullColor : Color.Lerp(color, Globals.m_HUD.m_EnergyFullColor, Mathf.InverseLerp(m_ChargeTimeFlashDuration, m_ChargeTimeFlashDuration * 2f, m_CurrentChargeTimeOffset))));
		int num = (int)Globals.m_PlayerController.GetMaxEnergy();
		for (int i = 0; i < 3; i++)
		{
			if (i >= num)
			{
				m_EnergyContainers[i].Hide(true);
				m_EnergyBars[i].Hide(true);
				m_EnergyBars[i].Hide(true);
				continue;
			}
			m_EnergyContainers[i].Hide(false);
			m_EnergyBars[i].Hide(false);
			m_EnergyBars[i].Hide(false);
			if (TotalEnergy >= (float)(i + 1))
			{
				m_EnergyContainers[i].SetColor(Globals.m_HUD.m_EnergyFullColor);
				m_EnergyBars[i].SetColor(Globals.m_HUD.m_EnergyFullColor);
				m_EnergyBars[i].transform.localScale = Vector3.one;
			}
			else if (Depleting)
			{
				if (TotalEnergy > (float)i)
				{
					if (TotalEnergy - Mathf.Floor(TotalEnergy) < 0.5f)
					{
						m_EnergyContainers[i].SetColor(color);
						m_EnergyBars[i].SetColor(color);
					}
					else
					{
						m_EnergyContainers[i].SetColor(Globals.m_HUD.m_EnergyFullColor);
						m_EnergyBars[i].SetColor(Globals.m_HUD.m_EnergyFullColor);
					}
					m_EnergyBars[i].transform.localScale = new Vector3(TotalEnergy - Mathf.Floor(TotalEnergy), 1f, 1f);
				}
				else
				{
					m_EnergyContainers[i].SetColor(Globals.m_HUD.m_EnergyEmptyColor);
					m_EnergyBars[i].SetColor(Globals.m_HUD.m_EnergyEmptyColor);
					m_EnergyBars[i].transform.localScale = new Vector3(0f, 1f, 1f);
				}
			}
			else if (TotalEnergy > (float)i)
			{
				m_EnergyContainers[i].SetColor(color);
				m_EnergyBars[i].SetColor(color);
				m_EnergyBars[i].transform.localScale = new Vector3(TotalEnergy - Mathf.Floor(TotalEnergy), 1f, 1f);
			}
			else
			{
				m_EnergyContainers[i].SetColor(Globals.m_HUD.m_EnergyEmptyColor);
				m_EnergyBars[i].SetColor(Globals.m_HUD.m_EnergyEmptyColor);
				m_EnergyBars[i].transform.localScale = new Vector3(0f, 1f, 1f);
			}
		}
	}

	private void SetupEnergyMeter()
	{
		GameObject gameObject = Object.Instantiate(Globals.m_HackingGlobals.m_EnergyMeter) as GameObject;
		Transform transform = gameObject.transform;
		Vector3 position = transform.position;
		transform.parent = m_hackingPanel.gameObject.transform;
		transform.localPosition = position;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		PackedSprite[] componentsInChildren = gameObject.GetComponentsInChildren<PackedSprite>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.name == "Sprite_EnergyContainer1")
			{
				m_EnergyContainers[0] = componentsInChildren[i];
			}
			else if (componentsInChildren[i].gameObject.name == "Sprite_EnergyContainer2")
			{
				m_EnergyContainers[1] = componentsInChildren[i];
			}
			else if (componentsInChildren[i].gameObject.name == "Sprite_EnergyContainer3")
			{
				m_EnergyContainers[2] = componentsInChildren[i];
			}
			else if (componentsInChildren[i].gameObject.name == "Sprite_EnergyBar1")
			{
				m_EnergyBars[0] = componentsInChildren[i];
			}
			else if (componentsInChildren[i].gameObject.name == "Sprite_EnergyBar2")
			{
				m_EnergyBars[1] = componentsInChildren[i];
			}
			else if (componentsInChildren[i].gameObject.name == "Sprite_EnergyBar3")
			{
				m_EnergyBars[2] = componentsInChildren[i];
			}
		}
		SetCurrentEnergy(Globals.m_PlayerController.GetCurrentEnergy());
	}

	private void SetupUI()
	{
		GameObject gameObject = Object.Instantiate(Globals.m_HackingGlobals.m_HackingUI) as GameObject;
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Transform child = gameObject.transform.GetChild(i);
			Vector3 position = child.position;
			Vector3 localScale = child.localScale;
			if (child.name == "Hacking_AlertFrame")
			{
				child.parent = m_AlertPanel.transform;
				child.localPosition = position;
				child.localRotation = Quaternion.identity;
				child.localScale = localScale;
				i--;
			}
			else if (child.name == "Hacking_AlertBackground")
			{
				for (int j = 0; j < 3; j++)
				{
					m_AlertBackground[j] = child.GetChild(j).gameObject.GetComponent<SimpleSprite>();
					m_AlertBackground[j].SetColor(m_NormalBackgroundColor);
				}
				child.parent = m_hackingPanel.transform;
				child.localPosition = position;
				child.localRotation = Quaternion.identity;
				child.localScale = Vector3.one;
				i--;
			}
			else if (child.name == "Hacking_Background")
			{
				child.parent = m_hackingPanel.transform;
				child.localPosition = position;
				child.localRotation = Quaternion.identity;
				child.localScale = Vector3.one;
				i--;
			}
			else if (child.name == "Auto Hack Button")
			{
				UIButton component = child.gameObject.GetComponent<UIButton>();
				component.scriptWithMethodToInvoke = this;
				m_autoHackButton = component;
				child.parent = m_hackingPanel.transform;
				child.localPosition = position;
				child.localRotation = Quaternion.identity;
				child.localScale = localScale;
				m_AutoHackCount = child.gameObject.GetComponentInChildren<SpriteText>();
				m_AutoHackCount.Text = "-" + Globals.m_Inventory.m_AutoHacks + "-";
				if (Globals.m_Inventory.m_AutoHacks == 0)
				{
					m_autoHackButton.SetColor(Color.gray);
					m_AutoHackCount.SetColor(Color.gray);
					m_autoHackButton.gameObject.collider.enabled = false;
					m_beganHacking = true;
				}
				i--;
			}
			else if (child.name == "Back Button")
			{
				child.gameObject.GetComponent<UIButton>().scriptWithMethodToInvoke = this;
				child.parent = m_hackingPanel.transform;
				child.localPosition = position;
				child.localRotation = Quaternion.identity;
				child.localScale = localScale;
				i--;
			}
		}
		Object.Destroy(gameObject);
	}
}
