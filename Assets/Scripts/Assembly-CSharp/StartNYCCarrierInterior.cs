using UnityEngine;

public class StartNYCCarrierInterior : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Exit;

	public Transform m_WidowLocation;

	public GameObject m_StairsToWidowTrigger;

	public GameObject m_StairsToWidowFinalTrigger;

	public GameObject m_StairsToExitTrigger;

	public GameObject m_WidowIntroductionConversation;

	public GameObject m_WidowFinalConversation;

	public GameObject m_JanusMonitor;

	public GameObject m_AnnaScreenCamera;

	public GameObject m_AnnaSamCamera;

	public GameObject m_SamObject;

	public GameObject m_BelltowerAttackGroup;

	private bool m_AnnaOpticsSeqStarted;

	private bool m_AnnaOpticsComm;

	private float m_AnnaOpticsCommTimer = 1.5f;

	private bool m_SamBreaksIn;

	private float m_SamBreaksInTimer = 11.5f;

	private bool m_EndAnnaSeq;

	private float m_EndAnnaSeqTimer = 7.5f;

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
		m_WidowIntroductionConversation.SetActiveRecursively(false);
		m_WidowFinalConversation.SetActiveRecursively(false);
		m_StairsToWidowTrigger.active = false;
		m_StairsToExitTrigger.active = false;
		m_StairsToWidowFinalTrigger.active = false;
		m_JanusMonitor.active = false;
		m_SamObject.SetActiveRecursively(false);
		m_AnnaScreenCamera.active = false;
		m_AnnaSamCamera.active = false;
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedIsaac, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHackers, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHaveUplink, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHackers))
		{
			m_WidowIntroductionConversation.SetActiveRecursively(true);
			m_StairsToWidowTrigger.active = true;
			Globals.m_PrimaryObjective = m_StairsToWidowTrigger.transform;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCPlantedUplink) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHackedBank))
		{
			m_WidowFinalConversation.SetActiveRecursively(true);
			m_JanusMonitor.active = true;
			m_StairsToWidowFinalTrigger.active = true;
			Globals.m_PrimaryObjective = m_StairsToWidowFinalTrigger.transform;
		}
	}

	private void Update()
	{
		if (!m_AnnaOpticsSeqStarted)
		{
			return;
		}
		if (m_AnnaOpticsComm)
		{
			m_AnnaOpticsCommTimer -= Time.deltaTime;
			if (m_AnnaOpticsCommTimer <= 0f)
			{
				m_AnnaOpticsComm = false;
				CommLinkDialog.PlayDialog(0);
				m_SamBreaksIn = true;
			}
		}
		if (m_SamBreaksIn)
		{
			m_SamBreaksInTimer -= Time.deltaTime;
			if (m_SamBreaksInTimer <= 0f)
			{
				m_SamBreaksIn = false;
				CommLinkDialog.PlayDialog(1);
				m_SamObject.SetActiveRecursively(true);
				m_EndAnnaSeq = true;
				m_AnnaScreenCamera.active = false;
				m_AnnaSamCamera.active = true;
			}
		}
		if (m_EndAnnaSeq)
		{
			m_EndAnnaSeqTimer -= Time.deltaTime;
			if (m_EndAnnaSeqTimer <= 0f)
			{
				m_EndAnnaSeq = false;
				m_AnnaSamCamera.active = false;
				CommLinkDialog.PlayDialog(2);
				m_BelltowerAttackGroup.SetActiveRecursively(true);
				Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHackedBank, true);
				Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCAnnaWasAbducted, true);
			}
		}
	}

	private void MeetWidow()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHackers, true);
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHaveUplink, true);
		m_StairsToExitTrigger.active = true;
		Globals.m_PrimaryObjective = m_StairsToExitTrigger.transform;
	}

	private void StartAnnaSeq()
	{
		Globals.m_PlayerController.WeaponHolster(true);
		m_AnnaOpticsSeqStarted = true;
		m_AnnaOpticsComm = true;
		m_AnnaScreenCamera.active = true;
		m_StairsToExitTrigger.active = true;
		Globals.m_PrimaryObjective = m_StairsToExitTrigger.transform;
	}
}
