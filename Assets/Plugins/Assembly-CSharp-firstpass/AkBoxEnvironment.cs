using System;
using UnityEngine;

public class AkBoxEnvironment : AkAuxSend
{
	public override float GetAuxSendValueForPosition(Vector3 in_pos)
	{
		Vector3 lhs = in_pos - base.transform.position;
		lhs.x = Math.Abs(lhs.x);
		lhs.y = Math.Abs(lhs.y);
		lhs.z = Math.Abs(lhs.z);
		Vector3 vector = base.transform.lossyScale / 2f;
		lhs = Vector3.Min(lhs, vector);
		Vector3 vector2 = vector - new Vector3(rollOffDistance, rollOffDistance, rollOffDistance);
		float num = rollOffDistance;
		float num2 = 0f;
		if (lhs.x > vector2.x)
		{
			num = lhs.x;
			num2 = vector2.x;
		}
		else if (lhs.z > vector2.z)
		{
			num = lhs.z;
			num2 = vector2.z;
		}
		else if (lhs.y > vector2.y)
		{
			num = lhs.y;
			num2 = vector2.y;
		}
		if (num2 == 0f)
		{
			return 1f;
		}
		return 1f - (num - num2) / rollOffDistance;
	}
}
