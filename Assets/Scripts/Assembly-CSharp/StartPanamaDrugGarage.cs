using UnityEngine;

public class StartPanamaDrugGarage : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_LabAccess;

	public Transform m_EntryAccess;

	public Transform m_ReizolShipment;

	public GameObject m_RiezolShipmentGroup;

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
				Globals.SetMissionTransforms(2, new Transform[1] { m_ReizolShipment });
			}
			else
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_EntryAccess });
			}
			if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaKilledGangLeader))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_LabAccess });
			}
		}
		else
		{
			Globals.SetMissionTransforms(2, new Transform[1] { m_EntryAccess });
		}
	}

	private void BombPlanted()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol, true);
		Globals.SetMissionTransforms(2, new Transform[1] { m_EntryAccess });
		Globals.SetSubMissionStatus(2, 2, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(2, 3, MissionStatus.Completed_Fail);
		Globals.SetSubMissionStatus(2, 4, MissionStatus.Acquired);
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaMadeDealWithGang) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaKilledGangLeader))
		{
			Globals.m_SecondaryObjective = m_LabAccess;
		}
	}
}
