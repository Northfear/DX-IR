using UnityEngine;

public class StartPanamaHotelLobby : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Exit;

	public Transform m_Elevator;

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
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDoctorMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedHavok, true);
			Globals.SetMissionStatus(1, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(1, 0, MissionStatus.Completed_Success);
			Globals.SetMissionStatus(2, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 0, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 1, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 2, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 3, MissionStatus.Completed_Fail);
			Globals.SetSubMissionStatus(2, 4, MissionStatus.Completed_Success);
			Globals.SetMissionStatus(3, MissionStatus.Acquired);
			Globals.SetSubMissionStatus(3, 0, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(3, 1, MissionStatus.Acquired);
			Globals.TrackMission(3, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedHavok) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPFound))
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_Elevator });
		}
		else if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionComplete))
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_Elevator });
		}
		else
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_Exit });
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
}
