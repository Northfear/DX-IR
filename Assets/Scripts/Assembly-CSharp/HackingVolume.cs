using UnityEngine;

public class HackingVolume : MonoBehaviour
{
	public GameObject m_HackingSchematic;

	public GameObject m_HackingTargetObject;

	public GameObject m_hackingTerminalPrefab;

	public HackingTerminal_Info m_hackingTerminalInfo;

	private HackingTerminal m_hackingTerminal;

	private bool m_HackingVolumeReady = true;

	private bool m_HackWasSuccessful;

	private bool m_TerminalCompromised;

	private bool m_AlarmTripped;

	private float m_AlarmTimerMax = 6f;

	private float m_AlarmTimer;

	private void Update()
	{
		if (m_AlarmTripped)
		{
			m_AlarmTimer -= Time.deltaTime;
			if (m_AlarmTimer <= 0f)
			{
				m_AlarmTripped = false;
				SoundManager.TriggerEvent("Stop_Alarm", base.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_HackingVolumeReady || m_TerminalCompromised || !(other.gameObject.tag == "Player"))
		{
			return;
		}
		m_HackingVolumeReady = false;
		if ((bool)m_hackingTerminalPrefab)
		{
			DisableUI();
			Object.Instantiate(m_hackingTerminalPrefab);
			if (HackingTerminal_NumberPad.m_this != null)
			{
				m_hackingTerminal = HackingTerminal_NumberPad.m_this;
			}
			m_hackingTerminal.RegisterHackingTerminalInfo(m_hackingTerminalInfo, m_hackingTerminalInfo);
			m_hackingTerminal.SetCorrectPasscodeCallback(OnCorrectPasscodeCallback);
			m_hackingTerminal.SetIncorrectPasscodeCallback(OnIncorrectPasscodeCallback);
			m_hackingTerminal.SetHackingButtonPressedCallback(OnHackingButtonPressed);
			m_hackingTerminal.SetCloseButtonPressedCallback(OnCloseButtonPressed);
			m_hackingTerminal.BringInTerminal();
		}
		else
		{
			DisableUI();
			Object.Instantiate(m_HackingSchematic);
			HackingSystem.SetOnVictoryCallback(OnHackingVictory);
			HackingSystem.SetOnLossCallback(OnHackingLoss);
			HackingSystem.SetOnExitCallback(OnHackingExit);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!m_HackingVolumeReady && other.gameObject.tag == "Player")
		{
			m_HackingVolumeReady = true;
		}
	}

	private void OnCorrectPasscodeCallback()
	{
		m_hackingTerminal.CloseTerminal(true);
		EnableUI();
		m_TerminalCompromised = true;
		ActivateTargetObject();
	}

	private void OnIncorrectPasscodeCallback()
	{
	}

	private void OnHackingButtonPressed()
	{
		m_hackingTerminal.HideTerminal();
		Object.Instantiate(m_HackingSchematic);
		HackingSystem.SetOnVictoryCallback(OnHackingVictory);
		HackingSystem.SetOnLossCallback(OnHackingLoss);
		HackingSystem.SetOnExitCallback(OnHackingExit);
	}

	private void OnCloseButtonPressed()
	{
		EnableUI();
	}

	private void OnHackingVictory()
	{
		m_HackWasSuccessful = true;
		m_TerminalCompromised = true;
	}

	private void OnHackingLoss()
	{
		m_HackWasSuccessful = false;
		SoundManager.TriggerEvent("Play_Alarm", base.gameObject);
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
		}
		if (m_hackingTerminal != null)
		{
			m_hackingTerminal.CloseTerminal(false);
		}
		EnableUI();
	}

	private void ActivateTargetObject()
	{
		if (m_HackingTargetObject != null)
		{
			m_HackingTargetObject.animation.Play();
			SoundManager.TriggerEvent("Play_Door_Metal_Slide_Open", base.gameObject);
		}
		SoundManager.TriggerEvent("Play_Keypad_Deactivate", base.gameObject);
	}

	private void DisableUI()
	{
		Globals.m_HUD.Display(false, true, false);
		Globals.m_HUD.EnablePassThruInput(false);
		Globals.m_PlayerController.ToggleWeaponHolstered();
		Globals.m_PlayerController.CancelMovement();
	}

	private void EnableUI()
	{
		Globals.m_HUD.Display(true, true, false);
		Globals.m_HUD.EnablePassThruInput(true);
		Globals.m_PlayerController.ToggleWeaponHolstered();
	}
}
