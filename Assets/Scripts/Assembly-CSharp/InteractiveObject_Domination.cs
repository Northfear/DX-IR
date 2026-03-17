using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject_Domination : InteractiveObject_Base
{
	public GameObject m_DominationPrefab;

	public SecurityCamera[] m_ConnectedCameras = new SecurityCamera[4];

	[HideInInspector]
	public List<Turret> m_ConnectedTurrets;

	[HideInInspector]
	public List<Sentry> m_ConnectedSentries;

	[HideInInspector]
	public int m_SelectedCameraIdx;

	protected override void Awake()
	{
		m_BlockCover = true;
		base.Awake();
	}

	public override bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.EnableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.DisableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
		{
			return false;
		}
		if (m_DominationPrefab == null)
		{
			return false;
		}
		DisableUI();
		GameObject gameObject = (GameObject)Object.Instantiate(m_DominationPrefab);
		gameObject.GetComponent<DominationControlPanel>().RegisterInteractiveObject(this);
		return true;
	}

	public void RegisterTurret(Turret turret)
	{
		m_ConnectedTurrets.Add(turret);
	}

	public void RegisterSentry(Sentry sentry)
	{
		m_ConnectedSentries.Add(sentry);
	}

	private void DisableUI()
	{
		Globals.m_HUD.Display(false, true, false);
		Globals.m_HUD.EnablePassThruInput(false);
		Globals.m_PlayerController.ToggleWeaponHolstered();
		Globals.m_PlayerController.CancelMovement();
	}
}
