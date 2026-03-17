using UnityEngine;

public class StartNYCSubwaySouth : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_StreetAccess;

	public Transform m_PierAccess;

	public Transform m_TunnelAccess;

	public GameObject m_TunnelAccessTrigger;

	public GameObject m_EscapeBelltowerTrigger;

	public Transform m_DisplayScreen;

	public GameObject m_DisplayScreenTrigger;

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
		m_DisplayScreenTrigger.active = false;
		m_EscapeBelltowerTrigger.active = false;
		m_TunnelAccessTrigger.active = false;
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedIsaac, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHackers))
		{
			Globals.m_PrimaryObjective = m_PierAccess;
		}
		else
		{
			Globals.m_PrimaryObjective = m_StreetAccess;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHackedBank))
		{
			Globals.m_PrimaryObjective = m_PierAccess;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCAnnaWasAbducted) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCEscapedBelltowerRaid))
		{
			m_EscapeBelltowerTrigger.active = true;
			Globals.m_PrimaryObjective = m_EscapeBelltowerTrigger.transform;
		}
	}

	private void EscapeBelltower()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCEscapedBelltowerRaid, true);
	}

	private void NewsBroadcast()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCDiscoveredUndergroundRoute, true);
	}
}
