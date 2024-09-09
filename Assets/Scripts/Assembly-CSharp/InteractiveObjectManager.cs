using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectManager : MonoBehaviour
{
	public GameObject m_PopupPrefab;

	public float m_ZOffset = 1.5f;

	public float m_DefaultTriggerVolumeSize = 4f;

	public float m_TakedownTriggerVolumeSize = 2f;

	private LinkedList<InteractivePopup> m_ActivePopups = new LinkedList<InteractivePopup>();

	private LinkedList<InteractivePopup> m_InactivePopups = new LinkedList<InteractivePopup>();

	public static InteractivePopup m_PopupPressed;

	public void EnableInteractivePopup(InteractiveObject_Base obj, InteractivePopup.PopupType type)
	{
		if (m_InactivePopups.Count == 0)
		{
			AddPopup();
		}
		LinkedListNode<InteractivePopup> first = m_InactivePopups.First;
		InteractivePopup value = first.Value;
		m_InactivePopups.Remove(first);
		value.m_InteractiveObject = obj;
		value.m_PopupType = type;
		value.Enable(true);
		value.SetText(obj);
		m_ActivePopups.AddLast(value);
	}

	public void DisableInteractivePopup(InteractiveObject_Base obj)
	{
		InteractivePopup interactivePopup = null;
		foreach (InteractivePopup activePopup in m_ActivePopups)
		{
			if (activePopup.m_InteractiveObject == obj)
			{
				interactivePopup = activePopup;
				break;
			}
		}
		if (interactivePopup != null)
		{
			interactivePopup.Enable(false);
			m_ActivePopups.Remove(interactivePopup);
			m_InactivePopups.AddLast(interactivePopup);
		}
	}

	public void ClearAll()
	{
		m_ActivePopups.Clear();
		m_InactivePopups.Clear();
	}

	public void PopupPressed(POINTER_INFO ptr)
	{
		m_PopupPressed = GetPopup(ptr.hitInfo.collider.gameObject);
	}

	public void LethalTakedownPopupPressed(POINTER_INFO ptr)
	{
		m_PopupPressed = GetPopup(ptr.hitInfo.collider.gameObject);
	}

	public void NonLethalTakedownPopupPressed(POINTER_INFO ptr)
	{
		m_PopupPressed = GetPopup(ptr.hitInfo.collider.gameObject);
	}

	public void PopupReleased(POINTER_INFO ptr)
	{
		InteractivePopup popup = GetPopup(ptr.hitInfo.collider.gameObject);
		if (popup == m_PopupPressed)
		{
			popup.m_InteractiveObject.InteractWithObject();
		}
		m_PopupPressed = null;
	}

	public void LethalTakedownPopupReleased(POINTER_INFO ptr)
	{
		InteractivePopup popup = GetPopup(ptr.hitInfo.collider.gameObject);
		if (popup == m_PopupPressed)
		{
			((InteractiveObject_Takedown)popup.m_InteractiveObject).InteractWithObject(true);
		}
		m_PopupPressed = null;
	}

	public void NonLethalTakedownPopupReleased(POINTER_INFO ptr)
	{
		InteractivePopup popup = GetPopup(ptr.hitInfo.collider.gameObject);
		if (popup == m_PopupPressed && popup != null)
		{
			((InteractiveObject_Takedown)popup.m_InteractiveObject).InteractWithObject(false);
		}
		m_PopupPressed = null;
	}

	private void Awake()
	{
		Globals.m_InteractiveObjectManager = this;
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		LinkedListNode<InteractivePopup> linkedListNode = m_ActivePopups.First;
		while (linkedListNode != null)
		{
			LinkedListNode<InteractivePopup> next = linkedListNode.Next;
			if (linkedListNode.Value.m_InteractiveObject != null)
			{
				Vector3 popupLocation = linkedListNode.Value.m_InteractiveObject.GetPopupLocation();
				popupLocation.z += m_ZOffset;
				popupLocation += linkedListNode.Value.m_InteractiveObject.m_PositionOffset;
				linkedListNode.Value.SetPosition(popupLocation);
			}
			else
			{
				linkedListNode.Value.Enable(false);
				m_ActivePopups.Remove(linkedListNode.Value);
				m_InactivePopups.AddLast(linkedListNode.Value);
			}
			linkedListNode = next;
		}
	}

	private void AddPopup()
	{
		InteractivePopup interactivePopup = new InteractivePopup();
		GameObject gameObject = Object.Instantiate(m_PopupPrefab) as GameObject;
		gameObject.transform.parent = Globals.m_HUD.gameObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		UIButton[] componentsInChildren = gameObject.GetComponentsInChildren<UIButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.name == "NormalPopup_Background")
			{
				interactivePopup.m_NormalPopup.m_PopupBackground = componentsInChildren[i];
				interactivePopup.m_NormalPopup.m_PopupBackground.scriptWithMethodToInvoke = this;
			}
			if (componentsInChildren[i].gameObject.name == "TakedownPopup1_Background")
			{
				interactivePopup.m_TakedownPopup1.m_PopupBackground = componentsInChildren[i];
				interactivePopup.m_TakedownPopup1.m_PopupBackground.scriptWithMethodToInvoke = this;
			}
			if (componentsInChildren[i].gameObject.name == "TakedownPopup2_Background")
			{
				interactivePopup.m_TakedownPopup2.m_PopupBackground = componentsInChildren[i];
				interactivePopup.m_TakedownPopup2.m_PopupBackground.scriptWithMethodToInvoke = this;
			}
		}
		SpriteText[] componentsInChildren2 = gameObject.GetComponentsInChildren<SpriteText>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].gameObject.name == "NormalPopup_Text")
			{
				interactivePopup.m_NormalPopup.m_PopupText = componentsInChildren2[j];
			}
			if (componentsInChildren2[j].gameObject.name == "TakedownPopup1_Text")
			{
				interactivePopup.m_TakedownPopup1.m_PopupText = componentsInChildren2[j];
			}
			if (componentsInChildren2[j].gameObject.name == "TakedownPopup2_Text")
			{
				interactivePopup.m_TakedownPopup2.m_PopupText = componentsInChildren2[j];
			}
		}
		PackedSprite[] componentsInChildren3 = gameObject.GetComponentsInChildren<PackedSprite>();
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			if (componentsInChildren3[k].gameObject.name == "NormalPopup_Icon_TooHeavy")
			{
				interactivePopup.m_TooHeavyIcon = componentsInChildren3[k];
			}
		}
		m_InactivePopups.AddLast(interactivePopup);
	}

	private InteractivePopup GetPopup(GameObject obj)
	{
		foreach (InteractivePopup activePopup in m_ActivePopups)
		{
			if (obj == activePopup.m_NormalPopup.m_PopupBackground.gameObject || obj == activePopup.m_TakedownPopup1.m_PopupBackground.gameObject || obj == activePopup.m_TakedownPopup2.m_PopupBackground.gameObject)
			{
				return activePopup;
			}
		}
		return null;
	}
}
