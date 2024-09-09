using UnityEngine;

public class Hacking_Circuit : MonoBehaviour
{
	public UIButton m_circuitButton;

	public HackingNode m_nodeConnectedTo;

	public PackedSprite m_CircuitToNodeConnection_Off;

	public PackedSprite m_CircuitToNodeConnection_On;

	private bool m_isPressed;

	private bool m_isShortCircuiting;

	private bool m_finishedShortCircuiting;

	private float m_shortCircuitTime;

	public bool IsPressed()
	{
		return m_isPressed;
	}

	public bool IsFinishedShortCircuiting()
	{
		return m_finishedShortCircuiting;
	}

	public void OnCircuitButtonPressed(POINTER_INFO info)
	{
		m_isPressed = true;
		m_CircuitToNodeConnection_Off.Hide(true);
		m_CircuitToNodeConnection_On.Hide(false);
		if (!m_finishedShortCircuiting)
		{
			m_nodeConnectedTo.CircuitPressed();
		}
	}

	public void OnCircuitButtonReleased(POINTER_INFO info)
	{
		m_isPressed = false;
		m_CircuitToNodeConnection_Off.Hide(false);
		m_CircuitToNodeConnection_On.Hide(true);
		m_nodeConnectedTo.CircuitReleased();
	}

	public void BeginShortCircuit()
	{
		if (!m_finishedShortCircuiting)
		{
			m_isShortCircuiting = true;
		}
	}

	public void CancelShortCircuiting()
	{
		if (m_isShortCircuiting)
		{
			m_isShortCircuiting = false;
			m_shortCircuitTime = 0f;
			if (!m_isPressed)
			{
			}
		}
	}

	public void FinishedShortCircuiting()
	{
		m_finishedShortCircuiting = true;
		m_isShortCircuiting = false;
	}

	private void Start()
	{
		RegisterWithNode();
		m_CircuitToNodeConnection_Off.Hide(false);
		m_CircuitToNodeConnection_On.Hide(true);
	}

	private void Update()
	{
		if (!m_finishedShortCircuiting && m_isShortCircuiting)
		{
			m_shortCircuitTime += Time.deltaTime;
			if (m_shortCircuitTime >= HackingSystem.m_this.GetTimeToShortCircuit())
			{
				m_nodeConnectedTo.FinishedShortCircuiting();
			}
		}
	}

	private void RegisterWithNode()
	{
		m_nodeConnectedTo.RegisterCircuit(this);
	}
}
