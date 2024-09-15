public interface IKeyFocusable
{
	string Content { get; }

	void LostFocus();

	string SetInputText(string inputText, ref int insertPt);

	string GetInputText(ref KEYBOARD_INFO info);

	void Commit();

	void SetCommitDelegate(EZKeyboardCommitDelegate del);

	void GoUp();

	void GoDown();
}
