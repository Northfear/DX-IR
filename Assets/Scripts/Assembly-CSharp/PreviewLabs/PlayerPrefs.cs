using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PreviewLabs
{
	public static class PlayerPrefs
	{
		private const string PARAMETERS_SEPERATOR = ";";

		private const string KEY_VALUE_SEPERATOR = ":";

		private static Hashtable playerPrefsHashtable;

		private static bool hashTableChanged;

		private static string serializedOutput;

		private static string serializedInput;

		private static readonly string fileName;

		static PlayerPrefs()
		{
			playerPrefsHashtable = new Hashtable();
			hashTableChanged = false;
			serializedOutput = string.Empty;
			serializedInput = string.Empty;
			fileName = Application.persistentDataPath + "/PlayerPrefs.txt";
			StreamReader streamReader = null;
			if (File.Exists(fileName))
			{
				streamReader = new StreamReader(fileName);
				serializedInput = streamReader.ReadLine();
				Deserialize();
				streamReader.Close();
			}
		}

		public static bool HasKey(string key)
		{
			return playerPrefsHashtable.ContainsKey(key);
		}

		public static void SetString(string key, string value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetInt(string key, int value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetFloat(string key, float value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetBool(string key, bool value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static string GetString(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			return null;
		}

		public static string GetString(string key, string defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static int GetInt(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int)playerPrefsHashtable[key];
			}
			return 0;
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int)playerPrefsHashtable[key];
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static float GetFloat(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float)playerPrefsHashtable[key];
			}
			return 0f;
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float)playerPrefsHashtable[key];
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static bool GetBool(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)playerPrefsHashtable[key];
			}
			return false;
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)playerPrefsHashtable[key];
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static void DeleteKey(string key)
		{
			playerPrefsHashtable.Remove(key);
		}

		public static void DeleteAll()
		{
			playerPrefsHashtable.Clear();
		}

		public static void Flush()
		{
			if (hashTableChanged)
			{
				Serialize();
				StreamWriter streamWriter = null;
				streamWriter = File.CreateText(fileName);
				if (streamWriter == null)
				{
					Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
				}
				streamWriter.WriteLine(serializedOutput);
				streamWriter.Close();
				serializedOutput = string.Empty;
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator enumerator = playerPrefsHashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (serializedOutput != string.Empty)
				{
					serializedOutput += " ; ";
				}
				string text = serializedOutput;
				serializedOutput = text + EscapeNonSeperators(enumerator.Key.ToString()) + " : " + EscapeNonSeperators(enumerator.Value.ToString()) + " : " + enumerator.Value.GetType();
			}
		}

		private static void Deserialize()
		{
			string[] array = serializedInput.Split(new string[1] { " ; " }, StringSplitOptions.None);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(new string[1] { " : " }, StringSplitOptions.None);
				playerPrefsHashtable.Add(DeEscapeNonSeperators(array3[0]), GetTypeValue(array3[2], DeEscapeNonSeperators(array3[1])));
				if (array3.Length > 3)
				{
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + array3.Length + " elements");
				}
			}
		}

		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(":", "\\:");
			inputToEscape = inputToEscape.Replace(";", "\\;");
			return inputToEscape;
		}

		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\:", ":");
			inputToDeEscape = inputToDeEscape.Replace("\\;", ";");
			return inputToDeEscape;
		}

		public static object GetTypeValue(string typeName, string value)
		{
			switch (typeName)
			{
			case "System.String":
				return value.ToString();
			case "System.Int32":
				return Convert.ToInt32(value);
			case "System.Boolean":
				return Convert.ToBoolean(value);
			case "System.Single":
				return Convert.ToSingle(value);
			default:
				Debug.LogError("Unsupported type: " + typeName);
				return null;
			}
		}
	}
}
