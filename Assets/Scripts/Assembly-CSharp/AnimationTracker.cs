using UnityEngine;

public class AnimationTracker : MonoBehaviour
{
	public Animation m_Animator;

	public Rect m_Pos = new Rect(0f, 0f, 650f, 450f);

	private Vector2 m_Scroll = Vector2.zero;

	public bool m_ActiveOnly = true;

	private bool m_Visible;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F12))
		{
			m_Visible = !m_Visible;
		}
	}

	private void OnGUI()
	{
		if (m_Visible)
		{
			m_Pos = GUILayout.Window(12345, m_Pos, OnWindow, "Animations");
		}
	}

	private void OnWindow(int id)
	{
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 15f));
		if (m_Animator == null)
		{
			return;
		}
		m_Scroll = GUILayout.BeginScrollView(m_Scroll);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Name");
		GUILayout.Label("Active", GUILayout.Width(50f));
		GUILayout.Label("Weight", GUILayout.Width(50f));
		GUILayout.Label("Layer", GUILayout.Width(50f));
		GUILayout.Label("Wrapmode", GUILayout.Width(100f));
		GUILayout.EndHorizontal();
		foreach (AnimationState item in m_Animator)
		{
			if (!m_ActiveOnly || item.enabled)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(item.name);
				GUILayout.Label(item.enabled.ToString(), GUILayout.Width(50f));
				GUILayout.Label(item.weight.ToString("0.00"), GUILayout.Width(50f));
				GUILayout.Label(item.layer.ToString(), GUILayout.Width(50f));
				GUILayout.Label(item.wrapMode.ToString(), GUILayout.Width(100f));
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndScrollView();
	}
}
