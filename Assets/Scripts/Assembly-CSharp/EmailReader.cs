using Fabric;
using UnityEngine;

public class EmailReader : MonoBehaviour
{
	public string m_To;

	public string m_From;

	public string m_Subject;

	public string m_Body;

	public UIPanel m_EmailPanel;

	public Camera m_EmailCamera;

	public SpriteText m_InfoTextFrom;

	public SpriteText m_InfoTextSubject;

	public SpriteText m_TextBody;

	public SpriteText m_TextFrom;

	public SpriteText m_TextSubject;

	public SpriteText m_TextTo;

	private float m_CurrentDelay;

	public void Disconnect()
	{
		m_EmailPanel.AddTempTransitionDelegate(CleanupEmail);
		m_EmailPanel.Dismiss();
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
	}

	private void Awake()
	{
		m_InfoTextFrom.Text += m_From;
		m_TextFrom.Text += m_From;
		m_TextTo.Text += m_To;
		m_InfoTextSubject.Text += m_Subject;
		m_TextSubject.Text += m_Subject;
		m_Body = m_Body.Replace("\\n", "\n");
		m_TextBody.Text = m_Body;
		UIManager.instance.AddCamera(m_EmailCamera, 4096, 100f, 0);
	}

	private void CleanupEmail(UIPanelBase panel, EZTransition transition)
	{
		Globals.m_HUD.Display(true, true);
		Globals.m_HUD.EnablePassThruInput(true);
		Globals.m_PlayerController.ToggleWeaponHolstered();
		UIManager.instance.RemoveCamera(m_EmailCamera);
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (m_CurrentDelay < 1f)
		{
			m_CurrentDelay += Time.deltaTime;
			if (m_CurrentDelay >= 1f)
			{
				m_EmailPanel.BringIn();
				EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
			}
		}
	}
}
