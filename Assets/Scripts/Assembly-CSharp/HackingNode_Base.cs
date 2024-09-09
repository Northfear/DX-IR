using UnityEngine;

public class HackingNode_Base : MonoBehaviour
{
	public string m_name = "Node";

	public HackingNodeType m_type;

	public int m_rating = 1;

	public virtual void Setup()
	{
	}

	public virtual void CaptureNode()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
