using System.Collections.Generic;

public class EZLinkedList<T> where T : IEZLinkedListItem<T>
{
	private List<EZLinkedListIterator<T>> iters = new List<EZLinkedListIterator<T>>();

	private List<EZLinkedListIterator<T>> freeIters = new List<EZLinkedListIterator<T>>();

	protected T head;

	protected T cur;

	protected T nextItem;

	protected int count;

	public int Count
	{
		get
		{
			return count;
		}
	}

	public bool Empty
	{
		get
		{
			return head == null;
		}
	}

	public T Head
	{
		get
		{
			return head;
		}
	}

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

	public EZLinkedListIterator<T> Begin()
	{
		EZLinkedListIterator<T> eZLinkedListIterator;
		if (freeIters.Count > 0)
		{
			eZLinkedListIterator = freeIters[freeIters.Count - 1];
			freeIters.RemoveAt(freeIters.Count - 1);
		}
		else
		{
			eZLinkedListIterator = new EZLinkedListIterator<T>();
		}
		iters.Add(eZLinkedListIterator);
		eZLinkedListIterator.Begin(this);
		return eZLinkedListIterator;
	}

	public void End(EZLinkedListIterator<T> it)
	{
		if (iters.Remove(it))
		{
			freeIters.Add(it);
		}
	}

	public bool Rewind()
	{
		cur = head;
		if (cur != null)
		{
			nextItem = cur.next;
			return true;
		}
		nextItem = default(T);
		return false;
	}

	public bool MoveNext()
	{
		cur = nextItem;
		if (cur != null)
		{
			nextItem = cur.next;
		}
		return cur != null;
	}

	public void Add(T item)
	{
		if (head != null)
		{
			item.next = head;
			head.prev = item;
		}
		head = item;
		count++;
	}

	public void Remove(T item)
	{
		if (head == null || item == null)
		{
			return;
		}
		if (head.Equals(item))
		{
			head = item.next;
			if (iters.Count > 0)
			{
				for (int i = 0; i < iters.Count; i++)
				{
					if (iters[i].Current != null && iters[i].Current.Equals(item))
					{
						iters[i].Current = item.next;
					}
				}
			}
		}
		else
		{
			if (iters.Count > 0)
			{
				for (int j = 0; j < iters.Count; j++)
				{
					if (iters[j].Current != null && iters[j].Current.Equals(item))
					{
						iters[j].Current = item.prev;
					}
				}
			}
			if (item.next != null)
			{
				if (item.prev != null)
				{
					T prev = item.prev;
					prev.next = item.next;
				}
				T next = item.next;
				next.prev = item.prev;
			}
			else if (item.prev != null)
			{
				T prev2 = item.prev;
				prev2.next = default(T);
			}
		}
		item.next = default(T);
		item.prev = default(T);
		count--;
	}

	public void Clear()
	{
		count = 0;
		if (head != null)
		{
			cur = head;
			head = default(T);
			do
			{
				T next = cur.next;
				cur.prev = default(T);
				cur.next = default(T);
				cur = next;
			}
			while (cur != null);
		}
	}
}
