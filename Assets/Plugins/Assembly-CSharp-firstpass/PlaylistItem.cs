using System;
using System.Runtime.InteropServices;

public class PlaylistItem : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public uint audioNodeID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_PlaylistItem_audioNodeID_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_PlaylistItem_audioNodeID_set(swigCPtr, value);
		}
	}

	public int msDelay
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_PlaylistItem_msDelay_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_PlaylistItem_msDelay_set(swigCPtr, value);
		}
	}

	public IntPtr pCustomInfo
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_PlaylistItem_pCustomInfo_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_PlaylistItem_pCustomInfo_set(swigCPtr, value);
		}
	}

	internal PlaylistItem(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public PlaylistItem()
		: this(AkSoundEnginePINVOKE.CSharp_new_PlaylistItem__SWIG_0(), true)
	{
	}

	public PlaylistItem(PlaylistItem in_rCopy)
		: this(AkSoundEnginePINVOKE.CSharp_new_PlaylistItem__SWIG_1(getCPtr(in_rCopy)), true)
	{
	}

	internal static HandleRef getCPtr(PlaylistItem obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~PlaylistItem()
	{
		Dispose();
	}

	public virtual void Dispose()
	{
		lock (this)
		{
			if (swigCPtr.Handle != IntPtr.Zero)
			{
				if (swigCMemOwn)
				{
					swigCMemOwn = false;
					AkSoundEnginePINVOKE.CSharp_delete_PlaylistItem(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}

	public PlaylistItem Assign(PlaylistItem in_rCopy)
	{
		return new PlaylistItem(AkSoundEnginePINVOKE.CSharp_PlaylistItem_Assign(swigCPtr, getCPtr(in_rCopy)), false);
	}

	public bool IsEqualTo(PlaylistItem in_rCopy)
	{
		return AkSoundEnginePINVOKE.CSharp_PlaylistItem_IsEqualTo(swigCPtr, getCPtr(in_rCopy));
	}

	public AKRESULT SetExternalSources(uint in_nExternalSrc, AkExternalSourceInfo in_pExternalSrc)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PlaylistItem_SetExternalSources(swigCPtr, in_nExternalSrc, AkExternalSourceInfo.getCPtr(in_pExternalSrc));
	}
}
