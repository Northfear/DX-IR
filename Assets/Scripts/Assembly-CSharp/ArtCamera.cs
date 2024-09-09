using UnityEngine;

public class ArtCamera : MonoBehaviour
{
	private void Start()
	{
		if (!Globals.m_Bloom)
		{
			MobileBloom component = base.gameObject.GetComponent<MobileBloom>();
			if ((bool)component)
			{
				component.enabled = false;
			}
		}
	}
}
