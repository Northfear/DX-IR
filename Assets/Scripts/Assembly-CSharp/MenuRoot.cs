using UnityEngine;

public class MenuRoot : MonoBehaviour
{
	public Camera m_MenuCamera2D;

	public Camera m_MenuCamera3D;

	public UIPanelManager m_PanelManager;

	private void Awake()
	{
		Globals.m_MenuRoot = this;
	}

	private void Start()
	{
		SetupMenuCameras();
	}

	private void SetupMenuCameras()
	{
		bool flag = false;
		if (m_MenuCamera3D != null)
		{
			UIManager.instance.ResetCameras(m_MenuCamera3D, 2048, 100f);
			flag = true;
		}
		if (m_MenuCamera2D != null)
		{
			if (flag)
			{
				UIManager.instance.AddCamera(m_MenuCamera2D, 1024, 100f, 0);
			}
			else
			{
				UIManager.instance.ResetCameras(m_MenuCamera2D, 1024, 100f);
			}
			flag = true;
		}
		if (!flag)
		{
			UIManager.instance.ClearCameras();
		}
	}
}
