using UnityEngine;

public class DeadEnemySimulator : MonoBehaviour
{
	private void Update()
	{
		if (Globals.m_AIDirector.CheckVisualSenses(base.collider, DisturbanceEvent.MajorVisual, AudioEvent.DeadBodyFound))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
