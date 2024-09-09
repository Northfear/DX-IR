using System;
using UnityEngine;

public class TyrantOutroStart : MonoBehaviour
{
	[Serializable]
	public class FadeEvent
	{
		public float m_TriggerTime = -1f;

		public ScreenFade m_FadeComponent;

		public float m_FadeInDuration = 1f;

		public float m_IdleDuration = 1f;

		public float m_FadeOutDuration = 0.5f;

		public void TriggerScreenFade()
		{
			m_TriggerTime = -1f;
			if (m_FadeComponent != null)
			{
				m_FadeComponent.StartFade(m_FadeInDuration, m_IdleDuration, m_FadeOutDuration);
			}
		}
	}

	public GameObject m_ActivationObject;

	public GameObject m_DeactivationObject;

	public FadeEvent[] m_ScreenFades;

	public GameObject m_CinematicSpeaker;

	public FaceFXControllerScript ffxController;

	public AudioClip m_SpeechClip;

	public string m_AnimName;

	public AnimationState m_AnimState;

	public string m_BodyAnim;

	private bool m_CinematicStarted;

	private bool m_FaceFXPlaying;

	private float m_CinematicTimer;

	private void Start()
	{
		if (m_CinematicSpeaker != null)
		{
			if (m_CinematicSpeaker.animation != null)
			{
				m_AnimState = m_CinematicSpeaker.animation[m_AnimName];
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
				Debug.Log("SpeakerObject.animation is NULL!");
			}
		}
		else
		{
			Debug.Log("SpeakerObject is NULL!");
		}
	}

	private void Update()
	{
		if (m_CinematicStarted && !m_FaceFXPlaying)
		{
			m_CinematicSpeaker.animation.CrossFade(m_BodyAnim, 0.5f);
			ffxController.PlayAnim(m_AnimName, m_SpeechClip);
			m_FaceFXPlaying = true;
		}
		if (!m_CinematicStarted)
		{
			return;
		}
		m_CinematicTimer += Time.deltaTime;
		for (int i = 0; i < m_ScreenFades.Length; i++)
		{
			if (m_ScreenFades[i].m_TriggerTime >= 0f && m_ScreenFades[i].m_TriggerTime <= m_CinematicTimer)
			{
				m_ScreenFades[i].TriggerScreenFade();
			}
		}
	}

	private void StartOutro()
	{
		m_CinematicStarted = true;
		m_ActivationObject.SetActiveRecursively(true);
		m_DeactivationObject.SetActiveRecursively(false);
		Globals.m_PlayerController.gameObject.SetActiveRecursively(false);
	}
}
