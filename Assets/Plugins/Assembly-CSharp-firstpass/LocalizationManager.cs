using System.IO;
using System.Xml;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
	public enum Language
	{
		None = -1,
		English = 0,
		French = 1,
		Italian = 2,
		German = 3,
		Spanish = 4,
		Total = 5
	}

	private Language m_CurrentLanguage;

	public Language m_OverrideLanguage = Language.None;

	private static string[][] m_StringTables;

	private static int m_NumTables;

	private static int[] m_NumRows;

	public static string LocalizeString(string str)
	{
		if (str.StartsWith("<stringid:") || str.StartsWith("(stringid:"))
		{
			int num = str.IndexOf(':');
			int num2 = str.IndexOf(',');
			int num3 = str.IndexOf('>');
			if (num3 == -1)
			{
				num3 = str.IndexOf(')');
			}
			int num4 = int.Parse(str.Substring(num + 1, num2 - (num + 1))) - 1;
			int num5 = int.Parse(str.Substring(num2 + 1, num3 - (num2 + 1))) - 1;
			if (num4 >= m_NumTables)
			{
				return str;
			}
			if (num5 >= m_NumRows[num4])
			{
				return str;
			}
			return m_StringTables[num4][num5];
		}
		return str;
	}

	private void Start()
	{
		m_CurrentLanguage = Language.English;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.French:
			m_CurrentLanguage = Language.French;
			break;
		case SystemLanguage.Italian:
			m_CurrentLanguage = Language.Italian;
			break;
		case SystemLanguage.German:
			m_CurrentLanguage = Language.German;
			break;
		case SystemLanguage.Spanish:
			m_CurrentLanguage = Language.Spanish;
			break;
		}
		if (m_OverrideLanguage != Language.None)
		{
			m_CurrentLanguage = m_OverrideLanguage;
		}
		int num = 0;
		while (true)
		{
			string path = string.Format("String Tables/stringtable{0:D2}", num + 1);
			TextAsset textAsset = Resources.Load(path) as TextAsset;
			if (textAsset == null)
			{
				break;
			}
			num++;
		}
		m_StringTables = new string[num][];
		m_NumTables = num;
		m_NumRows = new int[m_NumTables];
		for (int i = 0; i < num; i++)
		{
			string path2 = string.Format("String Tables/stringtable{0:D2}", i + 1);
			TextAsset textAsset2 = Resources.Load(path2) as TextAsset;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(textAsset2.text));
			xmlTextReader.ReadToFollowing("Table");
			int num2 = int.Parse(xmlTextReader.GetAttribute("ss:ExpandedRowCount"));
			m_StringTables[i] = new string[num2];
			m_NumRows[i] = num2;
			for (int j = 0; j < num2; j++)
			{
				xmlTextReader.ReadToFollowing("Row");
				for (int k = 0; k < 5; k++)
				{
					xmlTextReader.ReadToFollowing("Cell");
					if (k == (int)m_CurrentLanguage)
					{
						if (xmlTextReader.IsEmptyElement)
						{
							m_StringTables[i][j] = string.Empty;
							break;
						}
						xmlTextReader.ReadToFollowing("Data");
						m_StringTables[i][j] = xmlTextReader.ReadElementString();
						break;
					}
				}
			}
		}
	}
}
