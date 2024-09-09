using Fabric;
using UnityEngine;

public class HackingRewards_Overlay : MonoBehaviour
{
	public static HackingRewards_Overlay m_this;

	public Reward_Info m_nukeVirus;

	public Reward_Info m_stopWorm;

	public Reward_Info m_xp;

	public Reward_Info m_credits;

	public float m_startingYHeight = -49f;

	public float m_offset = 9f;

	public void AddItem(DataStoreRewardType type, int amount)
	{
		switch (type)
		{
		case DataStoreRewardType.Money:
			m_credits.AddToRewardAmount(amount, "+{0}");
			m_credits.Activate();
			m_nukeVirus.Shift(m_offset);
			m_stopWorm.Shift(m_offset);
			m_xp.Shift(m_offset);
			EventManager.Instance.PostEvent("Hack_Datastore_Credits", EventAction.PlaySound, null, base.gameObject);
			break;
		case DataStoreRewardType.NukeVirus:
			m_nukeVirus.AddToRewardAmount(amount, "+{0} NUKE VIRUS");
			m_nukeVirus.Activate();
			m_credits.Shift(m_offset);
			m_stopWorm.Shift(m_offset);
			m_xp.Shift(m_offset);
			EventManager.Instance.PostEvent("Hack_Datastore_Nuke", EventAction.PlaySound, null, base.gameObject);
			break;
		case DataStoreRewardType.StopWorm:
			m_stopWorm.AddToRewardAmount(amount, "+{0} STOP WORM");
			m_stopWorm.Activate();
			m_credits.Shift(m_offset);
			m_nukeVirus.Shift(m_offset);
			m_xp.Shift(m_offset);
			EventManager.Instance.PostEvent("Hack_Datastore_Virus", EventAction.PlaySound, null, base.gameObject);
			break;
		case DataStoreRewardType.XP:
			m_xp.AddToRewardAmount(amount, "+{0} XP");
			m_xp.Activate();
			m_credits.Shift(m_offset);
			m_nukeVirus.Shift(m_offset);
			m_stopWorm.Shift(m_offset);
			EventManager.Instance.PostEvent("Hack_Datastore_XP", EventAction.PlaySound, null, base.gameObject);
			break;
		}
	}

	private void Awake()
	{
		if (m_this == null)
		{
			m_this = this;
		}
		m_nukeVirus.Deactivate(m_startingYHeight);
		m_stopWorm.Deactivate(m_startingYHeight);
		m_xp.Deactivate(m_startingYHeight);
		m_credits.Deactivate(m_startingYHeight);
	}

	private void Start()
	{
	}

	private void Update()
	{
		m_nukeVirus.Update();
		m_stopWorm.Update();
		m_xp.Update();
		m_credits.Update();
	}
}
