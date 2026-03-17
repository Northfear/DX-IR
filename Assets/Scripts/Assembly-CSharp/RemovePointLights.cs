using UnityEngine;

public class RemovePointLights : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
