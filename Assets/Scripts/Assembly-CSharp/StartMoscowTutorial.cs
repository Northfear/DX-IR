using UnityEngine;

public class StartMoscowTutorial : MonoBehaviour
{
	public Color m_AmbientColor = Color.gray;

	public bool m_EnableFog;

	public Color m_FogColor = Color.black;

	public FogMode m_FogMode = FogMode.Linear;

	public float m_FogDensity = 0.01f;

	public float m_FogStartDistance;

	public float m_FogEndDistance = 100f;

	private float m_TutorialTimer;

	private bool m_HolsterWeapon = true;

	private float m_HolsterWeaponTime = 1f;

	private bool m_MovementTutorial = true;

	private float m_MovementTutorialTime = 8f;

	private bool m_CoverTutorial;

	private float m_CoverTutorialTime = 1f;

	public GameObject m_CoverHighlight;

	private bool m_StartSnipingTutorial;

	public bool[] m_SnipingTutorial;

	public float[] m_SnipingTutorialTimer;

	private bool m_HardestyComm;

	private float m_HardestyTimer = 4f;

	private bool m_TapTargetTutorial;

	private float m_TapTargetTutorialTimer = 4f;

	public GameObject[] m_NamirStandin;

	public GameObject m_BarretStandin;

	public GameObject m_DeadBodyGroup;

	public GameObject[] m_GuardSpawners;

	private Enemy_Base[] m_GuardScripts;

	public GameObject m_EnemyBarrier;

	public AudioClip[] m_SniperSound;

	public AudioSource m_SniperSource;

	public GameObject m_FalseWall;

	private bool m_VaultingTutorial;

	private float m_VaultingTutorialTimer = 5f;

	private bool m_JammerObjective;

	private float m_JammerObjectiveTimer = 7f;

	public GameObject m_JammerObject;

	public GameObject m_JammerTriggerObject;

	public GameObject m_VentHatch;

	public GameObject m_VentTrigger;

	public GameObject m_LevelChangeTrigger;

	private void Awake()
	{
		if (GameManager.m_This != null)
		{
			GameManager.m_This.m_CurrentLocation = Location.Moscow;
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
		m_NamirStandin[0].SetActiveRecursively(true);
		m_NamirStandin[1].SetActiveRecursively(false);
		m_NamirStandin[2].SetActiveRecursively(false);
		m_BarretStandin.SetActiveRecursively(false);
		m_DeadBodyGroup.SetActiveRecursively(false);
		m_CoverHighlight.active = false;
		m_JammerObject.SetActiveRecursively(false);
		m_VentTrigger.active = false;
		m_JammerTriggerObject.active = false;
		Globals.SetMissionStatus(0, MissionStatus.Acquired);
		Globals.SetSubMissionStatus(0, 0, MissionStatus.Acquired);
		Globals.TrackMission(0, true);
	}

	private void Update()
	{
		m_TutorialTimer += Time.deltaTime;
		if (m_HolsterWeapon)
		{
			m_HolsterWeaponTime -= Time.deltaTime;
			if (m_HolsterWeaponTime <= 0f)
			{
				m_HolsterWeapon = false;
				Globals.m_PlayerController.m_WeaponScript.Holster();
				CommLinkDialog.PlayDialog(0);
			}
		}
		if (m_MovementTutorial && m_TutorialTimer >= m_MovementTutorialTime)
		{
			m_MovementTutorial = false;
			CommLinkDialog.PlayDialog(1);
			m_NamirStandin[0].SetActiveRecursively(false);
			m_NamirStandin[1].SetActiveRecursively(true);
			Globals.SetMissionTransforms(0, new Transform[1] { m_NamirStandin[1].transform });
		}
		if (m_CoverTutorial)
		{
			m_CoverTutorialTime -= Time.deltaTime;
			if (m_CoverTutorialTime <= 0f)
			{
				m_CoverTutorial = false;
			}
		}
		if (m_HardestyComm)
		{
			m_HardestyTimer -= Time.deltaTime;
			if (m_HardestyTimer <= 0f)
			{
				Globals.SetMissionTransforms(0, new Transform[1] { m_GuardScripts[2].transform });
				m_HardestyComm = false;
				CommLinkDialog.PlayDialog(4);
				m_TapTargetTutorial = true;
			}
		}
		if (m_TapTargetTutorial)
		{
			m_TapTargetTutorialTimer -= Time.deltaTime;
			if (m_TapTargetTutorialTimer <= 0f)
			{
				m_TapTargetTutorial = false;
				CommLinkDialog.PlayDialog(5);
			}
		}
		if (m_StartSnipingTutorial)
		{
			for (int num = 3; num > 0; num--)
			{
				if (m_SnipingTutorial[num - 1])
				{
					m_SnipingTutorialTimer[num - 1] -= Time.deltaTime;
					if (m_SnipingTutorialTimer[num - 1] <= 0f)
					{
						m_SniperSource.clip = m_SniperSound[num - 1];
						m_SniperSource.Play();
						m_GuardScripts[num - 1].TakeDamage(new DamageData(null, Vector3.zero, 999, DamageType.Normal, false));
						m_SnipingTutorial[num - 1] = false;
						if (num == 1)
						{
							m_StartSnipingTutorial = false;
							CommLinkDialog.PlayDialog(6);
							m_VaultingTutorial = true;
							Object.Destroy(m_EnemyBarrier);
							Object.Destroy(m_FalseWall);
						}
					}
				}
			}
		}
		if (m_VaultingTutorial)
		{
			m_VaultingTutorialTimer -= Time.deltaTime;
			if (m_VaultingTutorialTimer <= 0f)
			{
				m_VaultingTutorial = false;
				CommLinkDialog.PlayDialog(7);
				m_NamirStandin[1].SetActiveRecursively(false);
				m_DeadBodyGroup.SetActiveRecursively(true);
				Globals.SetMissionTransforms(0, new Transform[1] { m_DeadBodyGroup.transform });
			}
		}
		if (m_JammerObjective)
		{
			m_JammerObjectiveTimer -= Time.deltaTime;
			if (m_JammerObjectiveTimer <= 0f)
			{
				m_JammerObjective = false;
				CommLinkDialog.PlayDialog(10);
				m_JammerObject.SetActiveRecursively(true);
				Globals.SetMissionTransforms(0, new Transform[1] { m_JammerObject.transform });
			}
		}
	}

	private void SpawnGuards()
	{
		m_HardestyComm = true;
		int num = 3;
		m_GuardScripts = new Enemy_Base[num];
		m_SnipingTutorial = new bool[num];
		while (num > 0)
		{
			EnemySpawner component = m_GuardSpawners[num - 1].GetComponent<EnemySpawner>();
			m_GuardScripts[num - 1] = component.Spawn();
			m_StartSnipingTutorial = true;
			m_SnipingTutorial[num - 1] = true;
			num--;
		}
	}

	private void StartCoverTutorial()
	{
		m_CoverTutorial = true;
		m_CoverHighlight.active = true;
	}

	private void BodyFound()
	{
		m_NamirStandin[2].SetActiveRecursively(true);
		m_BarretStandin.SetActiveRecursively(true);
		Globals.SetMissionTransforms(0, new Transform[1] { m_NamirStandin[2].transform });
		Object.Destroy(m_DeadBodyGroup);
	}

	private void MeetBarrett()
	{
		m_JammerObjective = true;
	}

	private void JammerPlaced()
	{
		CommLinkDialog.PlayDialog(11);
		m_VentTrigger.active = true;
		Globals.SetMissionTransforms(0, new Transform[1] { m_VentHatch.transform });
	}

	private void VentOpen()
	{
		CommLinkDialog.PlayDialog(13);
		Globals.SetMissionTransforms(0, new Transform[1] { m_LevelChangeTrigger.transform });
	}
}
