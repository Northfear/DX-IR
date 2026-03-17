using UnityEngine;

public class StartPanamaLimb : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_FrontDesk;

	public GameObject m_FrontDeskConversation;

	public Transform m_Exit;

	public Transform m_Camila;

	public GameObject m_CamilaConversation;

	public Transform m_CamilaComplete;

	public GameObject m_CamilaConversationComplete;

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
		m_CamilaConversation.SetActiveRecursively(false);
		m_CamilaConversationComplete.SetActiveRecursively(false);
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted))
		{
			Debug.Log("Story Cheat: Configure missions starting at Panama LIMB.");
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDoctorMissionComplete, true);
			Globals.SetMissionStatus(1, MissionStatus.Completed_Success);
			Globals.SetSubMissionStatus(1, 0, MissionStatus.Completed_Success);
			Globals.SetMissionStatus(2, MissionStatus.Acquired);
			Globals.SetSubMissionStatus(2, 0, MissionStatus.Acquired);
			Globals.TrackMission(2, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb))
		{
			Globals.SetMissionTransforms(2, new Transform[1] { m_FrontDesk });
			m_CamilaConversation.SetActiveRecursively(false);
		}
		else if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete))
		{
			Object.Destroy(m_FrontDeskConversation);
			m_CamilaConversationComplete.SetActiveRecursively(true);
			Globals.SetMissionTransforms(2, new Transform[1] { m_CamilaComplete });
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
			Object.Destroy(m_FrontDeskConversation);
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

	private void AccessCamila()
	{
		m_CamilaConversation.SetActiveRecursively(true);
		Globals.SetMissionTransforms(2, new Transform[1] { m_Camila });
	}

	private void AcceptRizeolMission()
	{
		Globals.SetMissionTransforms(2, new Transform[1] { m_Exit });
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb, true);
		Globals.SetSubMissionStatus(2, 0, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(2, 1, MissionStatus.Acquired);
		Globals.SetSubMissionStatus(2, 2, MissionStatus.Acquired);
	}

	private void RiezolMissionComplete()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete, true);
		Globals.SetMissionStatus(2, MissionStatus.Completed_Success);
		Globals.SetSubMissionStatus(2, 4, MissionStatus.Completed_Success);
		Globals.SetMissionStatus(3, MissionStatus.Acquired);
		Globals.SetSubMissionStatus(3, 0, MissionStatus.Acquired);
		Globals.SetMissionTransforms(3, new Transform[1] { m_Exit });
		Globals.TrackMission(3, true);
	}
}
