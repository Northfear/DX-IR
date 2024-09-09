using System;
using UnityEngine;

[Serializable]
public class DialogSequence
{
	public enum DialogTarget
	{
		Player = 0,
		NPC = 1
	}

	public DialogTarget m_DialogTarget;

	public Camera m_SequenceCamera;

	public string m_AnimationName;

	public string m_BodyAnimationName;

	public AudioClip m_AudioVO;

	public SequenceDialogInfo[] m_SequenceDialog;

	[HideInInspector]
	public int m_SequenceDialogIndex;

	[HideInInspector]
	public float m_SequenceDialogTimer;
}
