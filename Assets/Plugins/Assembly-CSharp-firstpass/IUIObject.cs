using UnityEngine;

public interface IUIObject : IEZDragDrop
{
	bool controlIsEnabled { get; set; }

	bool DetargetOnDisable { get; set; }

	IUIContainer Container { get; set; }

	GameObject gameObject { get; }

	Transform transform { get; }

	string name { get; }

	IUIObject GetControl(ref POINTER_INFO ptr);

	new string ToString();

	bool RequestContainership(IUIContainer cont);

	bool GotFocus();

	void OnInput(POINTER_INFO ptr);

	void SetInputDelegate(EZInputDelegate del);

	void AddInputDelegate(EZInputDelegate del);

	void RemoveInputDelegate(EZInputDelegate del);

	void SetValueChangedDelegate(EZValueChangedDelegate del);

	void AddValueChangedDelegate(EZValueChangedDelegate del);

	void RemoveValueChangedDelegate(EZValueChangedDelegate del);
}
