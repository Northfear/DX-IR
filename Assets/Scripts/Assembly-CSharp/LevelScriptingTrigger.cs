using UnityEngine;

public class LevelScriptingTrigger : MonoBehaviour
{
	public GameObject[] m_ActivationTarget;

	public GameObject[] m_DeletionTarget;

	public float m_DeletionDelay;

	public GameObject[] m_RedundantTrigger;

	public EnemySquad[] m_RetiredSquad;

	public int m_CommLinkIndex = -1;

	public GameObject m_AnimationTarget;

	public bool m_SetPrimaryObjective;

	public Transform m_PrimaryObjective;

	public bool m_SetSecondaryObjective;

	public Transform m_SecondaryObjective;

	public MonoBehaviour m_ScriptingTriggerClass;

	public string m_ScriptingTriggerName;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.gameObject.tag == "Player"))
		{
			return;
		}
		for (int num = m_ActivationTarget.Length; num > 0; num--)
		{
			m_ActivationTarget[num - 1].SetActiveRecursively(true);
		}
		for (int num2 = m_DeletionTarget.Length; num2 > 0; num2--)
		{
			Object.Destroy(m_DeletionTarget[num2 - 1], m_DeletionDelay);
		}
		for (int num3 = m_RedundantTrigger.Length; num3 > 0; num3--)
		{
			Object.Destroy(m_RedundantTrigger[num3 - 1]);
		}
		for (int num4 = m_RetiredSquad.Length; num4 > 0; num4--)
		{
			Globals.m_AIDirector.RemoveSquad(m_RetiredSquad[num4 - 1]);
		}
		if (m_CommLinkIndex >= 0)
		{
			CommLinkDialog.PlayDialog(m_CommLinkIndex);
		}
		if (m_AnimationTarget != null)
		{
			foreach (AnimationState item in m_AnimationTarget.animation)
			{
				item.speed = 1f;
			}
			m_AnimationTarget.animation.Play();
		}
		if (m_SetPrimaryObjective)
		{
			Globals.m_PrimaryObjective = m_PrimaryObjective;
		}
		if (m_SetSecondaryObjective)
		{
			Globals.m_SecondaryObjective = m_SecondaryObjective;
		}
		if (m_ScriptingTriggerClass != null && m_ScriptingTriggerName != string.Empty)
		{
			m_ScriptingTriggerClass.Invoke(m_ScriptingTriggerName, 0f);
		}
		Object.Destroy(base.gameObject);
	}
}
