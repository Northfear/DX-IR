using UnityEngine;

public class CoverFlipVolume : MonoBehaviour
{
	[HideInInspector]
	public Vector3 m_Direction;

	[HideInInspector]
	public float m_Distance;

	private bool m_PlayerInside;

	private void Start()
	{
		Collider component = GetComponent<Collider>();
		if (component.bounds.size.x > component.bounds.size.z)
		{
			m_Direction = new Vector3(1f, 0f, 0f);
			m_Distance = component.bounds.size.x;
		}
		else
		{
			m_Direction = new Vector3(0f, 0f, 1f);
			m_Distance = component.bounds.size.z;
		}
		m_Direction = base.transform.rotation * m_Direction;
		m_Direction.Normalize();
	}

	private void Update()
	{
		if (!m_PlayerInside)
		{
			return;
		}
		float f = Vector3.Dot(m_Direction, Globals.m_PlayerController.transform.forward);
		if (Mathf.Abs(f) < 0.05f && Globals.m_PlayerController.m_CoverState != 0 && Globals.m_PlayerController.m_CoverEdge != 0)
		{
			Vector3 lhs = base.transform.position - Globals.m_PlayerController.transform.position;
			float num = Vector3.Dot(lhs, Globals.m_PlayerController.transform.right);
			if (num > 0f && (Globals.m_PlayerController.m_CoverEdge == PlayerController.CoverEdge.Right || Globals.m_PlayerController.m_CoverEdge == PlayerController.CoverEdge.Both))
			{
				Globals.m_HUD.TurnOnCoverFlipButton(true, MainHUD.CoverFlipButtonSide.Right);
			}
			else if (num < 0f && (Globals.m_PlayerController.m_CoverEdge == PlayerController.CoverEdge.Left || Globals.m_PlayerController.m_CoverEdge == PlayerController.CoverEdge.Both))
			{
				Globals.m_HUD.TurnOnCoverFlipButton(true, MainHUD.CoverFlipButtonSide.Left);
			}
			Globals.m_PlayerController.m_CoverFlipVolume = this;
		}
		else
		{
			Globals.m_HUD.TurnOnCoverFlipButton(false, MainHUD.CoverFlipButtonSide.Left);
			Globals.m_HUD.TurnOnCoverFlipButton(false, MainHUD.CoverFlipButtonSide.Right);
			Globals.m_PlayerController.m_CoverFlipVolume = null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			m_PlayerInside = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			m_PlayerInside = false;
			Globals.m_HUD.TurnOnCoverFlipButton(false, MainHUD.CoverFlipButtonSide.Left);
			Globals.m_HUD.TurnOnCoverFlipButton(false, MainHUD.CoverFlipButtonSide.Right);
			Globals.m_PlayerController.m_CoverFlipVolume = null;
		}
	}
}
