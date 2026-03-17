using UnityEngine;

public class StartPanamaDrugEntry : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_GarageAccess;

	public Transform m_LabAccess;

	public Transform m_FrontDoor;

	public Transform m_BackDoor;

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
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra))
		{
			if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_GarageAccess });
			}
			else
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_FrontDoor });
			}
			if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaKilledGangLeader))
			{
				Globals.SetMissionTransforms(5, new Transform[1] { m_LabAccess });
			}
		}
		else
		{
			Globals.SetMissionTransforms(2, new Transform[1] { m_FrontDoor });
		}
		Globals.SetMissionTransforms(1, new Transform[1] { m_FrontDoor });
		Globals.SetMissionTransforms(3, new Transform[1] { m_FrontDoor });
	}
}
