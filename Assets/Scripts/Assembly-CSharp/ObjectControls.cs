using System;
using UnityEngine;

[Serializable]
public class ObjectControls
{
	public enum DominationState
	{
		Default = 0,
		Disabled = 1,
		Enemy = 2
	}

	public ObjectControl_Button m_Default;

	public ObjectControl_Button m_Disable;

	public ObjectControl_Button m_Enemies;

	public ObjectControl_NoConnections m_NoConnections;

	[HideInInspector]
	public DominationState m_DominationState;

	public void NoConnections()
	{
		m_Default.Hide(true);
		m_Disable.Hide(true);
		m_Enemies.Hide(true);
		m_NoConnections.Hide(false);
	}

	public void Connections()
	{
		m_Default.Hide(false);
		m_Disable.Hide(false);
		m_Enemies.Hide(false);
		m_NoConnections.Hide(true);
	}

	public void ChangeState(DominationState newState)
	{
		m_DominationState = newState;
		Connections();
		m_Default.SetActive(newState == DominationState.Default);
		m_Disable.SetActive(newState == DominationState.Disabled);
		m_Enemies.SetActive(newState == DominationState.Enemy);
	}
}
