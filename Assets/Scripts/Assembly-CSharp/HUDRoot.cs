using UnityEngine;

public class HUDRoot : MonoBehaviour
{
	public Camera m_HUDCamera2D;

	public Camera m_HUDCamera3D;

	public UIPanelManager m_PanelManager;

	private void Awake()
	{
		Globals.m_HUDRoot = this;
	}

	private void Start()
	{
		SetupHUDCameras();
		SetParentCamera();
	}

	public void SetParentCamera()
	{
		base.transform.parent = Globals.m_PlayerController.m_CurrentCamera.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
	}

	private void SetupHUDCameras()
	{
		bool flag = false;
		if (m_HUDCamera3D != null)
		{
			UIManager.instance.ResetCameras(m_HUDCamera3D, 8192, 100f);
			flag = true;
		}
		if (m_HUDCamera2D != null)
		{
			if (flag)
			{
				UIManager.instance.AddCamera(m_HUDCamera2D, 4096, 100f, 0);
			}
			else
			{
				UIManager.instance.ResetCameras(m_HUDCamera2D, 4096, 100f);
			}
			flag = true;
		}
		if (!flag)
		{
			UIManager.instance.ClearCameras();
		}
	}
}
