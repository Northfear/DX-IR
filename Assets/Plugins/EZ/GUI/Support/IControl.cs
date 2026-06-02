public interface IControl
{
	string Text { get; set; }

	bool IncludeTextInAutoCollider { get; set; }

	EZTransitionList[] Transitions { get; set; }

	object Data { get; set; }

	void UpdateCollider();

	void Copy(IControl c);

	void Copy(IControl c, ControlCopyFlags flags);

	int DrawPreStateSelectGUI(int selState, bool inspector);

	int DrawPostStateSelectGUI(int selState);

	void DrawPreTransitionUI(int selState, IGUIScriptSelector gui);

	string[] EnumStateElements();

	EZTransitionList GetTransitions(int index);

	string GetStateLabel(int index);

	void SetStateLabel(int index, string label);

	ASCSEInfo GetStateElementInfo(int stateNum);
}
