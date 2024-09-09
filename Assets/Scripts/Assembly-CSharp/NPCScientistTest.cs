using UnityEngine;

public class NPCScientistTest : MonoBehaviour
{
	public AudioClip m_SpeechClip;

	public string m_AnimName;

	public AnimationState m_AnimState;

	public FaceFXControllerScript ffxController;

	public GameObject m_SpeakerObject;

	private GameObject m_PlayerObject;

	public float m_SpeechDelay;

	private float m_SpeechDelayMax = 10f;

	private bool m_Speaking;

	public string m_SpeakingAnim;

	public string m_IdleAnim;

	private float m_AnimBlendTime = 0.5f;

	private float m_TurnToFaceSpeed = 4f;

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

	private void Update()
	{
		if (m_Speaking)
		{
			Vector3 forward = m_PlayerObject.transform.position - base.transform.position;
			forward.y = 0f;
			Quaternion to = Quaternion.LookRotation(forward, new Vector3(0f, 1f, 0f));
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to, Time.deltaTime * m_TurnToFaceSpeed);
			if (m_SpeechDelay > 0f)
			{
				m_SpeechDelay -= Time.deltaTime;
				return;
			}
			m_Speaking = false;
			m_SpeechDelay = 0f;
			base.animation.CrossFade(m_IdleAnim, m_AnimBlendTime);
		}
	}

	private void Speak()
	{
		m_Speaking = true;
		ffxController.PlayAnim(m_AnimName, m_SpeechClip);
		m_SpeechDelay = m_SpeechDelayMax;
		base.animation.CrossFade(m_SpeakingAnim, m_AnimBlendTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			m_PlayerObject = other.transform.gameObject;
			if (!m_Speaking)
			{
				Speak();
			}
		}
	}
}
