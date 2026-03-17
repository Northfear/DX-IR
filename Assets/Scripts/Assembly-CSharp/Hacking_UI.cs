using UnityEngine;

public class Hacking_UI : MonoBehaviour
{
	public Camera m_HackingUICamera;

	public UIPanel m_HackingUIPanel;

	public BTButton m_Frame_Fill;

	public BTButton m_TitleBar;

	public SpriteText m_TitleBar_Text;

	public SpriteText m_SecurityRatingText;

	public PackedSprite[] m_SecurityRatingBars = new PackedSprite[5];

	public SpriteText m_AttemptsLeftText;

	public PackedSprite[] m_AttemptsLeftBars = new PackedSprite[5];

	public UIPanel m_LevelIndicatorPanel;

	public UIPanel m_CannotHackPanel;

	public UIPanel m_AlertPanel;

	public UIPanel m_ResultVictoryPanel;

	public UIPanel m_ResultLossPanel;

	public SpriteText m_TraceTimerText;

	public SpriteText m_AutoHackCountText;

	public SpriteText m_NukeCountText;

	public SpriteText m_StopWormCountText;

	public UIButton m_AutoHackButton;

	public UIButton m_BackButton;

	public SimpleSprite m_Screen;

	public GameObject m_ScreenInput;

	private Color m_NormalBackgroundColor = new Color(0f, 0f, 0f, 0.9f);

	private Color m_AlertBackgroundColor = new Color(0.5f, 0.09f, 0.03f, 0.9f);

	private RenderTexture m_RenderTexture;

	private Ray m_Ray;

	private float m_CurrentChargeTimeOffset;

	private float m_ChargeTimeCycleDuration = 1.25f;

	private float m_ChargeTimeFlashDuration = 0.5f;

	private PackedSprite[] m_EnergyContainers = new PackedSprite[3];

	private PackedSprite[] m_EnergyBars = new PackedSprite[3];

	public void SetupUI()
	{
		m_Frame_Fill.SetColor(m_NormalBackgroundColor);
		m_AutoHackButton.scriptWithMethodToInvoke = HackingSystem.m_this;
		m_AutoHackButton.Hide(true);
		m_BackButton.scriptWithMethodToInvoke = HackingSystem.m_this;
		m_BackButton.Hide(true);
		UIManager.instance.AddCamera(m_HackingUICamera, 4096, 100f, 0);
		HackingSystem.m_this.m_hackingCamera.transform.localPosition = HackingSystem.m_this.m_StartingCameraOffset;
		m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		m_Screen.renderer.material.mainTexture = m_RenderTexture;
		HackingSystem.m_this.m_hackingCamera.targetTexture = m_RenderTexture;
		m_HackingUIPanel.BringIn();
		m_CannotHackPanel.DismissImmediate();
		m_AlertPanel.DismissImmediate();
	}

	public void CleanUpUI()
	{
		UIManager.instance.RemoveCamera(m_HackingUICamera);
	}

	public void AttachTo(Transform Obj, Transform AttachTo, Vector3 scale)
	{
		Vector3 position = Obj.position;
		Vector3 localScale = Obj.localScale;
		Obj.parent = AttachTo;
		Obj.localPosition = position;
		Obj.localRotation = Quaternion.identity;
		Obj.localScale = localScale;
	}

	public void SetAutoHackCount()
	{
		int itemQuantity = Globals.m_Inventory.GetItemQuantity(3, 5);
		m_AutoHackCountText.Text = "-" + itemQuantity + "-";
		if (itemQuantity <= 0)
		{
			DisableAutoHack();
		}
	}

	public void SetNukeCount()
	{
		int itemQuantity = Globals.m_Inventory.GetItemQuantity(3, 6);
		m_NukeCountText.Text = string.Format("{0:00}", itemQuantity);
	}

	public void SetStopWormCount()
	{
		int itemQuantity = Globals.m_Inventory.GetItemQuantity(3, 7);
		m_StopWormCountText.Text = string.Format("{0:00}", itemQuantity);
	}

	public void DisableAutoHack()
	{
		m_AutoHackButton.SetColor(Color.gray);
		m_AutoHackCountText.SetColor(Color.gray);
		m_AutoHackButton.gameObject.collider.enabled = false;
	}

	public void BringInVictoryResults()
	{
		m_ResultVictoryPanel.BringIn();
	}

	public void BringInLossResults()
	{
		m_ResultLossPanel.BringIn();
	}

	public void UpdateBackgroundColor(bool AlarmTriggered)
	{
		if (AlarmTriggered)
		{
			m_Frame_Fill.SetColor(Color.Lerp(m_NormalBackgroundColor, m_AlertBackgroundColor, Mathf.PingPong(Time.time, 2f) * 0.5f));
		}
		else
		{
			m_Frame_Fill.SetColor(m_NormalBackgroundColor);
		}
	}

	public void UpdateTraceTimerText(float time)
	{
		m_TraceTimerText.Text = string.Format("{0:00.00}", time);
	}

	public void LIHackButtonPressed()
	{
		int subRoutineCaptureRating = HackingSystem.m_this.GetSubRoutineCaptureRating();
		Augmentation_HackingCapture augmentation_HackingCapture = (Augmentation_HackingCapture)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.HackingCapture);
		if (augmentation_HackingCapture.GetCaptureProgramRating() >= subRoutineCaptureRating)
		{
			SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
			SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
			m_LevelIndicatorPanel.Dismiss();
			m_AutoHackButton.Hide(false);
			SetAutoHackCount();
			m_BackButton.Hide(false);
		}
		else
		{
			SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
			m_CannotHackPanel.BringIn();
		}
	}

	public void CHCloseButtonPressed()
	{
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		m_CannotHackPanel.Dismiss();
	}

	public void LIBackButtonPressed()
	{
		InteractiveObject_HackingTerminal hackingTerminal = HackingSystem.m_this.GetHackingTerminal();
		m_HackingUIPanel.AddTempTransitionDelegate(ExitHacking);
		m_HackingUIPanel.Dismiss();
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
	}

	public void ScreenInputPressed(ref POINTER_INFO ptr)
	{
		Collider collider = m_ScreenInput.collider;
		Vector3 vector = ptr.hitInfo.point - collider.bounds.center;
		vector.x /= collider.bounds.extents.x;
		vector.y /= collider.bounds.extents.y;
		Collider collider2 = HackingSystem.m_this.m_hackingCamera.collider;
		if (collider2 == null)
		{
			return;
		}
		Vector3 position = collider2.bounds.center + new Vector3(collider2.bounds.extents.x * vector.x, collider2.bounds.extents.y * vector.y, collider2.bounds.center.z);
		Vector3 position2 = HackingSystem.m_this.m_hackingCamera.WorldToScreenPoint(position);
		m_Ray = HackingSystem.m_this.m_hackingCamera.ScreenPointToRay(position2);
		bool flag = false;
		RaycastHit hitInfo;
		if (Physics.Raycast(m_Ray, out hitInfo))
		{
			UIButton component = hitInfo.collider.gameObject.GetComponent<UIButton>();
			if ((bool)component)
			{
				if (!HackingNodeOptions.m_this.IsActive())
				{
					flag = true;
				}
				else if (hitInfo.collider.gameObject.GetComponent<HackingNode>() == null && hitInfo.collider.gameObject.GetComponent<Hacking_Circuit>() == null)
				{
					flag = true;
				}
				if (flag)
				{
					component.OnInput(ptr);
				}
			}
		}
		if (!flag && HackingSystem.m_this.GetCurrentlySelectedNode() != null)
		{
			HackingSystem.m_this.CloseNodeOptionsPanel();
		}
	}

	public void ScreenInputReleased(ref POINTER_INFO ptr)
	{
		Collider collider = m_ScreenInput.collider;
		Vector3 vector = ptr.hitInfo.point - collider.bounds.center;
		vector.x /= collider.bounds.extents.x;
		vector.y /= collider.bounds.extents.y;
		Collider collider2 = HackingSystem.m_this.m_hackingCamera.collider;
		if (collider2 == null)
		{
			return;
		}
		Vector3 position = collider2.bounds.center + new Vector3(collider2.bounds.extents.x * vector.x, collider2.bounds.extents.y * vector.y, collider2.bounds.center.z);
		Vector3 position2 = HackingSystem.m_this.m_hackingCamera.WorldToScreenPoint(position);
		m_Ray = HackingSystem.m_this.m_hackingCamera.ScreenPointToRay(position2);
		bool flag = false;
		RaycastHit hitInfo;
		if (!Physics.Raycast(m_Ray, out hitInfo))
		{
			return;
		}
		UIButton component = hitInfo.collider.gameObject.GetComponent<UIButton>();
		if ((bool)component)
		{
			if (!HackingNodeOptions.m_this.IsActive())
			{
				flag = true;
			}
			else if (hitInfo.collider.gameObject.GetComponent<HackingNode>() == null && hitInfo.collider.gameObject.GetComponent<Hacking_Circuit>() == null)
			{
				flag = true;
			}
			if (flag)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
				component.OnInput(ptr);
			}
		}
	}

	public void ScreenInputDrag(ref POINTER_INFO ptr)
	{
		if (HackingSystem.m_this.GetCurrentlySelectedNode() != null)
		{
			HackingSystem.m_this.CloseNodeOptionsPanel();
		}
		Vector3 localPosition = HackingSystem.m_this.m_hackingCamera.transform.localPosition;
		localPosition -= ptr.inputDelta * m_Screen.worldUnitsPerScreenPixel;
		Vector3 startingCameraOffset = HackingSystem.m_this.m_StartingCameraOffset;
		Vector2 scrollExtents = HackingSystem.m_this.m_ScrollExtents;
		localPosition.x = Mathf.Min(localPosition.x, startingCameraOffset.x + scrollExtents.x);
		localPosition.x = Mathf.Max(localPosition.x, startingCameraOffset.x - scrollExtents.x);
		localPosition.y = Mathf.Min(localPosition.y, startingCameraOffset.y + scrollExtents.y);
		localPosition.y = Mathf.Max(localPosition.y, startingCameraOffset.y - scrollExtents.y);
		HackingSystem.m_this.m_hackingCamera.transform.localPosition = localPosition;
	}

	public void SetupEnergyMeter()
	{
		GameObject gameObject = Object.Instantiate(Globals.m_HackingGlobals.m_EnergyMeter) as GameObject;
		Transform transform = gameObject.transform;
		Vector3 position = transform.position;
		transform.parent = m_HackingUIPanel.gameObject.transform;
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
		SetCurrentEnergy(Globals.m_PlayerController.GetCurrentEnergy(), false);
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

	public void SetupSecurityRating()
	{
		int subRoutineCaptureRating = HackingSystem.m_this.GetSubRoutineCaptureRating();
		m_SecurityRatingText.Text = subRoutineCaptureRating.ToString();
		for (int i = 0; i < m_SecurityRatingBars.Length; i++)
		{
			if (i >= subRoutineCaptureRating)
			{
				m_SecurityRatingBars[i].SetColor(Color.gray);
			}
		}
	}

	public void SetupAttemptsLeft()
	{
		int attemptsLeft = HackingSystem.m_this.GetHackingTerminal().GetAttemptsLeft();
		m_AttemptsLeftText.Text = attemptsLeft.ToString();
		for (int i = 0; i < m_AttemptsLeftBars.Length; i++)
		{
			if (i >= attemptsLeft)
			{
				m_AttemptsLeftBars[i].SetColor(Color.gray);
			}
		}
	}

	public void ExitHacking(UIPanelBase panel, EZTransition transition)
	{
		HackingSystem.m_this.ExitHacking();
	}

	public void ExitHacking()
	{
		HackingSystem.m_this.ExitHacking();
	}
}
