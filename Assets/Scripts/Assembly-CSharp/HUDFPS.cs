using System.Collections;
using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
	public Rect startRect = new Rect(10f, 10f, 75f, 50f);

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum;

	private int frames;

	private Color color = Color.white;

	private string sFPS = string.Empty;

	private GUIStyle style;

	private void Start()
	{
		StartCoroutine(FPS());
	}

	private void Update()
	{
		accum += Time.timeScale / Time.deltaTime;
		frames++;
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			float fps = accum / (float)frames;
			sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
			color = ((fps >= 28f) ? Color.green : ((!(fps > 10f)) ? Color.yellow : Color.red));
			accum = 0f;
			frames = 0;
			Globals.m_HUD.m_FPS.Color = color;
			Globals.m_HUD.m_FPS.Text = "FPS: " + sFPS;
			yield return new WaitForSeconds(frequency);
		}
	}

	private void OnEnable()
	{
		if (Globals.m_HUD != null && Globals.m_HUD.m_FPS != null)
		{
			Globals.m_HUD.m_FPS.gameObject.active = true;
		}
	}

	private void OnDisable()
	{
		if (Globals.m_HUD != null && Globals.m_HUD.m_FPS != null)
		{
			Globals.m_HUD.m_FPS.gameObject.active = false;
		}
	}
}
