using System;

[Serializable]
public class EnergyExpansionData : AugData
{
	public int m_EnergyContainerAmount = 1;

	public override void Purchase()
	{
		base.Purchase();
		Globals.m_PlayerController.SetMaxEnergy(m_EnergyContainerAmount);
	}
}
