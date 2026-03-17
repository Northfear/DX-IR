using UnityEngine;

public class StartPanamaSlums : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_DoctorDoor;

	public Transform m_BoardwalkAccess;

	public GameObject m_LeaveDocTrigger;

	public Transform m_DrugDenDoor;

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
			Globals.SetMissionTransforms(1, new Transform[1] { m_DoctorDoor });
			Globals.TrackMission(1, true);
		}
		else
		{
			if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_BoardwalkAccess });
			}
			if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_DrugDenDoor });
			}
			if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete))
			{
				Globals.SetMissionTransforms(2, new Transform[1] { m_BoardwalkAccess });
			}
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb))
		{
			m_LeaveDocTrigger.SetActiveRecursively(true);
		}
		else
		{
			Object.Destroy(m_LeaveDocTrigger);
		}
		Globals.SetMissionTransforms(5, new Transform[1] { m_DrugDenDoor });
	}

	private void Update()
	{
		if (m_ForceHolster)
		{
			Globals.m_PlayerController.WeaponHolster(true);
			m_ForceHolster = false;
		}
	}
}
