public enum AkCallbackType
{
	AK_EndOfEvent = 1,
	AK_EndOfDynamicSequenceItem = 2,
	AK_Marker = 4,
	AK_Duration = 8,
	AK_SpeakerVolumeMatrix = 16,
	AK_MusicPlayStarted = 128,
	AK_MusicSyncBeat = 256,
	AK_MusicSyncBar = 512,
	AK_MusicSyncEntry = 1024,
	AK_MusicSyncExit = 2048,
	AK_MusicSyncGrid = 4096,
	AK_MusicSyncUserCue = 8192,
	AK_MusicSyncPoint = 16384,
	AK_MusicSyncAll = 65280,
	AK_CallbackBits = 65535,
	AK_EnableGetSourcePlayPosition = 65536,
	AK_EnableGetMusicPlayPosition = 131072,
	AK_Monitoring = 536870912,
	AK_Bank = 1073741824
}
