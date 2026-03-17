using System.Collections.Generic;
using UnityEngine;

public class AkBankLoad : MonoBehaviour
{
	public List<string> bankNames = new List<string>();

	private uint[] m_BankIDs;

	private void Start()
	{
		m_BankIDs = new uint[bankNames.Count];
		for (int i = 0; i < bankNames.Count; i++)
		{
			AkSoundEngine.LoadBank(bankNames[i], -1, out m_BankIDs[i]);
		}
	}

	private void OnDisable()
	{
		uint[] bankIDs = m_BankIDs;
		foreach (uint in_bankID in bankIDs)
		{
			AkSoundEngine.UnloadBank(in_bankID);
		}
	}
}
