using System.Collections;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Load Scene Button")]
public class UIBtnLoadScene : UIButton
{
	public string scene;

	public UIPanelBase loadingPanel;

	public void LoadSceneDelegate(UIPanelBase panel, EZTransition trans)
	{
		StartCoroutine(LoadScene());
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
		{
			return;
		}
		base.OnInput(ref ptr);
		if (!m_controlIsEnabled || IsHidden() || ptr.evt != whenToInvoke)
		{
			return;
		}
		if (loadingPanel != null)
		{
			UIPanelManager uIPanelManager = (UIPanelManager)loadingPanel.Container;
			loadingPanel.AddTempTransitionDelegate(LoadSceneDelegate);
			if (uIPanelManager is UIPanelManager && uIPanelManager != null)
			{
				uIPanelManager.BringIn(loadingPanel);
			}
			else
			{
				loadingPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
			}
		}
		else
		{
			Invoke("DoLoadScene", delay);
		}
	}

	protected void DoLoadScene()
	{
		StartCoroutine(LoadScene());
	}

	protected IEnumerator LoadScene()
	{
		yield return null;
		Application.LoadLevel(scene);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (s is UIBtnLoadScene)
		{
			UIBtnLoadScene uIBtnLoadScene = (UIBtnLoadScene)s;
			if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
			{
				scene = uIBtnLoadScene.scene;
				loadingPanel = uIBtnLoadScene.loadingPanel;
			}
		}
	}

	public new static UIBtnLoadScene Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIBtnLoadScene)gameObject.AddComponent(typeof(UIBtnLoadScene));
	}

	public new static UIBtnLoadScene Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIBtnLoadScene)gameObject.AddComponent(typeof(UIBtnLoadScene));
	}
}
