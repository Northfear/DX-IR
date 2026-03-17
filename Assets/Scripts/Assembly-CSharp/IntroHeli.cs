using UnityEngine;

public class IntroHeli : MonoBehaviour
{
	public GameObject[] m_PropellerObject = new GameObject[2];

	private void Update()
	{
		for (int num = m_PropellerObject.Length - 1; num >= 0; num--)
		{
			Quaternion rotation = m_PropellerObject[num].transform.rotation;
			Vector3 eulerAngles = new Vector3(rotation.eulerAngles.x, Random.Range(0f, 360f), rotation.eulerAngles.z);
			rotation.eulerAngles = eulerAngles;
			m_PropellerObject[num].transform.rotation = rotation;
		}
	}
}
