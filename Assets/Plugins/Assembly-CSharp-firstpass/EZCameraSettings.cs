using System;
using UnityEngine;

[Serializable]
public class EZCameraSettings
{
	public Camera camera;

	public LayerMask mask = -1;

	public float rayDepth = float.PositiveInfinity;
}
