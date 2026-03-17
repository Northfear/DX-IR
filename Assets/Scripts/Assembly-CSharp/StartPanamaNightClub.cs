using UnityEngine;

public class StartPanamaNightClub : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	private int m_FogIndex;

	private int m_FogNextIndex;

	private float m_FogLerp;

	public float m_FogChangeRate = 0.5f;

	public Color[] m_FogColors;

	public bool m_EnableFog;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Exit;

	public Transform m_Kaspar;

	public GameObject m_KasparIntroConversation;

	public Transform m_KasparFinal;

	public GameObject m_KasparFinalConversation;

	private bool m_ForceHolster = true;

	private void Awake()
	{
		if (GameManager.m_This != null)
		{
			GameManager.m_This.m_CurrentLocation = Location.Panama;
		}
		RenderSettings.ambientLight = m_AmbientColor;
		if (m_FogColors == null || m_FogColors.Length <= 0)
		{
			m_FogColors = new Color[1];
			m_FogColors[0] = Color.gray;
		}
		m_FogIndex = Random.Range(0, m_FogColors.Length);
		m_FogNextIndex = Random.Range(0, m_FogColors.Length);
		if (m_FogNextIndex == m_FogIndex && m_FogColors.Length > 1)
		{
			m_FogNextIndex = (m_FogNextIndex + Random.Range(1, m_FogColors.Length - 1)) % m_FogColors.Length;
		}
		RenderSettings.fogColor = m_FogColors[m_FogIndex];
		RenderSettings.fog = m_EnableFog;
		RenderSettings.fogMode = m_FogMode;
		RenderSettings.fogDensity = m_FogDensity;
		RenderSettings.fogStartDistance = m_FogStartDistance;
		RenderSettings.fogEndDistance = m_FogEndDistance;
	}

	private void Start()
	{
		m_KasparIntroConversation.SetActiveRecursively(false);
		m_KasparFinalConversation.SetActiveRecursively(false);
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted))
		{
			Debug.Log("Progression Cheat: Advance to Alex meeting.");
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDoctorMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete, true);
			Globals.SetMissionStatus(1, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(1, 0, MissionStatus.Completed_Success);
			Globals.SetMissionStatus(2, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 0, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 1, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 2, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(2, 3, MissionStatus.Completed_Fail);
			Globals.SetSubMissionStatus(2, 4, MissionStatus.Completed_Success);
			Globals.SetMissionStatus(3, MissionStatus.Acquired);
			Globals.SetSubMissionStatus(3, 0, MissionStatus.Acquired);
			Globals.TrackMission(3, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedHavok))
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_Kaspar });
			m_KasparIntroConversation.SetActiveRecursively(true);
		}
		else if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPFound) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPMissionComplete))
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_KasparFinal });
			m_KasparFinalConversation.SetActiveRecursively(true);
		}
		else
		{
			Globals.SetMissionTransforms(3, new Transform[1] { m_Exit });
		}
		Globals.SetMissionTransforms(1, new Transform[1] { m_Exit });
		Globals.SetMissionTransforms(2, new Transform[1] { m_Exit });
		Globals.SetMissionTransforms(5, new Transform[1] { m_Exit });
	}

	private void Update()
	{
		if (m_FogColors.Length > 1)
		{
			m_FogLerp += m_FogChangeRate * Time.deltaTime;
			if (m_FogLerp >= 1f)
			{
				while (m_FogLerp >= 1f)
				{
					m_FogLerp -= 1f;
				}
				m_FogIndex = m_FogNextIndex;
				m_FogNextIndex = Random.Range(0, m_FogColors.Length);
				if (m_FogNextIndex == m_FogIndex)
				{
					m_FogNextIndex = (m_FogNextIndex + Random.Range(1, m_FogColors.Length - 1)) % m_FogColors.Length;
				}
			}
			RenderSettings.fogColor = Color.Lerp(m_FogColors[m_FogIndex], m_FogColors[m_FogNextIndex], m_FogLerp);
		}
		if (m_ForceHolster)
		{
			Globals.m_PlayerController.WeaponHolster(true);
			m_ForceHolster = false;
		}
	}

	private void KasparInvestigate()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedHavok, true);
		Globals.SetMissionTransforms(3, new Transform[1] { m_Exit });
		Globals.SetSubMissionStatus(3, 0, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(3, 1, MissionStatus.Acquired);
	}

	private void AcceptBelltowerMission()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPMissionComplete, true);
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionStarted, true);
		Globals.SetMissionTransforms(3, new Transform[1] { m_Exit });
		Globals.SetSubMissionStatus(3, 2, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(3, 3, MissionStatus.Acquired);
	}
}
