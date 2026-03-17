using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class InteractiveObject_BreakableWall : InteractiveObject_Base
{
	public Renderer m_PieceToRemove;

	public Animation m_WallAnimator;

	public GameObject m_VFXPrefab;

	public GameObject m_WallCubes;

	public Material m_AugMaterial;

	public float m_PieceDisappearTime = 1.833f;

	public float m_DistanceToCheckForEnemy = 6f;

	private bool m_WallIsBroken;

	private GameObject m_Effect;

	private bool m_AugActive;

	private float m_DisappearTimer;

	private bool m_TimeRunning;

	public bool IsWallBroken()
	{
		return m_WallIsBroken;
	}

	public override bool UseInteractiveCollider()
	{
		return false;
	}

	public override InteractivePopup.PopupType GetPopupType()
	{
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
		Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
		if (!augmentation_Strength.GetCanBreakThroughWall())
		{
			return true;
		}
		Enemy_Base enemy_Base = null;
		Vector2 vector = default(Vector2);
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.GetFirstEnemy(i); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				vector.x = linkedListNode.Value.m_RigMotion.transform.position.x - Globals.m_CameraController.transform.position.x;
				vector.y = linkedListNode.Value.m_RigMotion.transform.position.z - Globals.m_CameraController.transform.position.z;
				if (vector.magnitude < m_DistanceToCheckForEnemy)
				{
					enemy_Base = linkedListNode.Value;
					break;
				}
			}
			if (enemy_Base != null)
			{
				break;
			}
		}
		if (enemy_Base == null || enemy_Base.IsDead())
		{
			Globals.m_PlayerController.BeginWallTakedown(base.gameObject, null);
		}
		else
		{
			Globals.m_PlayerController.BeginWallTakedown(base.gameObject, enemy_Base.gameObject);
		}
		return true;
	}

	public override Vector3 GetPopupLocation()
	{
		Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
		if (!augmentation_Strength.GetCanBreakThroughWall())
		{
			return InteractiveObject_Base.m_OffScreen;
		}
		return base.GetPopupLocation();
	}

	public void BeginBreakingWall(bool instant = false)
	{
		m_Active = false;
		m_InteractiveCollider.enabled = false;
		Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
		m_TimeRunning = true;
		m_WallAnimator.Play();
		if (instant)
		{
			{
				IEnumerator enumerator = m_WallAnimator.animation.GetEnumerator();
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
		if (m_VFXPrefab != null)
		{
			m_Effect = UnityEngine.Object.Instantiate(m_VFXPrefab, base.gameObject.transform.position, base.gameObject.transform.rotation) as GameObject;
			m_Effect.transform.parent = base.gameObject.transform;
		}
		m_WallIsBroken = true;
	}

	public void CleanUpVFX()
	{
		if ((bool)m_Effect)
		{
			UnityEngine.Object.Destroy(m_Effect);
		}
	}

	public void SetAugMaterial()
	{
		if (m_AugMaterial != null)
		{
			m_ObjectRenderer.material = m_AugMaterial;
		}
		m_AugActive = true;
	}

	private void Update()
	{
		if (m_TimeRunning)
		{
			float disappearTimer = m_DisappearTimer;
			m_DisappearTimer += Time.deltaTime;
			if (disappearTimer < m_PieceDisappearTime && m_DisappearTimer >= m_PieceDisappearTime)
			{
				m_PieceToRemove.enabled = false;
				m_TimeRunning = false;
			}
		}
		if (m_WallIsBroken || m_AugActive)
		{
			return;
		}
		if (m_AugMaterial == null)
		{
			m_AugActive = true;
			return;
		}
		Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
		if (augmentation_Strength.GetCanBreakThroughWall())
		{
			SetAugMaterial();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_Save = true;
		Augmentation_Strength augmentation_Strength = (Augmentation_Strength)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Strength);
		if (augmentation_Strength.GetCanBreakThroughWall())
		{
			SetAugMaterial();
		}
	}

	public override XmlElement LoadGame(XmlElement root)
	{
		XmlElement result = base.LoadGame(root);
		if (m_Activated)
		{
			BeginBreakingWall(true);
		}
		return result;
	}
}
