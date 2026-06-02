using System.IO;
using UnityEngine;

public class AkBankPath
{
	private static string basePath;

	private static bool isToUsePosixPathSeparator;

	private static bool isToAppendTrailingPathSeparator;

	static AkBankPath()
	{
		basePath = Path.Combine("Audio", "GeneratedSoundBanks");
		isToUsePosixPathSeparator = false;
		isToAppendTrailingPathSeparator = true;
		isToUsePosixPathSeparator = false;
	}

	public static void UsePosixPath()
	{
		isToUsePosixPathSeparator = true;
	}

	public static void UsePlatformSpecificPath()
	{
		isToUsePosixPathSeparator = false;
	}

	public static void SetToAppendTrailingPathSeparator(bool add)
	{
		isToAppendTrailingPathSeparator = add;
	}

	public static bool Exists(string path)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		return directoryInfo.Exists;
	}

	public static string GetBasePath()
	{
		return basePath;
	}

	public static string GetFullBasePath()
	{
		string path = Path.Combine(Application.streamingAssetsPath, basePath);
		LazyAppendTrailingSeparator(ref path);
		LazyConvertPathConvention(ref path);
		return path;
	}

	public static string GetPlatformBasePath()
	{
		string path = Path.Combine(GetFullBasePath(), GetPlatformSubDirectory());
		LazyAppendTrailingSeparator(ref path);
		LazyConvertPathConvention(ref path);
		return path;
	}

	public static string GetPlatformSubDirectory()
	{
		string text = "Undefined platform sub-folder";
		return "iOS";
	}

	public static void LazyConvertPathConvention(ref string path)
	{
		if (isToUsePosixPathSeparator)
		{
			ConvertToPosixPath(ref path);
		}
		else if (Path.DirectorySeparatorChar == '/')
		{
			ConvertToPosixPath(ref path);
		}
		else
		{
			ConvertToWindowsPath(ref path);
		}
	}

	public static void ConvertToWindowsPath(ref string path)
	{
		path.Trim();
		path = path.Replace("/", "\\");
		path = path.TrimStart('\\');
	}

	public static void ConvertToPosixPath(ref string path)
	{
		path.Trim();
		path = path.Replace("\\", "/");
		path = path.TrimStart('\\');
	}

	public static void LazyAppendTrailingSeparator(ref string path)
	{
		if (isToAppendTrailingPathSeparator && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			path += Path.DirectorySeparatorChar;
		}
	}
}
