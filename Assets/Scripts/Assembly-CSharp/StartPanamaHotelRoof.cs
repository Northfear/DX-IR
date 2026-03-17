using UnityEngine;

public class StartPanamaHotelRoof : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	public Transform m_Exit;

	public Transform m_VTOLAccess;

	public GameObject m_VTOLGroup;

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
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionComplete))
		{
			m_VTOLGroup.SetActiveRecursively(true);
			Globals.SetMissionTransforms(3, new Transform[1] { m_VTOLAccess });
		}
		else
		{
			m_VTOLGroup.SetActiveRecursively(false);
			Globals.SetMissionTransforms(3, new Transform[1] { m_Exit });
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
}
