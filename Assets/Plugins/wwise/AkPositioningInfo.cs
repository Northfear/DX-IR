using System;
using System.Runtime.InteropServices;

public class AkPositioningInfo : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public float fCenterPct
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fCenterPct_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fCenterPct_set(swigCPtr, value);
		}
	}

	public AkPositioningType positioningType
	{
		get
		{
			return (AkPositioningType)AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_positioningType_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_positioningType_set(swigCPtr, (int)value);
		}
	}

	public bool bUpdateEachFrame
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUpdateEachFrame_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUpdateEachFrame_set(swigCPtr, value);
		}
	}

	public bool bUseSpatialization
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseSpatialization_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseSpatialization_set(swigCPtr, value);
		}
	}

	public bool bUseAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseAttenuation_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseAttenuation_set(swigCPtr, value);
		}
	}

	public bool bUseConeAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseConeAttenuation_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseConeAttenuation_set(swigCPtr, value);
		}
	}

	public float fInnerAngle
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fInnerAngle_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fInnerAngle_set(swigCPtr, value);
		}
	}

	public float fOuterAngle
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fOuterAngle_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fOuterAngle_set(swigCPtr, value);
		}
	}

	public float fConeMaxAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fConeMaxAttenuation_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fConeMaxAttenuation_set(swigCPtr, value);
		}
	}

	public float LPFCone
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFCone_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFCone_set(swigCPtr, value);
		}
	}

	public float fMaxDistance
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fMaxDistance_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fMaxDistance_set(swigCPtr, value);
		}
	}

	public float fVolDryAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolDryAtMaxDist_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolDryAtMaxDist_set(swigCPtr, value);
		}
	}

	public float fVolWetAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolWetAtMaxDist_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolWetAtMaxDist_set(swigCPtr, value);
		}
	}

	public float LPFValueAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFValueAtMaxDist_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFValueAtMaxDist_set(swigCPtr, value);
		}
	}

	internal AkPositioningInfo(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkPositioningInfo()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkPositioningInfo(), true)
	{
	}

	internal static HandleRef getCPtr(AkPositioningInfo obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkPositioningInfo()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkPositioningInfo(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
