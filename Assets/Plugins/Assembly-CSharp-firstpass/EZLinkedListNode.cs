public class EZLinkedListNode<T> : IEZLinkedListItem<EZLinkedListNode<T>>
{
	public T val;

	private EZLinkedListNode<T> m_prev;

	private EZLinkedListNode<T> m_next;

	public EZLinkedListNode<T> prev
	{
		get
		{
			return m_prev;
		}
		set
		{
			m_prev = value;
		}
	}

	public EZLinkedListNode<T> next
	{
		get
		{
			return m_next;
		}
		set
		{
			m_next = value;
		}
	}

	public EZLinkedListNode(T v)
	{
		val = v;
	}
}
