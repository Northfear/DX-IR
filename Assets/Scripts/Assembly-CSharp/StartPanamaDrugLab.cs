using UnityEngine;

public class StartPanamaDrugLab : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_GarageAccess;

	public Transform m_EntryAccess;

	public GameObject m_GangLeaderConversation;

	public Transform m_GangLeaderPosition;

	public GameObject m_HostileGangsters;

	public GameObject m_NeutralGangsters;

	public EnemySpawner[] m_Gangsters;

	public GameObject m_CivilianGroup;

	public NPC_Base[] m_NPCs;

	private void Awake()
	{
		if (GameManager.m_This != null)
		{
			GameManager.m_This.m_CurrentLocation = Location.Panama;
		}
		RenderSettings.ambientLight = m_AmbientColor;
		RenderSettings.fog = m_EnableFog;
		RenderSettings.fogColor = m_FogColor;
		RenderSettings.fogMode = m_FogMode;
		RenderSettings.fogDensity = m_FogDensity;
		RenderSettings.fogStartDistance = m_FogStartDistance;
		RenderSettings.fogEndDistance = m_FogEndDistance;
	}

	private void Start()
	{
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDoctorMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra))
		{
			if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_GarageAccess });
			}
			else
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_EntryAccess });
			}
			if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaKilledGangLeader))
			{
				Globals.SetMissionTransforms(5, new Transform[1] { m_GangLeaderPosition });
				m_CivilianGroup.SetActiveRecursively(true);
			}
		}
		else
		{
			Globals.SetMissionTransforms(2, new Transform[1] { m_EntryAccess });
		}
		Globals.SetMissionTransforms(1, new Transform[1] { m_EntryAccess });
		Globals.SetMissionTransforms(3, new Transform[1] { m_EntryAccess });
	}

	public void SpawnHostileGangsters()
	{
		Object.Destroy(m_GangLeaderConversation);
		RemoveCivs();
		m_HostileGangsters.SetActiveRecursively(true);
		Globals.SetMissionTransforms(5, new Transform[1]);
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaKilledGangLeader, true);
	}

	public void SpawnNeutralGangsters()
	{
		Object.Destroy(m_GangLeaderConversation);
		RemoveCivs();
		for (int num = m_Gangsters.Length - 1; num >= 0; num--)
		{
			m_Gangsters[num].m_SpawnNeutral = true;
		}
		m_NeutralGangsters.SetActiveRecursively(true);
		Globals.SetMissionTransforms(5, new Transform[1]);
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang, true);
		Globals.SetSubMissionStatus(2, 2, MissionStatus.Completed_Fail);
		Globals.SetSubMissionStatus(2, 3, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(2, 4, MissionStatus.Acquired);
	}

	private void RemoveCivs()
	{
		for (int num = m_NPCs.Length - 1; num >= 0; num--)
		{
			Globals.m_AIDirector.RemoveNPC(m_NPCs[num]);
		}
	}
}
