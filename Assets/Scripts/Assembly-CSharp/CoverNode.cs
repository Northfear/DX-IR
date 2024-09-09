using UnityEngine;

public class CoverNode : MonoBehaviour
{
	public enum CoverType
	{
		None = 0,
		CrouchLeft = 1,
		CrouchRight = 2,
		CrouchOver = 4,
		StandLeft = 8,
		StandRight = 0x10
	}

	[HideInInspector]
	public int m_CoverType;

	[HideInInspector]
	public bool m_SafelyPositioned;

	private void Start()
	{
		if (m_SafelyPositioned)
		{
			Globals.m_AIDirector.AddCoverNode(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		if (m_SafelyPositioned)
		{
			Gizmos.DrawIcon(base.transform.position, "CoverNode_Good.tga", false);
			Debug.DrawLine(base.transform.position, base.transform.position + 0.5f * base.transform.forward, Color.green, 0f, true);
		}
		else
		{
			Gizmos.DrawIcon(base.transform.position, "CoverNode_Bad.tga", false);
			Debug.DrawLine(base.transform.position, base.transform.position + 0.5f * base.transform.forward, Color.red, 0f, true);
		}
	}
}
