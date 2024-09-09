using System;
using UnityEngine;

public interface IGUIHelper
{
	Enum EnumField(string label, Enum selected);

	Color ColorField(string label, Color color);

	Vector3 Vector3Field(string label, Vector3 val);

	float FloatField(string label, float val);

	string TextField(string label, string val);

	UnityEngine.Object ObjectField(string label, Type type, UnityEngine.Object obj);
}
