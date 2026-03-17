using UnityEngine;

public class StartUndergroundDoc : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Doctor;

	public Transform m_Exit;

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
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor))
		{
			Globals.SetMissionTransforms(1, new Transform[1] { m_Doctor });
		}
		else
		{
			Globals.SetMissionTransforms(2, new Transform[1] { m_Exit });
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

	private void VisitDoctor()
	{
		Debug.Log("Visit doctor quest complete.\n+500 experience points!");
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
		Globals.SetMissionStatus(1, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(1, 0, MissionStatus.Completed_Success);
		Globals.SetMissionStatus(2, MissionStatus.Acquired);
		Globals.SetSubMissionStatus(2, 0, MissionStatus.Acquired);
		Globals.SetMissionTransforms(2, new Transform[1] { m_Exit });
		Globals.TrackMission(2, true);
	}
}
