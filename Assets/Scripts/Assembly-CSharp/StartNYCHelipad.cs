using UnityEngine;

public class StartNYCHelipad : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_HelipadExit;

	public GameObject m_WelcomeTrigger;

	private void Awake()
	{
		if (GameManager.m_This != null)
		{
			GameManager.m_This.m_CurrentLocation = Location.NewYork;
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
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPFound, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionComplete, true);
		}
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted, true);
		}
		Globals.m_PrimaryObjective = m_HelipadExit;
	}
}
