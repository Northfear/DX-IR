using UnityEngine;

public class PopupInfo
{
	public UIButton m_PopupBackground;

	public SpriteText m_PopupText;

	public void Enable(bool tf)
	{
		if ((bool)m_PopupBackground)
		{
			m_PopupBackground.Hide(!tf);
		}
		if ((bool)m_PopupText)
		{
			m_PopupText.Hide(!tf);
		}
	}

	public void SetText(string text)
	{
		m_PopupText.Text = text;
	}

	public void SetPosition(Vector3 position, Vector3 offset)
	{
		float num = position.x + offset.x;
		float num2 = position.y + offset.y;
		num -= (float)Screen.width * 0.5f;
		num2 = (float)Screen.height * 0.5f - num2;
		Vector3 localPosition = new Vector3(num * m_PopupBackground.worldUnitsPerScreenPixel, num2 * m_PopupBackground.worldUnitsPerScreenPixel, position.z + offset.z);
		m_PopupBackground.transform.parent.localPosition = localPosition;
	}
}
