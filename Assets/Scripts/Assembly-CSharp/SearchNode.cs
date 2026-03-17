using UnityEngine;

public class SearchNode : MonoBehaviour
{
	public enum SearchType
	{
		Room = 0
	}

	public SearchType m_SearchType;

	[HideInInspector]
	public bool m_SafelyPositioned;

	private void Start()
	{
		if (m_SafelyPositioned)
		{
			Globals.m_AIDirector.AddSearchNode(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, (!m_SafelyPositioned) ? "SearchNode_Bad.tga" : "SearchNode_Good.tga", false);
		Debug.DrawRay(base.transform.position, base.transform.forward, (!m_SafelyPositioned) ? Color.red : Color.blue, 0f, true);
	}
}
