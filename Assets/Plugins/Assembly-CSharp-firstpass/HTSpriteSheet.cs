using UnityEngine;

public class HTSpriteSheet : MonoBehaviour
{
	public enum CameraFacingMode
	{
		BillBoard = 0,
		Horizontal = 1,
		Vertical = 2,
		Never = 3
	}

	public Material spriteSheetMaterial;

	public int spriteCount;

	public int uvAnimationTileX;

	public int uvAnimationTileY;

	public int framesPerSecond;

	public Vector3 sizeStart = new Vector3(1f, 1f, 1f);

	public Vector3 sizeEnd = new Vector3(1f, 1f, 1f);

	public bool randomRotation;

	public float rotationStart;

	public float rotationEnd;

	public bool isOneShot = true;

	public float life;

	public CameraFacingMode billboarding;

	public bool addLightEffect;

	public float lightRange;

	public Color lightColor;

	public float lightFadeSpeed = 1f;

	public bool addColorEffect;

	public Color colorStart = new Color(1f, 1f, 1f, 1f);

	public Color colorEnd = new Color(1f, 1f, 1f, 1f);

	public bool foldOut;

	public Vector3 offset;

	public float waittingTime;

	public bool copy;

	private Mesh mesh;

	private MeshRenderer meshRender;

	private AudioSource soundEffect;

	private float startTime;

	private Transform mainCamTransform;

	private bool effectEnd;

	private float randomZAngle;

	private Color colorStep;

	private Color currentColor;

	private Vector3 sizeStep;

	private Vector3 currentSize;

	private float currentRotation;

	private float rotationStep;

	private float lifeStart;

	private Transform myTransform;

	private void Awake()
	{
		CreateParticle();
		mainCamTransform = Camera.main.transform;
		soundEffect = GetComponent<AudioSource>();
		if (addLightEffect)
		{
			base.gameObject.AddComponent("Light");
			base.gameObject.light.color = lightColor;
			base.gameObject.light.range = lightRange;
		}
		base.renderer.enabled = false;
	}

	private void Start()
	{
		InitSpriteSheet();
	}

	private void Update()
	{
		bool flag = false;
		Camera_BillboardingMode();
		float num = (Time.time - startTime) * (float)framesPerSecond;
		if (!isOneShot && life > 0f && Time.time - lifeStart > life)
		{
			effectEnd = true;
		}
		if ((num <= (float)spriteCount || !isOneShot) && !effectEnd)
		{
			if (num >= (float)spriteCount)
			{
				startTime = Time.time;
				num = 0f;
				if (addColorEffect)
				{
					currentColor = colorStart;
					meshRender.material.SetColor("_Color", currentColor);
				}
				currentSize = sizeStart;
				myTransform.localScale = currentSize;
				if (randomRotation)
				{
					currentRotation = Random.Range(-180f, 180f);
				}
				else
				{
					currentRotation = rotationStart;
				}
			}
			num %= (float)(uvAnimationTileX * uvAnimationTileY);
			Vector2 scale = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
			float num2 = Mathf.Floor(num % (float)uvAnimationTileX);
			float num3 = Mathf.Floor(num / (float)uvAnimationTileX);
			Vector2 vector = new Vector2(num2 * scale.x, 1f - scale.y - num3 * scale.y);
			base.renderer.material.SetTextureOffset("_MainTex", vector);
			base.renderer.material.SetTextureScale("_MainTex", scale);
			base.renderer.enabled = true;
		}
		else
		{
			effectEnd = true;
			base.renderer.enabled = false;
			flag = true;
			if ((bool)soundEffect && soundEffect.isPlaying)
			{
				flag = false;
			}
			if (addLightEffect && flag && base.gameObject.light.intensity > 0f)
			{
				flag = false;
			}
			if (flag)
			{
				Object.Destroy(base.gameObject);
			}
		}
		if (sizeStart != sizeEnd)
		{
			myTransform.localScale += sizeStep * Time.deltaTime;
		}
		if (addLightEffect && lightFadeSpeed != 0f)
		{
			base.gameObject.light.intensity -= lightFadeSpeed * Time.deltaTime;
		}
		if (addColorEffect)
		{
			currentColor = new Color(currentColor.r + colorStep.r * Time.deltaTime, currentColor.g + colorStep.g * Time.deltaTime, currentColor.b + colorStep.b * Time.deltaTime, currentColor.a + colorStep.a * Time.deltaTime);
			meshRender.material.SetColor("_TintColor", currentColor);
		}
	}

	private void CreateParticle()
	{
		mesh = base.gameObject.AddComponent<MeshFilter>().mesh;
		meshRender = base.gameObject.AddComponent<MeshRenderer>();
		mesh.vertices = new Vector3[4]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f)
		};
		mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		mesh.uv = new Vector2[4]
		{
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),
			new Vector2(0f, 0f)
		};
		meshRender.castShadows = false;
		meshRender.receiveShadows = false;
		mesh.RecalculateNormals();
		base.renderer.material = spriteSheetMaterial;
	}

	private void Camera_BillboardingMode()
	{
		Vector3 vector = myTransform.position - mainCamTransform.position;
		switch (billboarding)
		{
		case CameraFacingMode.BillBoard:
			myTransform.LookAt(mainCamTransform.position - vector);
			break;
		case CameraFacingMode.Horizontal:
			vector.x = (vector.z = 0f);
			myTransform.LookAt(mainCamTransform.position - vector);
			break;
		case CameraFacingMode.Vertical:
			vector.y = (vector.z = 0f);
			myTransform.LookAt(mainCamTransform.position - vector);
			break;
		}
		if (rotationStart != rotationEnd)
		{
			currentRotation += rotationStep * Time.deltaTime;
		}
		myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y, currentRotation);
	}

	public void InitSpriteSheet()
	{
		startTime = Time.time;
		lifeStart = Time.time;
		myTransform = base.transform;
		float num = (float)spriteCount / (float)framesPerSecond;
		sizeStep = new Vector3((sizeEnd.x - sizeStart.x) / num, (sizeEnd.y - sizeStart.y) / num, (sizeEnd.z - sizeStart.z) / num);
		currentSize = sizeStart;
		myTransform.localScale = currentSize;
		rotationStep = (rotationEnd - rotationStart) / num;
		if (randomRotation)
		{
			currentRotation = Random.Range(-180f, 180f);
		}
		else
		{
			currentRotation = rotationStart;
		}
		if (addColorEffect)
		{
			colorStep = new Color((colorEnd.r - colorStart.r) / num, (colorEnd.g - colorStart.g) / num, (colorEnd.b - colorStart.b) / num, (colorEnd.a - colorStart.a) / num);
			currentColor = colorStart;
			meshRender.material.SetColor("_TintColor", currentColor);
		}
	}
}
