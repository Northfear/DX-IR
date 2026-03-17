using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AkAuxSendAware : MonoBehaviour
{
	private ArrayList m_activeAuxSends = new ArrayList();

	private AkAuxSendArray m_auxSendValues;

	private void Start()
	{
		GameObject gameObject = base.gameObject;
		while (gameObject != null && gameObject.GetComponent("AkAuxSend") == null && gameObject.transform.parent != null)
		{
			gameObject = gameObject.transform.parent.gameObject;
		}
		AddAuxSend(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		AddAuxSend(other.gameObject);
	}

	private void AddAuxSend(GameObject in_AuxSendObject)
	{
		AkAuxSend akAuxSend = (AkAuxSend)in_AuxSendObject.GetComponent("AkAuxSend");
		if (akAuxSend != null)
		{
			m_activeAuxSends.Add(akAuxSend);
			m_auxSendValues = null;
			UpdateAuxSend();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		AkAuxSend akAuxSend = (AkAuxSend)other.gameObject.GetComponent("AkAuxSend");
		if (akAuxSend != null)
		{
			m_activeAuxSends.Remove(akAuxSend);
			m_auxSendValues = null;
			UpdateAuxSend();
		}
	}

	private void Update()
	{
		AkGameObjectTracker akGameObjectTracker = (AkGameObjectTracker)base.gameObject.GetComponent("AkGameObjectTracker");
		if (akGameObjectTracker != null && akGameObjectTracker.HasMovedInLastFrame())
		{
			UpdateAuxSend();
		}
	}

	private void UpdateAuxSend()
	{
		if (m_auxSendValues == null)
		{
			m_auxSendValues = new AkAuxSendArray((uint)m_activeAuxSends.Count);
		}
		else
		{
			m_auxSendValues.Reset();
		}
		foreach (AkAuxSend activeAuxSend in m_activeAuxSends)
		{
			m_auxSendValues.Add(activeAuxSend.GetAuxBusID(), activeAuxSend.GetAuxSendValueForPosition(base.gameObject.transform.position));
		}
		AkSoundEngine.SetGameObjectAuxSendValues(base.gameObject, m_auxSendValues, (uint)m_activeAuxSends.Count);
	}
}
