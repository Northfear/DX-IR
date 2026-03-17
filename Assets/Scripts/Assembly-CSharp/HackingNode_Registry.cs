public class HackingNode_Registry : HackingNode_Base
{
	public override void Setup()
	{
		m_name = "Registry";
		m_type = HackingNodeType.Registry;
	}

	public override void CaptureNode()
	{
		HackingSystem.m_this.CapturedRegistry();
	}

	public override void Start()
	{
	}

	public override void Update()
	{
	}
}
