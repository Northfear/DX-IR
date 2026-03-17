public class HackingNode_DataStore : HackingNode_Base
{
	public DataStoreRewardType m_rewardType;

	public int m_RewardValue;

	public override void Setup()
	{
		m_name = "Data Store";
		m_type = HackingNodeType.DataStore;
	}

	public override void CaptureNode()
	{
		HackingRewards_Overlay.m_this.AddItem(m_rewardType, m_RewardValue);
	}

	public override void Start()
	{
	}

	public override void Update()
	{
	}
}
