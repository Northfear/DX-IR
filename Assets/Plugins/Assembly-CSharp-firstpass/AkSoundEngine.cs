using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkSoundEngine
{
	public const int AK_SIMD_ALIGNMENT = 16;

	public const int AK_BUFFER_ALIGNMENT = 16;

	public const int AK_MAX_PATH = 260;

	public const int AK_BANK_PLATFORM_DATA_ALIGNMENT = 16;

	public const uint AK_INVALID_PLUGINID = uint.MaxValue;

	public const uint AK_INVALID_GAME_OBJECT = uint.MaxValue;

	public const uint AK_INVALID_UNIQUE_ID = 0u;

	public const uint AK_INVALID_RTPC_ID = 0u;

	public const uint AK_INVALID_LISTENER_INDEX = uint.MaxValue;

	public const uint AK_INVALID_PLAYING_ID = 0u;

	public const uint AK_DEFAULT_SWITCH_STATE = 0u;

	public const uint AK_INVALID_POOL_ID = uint.MaxValue;

	public const int AK_DEFAULT_POOL_ID = -1;

	public const uint AK_INVALID_ENV_ID = 0u;

	public const uint AK_INVALID_FILE_ID = uint.MaxValue;

	public const uint AK_INVALID_DEVICE_ID = uint.MaxValue;

	public const uint AK_INVALID_BANK_ID = 0u;

	public const uint AK_FALLBACK_ARGUMENTVALUE_ID = 0u;

	public const uint AK_DEFAULT_PRIORITY = 50u;

	public const uint AK_MIN_PRIORITY = 0u;

	public const uint AK_MAX_PRIORITY = 100u;

	public const uint AK_DEFAULT_BANK_IO_PRIORITY = 50u;

	public const double AK_DEFAULT_BANK_THROUGHPUT = 1048.576;

	public const int AKCURVEINTERPOLATION_NUM_STORAGE_BIT = 5;

	public const int AK_MAX_AUX_PER_OBJ = 4;

	public const int AK_MAX_AUX_SUPPORTED = 8;

	public const int AK_NUM_LISTENERS = 8;

	public const int AK_MAX_LANGUAGE_NAME_SIZE = 32;

	public const int AKCOMPANYID_AUDIOKINETIC = 0;

	public const int AKCOMPANYID_AUDIOKINETIC_EXTERNAL = 1;

	public const int AKCOMPANYID_MCDSP = 256;

	public const int AKCOMPANYID_WAVEARTS = 257;

	public const int AKCOMPANYID_PHONETICARTS = 258;

	public const int AKCOMPANYID_IZOTOPE = 259;

	public const int AKCODECID_BANK = 0;

	public const int AKCODECID_PCM = 1;

	public const int AKCODECID_ADPCM = 2;

	public const int AKCODECID_XMA = 3;

	public const int AKCODECID_VORBIS = 4;

	public const int AKCODECID_WIIADPCM = 5;

	public const int AKCODECID_PCMEX = 7;

	public const int AKCODECID_EXTERNAL_SOURCE = 8;

	public const int AKCODECID_XWMA = 9;

	public const int AKCODECID_AAC = 10;

	public const int AKCODECID_FILE_PACKAGE = 11;

	public const int AKCODECID_ATRAC9 = 12;

	public const int AKCODECID_VAG = 13;

	public const int AKCODECID_PROFILERCAPTURE = 14;

	public const int AK_WAVE_FORMAT_VAG = 65531;

	public const int AK_WAVE_FORMAT_AT9 = 65532;

	public const int AK_WAVE_FORMAT_VORBIS = 65535;

	public const int AK_WAVE_FORMAT_AAC = 43712;

	public const int AK_OS_STRUCT_ALIGN = 4;

	public static uint AK_INVALID_AUX_ID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_AUX_ID_get();
		}
	}

	public static uint AK_INVALID_CHANNELMASK
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_CHANNELMASK_get();
		}
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, DynamicSequenceType in_eDynamicSequenceType)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_0(instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), (int)in_eDynamicSequenceType);
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_1(instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_2(instanceID, in_uFlags);
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_3(instanceID);
	}

	public static AKRESULT DynamicSequenceClose(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceClose(in_playingID);
	}

	public static AKRESULT DynamicSequencePlay(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequencePlay(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequencePlay(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequencePause(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequencePause(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequencePause(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequenceResume(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequenceResume(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequenceResume(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequenceStop(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequenceStop(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequenceStop(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequenceBreak(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceBreak(in_playingID);
	}

	public static Playlist DynamicSequenceLockPlaylist(uint in_playingID)
	{
		IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_DynamicSequenceLockPlaylist(in_playingID);
		return (!(intPtr == IntPtr.Zero)) ? new Playlist(intPtr, false) : null;
	}

	public static AKRESULT DynamicSequenceUnlockPlaylist(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceUnlockPlaylist(in_playingID);
	}

	public static uint GetSpeakerConfiguration()
	{
		return AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration();
	}

	public static AkPanningRule GetPanningRule()
	{
		return (AkPanningRule)AkSoundEnginePINVOKE.CSharp_GetPanningRule();
	}

	public static AKRESULT SetPanningRule(AkPanningRule in_panningRule)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetPanningRule((int)in_panningRule);
	}

	public static AKRESULT SetVolumeThreshold(float in_fVolumeThresholdDB)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetVolumeThreshold(in_fVolumeThresholdDB);
	}

	public static AKRESULT SetMaxNumVoicesLimit(ushort in_maxNumberVoices)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMaxNumVoicesLimit(in_maxNumberVoices);
	}

	public static AKRESULT RenderAudio()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RenderAudio();
	}

	public static uint GetIDFromString(string in_pszString)
	{
		return AkSoundEnginePINVOKE.CSharp_GetIDFromString__SWIG_0(in_pszString);
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources, uint in_PlayingID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_0(in_eventID, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources), in_PlayingID);
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_1(in_eventID, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_2(in_eventID, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_cExternals);
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_3(in_eventID, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_4(in_eventID, instanceID, in_uFlags);
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_5(in_eventID, instanceID);
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources, uint in_PlayingID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_6(in_pszEventName, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources), in_PlayingID);
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_7(in_pszEventName, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_8(in_pszEventName, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_cExternals);
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		in_pCookie = new AkCallbackManager.EventCallbackPackage(in_pfnCallback, in_pCookie);
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_9(in_pszEventName, instanceID, in_uFlags, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_10(in_pszEventName, instanceID, in_uFlags);
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_11(in_pszEventName, instanceID);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve, uint in_PlayingID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_0(in_eventID, (int)in_ActionType, instanceID, in_uTransitionDuration, (int)in_eFadeCurve, in_PlayingID);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_1(in_eventID, (int)in_ActionType, instanceID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_2(in_eventID, (int)in_ActionType, instanceID, in_uTransitionDuration);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_3(in_eventID, (int)in_ActionType, instanceID);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_4(in_eventID, (int)in_ActionType);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve, uint in_PlayingID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_5(in_pszEventName, (int)in_ActionType, instanceID, in_uTransitionDuration, (int)in_eFadeCurve, in_PlayingID);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_6(in_pszEventName, (int)in_ActionType, instanceID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_7(in_pszEventName, (int)in_ActionType, instanceID, in_uTransitionDuration);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_8(in_pszEventName, (int)in_ActionType, instanceID);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_9(in_pszEventName, (int)in_ActionType);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_0(in_eventID, instanceID, in_iPosition, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_1(in_eventID, instanceID, in_iPosition);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_2(in_pszEventName, instanceID, in_iPosition, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_3(in_pszEventName, instanceID, in_iPosition);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_6(in_eventID, instanceID, in_fPercent, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_7(in_eventID, instanceID, in_fPercent);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_8(in_pszEventName, instanceID, in_fPercent, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_9(in_pszEventName, instanceID, in_fPercent);
	}

	public static void CancelEventCallbackCookie(object in_pCookie)
	{
		AkSoundEnginePINVOKE.CSharp_CancelEventCallbackCookie((IntPtr)in_pCookie.GetHashCode());
	}

	public static void CancelEventCallback(uint in_playingID)
	{
		AkSoundEnginePINVOKE.CSharp_CancelEventCallback(in_playingID);
	}

	public static AKRESULT GetSourcePlayPosition(uint in_PlayingID, out int out_puPosition, bool in_bExtrapolate)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSourcePlayPosition__SWIG_0(in_PlayingID, out out_puPosition, in_bExtrapolate);
	}

	public static AKRESULT GetSourcePlayPosition(uint in_PlayingID, out int out_puPosition)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSourcePlayPosition__SWIG_1(in_PlayingID, out out_puPosition);
	}

	public static void StopAll(GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		AkSoundEnginePINVOKE.CSharp_StopAll__SWIG_0(instanceID);
	}

	public static void StopAll()
	{
		AkSoundEnginePINVOKE.CSharp_StopAll__SWIG_1();
	}

	public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration)
	{
		AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static void StopPlayingID(uint in_playingID)
	{
		AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_2(in_playingID);
	}

	public static AKRESULT RegisterGameObj(GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RegisterGameObj__SWIG_0(instanceID);
	}

	public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, string in_pszObjName)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RegisterGameObj__SWIG_1(instanceID, in_pszObjName);
	}

	public static AKRESULT UnregisterGameObj(GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnregisterGameObj(instanceID);
	}

	public static AKRESULT UnregisterAllGameObj()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnregisterAllGameObj();
	}

	public static AKRESULT SetMultiplePositions(GameObject in_GameObjectID, AkPositionArray in_pPositions, ushort in_NumPositions, MultiPositionType in_eMultiPositionType)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMultiplePositions__SWIG_0(instanceID, in_pPositions.m_Buffer, in_NumPositions, (int)in_eMultiPositionType);
	}

	public static AKRESULT SetMultiplePositions(GameObject in_GameObjectID, AkPositionArray in_pPositions, ushort in_NumPositions)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMultiplePositions__SWIG_1(instanceID, in_pPositions.m_Buffer, in_NumPositions);
	}

	public static AKRESULT SetAttenuationScalingFactor(GameObject in_GameObjectID, float in_fAttenuationScalingFactor)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetAttenuationScalingFactor(instanceID, in_fAttenuationScalingFactor);
	}

	public static AKRESULT SetListenerScalingFactor(uint in_uListenerIndex, float in_fListenerScalingFactor)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerScalingFactor(in_uListenerIndex, in_fListenerScalingFactor);
	}

	public static AKRESULT ClearBanks()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ClearBanks();
	}

	public static AKRESULT SetBankLoadIOSettings(float in_fThroughput, sbyte in_priority)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBankLoadIOSettings(in_fThroughput, in_priority);
	}

	public static AKRESULT LoadBank(string in_pszString, int in_memPoolId, out uint out_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_0(in_pszString, in_memPoolId, out out_bankID);
	}

	public static AKRESULT LoadBank(uint in_bankID, int in_memPoolId)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_1(in_bankID, in_memPoolId);
	}

	public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, out uint out_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_2(in_pInMemoryBankPtr, in_uInMemoryBankSize, out out_bankID);
	}

	public static AKRESULT LoadBank(string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_memPoolId, out uint out_bankID)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_3(in_pszString, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_memPoolId, out out_bankID);
	}

	public static AKRESULT LoadBank(uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_memPoolId)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_4(in_bankID, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), in_memPoolId);
	}

	public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, out uint out_bankID)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_5(in_pInMemoryBankPtr, in_uInMemoryBankSize, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), out out_bankID);
	}

	public static AKRESULT UnloadBank(string in_pszString, out int out_pMemPoolId)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_0(in_pszString, out out_pMemPoolId);
	}

	public static AKRESULT UnloadBank(string in_pszString)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_1(in_pszString);
	}

	public static AKRESULT UnloadBank(uint in_bankID, out int out_pMemPoolId)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_4(in_bankID, out out_pMemPoolId);
	}

	public static AKRESULT UnloadBank(uint in_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_5(in_bankID);
	}

	public static AKRESULT UnloadBank(string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_6(in_pszString, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static AKRESULT UnloadBank(uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_8(in_bankID, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static void CancelBankCallbackCookie(object in_pCookie)
	{
		AkSoundEnginePINVOKE.CSharp_CancelBankCallbackCookie((IntPtr)in_pCookie.GetHashCode());
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkBankContent in_uFlags)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_0((int)in_PreparationType, in_pszString, (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_1((int)in_PreparationType, in_pszString);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkBankContent in_uFlags)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_4((int)in_PreparationType, in_bankID, (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_5((int)in_PreparationType, in_bankID);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, AkBankContent in_uFlags)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_6((int)in_PreparationType, in_pszString, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_7((int)in_PreparationType, in_pszString, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, AkBankContent in_uFlags)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_10((int)in_PreparationType, in_bankID, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode(), (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_11((int)in_PreparationType, in_bankID, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static AKRESULT ClearPreparedEvents()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ClearPreparedEvents();
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, string[] in_ppszString, uint in_uNumEvent)
	{
		int num = 0;
		foreach (string text in in_ppszString)
		{
			num += text.Length + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszString.Length);
		IntPtr destination = (IntPtr)(intPtr.ToInt64() + num2);
		foreach (string text2 in in_ppszString)
		{
			Marshal.Copy(text2.ToCharArray(), 0, destination, text2.Length);
			destination = (IntPtr)(destination.ToInt64() + num2 * text2.Length);
			Marshal.WriteInt16(destination, 0);
			destination = (IntPtr)(destination.ToInt64() + num2);
		}
		try
		{
			return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_0((int)in_PreparationType, intPtr, in_uNumEvent);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, uint[] in_pEventID, uint in_uNumEvent)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_1((int)in_PreparationType, in_pEventID, in_uNumEvent);
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, string[] in_ppszString, uint in_uNumEvent, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		int num = 0;
		foreach (string text in in_ppszString)
		{
			num += text.Length + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszString.Length);
		IntPtr destination = (IntPtr)(intPtr.ToInt64() + num2);
		foreach (string text2 in in_ppszString)
		{
			Marshal.Copy(text2.ToCharArray(), 0, destination, text2.Length);
			destination = (IntPtr)(destination.ToInt64() + num2 * text2.Length);
			Marshal.WriteInt16(destination, 0);
			destination = (IntPtr)(destination.ToInt64() + num2);
		}
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		try
		{
			return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_2((int)in_PreparationType, intPtr, in_uNumEvent, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, uint[] in_pEventID, uint in_uNumEvent, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_3((int)in_PreparationType, in_pEventID, in_uNumEvent, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, string in_pszGroupName, string[] in_ppszGameSyncName, uint in_uNumGameSyncs)
	{
		int num = 0;
		foreach (string text in in_ppszGameSyncName)
		{
			num += text.Length + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszGameSyncName.Length);
		IntPtr destination = (IntPtr)(intPtr.ToInt64() + num2);
		foreach (string text2 in in_ppszGameSyncName)
		{
			Marshal.Copy(text2.ToCharArray(), 0, destination, text2.Length);
			destination = (IntPtr)(destination.ToInt64() + num2 * text2.Length);
			Marshal.WriteInt16(destination, 0);
			destination = (IntPtr)(destination.ToInt64() + num2);
		}
		try
		{
			return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_0((int)in_PreparationType, (int)in_eGameSyncType, in_pszGroupName, intPtr, in_uNumGameSyncs);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, uint in_GroupID, uint[] in_paGameSyncID, uint in_uNumGameSyncs)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_1((int)in_PreparationType, (int)in_eGameSyncType, in_GroupID, in_paGameSyncID, in_uNumGameSyncs);
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, string in_pszGroupName, string[] in_ppszGameSyncName, uint in_uNumGameSyncs, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		int num = 0;
		foreach (string text in in_ppszGameSyncName)
		{
			num += text.Length + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszGameSyncName.Length);
		IntPtr destination = (IntPtr)(intPtr.ToInt64() + num2);
		foreach (string text2 in in_ppszGameSyncName)
		{
			Marshal.Copy(text2.ToCharArray(), 0, destination, text2.Length);
			destination = (IntPtr)(destination.ToInt64() + num2 * text2.Length);
			Marshal.WriteInt16(destination, 0);
			destination = (IntPtr)(destination.ToInt64() + num2);
		}
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		try
		{
			return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_2((int)in_PreparationType, (int)in_eGameSyncType, in_pszGroupName, intPtr, in_uNumGameSyncs, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, uint in_GroupID, uint[] in_paGameSyncID, uint in_uNumGameSyncs, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_3((int)in_PreparationType, (int)in_eGameSyncType, in_GroupID, in_paGameSyncID, in_uNumGameSyncs, (IntPtr)0, (IntPtr)in_pCookie.GetHashCode());
	}

	public static AKRESULT SetActiveListeners(GameObject in_GameObjectID, uint in_uListenerMask)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetActiveListeners(instanceID, in_uListenerMask);
	}

	public static AKRESULT SetListenerSpatialization(uint in_uIndex, bool in_bSpatialized, AkSpeakerVolumes in_pVolumeOffsets)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerSpatialization__SWIG_0(in_uIndex, in_bSpatialized, AkSpeakerVolumes.getCPtr(in_pVolumeOffsets));
	}

	public static AKRESULT SetListenerSpatialization(uint in_uIndex, bool in_bSpatialized)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerSpatialization__SWIG_1(in_uIndex, in_bSpatialized);
	}

	public static AKRESULT SetListenerPipeline(uint in_uIndex, bool in_bAudio, bool in_bMotion)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerPipeline(in_uIndex, in_bAudio, in_bMotion);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_0(in_rtpcID, in_value, instanceID, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_1(in_rtpcID, in_value, instanceID, in_uValueChangeDuration);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_2(in_rtpcID, in_value, instanceID);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_3(in_rtpcID, in_value);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_4(in_pszRtpcName, in_value, instanceID, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_5(in_pszRtpcName, in_value, instanceID, in_uValueChangeDuration);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_6(in_pszRtpcName, in_value, instanceID);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_7(in_pszRtpcName, in_value);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_0(in_rtpcID, instanceID, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_1(in_rtpcID, instanceID, in_uValueChangeDuration);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_2(in_rtpcID, instanceID);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_3(in_rtpcID);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_4(in_pszRtpcName, instanceID, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_5(in_pszRtpcName, instanceID, in_uValueChangeDuration);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_6(in_pszRtpcName, instanceID);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_7(in_pszRtpcName);
	}

	public static AKRESULT SetSwitch(uint in_switchGroup, uint in_switchState, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSwitch__SWIG_0(in_switchGroup, in_switchState, instanceID);
	}

	public static AKRESULT SetSwitch(string in_pszSwitchGroup, string in_pszSwitchState, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSwitch__SWIG_1(in_pszSwitchGroup, in_pszSwitchState, instanceID);
	}

	public static AKRESULT PostTrigger(uint in_triggerID, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostTrigger__SWIG_0(in_triggerID, instanceID);
	}

	public static AKRESULT PostTrigger(string in_pszTrigger, GameObject in_gameObjectID)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostTrigger__SWIG_1(in_pszTrigger, instanceID);
	}

	public static AKRESULT SetState(uint in_stateGroup, uint in_state)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetState__SWIG_0(in_stateGroup, in_state);
	}

	public static AKRESULT SetState(string in_pszStateGroup, string in_pszState)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetState__SWIG_1(in_pszStateGroup, in_pszState);
	}

	public static AKRESULT SetGameObjectAuxSendValues(GameObject in_gameObjectID, AkAuxSendArray in_aAuxSendValues, uint in_uNumSendValues)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetGameObjectAuxSendValues(instanceID, in_aAuxSendValues.m_Buffer, in_uNumSendValues);
	}

	public static AKRESULT SetGameObjectOutputBusVolume(GameObject in_gameObjectID, float in_fControlValue)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetGameObjectOutputBusVolume(instanceID, in_fControlValue);
	}

	public static AKRESULT SetAuxBusVolumes(uint in_AuxBusID, AkSpeakerVolumes in_sVolumes)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetAuxBusVolumes(in_AuxBusID, AkSpeakerVolumes.getCPtr(in_sVolumes));
	}

	public static AKRESULT SetActorMixerEffect(uint in_audioNodeID, uint in_uFXIndex, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetActorMixerEffect(in_audioNodeID, in_uFXIndex, in_shareSetID);
	}

	public static AKRESULT SetBusEffect(uint in_audioNodeID, uint in_uFXIndex, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBusEffect__SWIG_0(in_audioNodeID, in_uFXIndex, in_shareSetID);
	}

	public static AKRESULT SetBusEffect(string in_pszBusName, uint in_uFXIndex, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBusEffect__SWIG_1(in_pszBusName, in_uFXIndex, in_shareSetID);
	}

	public static AKRESULT SetObjectObstructionAndOcclusion(GameObject in_ObjectID, uint in_uListener, float in_fObstructionLevel, float in_fOcclusionLevel)
	{
		uint instanceID = (uint)in_ObjectID.GetInstanceID();
		if (in_ObjectID.GetComponent("AkGameObject") == null)
		{
			in_ObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetObjectObstructionAndOcclusion(instanceID, in_uListener, in_fObstructionLevel, in_fOcclusionLevel);
	}

	public static AKRESULT StartOutputCapture(string in_CaptureFileName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StartOutputCapture(in_CaptureFileName);
	}

	public static AKRESULT StopOutputCapture()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StopOutputCapture();
	}

	public static AKRESULT StartProfilerCapture(string in_CaptureFileName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StartProfilerCapture(in_CaptureFileName);
	}

	public static AKRESULT StopProfilerCapture()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StopProfilerCapture();
	}

	public static AKRESULT GetPlayingSegmentInfo(uint in_PlayingID, AkSegmentInfo out_segmentInfo, bool in_bExtrapolate)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPlayingSegmentInfo__SWIG_0(in_PlayingID, AkSegmentInfo.getCPtr(out_segmentInfo), in_bExtrapolate);
	}

	public static AKRESULT GetPlayingSegmentInfo(uint in_PlayingID, AkSegmentInfo out_segmentInfo)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPlayingSegmentInfo__SWIG_1(in_PlayingID, AkSegmentInfo.getCPtr(out_segmentInfo));
	}

	public static AKRESULT PostCode(ErrorCode in_eError, ErrorLevel in_eErrorLevel)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostCode((int)in_eError, (int)in_eErrorLevel);
	}

	public static AKRESULT PostString(string in_pszError, ErrorLevel in_eErrorLevel)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostString__SWIG_0(in_pszError, (int)in_eErrorLevel);
	}

	public static int GetTimeStamp()
	{
		return AkSoundEnginePINVOKE.CSharp_GetTimeStamp();
	}

	public static uint ResolveDialogueEvent(uint in_eventID, uint[] in_aArgumentValues, uint in_uNumArguments, uint in_idSequence)
	{
		return AkSoundEnginePINVOKE.CSharp_ResolveDialogueEvent__SWIG_0(in_eventID, in_aArgumentValues, in_uNumArguments, in_idSequence);
	}

	public static uint ResolveDialogueEvent(uint in_eventID, uint[] in_aArgumentValues, uint in_uNumArguments)
	{
		return AkSoundEnginePINVOKE.CSharp_ResolveDialogueEvent__SWIG_1(in_eventID, in_aArgumentValues, in_uNumArguments);
	}

	public static AKRESULT GetPosition(GameObject in_GameObjectID, AkSoundPosition out_rPosition)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPosition(instanceID, AkSoundPosition.getCPtr(out_rPosition));
	}

	public static AKRESULT GetActiveListeners(GameObject in_GameObjectID, out uint out_ruListenerMask)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetActiveListeners(instanceID, out out_ruListenerMask);
	}

	public static AKRESULT GetListenerPosition(uint in_uIndex, AkListenerPosition out_rPosition)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetListenerPosition(in_uIndex, AkListenerPosition.getCPtr(out_rPosition));
	}

	public static AKRESULT GetListenerSpatialization(uint in_uIndex, out int out_rbSpatialized, AkSpeakerVolumes out_rVolumeOffsets)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetListenerSpatialization(in_uIndex, out out_rbSpatialized, AkSpeakerVolumes.getCPtr(out_rVolumeOffsets));
	}

	public static AKRESULT GetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, out float out_rValue, ref int io_rValueType)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetRTPCValue__SWIG_0(in_rtpcID, instanceID, out out_rValue, ref io_rValueType);
	}

	public static AKRESULT GetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, out float out_rValue, ref int io_rValueType)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetRTPCValue__SWIG_1(in_pszRtpcName, instanceID, out out_rValue, ref io_rValueType);
	}

	public static AKRESULT GetSwitch(uint in_switchGroup, GameObject in_gameObjectID, out uint out_rSwitchState)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSwitch__SWIG_0(in_switchGroup, instanceID, out out_rSwitchState);
	}

	public static AKRESULT GetSwitch(string in_pstrSwitchGroupName, GameObject in_GameObj, out uint out_rSwitchState)
	{
		uint instanceID = (uint)in_GameObj.GetInstanceID();
		if (in_GameObj.GetComponent("AkGameObject") == null)
		{
			in_GameObj.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSwitch__SWIG_1(in_pstrSwitchGroupName, instanceID, out out_rSwitchState);
	}

	public static AKRESULT GetState(uint in_stateGroup, out uint out_rState)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetState__SWIG_0(in_stateGroup, out out_rState);
	}

	public static AKRESULT GetState(string in_pstrStateGroupName, out uint out_rState)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetState__SWIG_1(in_pstrStateGroupName, out out_rState);
	}

	public static AKRESULT GetGameObjectAuxSendValues(GameObject in_gameObjectID, AkAuxSendArray out_paAuxSendValues, ref uint io_ruNumSendValues)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetGameObjectAuxSendValues(instanceID, out_paAuxSendValues.m_Buffer, ref io_ruNumSendValues);
	}

	public static AKRESULT GetGameObjectDryLevelValue(GameObject in_gameObjectID, out float out_rfControlValue)
	{
		uint instanceID = (uint)in_gameObjectID.GetInstanceID();
		if (in_gameObjectID.GetComponent("AkGameObject") == null)
		{
			in_gameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetGameObjectDryLevelValue(instanceID, out out_rfControlValue);
	}

	public static AKRESULT GetAuxBusVolumes(uint in_AuxBusID, AkSpeakerVolumes out_rsVolumes)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetAuxBusVolumes(in_AuxBusID, AkSpeakerVolumes.getCPtr(out_rsVolumes));
	}

	public static AKRESULT GetObjectObstructionAndOcclusion(GameObject in_ObjectID, uint in_uListener, out float out_rfObstructionLevel, out float out_rfOcclusionLevel)
	{
		uint instanceID = (uint)in_ObjectID.GetInstanceID();
		if (in_ObjectID.GetComponent("AkGameObject") == null)
		{
			in_ObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetObjectObstructionAndOcclusion(instanceID, in_uListener, out out_rfObstructionLevel, out out_rfOcclusionLevel);
	}

	public static AKRESULT QueryAudioObjectIDs(uint in_eventID, ref uint io_ruNumItems, AkObjectInfo out_aObjectInfos)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_QueryAudioObjectIDs__SWIG_0(in_eventID, ref io_ruNumItems, AkObjectInfo.getCPtr(out_aObjectInfos));
	}

	public static AKRESULT QueryAudioObjectIDs(string in_pszEventName, ref uint io_ruNumItems, AkObjectInfo out_aObjectInfos)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_QueryAudioObjectIDs__SWIG_1(in_pszEventName, ref io_ruNumItems, AkObjectInfo.getCPtr(out_aObjectInfos));
	}

	public static AKRESULT GetPositioningInfo(uint in_ObjectID, AkPositioningInfo out_rPositioningInfo)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPositioningInfo(in_ObjectID, AkPositioningInfo.getCPtr(out_rPositioningInfo));
	}

	public static bool GetIsGameObjectActive(GameObject in_GameObjId)
	{
		uint instanceID = (uint)in_GameObjId.GetInstanceID();
		if (in_GameObjId.GetComponent("AkGameObject") == null)
		{
			in_GameObjId.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_GetIsGameObjectActive(instanceID);
	}

	public static float GetMaxRadius(GameObject in_GameObjId)
	{
		uint instanceID = (uint)in_GameObjId.GetInstanceID();
		if (in_GameObjId.GetComponent("AkGameObject") == null)
		{
			in_GameObjId.AddComponent("AkGameObject");
		}
		return AkSoundEnginePINVOKE.CSharp_GetMaxRadius(instanceID);
	}

	public static uint GetEventIDFromPlayingID(uint in_playingID)
	{
		return AkSoundEnginePINVOKE.CSharp_GetEventIDFromPlayingID(in_playingID);
	}

	public static uint GetGameObjectFromPlayingID(uint in_playingID)
	{
		return AkSoundEnginePINVOKE.CSharp_GetGameObjectFromPlayingID(in_playingID);
	}

	public static AKRESULT GetPlayingIDsFromGameObject(GameObject in_GameObjId, ref uint io_ruNumIDs, uint[] out_aPlayingIDs)
	{
		uint instanceID = (uint)in_GameObjId.GetInstanceID();
		if (in_GameObjId.GetComponent("AkGameObject") == null)
		{
			in_GameObjId.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPlayingIDsFromGameObject(instanceID, ref io_ruNumIDs, out_aPlayingIDs);
	}

	public static AKRESULT GetCustomPropertyValue(uint in_ObjectID, uint in_uPropID, out int out_iValue)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetCustomPropertyValue__SWIG_0(in_ObjectID, in_uPropID, out out_iValue);
	}

	public static AKRESULT GetCustomPropertyValue(uint in_ObjectID, uint in_uPropID, out float out_fValue)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetCustomPropertyValue__SWIG_1(in_ObjectID, in_uPropID, out out_fValue);
	}

	public static AKRESULT AddPlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID, IntPtr in_pDevice)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddPlayerMotionDevice__SWIG_0(in_iPlayerID, in_iCompanyID, in_iDeviceID, in_pDevice);
	}

	public static AKRESULT AddPlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddPlayerMotionDevice__SWIG_1(in_iPlayerID, in_iCompanyID, in_iDeviceID);
	}

	public static void RemovePlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID)
	{
		AkSoundEnginePINVOKE.CSharp_RemovePlayerMotionDevice(in_iPlayerID, in_iCompanyID, in_iDeviceID);
	}

	public static void SetPlayerListener(byte in_iPlayerID, byte in_iListener)
	{
		AkSoundEnginePINVOKE.CSharp_SetPlayerListener(in_iPlayerID, in_iListener);
	}

	public static void SetPlayerVolume(byte in_iPlayerID, float in_fVolume)
	{
		AkSoundEnginePINVOKE.CSharp_SetPlayerVolume(in_iPlayerID, in_fVolume);
	}

	public static void Term()
	{
		AkSoundEnginePINVOKE.CSharp_Term();
	}

	public static AKRESULT Init(AkMemSettings in_pMemSettings, AkStreamMgrSettings in_pStmSettings, AkDeviceSettings in_pDefaultDeviceSettings, AkInitSettings in_pSettings, AkPlatformInitSettings in_pPlatformSettings, AkMusicSettings in_pMusicSettings)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_Init(AkMemSettings.getCPtr(in_pMemSettings), AkStreamMgrSettings.getCPtr(in_pStmSettings), AkDeviceSettings.getCPtr(in_pDefaultDeviceSettings), AkInitSettings.getCPtr(in_pSettings), AkPlatformInitSettings.getCPtr(in_pPlatformSettings), AkMusicSettings.getCPtr(in_pMusicSettings));
	}

	public static void GetDefaultStreamSettings(AkStreamMgrSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultStreamSettings(AkStreamMgrSettings.getCPtr(out_settings));
	}

	public static void GetDefaultDeviceSettings(AkDeviceSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultDeviceSettings(AkDeviceSettings.getCPtr(out_settings));
	}

	public static void GetDefaultMusicSettings(AkMusicSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultMusicSettings(AkMusicSettings.getCPtr(out_settings));
	}

	public static void GetDefaultInitSettings(AkInitSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultInitSettings(AkInitSettings.getCPtr(out_settings));
	}

	public static void GetDefaultPlatformInitSettings(AkPlatformInitSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultPlatformInitSettings(AkPlatformInitSettings.getCPtr(out_settings));
	}

	public static AKRESULT SetBasePath(string in_pszBasePath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBasePath(in_pszBasePath);
	}

	public static AKRESULT SetBankPath(string in_pszBankPath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBankPath(in_pszBankPath);
	}

	public static AKRESULT SetAudioSrcPath(string in_pszAudioSrcPath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetAudioSrcPath(in_pszAudioSrcPath);
	}

	public static AKRESULT SetCurrentLanguage(string in_pszAudioSrcPath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetCurrentLanguage(in_pszAudioSrcPath);
	}

	public static AKRESULT SetObjectPosition(GameObject in_GameObjectID, float PosX, float PosY, float PosZ, float OrientationX, float OrientationY, float OrientationZ)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetObjectPosition(instanceID, PosX, PosY, PosZ, OrientationX, OrientationY, OrientationZ);
	}

	public static AKRESULT SetObjectPositionWithListener(GameObject in_GameObjectID, float PosX, float PosY, float PosZ, float OrientationX, float OrientationY, float OrientationZ, uint in_ulListenerIndex)
	{
		uint instanceID = (uint)in_GameObjectID.GetInstanceID();
		if (in_GameObjectID.GetComponent("AkGameObject") == null)
		{
			in_GameObjectID.AddComponent("AkGameObject");
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetObjectPositionWithListener(instanceID, PosX, PosY, PosZ, OrientationX, OrientationY, OrientationZ, in_ulListenerIndex);
	}

	public static AKRESULT SetListenerPosition(float FrontX, float FrontY, float FrontZ, float TopX, float TopY, float TopZ, float PosX, float PosY, float PosZ, uint in_ulListenerIndex)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerPosition(FrontX, FrontY, FrontZ, TopX, TopY, TopZ, PosX, PosY, PosZ, in_ulListenerIndex);
	}

	public static bool IsInitialized()
	{
		return AkSoundEnginePINVOKE.CSharp_IsInitialized();
	}
}
