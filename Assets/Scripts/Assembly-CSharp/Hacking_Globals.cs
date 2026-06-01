using UnityEngine;

public class Hacking_Globals : MonoBehaviour
{
	public GameObject m_CaptureEffectPrefab;

	public GameObject m_NukingEffectPrefab;

	public GameObject m_EnergyMeter;

	public GameObject m_HackingUI;

	[HideInInspector]
	public InteractiveObject_HackingTerminal m_ActiveTerminal;

	private void Awake()
	{
		Globals.m_HackingGlobals = this;
	}
}
