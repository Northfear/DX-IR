using Fabric;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/List Button")]
public class UIListButton : UIListItem
{
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
		{
			return;
		}
		if (!m_controlIsEnabled)
		{
			DoNeccessaryInput(ref ptr);
			return;
		}
		if (list != null && Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > list.dragThreshold * list.dragThreshold)
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
		}
		else
		{
			ptr.isTap = true;
		}
		if (inputDelegate != null)
		{
			inputDelegate(ref ptr);
		}
		if (!m_controlIsEnabled)
		{
			DoNeccessaryInput(ref ptr);
			return;
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (ptr.active && list != null)
			{
				list.ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE:
			if (soundOnOver != null && m_ctrlState != CONTROL_STATE.OVER)
			{
				soundOnOver.PlayOneShot(soundOnOver.clip);
			}
			SetControlState(CONTROL_STATE.OVER);
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (!ptr.isTap)
			{
				SetControlState(CONTROL_STATE.NORMAL);
				if (list != null)
				{
					list.ListDragged(ptr);
				}
			}
			else
			{
				SetControlState(CONTROL_STATE.ACTIVE);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.PRESS:
			SetControlState(CONTROL_STATE.ACTIVE);
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			if (list != null && ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				list.DidClick(this);
			}
			if (list != null)
			{
				list.PointerReleased();
			}
			if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD && ptr.hitInfo.collider == base.collider)
			{
				SetControlState(CONTROL_STATE.OVER);
			}
			else
			{
				SetControlState(CONTROL_STATE.NORMAL);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			SetControlState(CONTROL_STATE.NORMAL);
			break;
		}
		if (list != null && ptr.inputDelta.z != 0f && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			list.ScrollWheel(ptr.inputDelta.z);
		}
		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
		if (repeat)
		{
			if (m_ctrlState != CONTROL_STATE.ACTIVE)
			{
				return;
			}
		}
		else if (ptr.evt != whenToInvoke)
		{
			return;
		}
		if (ptr.evt == whenToInvoke)
		{
			if (soundOnClick != null)
			{
				soundOnClick.PlayOneShot(soundOnClick.clip);
			}
			if (eventsOnClick != null)
			{
				for (int i = 0; i < eventsOnClick.Length; i++)
				{
					if (eventsOnClick[i] != null)
					{
						EventManager.Instance.PostEvent(eventsOnClick[i].SoundEventName, eventsOnClick[i].SoundEventAction, eventsOnClick[i].SoundEventParameter);
					}
				}
			}
		}
		if (scriptWithMethodToInvoke != null)
		{
			scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}
		if (changeDelegate != null)
		{
			changeDelegate(this);
		}
	}

	public new static UIListButton Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIListButton)gameObject.AddComponent(typeof(UIListButton));
	}

	public new static UIListButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIListButton)gameObject.AddComponent(typeof(UIListButton));
	}
}
