using UnityEngine;

public class StartNYCLimb : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_StreetAccess;

	public Transform m_IsaacLocation;

	public GameObject m_IsaacConversation;

	public GameObject m_PostOperationGroup;

	public GameObject m_OperationTrigger;

	public GameObject m_OperationCamera;

	public Transform m_HidePlayerPosition;

	public Transform m_ShowPlayerPosition;

	private bool m_OperationStarted;

	private float m_OperationTimer;

	private bool m_ResetCameraPosition = true;

	private float m_ResetCameraPositionTimer = 3f;

	private bool m_AnnaIntroComm;

	private float m_AnnaIntroCommTimer = 0.5f;

	private bool m_IsaacExplainsComm;

	private float m_IsaacExplainsCommTimer = 11.5f;

	private bool m_AnnaWrapsComm;

	private float m_AnnaWrapsCommTimer = 32f;

	private bool m_EndOperationSeq;

	private float m_EndOperationSeqTimer = 11.5f;

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
		m_IsaacConversation.SetActiveRecursively(false);
		m_PostOperationGroup.SetActiveRecursively(false);
		m_OperationTrigger.active = false;
		m_OperationCamera.active = false;
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedHotelReception, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedIsaac))
		{
			m_IsaacConversation.SetActiveRecursively(true);
			Globals.m_PrimaryObjective = m_IsaacLocation;
		}
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedAnna) || Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedIsaac))
		{
			Globals.m_PrimaryObjective = m_StreetAccess;
		}
	}

	private void Update()
	{
		if (!m_OperationStarted)
		{
			return;
		}
		m_OperationTimer += Time.deltaTime;
		if (m_ResetCameraPosition)
		{
			m_ResetCameraPositionTimer -= Time.deltaTime;
			if (m_ResetCameraPositionTimer <= 0f)
			{
				m_OperationCamera.transform.localPosition = Vector3.zero;
				m_ResetCameraPosition = false;
				m_AnnaIntroComm = true;
			}
		}
		if (m_AnnaIntroComm)
		{
			m_AnnaIntroCommTimer -= Time.deltaTime;
			if (m_AnnaIntroCommTimer <= 0f)
			{
				CommLinkDialog.PlayDialog(0);
				m_AnnaIntroComm = false;
				m_IsaacExplainsComm = true;
			}
		}
		if (m_IsaacExplainsComm)
		{
			m_IsaacExplainsCommTimer -= Time.deltaTime;
			if (m_IsaacExplainsCommTimer <= 0f)
			{
				CommLinkDialog.PlayDialog(1);
				m_IsaacExplainsComm = false;
				m_AnnaWrapsComm = true;
			}
		}
		if (m_AnnaWrapsComm)
		{
			m_AnnaWrapsCommTimer -= Time.deltaTime;
			if (m_AnnaWrapsCommTimer <= 0f)
			{
				CommLinkDialog.PlayDialog(2);
				m_AnnaWrapsComm = false;
				m_EndOperationSeq = true;
			}
		}
		if (m_EndOperationSeq)
		{
			m_EndOperationSeqTimer -= Time.deltaTime;
			if (m_EndOperationSeqTimer <= 0f)
			{
				m_EndOperationSeq = false;
				m_OperationStarted = false;
				Globals.m_PlayerController.transform.position = m_ShowPlayerPosition.position;
				Object.Destroy(m_PostOperationGroup);
				Globals.m_PrimaryObjective = m_StreetAccess;
				Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCVisitedIsaac, true);
				Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.NYCImplantWasRemoved, true);
			}
		}
	}

	private void OperationReady()
	{
		m_OperationTrigger.active = true;
		Globals.m_PrimaryObjective = m_OperationTrigger.transform;
	}

	private void StartOperation()
	{
		Globals.m_PlayerController.WeaponHolster(true);
		m_OperationStarted = true;
		m_PostOperationGroup.SetActiveRecursively(true);
		Object.Destroy(m_IsaacConversation);
		m_OperationCamera.transform.localPosition = new Vector3(0f, 999f, 0f);
		Globals.m_PlayerController.transform.position = m_HidePlayerPosition.position;
	}
}
