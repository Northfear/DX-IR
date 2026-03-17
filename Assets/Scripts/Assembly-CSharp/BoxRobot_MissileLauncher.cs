using UnityEngine;

public class BoxRobot_MissileLauncher : MonoBehaviour
{
	public BoxRobot m_BoxRobotBase;

	public Transform[] m_RocketSpawnLocations;

	public GameObject m_RocketPrefab;

	public void FireMissile(int chamber)
	{
		if (m_RocketSpawnLocations == null || chamber < 0 || chamber >= m_RocketSpawnLocations.Length || !(m_RocketSpawnLocations[chamber] != null) || !(m_RocketPrefab != null))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(m_RocketPrefab, m_RocketSpawnLocations[chamber].position, m_RocketSpawnLocations[chamber].rotation) as GameObject;
		SoundManager.TriggerEvent("Play_MiniRPG_3rd_Fire", base.gameObject);
		if (gameObject != null)
		{
			Projectile_BoxRobotRocket component = gameObject.GetComponent<Projectile_BoxRobotRocket>();
			if (component != null)
			{
				component.m_TargetLocation = m_BoxRobotBase.m_TargetMissileLocation;
				component.m_Owner = m_BoxRobotBase;
				Vector2 vector = Random.insideUnitCircle * 4f;
				component.m_TargetLocation.x += vector.x;
				component.m_TargetLocation.z += vector.y;
			}
		}
	}

	public void SoundFootstep()
	{
		SoundManager.TriggerEvent("Play_BoxGuard_Footstep", base.gameObject);
	}

	public void SoundFoleyRel()
	{
		SoundManager.TriggerEvent("Play_BoxGuard_Foley_Rel", base.gameObject);
	}

	public void SoundFoleyShort()
	{
		SoundManager.TriggerEvent("Play_BoxGuard_Foley_Short", base.gameObject);
	}

	public void SoundFoleyMotors()
	{
		SoundManager.TriggerEvent("Play_BoxGuard_Motors", base.gameObject);
	}

	public void SoundHydraulic()
	{
		SoundManager.TriggerEvent("Play_BoxGuard_Hydraulic", base.gameObject);
	}

	public void SoundFall()
	{
		SoundManager.TriggerEvent("Play_BoxGuard_Fall", base.gameObject);
	}

	public void SoundString(string SoundString)
	{
		SoundManager.TriggerEvent(SoundString, base.gameObject);
	}
}
