using UnityEngine;

public class StartPanamaHotelRooms : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Elevator;

	public Transform m_RooftopAccess;

	public Transform m_VIP;

	public GameObject m_VIPGroup;

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
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedHavok) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPFound))
		{
			m_VIPGroup.SetActiveRecursively(true);
			Globals.SetMissionTransforms(3, new Transform[1] { m_VIP });
		}
		else if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionComplete))
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_RooftopAccess });
		}
		else
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_Elevator });
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

	private void BodyFound()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPFound, true);
		Globals.SetMissionTransforms(3, new Transform[1] { m_Elevator });
		Globals.SetSubMissionStatus(3, 1, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(3, 2, MissionStatus.Acquired);
	}
}
