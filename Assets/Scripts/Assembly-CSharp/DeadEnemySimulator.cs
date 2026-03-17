using UnityEngine;

public class DeadEnemySimulator : MonoBehaviour
{
	private bool DeadBodyFound;

	private bool Simulate;

	private void Awake()
	{
		base.renderer.enabled = Simulate;
	}

	private void Update()
	{
		if (Simulate)
		{
			if (!DeadBodyFound && Globals.m_AIDirector.CheckVisualSenses(base.collider, DisturbanceEvent.MajorVisual, AudioEvent.DeadBodyFound))
			{
				DeadBodyFound = true;
			}
			Globals.m_AIDirector.ShowBodyToCameras(base.collider);
		}
		if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha1))
		{
			Simulate = !Simulate;
			base.renderer.enabled = Simulate;
		}
		else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha2))
		{
			DeadBodyFound = !DeadBodyFound;
		}
	}
}
