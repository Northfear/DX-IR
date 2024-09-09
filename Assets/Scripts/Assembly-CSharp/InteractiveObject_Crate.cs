using Fabric;
using UnityEngine;

public class InteractiveObject_Crate : InteractiveObject_Base
{
	public Animation m_Animator;

	public string m_OpenSoundEffect;

	public string m_ErrorSoundEffect;

	public bool m_RemoveCollider;

	public bool m_HeavyObject;

	public override InteractivePopup.PopupType GetPopupType()
	{
		Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
		if (m_HeavyObject && !augmentation_Strength.GetCanMoveHeavyObject())
		{
			return InteractivePopup.PopupType.HeavyObject;
		}
		return InteractivePopup.PopupType.Normal;
	}

	public override bool EnableInteractiveObject()
	{
		if (!base.EnableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject()
	{
		if (!base.DisableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject()
	{
		if (!base.InteractWithObject())
		{
			return false;
		}
		if (m_HeavyObject)
		{
			Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
			if (!augmentation_Strength.GetCanMoveHeavyObject())
			{
				if (m_ErrorSoundEffect != null)
				{
					EventManager.Instance.PostEvent(m_ErrorSoundEffect, EventAction.PlaySound, null, base.gameObject);
				}
				return false;
			}
		}
		m_Animator.Play();
		EventManager.Instance.PostEvent(m_OpenSoundEffect, EventAction.PlaySound, null, base.gameObject);
		if (m_RemoveCollider)
		{
			m_InteractiveCollider.enabled = false;
		}
		m_Active = false;
		return true;
	}
}
