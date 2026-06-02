using System;
using System.Runtime.InteropServices;

public class AkExternalSourceInfo : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public uint iExternalSrcCookie
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_iExternalSrcCookie_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_iExternalSrcCookie_set(swigCPtr, value);
		}
	}

	public uint idCodec
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idCodec_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idCodec_set(swigCPtr, value);
		}
	}

	public string szFile
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_szFile_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_szFile_set(swigCPtr, value);
		}
	}

	public IntPtr pInMemory
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_pInMemory_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_pInMemory_set(swigCPtr, value);
		}
	}

	public uint uiMemorySize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_uiMemorySize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_uiMemorySize_set(swigCPtr, value);
		}
	}

	public uint idFile
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idFile_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idFile_set(swigCPtr, value);
		}
	}

	internal AkExternalSourceInfo(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkExternalSourceInfo()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_0(), true)
	{
	}

	public AkExternalSourceInfo(IntPtr in_pInMemory, uint in_uiMemorySize, uint in_iExternalSrcCookie, uint in_idCodec)
		: this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_1(in_pInMemory, in_uiMemorySize, in_iExternalSrcCookie, in_idCodec), true)
	{
	}

	public AkExternalSourceInfo(string in_pszFileName, uint in_iExternalSrcCookie, uint in_idCodec)
		: this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_2(in_pszFileName, in_iExternalSrcCookie, in_idCodec), true)
	{
	}

	public AkExternalSourceInfo(uint in_idFile, uint in_iExternalSrcCookie, uint in_idCodec)
		: this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_3(in_idFile, in_iExternalSrcCookie, in_idCodec), true)
	{
	}

	internal static HandleRef getCPtr(AkExternalSourceInfo obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkExternalSourceInfo()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkExternalSourceInfo(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
