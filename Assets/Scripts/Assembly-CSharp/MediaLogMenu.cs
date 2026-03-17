using System.Collections.Generic;
using UnityEngine;

public class MediaLogMenu : MonoBehaviour
{
	public static MediaLogMenu m_This;

	private int m_CurrentLocation;

	public SpriteText m_LocationText;

	private int m_CurrentListItem = -1;

	public MediaLogContainer m_LogEntryPrefab;

	public UIScrollList m_ScrollList;

	public SpriteText m_Subject;

	public SpriteText m_From;

	public SpriteText m_To;

	public UIScrollList m_BodyList;

	public Color m_SelectedEntry = new Color(0.929f, 0.655f, 0.137f, 1f);

	public Color m_UnselectedEntry = new Color(0.5f, 0.5f, 0.5f, 1f);

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	private void LateUpdate()
	{
		Color brightHUD = Globals.m_This.m_BrightHUD;
		brightHUD.a = m_ScrollList.slider.GetKnob().Color.a;
		m_ScrollList.slider.GetKnob().SetColor(brightHUD);
		m_BodyList.slider.GetKnob().SetColor(brightHUD);
	}

	public static void MediaLogOpening()
	{
		if (!(m_This == null))
		{
			m_This.m_CurrentLocation = 0;
			if (GameManager.m_This.m_CurrentLocation > Location.None && GameManager.m_This.m_CurrentLocation < Location.Total)
			{
				m_This.m_CurrentLocation = (int)GameManager.m_This.m_CurrentLocation;
			}
			m_This.SetLocationText();
			m_This.PopulateList();
			m_This.m_CurrentListItem = ((m_This.m_ScrollList.Count <= 0) ? (-1) : 0);
			m_This.DisplayLog();
			m_This.ColorListings();
		}
	}

	public void CycleLocation()
	{
		if (GameManager.m_This.m_CurrentLocation != Location.None)
		{
			m_CurrentLocation = (m_CurrentLocation + 1) % (int)(GameManager.m_This.m_CurrentLocation + 1);
		}
		SetLocationText();
		PopulateList();
		m_CurrentListItem = ((m_This.m_ScrollList.Count <= 0) ? (-1) : 0);
		DisplayLog();
		ColorListings();
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	private void SetLocationText()
	{
		if (m_LocationText != null && m_CurrentLocation >= 0 && m_CurrentLocation < 6 && m_CurrentLocation < Globals.m_LocationNames.Length)
		{
			m_LocationText.Text = Globals.m_LocationNames[m_CurrentLocation];
		}
	}

	private void PopulateList()
	{
		if (m_LogEntryPrefab == null)
		{
			return;
		}
		if (m_CurrentLocation < 0 || m_CurrentLocation >= 6 || GameManager.m_This.m_MediaLogs[m_CurrentLocation].Count <= 0)
		{
			while (m_ScrollList.Count > 0)
			{
				m_ScrollList.RemoveItem(0, true);
			}
			m_ScrollList.slider.GetKnob().Hide(true);
			return;
		}
		LinkedList<MediaLog> linkedList = GameManager.m_This.m_MediaLogs[m_CurrentLocation];
		int count = linkedList.Count;
		while (m_ScrollList.Count > count)
		{
			m_ScrollList.RemoveItem(count, true);
		}
		while (m_ScrollList.Count < count)
		{
			m_ScrollList.CreateItem(m_LogEntryPrefab.gameObject);
		}
		LinkedListNode<MediaLog> linkedListNode = linkedList.First;
		for (int i = 0; i < count; i++)
		{
			MediaLogContainer component = m_ScrollList.GetItem(i).gameObject.GetComponent<MediaLogContainer>();
			if (!(component == null) && linkedListNode != null)
			{
				component.m_Button.Data = i;
				component.m_Button.SetValueChangedDelegate(SelectionMade);
				component.m_Type.Text = ((linkedListNode.Value.m_MediaType != MediaType.EMail) ? "POCKET SECRETARY" : "EMAIL");
				component.m_NEW.Text = (linkedListNode.Value.m_Read ? string.Empty : "NEW");
				component.m_Subject.Text = linkedListNode.Value.m_Subject;
				component.m_From.Text = linkedListNode.Value.m_From;
			}
			if (linkedListNode != null)
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		m_ScrollList.ScrollToItem(0, 0f);
		UISlider slider = m_ScrollList.slider;
		UIScrollKnob knob = slider.GetKnob();
		UIListItemContainer uIListItemContainer = m_ScrollList.GetItem(0) as UIListItemContainer;
		if (uIListItemContainer == null)
		{
			knob.Hide(true);
			return;
		}
		float num = (float)m_ScrollList.Count * (uIListItemContainer.TopLeftEdge.y - uIListItemContainer.BottomRightEdge.y) + (float)(m_ScrollList.Count - 1) * m_ScrollList.itemSpacing;
		float num2 = Mathf.Max(num / m_ScrollList.viewableArea.y, 0f);
		knob.Hide(num2 <= 1f);
		if (!knob.IsHidden())
		{
			knob.SetSize(Mathf.Max(slider.width * (1f / num2), 2f), knob.height);
			slider.stopKnobFromEdge = knob.width * 0.5f;
			slider.SetSize(slider.width, slider.height);
		}
	}

	private void SelectionMade(IUIObject obj)
	{
		m_CurrentListItem = (int)obj.Data;
		if (m_CurrentLocation > -1 && m_CurrentLocation < 6 && m_CurrentListItem >= 0)
		{
			int num = 0;
			LinkedListNode<MediaLog> linkedListNode = GameManager.m_This.m_MediaLogs[m_CurrentLocation].First;
			while (linkedListNode != null)
			{
				if (num == m_CurrentListItem)
				{
					linkedListNode.Value.m_Read = true;
					break;
				}
				linkedListNode = linkedListNode.Next;
				num++;
			}
		}
		DisplayLog();
		ColorListings();
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
	}

	private void DisplayLog()
	{
		if (m_CurrentLocation > -1 && m_CurrentLocation < 6 && m_CurrentListItem >= 0)
		{
			int num = 0;
			LinkedListNode<MediaLog> linkedListNode = GameManager.m_This.m_MediaLogs[m_CurrentLocation].First;
			while (linkedListNode != null)
			{
				if (num == m_CurrentListItem)
				{
					m_Subject.Hide(false);
					m_From.Hide(false);
					m_To.Hide(false);
					m_Subject.Text = linkedListNode.Value.m_Subject;
					m_From.Text = "[#FFFFFF]FROM: [#EDA723]" + LocalizationManager.LocalizeString(linkedListNode.Value.m_From);
					m_To.Text = "[#FFFFFF]TO: [#EDA723]" + LocalizationManager.LocalizeString(linkedListNode.Value.m_To);
					SetBodyText(false, linkedListNode.Value.m_Body);
					return;
				}
				linkedListNode = linkedListNode.Next;
				num++;
			}
		}
		m_Subject.Hide(true);
		m_From.Hide(true);
		m_To.Hide(true);
		SetBodyText(true, string.Empty);
	}

	private void ColorListings()
	{
		if (m_CurrentLocation <= -1 || m_CurrentLocation >= 6)
		{
			return;
		}
		LinkedListNode<MediaLog> linkedListNode = GameManager.m_This.m_MediaLogs[m_CurrentLocation].First;
		for (int i = 0; i < m_ScrollList.Count; i++)
		{
			MediaLogContainer component = m_ScrollList.GetItem(i).gameObject.GetComponent<MediaLogContainer>();
			component.m_Button.SetColor((i != m_CurrentListItem) ? m_UnselectedEntry : m_SelectedEntry);
			if (linkedListNode != null)
			{
				component.m_NEW.Text = (linkedListNode.Value.m_Read ? string.Empty : "NEW");
			}
			if (linkedListNode != null)
			{
				linkedListNode = linkedListNode.Next;
			}
		}
	}

	private void SetBodyText(bool hide, string text = "")
	{
		UIListItemContainer uIListItemContainer = m_BodyList.GetItem(0) as UIListItemContainer;
		if (uIListItemContainer == null)
		{
			return;
		}
		if (hide)
		{
			uIListItemContainer.Text = string.Empty;
			m_BodyList.slider.GetKnob().Hide(true);
			return;
		}
		uIListItemContainer.Text = text;
		UISlider slider = m_BodyList.slider;
		UIScrollKnob knob = slider.GetKnob();
		float num = (uIListItemContainer.TopLeftEdge.y - uIListItemContainer.BottomRightEdge.y) / m_BodyList.viewableArea.y;
		knob.Hide(num <= 1f);
		if (!knob.IsHidden())
		{
			knob.SetSize(Mathf.Max(slider.width * (1f / num), 2f), knob.height);
			slider.stopKnobFromEdge = knob.width * 0.5f;
			slider.SetSize(slider.width, slider.height);
		}
	}
}
