public class EZLinkedListIterator<T> where T : IEZLinkedListItem<T>
{
	protected T cur;

	protected EZLinkedList<T> list;

	public T Current
	{
		get
		{
			return cur;
		}
		set
		{
			cur = value;
		}
	}

	public bool Done
	{
		get
		{
			return cur == null;
		}
	}

	public bool Begin(EZLinkedList<T> l)
	{
		list = l;
		cur = l.Head;
		return cur == null;
	}

	public void End()
	{
		list.End(this);
	}

	public bool Next()
	{
		if (cur != null)
		{
			cur = cur.next;
		}
		if (cur == null)
		{
			list.End(this);
			return false;
		}
		return true;
	}

	public bool NextNoRemove()
	{
		if (cur != null)
		{
			cur = cur.next;
		}
		return cur != null;
	}
}
