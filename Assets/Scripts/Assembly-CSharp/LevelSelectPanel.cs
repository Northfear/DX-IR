using Fabric;
using UnityEngine;

public class LevelSelectPanel : MonoBehaviour
{
	private static LevelSelectPanel m_This;

	public UIPanel m_Panel;

	public GameObject m_Window;

	public UIScrollList m_LevelList;

	public GameObject m_LevelButtonPrefab;

	private void Awake()
	{
		m_This = this;
		base.transform.localPosition = new Vector3(0f, 0f, 5f);
		base.gameObject.SetActiveRecursively(false);
	}

	private void Start()
	{
		if (GameManager.m_This.m_GameScenes == null || !(m_LevelList != null))
		{
			return;
		}
		for (int i = 0; i < GameManager.m_This.m_GameScenes.Length; i++)
		{
			if (GameManager.m_This.m_GameScenes[i].m_IncludeInBuild)
			{
				GameObject gameObject = Object.Instantiate(m_LevelButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				UIListButton component = gameObject.GetComponent<UIListButton>();
				if ((bool)component)
				{
					component.RenderCamera = Globals.m_MenuRoot.m_MenuCamera2D;
					component.Data = i;
					component.AddValueChangedDelegate(LevelSelected);
					component.Text = GameManager.m_This.m_GameScenes[i].m_DisplayName;
				}
				m_Panel.AddSubject(gameObject);
				m_LevelList.AddItem(gameObject);
			}
		}
	}

	public static void OpenPanel()
	{
		if (!(m_This == null) && !(m_This.m_Panel == null))
		{
			m_This.m_Panel.BringIn();
			if ((bool)m_This.m_Window)
			{
				AnimateScale.Do(m_This.m_Window, EZAnimation.ANIM_MODE.FromTo, new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.35f, 0f, null, null);
			}
		}
	}

	public void CancelPressed()
	{
		m_Panel.Dismiss();
		Globals.m_MenuRoot.m_PanelManager.BringIn("OptionsPanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}

	public void LevelSelected(IUIObject obj)
	{
		GameManager.LoadLevel((int)obj.Data);
		EventManager.Instance.PostEvent("Music_Menu", EventAction.StopSound, null);
	}
}
