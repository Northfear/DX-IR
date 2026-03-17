using UnityEngine;

public class StartPanamaTrainStation : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_StationExit;

	public GameObject m_StartupTrigger;

	public Transform m_Cobra;

	public GameObject m_CobraConversation;

	public GameObject m_EnemyGroup;

	public GameObject m_CivilianGroup;

	public NPC_Base[] m_NPCs;

	private bool m_ForceHolster = true;

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
		if (false)
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDoctorMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb, true);
		}
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetMissionStatus(1, MissionStatus.Acquired);
			Globals.SetSubMissionStatus(1, 0, MissionStatus.Acquired);
			Globals.SetMissionTransforms(1, new Transform[1] { m_StationExit });
			Globals.TrackMission(1, true);
			m_CivilianGroup.SetActiveRecursively(true);
		}
		else
		{
			Object.Destroy(m_StartupTrigger);
			if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_Cobra });
				m_CobraConversation.SetActiveRecursively(true);
			}
			else
			{
				Globals.SetMissionTransforms(1, new Transform[1] { m_StationExit });
				m_CivilianGroup.SetActiveRecursively(true);
			}
		}
		Globals.SetMissionTransforms(5, new Transform[1] { m_StationExit });
		if (m_CivilianGroup != null)
		{
			m_CivilianGroup.SetActiveRecursively(true);
		}
	}

	private void Update()
	{
		if (m_ForceHolster)
		{
			Globals.m_PlayerController.WeaponHolster(true);
			m_ForceHolster = false;
		}
	}

	private void CobraDealDone()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra, true);
		Globals.SetMissionTransforms(2, new Transform[1] { m_StationExit });
		Globals.SetSubMissionStatus(2, 1, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(2, 3, MissionStatus.Acquired);
		Globals.SetMissionStatus(5, MissionStatus.Acquired);
		Globals.SetSubMissionStatus(5, 0, MissionStatus.Acquired);
		Globals.TrackMission(5, true);
		Globals.SetMissionTransforms(5, new Transform[1] { m_StationExit });
		if (m_EnemyGroup != null)
		{
			m_EnemyGroup.SetActiveRecursively(true);
		}
		for (int num = m_NPCs.Length - 1; num >= 0; num--)
		{
			Globals.m_AIDirector.RemoveNPC(m_NPCs[num]);
		}
	}
}
