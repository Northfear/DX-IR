using UnityEngine;

public class DominationControlPanel : MonoBehaviour
{
	public UIPanel m_DominationPanel;

	public Camera m_DominationCamera;

	public ObjectControls m_Turrets;

	public ObjectControls m_Robots;

	public ObjectControls m_Doors;

	public PackedSprite m_MonitorLight;

	public SimpleSprite m_MonitorDisplay;

	public CameraThumbnailFrame[] m_CameraThumbnails = new CameraThumbnailFrame[4];

	private InteractiveObject_Domination m_InteractiveObject;

	private float m_CurrentDelay;

	public void Thumbnail1Pressed()
	{
		ThumbnailPressed(0);
	}

	public void Thumbnail2Pressed()
	{
		ThumbnailPressed(1);
	}

	public void Thumbnail3Pressed()
	{
		ThumbnailPressed(2);
	}

	public void Thumbnail4Pressed()
	{
		ThumbnailPressed(3);
	}

	public void TurretDefaultPressed()
	{
		TurretButtonPressed(ObjectControls.DominationState.Default);
	}

	public void TurretDisabledPressed()
	{
		TurretButtonPressed(ObjectControls.DominationState.Disabled);
	}

	public void TurretEnemyPressed()
	{
		TurretButtonPressed(ObjectControls.DominationState.Enemy);
	}

	public void RobotDefaultPressed()
	{
		RobotButtonPressed(ObjectControls.DominationState.Default);
	}

	public void RobotDisabledPressed()
	{
		RobotButtonPressed(ObjectControls.DominationState.Disabled);
	}

	public void RobotEnemyPressed()
	{
		RobotButtonPressed(ObjectControls.DominationState.Enemy);
	}

	public void ToggleCameraDisconnect()
	{
		if (m_InteractiveObject.m_SelectedCameraIdx == -1)
		{
			return;
		}
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		if (augmentation_HackingCapture.IsCameraDominationActive())
		{
			SecurityCamera securityCamera = m_InteractiveObject.m_ConnectedCameras[m_InteractiveObject.m_SelectedCameraIdx];
			if (securityCamera.GetCameraState() == SecurityCamera.CameraState.Deactivated)
			{
				securityCamera.Dominate(MachineAwareness.Hostile);
				m_MonitorLight.PlayAnim("Active");
			}
			else
			{
				securityCamera.Dominate(MachineAwareness.Deactivated);
				m_MonitorLight.PlayAnim("Inactive");
			}
		}
	}

	public void RegisterInteractiveObject(InteractiveObject_Domination interactiveObject)
	{
		m_InteractiveObject = interactiveObject;
		Setup();
	}

	public void Disconnect()
	{
		m_DominationPanel.AddTempTransitionDelegate(CleanupControlPanel);
		m_DominationPanel.Dismiss();
		SoundManager.TriggerEvent("Play_HUB_Exit", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		Globals.m_PlayerController.m_DisableController = false;
	}

	private void Setup()
	{
		Turret turret = null;
		for (int i = 0; i < m_InteractiveObject.m_ConnectedTurrets.Count; i++)
		{
			if (!(m_InteractiveObject.m_ConnectedTurrets[i] == null) && turret == null)
			{
				turret = m_InteractiveObject.m_ConnectedTurrets[i];
				break;
			}
		}
		if (turret == null)
		{
			m_Turrets.NoConnections();
		}
		else if (turret.GetMachineAwareness() == MachineAwareness.Hostile)
		{
			m_Turrets.ChangeState(ObjectControls.DominationState.Default);
		}
		else if (turret.GetMachineAwareness() == MachineAwareness.Deactivated)
		{
			m_Turrets.ChangeState(ObjectControls.DominationState.Disabled);
		}
		else
		{
			m_Turrets.ChangeState(ObjectControls.DominationState.Enemy);
		}
		Sentry sentry = null;
		for (int j = 0; j < m_InteractiveObject.m_ConnectedSentries.Count; j++)
		{
			if (!(m_InteractiveObject.m_ConnectedSentries[j] == null) && sentry == null)
			{
				sentry = m_InteractiveObject.m_ConnectedSentries[j];
				break;
			}
		}
		if (sentry == null)
		{
			m_Robots.NoConnections();
		}
		else if (sentry.GetMachineAwareness() == MachineAwareness.Hostile)
		{
			m_Robots.ChangeState(ObjectControls.DominationState.Default);
		}
		else if (sentry.GetMachineAwareness() == MachineAwareness.Deactivated)
		{
			m_Robots.ChangeState(ObjectControls.DominationState.Disabled);
		}
		else
		{
			m_Robots.ChangeState(ObjectControls.DominationState.Enemy);
		}
		m_Doors.NoConnections();
		for (int k = 0; k < m_InteractiveObject.m_ConnectedCameras.Length; k++)
		{
			if (m_InteractiveObject.m_ConnectedCameras[k] != null)
			{
				if (m_InteractiveObject.m_ConnectedCameras[k].GetCameraState() == SecurityCamera.CameraState.Destroyed)
				{
					m_CameraThumbnails[k].LostConnection();
					continue;
				}
				m_CameraThumbnails[k].HasConnection();
				Camera camera = m_InteractiveObject.m_ConnectedCameras[k].m_Camera;
				camera.enabled = true;
				m_CameraThumbnails[k].m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
				m_CameraThumbnails[k].m_Texture = m_CameraThumbnails[k].m_RenderTexture;
				m_CameraThumbnails[k].m_Display.renderer.material.SetTexture("_MainTex", m_CameraThumbnails[k].m_Texture);
				camera.targetTexture = m_CameraThumbnails[k].m_RenderTexture;
			}
			else
			{
				m_CameraThumbnails[k].NoConnection();
			}
		}
		ThumbnailPressed(m_InteractiveObject.m_SelectedCameraIdx);
		UIManager.instance.AddCamera(m_DominationCamera, 4096, 100f, 0);
		Globals.m_PlayerController.m_DisableController = true;
	}

	private void CleanupControlPanel(UIPanelBase panel, EZTransition transition)
	{
		Globals.m_HUD.Display(true, true, false);
		Globals.m_HUD.EnablePassThruInput(true);
		Globals.m_PlayerController.ToggleWeaponHolstered();
		UIManager.instance.RemoveCamera(m_DominationCamera);
		Object.Destroy(base.gameObject);
	}

	private void ThumbnailPressed(int idx)
	{
		if (idx < 0 || idx >= 4)
		{
			return;
		}
		if (m_InteractiveObject.m_ConnectedCameras.Length <= idx)
		{
			return;
		}
		if (m_InteractiveObject.m_ConnectedCameras[idx] == null || m_InteractiveObject.m_ConnectedCameras[idx].GetCameraState() == SecurityCamera.CameraState.Destroyed)
		{
			m_MonitorDisplay.enabled = false;
			return;
		}
		m_MonitorDisplay.enabled = true;
		m_MonitorDisplay.SetColor(Color.white);
		m_InteractiveObject.m_ConnectedCameras[idx].m_Camera.targetTexture = m_CameraThumbnails[idx].m_RenderTexture;
		m_MonitorDisplay.renderer.material.SetTexture("_MainTex", m_CameraThumbnails[idx].m_Texture);
		m_InteractiveObject.m_SelectedCameraIdx = idx;
		if (m_InteractiveObject.m_ConnectedCameras[idx].GetCameraState() == SecurityCamera.CameraState.Deactivated)
		{
			m_MonitorLight.PlayAnim("Inactive");
			SoundManager.TriggerEvent("Play_HUB_Select");
		}
		else
		{
			m_MonitorLight.PlayAnim("Active");
			SoundManager.TriggerEvent("Play_HUB_Select");
		}
	}

	private void TurretButtonPressed(ObjectControls.DominationState state)
	{
		if (m_InteractiveObject.m_ConnectedTurrets.Count == 0)
		{
			return;
		}
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		if (!augmentation_HackingCapture.IsTurretDominationActive())
		{
			return;
		}
		m_Turrets.ChangeState(state);
		for (int i = 0; i < m_InteractiveObject.m_ConnectedTurrets.Count; i++)
		{
			if (m_InteractiveObject.m_ConnectedTurrets != null)
			{
				switch (state)
				{
				case ObjectControls.DominationState.Default:
					m_InteractiveObject.m_ConnectedTurrets[i].Dominate(MachineAwareness.Hostile);
					SoundManager.TriggerEvent("Play_HUB_On");
					break;
				case ObjectControls.DominationState.Disabled:
					m_InteractiveObject.m_ConnectedTurrets[i].Dominate(MachineAwareness.Deactivated);
					SoundManager.TriggerEvent("Play_HUB_Off");
					break;
				default:
					m_InteractiveObject.m_ConnectedTurrets[i].Dominate(MachineAwareness.Friendly);
					SoundManager.TriggerEvent("Play_HUB_On");
					break;
				}
			}
		}
	}

	private void RobotButtonPressed(ObjectControls.DominationState state)
	{
		if (m_InteractiveObject.m_ConnectedSentries.Count == 0)
		{
			return;
		}
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		if (!augmentation_HackingCapture.IsRobotDominationActive())
		{
			return;
		}
		m_Robots.ChangeState(state);
		for (int i = 0; i < m_InteractiveObject.m_ConnectedSentries.Count; i++)
		{
			switch (state)
			{
			case ObjectControls.DominationState.Default:
				m_InteractiveObject.m_ConnectedSentries[i].Dominate(MachineAwareness.Hostile);
				SoundManager.TriggerEvent("Play_HUB_On");
				break;
			case ObjectControls.DominationState.Disabled:
				m_InteractiveObject.m_ConnectedSentries[i].Dominate(MachineAwareness.Deactivated);
				SoundManager.TriggerEvent("Play_HUB_Off");
				break;
			default:
				m_InteractiveObject.m_ConnectedSentries[i].Dominate(MachineAwareness.Friendly);
				SoundManager.TriggerEvent("Play_HUB_On");
				break;
			}
		}
	}

	private void Update()
	{
		if (m_CurrentDelay < 1f)
		{
			m_CurrentDelay += Time.deltaTime;
			if (m_CurrentDelay >= 1f)
			{
				m_DominationPanel.BringIn();
				SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
			}
		}
	}
}
