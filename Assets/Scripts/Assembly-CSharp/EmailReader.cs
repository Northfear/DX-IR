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
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		Globals.m_PlayerController.m_DisableController = false;
	}

	private void Awake()
	{
		m_InfoTextFrom.Text = m_From;
		m_InfoTextFrom.Text = "[#FFFFFF]" + m_InfoTextFrom.Text;
		m_TextFrom.Text = m_From;
		m_TextFrom.Text = "[#E4A52E]FROM: [#FFFF]" + m_TextFrom.Text;
		m_TextTo.Text = m_To;
		m_TextTo.Text = "[#FFFFFF]TO: [#000000]" + m_TextTo.Text;
		m_InfoTextSubject.Text = m_Subject;
		m_InfoTextSubject.Text = "[#000000]" + m_InfoTextSubject.Text;
		m_TextSubject.Text = m_Subject;
		m_TextSubject.Text = "[#FFFFFF]" + m_TextSubject.Text;
		m_Body = m_Body.Replace("\\n", "\n");
		m_TextBody.Text = m_Body;
		m_TextBody.Text = "[#C6C6C6]" + m_TextBody.Text;
		UIManager.instance.AddCamera(m_EmailCamera, 4096, 100f, 0);
		GameManager.AddMediaLog(MediaType.EMail, m_To, m_From, m_Subject, m_Body);
	}

	private void CleanupEmail(UIPanelBase panel, EZTransition transition)
	{
		Globals.m_HUD.Display(true, true, false);
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
				SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
				Globals.m_PlayerController.m_DisableController = true;
			}
		}
	}
}
