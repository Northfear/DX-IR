using UnityEngine;

public class BlobShadowFix : MonoBehaviour
{
	private Transform m_OurTransform;

	public Transform m_Attachment;

	public float m_DesiredHeight = 1.5f;

	private float m_OriginalAttachmentHeight;

	private void Awake()
	{
		m_OurTransform = base.transform;
	}

	private void Start()
	{
		if (m_Attachment != null)
		{
			m_OriginalAttachmentHeight = m_Attachment.position.y;
		}
	}

	private void LateUpdate()
	{
		if (m_Attachment != null)
		{
			float num = m_OriginalAttachmentHeight - m_Attachment.position.y;
			m_OurTransform.position = m_Attachment.position + Vector3.up * (m_DesiredHeight + num);
		}
	}
}
