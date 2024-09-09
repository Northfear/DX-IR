using Fabric;
using UnityEngine;

public class InteractiveObject_HackingTerminal : InteractiveObject_Base
{
	public GameObject m_HackingSchematic;

	public GameObject m_HackingTargetObject;

	public GameObject m_hackingTerminalPrefab;

	public HackingTerminal_Info m_hackingTerminalInfo;

	public GameObject m_EmailPrefab;

	public InteractiveObject_HackingTerminal m_LinkedHackingTerminalForEmail;

	public int m_SecurityRating = 1;

	private HackingTerminal m_hackingTerminal;

	private bool m_HackWasSuccessful;

	private bool m_InAHack;

	private bool m_PasswordKnown;

	private bool m_AlarmTripped;

	private float m_AlarmTimerMax = 6f;

	private float m_AlarmTimer;

	protected override void Awake()
	{
		m_BlockCover = true;
		base.Awake();
	}

	public override bool EnableInteractiveObject()
	{
		if (!base.EnableInteractiveObject())
		{
			return false;
		}
		m_InAHack = false;
		return true;
	}

	public override bool DisableInteractiveObject()
	{
		if (!base.DisableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject()
	{
		if (!base.InteractWithObject())
		{
			return false;
		}
		if ((bool)m_hackingTerminalPrefab)
		{
			DisableUI();
			if (!m_HackWasSuccessful)
			{
				Object.Instantiate(m_hackingTerminalPrefab);
				if (HackingTerminal_NumberPad.m_this != null)
				{
					m_hackingTerminal = HackingTerminal_NumberPad.m_this;
				}
				m_hackingTerminal.RegisterHackingTerminalInfo(m_hackingTerminalInfo, m_PasswordKnown);
				m_hackingTerminal.SetCorrectPasscodeCallback(OnCorrectPasscodeCallback);
				m_hackingTerminal.SetIncorrectPasscodeCallback(OnIncorrectPasscodeCallback);
				m_hackingTerminal.SetHackingButtonPressedCallback(OnHackingButtonPressed);
				m_hackingTerminal.SetCloseButtonPressedCallback(OnCloseButtonPressed);
				m_hackingTerminal.BringInTerminal();
			}
			else
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
			return InteractiveObject_Base.m_OffScreen;
		}
		return base.GetPopupLocation();
	}

	public void LearnedPassword()
	{
		m_PasswordKnown = true;
	}

	private void Update()
	{
		if (m_AlarmTripped)
		{
			m_AlarmTimer -= Time.deltaTime;
			if (m_AlarmTimer <= 0f)
			{
				m_AlarmTripped = false;
				EventManager.Instance.PostEvent("Alarm", EventAction.StopSound, null, base.gameObject);
			}
		}
	}

	private void OnCorrectPasscodeCallback()
	{
		m_HackWasSuccessful = true;
		OnHackingExit();
	}

	private void OnIncorrectPasscodeCallback()
	{
	}

	private void OnHackingButtonPressed()
	{
		int num = 1;
		if (num >= m_SecurityRating)
		{
			m_hackingTerminal.HideTerminal();
			Object.Instantiate(m_HackingSchematic);
			HackingSystem.SetOnVictoryCallback(OnHackingVictory);
			HackingSystem.SetOnLossCallback(OnHackingLoss);
			HackingSystem.SetOnExitCallback(OnHackingExit);
		}
		else
		{
			Debug.Log("Player does not have a high enough Capture Program Rating.");
		}
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
		EventManager.Instance.PostEvent("Alarm", EventAction.PlaySound, null, base.gameObject);
		m_AlarmTripped = true;
		m_AlarmTimer = m_AlarmTimerMax;
	}

	private void OnHackingEnter()
	{
	}

	private void OnHackingExit()
	{
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
	}

	private void ActivateTargetObject()
	{
		if (m_HackingTargetObject != null)
		{
			m_HackingTargetObject.animation.Play();
			EventManager.Instance.PostEvent("Door_Metal_Slide_Open", EventAction.PlaySound, null, base.gameObject);
		}
		EventManager.Instance.PostEvent("Keypad_Deactivate", EventAction.PlaySound, null, base.gameObject);
	}

	private void DisableUI()
	{
		Globals.m_HUD.Display(false, true);
		Globals.m_HUD.EnablePassThruInput(false);
		Globals.m_PlayerController.ToggleWeaponHolstered();
		Globals.m_PlayerController.CancelMovement();
	}

	private void EnableUI()
	{
		Globals.m_HUD.Display(true, true);
		Globals.m_HUD.EnablePassThruInput(true);
		Globals.m_PlayerController.ToggleWeaponHolstered();
	}
}
