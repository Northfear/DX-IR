using UnityEngine;

public class InteractiveVolume : MonoBehaviour
{
	public InteractiveObject_Base[] m_InteractiveObject;

	private bool m_VolumeReady = true;

	public void EnableCollider()
	{
		base.collider.enabled = true;
	}

	public void DisableCollider()
	{
		base.collider.enabled = false;
	}

	private void Start()
	{
		float num = ((m_InteractiveObject.Length <= 0 || m_InteractiveObject[0].GetType() != typeof(InteractiveObject_Takedown)) ? Globals.m_InteractiveObjectManager.m_DefaultTriggerVolumeSize : Globals.m_InteractiveObjectManager.m_TakedownTriggerVolumeSize);
		Collider collider = base.collider;
		if (collider.GetType() == typeof(BoxCollider))
		{
			BoxCollider boxCollider = collider as BoxCollider;
			boxCollider.size = new Vector3(num, num, num);
		}
		else if (collider.GetType() == typeof(SphereCollider))
		{
			SphereCollider sphereCollider = collider as SphereCollider;
			sphereCollider.radius = num;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_VolumeReady || !(other.gameObject.tag == "Player"))
		{
			return;
		}
		m_VolumeReady = false;
		for (int i = 0; i < m_InteractiveObject.Length; i++)
		{
			if ((bool)m_InteractiveObject[i])
			{
				m_InteractiveObject[i].EnableInteractiveObject();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (m_VolumeReady || !(other.gameObject.tag == "Player"))
		{
			return;
		}
		m_VolumeReady = true;
		for (int i = 0; i < m_InteractiveObject.Length; i++)
		{
			if ((bool)m_InteractiveObject[i])
			{
				m_InteractiveObject[i].DisableInteractiveObject();
			}
		}
	}

	private void Update()
	{
		if (m_VolumeReady)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < m_InteractiveObject.Length; i++)
		{
			if (!(m_InteractiveObject[i] == null))
			{
				num++;
				if (m_InteractiveObject[i].IsMarkedForDelete())
				{
					InteractiveObject_Base interactiveObject_Base = m_InteractiveObject[i];
					m_InteractiveObject[i] = null;
					Object.Destroy(interactiveObject_Base.gameObject);
				}
			}
		}
		if (num != 0)
		{
		}
	}
}
