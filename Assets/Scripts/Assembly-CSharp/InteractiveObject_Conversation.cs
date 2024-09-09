using Fabric;
using UnityEngine;

public class InteractiveObject_Conversation : InteractiveObject_Base
{
	public enum ConvoMusicStart
	{
		Conv_SaDu = 0,
		Conv_SaTi = 1
	}

	public enum ConvoMusicEnd
	{
		Ambient = 0,
		None = 1
	}

	public Conversation_Base m_StartingConversation;

	public FaceFXControllerScript m_FaceFXController;

	public float m_Offset = 1.25f;

	public float m_RotationalOffset = 180f;

	public Animation m_NPCAnimator;

	public string m_NPCIdleAnimName;

	public FaceFXControllerScript m_PlayerFaceFXController;

	public Animation m_PlayerAnimator;

	public string m_PlayerIdleAnimName;

	public GameObject m_PlayerDialogModel;

	public ConvoMusicStart m_ConvoMusicStart;

	public ConvoMusicEnd m_ConvoMusicEnd;

	public int m_StartingPersuasionLevel = 50;

	private bool m_HavingConversation;

	public override bool UseInteractiveCollider()
	{
		return false;
	}

	public AudioSource GetAudioSource()
	{
		return m_NPCAnimator.gameObject.GetComponent<AudioSource>();
	}

	public override bool EnableInteractiveObject()
	{
		if (!base.EnableInteractiveObject())
		{
			return false;
		}
		m_HavingConversation = false;
		Globals.m_ConversationSystem.SetPartnerFaceFXController(m_FaceFXController);
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
		if (m_HavingConversation)
		{
			return false;
		}
		m_HavingConversation = true;
		Globals.m_ConversationSystem.RegisterInteractiveObject(this);
		Globals.m_ConversationSystem.BeginConversation(m_StartingConversation);
		DisableUI();
		EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, m_ConvoMusicStart.ToString());
		Vector3 normalized = base.gameObject.transform.forward.normalized;
		Globals.m_PlayerController.gameObject.transform.position = base.gameObject.transform.position + normalized * m_Offset;
		Globals.m_CameraController.SetYaw(base.gameObject.transform.rotation.eulerAngles.y + m_RotationalOffset);
		Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
		return true;
	}

	public override Vector3 GetPopupLocation()
	{
		if (m_HavingConversation)
		{
			return InteractiveObject_Base.m_OffScreen;
		}
		return base.GetPopupLocation();
	}

	public void ExitedConversation()
	{
		m_HavingConversation = false;
		EnableUI();
		EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, m_ConvoMusicEnd.ToString());
	}

	private void Update()
	{
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
