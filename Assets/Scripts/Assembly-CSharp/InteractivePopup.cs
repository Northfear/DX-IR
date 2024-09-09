using UnityEngine;

public class InteractivePopup
{
	public enum PopupType
	{
		Normal = 0,
		Takedown = 1,
		HeavyObject = 2
	}

	public PopupInfo m_NormalPopup = new PopupInfo();

	public PopupInfo m_TakedownPopup1 = new PopupInfo();

	public PopupInfo m_TakedownPopup2 = new PopupInfo();

	public PopupType m_PopupType;

	public PackedSprite m_TooHeavyIcon;

	public InteractiveObject_Base m_InteractiveObject;

	public void Enable(bool tf)
	{
		float x = 0f;
		bool flag = m_PopupType == PopupType.Takedown;
		bool flag2 = m_PopupType == PopupType.HeavyObject;
		m_NormalPopup.Enable(!flag && tf);
		m_TakedownPopup1.Enable(flag && tf);
		m_TakedownPopup2.Enable(flag && tf);
		m_TooHeavyIcon.Hide(!flag2 || !tf);
		if (!flag2)
		{
			m_NormalPopup.m_PopupBackground.width = 2.681557f;
			m_NormalPopup.m_PopupBackground.width = 3.18f;
		}
		else
		{
			x = 0.3278348f;
		}
		Vector3 localPosition = m_NormalPopup.m_PopupBackground.transform.localPosition;
		localPosition.x = x;
		m_NormalPopup.m_PopupBackground.transform.localPosition = localPosition;
	}

	public void SetText(InteractiveObject_Base obj)
	{
		switch (m_PopupType)
		{
		case PopupType.Normal:
		case PopupType.HeavyObject:
			m_NormalPopup.m_PopupText.Text = obj.m_PopupString;
			break;
		case PopupType.Takedown:
		{
			InteractiveObject_Takedown interactiveObject_Takedown = (InteractiveObject_Takedown)obj;
			m_TakedownPopup1.SetText(interactiveObject_Takedown.m_PopupString);
			m_TakedownPopup2.SetText(interactiveObject_Takedown.m_PopupString2);
			break;
		}
		}
	}

	public void SetPosition(Vector3 position)
	{
		switch (m_PopupType)
		{
		case PopupType.Normal:
		case PopupType.HeavyObject:
		{
			Vector3 zero2 = Vector3.zero;
			zero2.y = 0f - m_NormalPopup.m_PopupBackground.ImageSize.y;
			m_NormalPopup.SetPosition(position, zero2);
			break;
		}
		case PopupType.Takedown:
		{
			Vector3 zero = Vector3.zero;
			zero.x = (0f - m_TakedownPopup1.m_PopupBackground.width) / Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel;
			m_TakedownPopup1.SetPosition(position, zero);
			zero.x = m_TakedownPopup2.m_PopupBackground.width / Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel;
			m_TakedownPopup2.SetPosition(position, zero);
			break;
		}
		}
	}
}
