using UnityEngine;

public interface IUIContainer : IEZDragDrop, IUIObject
{
	void AddChild(GameObject go);

	void RemoveChild(GameObject go);

	void AddSubject(GameObject go);

	void RemoveSubject(GameObject go);
}
