using UnityEngine;

public class NPCCivilianTest : MonoBehaviour
{
	public string m_SpeechEvent;

	public string m_AnimName;

	public AnimationState m_AnimState;

	public FaceFXControllerScript ffxController;

	public GameObject m_SpeakerObject;

	public string m_SpeakingAnim;

	public string m_IdleAnim;

	private float m_AnimBlendTime = 0.5f;

	private void Start()
	{
		if (m_SpeakerObject != null)
		{
			if (m_SpeakerObject.animation != null)
			{
				m_AnimState = m_SpeakerObject.animation[m_AnimName];
				if (m_AnimState != null)
				{
					m_AnimState.layer = 1;
					m_AnimState.wrapMode = WrapMode.ClampForever;
					m_AnimState.blendMode = AnimationBlendMode.Blend;
				}
				else
				{
					Debug.Log("animState is NULL!");
				}
			}
			else
			{
				Debug.Log("PlayerObject.animation is NULL!");
			}
		}
		else
		{
			Debug.Log("PlayerObject is NULL!");
		}
		base.animation.CrossFade(m_IdleAnim, m_AnimBlendTime);
	}

	private void Speak()
	{
		Debug.Log("Force NPC to talk.");
		ffxController.PlayAnim(m_AnimName, m_SpeechEvent);
		base.animation.CrossFade(m_SpeakingAnim, m_AnimBlendTime);
	}
}
