using Fabric;
using UnityEngine;

public class FabricCustomerTimer : MonoBehaviour, ICustomTimer
{
	float ICustomTimer.Get()
	{
		return Time.realtimeSinceStartup;
	}

	public void Start()
	{
		FabricManager.Instance.RegisterCustomTimer(this);
	}

	public void Destroy()
	{
		FabricManager.Instance.RegisterCustomTimer(null);
	}
}
