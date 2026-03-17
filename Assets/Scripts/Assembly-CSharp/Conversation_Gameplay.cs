public class Conversation_Gameplay : Conversation_Base
{
	public override void BeginConversation(bool postSequence)
	{
		base.BeginConversation(postSequence);
	}

	public override void DialogChoicePressed(DialogChoiceEnum choice)
	{
		base.DialogChoicePressed(choice);
	}

	protected override void EndConversation()
	{
		base.EndConversation();
	}

	private void Start()
	{
	}
}
