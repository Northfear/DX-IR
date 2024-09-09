using System.Collections.Generic;
using UnityEngine;

public class Hacking_VictoryResults : MonoBehaviour
{
	public static Hacking_VictoryResults m_this;

	public PackedSprite m_topPanel;

	public PackedSprite[] m_centerPanel;

	public PackedSprite m_bottomPanel;

	public Reward_Info m_nukeVirus;

	public Reward_Info m_stopWorm;

	public Reward_Info m_xp;

	public Reward_Info m_credits;

	public void SetupResults()
	{
		List<HackingNode> dataStoreList = HackingSystem.m_this.GetDataStoreList();
		for (int i = 0; i < dataStoreList.Count; i++)
		{
			if (dataStoreList[i].IsCapturedByPlayer() || HackingSystem.m_this.IsGivingAllRewards())
			{
				switch (((HackingNode_DataStore)dataStoreList[i].m_node).m_rewardType)
				{
				case DataStoreRewardType.Money:
					m_credits.AddToRewardAmount(((HackingNode_DataStore)dataStoreList[i].m_node).m_RewardValue, "+{0}");
					m_credits.UnHide();
					break;
				case DataStoreRewardType.NukeVirus:
					m_nukeVirus.AddToRewardAmount(((HackingNode_DataStore)dataStoreList[i].m_node).m_RewardValue, "+{0} NUKE VIRUS");
					m_nukeVirus.UnHide();
					break;
				case DataStoreRewardType.StopWorm:
					m_stopWorm.AddToRewardAmount(((HackingNode_DataStore)dataStoreList[i].m_node).m_RewardValue, "+{0} STOP WORM");
					m_stopWorm.UnHide();
					break;
				case DataStoreRewardType.XP:
					m_xp.AddToRewardAmount(((HackingNode_DataStore)dataStoreList[i].m_node).m_RewardValue, "+{0} XP");
					m_xp.UnHide();
					break;
				}
			}
		}
		int num = 0;
		if (m_credits.GetRewardAmount() > 0)
		{
			num++;
		}
		if (m_nukeVirus.GetRewardAmount() > 0)
		{
			num++;
		}
		if (m_stopWorm.GetRewardAmount() > 0)
		{
			num++;
		}
		if (m_xp.GetRewardAmount() > 0)
		{
			num++;
		}
		float num2 = 0f;
		if (num % 2 == 0)
		{
			num2 = m_centerPanel[0].height * (float)(num / 2);
		}
		float num3 = m_centerPanel[0].height / 2f;
		float num4 = m_topPanel.height / 2f;
		float num5 = m_bottomPanel.height / 2f;
		Vector3 localPosition = new Vector3(0f, num2 + (num4 + num3), 0f);
		m_topPanel.gameObject.transform.localPosition = localPosition;
		m_topPanel.Hide(false);
		localPosition.y -= num4;
		for (int j = 0; j < m_centerPanel.Length; j++)
		{
			if (m_centerPanel[j] != null)
			{
				m_centerPanel[j].Hide(true);
			}
		}
		for (int k = 0; k < num; k++)
		{
			localPosition.y -= num3;
			m_centerPanel[k].gameObject.transform.localPosition = localPosition;
			m_centerPanel[k].Hide(false);
			float num6 = ((k != 0) ? 4 : 2);
			if (m_nukeVirus.GetRewardAmount() > 0)
			{
				Vector3 position = m_nukeVirus.m_background.gameObject.transform.parent.position;
				position.y = m_centerPanel[k].gameObject.transform.position.y - num6;
				m_nukeVirus.m_background.gameObject.transform.parent.position = position;
				m_nukeVirus.ResetRewardAmount();
				m_nukeVirus.UnHide();
			}
			else if (m_stopWorm.GetRewardAmount() > 0)
			{
				Vector3 position2 = m_nukeVirus.m_background.gameObject.transform.parent.position;
				position2.y = m_centerPanel[k].gameObject.transform.position.y - num6;
				m_stopWorm.m_background.gameObject.transform.parent.position = position2;
				m_stopWorm.ResetRewardAmount();
				m_stopWorm.UnHide();
			}
			else if (m_xp.GetRewardAmount() > 0)
			{
				Vector3 position3 = m_nukeVirus.m_background.gameObject.transform.parent.position;
				position3.y = m_centerPanel[k].gameObject.transform.position.y - num6;
				m_xp.m_background.gameObject.transform.parent.position = position3;
				m_xp.ResetRewardAmount();
				m_xp.UnHide();
			}
			else if (m_credits.GetRewardAmount() > 0)
			{
				Vector3 position4 = m_nukeVirus.m_background.gameObject.transform.parent.position;
				position4.y = m_centerPanel[k].gameObject.transform.position.y - num6;
				m_credits.m_background.gameObject.transform.parent.position = position4;
				m_credits.ResetRewardAmount();
				m_credits.UnHide();
			}
			localPosition.y -= num3;
		}
		localPosition.y -= num5;
		localPosition.x = 1.2f;
		m_bottomPanel.gameObject.transform.localPosition = localPosition;
		m_bottomPanel.Hide(false);
	}

	private void Awake()
	{
		if (m_this == null)
		{
			m_this = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
