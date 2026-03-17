using UnityEngine;

public class DestroyOnAwake : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
