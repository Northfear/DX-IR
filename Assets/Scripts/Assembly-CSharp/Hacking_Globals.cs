using UnityEngine;

public class Hacking_Globals : MonoBehaviour
{
	public GameObject m_CaptureEffectPrefab;

	public GameObject m_NukingEffectPrefab;

	public GameObject m_EnergyMeter;

	public GameObject m_HackingUI;

	public GameObject m_BridgeDirectionalArrowPrefab;

	public GameObject m_CaptureRatingPrefab;

	public GameObject m_SubRoutineSpamPrefab;

	private void Awake()
	{
		Globals.m_HackingGlobals = this;
	}
}
