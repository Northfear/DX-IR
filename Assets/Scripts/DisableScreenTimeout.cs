using UnityEngine;
using System.Collections;

public class DisableScreenTimeout : MonoBehaviour
{
	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
}
