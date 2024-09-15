using System;

[Serializable]
public class EZTransitionList
{
	public EZTransition[] list = new EZTransition[0];

	public EZTransitionList(EZTransition[] l)
	{
		list = l;
	}

	public EZTransitionList()
	{
		list = new EZTransition[0];
	}

	public void Clone(int source, bool force)
	{
		if (source >= list.Length)
		{
			return;
		}
		EZTransition eZTransition = list[source];
		if (!force && eZTransition.animationTypes.Length < 1)
		{
			return;
		}
		for (int i = 0; i < list.Length; i++)
		{
			if (i != source && (force || !list[i].initialized))
			{
				list[i].Copy(eZTransition);
			}
		}
	}

	public void CloneAsNeeded(int source)
	{
		Clone(source, false);
	}

	public void CloneAll(int source)
	{
		Clone(source, true);
	}

	public void MarkAllInitialized()
	{
		for (int i = 0; i < list.Length; i++)
		{
			list[i].initialized = true;
		}
	}

	public string[] GetTransitionNames()
	{
		if (list == null)
		{
			return null;
		}
		string[] array = new string[list.Length];
		for (int i = 0; i < list.Length; i++)
		{
			array[i] = list[i].name;
		}
		return array;
	}

	public void CopyTo(EZTransitionList target)
	{
		CopyTo(target, false);
	}

	public void CopyTo(EZTransitionList target, bool copyInit)
	{
		if (target == null || target.list == null)
		{
			return;
		}
		for (int i = 0; i < list.Length && i < target.list.Length; i++)
		{
			if (target.list[i] != null)
			{
				target.list[i].Copy(list[i]);
				if (copyInit)
				{
					target.list[i].initialized = list[i].initialized;
				}
			}
		}
	}

	public void CopyToNew(EZTransitionList target)
	{
		CopyToNew(target, false);
	}

	public void CopyToNew(EZTransitionList target, bool copyInit)
	{
		if (target != null && target.list != null)
		{
			target.list = new EZTransition[list.Length];
			for (int i = 0; i < target.list.Length; i++)
			{
				target.list[i] = new EZTransition(list[i].name);
			}
			CopyTo(target, copyInit);
		}
	}
}
