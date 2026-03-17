using System;
using UnityEngine;

public class AugMenu : MonoBehaviour
{
	public enum AugLocation
	{
		None = -1,
		Cranium = 0,
		Torso = 1,
		Arms = 2,
		Eye = 3,
		Back = 4,
		Skin = 5,
		Leg = 6,
		Total = 7
	}

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

	public Transform m_Effect;

	public Animation m_EffectAnimator;

	private AnimationState m_EffectState;

	public Renderer m_CodeRenderer;

	private Vector2 m_uvOffset = Vector2.zero;

	private float m_EffectAnimationTime;

	public Transform[] m_AugLocations = new Transform[7];

	private float m_CurrentFlashTime;

	private bool m_QuickslotMode;

	private int m_CurrentSlot = -1;

	public Collider m_QuickslotsCollider;

	public UIButton[] m_QuickslotButtons;

	public UIButton[] m_QuickslotRemoves;

	public SimpleSprite[] m_QuickslotIcons;

	public SpriteText[] m_QuickslotQuantities;

	private int m_CurrentAug = 3;

	private int m_CurrentSubAug;

	public Aug[] m_Augs;

	public PackedSprite m_CategorySelector;

	public PackedSprite m_TreeSelector;

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

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		for (int i = 0; i < m_Augs.Length; i++)
		{
			if (m_Augs[i].m_SubAugs == null || m_Augs[i].m_SubAugs.Length <= 0)
			{
				m_Augs[i].m_Button.Data = -1;
				continue;
			}
			m_Augs[i].m_Button.Data = i;
			m_Augs[i].m_Button.SetValueChangedDelegate(AugSelected);
			if (m_Augs[i].m_SubAugs != null && m_Augs[i].m_SubAugs.Length > 0)
			{
				for (int j = 0; j < m_Augs[i].m_SubAugs.Length; j++)
				{
					m_Augs[i].m_SubAugs[j].Data = j;
					m_Augs[i].m_SubAugs[j].SetValueChangedDelegate(SubAugSelected);
				}
			}
		}
		for (int k = 0; k < 4; k++)
		{
			m_QuickslotButtons[k].Data = k;
			m_QuickslotButtons[k].SetValueChangedDelegate(QuickslotButtonPressed);
			m_QuickslotRemoves[k].Data = k;
			m_QuickslotRemoves[k].SetValueChangedDelegate(QuickslotRemovePressed);
		}
		m_EffectState = m_EffectAnimator["Take 001"];
		m_uvOffset = m_CodeRenderer.sharedMaterial.GetTextureOffset("_MainTex");
	}

	public static void AugsOpening()
	{
		if (!(m_This == null))
		{
			m_This.m_CurrentAug = 3;
			m_This.m_CurrentSubAug = 0;
			m_This.CancelQuickslotMode();
			m_This.UpdateAugCategories();
			m_This.UpdateAugTree();
		}
	}

	private void Update()
	{
		if (m_EffectAnimator != null)
		{
			m_EffectAnimationTime += PauseTabs.GetDeltaTime();
			while (m_EffectAnimationTime >= m_EffectState.length)
			{
				m_EffectAnimationTime -= m_EffectState.length;
			}
			m_EffectState.normalizedTime = m_EffectAnimationTime / m_EffectState.length;
			m_EffectAnimator.Sample();
			m_uvOffset += new Vector2(0f, 0.25f) * PauseTabs.GetDeltaTime();
			while (m_uvOffset.x >= 1f)
			{
				m_uvOffset.x -= 1f;
			}
			while (m_uvOffset.y >= 1f)
			{
				m_uvOffset.y -= 1f;
			}
			m_CodeRenderer.sharedMaterial.SetTextureOffset("_MainTex", m_uvOffset);
		}
	}

	private void LateUpdate()
	{
		m_CategorySelector.Hide(m_QuickslotMode);
		m_TreeSelector.Hide(m_QuickslotMode);
		m_CurrentFlashTime += PauseTabs.GetDeltaTime();
		if (m_CurrentFlashTime >= 1.2f)
		{
			m_CurrentFlashTime = 0f;
		}
		float t = 0f;
		if (m_CurrentFlashTime < 0.5f)
		{
			t = Mathf.Clamp01(m_CurrentFlashTime / 0.5f);
		}
		else if (m_CurrentFlashTime < 1f)
		{
			t = Mathf.Clamp01((1f - m_CurrentFlashTime) / 0.5f);
		}
		Color color = Color.Lerp(Globals.m_This.m_BlackHUD, Globals.m_This.m_BrightHUD, t);
		Color color2 = Color.Lerp(Globals.m_This.m_BlackHUD, m_Finished, t);
		Color color3 = Color.Lerp(Globals.m_This.m_BlackHUD, m_Available, t);
		Color color4 = Color.Lerp(Globals.m_This.m_BlackHUD, m_Unavailable, t);
		for (int i = 0; i < 4; i++)
		{
			if (m_QuickslotButtons[i].controlState == UIButton.CONTROL_STATE.OVER || m_QuickslotButtons[i].controlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				m_QuickslotButtons[i].SetColor(Globals.m_This.m_BrightHUD);
			}
			else if (m_QuickslotMode && i == m_CurrentSlot)
			{
				m_QuickslotButtons[i].SetColor(color);
			}
			else
			{
				m_QuickslotButtons[i].SetColor(Globals.m_This.m_BlackHUD);
			}
		}
		int num = 0;
		for (int j = 0; j < m_Augs.Length; j++)
		{
			Color color5 = m_Unavailable;
			if ((int)m_Augs[j].m_Button.Data >= 0)
			{
				num = 0;
				for (int k = 0; k < m_Augs[j].m_SubAugs.Length; k++)
				{
					AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)j, k);
					if (augmentationData != null && augmentationData.m_Purchased)
					{
						num++;
					}
				}
				color5 = (((!m_QuickslotMode || Globals.m_AugmentationData.GetAugmentationContainer((AugmentationData.Augmentations)j).m_Passive) && (m_CurrentAug != j || m_QuickslotMode)) ? ((num > 0) ? ((num < m_Augs[j].m_SubAugs.Length) ? m_Partial : m_Finished) : m_Available) : ((num > 0) ? ((num < m_Augs[j].m_SubAugs.Length) ? color : color2) : color3));
			}
			else if (m_CurrentAug == j)
			{
				color5 = color4;
			}
			color5.a *= m_AlphaTracker.Color.a;
			m_Augs[j].m_Button.SetColor(color5);
		}
		for (int l = 0; l < m_Augs[m_CurrentAug].m_SubAugs.Length; l++)
		{
			AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, l);
			Color color5 = m_Available;
			if (augmentationData != null && augmentationData.m_Purchased)
			{
				color5 = m_Partial;
			}
			else if (augmentationData == null)
			{
				color5 = m_Unavailable;
			}
			else if (augmentationData.m_Parents != null && augmentationData.m_Parents.Length > 0)
			{
				for (int m = 0; m < augmentationData.m_Parents.Length; m++)
				{
					if (!Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, augmentationData.m_Parents[m]).m_Purchased)
					{
						color5 = m_Unavailable;
						break;
					}
				}
			}
			color5.a *= m_AlphaTracker.Color.a;
			m_Augs[m_CurrentAug].m_SubAugs[l].SetColor(color5);
		}
	}

	private void UpdateAugCategories()
	{
		m_CategorySelector.transform.parent = m_Augs[m_CurrentAug].m_Button.transform.parent;
		m_CategorySelector.transform.localPosition = new Vector3(0f, 0f, -0.2f);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_Augs.Length; i++)
		{
			num = (num2 = 0);
			if (m_Augs[i].m_SubAugs != null)
			{
				m_Augs[i].m_SubAugGroup.SetActiveRecursively(m_CurrentAug == i);
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
		m_Effect.parent = m_AugLocations[(int)Globals.m_AugmentationData.GetAugmentationContainer((AugmentationData.Augmentations)m_CurrentAug).m_Location];
		m_Effect.localPosition = Vector3.zero;
		m_Effect.localRotation = Quaternion.identity;
	}

	private void UpdateAugTree()
	{
		m_TreeSelector.gameObject.active = true;
		m_TreeSelector.transform.parent = m_Augs[m_CurrentAug].m_SubAugs[m_CurrentSubAug].transform.parent;
		m_TreeSelector.transform.localPosition = new Vector3(0f, 0f, -0.2f);
		AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, m_CurrentSubAug);
		if (augmentationData != null)
		{
			m_AvailableText.Text = Globals.m_Inventory.m_Items[3][4].m_Quantity.ToString();
			m_CostText.Text = augmentationData.m_Cost.ToString();
			m_TreeCategoryText.Text = Globals.m_AugmentationData.GetAugmentationContainer((AugmentationData.Augmentations)m_CurrentAug).m_Name;
			m_TreeNameText.Text = augmentationData.m_Name;
			m_TreeShortDescText.Text = augmentationData.m_ShortDescription;
		}
		else
		{
			m_AvailableText.Text = Globals.m_Inventory.m_Items[3][4].m_Quantity.ToString();
			m_CostText.Text = "0";
			m_TreeCategoryText.Text = "Miscellaneous";
			m_TreeNameText.Text = "Unknown";
			m_TreeShortDescText.Text = "Data unavailable";
		}
	}

	public void AugSelected(IUIObject obj)
	{
		if (m_QuickslotMode)
		{
			int num = (int)obj.Data;
			int num2 = 0;
			for (int i = 0; i < m_Augs[num].m_SubAugs.Length; i++)
			{
				AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)num, i);
				if (augmentationData != null && augmentationData.m_Purchased)
				{
					num2++;
				}
			}
			if (num2 <= 0)
			{
				CancelQuickslotMode();
				return;
			}
			switch (num)
			{
			case 7:
				num = 7;
				break;
			case 9:
				num = 9;
				break;
			case 10:
				num = 10;
				break;
			default:
				CancelQuickslotMode();
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				if (Globals.m_Inventory.m_ItemQuickslots[j].m_CategoryID == 4 && Globals.m_Inventory.m_ItemQuickslots[j].m_ItemID == num)
				{
					Globals.m_Inventory.m_ItemQuickslots[j].m_CategoryID = -1;
					Globals.m_Inventory.m_ItemQuickslots[j].m_ItemID = -1;
				}
			}
			Globals.m_Inventory.m_ItemQuickslots[m_CurrentSlot].m_CategoryID = 4;
			Globals.m_Inventory.m_ItemQuickslots[m_CurrentSlot].m_ItemID = num;
			SoundManager.TriggerEvent("Play_UI_Quick_Slot_Equip");
			CancelQuickslotMode();
		}
		else
		{
			SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
			if ((int)obj.Data >= 0 && m_CurrentAug != (int)obj.Data)
			{
				m_CurrentAug = (int)obj.Data;
				m_CurrentSubAug = 0;
				UpdateAugCategories();
				UpdateAugTree();
			}
		}
	}

	public void SubAugSelected(IUIObject obj)
	{
		if (m_QuickslotMode)
		{
			CancelQuickslotMode();
			return;
		}
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		if (m_CurrentSubAug != (int)obj.Data)
		{
			m_CurrentSubAug = (int)obj.Data;
			UpdateAugTree();
		}
	}

	public void QuickslotButtonPressed(IUIObject obj)
	{
		if (m_QuickslotMode)
		{
			CancelQuickslotMode();
			return;
		}
		m_QuickslotMode = true;
		m_CurrentSlot = (int)obj.Data;
		m_CurrentFlashTime = 0f;
		m_QuickslotsCollider.enabled = true;
		m_QuickslotRemoves[m_CurrentSlot].Hide(false);
		m_QuickslotRemoves[m_CurrentSlot].SetControlState(UIButton.CONTROL_STATE.NORMAL);
		FadeSpriteAlpha.Do(m_QuickslotRemoves[m_CurrentSlot], EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, 0.4f, 0f, null, null);
		AnimateRotation.Do(m_QuickslotRemoves[m_CurrentSlot].transform.parent.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0f, 0f, -90f), Vector3.zero, EZAnimation.spring, 0.25f, 0f, null, null);
		SoundManager.TriggerEvent("Play_UI_Quick_Slot_Activate");
	}

	public void QuickslotRemovePressed(IUIObject obj)
	{
		if (m_QuickslotMode)
		{
			Globals.m_Inventory.m_ItemQuickslots[(int)obj.Data].m_CategoryID = -1;
			Globals.m_Inventory.m_ItemQuickslots[(int)obj.Data].m_ItemID = -1;
		}
		CancelQuickslotMode();
		SoundManager.TriggerEvent("Play_UI_Quick_Slot_Unequip");
	}

	public void CancelQuickslotMode()
	{
		m_QuickslotMode = false;
		m_QuickslotsCollider.enabled = false;
		SoundManager.TriggerEvent("Stop_UI_Quick_Slot_Glow");
		for (int i = 0; i < 4; i++)
		{
			m_QuickslotRemoves[i].Hide(true);
			Item_Base itemQuickslotItem = Globals.m_Inventory.GetItemQuickslotItem(i);
			if (itemQuickslotItem != null && itemQuickslotItem.m_Texture != null)
			{
				m_QuickslotIcons[i].Hide(false);
				m_QuickslotIcons[i].renderer.material.mainTexture = itemQuickslotItem.m_Texture;
				if (Globals.m_Inventory.m_ItemQuickslots[i].m_CategoryID == 4)
				{
					m_QuickslotIcons[i].transform.localScale = Vector3.one;
				}
				else
				{
					m_QuickslotIcons[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				}
				m_QuickslotQuantities[i].Hide(itemQuickslotItem.m_MaxQuantity == 1);
				m_QuickslotQuantities[i].Text = itemQuickslotItem.m_Quantity.ToString();
			}
			else
			{
				m_QuickslotIcons[i].Hide(true);
				m_QuickslotQuantities[i].Hide(true);
			}
		}
	}

	public void InfoPressed()
	{
		CancelQuickslotMode();
		AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, m_CurrentSubAug);
		if (augmentationData != null)
		{
			DescriptionPanel.OpenDescriptionPanel(augmentationData.m_Name, augmentationData.m_Description, DescriptionClosed);
		}
	}

	public void BuyPressed()
	{
		if (m_QuickslotMode)
		{
			CancelQuickslotMode();
			return;
		}
		AugData augmentationData = Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, m_CurrentSubAug);
		if (augmentationData != null && !augmentationData.m_Purchased && augmentationData.m_Cost <= Globals.m_Inventory.GetItemQuantity(3, 4))
		{
			if (augmentationData.m_Parents != null && augmentationData.m_Parents.Length > 0)
			{
				for (int i = 0; i < augmentationData.m_Parents.Length; i++)
				{
					if (!Globals.m_AugmentationData.GetAugmentationData((AugmentationData.Augmentations)m_CurrentAug, augmentationData.m_Parents[i]).m_Purchased)
					{
						SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
						return;
					}
				}
			}
			augmentationData.Purchase();
			Globals.m_Inventory.AdjustItemQuantity(3, 4, -augmentationData.m_Cost);
			SoundManager.TriggerEvent("Play_UI_Transaction", base.gameObject);
			UpdateAugCategories();
			UpdateAugTree();
		}
		else
		{
			SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
			PopUpPanel.OpenPopUp("You don't have enough Praxis Kits.\nWould you like to buy more?", MessageButtons.YesNo, PraxisPurchaseCallback);
		}
	}

	private void PraxisPurchaseCallback(bool choice)
	{
		if (choice)
		{
			InfoPanel.OpenInfoPanel(3, 4, DescriptionClosed, "Return To Augs");
			return;
		}
		Globals.m_HUDRoot.m_PanelManager.BringIn("AugmentationMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		UpdateAugCategories();
		UpdateAugTree();
		PauseTabs.SetMenu(2, false);
	}

	private void DescriptionClosed()
	{
		Globals.m_HUDRoot.m_PanelManager.BringIn("AugmentationMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		UpdateAugCategories();
		UpdateAugTree();
		PauseTabs.SetMenu(2, false);
	}
}
