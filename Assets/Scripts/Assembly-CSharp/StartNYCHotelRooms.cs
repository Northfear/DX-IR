using UnityEngine;

public class StartNYCHotelRooms : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_ElevatorAccess;

	public GameObject m_AnnaObject;

	public Transform m_AnnaLocation;

	public GameObject m_AnnaDoor;

	public GameObject m_AnnaDoorTrigger;

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
		m_AnnaDoorTrigger.active = false;
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception))
		{
			Globals.m_PrimaryObjective = m_ElevatorAccess;
		}
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna) && Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception))
		{
			Globals.m_PrimaryObjective = m_AnnaDoor.transform;
			m_AnnaDoorTrigger.active = true;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna))
		{
			Globals.m_PrimaryObjective = m_ElevatorAccess;
			Object.Destroy(m_AnnaDoor);
			Object.Destroy(m_AnnaObject);
		}
	}

	private void MetAnna()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna, true);
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception, true);
		Globals.m_PrimaryObjective = m_ElevatorAccess;
	}
}
