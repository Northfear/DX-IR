public class HackingNode_DiagnosticSubroutine : HackingNode_Base
{
	public int m_cpuCycles;

	public override void Setup()
	{
		m_name = "Diagnostic Sub-Routine";
		m_type = HackingNodeType.DiagnosticSubroutine;
	}

	public override void CaptureNode()
	{
		HackingSystem.m_this.CaptureSubRoutine();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
