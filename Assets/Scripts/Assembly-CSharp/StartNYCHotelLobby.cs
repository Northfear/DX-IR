using UnityEngine;

public class StartNYCHotelLobby : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_StreetAccess;

	public GameObject m_ReceptionistConversation;

	public Transform m_ReceptionistLocation;

	public Transform m_ElevatorAccess;

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
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception))
		{
			Globals.m_PrimaryObjective = m_ReceptionistLocation;
		}
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna) && Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception))
		{
			Globals.m_PrimaryObjective = m_ElevatorAccess;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna))
		{
			Globals.m_PrimaryObjective = m_StreetAccess;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception))
		{
			Object.Destroy(m_ReceptionistConversation);
		}
	}

	private void AcquireKey()
	{
		Globals.m_PrimaryObjective = m_ElevatorAccess;
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception, true);
	}
}
