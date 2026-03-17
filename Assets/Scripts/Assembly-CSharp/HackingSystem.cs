using System.Collections.Generic;
using UnityEngine;

public class HackingSystem : MonoBehaviour
{
	public delegate void CallbackDelegate();

	private static float m_subRoutineDefaultSpeed = 1f;

	private static float m_subRoutineStopWormSpeed = 0f;

	private static Vector3 m_DefaultScale = new Vector3(0.75f, 0.75f, 1f);

	public static HackingSystem m_this = null;

	public Camera m_hackingCamera;

	public UIPanelManager m_panelManager;

	public UIPanel m_hackingPanel;

	public UIPanel m_hackingNodeOptionsPanel;

	public BridgeSpriteInfo m_bridgeLine = new BridgeSpriteInfo();

	public BridgeSpriteInfo m_bridgeDottedLine = new BridgeSpriteInfo();

	public List<HackingBridge> m_nodeBridges = new List<HackingBridge>();

	private List<HackingNode> m_subRoutines = new List<HackingNode>();

	private List<float> m_subRoutinesTraceTime = new List<float>();

	public float m_defaultZoom = 1f;

	public float m_playerBridgeTime = 1f;

	public float m_subRoutineBridgeTime = 1f;

	public float m_timeToShortCircuit = 1f;

	public int m_shortCircuitChangeRatingTo;

	public float m_ShortCircuitingDrainRate = 2f;

	public Vector3 m_StartingCameraOffset = Vector3.zero;

	[HideInInspector]
	public List<HackingNode> m_HackingNodes;

	public Vector2 m_ScrollExtents = new Vector2(200f, 200f);

	private InteractiveObject_HackingTerminal m_HackingTerminal;

	private Hacking_UI m_HackingUI;

	private HackingNode m_currentlySelectedNode;

	private HackingNode m_startingNode;

	private int m_numberOfRegistries;

	private int m_numberOfCapturedRegistries;

	private int m_tempNukes;

	private int m_tempStopWorms;

	private float m_subRoutineUpdateSpeed = 1f;

	private float m_stopWormTimer;

	private HackingSystemState m_hackingSystemState;

	private bool m_alarmTriggered;

	private bool m_giveAllRewards;

	private bool m_beganHacking;

	private int m_SubRoutineCaptureProgramRating = 1;

	private bool m_EnterHacking = true;

	private List<HackingNode> m_ShortestPath = new List<HackingNode>();

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

	public void SetSubRoutineCaptureRating(int rating)
	{
		m_SubRoutineCaptureProgramRating = rating;
	}

	public int GetSubRoutineCaptureRating()
	{
		return m_SubRoutineCaptureProgramRating;
	}

	public float GetTimeToShortCircuit()
	{
		return m_timeToShortCircuit;
	}

	public int GetShortCircuitChangeRatingTo()
	{
		return m_shortCircuitChangeRatingTo;
	}

	public void RegisterHackingTerminal(InteractiveObject_HackingTerminal hackingTerminal)
	{
		m_HackingTerminal = hackingTerminal;
	}

	public InteractiveObject_HackingTerminal GetHackingTerminal()
	{
		return m_HackingTerminal;
	}

	public Hacking_UI GetHackingUI()
	{
		return m_HackingUI;
	}

	public int GetPlayerCaptureProgramRating()
	{
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		return augmentation_HackingCapture.GetCaptureProgramRating();
	}

	public int GetPlayerStealthProgramRating()
	{
		Augmentation_HackingStealth augmentation_HackingStealth = (Augmentation_HackingStealth)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingStealth);
		return augmentation_HackingStealth.GetStealthProgramRating();
	}

	public void UseNuke()
	{
		Globals.m_Inventory.AdjustItemQuantity(3, 6, -1);
		m_HackingUI.SetNukeCount();
	}

	public void UseStopWorm()
	{
		Globals.m_Inventory.AdjustItemQuantity(3, 7, -1);
		m_HackingUI.SetStopWormCount();
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
			Globals.m_Inventory.AdjustItemQuantity(3, 5, -1);
			m_HackingUI.SetAutoHackCount();
			m_numberOfCapturedRegistries = m_numberOfRegistries;
			CapturedRegistry();
		}
	}

	public void BeganHacking()
	{
		if (!m_beganHacking)
		{
			m_HackingUI.SetAutoHackCount();
			m_HackingUI.DisableAutoHack();
		}
		m_beganHacking = true;
	}

	public void OpenNodeOptionsPanel(HackingNode node)
	{
		if (node.GetNodeState() != HackingNodeState.Inaccessible && m_hackingSystemState == HackingSystemState.Hacking)
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
		if (m_currentlySelectedNode != null)
		{
			HackingNodeOptions.m_this.Dismiss();
		}
		m_currentlySelectedNode = null;
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
			Globals.m_Inventory.AdjustItemQuantity(3, 6, m_tempNukes);
			m_subRoutineUpdateSpeed = 0f;
			Hacking_VictoryResults.m_this.SetupResults();
			m_HackingUI.BringInVictoryResults();
			m_hackingSystemState = HackingSystemState.Results;
		}
		SoundManager.PlayLevelMusic();
		SoundManager.TriggerEvent("Play_Hack_Success", base.gameObject);
	}

	public void CaptureSubRoutine()
	{
		if (m_onVictoryCallback != null)
		{
			m_onVictoryCallback();
		}
		Globals.m_Inventory.AdjustItemQuantity(3, 6, m_tempNukes);
		m_subRoutineUpdateSpeed = 0f;
		m_giveAllRewards = true;
		Hacking_VictoryResults.m_this.SetupResults();
		m_HackingUI.BringInVictoryResults();
		m_hackingSystemState = HackingSystemState.Results;
		SoundManager.TriggerEvent("Play_Hack_Success", base.gameObject);
	}

	public void CapturedIOPort()
	{
		if (m_onLossCallback != null)
		{
			m_onLossCallback();
		}
		m_HackingUI.BringInLossResults();
		m_hackingSystemState = HackingSystemState.Results;
		SoundManager.PlayLevelMusic();
		SoundManager.TriggerEvent("Play_Hack_Fail", base.gameObject);
	}

	public void TriggerAlarm()
	{
		if (m_alarmTriggered)
		{
			return;
		}
		m_alarmTriggered = true;
		m_HackingUI.m_AlertPanel.BringIn();
		SoundManager.SetSoundSwitch("Music_Hacking", "Alert", Globals.m_This.gameObject);
		SoundManager.TriggerEvent("Play_Hack_Subroutine_Activate", base.gameObject);
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
							hackingBridge.StartTraversing(false, m_subRoutines[i].m_connections[j], false);
						}
						else if (m_subRoutines[i].m_connections[j] == hackingBridge.m_connectedNode1 && m_subRoutines[i] == hackingBridge.m_connectedNode2)
						{
							hackingBridge.StartTraversing(false, m_subRoutines[i].m_connections[j], false);
						}
					}
				}
			}
		}
		GetTraceTimes(true);
	}

	public void ActivateSpam()
	{
		SoundManager.TriggerEvent("Play_Hack_Spam", base.gameObject);
		int subRoutineCaptureProgramRating = m_SubRoutineCaptureProgramRating;
		m_SubRoutineCaptureProgramRating = Mathf.Max(m_SubRoutineCaptureProgramRating - 1, 1);
		for (int i = 0; i < m_subRoutines.Count; i++)
		{
			((HackingNode_DiagnosticSubroutine)m_subRoutines[i].m_node).m_rating = m_SubRoutineCaptureProgramRating;
			((HackingNode_DiagnosticSubroutine)m_subRoutines[i].m_node).ActivateSpam();
			m_subRoutines[i].m_ratingSprite.Text = m_SubRoutineCaptureProgramRating.ToString();
		}
		for (int j = 0; j < m_HackingNodes.Count; j++)
		{
			if (m_HackingNodes[j].IsBeingCapturedBySubRoutine())
			{
				m_HackingNodes[j].UpdateSubRoutineCaptureRating(subRoutineCaptureProgramRating);
			}
		}
		GetTraceTimes(false);
	}

	public void ActivateClearance()
	{
		SoundManager.TriggerEvent("Play_Hack_Clearance", base.gameObject);
		for (int i = 0; i < m_HackingNodes.Count; i++)
		{
			if (m_HackingNodes[i].m_node.m_type == HackingNodeType.DataStore)
			{
				int rating = (m_HackingNodes[i].m_node.m_rating = Mathf.Max(m_HackingNodes[i].m_node.m_rating - 1, 1));
				m_HackingNodes[i].m_node.m_rating = rating;
				m_HackingNodes[i].m_ratingSprite.Text = rating.ToString();
			}
		}
	}

	public void EnterHacking()
	{
		for (int i = 0; i < m_HackingNodes.Count; i++)
		{
			if (m_HackingNodes[i].m_node.m_type != HackingNodeType.IOPort)
			{
				m_HackingNodes[i].LockNode();
			}
			else
			{
				m_startingNode = m_HackingNodes[i];
			}
			if (m_HackingNodes[i].m_node.m_type == HackingNodeType.Registry)
			{
				m_numberOfRegistries++;
			}
			if (m_HackingNodes[i].m_node.m_type == HackingNodeType.DiagnosticSubroutine)
			{
				SetSubRoutineCaptureRating(m_HackingNodes[i].m_node.m_rating);
				m_subRoutines.Add(m_HackingNodes[i]);
				m_subRoutinesTraceTime.Add(0f);
			}
		}
		for (int j = 0; j < m_startingNode.m_connections.Count; j++)
		{
			if ((bool)m_startingNode.m_connections[j])
			{
				m_startingNode.m_connections[j].UnlockNode();
			}
		}
		m_startingNode.UnlockNode();
		m_HackingUI.SetupSecurityRating();
		m_HackingUI.SetupAttemptsLeft();
	}

	public void ExitHacking()
	{
		if (!IsAlarmTriggered())
		{
			GetHackingTerminal().SetAttemptsLeft(GetHackingTerminal().GetAttemptsLeft() + 1);
		}
		if (m_onExitHackingCallback != null)
		{
			m_onExitHackingCallback();
		}
		m_HackingUI.CleanUpUI();
		SoundManager.TriggerEvent("State_Hacking_Exit");
		if (m_hackingSystemState != HackingSystemState.Results)
		{
			SoundManager.PlayLevelMusic();
		}
		Object.Destroy(m_HackingUI.gameObject);
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

	public static void ReSetupDirectionalArrows()
	{
		HackingSystem hackingSystem = ((!(m_this != null)) ? (Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem) : m_this);
		for (int i = 0; i < hackingSystem.m_nodeBridges.Count; i++)
		{
			hackingSystem.m_nodeBridges[i].ReSetupDirectionalArrow();
		}
	}

	public static void ReSetupBridges()
	{
		HackingSystem hackingSystem = ((!(m_this != null)) ? (Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem) : m_this);
		for (int i = 0; i < hackingSystem.m_nodeBridges.Count; i++)
		{
			hackingSystem.m_nodeBridges[i].ReSetupBridge();
		}
	}

	public static void FixUpHackingSchematic()
	{
		HackingSystem hackingSystem = Object.FindObjectOfType(typeof(HackingSystem)) as HackingSystem;
		HackingNode[] componentsInChildren = hackingSystem.GetComponentsInChildren<HackingNode>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Vector3 localPosition = componentsInChildren[i].gameObject.transform.localPosition;
			localPosition.z = 1.75f;
			componentsInChildren[i].gameObject.transform.localPosition = localPosition;
			UIButton component = componentsInChildren[i].GetComponent<UIButton>();
			component.whenToInvoke = POINTER_INFO.INPUT_EVENT.PRESS;
		}
		GameObject gameObject = GameObject.Find("Bridges");
		for (int j = 0; j < hackingSystem.m_nodeBridges.Count; j++)
		{
			hackingSystem.m_nodeBridges[j].transform.parent = gameObject.transform;
			Vector3 localPosition2 = hackingSystem.m_nodeBridges[j].transform.localPosition;
			localPosition2.z = 7f;
			hackingSystem.m_nodeBridges[j].transform.localPosition = localPosition2;
			if ((bool)hackingSystem.m_nodeBridges[j].m_directionalArrow)
			{
				Vector3 localPosition3 = hackingSystem.m_nodeBridges[j].m_directionalArrow.transform.localPosition;
				localPosition3.z = 0f;
				hackingSystem.m_nodeBridges[j].m_directionalArrow.transform.localPosition = localPosition3;
			}
		}
		Hacking_Circuit[] componentsInChildren2 = hackingSystem.GetComponentsInChildren<Hacking_Circuit>();
		if (componentsInChildren2.Length > 0)
		{
			Vector3 localPosition4 = componentsInChildren2[0].gameObject.transform.parent.gameObject.transform.localPosition;
			componentsInChildren2[0].gameObject.transform.parent.gameObject.transform.localPosition = Vector3.zero;
			for (int k = 0; k < componentsInChildren2.Length; k++)
			{
				Vector3 localPosition5 = componentsInChildren2[k].gameObject.transform.localPosition;
				localPosition5 += localPosition4;
				localPosition5.z = 5f;
				componentsInChildren2[k].gameObject.transform.localPosition = localPosition5;
			}
		}
		hackingSystem.transform.position = new Vector3(0f, 1000f, 0f);
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
		m_panelManager.gameObject.transform.localScale = new Vector3(m_defaultZoom * m_DefaultScale.x, m_defaultZoom * m_DefaultScale.y, 1f);
		m_hackingSystemState = HackingSystemState.Hacking;
		SetupUI();
		m_tempNukes = 0;
		m_HackingUI.SetNukeCount();
		m_tempStopWorms = 0;
		m_HackingUI.SetStopWormCount();
		SoundManager.SetSoundSwitch("Music_Hacking", "Ambient", Globals.m_This.gameObject);
		SoundManager.PlayMusic("Music_Hacking");
		SoundManager.TriggerEvent("State_Hacking_Enter");
		m_HackingUI.SetupEnergyMeter();
	}

	private void Update()
	{
		if (m_EnterHacking)
		{
			EnterHacking();
			m_EnterHacking = false;
		}
		if (m_alarmTriggered)
		{
			float num = 9999f;
			for (int i = 0; i < m_subRoutinesTraceTime.Count; i++)
			{
				float num2 = m_subRoutinesTraceTime[i];
				List<float> subRoutinesTraceTime;
				List<float> list = (subRoutinesTraceTime = m_subRoutinesTraceTime);
				int index2;
				int index = (index2 = i);
				float num3 = subRoutinesTraceTime[index2];
				list[index] = num3 - Time.deltaTime * m_subRoutineUpdateSpeed;
				m_subRoutinesTraceTime[i] = Mathf.Max(m_subRoutinesTraceTime[i], 0f);
				if (m_subRoutinesTraceTime[i] <= 10f && num2 > 10f)
				{
					SoundManager.TriggerEvent("Play_Hack_Alarm_Timer", base.gameObject);
				}
				num = ((!(m_subRoutinesTraceTime[i] < num)) ? num : m_subRoutinesTraceTime[i]);
			}
			float time = Mathf.Round(num * 100f) / 100f;
			m_HackingUI.UpdateTraceTimerText(time);
			m_HackingUI.UpdateBackgroundColor(true);
		}
		else
		{
			m_HackingUI.UpdateBackgroundColor(false);
		}
		if (m_stopWormTimer > 0f)
		{
			m_stopWormTimer -= Time.deltaTime;
			if (m_stopWormTimer <= 0f)
			{
				m_subRoutineUpdateSpeed = m_subRoutineDefaultSpeed;
				SoundManager.TriggerEvent("Play_Hack_Subroutine_Activate", base.gameObject);
			}
		}
	}

	private float TimeToGetToIOPort(float lowestTime, HackingNode currentNode, List<HackingNode> previousPath, ref float shortestTime)
	{
		previousPath.Add(currentNode);
		if (currentNode.m_node.m_type != HackingNodeType.IOPort)
		{
			for (int i = 0; i < currentNode.m_connections.Count; i++)
			{
				if (!previousPath.Contains(currentNode.m_connections[i]))
				{
					float num = TimeToGetToIOPort(lowestTime, currentNode.m_connections[i], previousPath, ref shortestTime);
					lowestTime = ((!(num < lowestTime)) ? lowestTime : num);
				}
			}
		}
		else
		{
			float num2 = 0f;
			int num3 = 0;
			for (int j = 0; j < previousPath.Count; j++)
			{
				if (previousPath[j].m_node.m_type == HackingNodeType.DiagnosticSubroutine || previousPath[j].IsCapturedBySubRoutine())
				{
					continue;
				}
				float subRoutineTimeLeftToCapture = previousPath[j].GetSubRoutineTimeLeftToCapture();
				if (subRoutineTimeLeftToCapture == -1f)
				{
					if (num3 == 0)
					{
						for (int k = 0; k < m_nodeBridges.Count; k++)
						{
							if (((m_nodeBridges[k].m_connectedNode1 == previousPath[j] && m_nodeBridges[k].m_connectedNode2 == previousPath[j - 1]) || (m_nodeBridges[k].m_connectedNode1 == previousPath[j - 1] && m_nodeBridges[k].m_connectedNode2 == previousPath[j])) && m_nodeBridges[k].IsSubRoutineInTransit())
							{
								num2 -= m_nodeBridges[k].m_subRoutineBridge.m_currentTransitTime;
							}
						}
					}
					num3++;
					num2 += (float)previousPath[j].TimeToCapture(m_SubRoutineCaptureProgramRating);
				}
				else
				{
					num2 += subRoutineTimeLeftToCapture;
				}
			}
			num2 += (float)num3 * m_subRoutineBridgeTime;
			lowestTime = num2;
		}
		if (lowestTime < shortestTime)
		{
			shortestTime = lowestTime;
			m_ShortestPath = previousPath;
		}
		return lowestTime;
	}

	public void GetTraceTimes(bool triggeredByAlarm)
	{
		float shortestTime = 9999f;
		for (int i = 0; i < m_subRoutines.Count; i++)
		{
			List<HackingNode> previousPath = new List<HackingNode>();
			m_subRoutinesTraceTime[i] = TimeToGetToIOPort(9999f, m_subRoutines[i], previousPath, ref shortestTime);
			if (triggeredByAlarm && m_subRoutinesTraceTime[i] <= 10f)
			{
				SoundManager.TriggerEvent("Play_Hack_Alarm_Timer", base.gameObject);
			}
		}
	}

	private void SetupUI()
	{
		GameObject gameObject = Object.Instantiate(Globals.m_HackingGlobals.m_HackingUI) as GameObject;
		m_HackingUI = gameObject.GetComponent<Hacking_UI>();
		m_HackingUI.SetupUI();
		if (Globals.m_Inventory.GetItemQuantity(3, 5) <= 0)
		{
			m_beganHacking = true;
		}
	}
}
