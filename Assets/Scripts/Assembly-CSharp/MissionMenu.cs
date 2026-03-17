using System.Collections.Generic;
using UnityEngine;

public class MissionMenu : MonoBehaviour
{
	public enum Category
	{
		Current = 0,
		Finished = 1
	}

	public enum SubIcon
	{
		InProgress = 0,
		Success = 1,
		Fail = 2
	}

	public static MissionMenu m_This;

	private Category m_Category;

	private List<int> m_ListedMissions = new List<int>();

	private int m_MissionIndex = -1;

	public UIScrollList m_MissionList;

	public UIScrollList m_DetailList;

	public UIRadioBtn m_CurrentButton;

	public UIRadioBtn m_FinishedButton;

	public UIListItemContainer m_PrimaryHeaderPrefab;

	public UIListItemContainer m_SideQuestHeaderPrefab;

	public MissionEntryContainer m_MissionEntryPrefab;

	public MissionDetailContainer m_MissionDetailPrefab;

	public SpriteText m_DetailMissionName;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		m_CurrentButton.Data = 0;
		m_CurrentButton.SetValueChangedDelegate(RadioPressed);
		m_FinishedButton.Data = 1;
		m_FinishedButton.SetValueChangedDelegate(RadioPressed);
	}

	public static void MissionMenuOpening()
	{
		if (!(m_This == null))
		{
			m_This.m_Category = Category.Current;
			m_This.m_CurrentButton.ManuallySetValue(true);
			m_This.GatherMissions();
			m_This.m_MissionIndex = ((m_This.m_ListedMissions.Count <= 0) ? (-1) : 0);
			m_This.PopulateMissionList();
			m_This.PopulateMissionDetails();
		}
	}

	private void LateUpdate()
	{
		Color brightHUD = Globals.m_This.m_BrightHUD;
		brightHUD.a = m_MissionList.slider.GetKnob().Color.a;
		m_MissionList.slider.GetKnob().SetColor(brightHUD);
		m_DetailList.slider.GetKnob().SetColor(brightHUD);
	}

	public void RadioPressed(IUIObject obj)
	{
		if (m_Category != (Category)(int)obj.Data)
		{
			m_Category = (Category)(int)obj.Data;
			m_This.GatherMissions();
			m_This.m_MissionIndex = ((m_This.m_ListedMissions.Count <= 0) ? (-1) : 0);
			m_This.PopulateMissionList();
			m_This.PopulateMissionDetails();
		}
	}

	public void ActivatePressed(IUIObject obj)
	{
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		if (m_Category == Category.Finished)
		{
			return;
		}
		int num = (int)obj.Data;
		int index = ((!Globals.m_Missions[m_ListedMissions[num]].m_Primary) ? (num + 2) : (num + 1));
		MissionEntryContainer component = m_MissionList.GetItem(index).gameObject.GetComponent<MissionEntryContainer>();
		if ((bool)component)
		{
			Globals.TrackMission(m_ListedMissions[num], !Globals.m_Missions[m_ListedMissions[num]].m_Tracked);
			if (Globals.m_Missions[m_ListedMissions[num]].m_Tracked)
			{
				component.m_ActiveBack.SetColor(Globals.m_This.m_BrightHUD);
				component.m_Active.SetColor(Color.black);
				component.m_Active.Text = "ACTIVE";
				component.m_ActivateText.Text = "INACTIVE";
			}
			else
			{
				component.m_ActiveBack.SetColor(Color.black);
				component.m_Active.SetColor(Color.gray);
				component.m_Active.Text = "INACTIVE";
				component.m_ActivateText.Text = "ACTIVE";
			}
		}
	}

	public void SelectionMade(IUIObject obj)
	{
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		if (m_MissionIndex == (int)obj.Data)
		{
			return;
		}
		int index;
		MissionEntryContainer component;
		if (m_MissionIndex >= 0)
		{
			index = ((!Globals.m_Missions[m_ListedMissions[m_MissionIndex]].m_Primary) ? (m_MissionIndex + 2) : (m_MissionIndex + 1));
			component = m_MissionList.GetItem(index).gameObject.GetComponent<MissionEntryContainer>();
			if ((bool)component)
			{
				component.m_Outline.transform.localScale = Vector3.zero;
			}
		}
		m_MissionIndex = (int)obj.Data;
		index = ((!Globals.m_Missions[m_ListedMissions[m_MissionIndex]].m_Primary) ? (m_MissionIndex + 2) : (m_MissionIndex + 1));
		component = m_MissionList.GetItem(index).gameObject.GetComponent<MissionEntryContainer>();
		if ((bool)component)
		{
			component.m_Outline.transform.localScale = Vector3.one;
		}
		PopulateMissionDetails();
		m_MissionList.ClipItems();
	}

	private void GatherMissions()
	{
		m_ListedMissions.Clear();
		for (int i = 0; i < 8; i++)
		{
			if (m_Category == Category.Current)
			{
				if (Globals.m_Missions[i].m_Location == GameManager.m_This.m_CurrentLocation && Globals.m_Missions[i].m_Status == MissionStatus.Acquired)
				{
					m_ListedMissions.Add(i);
				}
			}
			else if (Globals.m_Missions[i].m_Location <= GameManager.m_This.m_CurrentLocation && Globals.m_Missions[i].m_Status == MissionStatus.Completed_Success)
			{
				m_ListedMissions.Add(i);
			}
		}
	}

	private void PopulateMissionList()
	{
		while (m_MissionList.Count > 0)
		{
			m_MissionList.RemoveItem(0, true);
		}
		if (m_MissionEntryPrefab == null || m_PrimaryHeaderPrefab == null || m_SideQuestHeaderPrefab == null || m_ListedMissions == null || m_ListedMissions.Count <= 0)
		{
			m_MissionList.slider.GetKnob().Hide(true);
			return;
		}
		m_MissionList.CreateItem(m_PrimaryHeaderPrefab.gameObject);
		for (int i = 0; i < m_ListedMissions.Count; i++)
		{
			Mission mission = Globals.m_Missions[m_ListedMissions[i]];
			if (mission.m_Primary)
			{
				IUIListObject iUIListObject = m_MissionList.CreateItem(m_MissionEntryPrefab.gameObject);
				if (iUIListObject != null)
				{
					FillInMissionEntry(iUIListObject.gameObject.GetComponent<MissionEntryContainer>(), mission, i);
				}
			}
		}
		m_MissionList.CreateItem(m_SideQuestHeaderPrefab.gameObject);
		for (int j = 0; j < m_ListedMissions.Count; j++)
		{
			Mission mission2 = Globals.m_Missions[m_ListedMissions[j]];
			if (!mission2.m_Primary)
			{
				IUIListObject iUIListObject = m_MissionList.CreateItem(m_MissionEntryPrefab.gameObject);
				if (iUIListObject != null)
				{
					FillInMissionEntry(iUIListObject.gameObject.GetComponent<MissionEntryContainer>(), mission2, j);
				}
			}
		}
		m_MissionList.ScrollToItem(0, 0f);
		UISlider slider = m_MissionList.slider;
		UIScrollKnob knob = slider.GetKnob();
		UIListItemContainer uIListItemContainer = m_MissionList.GetItem(0) as UIListItemContainer;
		if (uIListItemContainer == null)
		{
			knob.Hide(true);
			return;
		}
		float num = (float)m_MissionList.Count * (uIListItemContainer.TopLeftEdge.y - uIListItemContainer.BottomRightEdge.y) + (float)(m_MissionList.Count - 1) * m_MissionList.itemSpacing;
		float num2 = Mathf.Max(num / m_MissionList.viewableArea.y, 0f);
		knob.Hide(num2 <= 1f);
		if (!knob.IsHidden())
		{
			knob.SetSize(Mathf.Max(slider.width * (1f / num2), 2f), knob.height);
			slider.stopKnobFromEdge = knob.width * 0.5f;
			slider.SetSize(slider.width, slider.height);
		}
	}

	private void FillInMissionEntry(MissionEntryContainer entry, Mission mission, int arrayIndex)
	{
		if (!(entry == null) && mission != null)
		{
			entry.m_ActivateButton.Data = arrayIndex;
			entry.m_ActivateButton.SetValueChangedDelegate(ActivatePressed);
			if (mission.m_Tracked)
			{
				entry.m_ActiveBack.SetColor(Globals.m_This.m_BrightHUD);
				entry.m_Active.SetColor(Color.black);
				entry.m_Active.Text = "ACTIVE";
				entry.m_ActivateText.Text = "INACTIVE";
			}
			else
			{
				entry.m_ActiveBack.SetColor(Color.black);
				entry.m_Active.SetColor(Color.gray);
				entry.m_Active.Text = "INACTIVE";
				entry.m_ActivateText.Text = "ACTIVE";
			}
			entry.m_Button.Data = arrayIndex;
			entry.m_Button.SetValueChangedDelegate(SelectionMade);
			entry.m_Outline.transform.localScale = ((arrayIndex != m_MissionIndex) ? Vector3.zero : Vector3.one);
			if (mission.m_Primary)
			{
				entry.m_Title.Text = "M" + (mission.m_LocalIndex + 1) + " - " + LocalizationManager.LocalizeString(mission.m_Name);
			}
			else
			{
				entry.m_Title.Text = "S" + (mission.m_LocalIndex + 1) + " - " + LocalizationManager.LocalizeString(mission.m_Name);
			}
			if (m_Category == Category.Finished)
			{
				entry.m_ActivateButton.transform.localScale = Vector3.zero;
				entry.m_ActivateText.transform.localScale = Vector3.zero;
				entry.m_ActiveBack.SetColor(Globals.m_This.m_BrightHUD);
				entry.m_Active.SetColor(Color.black);
				entry.m_Active.Text = "COMPLETED";
				entry.m_Location.transform.localScale = Vector3.one;
				entry.m_Location.Text = Globals.m_LocationNames[(int)mission.m_Location];
			}
			else
			{
				entry.m_ActivateButton.transform.localScale = Vector3.one;
				entry.m_ActivateText.transform.localScale = Vector3.one;
				entry.m_Location.transform.localScale = Vector3.zero;
			}
		}
	}

	private void PopulateMissionDetails()
	{
		while (m_DetailList.Count > 0)
		{
			m_DetailList.RemoveItem(0, true);
		}
		if (m_MissionIndex < 0 || m_MissionDetailPrefab == null || m_ListedMissions == null || m_ListedMissions.Count <= 0)
		{
			m_DetailList.slider.GetKnob().Hide(true);
			m_DetailMissionName.Text = string.Empty;
			return;
		}
		Mission mission = Globals.m_Missions[m_ListedMissions[m_MissionIndex]];
		if (mission.m_Primary)
		{
			m_DetailMissionName.Text = "M" + (mission.m_LocalIndex + 1) + " - " + LocalizationManager.LocalizeString(mission.m_Name);
		}
		else
		{
			m_DetailMissionName.Text = "S" + (mission.m_LocalIndex + 1) + " - " + LocalizationManager.LocalizeString(mission.m_Name);
		}
		if (mission.m_Subs != null && mission.m_Subs.Length > 0)
		{
			float num = 0f;
			for (int num2 = mission.m_Subs.Length - 1; num2 >= 0; num2--)
			{
				if (mission.m_Subs[num2].m_Status != MissionStatus.Unknown)
				{
					UIListItemContainer component = (Object.Instantiate(m_MissionDetailPrefab.gameObject) as GameObject).GetComponent<UIListItemContainer>();
					if (component != null)
					{
						MissionDetailContainer component2 = component.gameObject.GetComponent<MissionDetailContainer>();
						if (component2 != null)
						{
							if (mission.m_Subs[num2].m_Status == MissionStatus.Acquired)
							{
								component2.m_Icon.SetState(0);
								component2.m_Icon.SetColor(Globals.m_This.m_BrightHUD);
							}
							else if (mission.m_Subs[num2].m_Status == MissionStatus.Completed_Fail)
							{
								component2.m_Icon.SetState(2);
								component2.m_Icon.SetColor(Color.red);
							}
							else
							{
								component2.m_Icon.SetState(1);
								component2.m_Icon.SetColor(Color.green);
							}
							component2.m_Title.Text = mission.m_Subs[num2].m_Name;
							component2.m_Desc.Text = mission.m_Subs[num2].m_Desc;
							component.FindOuterEdges();
							num += component.TopLeftEdge.y - component.BottomRightEdge.y;
							m_DetailList.AddItem(component);
						}
					}
				}
			}
			m_DetailList.RepositionItems();
			m_DetailList.ScrollToItem(0, 0f);
			UISlider slider = m_DetailList.slider;
			UIScrollKnob knob = slider.GetKnob();
			UIListItemContainer uIListItemContainer = m_DetailList.GetItem(0) as UIListItemContainer;
			if (uIListItemContainer == null)
			{
				knob.Hide(true);
				return;
			}
			num += (float)(m_DetailList.Count - 1) * m_DetailList.itemSpacing;
			float num3 = Mathf.Max(num / m_DetailList.viewableArea.y, 0f);
			knob.Hide(num3 <= 1f);
			if (!knob.IsHidden())
			{
				knob.SetSize(Mathf.Max(slider.width * (1f / num3), 2f), knob.height);
				slider.stopKnobFromEdge = knob.width * 0.5f;
				slider.SetSize(slider.width, slider.height);
			}
		}
		else
		{
			m_DetailList.slider.GetKnob().Hide(true);
		}
	}
}
