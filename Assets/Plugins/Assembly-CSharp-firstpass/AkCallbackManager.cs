using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AkCallbackManager
{
	public class EventCallbackPackage
	{
		public object m_Cookie;

		public EventCallback m_Callback;

		public EventCallbackPackage(EventCallback in_cb, object in_cookie)
		{
			m_Callback = in_cb;
			m_Cookie = in_cookie;
			m_mapEventCallbacks[GetHashCode()] = this;
		}
	}

	public class BankCallbackPackage
	{
		public object m_Cookie;

		public BankCallback m_Callback;

		public BankCallbackPackage(BankCallback in_cb, object in_cookie)
		{
			m_Callback = in_cb;
			m_Cookie = in_cookie;
			m_mapBankCallbacks[GetHashCode()] = this;
		}
	}

	private struct AkCommonCallback
	{
		public AkCallbackType eType;

		public IntPtr pPackage;

		public IntPtr pNext;
	}

	public struct AkEventCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;
	}

	public struct AkDynamicSequenceItemCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint audioNodeID;

		public IntPtr pCustomInfo;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AkMarkerCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public uint uIdentifier;

		public uint uPosition;

		public string strLabel;
	}

	public struct AkDurationCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public float fDuration;

		public float fEstimatedDuration;

		public uint audioNodeID;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class AkMusicSyncCallbackInfoBase
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public AkCallbackType musicSyncType;

		public float fBeatDuration;

		public float fBarDuration;

		public float fGridDuration;

		public float fGridOffset;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class AkMusicSyncCallbackInfo : AkMusicSyncCallbackInfoBase
	{
		public string pszUserCueName;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct AkMonitoringMsg
	{
		public ErrorCode errorCode;

		public ErrorLevel errorLevel;

		public uint playingID;

		public IntPtr gameObjID;

		public string msg;
	}

	public struct AkBankInfo
	{
		public uint bankID;

		public AKRESULT eLoadResult;

		public uint memPoolId;
	}

	public delegate void EventCallback(object in_cookie, AkCallbackType in_type, object in_info);

	public delegate void MonitoringCallback(ErrorCode in_errorCode, ErrorLevel in_errorLevel, uint in_playingID, IntPtr in_gameObjID, string in_msg);

	public delegate void BankCallback(uint in_bankID, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie);

	private static Dictionary<int, EventCallbackPackage> m_mapEventCallbacks = new Dictionary<int, EventCallbackPackage>();

	private static Dictionary<int, BankCallbackPackage> m_mapBankCallbacks = new Dictionary<int, BankCallbackPackage>();

	private static IntPtr m_pNotifMem;

	private static MonitoringCallback m_MonitoringCB;

	public static AKRESULT Init()
	{
		m_pNotifMem = Marshal.AllocHGlobal(1024);
		return AkCallbackSerializer.Init(m_pNotifMem, 1024u);
	}

	public static void Term()
	{
		AkCallbackSerializer.Term();
		Marshal.FreeHGlobal(m_pNotifMem);
		m_pNotifMem = IntPtr.Zero;
	}

	public static void SetMonitoringCallback(ErrorLevel in_Level, MonitoringCallback in_CB)
	{
		AkCallbackSerializer.SetLocalOutput((uint)in_Level);
		m_MonitoringCB = in_CB;
	}

	public static void PostCallbacks()
	{
		if (!AkSoundEngine.IsInitialized() || m_pNotifMem == IntPtr.Zero)
		{
			return;
		}
		IntPtr pData = AkCallbackSerializer.Lock();
		if (pData == IntPtr.Zero)
		{
			AkCallbackSerializer.Unlock();
			return;
		}
		AkCommonCallback akCommonCallback = default(AkCommonCallback);
		akCommonCallback.eType = (AkCallbackType)0;
		akCommonCallback.pPackage = IntPtr.Zero;
		akCommonCallback.pNext = IntPtr.Zero;
		IntPtr intPtr = pData;
		try
		{
			akCommonCallback = new AkCommonCallback
			{
				eType = (AkCallbackType)Marshal.ReadInt32(pData)
			};
			GotoEndOfCurrentStructMemberOfEnumType<AkCallbackType>(ref pData);
			akCommonCallback.pPackage = Marshal.ReadIntPtr(pData);
			GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
			akCommonCallback.pNext = Marshal.ReadIntPtr(pData);
			pData = intPtr;
		}
		catch (Exception arg)
		{
			pData = intPtr;
			string message = string.Format("PostCallbacks aborted due to exception: {0}.", arg);
			Debug.LogError(message);
			AkCallbackSerializer.Unlock();
			return;
		}
		while (true)
		{
			pData = (IntPtr)(pData.ToInt64() + Marshal.SizeOf(typeof(AkCommonCallback)));
			if (akCommonCallback.eType == AkCallbackType.AK_Monitoring)
			{
				AkMonitoringMsg akMonitoringMsg = new AkMonitoringMsg
				{
					errorCode = (ErrorCode)Marshal.ReadInt32(pData)
				};
				GotoEndOfCurrentStructMemberOfEnumType<ErrorCode>(ref pData);
				akMonitoringMsg.errorLevel = (ErrorLevel)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMemberOfEnumType<ErrorLevel>(ref pData);
				akMonitoringMsg.playingID = (uint)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
				akMonitoringMsg.gameObjID = Marshal.ReadIntPtr(pData);
				GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
				akMonitoringMsg.msg = SafeMarshalString(pData);
				if (m_MonitoringCB != null)
				{
					m_MonitoringCB(akMonitoringMsg.errorCode, akMonitoringMsg.errorLevel, akMonitoringMsg.playingID, akMonitoringMsg.gameObjID, akMonitoringMsg.msg);
				}
			}
			else if (akCommonCallback.eType == AkCallbackType.AK_Bank)
			{
				AkBankInfo akBankInfo = new AkBankInfo
				{
					bankID = (uint)Marshal.ReadInt32(pData)
				};
				GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
				akBankInfo.eLoadResult = (AKRESULT)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMemberOfEnumType<AKRESULT>(ref pData);
				akBankInfo.memPoolId = (uint)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
				BankCallbackPackage value = null;
				if (m_mapBankCallbacks.TryGetValue((int)akCommonCallback.pPackage, out value))
				{
					value.m_Callback(akBankInfo.bankID, akBankInfo.eLoadResult, akBankInfo.memPoolId, value.m_Cookie);
				}
			}
			else
			{
				EventCallbackPackage value2 = null;
				if (m_mapEventCallbacks.TryGetValue((int)akCommonCallback.pPackage, out value2))
				{
					switch (akCommonCallback.eType)
					{
					case AkCallbackType.AK_EndOfEvent:
					{
						AkEventCallbackInfo akEventCallbackInfo = new AkEventCallbackInfo
						{
							pCookie = Marshal.ReadIntPtr(pData)
						};
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akEventCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akEventCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akEventCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						value2.m_Callback(value2.m_Cookie, akCommonCallback.eType, akEventCallbackInfo);
						break;
					}
					case AkCallbackType.AK_EndOfDynamicSequenceItem:
					{
						AkDynamicSequenceItemCallbackInfo akDynamicSequenceItemCallbackInfo = new AkDynamicSequenceItemCallbackInfo
						{
							pCookie = Marshal.ReadIntPtr(pData)
						};
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akDynamicSequenceItemCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akDynamicSequenceItemCallbackInfo.audioNodeID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akDynamicSequenceItemCallbackInfo.pCustomInfo = Marshal.ReadIntPtr(pData);
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						value2.m_Callback(value2.m_Cookie, akCommonCallback.eType, akDynamicSequenceItemCallbackInfo);
						break;
					}
					case AkCallbackType.AK_Marker:
					{
						AkMarkerCallbackInfo akMarkerCallbackInfo = new AkMarkerCallbackInfo
						{
							pCookie = Marshal.ReadIntPtr(pData)
						};
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akMarkerCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akMarkerCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akMarkerCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akMarkerCallbackInfo.uIdentifier = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akMarkerCallbackInfo.uPosition = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akMarkerCallbackInfo.strLabel = SafeMarshalString(pData);
						value2.m_Callback(value2.m_Cookie, akCommonCallback.eType, akMarkerCallbackInfo);
						break;
					}
					case AkCallbackType.AK_Duration:
					{
						AkDurationCallbackInfo akDurationCallbackInfo = new AkDurationCallbackInfo
						{
							pCookie = Marshal.ReadIntPtr(pData)
						};
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akDurationCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akDurationCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akDurationCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akDurationCallbackInfo.fDuration = Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<float>(ref pData);
						akDurationCallbackInfo.fEstimatedDuration = Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<float>(ref pData);
						akDurationCallbackInfo.audioNodeID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						value2.m_Callback(value2.m_Cookie, akCommonCallback.eType, akDurationCallbackInfo);
						break;
					}
					case AkCallbackType.AK_MusicPlayStarted:
					case AkCallbackType.AK_MusicSyncBeat:
					case AkCallbackType.AK_MusicSyncBar:
					case AkCallbackType.AK_MusicSyncEntry:
					case AkCallbackType.AK_MusicSyncExit:
					case AkCallbackType.AK_MusicSyncGrid:
					case AkCallbackType.AK_MusicSyncUserCue:
					case AkCallbackType.AK_MusicSyncPoint:
					{
						AkMusicSyncCallbackInfo akMusicSyncCallbackInfo = new AkMusicSyncCallbackInfo();
						akMusicSyncCallbackInfo.pCookie = Marshal.ReadIntPtr(pData);
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akMusicSyncCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
						GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
						akMusicSyncCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<uint>(ref pData);
						akMusicSyncCallbackInfo.musicSyncType = (AkCallbackType)Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfEnumType<AkCallbackType>(ref pData);
						akMusicSyncCallbackInfo.fBeatDuration = Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<float>(ref pData);
						akMusicSyncCallbackInfo.fBarDuration = Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<float>(ref pData);
						akMusicSyncCallbackInfo.fGridDuration = Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<float>(ref pData);
						akMusicSyncCallbackInfo.fGridOffset = Marshal.ReadInt32(pData);
						GotoEndOfCurrentStructMemberOfValueType<float>(ref pData);
						akMusicSyncCallbackInfo.pszUserCueName = SafeMarshalString(pData);
						value2.m_Callback(value2.m_Cookie, akCommonCallback.eType, akMusicSyncCallbackInfo);
						break;
					}
					}
				}
			}
			if (akCommonCallback.pNext == IntPtr.Zero)
			{
				break;
			}
			pData = akCommonCallback.pNext;
			intPtr = pData;
			try
			{
				akCommonCallback = new AkCommonCallback
				{
					eType = (AkCallbackType)Marshal.ReadInt32(pData)
				};
				GotoEndOfCurrentStructMemberOfEnumType<AkCallbackType>(ref pData);
				akCommonCallback.pPackage = Marshal.ReadIntPtr(pData);
				GotoEndOfCurrentStructMemberOfIntPtr(ref pData);
				akCommonCallback.pNext = Marshal.ReadIntPtr(pData);
				pData = intPtr;
			}
			catch (Exception arg2)
			{
				pData = intPtr;
				string message2 = string.Format("PostCallbacks aborted due to exception: {0}.", arg2);
				Debug.LogError(message2);
				AkCallbackSerializer.Unlock();
				return;
			}
		}
		AkCallbackSerializer.Unlock();
	}

	private static string SafeMarshalString(IntPtr pData)
	{
		return Marshal.PtrToStringAnsi(pData);
	}

	private static void GotoEndOfCurrentStructMemberOfValueType<T>(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + Marshal.SizeOf(typeof(T)));
	}

	private static void GotoEndOfCurrentStructMemberOfIntPtr(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + IntPtr.Size);
	}

	private static void GotoEndOfCurrentStructMemberOfEnumType<T>(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T))));
	}
}
