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
		IcarusLanding = 8,
		Armor = 9,
		SmartVision = 10,
		ReflexBooster = 11,
		HackingFortify = 12,
		HackingStealth = 13,
		Total = 14
	}

	public Augmentation_AimStabilization m_AimStabilizationUpgrades;

	public Augmentation_Movement m_MoveSilentlyUpgrades;

	public Augmentation_HackingCapture m_HackingCaptureUpgrades;

	public Augmentation_SocialEnhancer m_SocialEnhancerUpgrades;

	public Augmentation_Energy m_EnergyUpgrades;

	public Augmentation_Strength m_StengthUpgrades;

	public Augmentation_InfoLink m_InfoLinkUpgrades;

	public Augmentation_Cloaking m_CloakingUpgrades;

	public Augmentation_IcarusLanding m_IcarusLandingUpgrades;

	public Augmentation_Armor m_ArmorUpgrades;

	public Augmentation_SmartVision m_SmartVision;

	public Augmentation_ReflexBooster m_ReflexBooster;

	public Augmentation_HackingFortify m_HackingFortify;

	public Augmentation_HackingStealth m_HackingStealth;

	private void Awake()
	{
		Globals.m_AugmentationData = this;
		if (m_AimStabilizationUpgrades.m_UpgradeData.Length == 0)
		{
			m_AimStabilizationUpgrades.m_UpgradeData = new AimStabilizationData[2];
		}
		if (m_MoveSilentlyUpgrades.m_UpgradeData.Length == 0)
		{
			m_MoveSilentlyUpgrades.m_UpgradeData = new MovementData[1];
		}
		if (m_HackingCaptureUpgrades.m_UpgradeData.Length == 0)
		{
			m_HackingCaptureUpgrades.m_UpgradeData = new HackingCaptureData[7];
		}
		if (m_SocialEnhancerUpgrades.m_UpgradeData.Length == 0)
		{
			m_SocialEnhancerUpgrades.m_UpgradeData = new SocialEnhancerData[1];
		}
		if (m_EnergyUpgrades.m_UpgradeData.Length == 0)
		{
			m_EnergyUpgrades.m_UpgradeData = new EnergyData[6];
		}
		if (m_StengthUpgrades.m_UpgradeData.Length == 0)
		{
			m_StengthUpgrades.m_UpgradeData = new StrengthData[2];
		}
		if (m_InfoLinkUpgrades.m_UpgradeData.Length == 0)
		{
			m_InfoLinkUpgrades.m_UpgradeData = new InfoLinkData[1];
		}
		if (m_CloakingUpgrades.m_UpgradeData.Length == 0)
		{
			m_CloakingUpgrades.m_UpgradeData = new CloakingData[3];
		}
		if (m_IcarusLandingUpgrades.m_UpgradeData.Length == 0)
		{
			m_IcarusLandingUpgrades.m_UpgradeData = new IcarusLandingData[1];
		}
		if (m_ArmorUpgrades.m_UpgradeData.Length == 0)
		{
			m_ArmorUpgrades.m_UpgradeData = new ArmorData[3];
		}
		if (m_SmartVision.m_UpgradeData.Length == 0)
		{
			m_SmartVision.m_UpgradeData = new SmartVisionData[1];
		}
		if (m_ReflexBooster.m_UpgradeData.Length == 0)
		{
			m_ReflexBooster.m_UpgradeData = new ReflexBoosterData[1];
		}
		if (m_HackingFortify.m_UpgradeData.Length == 0)
		{
			m_HackingFortify.m_UpgradeData = new HackingFortifyData[3];
		}
		if (m_HackingStealth.m_UpgradeData.Length == 0)
		{
			m_HackingStealth.m_UpgradeData = new HackingStealthData[3];
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
		case Augmentations.InfoLink:
			return m_InfoLinkUpgrades;
		case Augmentations.Cloaking:
			return m_CloakingUpgrades;
		case Augmentations.IcarusLanding:
			return m_IcarusLandingUpgrades;
		case Augmentations.Armor:
			return m_ArmorUpgrades;
		case Augmentations.SmartVision:
			return m_SmartVision;
		case Augmentations.ReflexBooster:
			return m_ReflexBooster;
		case Augmentations.HackingFortify:
			return m_HackingFortify;
		case Augmentations.HackingStealth:
			return m_HackingStealth;
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
