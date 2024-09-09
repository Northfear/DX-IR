using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Image Effects/Mobile Bloom")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class MobileBloom : MonoBehaviour
{
	public float intensity;

	public Color colorMix;

	public float colorMixBlend;

	public float agonyTint;

	private Shader bloomShader;

	private Material apply;

	private RenderTextureFormat rtFormat;

	public MobileBloom()
	{
		intensity = 0.5f;
		colorMix = Color.white;
		colorMixBlend = 0.25f;
		rtFormat = RenderTextureFormat.Default;
	}

	public virtual void Start()
	{
		FindShaders();
		CheckSupport();
		CreateMaterials();
	}

	public virtual void FindShaders()
	{
		if (!bloomShader)
		{
			bloomShader = Shader.Find("Hidden/MobileBloom");
		}
	}

	public virtual void CreateMaterials()
	{
		if (!apply)
		{
			apply = new Material(bloomShader);
			apply.hideFlags = HideFlags.DontSave;
		}
	}

	public virtual void OnDamage()
	{
		agonyTint = 1f;
	}

	public virtual bool Supported()
	{
		bool num = SystemInfo.supportsImageEffects;
		if (num)
		{
			num = SystemInfo.supportsRenderTextures;
		}
		if (num)
		{
			num = bloomShader.isSupported;
		}
		return num;
	}

	public virtual bool CheckSupport()
	{
		int result;
		if (!Supported())
		{
			enabled = false;
			result = 0;
		}
		else
		{
			rtFormat = ((!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565)) ? RenderTextureFormat.Default : RenderTextureFormat.RGB565);
			result = 1;
		}
		return (byte)result != 0;
	}

	public virtual void OnDisable()
	{
		if ((bool)apply)
		{
			UnityEngine.Object.DestroyImmediate(apply);
			apply = null;
		}
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		agonyTint = Mathf.Clamp01(agonyTint - Time.deltaTime * 2.75f);
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, (int)rtFormat);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, (int)rtFormat);
		apply.SetColor("_ColorMix", colorMix);
		apply.SetVector("_Parameter", new Vector4(colorMixBlend * 0.25f, 0f, 0f, 1f - intensity - agonyTint));
		Graphics.Blit(source, temporary, apply, (!(agonyTint >= 0.5f)) ? 1 : 5);
		Graphics.Blit(temporary, temporary2, apply, 2);
		Graphics.Blit(temporary2, temporary, apply, 3);
		apply.SetTexture("_Bloom", temporary);
		Graphics.Blit(source, destination, apply, (QualityManager.quality > Quality.Medium) ? 4 : 0);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}

	public virtual void Main()
	{
	}
}
