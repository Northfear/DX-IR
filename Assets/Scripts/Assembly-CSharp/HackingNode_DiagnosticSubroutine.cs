using UnityEngine;

public class HackingNode_DiagnosticSubroutine : HackingNode_Base
{
	public int m_cpuCycles;

	[HideInInspector]
	public PackedSprite m_SubRoutineSpam;

	private static Color m_Faded = new Color(1f, 1f, 1f, 0f);

	private static Color m_UnFaded = new Color(1f, 1f, 1f, 1f);

	public override void Setup()
	{
		m_name = "Diagnostic Sub-Routine";
		m_type = HackingNodeType.DiagnosticSubroutine;
	}

	public override void CaptureNode()
	{
		HackingSystem.m_this.CaptureSubRoutine();
	}

	public void ActivateSpam()
	{
		if ((bool)Globals.m_HackingGlobals.m_SubRoutineSpamPrefab && !m_SubRoutineSpam)
		{
			GameObject gameObject = Object.Instantiate(Globals.m_HackingGlobals.m_SubRoutineSpamPrefab) as GameObject;
			m_SubRoutineSpam = gameObject.GetComponent<PackedSprite>();
			m_SubRoutineSpam.transform.parent = base.gameObject.transform;
			m_SubRoutineSpam.transform.localPosition = Vector3.zero;
			m_SubRoutineSpam.Color = m_Faded;
		}
	}

	public override void Start()
	{
	}

	public override void Update()
	{
		if ((bool)m_SubRoutineSpam && m_SubRoutineSpam.Color.a < 1f)
		{
			m_SubRoutineSpam.Color = Color.Lerp(m_Faded, m_UnFaded, m_SubRoutineSpam.Color.a + Time.deltaTime);
		}
	}
}
