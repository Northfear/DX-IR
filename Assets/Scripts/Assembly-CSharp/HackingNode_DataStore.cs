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

	private void Start()
	{
	}

	private void Update()
	{
	}
}
