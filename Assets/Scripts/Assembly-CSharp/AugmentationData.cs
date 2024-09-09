using UnityEngine;

public class AugmentationData : MonoBehaviour
{
	public enum Augmentations
	{
		AimStabilization = 0,
		Movement = 1,
		HackingCapture = 2,
		SocialEnhancer = 3,
		Energy = 4,
		Strength = 5,
		InfoLink = 6,
		Cloaking = 7,
		BulletTime = 8,
		IcarusLanding = 9
	}

	public Augmentation_AimStabilization m_AimStabilizationUpgrades;

	public Augmentation_Movement m_MoveSilentlyUpgrades;

	public Augmentation_HackingCapture m_HackingCaptureUpgrades;

	public Augmentation_SocialEnhancer m_SocialEnhancerUpgrades;

	public Augmentation_Energy m_EnergyUpgrades;

	public Augmentation_Strength m_StengthUpgrades;

	private void Awake()
	{
		Globals.m_AugmentationData = this;
		if (m_AimStabilizationUpgrades.m_UpgradeData.Length == 0)
		{
			m_AimStabilizationUpgrades.m_UpgradeData = new AimStabilizationData[1];
		}
		if (m_MoveSilentlyUpgrades.m_UpgradeData.Length == 0)
		{
			m_MoveSilentlyUpgrades.m_UpgradeData = new MovementData[1];
		}
		if (m_HackingCaptureUpgrades.m_UpgradeData.Length == 0)
		{
			m_HackingCaptureUpgrades.m_UpgradeData = new HackingCaptureData[1];
		}
		if (m_SocialEnhancerUpgrades.m_UpgradeData.Length == 0)
		{
			m_SocialEnhancerUpgrades.m_UpgradeData = new SocialEnhancerData[1];
		}
		if (m_EnergyUpgrades.m_UpgradeData.Length == 0)
		{
			m_EnergyUpgrades.m_UpgradeData = new EnergyExpansionData[1];
		}
		if (m_StengthUpgrades.m_UpgradeData.Length == 0)
		{
			m_StengthUpgrades.m_UpgradeData = new StrengthData[1];
		}
	}

	public AugmentationContainer GetAugmentationContainer(Augmentations augment)
	{
		switch (augment)
		{
		case Augmentations.AimStabilization:
			return m_AimStabilizationUpgrades;
		case Augmentations.Movement:
			return m_MoveSilentlyUpgrades;
		case Augmentations.HackingCapture:
			return m_HackingCaptureUpgrades;
		case Augmentations.SocialEnhancer:
			return m_SocialEnhancerUpgrades;
		case Augmentations.Energy:
			return m_EnergyUpgrades;
		case Augmentations.Strength:
			return m_StengthUpgrades;
		default:
			return null;
		}
	}

	public AugData GetAugmentationData(Augmentations augment, int dataIndex)
	{
		AugmentationContainer augmentationContainer = GetAugmentationContainer(augment);
		if (augmentationContainer != null)
		{
			return augmentationContainer.GetAugData(dataIndex);
		}
		return null;
	}
}
