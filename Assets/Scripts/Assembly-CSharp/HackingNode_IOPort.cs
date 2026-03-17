public class HackingNode_IOPort : HackingNode_Base
{
	public int m_baseSystemDifficulty = 1;

	public int m_xpReward;

	public override void Setup()
	{
		m_name = "I/O Port";
		m_type = HackingNodeType.IOPort;
	}

	public override void Start()
	{
	}

	public override void Update()
	{
	}
}
