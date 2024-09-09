using System;
using Fabric;
using UnityEngine;

public class AugMenu : MonoBehaviour
{
	[Serializable]
	public class Aug
	{
		public UIButton m_Button;

		public PackedSprite m_Plaque;

		public SpriteText m_Text;

		public GameObject m_SubAugGroup;

		public UIButton[] m_SubAugs;
	}

	public static AugMenu m_This;

	private int m_CurrentAug = 3;

	private int m_CurrentSubAug;

	public Aug[] m_Augs;

	public Transform m_CategorySelector;

	public Transform m_TreeSelector;

	public SpriteText m_TreeCategoryText;

	public SpriteText m_TreeNameText;

	public SpriteText m_TreeShortDescText;

	public SpriteText m_CostText;

	public SpriteText m_AvailableText;

	public PackedSprite m_AlphaTracker;

	public Color m_Finished;

	public Color m_Partial;

	public Color m_Available;

	public Color m_Unavailable;

	private float m_FlashTimer;

	private void Awake()
	{
		m_This = this;
		for (int i = 0; i < m_Augs.Length; i++)
		{
			if (i > 5)
			{
				m_Augs[i].m_Button.Data = -1;
				continue;
			}
			if (m_Augs[i].m_SubAugs == null || m_Augs[i].m_SubAugs.Length <= 0)
			{
				m_Augs[i].m_Button.Data = -1;
				continue;
			}
			m_Augs[i].m_Button.Data = i;
			m_Augs[i].m_Button.AddValueChangedDelegate(AugSelected);
			if (m_Augs[i].m_SubAugs != null && m_Augs[i].m_SubAugs.Length > 0)
			{
				for (int j = 0; j < m_Augs[i].m_SubAugs.Length; j++)
				{
					m_Augs[i].m_SubAugs[j].Data = j;
					m_Augs[i].m_SubAugs[j].AddValueChangedDelegate(SubAugSelected);
				}
			}
		}
	}

	public static void AugsOpening(bool FreshEnter = true)
	{
		if (!(m_This == null))
		{
			if (FreshEnter)
			{
				m_This.m_CurrentAug = 3;
				m_This.m_CurrentSubAug = 0;
			}
			m_This.UpdateAugCategories();
			m_This.UpdateAugTree();
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < m_Augs.Length; i++)
		{
			if (m_Augs[i].m_SubAugGroup != null)
			{
				m_Augs[i].m_SubAugGroup.SetActiveRecursively(m_CurrentAug == i);
			}
		}
		UpdateButtonColors();
	}

	private void UpdateButtonColors()
	{
		float num = 0f;
		int num2 = 0;
		m_FlashTimer += 1.5f * PauseTabs.GetDeltaTime();
		if (m_FlashTimer >= 1f)
		{
			m_FlashTimer = 0f;
		}
		if (m_FlashTimer <= 0.5f)
		{
			num = m_FlashTimer / 0.5f;
		}
		else if (m_FlashTimer <= 1f)
		{
			num = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0.5f, 1f, m_FlashTimer));
		}
		for (int i = 0; i < m_Augs.Length; i++)
		{
			Color color = m_Unavailable;
			if ((int)m_Augs[i].m_Button.Data >= 0)
			{
				num2 = 0;
				for (int j = 0; j < m_Augs[i].m_SubAugs.Length; j++)
				{
					AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)i, j);
					if (augmentationData != null && augmentationData.m_Purchased)
					{
						num2++;
					}
				}
				color = ((num2 <= 0) ? m_Available : ((num2 < m_Augs[i].m_SubAugs.Length) ? m_Partial : m_Finished));
			}
			color.a *= m_AlphaTracker.Color.a;
			m_Augs[i].m_Plaque.SetColor(color);
			if (m_CurrentAug == i)
			{
				color.r = Mathf.Min(color.r + num * 0.3f, 1f);
				color.g = Mathf.Min(color.g + num * 0.3f, 1f);
				color.b = Mathf.Min(color.b + num * 0.3f, 1f);
			}
			m_Augs[i].m_Button.SetColor(color);
		}
		for (int k = 0; k < m_Augs[m_CurrentAug].m_SubAugs.Length; k++)
		{
			AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, k);
			Color color = m_Available;
			if (augmentationData != null && augmentationData.m_Purchased)
			{
				color = m_Partial;
			}
			else if (augmentationData == null)
			{
				color = m_Unavailable;
			}
			else if (augmentationData.m_Parents != null && augmentationData.m_Parents.Length <= 0)
			{
				for (int l = 0; l < augmentationData.m_Parents.Length; l++)
				{
					if (!Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, l).m_Purchased)
					{
						color = m_Unavailable;
						break;
					}
				}
			}
			color.a *= m_AlphaTracker.Color.a;
			m_Augs[m_CurrentAug].m_SubAugs[k].SetColor(color);
		}
	}

	private void UpdateAugCategories()
	{
		m_CategorySelector.parent = m_Augs[m_CurrentAug].m_Button.transform.parent;
		m_CategorySelector.localPosition = new Vector3(0f, 0f, -0.2f);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_Augs.Length; i++)
		{
			num = (num2 = 0);
			if (m_Augs[i].m_SubAugs != null)
			{
				for (int j = 0; j < m_Augs[i].m_SubAugs.Length; j++)
				{
					AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)i, j);
					if (augmentationData != null)
					{
						num2++;
						if (augmentationData.m_Purchased)
						{
							num++;
						}
					}
				}
			}
			m_Augs[i].m_Text.Text = num + "/" + num2;
		}
	}

	private void UpdateAugTree()
	{
		m_TreeSelector.gameObject.active = true;
		m_TreeSelector.parent = m_Augs[m_CurrentAug].m_SubAugs[m_CurrentSubAug].transform.parent;
		m_TreeSelector.localPosition = new Vector3(0f, 0f, -0.2f);
		AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, m_CurrentSubAug);
		if (augmentationData != null)
		{
			m_AvailableText.Text = Globals.m_Inventory.m_PraxisKits.ToString();
			m_CostText.Text = augmentationData.m_Cost.ToString();
			m_TreeCategoryText.Text = Globals.m_AugmentationData.GetAugmentationContainer((AugmentationData.Augmentations)m_CurrentAug).m_Name;
			m_TreeNameText.Text = augmentationData.m_Name;
			m_TreeShortDescText.Text = augmentationData.m_ShortDescription;
		}
		else
		{
			m_AvailableText.Text = Globals.m_Inventory.m_PraxisKits.ToString();
			m_CostText.Text = "0";
			m_TreeCategoryText.Text = "Miscellaneous";
			m_TreeNameText.Text = "Unknown";
			m_TreeShortDescText.Text = "Data unavailable";
		}
	}

	public void AugSelected(IUIObject obj)
	{
		EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		if ((int)obj.Data >= 0 && m_CurrentAug != (int)obj.Data)
		{
			m_CurrentAug = (int)obj.Data;
			m_CurrentSubAug = 0;
			UpdateAugCategories();
			UpdateAugTree();
		}
	}

	public void SubAugSelected(IUIObject obj)
	{
		EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		if (m_CurrentSubAug != (int)obj.Data)
		{
			m_CurrentSubAug = (int)obj.Data;
			UpdateAugTree();
		}
	}

	public void InfoPressed()
	{
		AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, m_CurrentSubAug);
		if (augmentationData != null)
		{
			DescriptionPanel.OpenDescriptionPanel(augmentationData.m_Name, augmentationData.m_Description);
		}
	}

	public void BuyPressed()
	{
		AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, m_CurrentSubAug);
		if (augmentationData != null && !augmentationData.m_Purchased && augmentationData.m_Cost <= Globals.m_Inventory.m_PraxisKits)
		{
			if (augmentationData.m_Parents != null && augmentationData.m_Parents.Length > 0)
			{
				for (int i = 0; i < augmentationData.m_Parents.Length; i++)
				{
					if (!Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, i).m_Purchased)
					{
						EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
						return;
					}
				}
			}
			augmentationData.Purchase();
			Globals.m_Inventory.m_PraxisKits -= augmentationData.m_Cost;
			EventManager.Instance.PostEvent("UI_Transaction", EventAction.PlaySound, null, base.gameObject);
			UpdateAugCategories();
			UpdateAugTree();
		}
		else
		{
			EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
		}
	}
}
