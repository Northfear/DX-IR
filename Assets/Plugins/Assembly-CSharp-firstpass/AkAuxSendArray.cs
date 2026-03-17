using System;
using System.Runtime.InteropServices;

public class AkAuxSendArray
{
	public IntPtr m_Buffer;

	private IntPtr m_Current;

	private uint m_MaxCount;

	public uint m_Count;

	public AkAuxSendArray(uint in_Count)
	{
		m_Buffer = Marshal.AllocHGlobal((int)(in_Count * 8));
		m_Current = m_Buffer;
		m_MaxCount = in_Count;
		m_Count = 0u;
	}

	~AkAuxSendArray()
	{
		Marshal.FreeHGlobal(m_Buffer);
		m_Buffer = IntPtr.Zero;
	}

	public void Reset()
	{
		m_Current = m_Buffer;
		m_Count = 0u;
	}

	public void Add(uint in_EnvID, float in_fValue)
	{
		if (m_Count >= m_MaxCount)
		{
			throw new IndexOutOfRangeException("Out of range access in AkAuxSendArray");
		}
		Marshal.WriteInt32(m_Current, (int)in_EnvID);
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		m_Count++;
	}
}
