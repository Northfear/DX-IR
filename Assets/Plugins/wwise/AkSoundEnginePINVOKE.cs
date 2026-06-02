using System;
using System.Runtime.InteropServices;

internal class AkSoundEnginePINVOKE
{
	static AkSoundEnginePINVOKE()
	{
	}

	[DllImport("__Internal")]
	public static extern uint CSharp_AK_INVALID_AUX_ID_get();

	[DllImport("__Internal")]
	public static extern uint CSharp_AK_INVALID_CHANNELMASK_get();

	[DllImport("__Internal")]
	public static extern void CSharp_AkExternalSourceInfo_iExternalSrcCookie_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkExternalSourceInfo_iExternalSrcCookie_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkExternalSourceInfo_idCodec_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkExternalSourceInfo_idCodec_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkExternalSourceInfo_szFile_set(HandleRef jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

	[DllImport("__Internal")]
	public static extern string CSharp_AkExternalSourceInfo_szFile_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkExternalSourceInfo_pInMemory_set(HandleRef jarg1, IntPtr jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkExternalSourceInfo_pInMemory_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkExternalSourceInfo_uiMemorySize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkExternalSourceInfo_uiMemorySize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkExternalSourceInfo_idFile_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkExternalSourceInfo_idFile_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_0();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_1(IntPtr jarg1, uint jarg2, uint jarg3, uint jarg4);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_2([MarshalAs(UnmanagedType.LPStr)] string jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_3(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkExternalSourceInfo(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkVector_X_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkVector_X_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkVector_Y_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkVector_Y_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkVector_Z_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkVector_Z_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkVector();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkVector(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSoundPosition_Position_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkSoundPosition_Position_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSoundPosition_Orientation_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkSoundPosition_Orientation_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkSoundPosition();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkSoundPosition(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkListenerPosition_OrientationFront_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkListenerPosition_OrientationFront_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkListenerPosition_OrientationTop_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkListenerPosition_OrientationTop_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkListenerPosition_Position_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkListenerPosition_Position_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkListenerPosition();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkListenerPosition(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSpeakerVolumes_fFrontLeft_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkSpeakerVolumes_fFrontLeft_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSpeakerVolumes_fFrontRight_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkSpeakerVolumes_fFrontRight_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkSpeakerVolumes();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkSpeakerVolumes(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkAuxSendValue_auxBusID_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkAuxSendValue_auxBusID_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkAuxSendValue_fControlValue_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkAuxSendValue_fControlValue_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkAuxSendValue(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern bool CSharp_WwiseObjectIDext_IsEqualTo(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_WwiseObjectIDext_GetType(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_WwiseObjectIDext_id_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_WwiseObjectIDext_id_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_WwiseObjectIDext_bIsBus_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_WwiseObjectIDext_bIsBus_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_WwiseObjectIDext();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_WwiseObjectIDext(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_0();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_1(uint jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_2(uint jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_3(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_delete_WwiseObjectID(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_Iterator_pItem_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_Iterator_pItem_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_Iterator_NextIter(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_Iterator_PrevIter(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_Iterator_GetItem(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern bool CSharp_Iterator_IsEqualTo(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_Iterator_IsDifferentFrom(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_Iterator();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_Iterator(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_ArrayPoolDefault_Get();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_ArrayPoolDefault();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_ArrayPoolDefault(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_ArrayPoolLEngineDefault_Get();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_ArrayPoolLEngineDefault();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_ArrayPoolLEngineDefault(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_PlaylistItem__SWIG_0();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_PlaylistItem__SWIG_1(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_delete_PlaylistItem(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_PlaylistItem_Assign(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_PlaylistItem_IsEqualTo(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_PlaylistItem_SetExternalSources(HandleRef jarg1, uint jarg2, HandleRef jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_PlaylistItem_audioNodeID_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_PlaylistItem_audioNodeID_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_PlaylistItem_msDelay_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_PlaylistItem_msDelay_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_PlaylistItem_pCustomInfo_set(HandleRef jarg1, IntPtr jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_PlaylistItem_pCustomInfo_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkPlaylistArray();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkPlaylistArray(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_Begin(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_End(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_FindEx(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_Erase__SWIG_0(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlaylistArray_Erase__SWIG_1(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_EraseSwap(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkPlaylistArray_Reserve(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkPlaylistArray_Reserved(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlaylistArray_Term(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkPlaylistArray_Length(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPlaylistArray_IsEmpty(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_Exists(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_AddLast__SWIG_0(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_AddLast__SWIG_1(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_Last(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlaylistArray_RemoveLast(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_AkPlaylistArray_Remove(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkPlaylistArray_RemoveSwap(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlaylistArray_RemoveAll(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_ItemAtIndex(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlaylistArray_Insert(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPlaylistArray_GrowArray__SWIG_0(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPlaylistArray_GrowArray__SWIG_1(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPlaylistArray_Resize(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_0(HandleRef jarg1, uint jarg2, int jarg3, IntPtr jarg4, uint jarg5, HandleRef jarg6);

	[DllImport("__Internal")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_1(HandleRef jarg1, uint jarg2, int jarg3, IntPtr jarg4, uint jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_2(HandleRef jarg1, uint jarg2, int jarg3, IntPtr jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_3(HandleRef jarg1, uint jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_4(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_Playlist();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_Playlist(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_0(uint jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_1(uint jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("__Internal")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_2(uint jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_3(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceClose(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequencePlay__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequencePlay__SWIG_1(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequencePlay__SWIG_2(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequencePause__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequencePause__SWIG_1(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequencePause__SWIG_2(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceResume__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceResume__SWIG_1(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceResume__SWIG_2(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceStop__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceStop__SWIG_1(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceStop__SWIG_2(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceBreak(uint jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_DynamicSequenceLockPlaylist(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_DynamicSequenceUnlockPlaylist(uint jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_pfnAssertHook_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkInitSettings_pfnAssertHook_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uMaxNumPaths_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uMaxNumPaths_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uMaxNumTransitions_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uMaxNumTransitions_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uDefaultPoolSize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uDefaultPoolSize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_fDefaultPoolRatioThreshold_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkInitSettings_fDefaultPoolRatioThreshold_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uCommandQueueSize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uCommandQueueSize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uPrepareEventMemoryPoolID_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkInitSettings_uPrepareEventMemoryPoolID_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_bEnableGameSyncPreparation_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkInitSettings_bEnableGameSyncPreparation_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uContinuousPlaybackLookAhead_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uContinuousPlaybackLookAhead_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uMonitorPoolSize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uMonitorPoolSize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkInitSettings_uMonitorQueuePoolSize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkInitSettings_uMonitorQueuePoolSize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkInitSettings();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkInitSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern uint CSharp_GetSpeakerConfiguration();

	[DllImport("__Internal")]
	public static extern int CSharp_GetPanningRule();

	[DllImport("__Internal")]
	public static extern int CSharp_SetPanningRule(int jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetVolumeThreshold(float jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetMaxNumVoicesLimit(ushort jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_RenderAudio();

	[DllImport("__Internal")]
	public static extern uint CSharp_GetIDFromString__SWIG_0([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_0(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, HandleRef jarg7, uint jarg8);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_1(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, HandleRef jarg7);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_2(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_3(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_4(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_5(uint jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_6([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, HandleRef jarg7, uint jarg8);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_7([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, HandleRef jarg7);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_8([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_9([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_10([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern uint CSharp_PostEvent__SWIG_11([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_0(uint jarg1, int jarg2, uint jarg3, int jarg4, int jarg5, uint jarg6);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_1(uint jarg1, int jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_2(uint jarg1, int jarg2, uint jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_3(uint jarg1, int jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_4(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_5([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2, uint jarg3, int jarg4, int jarg5, uint jarg6);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_6([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_7([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2, uint jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_8([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_9([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_0(uint jarg1, uint jarg2, int jarg3, bool jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_1(uint jarg1, uint jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_2([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, int jarg3, bool jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_3([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_6(uint jarg1, uint jarg2, float jarg3, bool jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_7(uint jarg1, uint jarg2, float jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_8([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, float jarg3, bool jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SeekOnEvent__SWIG_9([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, float jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_CancelEventCallbackCookie(IntPtr jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_CancelEventCallback(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_GetSourcePlayPosition__SWIG_0(uint jarg1, out int jarg2, bool jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetSourcePlayPosition__SWIG_1(uint jarg1, out int jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_StopAll__SWIG_0(uint jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_StopAll__SWIG_1();

	[DllImport("__Internal")]
	public static extern void CSharp_StopPlayingID__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_StopPlayingID__SWIG_1(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_StopPlayingID__SWIG_2(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_RegisterGameObj__SWIG_0(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_RegisterGameObj__SWIG_1(uint jarg1, string jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_UnregisterGameObj(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_UnregisterAllGameObj();

	[DllImport("__Internal")]
	public static extern int CSharp_SetMultiplePositions__SWIG_0(uint jarg1, IntPtr jarg2, ushort jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SetMultiplePositions__SWIG_1(uint jarg1, IntPtr jarg2, ushort jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetAttenuationScalingFactor(uint jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetListenerScalingFactor(uint jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_ClearBanks();

	[DllImport("__Internal")]
	public static extern int CSharp_SetBankLoadIOSettings(float jarg1, sbyte jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_LoadBank__SWIG_0([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2, out uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_LoadBank__SWIG_1(uint jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_LoadBank__SWIG_2(IntPtr jarg1, uint jarg2, out uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_LoadBank__SWIG_3([MarshalAs(UnmanagedType.LPWStr)] string jarg1, IntPtr jarg2, IntPtr jarg3, int jarg4, out uint jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_LoadBank__SWIG_4(uint jarg1, IntPtr jarg2, IntPtr jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_LoadBank__SWIG_5(IntPtr jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, out uint jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_UnloadBank__SWIG_0([MarshalAs(UnmanagedType.LPWStr)] string jarg1, out int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_UnloadBank__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_UnloadBank__SWIG_4(uint jarg1, out int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_UnloadBank__SWIG_5(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_UnloadBank__SWIG_6([MarshalAs(UnmanagedType.LPWStr)] string jarg1, IntPtr jarg2, IntPtr jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_UnloadBank__SWIG_8(uint jarg1, IntPtr jarg2, IntPtr jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_CancelBankCallbackCookie(IntPtr jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_0(int jarg1, [MarshalAs(UnmanagedType.LPWStr)] string jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_1(int jarg1, [MarshalAs(UnmanagedType.LPWStr)] string jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_4(int jarg1, uint jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_5(int jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_6(int jarg1, [MarshalAs(UnmanagedType.LPWStr)] string jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_7(int jarg1, [MarshalAs(UnmanagedType.LPWStr)] string jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_10(int jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareBank__SWIG_11(int jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_ClearPreparedEvents();

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareEvent__SWIG_0(int jarg1, IntPtr jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareEvent__SWIG_1(int jarg1, [In][MarshalAs(UnmanagedType.LPArray)] uint[] jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareEvent__SWIG_2(int jarg1, IntPtr jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareEvent__SWIG_3(int jarg1, [In][MarshalAs(UnmanagedType.LPArray)] uint[] jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_0(int jarg1, int jarg2, [MarshalAs(UnmanagedType.LPWStr)] string jarg3, IntPtr jarg4, uint jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_1(int jarg1, int jarg2, uint jarg3, [In][MarshalAs(UnmanagedType.LPArray)] uint[] jarg4, uint jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_2(int jarg1, int jarg2, [MarshalAs(UnmanagedType.LPWStr)] string jarg3, IntPtr jarg4, uint jarg5, IntPtr jarg6, IntPtr jarg7);

	[DllImport("__Internal")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_3(int jarg1, int jarg2, uint jarg3, [In][MarshalAs(UnmanagedType.LPArray)] uint[] jarg4, uint jarg5, IntPtr jarg6, IntPtr jarg7);

	[DllImport("__Internal")]
	public static extern int CSharp_SetActiveListeners(uint jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetListenerSpatialization__SWIG_0(uint jarg1, bool jarg2, HandleRef jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetListenerSpatialization__SWIG_1(uint jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetListenerPipeline(uint jarg1, bool jarg2, bool jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_0(uint jarg1, float jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_1(uint jarg1, float jarg2, uint jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_2(uint jarg1, float jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_3(uint jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_4([MarshalAs(UnmanagedType.LPWStr)] string jarg1, float jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_5([MarshalAs(UnmanagedType.LPWStr)] string jarg1, float jarg2, uint jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_6([MarshalAs(UnmanagedType.LPWStr)] string jarg1, float jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetRTPCValue__SWIG_7([MarshalAs(UnmanagedType.LPWStr)] string jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_0(uint jarg1, uint jarg2, int jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_1(uint jarg1, uint jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_2(uint jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_3(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_4([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, int jarg3, int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_5([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_6([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_ResetRTPCValue__SWIG_7([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetSwitch__SWIG_0(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetSwitch__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, [MarshalAs(UnmanagedType.LPWStr)] string jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_PostTrigger__SWIG_0(uint jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_PostTrigger__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetState__SWIG_0(uint jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetState__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, [MarshalAs(UnmanagedType.LPWStr)] string jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetGameObjectAuxSendValues(uint jarg1, IntPtr jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetGameObjectOutputBusVolume(uint jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetAuxBusVolumes(uint jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_SetActorMixerEffect(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetBusEffect__SWIG_0(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetBusEffect__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_SetObjectObstructionAndOcclusion(uint jarg1, uint jarg2, float jarg3, float jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_StartOutputCapture([MarshalAs(UnmanagedType.LPStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_StopOutputCapture();

	[DllImport("__Internal")]
	public static extern int CSharp_StartProfilerCapture([MarshalAs(UnmanagedType.LPStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_StopProfilerCapture();

	[DllImport("__Internal")]
	public static extern void CSharp_AkMemSettings_uMaxNumPools_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkMemSettings_uMaxNumPools_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkMemSettings();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkMemSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkMusicSettings_fStreamingLookAheadRatio_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkMusicSettings_fStreamingLookAheadRatio_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkMusicSettings();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkMusicSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSegmentInfo_iCurrentPosition_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkSegmentInfo_iCurrentPosition_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSegmentInfo_iPreEntryDuration_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkSegmentInfo_iPreEntryDuration_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSegmentInfo_iActiveDuration_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkSegmentInfo_iActiveDuration_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSegmentInfo_iPostExitDuration_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkSegmentInfo_iPostExitDuration_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkSegmentInfo_iRemainingLookAheadTime_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkSegmentInfo_iRemainingLookAheadTime_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkSegmentInfo();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkSegmentInfo(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_GetPlayingSegmentInfo__SWIG_0(uint jarg1, HandleRef jarg2, bool jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetPlayingSegmentInfo__SWIG_1(uint jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkCallbackSerializer_Init(IntPtr jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_AkCallbackSerializer_Term();

	[DllImport("__Internal")]
	public static extern void CSharp_AkCallbackSerializer_SetLocalOutput(uint jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkCallbackSerializer_Lock();

	[DllImport("__Internal")]
	public static extern void CSharp_AkCallbackSerializer_Unlock();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkCallbackSerializer();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkCallbackSerializer(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_PostCode(int jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_PostString__SWIG_0([MarshalAs(UnmanagedType.LPWStr)] string jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetTimeStamp();

	[DllImport("__Internal")]
	public static extern uint CSharp_ResolveDialogueEvent__SWIG_0(uint jarg1, [In][MarshalAs(UnmanagedType.LPArray)] uint[] jarg2, uint jarg3, uint jarg4);

	[DllImport("__Internal")]
	public static extern uint CSharp_ResolveDialogueEvent__SWIG_1(uint jarg1, [In][MarshalAs(UnmanagedType.LPArray)] uint[] jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fCenterPct_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fCenterPct_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_positioningType_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkPositioningInfo_positioningType_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_bUpdateEachFrame_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPositioningInfo_bUpdateEachFrame_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_bUseSpatialization_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPositioningInfo_bUseSpatialization_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_bUseAttenuation_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPositioningInfo_bUseAttenuation_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_bUseConeAttenuation_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPositioningInfo_bUseConeAttenuation_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fInnerAngle_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fInnerAngle_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fOuterAngle_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fOuterAngle_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fConeMaxAttenuation_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fConeMaxAttenuation_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_LPFCone_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_LPFCone_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fMaxDistance_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fMaxDistance_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fVolDryAtMaxDist_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fVolDryAtMaxDist_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_fVolWetAtMaxDist_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_fVolWetAtMaxDist_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPositioningInfo_LPFValueAtMaxDist_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPositioningInfo_LPFValueAtMaxDist_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkPositioningInfo();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkPositioningInfo(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkObjectInfo_objID_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkObjectInfo_objID_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkObjectInfo_parentID_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkObjectInfo_parentID_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkObjectInfo_iDepth_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkObjectInfo_iDepth_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkObjectInfo();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkObjectInfo(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_GetPosition(uint jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetActiveListeners(uint jarg1, out uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetListenerPosition(uint jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetListenerSpatialization(uint jarg1, out int jarg2, HandleRef jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetRTPCValue__SWIG_0(uint jarg1, uint jarg2, out float jarg3, ref int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_GetRTPCValue__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, out float jarg3, ref int jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_GetSwitch__SWIG_0(uint jarg1, uint jarg2, out uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetSwitch__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, uint jarg2, out uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetState__SWIG_0(uint jarg1, out uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetState__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, out uint jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetGameObjectAuxSendValues(uint jarg1, IntPtr jarg2, ref uint jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetGameObjectDryLevelValue(uint jarg1, out float jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetAuxBusVolumes(uint jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_GetObjectObstructionAndOcclusion(uint jarg1, uint jarg2, out float jarg3, out float jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_QueryAudioObjectIDs__SWIG_0(uint jarg1, ref uint jarg2, HandleRef jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_QueryAudioObjectIDs__SWIG_1([MarshalAs(UnmanagedType.LPWStr)] string jarg1, ref uint jarg2, HandleRef jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetPositioningInfo(uint jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_GetIsGameObjectActive(uint jarg1);

	[DllImport("__Internal")]
	public static extern float CSharp_GetMaxRadius(uint jarg1);

	[DllImport("__Internal")]
	public static extern uint CSharp_GetEventIDFromPlayingID(uint jarg1);

	[DllImport("__Internal")]
	public static extern uint CSharp_GetGameObjectFromPlayingID(uint jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_GetPlayingIDsFromGameObject(uint jarg1, ref uint jarg2, [Out][MarshalAs(UnmanagedType.LPArray)] uint[] jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetCustomPropertyValue__SWIG_0(uint jarg1, uint jarg2, out int jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_GetCustomPropertyValue__SWIG_1(uint jarg1, uint jarg2, out float jarg3);

	[DllImport("__Internal")]
	public static extern int CSharp_AddPlayerMotionDevice__SWIG_0(byte jarg1, uint jarg2, uint jarg3, IntPtr jarg4);

	[DllImport("__Internal")]
	public static extern int CSharp_AddPlayerMotionDevice__SWIG_1(byte jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_RemovePlayerMotionDevice(byte jarg1, uint jarg2, uint jarg3);

	[DllImport("__Internal")]
	public static extern void CSharp_SetPlayerListener(byte jarg1, byte jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_SetPlayerVolume(byte jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern void CSharp_AkStreamMgrSettings_uMemorySize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkStreamMgrSettings_uMemorySize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkStreamMgrSettings();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkStreamMgrSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_pIOMemory_set(HandleRef jarg1, IntPtr jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkDeviceSettings_pIOMemory_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_uIOMemorySize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkDeviceSettings_uIOMemorySize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_uIOMemoryAlignment_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkDeviceSettings_uIOMemoryAlignment_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_ePoolAttributes_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkDeviceSettings_ePoolAttributes_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_uGranularity_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkDeviceSettings_uGranularity_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_uSchedulerTypeFlags_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkDeviceSettings_uSchedulerTypeFlags_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_threadProperties_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkDeviceSettings_threadProperties_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_uMaxConcurrentIO_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkDeviceSettings_uMaxConcurrentIO_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkDeviceSettings_fMaxCacheRatio_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkDeviceSettings_fMaxCacheRatio_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkDeviceSettings();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkDeviceSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkThreadProperties_nPriority_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkThreadProperties_nPriority_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkThreadProperties_uStackSize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkThreadProperties_uStackSize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkThreadProperties_uSchedPolicy_set(HandleRef jarg1, int jarg2);

	[DllImport("__Internal")]
	public static extern int CSharp_AkThreadProperties_uSchedPolicy_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkThreadProperties();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkThreadProperties(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_threadLEngine_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlatformInitSettings_threadLEngine_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_threadBankManager_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlatformInitSettings_threadBankManager_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_threadMonitor_set(HandleRef jarg1, HandleRef jarg2);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_AkPlatformInitSettings_threadMonitor_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_set(HandleRef jarg1, float jarg2);

	[DllImport("__Internal")]
	public static extern float CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_uSampleRate_set(HandleRef jarg1, uint jarg2);

	[DllImport("__Internal")]
	public static extern uint CSharp_AkPlatformInitSettings_uSampleRate_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_uNumRefillsInVoice_set(HandleRef jarg1, ushort jarg2);

	[DllImport("__Internal")]
	public static extern ushort CSharp_AkPlatformInitSettings_uNumRefillsInVoice_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_AkPlatformInitSettings_bMuteOtherApps_set(HandleRef jarg1, bool jarg2);

	[DllImport("__Internal")]
	public static extern bool CSharp_AkPlatformInitSettings_bMuteOtherApps_get(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_new_AkPlatformInitSettings();

	[DllImport("__Internal")]
	public static extern void CSharp_delete_AkPlatformInitSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_Term();

	[DllImport("__Internal")]
	public static extern int CSharp_Init(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3, HandleRef jarg4, HandleRef jarg5, HandleRef jarg6);

	[DllImport("__Internal")]
	public static extern void CSharp_GetDefaultStreamSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_GetDefaultDeviceSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_GetDefaultMusicSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_GetDefaultInitSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern void CSharp_GetDefaultPlatformInitSettings(HandleRef jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetBasePath([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetBankPath([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetAudioSrcPath([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetCurrentLanguage([MarshalAs(UnmanagedType.LPWStr)] string jarg1);

	[DllImport("__Internal")]
	public static extern int CSharp_SetObjectPosition(uint jarg1, float jarg2, float jarg3, float jarg4, float jarg5, float jarg6, float jarg7);

	[DllImport("__Internal")]
	public static extern int CSharp_SetObjectPositionWithListener(uint jarg1, float jarg2, float jarg3, float jarg4, float jarg5, float jarg6, float jarg7, uint jarg8);

	[DllImport("__Internal")]
	public static extern int CSharp_SetListenerPosition(float jarg1, float jarg2, float jarg3, float jarg4, float jarg5, float jarg6, float jarg7, float jarg8, float jarg9, uint jarg10);

	[DllImport("__Internal")]
	public static extern bool CSharp_IsInitialized();

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_WwiseObjectID_SWIGUpcast(IntPtr jarg1);

	[DllImport("__Internal")]
	public static extern IntPtr CSharp_Playlist_SWIGUpcast(IntPtr jarg1);
}
