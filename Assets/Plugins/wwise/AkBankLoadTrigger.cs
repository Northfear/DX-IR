using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AkBankLoadTrigger : MonoBehaviour
{
	public string bankName = string.Empty;

	private uint m_BankID;

	private void OnTriggerEnter(Collider other)
	{
		AkSoundEngine.LoadBank(bankName, BankCallback, null, -1, out m_BankID);
	}

	private void OnTriggerExit(Collider other)
	{
		AkSoundEngine.UnloadBank(m_BankID, BankCallback, null);
	}

	private void BankCallback(uint in_bankID, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie)
	{
	}
}
