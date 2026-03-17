using System;

[Serializable]
public class EnergyData : AugData
{
	public int m_EnergyContainerAmount = 2;

	public int m_RechargeCapacity = 2;

	public float m_EnergyRechargeRate = 0.05f;

	public override void Purchase()
	{
		base.Purchase();
		Globals.m_PlayerController.CheckForRechargeCapacityIncrease();
	}
}
