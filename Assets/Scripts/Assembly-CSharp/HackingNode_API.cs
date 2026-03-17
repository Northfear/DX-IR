public class HackingNode_API : HackingNode_Base
{
	public APINodeType m_APINodeType;

	public SpriteText m_TypeText;

	public override void Setup()
	{
		m_name = "API";
		m_type = HackingNodeType.API;
	}

	public override void CaptureNode()
	{
		switch (m_APINodeType)
		{
		case APINodeType.Clearance:
			HackingSystem.m_this.ActivateClearance();
			break;
		case APINodeType.Spam:
			HackingSystem.m_this.ActivateSpam();
			break;
		}
	}

	public override void Start()
	{
		if ((bool)m_TypeText)
		{
			if (m_APINodeType == APINodeType.Clearance)
			{
				m_TypeText.Text = "CLEARANCE";
			}
			else if (m_APINodeType == APINodeType.Spam)
			{
				m_TypeText.Text = "SPAM";
			}
		}
	}

	public override void Update()
	{
	}
}
