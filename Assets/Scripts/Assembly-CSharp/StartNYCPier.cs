using UnityEngine;

public class StartNYCPier : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_SubwayAccess;

	public Transform m_IntrepidEntrance;

	public GameObject m_BelltowerAttackGroup;

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
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHackers))
		{
			Globals.m_PrimaryObjective = m_IntrepidEntrance;
		}
		else
		{
			Globals.m_PrimaryObjective = m_SubwayAccess;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHackedBank))
		{
			Globals.m_PrimaryObjective = m_IntrepidEntrance;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCAnnaWasAbducted) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCEscapedBelltowerRaid))
		{
			m_BelltowerAttackGroup.SetActiveRecursively(true);
			Globals.m_PrimaryObjective = m_SubwayAccess;
		}
	}
}
