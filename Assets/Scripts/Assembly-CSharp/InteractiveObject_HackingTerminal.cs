using System.Xml;
using UnityEngine;

public class InteractiveObject_HackingTerminal : InteractiveObject_Base
{
	public GameObject m_HackingSchematic;

	public GameObject m_HackingTargetObject;

	public GameObject m_hackingTerminalPrefab;

	public HackingTerminal_Info m_hackingTerminalInfo;

	public GameObject m_EmailPrefab;

	public InteractiveObject_HackingTerminal m_LinkedHackingTerminalForEmail;

	public int m_AttemptsLeft = 5;

	public float m_LockoutTime = 30f;

	private HackingTerminal m_hackingTerminal;

	private bool m_HackWasSuccessful;

	private bool m_InAHack;

	private bool m_PasswordKnown;

	private bool m_IsHolstered;

	private float m_AlarmTimer;

	private int m_CurrentAttemptsLeft;

	public int GetAttemptsLeft()
	{
		return m_CurrentAttemptsLeft;
	}

	public void SetAttemptsLeft(int attemptsLeft)
	{
		m_CurrentAttemptsLeft = attemptsLeft;
	}

	public float GetAlarmTimeLeft()
	{
		return m_AlarmTimer;
	}

	protected override void Awake()
	{
		m_BlockCover = true;
		m_CurrentAttemptsLeft = m_AttemptsLeft;
		m_Save = true;
		base.Awake();
	}

	public override bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.EnableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		m_InAHack = false;
		return true;
	}

	public override bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.DisableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
		{
			return false;
		}

		if ((bool)m_hackingTerminalPrefab)
		{
			DisableUI();

			if (!m_HackWasSuccessful)
			{
				Globals.m_HackingGlobals.m_ActiveTerminal = this;
				Object.Instantiate(m_hackingTerminalPrefab);

				if (HackingTerminal_NumberPad.m_this != null)
				{
					m_hackingTerminal = HackingTerminal_NumberPad.m_this;
				}

				m_hackingTerminal.RegisterHackingTerminal(this);
				m_hackingTerminal.RegisterHackingTerminalInfo(m_hackingTerminalInfo, m_PasswordKnown);
				m_hackingTerminal.SetCorrectPasscodeCallback(OnCorrectPasscodeCallback);
				m_hackingTerminal.SetIncorrectPasscodeCallback(OnIncorrectPasscodeCallback);
				m_hackingTerminal.SetHackingButtonPressedCallback(OnHackingButtonPressed);
				m_hackingTerminal.SetCloseButtonPressedCallback(OnCloseButtonPressed);
				m_hackingTerminal.BringInTerminal();
			}
			else if ((bool)m_EmailPrefab)
			{
				Object.Instantiate(m_EmailPrefab);
			}
		}
		else
		{
			DisableUI();
			Object.Instantiate(m_HackingSchematic);
			HackingSystem.SetOnVictoryCallback(OnHackingVictory);
			HackingSystem.SetOnLossCallback(OnHackingLoss);
			HackingSystem.SetOnExitCallback(OnHackingExit);
		}

		m_InAHack = true;
		return true;
	}

	public override Vector3 GetPopupLocation()
	{
		if (m_InAHack)
		{
			return m_OffScreen;
		}
		return base.GetPopupLocation();
	}

	public void LearnedPassword()
	{
		m_PasswordKnown = true;
	}

	private void Update()
	{
		if (m_CurrentAttemptsLeft <= 0)
		{
			m_AlarmTimer -= Time.deltaTime;
			if (m_AlarmTimer <= 0f)
			{
				m_CurrentAttemptsLeft = m_AttemptsLeft;
				SoundManager.TriggerEvent("Stop_Alarm", base.gameObject);
				m_hackingTerminal.EnableLockout(false);
			}
		}
	}

	private void OnCorrectPasscodeCallback()
	{
		m_HackWasSuccessful = true;
		if (m_EmailPrefab == null)
		{
			m_Active = false;
		}
		OnHackingExit();
	}

	private void OnIncorrectPasscodeCallback()
	{
	}

	private void OnHackingButtonPressed()
	{
		m_hackingTerminal.HideTerminal();
		Object.Instantiate(m_HackingSchematic);
		HackingSystem.m_this.RegisterHackingTerminal(this);
		HackingSystem.SetOnVictoryCallback(OnHackingVictory);
		HackingSystem.SetOnLossCallback(OnHackingLoss);
		HackingSystem.SetOnExitCallback(OnHackingExit);
	}

	private void OnCloseButtonPressed()
	{
		EnableUI();
		m_InAHack = false;
	}

	private void OnHackingVictory()
	{
		m_HackWasSuccessful = true;
		if (m_EmailPrefab == null)
		{
			m_Active = false;
		}
	}

	private void OnHackingLoss()
	{
		m_HackWasSuccessful = false;
		LockoutTerminal();
	}

	private void OnHackingEnter()
	{
	}

	private void OnHackingExit()
	{
		if (!m_HackWasSuccessful)
		{
			m_CurrentAttemptsLeft--;
		}

		if (m_CurrentAttemptsLeft <= 0)
		{
			LockoutTerminal();
		}

		if (m_HackWasSuccessful)
		{
			ActivateTargetObject();

			if (m_hackingTerminal != null)
			{
				m_hackingTerminal.CloseTerminal(false);
			}

			if (m_EmailPrefab == null)
			{
				EnableUI();
				m_Active = false;
			}
			else
			{
				Object.Instantiate(m_EmailPrefab);
				if ((bool)m_LinkedHackingTerminalForEmail)
				{
					m_LinkedHackingTerminalForEmail.LearnedPassword();
				}
			}

			m_InAHack = false;
		}
		else
		{
			m_hackingTerminal.BringInTerminal();
		}

		Globals.m_HackingGlobals.m_ActiveTerminal = null;
	}

	public void LockoutTerminal()
	{
		SoundManager.TriggerEvent("Play_Alarm", base.gameObject);
		m_CurrentAttemptsLeft = 0;
		m_AlarmTimer = m_LockoutTime;
	}

	private void ActivateTargetObject()
	{
		if (m_HackingTargetObject != null)
		{
			InteractiveObject_Doors component = m_HackingTargetObject.GetComponent<InteractiveObject_Doors>();
			if (component != null)
			{
				component.m_LockedFromFront = false;
				component.m_LockedFromBack = false;
				component.InteractWithDoor(true, false);
				SoundManager.TriggerEvent("Play_Door_Metal_Slide_Open", base.gameObject);
				Debug.Log("Hacking terminal unlocked and opened door.");
			}
			else
			{
				m_HackingTargetObject.animation.Play();
				SoundManager.TriggerEvent("Play_Door_Metal_Slide_Open", base.gameObject);
				Debug.Log("Door class not found.");
			}
		}
		SoundManager.TriggerEvent("Play_Keypad_Deactivate", base.gameObject);
	}

	private void DisableUI()
	{
		Globals.m_HUD.Display(false, true, false);
		Globals.m_HUD.EnablePassThruInput(false);
		m_IsHolstered = Globals.m_PlayerController.IsWeaponHolstered();
		if (!m_IsHolstered)
		{
			Globals.m_PlayerController.WeaponHolster(true);
		}
		Globals.m_PlayerController.CancelMovement();
		Globals.m_PlayerController.m_DisableController = true;
	}

	private void EnableUI()
	{
		Globals.m_HUD.Display(true, true, false);
		Globals.m_HUD.EnablePassThruInput(true);
		if (!m_IsHolstered)
		{
			Globals.m_PlayerController.WeaponHolster(false);
		}
		Globals.m_PlayerController.m_DisableController = false;
	}

	public override XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = base.SaveGame(root, doc);
		xmlElement.SetAttribute("PasswordKnown", m_PasswordKnown.ToString());
		xmlElement.SetAttribute("HackWasSuccessful", m_HackWasSuccessful.ToString());
		xmlElement.SetAttribute("AlarmTimer", m_AlarmTimer.ToString());
		xmlElement.SetAttribute("CurrentAttemptsLeft", m_CurrentAttemptsLeft.ToString());
		return xmlElement;
	}

	public override XmlElement LoadGame(XmlElement root)
	{
		XmlElement xmlElement = base.LoadGame(root);
		m_PasswordKnown = bool.Parse(xmlElement.GetAttribute("PasswordKnown"));
		m_HackWasSuccessful = bool.Parse(xmlElement.GetAttribute("HackWasSuccessful"));
		m_AlarmTimer = float.Parse(xmlElement.GetAttribute("AlarmTimer"));
		m_CurrentAttemptsLeft = int.Parse(xmlElement.GetAttribute("CurrentAttemptsLeft"));
		if (m_AlarmTimer > 0f && m_CurrentAttemptsLeft <= 0)
		{
			SoundManager.TriggerEvent("Play_Alarm", base.gameObject);
		}
		return xmlElement;
	}
}
