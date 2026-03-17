using UnityEngine;

public class StartNYCStreets : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_EnterNYSE;

	public GameObject m_EnterNYSEObject;

	public Transform m_EnterBank;

	public Transform m_EnterHotel;

	public Transform m_EnterSubway;

	public Transform m_EnterLimb;

	public GameObject m_AnnaInviteTrigger;

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
		m_EnterNYSEObject.active = false;
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception))
		{
			Globals.m_PrimaryObjective = m_EnterHotel;
		}
		else
		{
			Object.Destroy(m_AnnaInviteTrigger);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedIsaac))
		{
			Globals.m_PrimaryObjective = m_EnterLimb;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHackers))
		{
			Globals.m_PrimaryObjective = m_EnterSubway;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHaveUplink) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink))
		{
			Globals.m_PrimaryObjective = m_EnterBank;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHackedBank))
		{
			Globals.m_PrimaryObjective = m_EnterSubway;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCEscapedBelltowerRaid))
		{
			m_EnterNYSEObject.active = true;
			Globals.m_PrimaryObjective = m_EnterNYSE;
			Globals.m_SecondaryObjective = m_EnterSubway;
		}
	}
}
