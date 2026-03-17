using System;
using System.Collections;
using UnityEngine;

public class InteractiveObject_Vent : InteractiveObject_Base
{
	public Animation m_VentAnimator;

	public string m_VentOpenSoundEffect = "Vent_Open";

	private void Start()
	{
		m_Save = true;
		m_InteractOnLoad = true;
	}

	private void Update()
	{
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
		m_VentAnimator.Play();
		if (instant)
		{
			{
				IEnumerator enumerator = m_VentAnimator.animation.GetEnumerator();
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
			SoundManager.TriggerEvent(m_VentOpenSoundEffect, base.gameObject);
		}
		DisableInteractiveObject(null, true);
		m_InteractiveCollider.enabled = false;
		m_Active = false;
		return true;
	}
}
