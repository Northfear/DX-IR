using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("EZ GUI/Panels/Panel")]
public class UIPanel : UIPanelBase
{
	[HideInInspector]
	public EZTransitionList transitions = new EZTransitionList(new EZTransition[4]
	{
		new EZTransition("Bring In Forward"),
		new EZTransition("Bring In Back"),
		new EZTransition("Dismiss Forward"),
		new EZTransition("Dismiss Back")
	});

	public override EZTransitionList Transitions
	{
		get
		{
			return transitions;
		}
	}

	public static UIPanel Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIPanel)gameObject.AddComponent(typeof(UIPanel));
	}

	public static UIPanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIPanel)gameObject.AddComponent(typeof(UIPanel));
	}
}
