using UnityEngine;

public class MenuAnim : MonoBehaviour
{
	public string m_AnimName;

	public float m_MenuAnimSpeed = 1f;

	private void Start()
	{
		base.animation[m_AnimName].speed = m_MenuAnimSpeed;
	}
}
