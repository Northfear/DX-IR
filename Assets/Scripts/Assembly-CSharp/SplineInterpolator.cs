using System;
using System.Collections;
using UnityEngine;

public class SplineInterpolator
{
	internal class SplineNode
	{
		internal Vector3 Point;

		internal Quaternion Rot;

		internal float Time;

		internal Vector2 EaseIO;

		internal SplineNode(Vector3 p, Quaternion q, float t, Vector2 io)
		{
			Point = p;
			Rot = q;
			Time = t;
			EaseIO = io;
		}

		internal SplineNode(SplineNode o)
		{
			Point = o.Point;
			Rot = o.Rot;
			Time = o.Time;
			EaseIO = o.EaseIO;
		}
	}

	public class SplineNodeList : IList, IEnumerable, ICollection
	{
		private SplineNode[] _contents = new SplineNode[32];

		private int _count;

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public object this[int index]
		{
			get
			{
				return _contents[index];
			}
			set
			{
				_contents[index] = (SplineNode)value;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public SplineNodeList()
		{
			_count = 0;
		}

		public int Add(object value)
		{
			if (_count < _contents.Length)
			{
				_contents[_count] = (SplineNode)value;
				_count++;
				return _count - 1;
			}
			return -1;
		}

		public void Clear()
		{
			_count = 0;
		}

		public bool Contains(object value)
		{
			bool result = false;
			for (int i = 0; i < Count; i++)
			{
				if (_contents[i] == value)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool Contains(Vector3 value)
		{
			bool result = false;
			for (int i = 0; i < Count; i++)
			{
				if (_contents[i].Point == value)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool Contains(Quaternion value)
		{
			bool result = false;
			for (int i = 0; i < Count; i++)
			{
				if (_contents[i].Rot == value)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool Contains(float value)
		{
			bool result = false;
			for (int i = 0; i < Count; i++)
			{
				if (_contents[i].Time == value)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public int IndexOf(object value)
		{
			int result = -1;
			for (int i = 0; i < Count; i++)
			{
				if (_contents[i] == (SplineNode)value)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public void Insert(int index, object value)
		{
			if (_count + 1 <= _contents.Length && index < Count && index >= 0)
			{
				_count++;
				for (int num = Count - 1; num > index; num--)
				{
					_contents[num] = _contents[num - 1];
				}
				_contents[index] = (SplineNode)value;
			}
		}

		public void Remove(object value)
		{
			RemoveAt(IndexOf(value));
		}

		public void RemoveAt(int index)
		{
			if (index >= 0 && index < Count)
			{
				for (int i = index; i < Count - 1; i++)
				{
					_contents[i] = _contents[i + 1];
				}
				_count--;
			}
		}

		public void CopyTo(Array array, int index)
		{
			int num = index;
			for (int i = 0; i < Count; i++)
			{
				array.SetValue(_contents[i], num);
				num++;
			}
		}

		public IEnumerator GetEnumerator()
		{
			SplineNode[] contents = _contents;
			for (int i = 0; i < contents.Length; i++)
			{
				yield return contents[i];
			}
		}

		public void PrintContents()
		{
			Debug.Log("SplineNodeList has a capacity of " + _contents.Length + " and currently has " + _count + " elements.");
			Debug.Log("SplineNodeList contents:");
			for (int i = 0; i < Count; i++)
			{
				Debug.Log(_contents[i]);
			}
		}
	}

	private const int mMaxNodes = 32;

	private eEndPointsMode mEndPointsMode;

	private SplineNodeList mNodes = new SplineNodeList();

	private string mState = string.Empty;

	private bool mRotations;

	public void StartInterpolation(bool bRotations, eWrapMode mode)
	{
		if (mState != "Reset")
		{
			throw new Exception("First reset, add points and then call here");
		}
		mState = ((mode != 0) ? "Loop" : "Once");
		mRotations = bRotations;
		SetInput();
	}

	public void Reset()
	{
		mNodes.Clear();
		mState = "Reset";
		mRotations = false;
		mEndPointsMode = eEndPointsMode.AUTO;
	}

	public void AddPoint(Vector3 pos, Quaternion quat, float timeInSeconds, Vector2 easeInOut)
	{
		if (mState != "Reset")
		{
			throw new Exception("Cannot add points after start");
		}
		mNodes.Add(new SplineNode(pos, quat, timeInSeconds, easeInOut));
	}

	private void SetInput()
	{
		if (mNodes.Count < 2)
		{
			throw new Exception("Invalid number of points");
		}
		if (mRotations)
		{
			for (int i = 1; i < mNodes.Count; i++)
			{
				SplineNode splineNode = (SplineNode)mNodes[i];
				SplineNode splineNode2 = (SplineNode)mNodes[i - 1];
				if (Quaternion.Dot(splineNode.Rot, splineNode2.Rot) < 0f)
				{
					splineNode.Rot.x = 0f - splineNode.Rot.x;
					splineNode.Rot.y = 0f - splineNode.Rot.y;
					splineNode.Rot.z = 0f - splineNode.Rot.z;
					splineNode.Rot.w = 0f - splineNode.Rot.w;
				}
			}
		}
		if (mEndPointsMode == eEndPointsMode.AUTO)
		{
			mNodes.Insert(0, mNodes[0]);
			mNodes.Add(mNodes[mNodes.Count - 1]);
		}
		else if (mEndPointsMode == eEndPointsMode.EXPLICIT && mNodes.Count < 4)
		{
			throw new Exception("Invalid number of points");
		}
	}

	private void SetExplicitMode()
	{
		if (mState != "Reset")
		{
			throw new Exception("Cannot change mode after start");
		}
		mEndPointsMode = eEndPointsMode.EXPLICIT;
	}

	public void SetAutoCloseMode(float joiningPointTime)
	{
		if (mState != "Reset")
		{
			throw new Exception("Cannot change mode after start");
		}
		mEndPointsMode = eEndPointsMode.AUTOCLOSED;
		mNodes.Add(new SplineNode(mNodes[0] as SplineNode));
		((SplineNode)mNodes[mNodes.Count - 1]).Time = joiningPointTime;
		Vector3 normalized = (((SplineNode)mNodes[1]).Point - ((SplineNode)mNodes[0]).Point).normalized;
		Vector3 normalized2 = (((SplineNode)mNodes[mNodes.Count - 2]).Point - ((SplineNode)mNodes[mNodes.Count - 1]).Point).normalized;
		float magnitude = (((SplineNode)mNodes[1]).Point - ((SplineNode)mNodes[0]).Point).magnitude;
		float magnitude2 = (((SplineNode)mNodes[mNodes.Count - 2]).Point - ((SplineNode)mNodes[mNodes.Count - 1]).Point).magnitude;
		SplineNode splineNode = new SplineNode(mNodes[0] as SplineNode);
		splineNode.Point = ((SplineNode)mNodes[0]).Point + normalized2 * magnitude;
		SplineNode splineNode2 = new SplineNode(mNodes[mNodes.Count - 1] as SplineNode);
		splineNode2.Point = ((SplineNode)mNodes[0]).Point + normalized * magnitude2;
		mNodes.Insert(0, splineNode);
		mNodes.Add(splineNode2);
	}

	private Quaternion GetSquad(int idxFirstPoint, float t)
	{
		int num = idxFirstPoint - 1;
		if (num < 0)
		{
			num = 0;
		}
		Quaternion rot = ((SplineNode)mNodes[num]).Rot;
		Quaternion rot2 = ((SplineNode)mNodes[idxFirstPoint]).Rot;
		Quaternion rot3 = ((SplineNode)mNodes[idxFirstPoint + 1]).Rot;
		Quaternion rot4 = ((SplineNode)mNodes[idxFirstPoint + 2]).Rot;
		Quaternion squadIntermediate = MathUtils.GetSquadIntermediate(rot, rot2, rot3);
		Quaternion squadIntermediate2 = MathUtils.GetSquadIntermediate(rot2, rot3, rot4);
		return MathUtils.GetQuatSquad(t, rot2, rot3, squadIntermediate, squadIntermediate2);
	}

	private Vector3 GetHermiteInternal(int idxFirstPoint, float t)
	{
		float num = t * t;
		float num2 = num * t;
		int num3 = idxFirstPoint - 1;
		if (num3 < 0)
		{
			num3 = 0;
		}
		Vector3 point = ((SplineNode)mNodes[num3]).Point;
		Vector3 point2 = ((SplineNode)mNodes[idxFirstPoint]).Point;
		Vector3 point3 = ((SplineNode)mNodes[idxFirstPoint + 1]).Point;
		Vector3 point4 = ((SplineNode)mNodes[idxFirstPoint + 2]).Point;
		float num4 = 0.5f;
		Vector3 vector = num4 * (point3 - point);
		Vector3 vector2 = num4 * (point4 - point2);
		float num5 = 2f * num2 - 3f * num + 1f;
		float num6 = -2f * num2 + 3f * num;
		float num7 = num2 - 2f * num + t;
		float num8 = num2 - num;
		return num5 * point2 + num6 * point3 + num7 * vector + num8 * vector2;
	}

	public void GetHermiteAtTime(float timeParam, out Vector3 pos, out Quaternion rot)
	{
		if (timeParam >= ((SplineNode)mNodes[mNodes.Count - 2]).Time)
		{
			if (mNodes.Count > 2)
			{
				pos = ((SplineNode)mNodes[mNodes.Count - 2]).Point;
				rot = ((SplineNode)mNodes[mNodes.Count - 2]).Rot;
			}
			else
			{
				float t = (timeParam - ((SplineNode)mNodes[0]).Time) / (((SplineNode)mNodes[1]).Time - ((SplineNode)mNodes[0]).Time);
				pos = Vector3.Lerp(((SplineNode)mNodes[0]).Point, ((SplineNode)mNodes[1]).Point, t);
				rot = Quaternion.Slerp(((SplineNode)mNodes[0]).Rot, ((SplineNode)mNodes[1]).Rot, t);
			}
			return;
		}
		int i;
		for (i = 1; i < mNodes.Count - 2 && !(((SplineNode)mNodes[i]).Time > timeParam); i++)
		{
		}
		int num = i - 1;
		float t2 = (timeParam - ((SplineNode)mNodes[num]).Time) / (((SplineNode)mNodes[num + 1]).Time - ((SplineNode)mNodes[num]).Time);
		t2 = MathUtils.Ease(t2, ((SplineNode)mNodes[num]).EaseIO.x, ((SplineNode)mNodes[num]).EaseIO.y);
		pos = GetHermiteInternal(num, t2);
		rot = GetSquad(num, t2);
	}
}
