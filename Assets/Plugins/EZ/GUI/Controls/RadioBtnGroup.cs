using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioBtnGroup
{
	private static List<RadioBtnGroup> groups = new List<RadioBtnGroup>();

	public int groupID;

	public ArrayList buttons = new ArrayList();

	public RadioBtnGroup(int id)
	{
		groupID = id;
		groups.Add(this);
	}

	~RadioBtnGroup()
	{
		groups.Remove(this);
	}

	public static IRadioButton GetSelected(GameObject go)
	{
		return GetSelected(go.transform.GetHashCode());
	}

	public static IRadioButton GetSelected(int id)
	{
		RadioBtnGroup radioBtnGroup = null;
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].groupID == id)
			{
				radioBtnGroup = groups[i];
				break;
			}
		}
		if (radioBtnGroup == null)
		{
			return null;
		}
		for (int j = 0; j < radioBtnGroup.buttons.Count; j++)
		{
			if (((IRadioButton)radioBtnGroup.buttons[j]).Value)
			{
				return (IRadioButton)radioBtnGroup.buttons[j];
			}
		}
		return null;
	}

	public static RadioBtnGroup GetGroup(int id)
	{
		RadioBtnGroup radioBtnGroup = null;
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].groupID == id)
			{
				radioBtnGroup = groups[i];
				break;
			}
		}
		if (radioBtnGroup == null)
		{
			radioBtnGroup = new RadioBtnGroup(id);
		}
		return radioBtnGroup;
	}
}
