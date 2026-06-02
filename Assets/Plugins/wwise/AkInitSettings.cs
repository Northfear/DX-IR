using System;
using System.Runtime.InteropServices;

public class AkInitSettings : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public int pfnAssertHook
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public uint uMaxNumPaths
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumPaths_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumPaths_set(swigCPtr, value);
		}
	}

	public uint uMaxNumTransitions
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumTransitions_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumTransitions_set(swigCPtr, value);
		}
	}

	public uint uDefaultPoolSize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uDefaultPoolSize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uDefaultPoolSize_set(swigCPtr, value);
		}
	}

	public float fDefaultPoolRatioThreshold
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_fDefaultPoolRatioThreshold_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_fDefaultPoolRatioThreshold_set(swigCPtr, value);
		}
	}

	public uint uCommandQueueSize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCommandQueueSize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCommandQueueSize_set(swigCPtr, value);
		}
	}

	public int uPrepareEventMemoryPoolID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uPrepareEventMemoryPoolID_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uPrepareEventMemoryPoolID_set(swigCPtr, value);
		}
	}

	public bool bEnableGameSyncPreparation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_bEnableGameSyncPreparation_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_bEnableGameSyncPreparation_set(swigCPtr, value);
		}
	}

	public uint uContinuousPlaybackLookAhead
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uContinuousPlaybackLookAhead_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uContinuousPlaybackLookAhead_set(swigCPtr, value);
		}
	}

	public uint uMonitorPoolSize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorPoolSize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorPoolSize_set(swigCPtr, value);
		}
	}

	public uint uMonitorQueuePoolSize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorQueuePoolSize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorQueuePoolSize_set(swigCPtr, value);
		}
	}

	internal AkInitSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkInitSettings()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkInitSettings(), true)
	{
	}

	internal static HandleRef getCPtr(AkInitSettings obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkInitSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkInitSettings(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
