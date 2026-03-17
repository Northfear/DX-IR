using System;
using System.Collections;
using UnityEngine;

public class InteractiveObject_Crate : InteractiveObject_Base
{
	public Animation m_Animator;

	public string m_OpenSoundEffect;

	public string m_ErrorSoundEffect;

	public bool m_RemoveCollider;

	public bool m_HeavyObject;

	private void Start()
	{
		m_Save = true;
		m_InteractOnLoad = true;
	}

	public override InteractivePopup.PopupType GetPopupType()
	{
		Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
		if (m_HeavyObject && !augmentation_Strength.GetCanMoveHeavyObject())
		{
			return InteractivePopup.PopupType.HeavyObject;
		}
		return InteractivePopup.PopupType.Normal;
	}

	public override bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.EnableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.DisableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
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
					SoundManager.TriggerEvent(m_ErrorSoundEffect, base.gameObject);
				}
				return false;
			}
		}
		m_Animator.Play();
		if (instant)
		{
			{
				IEnumerator enumerator = m_Animator.animation.GetEnumerator();
				try
				{
					if (enumerator.MoveNext())
					{
						AnimationState animationState = (AnimationState)enumerator.Current;
						animationState.time = animationState.length;
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
		if (!instant)
		{
			SoundManager.TriggerEvent(m_OpenSoundEffect, base.gameObject);
		}
		if (m_RemoveCollider)
		{
			m_InteractiveCollider.enabled = false;
		}
		m_Active = false;
		return true;
	}
}
