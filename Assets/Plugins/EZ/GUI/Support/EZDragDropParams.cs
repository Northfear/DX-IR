public struct EZDragDropParams
{
	public EZDragDropEvent evt;

	public IUIObject dragObj;

	public POINTER_INFO ptr;

	public EZDragDropParams(EZDragDropEvent e, IUIObject obj, POINTER_INFO p)
	{
		evt = e;
		dragObj = obj;
		ptr = p;
	}
}
