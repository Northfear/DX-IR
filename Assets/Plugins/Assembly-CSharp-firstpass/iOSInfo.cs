using System.Runtime.InteropServices;
using UnityEngine;

public class iOSInfo
{
	public static bool isIPadMini
	{
		get
		{
			string deviceModel = GetDeviceModel();
			if (!deviceModel.StartsWith("iPad2,"))
			{
				return false;
			}
			if (GetMinorNumber(deviceModel) >= 5)
			{
				return true;
			}
			return false;
		}
	}

	public static bool isIPad4Gen
	{
		get
		{
			string deviceModel = GetDeviceModel();
			if (!deviceModel.StartsWith("iPad3,"))
			{
				return false;
			}
			if (GetMinorNumber(deviceModel) >= 4)
			{
				return true;
			}
			return false;
		}
	}

	public static float dpi
	{
		get
		{
			return (!isIPadMini) ? Screen.dpi : 167f;
		}
	}

	[DllImport("__Internal")]
	public static extern string GetDeviceModel();

	protected static int GetMinorNumber(string hwid)
	{
		return int.Parse(hwid.Substring(hwid.IndexOf(',') + 1));
	}
}
