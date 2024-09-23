using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class QualityManager : MonoBehaviour
{
	public bool autoChoseQualityOnStart;

	public Quality currentQuality;

	public MobileBloom bloom;

	[NonSerialized]
	public static Quality quality = Quality.Highest;

	public QualityManager()
	{
		autoChoseQualityOnStart = true;
		currentQuality = Quality.Highest;
	}

	public virtual void Awake()
	{
		if (!bloom)
		{
			bloom = GetComponent<MobileBloom>();
		}
		if (autoChoseQualityOnStart)
		{
			AutoDetectQuality();
		}
		ApplyAndSetQuality(currentQuality);
	}

	private void AutoDetectQuality()
	{
#if UNITY_IPHONE
		switch (iPhone.generation)
		{
		case iPhoneGeneration.iPad1Gen:
			currentQuality = Quality.Low;
			break;
		case iPhoneGeneration.iPad2Gen:
			currentQuality = Quality.High;
			break;
		case iPhoneGeneration.iPhone3GS:
		case iPhoneGeneration.iPodTouch3Gen:
			currentQuality = Quality.Low;
			break;
		default:
			currentQuality = Quality.Medium;
			break;
		}
		Debug.Log(string.Format("AngryBots: Quality set to '{0}'{1}", currentQuality, " (" + iPhone.generation + " class iOS)"));
#else
		currentQuality = Quality.Highest;
#endif
	}

	private void ApplyAndSetQuality(Quality newQuality)
	{
		quality = newQuality;
		if (quality == Quality.Lowest)
		{
			DisableAllFx();
			EnableFx(bloom, false);
			camera.depthTextureMode = DepthTextureMode.None;
		}
		else if (quality == Quality.Poor)
		{
			EnableFx(bloom, false);
			camera.depthTextureMode = DepthTextureMode.None;
		}
		else if (quality == Quality.Low)
		{
			EnableFx(bloom, false);
			camera.depthTextureMode = DepthTextureMode.None;
		}
		else if (quality == Quality.Medium)
		{
			EnableFx(bloom, true);
			camera.depthTextureMode = DepthTextureMode.None;
		}
		else if (quality == Quality.High)
		{
			EnableFx(bloom, true);
			camera.depthTextureMode = DepthTextureMode.None;
		}
		else
		{
			EnableFx(bloom, true);
			camera.depthTextureMode |= DepthTextureMode.Depth;
		}
		Debug.Log("AngryBots: setting shader LOD to " + quality);
	}

	private void DisableAllFx()
	{
		camera.depthTextureMode = DepthTextureMode.None;
		EnableFx(bloom, false);
	}

	private void EnableFx(MonoBehaviour fx, bool enable)
	{
		if ((bool)fx)
		{
			fx.enabled = enable;
		}
	}

	public virtual void Main()
	{
	}
}
