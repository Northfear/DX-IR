public class HackingTerminal_NumberPad : HackingTerminal
{
	public static HackingTerminal_NumberPad m_this;

	public int m_passcodeMaxLength = 4;

	private void Awake()
	{
		if (m_this == null)
		{
			m_this = this;
		}
	}

	public override void PressButton(string buttonKey)
	{
		if (m_currentPasscode.Length != m_passcodeMaxLength)
		{
			base.PressButton(buttonKey);
			if (m_currentPasscode == m_info.m_correctPasscode && m_correctPasscodeCallback != null)
			{
				m_correctPasscodeCallback();
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
