using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Action Button")]
public class UIActionBtn : UIButton
{
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
		{
			return;
		}
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (repeat)
		{
			UIManager.instance.RayActive = ((base.controlState == CONTROL_STATE.ACTIVE) ? UIManager.RAY_ACTIVE_STATE.Constant : UIManager.RAY_ACTIVE_STATE.Inactive);
		}
		else if (ptr.evt == whenToInvoke)
		{
			UIManager.instance.RayActive = UIManager.RAY_ACTIVE_STATE.Momentary;
		}
		base.OnInput(ref ptr);
	}

	public new static UIActionBtn Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIActionBtn)gameObject.AddComponent(typeof(UIActionBtn));
	}

	public new static UIActionBtn Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIActionBtn)gameObject.AddComponent(typeof(UIActionBtn));
	}
}
