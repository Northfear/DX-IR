using UnityEngine;

public class ConversationSystem : MonoBehaviour
{
	public ConversationButton m_ConversationButton1;

	public ConversationButton m_ConversationButton2;

	public ConversationButton m_ConversationButton3;

	public ConversationButton m_ConversationButton4;

	public ConversationButton m_ConversationButton5;

	public PersuasionProfile m_PersuasionProfile;

	public PersuasionMeter m_PersuasionMeter;

	public ConversationText m_ConversationText;

	public SpriteText m_DialogSequenceText;

	public UIPanel m_ConversationPanel;

	public UIPanel m_SequencePanel;

	public UIPanel m_PersuasionPanel;

	public Camera m_ConversationUICamera;

	private float m_TurnToFaceSpeed = 4f;

	public float m_AnimBlendTime = 0.5f;

	private AudioSource m_AudioSource;

	private AudioSource m_PartnerAudioSource;

	private Conversation_Base m_CurrentConversation;

	private DialogChoice m_ChoicePressed;

	private DialogSequence m_CurrentSequence;

	private InteractiveObject_Conversation m_InteractiveObject;

	private FaceFXControllerScript m_PartnerFaceFXController;

	private int m_PersuasionLevel;

	private Camera m_CurrentCamera;

	private bool m_Speaking;

	public int GetPersuasionLevel()
	{
		return m_PersuasionLevel;
	}

	public void AddPersuasionLevel(int amount)
	{
		m_PersuasionLevel = Mathf.Max(Mathf.Min(m_PersuasionLevel + amount, 100), 0);
	}

	public bool IsSpeaking()
	{
		return m_Speaking;
	}

	public bool IsFaceFXAnimating()
	{
		if (m_CurrentSequence != null)
		{
			if (m_CurrentSequence.m_DialogTarget != DialogSequence.DialogTarget.NPC)
			{
				if ((bool)m_InteractiveObject.m_PlayerFaceFXController)
				{
					return m_InteractiveObject.m_PlayerFaceFXController.GetPlayState() != 0;
				}
				return m_AudioSource.isPlaying;
			}
			if ((bool)m_PartnerFaceFXController)
			{
				return m_PartnerFaceFXController.GetPlayState() != 0;
			}
			if (m_PartnerAudioSource != null)
			{
				return m_PartnerAudioSource.isPlaying;
			}
		}
		return false;
	}

	public void SetPartnerFaceFXController(FaceFXControllerScript controller)
	{
		m_PartnerFaceFXController = controller;
	}

	public void RegisterInteractiveObject(InteractiveObject_Conversation conversation)
	{
		m_InteractiveObject = conversation;
	}

	public void DialogChoice1Pressed()
	{
		m_CurrentConversation.DialogChoicePressed(Conversation_Base.DialogChoiceEnum.DialogChoice1);
	}

	public void DialogChoice2Pressed()
	{
		m_CurrentConversation.DialogChoicePressed(Conversation_Base.DialogChoiceEnum.DialogChoice2);
	}

	public void DialogChoice3Pressed()
	{
		m_CurrentConversation.DialogChoicePressed(Conversation_Base.DialogChoiceEnum.DialogChoice3);
	}

	public void DialogChoice4Pressed()
	{
		m_CurrentConversation.DialogChoicePressed(Conversation_Base.DialogChoiceEnum.DialogChoice4);
	}

	public void DialogChoice5Pressed()
	{
		m_CurrentConversation.SetSelectedDialogChoice(Conversation_Base.DialogChoiceEnum.DialogChoice5);
		m_CurrentConversation.DialogChoiceSelected();
	}

	public void DialogChoiceSelected()
	{
		m_CurrentConversation.DialogChoiceSelected();
	}

	public void SkipDialogSequencePressed()
	{
		m_CurrentConversation.SkipCurrentDialogSequence();
	}

	public void BeginConversation(Conversation_Base conversation)
	{
		if ((bool)m_InteractiveObject.m_NPCAnimator && m_InteractiveObject.m_NPCIdleAnimName != string.Empty)
		{
			AnimationState animationState = m_InteractiveObject.m_NPCAnimator[m_InteractiveObject.m_NPCIdleAnimName];
			if ((bool)animationState)
			{
				animationState.wrapMode = WrapMode.Loop;
				m_InteractiveObject.m_NPCAnimator.Play();
			}
		}
		if ((bool)m_InteractiveObject.m_PlayerAnimator && m_InteractiveObject.m_PlayerIdleAnimName != string.Empty)
		{
			AnimationState animationState2 = m_InteractiveObject.m_PlayerAnimator[m_InteractiveObject.m_PlayerIdleAnimName];
			if ((bool)animationState2)
			{
				animationState2.wrapMode = WrapMode.Loop;
				m_InteractiveObject.m_PlayerAnimator.Play();
			}
		}
		m_ConversationUICamera.enabled = true;
		UIManager.instance.AddCamera(m_ConversationUICamera, 4096, 100f, 0);
		m_CurrentCamera = Globals.m_PlayerController.m_CurrentCamera;
		conversation.BeginConversation(false);
		Augmentation_SocialEnhancer augmentation_SocialEnhancer = (Augmentation_SocialEnhancer)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.SocialEnhancer);
		bool flag = !augmentation_SocialEnhancer.IsSocialEnhancerActive();
		m_PersuasionLevel = m_InteractiveObject.m_StartingPersuasionLevel;
		if (!flag)
		{
			m_PersuasionPanel.BringIn();
			bool hide = m_CurrentConversation.m_PersuasionInfo.m_ProfileText == null;
			if (m_CurrentConversation.m_PersuasionInfo.m_ProfileText != null)
			{
				m_PersuasionProfile.m_ProfileText.Text = m_CurrentConversation.m_PersuasionInfo.m_ProfileText;
			}
			if (m_CurrentConversation.m_PersuasionInfo.m_ProfileTitleText != null)
			{
				m_PersuasionProfile.m_ProfileTitleText.Text = m_CurrentConversation.m_PersuasionInfo.m_ProfileTitleText;
			}
			m_PersuasionProfile.Hide(hide);
			m_PersuasionMeter.SetPersuasionAmount(m_PersuasionLevel);
			m_PersuasionMeter.Hide(false);
		}
		else
		{
			m_PersuasionProfile.Hide(true);
			m_PersuasionMeter.Hide(true);
		}
		Globals.m_PlayerController.m_ModelFirstPerson.SetActiveRecursively(false);
		Globals.m_PlayerController.m_ModelThirdPerson.SetActiveRecursively(false);
		m_InteractiveObject.m_PlayerDialogModel.SetActiveRecursively(true);
		Transform transform = m_InteractiveObject.m_PlayerDialogModel.transform;
		transform.parent = Globals.m_PlayerController.gameObject.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		m_PartnerAudioSource = m_InteractiveObject.GetAudioSource();
	}

	public void SetupConvsersation(Conversation_Base conversation)
	{
		if (!(conversation == null))
		{
			m_CurrentConversation = conversation;
		}
	}

	public void SetupChoicePressed(DialogChoice choice)
	{
		bool flag = choice == null;
		m_ConversationText.Hide(flag);
		m_ConversationText.m_Button.Hide(flag);
		m_ConversationText.m_Text.Hide(flag);
		if (!flag)
		{
			m_ConversationText.m_Text.Text = choice.m_ChoiceHighlightString;
		}
	}

	public void SetupDialog(DialogSequence sequence)
	{
		if (sequence.m_AudioVO == null)
		{
			Debug.Log("No Audio Clip set.");
			return;
		}
		m_CurrentCamera.enabled = false;
		m_Speaking = true;
		m_CurrentSequence = sequence;
		if (sequence.m_DialogTarget == DialogSequence.DialogTarget.NPC)
		{
			if (m_PartnerFaceFXController != null)
			{
				if (m_InteractiveObject.m_NPCAnimator != null && sequence.m_BodyAnimationName != string.Empty)
				{
					AnimationState animationState = m_InteractiveObject.m_NPCAnimator[sequence.m_AnimationName];
					if (animationState != null)
					{
						animationState.layer = 1;
						animationState.wrapMode = WrapMode.ClampForever;
						animationState.blendMode = AnimationBlendMode.Blend;
					}
					else
					{
						Debug.Log("This AnimationState does not exist on the player.");
					}
					m_InteractiveObject.m_NPCAnimator.CrossFade(sequence.m_BodyAnimationName, m_AnimBlendTime);
				}
				m_PartnerFaceFXController.PlayAnim(sequence.m_AnimationName, sequence.m_AudioVO);
			}
			else
			{
				if (m_PartnerAudioSource != null)
				{
					m_PartnerAudioSource.clip = sequence.m_AudioVO;
					m_PartnerAudioSource.Play();
				}
				Debug.Log("No partner Face FX Controller set.");
			}
		}
		else if (m_InteractiveObject.m_PlayerFaceFXController != null)
		{
			if (m_InteractiveObject.m_PlayerAnimator != null && sequence.m_BodyAnimationName != string.Empty)
			{
				AnimationState animationState2 = m_InteractiveObject.m_PlayerAnimator[sequence.m_AnimationName];
				if (animationState2 != null)
				{
					animationState2.layer = 1;
					animationState2.wrapMode = WrapMode.ClampForever;
					animationState2.blendMode = AnimationBlendMode.Blend;
				}
				else
				{
					Debug.Log("This AnimationState does not exist on the player.");
				}
				m_InteractiveObject.m_PlayerAnimator.CrossFade(sequence.m_BodyAnimationName, m_AnimBlendTime);
			}
			m_InteractiveObject.m_PlayerFaceFXController.PlayAnim(sequence.m_AnimationName, sequence.m_AudioVO);
		}
		else
		{
			m_AudioSource.clip = sequence.m_AudioVO;
			m_AudioSource.Play();
			Debug.Log("No player Face FX Controller set.");
		}
		m_CurrentSequence.m_SequenceDialogTimer = 0f;
		m_CurrentSequence.m_SequenceDialogIndex = 0;
		if (m_CurrentSequence.m_SequenceDialogIndex != m_CurrentSequence.m_SequenceDialog.Length)
		{
			m_DialogSequenceText.Text = m_CurrentSequence.m_SequenceDialog[m_CurrentSequence.m_SequenceDialogIndex].m_DialogText;
		}
		else
		{
			m_DialogSequenceText.Text = string.Empty;
		}
		m_CurrentCamera = sequence.m_SequenceCamera;
		m_CurrentCamera.enabled = true;
	}

	public void StopCurrentDialog()
	{
		if (m_CurrentSequence.m_DialogTarget == DialogSequence.DialogTarget.NPC)
		{
			if (m_PartnerFaceFXController != null)
			{
				m_PartnerFaceFXController.StopAnim();
			}
			else if (m_PartnerAudioSource != null)
			{
				m_PartnerAudioSource.Stop();
			}
		}
		else if (m_InteractiveObject.m_PlayerFaceFXController != null)
		{
			m_InteractiveObject.m_PlayerFaceFXController.StopAnim();
		}
		else
		{
			m_AudioSource.Stop();
		}
	}

	public void BringInConversationPanel(bool postSequence)
	{
		if (postSequence)
		{
			m_SequencePanel.Dismiss();
		}
		m_ConversationPanel.BringIn();
		if ((bool)m_CurrentConversation.m_ConversationCamera)
		{
			m_CurrentCamera.enabled = false;
			m_CurrentCamera = m_CurrentConversation.m_ConversationCamera;
			m_CurrentCamera.enabled = true;
		}
		SetupButton(m_ConversationButton1, m_CurrentConversation.m_ConversationInfo.m_DialogChoice1);
		SetupButton(m_ConversationButton2, m_CurrentConversation.m_ConversationInfo.m_DialogChoice2);
		SetupButton(m_ConversationButton3, m_CurrentConversation.m_ConversationInfo.m_DialogChoice3);
		SetupButton(m_ConversationButton4, m_CurrentConversation.m_ConversationInfo.m_DialogChoice4);
		Augmentation_SocialEnhancer augmentation_SocialEnhancer = (Augmentation_SocialEnhancer)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.SocialEnhancer);
		DialogChoice dialogChoice = m_CurrentConversation.m_ConversationInfo.m_DialogChoice5;
		bool tf = false;
		if (!augmentation_SocialEnhancer.IsSocialEnhancerActive() || dialogChoice.m_ChoiceString == null)
		{
			tf = true;
		}
		if (dialogChoice.m_ChoiceString != null)
		{
			m_ConversationButton5.m_TabText.Text = dialogChoice.m_ChoiceString;
		}
		m_ConversationButton5.m_Button.Hide(tf);
		m_ConversationButton5.m_TabText.Hide(tf);
	}

	public void BringInDialogPanel(bool preSequence)
	{
		if (!preSequence)
		{
			m_ConversationPanel.Dismiss();
		}
		m_SequencePanel.BringIn();
	}

	public void EndConversation()
	{
		m_Speaking = false;
		m_CurrentCamera.enabled = false;
		Globals.m_PlayerController.m_CurrentCamera.enabled = true;
		UIManager.instance.RemoveCamera(0);
		m_ConversationUICamera.enabled = false;
		m_SequencePanel.Dismiss();
		Globals.m_PlayerController.m_ModelFirstPerson.SetActiveRecursively(true);
		Globals.m_PlayerController.m_ModelThirdPerson.SetActiveRecursively(false);
		Object.Destroy(m_InteractiveObject.m_PlayerDialogModel);
		m_InteractiveObject.MarkInactive();
		m_InteractiveObject.ExitedConversation();
		if (m_InteractiveObject.m_NPCAnimator != null && m_InteractiveObject.m_NPCIdleAnimName != string.Empty)
		{
			m_InteractiveObject.m_NPCAnimator.CrossFade(m_InteractiveObject.m_NPCIdleAnimName, m_AnimBlendTime);
		}
	}

	private void Awake()
	{
		Globals.m_ConversationSystem = this;
		m_AudioSource = base.gameObject.AddComponent<AudioSource>();
		m_ConversationUICamera.enabled = false;
		m_ConversationPanel.DismissImmediate();
		m_SequencePanel.DismissImmediate();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_Speaking)
		{
			return;
		}
		if (m_CurrentSequence != null && m_CurrentSequence.m_SequenceDialogIndex != m_CurrentSequence.m_SequenceDialog.Length)
		{
			m_CurrentSequence.m_SequenceDialogTimer += Time.deltaTime;
			if (m_CurrentSequence.m_SequenceDialogTimer >= m_CurrentSequence.m_SequenceDialog[m_CurrentSequence.m_SequenceDialogIndex].m_Duration)
			{
				m_CurrentSequence.m_SequenceDialogIndex++;
				m_CurrentSequence.m_SequenceDialogTimer = 0f;
				if (m_CurrentSequence.m_SequenceDialogIndex != m_CurrentSequence.m_SequenceDialog.Length)
				{
					m_DialogSequenceText.Text = m_CurrentSequence.m_SequenceDialog[m_CurrentSequence.m_SequenceDialogIndex].m_DialogText;
				}
				else
				{
					m_DialogSequenceText.Text = string.Empty;
				}
			}
		}
		Vector3 forward = Globals.m_PlayerController.m_ModelThirdPerson.transform.position - base.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward, new Vector3(0f, 1f, 0f));
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to, Time.deltaTime * m_TurnToFaceSpeed);
		m_PersuasionMeter.UpdateTicks(m_PersuasionLevel);
	}

	private void SetupButton(ConversationButton button, DialogChoice choice)
	{
		bool tf = ((choice.m_ChoiceString == string.Empty) ? true : false);
		button.m_TabText.Text = choice.m_ChoiceString;
		button.m_Tab.Hide(tf);
		button.m_Button.Hide(tf);
	}
}
