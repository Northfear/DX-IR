using UnityEngine;

public class StartNYCBank : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Exit;

	public Transform m_UplinkLocation;

	public GameObject m_UplinkObject;

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
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHaveUplink) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink))
		{
			Globals.m_PrimaryObjective = m_UplinkLocation;
			return;
		}
		Globals.m_PrimaryObjective = m_Exit;
		Object.Destroy(m_UplinkObject);
	}

	private void UplinkPlaced()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink, true);
		Globals.m_PrimaryObjective = m_Exit;
	}
}
