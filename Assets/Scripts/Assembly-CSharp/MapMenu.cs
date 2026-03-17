using UnityEngine;

public class MapMenu : MonoBehaviour
{
	public static MapMenu m_This;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	public static void MapMenuOpening()
	{
		if (!(m_This == null))
		{
		}
	}
}
