using UnityEngine;

public class PlayerMovement : TouchDragBase
{
	private const float m_CoverEdgeCollisionRayLength = 0.7f;

	[HideInInspector]
	public bool m_IsMoving;

	[HideInInspector]
	public float m_CurrentMovementSpeed;

	[HideInInspector]
	public float m_NormalizedSpeed;

	[HideInInspector]
	public Vector3 m_CharacterMovementVector;

	[HideInInspector]
	public bool m_CoverMovingBackward;

	[HideInInspector]
	public CharacterController m_PlayerCharacterController;

	public GameObject m_TapToMoveTargetObject;

	public GameObject m_TapToCoverTargetObject;

	public float m_TapToMoveTurnSpeed = 4f;

	public float m_TapMinRadius = 30f;

	private bool m_TapToMoving;

	private Vector2 m_TapPressPosition;

	private Vector3 m_TargetLocation;

	private Vector3 m_TargetNormal;

	private Collider m_TargetCollider;

	private int m_MovementTapID = -1;

	private SplineInterpolator m_CameraSpline = new SplineInterpolator();

	[HideInInspector]
	public NavMeshAgent m_NavAgent;

	protected NavMeshPath m_NavMeshPath = new NavMeshPath();

	private int m_CurrentNavMeshCorner;

	private float m_PathLength;

	private float m_PathDistanceTravelled;

	private float m_DistToNextCorner;

	private static float m_CameraDraggedTimer;

	public static float m_CameraDragDelay = 1f;

	public float m_CoverEdgeCollisionDist = 0.2f;

	[HideInInspector]
	public float m_CoverWallCollisionDist = 0.49f;

	private bool m_CoverMove;

	private bool m_InCoverMove;

	private float m_InCoverMoveDist;

	private Vector3 m_InCoverMoveDir;

	private bool m_MovementPressed;

	private float m_StickTimer;

	private float m_MovementPullRange = (float)Screen.width * 0.1f;

	private Vector2 m_MovementPressPosition;

	private Vector2 m_MovementHoldPosition;

	private int m_MovementStickID = -1;

	public float m_DeadZoneVertical = 0.05f;

	public float m_DeadZoneHorizontal = 0.05f;

	protected void Start()
	{
		m_PlayerCharacterController = GetComponent<CharacterController>();
		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.Stop(true);
		m_TapToMoveTargetObject.transform.parent = null;
		m_TapToMoveTargetObject.SetActiveRecursively(false);
		m_TapToCoverTargetObject.transform.parent = null;
		m_TapToCoverTargetObject.SetActiveRecursively(false);
	}

	protected void Update()
	{
		m_CharacterMovementVector = Vector3.zero;
		m_CurrentMovementSpeed = 0f;
		m_IsMoving = false;
		UpdateTap();
		UpdateStick();
		m_NormalizedSpeed = (Mathf.Abs(m_CurrentMovementSpeed) - Globals.m_PlayerController.GetMinSpeed()) / (Globals.m_PlayerController.GetRunSpeed() - Globals.m_PlayerController.GetMinSpeed());
		if (m_IsMoving)
		{
			m_PlayerCharacterController.SimpleMove(m_CharacterMovementVector);
		}
		if (Globals.m_PlayerController.m_CoverState != 0 && m_IsMoving)
		{
			Globals.m_PlayerController.UpdateCover();
			if (!Globals.m_PlayerController.m_ForceCoverEdgeFacing || Globals.m_PlayerController.m_CoverEdge == PlayerController.CoverEdge.None)
			{
				float num = Vector3.Dot(base.transform.right, m_CharacterMovementVector);
				Vector3 forward = Globals.m_PlayerController.m_Camera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				float num2 = Vector3.Dot(base.transform.right, forward);
				m_CoverMovingBackward = false;
				if (num > 0f && num2 < 0f - Globals.m_PlayerController.m_CoverSwitchSidesDot && Globals.m_PlayerController.m_CoverSide != PlayerController.CoverSide.Right)
				{
					m_CoverMovingBackward = true;
				}
				else if (num < 0f && num2 > Globals.m_PlayerController.m_CoverSwitchSidesDot && Globals.m_PlayerController.m_CoverSide != 0)
				{
					m_CoverMovingBackward = true;
				}
				if (num > 0f && num2 > 0f - Globals.m_PlayerController.m_CoverSwitchSidesDot && Globals.m_PlayerController.m_CoverSide != PlayerController.CoverSide.Right)
				{
					Globals.m_PlayerController.SetCoverState(PlayerController.CoverState.SwitchingSides);
				}
				else if (num < 0f && num2 < Globals.m_PlayerController.m_CoverSwitchSidesDot && Globals.m_PlayerController.m_CoverSide != 0)
				{
					Globals.m_PlayerController.SetCoverState(PlayerController.CoverState.SwitchingSides);
				}
			}
		}
		if (!(Globals.m_HUD != null))
		{
			return;
		}
		Globals.m_HUD.TurnOnCoverOuterFlipButton(false, MainHUD.CoverFlipButtonSide.Left);
		Globals.m_HUD.TurnOnCoverOuterFlipButton(false, MainHUD.CoverFlipButtonSide.Right);
		if (Globals.m_PlayerController.m_CoverEdge != 0 && Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Inside && Globals.m_PlayerController.m_CoverCollider.gameObject.tag != "Cover_DisableCornering" && Globals.m_PlayerController.m_CoverAllowsCornering)
		{
			float num3 = 1f;
			if (Globals.m_PlayerController.m_CoverEdge == PlayerController.CoverEdge.Left)
			{
				num3 = -1f;
			}
			float num4 = 0.5f;
			Ray ray = new Ray(base.transform.position + base.transform.right * num3 * (m_CoverEdgeCollisionDist + num4) + base.transform.forward * (m_CoverEdgeCollisionDist + num4 + Globals.m_PlayerController.m_CoverDistFromWall) + new Vector3(0f, 0.5f, 0f), base.transform.right * (0f - num3));
			RaycastHit hitInfo;
			switch (Globals.m_PlayerController.m_CoverSide)
			{
			case PlayerController.CoverSide.Left:
				if (Physics.Raycast(ray, out hitInfo, m_CoverEdgeCollisionDist + num4, 256))
				{
					Globals.m_HUD.TurnOnCoverOuterFlipButton(true, MainHUD.CoverFlipButtonSide.Left);
				}
				break;
			case PlayerController.CoverSide.Right:
				if (Physics.Raycast(ray, out hitInfo, m_CoverEdgeCollisionDist + num4, 256))
				{
					Globals.m_HUD.TurnOnCoverOuterFlipButton(true, MainHUD.CoverFlipButtonSide.Right);
				}
				break;
			}
		}
		Globals.m_HUD.TurnOnCoverInnerFlipButton(false, MainHUD.CoverFlipButtonSide.Left);
		Globals.m_HUD.TurnOnCoverInnerFlipButton(false, MainHUD.CoverFlipButtonSide.Right);
		if (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Inside)
		{
			return;
		}
		Vector3 direction = base.transform.right;
		if (Globals.m_PlayerController.m_CoverSide == PlayerController.CoverSide.Left)
		{
			direction = -base.transform.right;
		}
		Ray ray2 = new Ray(base.transform.position + new Vector3(0f, 0.5f, 0f), direction);
		RaycastHit hitInfo2;
		if (Physics.Raycast(ray2, out hitInfo2, m_CoverWallCollisionDist, 256))
		{
			if (Globals.m_PlayerController.m_CoverSide == PlayerController.CoverSide.Left)
			{
				Globals.m_HUD.TurnOnCoverInnerFlipButton(true, MainHUD.CoverFlipButtonSide.Left);
			}
			else
			{
				Globals.m_HUD.TurnOnCoverInnerFlipButton(true, MainHUD.CoverFlipButtonSide.Right);
			}
		}
	}

	private void UpdateTap()
	{
		if (m_InCoverMove)
		{
			if (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.SwitchingSides)
			{
				Vector3 inCoverMoveDir = m_InCoverMoveDir;
				inCoverMoveDir.y = 0f;
				if (!CoverMovementCollision(inCoverMoveDir * m_CoverEdgeCollisionDist))
				{
					m_InCoverMoveDist -= Globals.m_PlayerController.GetRunSpeed() * Time.deltaTime;
					if (m_InCoverMoveDist > 0f)
					{
						m_CharacterMovementVector = m_InCoverMoveDir;
						m_CharacterMovementVector *= Globals.m_PlayerController.GetRunSpeed();
					}
					else
					{
						m_InCoverMove = false;
						m_InCoverMoveDist = 0f;
					}
				}
				else
				{
					CancelMovement();
				}
			}
		}
		else if (m_TapToMoving)
		{
			if (m_NavAgent.remainingDistance > 0f)
			{
				m_CharacterMovementVector = m_NavAgent.steeringTarget - base.transform.position;
				m_CharacterMovementVector.Normalize();
				m_CharacterMovementVector *= Globals.m_PlayerController.GetRunSpeed();
				Vector3 vector = m_NavAgent.steeringTarget;
				if (m_CurrentNavMeshCorner == 0 && vector == m_NavMeshPath.corners[m_NavMeshPath.corners.Length - 1])
				{
					vector = m_NavMeshPath.corners[1];
				}
				bool flag = false;
				for (int i = 1; i < m_NavMeshPath.corners.Length; i++)
				{
					Vector3 vector2 = m_NavMeshPath.corners[i];
					if (!(vector2 == vector) && !flag)
					{
						continue;
					}
					if (i >= m_CurrentNavMeshCorner || flag)
					{
						vector = vector2;
						if (i != m_CurrentNavMeshCorner)
						{
							m_DistToNextCorner = Vector3.Distance(base.transform.position, vector);
						}
						m_CurrentNavMeshCorner = i;
						break;
					}
					flag = true;
				}
				float num = Vector3.Distance(base.transform.position, vector);
				m_PathDistanceTravelled += m_DistToNextCorner - num;
				m_DistToNextCorner = num;
				m_CameraDraggedTimer -= Time.deltaTime;
				if (m_CameraDraggedTimer <= 0f && Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Outside)
				{
					Quaternion rot;
					if (m_CurrentNavMeshCorner == m_NavMeshPath.corners.Length - 1)
					{
						Vector3 vector3 = vector;
						vector3.y = base.transform.position.y;
						rot = Quaternion.LookRotation(vector3 - base.transform.position, new Vector3(0f, 1f, 0f));
					}
					else
					{
						float value = m_PathDistanceTravelled / m_PathLength;
						value = Mathf.Clamp(value, 0f, 1f);
						Vector3 pos;
						m_CameraSpline.GetHermiteAtTime(value, out pos, out rot);
					}
					Quaternion quaternion = Quaternion.Slerp(base.transform.rotation, rot, Time.deltaTime * m_TapToMoveTurnSpeed);
					Globals.m_CameraController.SetYaw(quaternion.eulerAngles.y);
				}
			}
			else
			{
				m_TapToMoving = false;
			}
		}
		if (m_InCoverMove || m_TapToMoving)
		{
			m_IsMoving = true;
			m_CurrentMovementSpeed = Globals.m_PlayerController.GetRunSpeed();
			return;
		}
		if (m_CoverMove && Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Outside)
		{
			if (Globals.m_PlayerController.m_NearInteractiveObject <= 0)
			{
				Globals.m_PlayerController.EnterCover(m_TargetLocation, m_TargetNormal, m_TargetCollider, true);
			}
			m_NavAgent.ResetPath();
			m_NavAgent.Stop(true);
		}
		m_InCoverMove = false;
		m_CoverMove = false;
		m_TapToCoverTargetObject.SetActiveRecursively(false);
		m_TapToMoveTargetObject.SetActiveRecursively(false);
	}

	private void TapWorld(Vector2 screenposition)
	{
		if (Globals.m_PlayerController.m_CoverState != 0 && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Inside && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.SwitchingSides && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Reloading)
		{
			return;
		}
		Ray ray = Globals.m_PlayerController.m_CurrentCamera.ScreenPointToRay(screenposition);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 50f, 2162945))
		{
			return;
		}
		m_TapToCoverTargetObject.SetActiveRecursively(false);
		m_TapToMoveTargetObject.SetActiveRecursively(false);
		Vector3 vector = hitInfo.point;
		vector.y = Globals.m_PlayerController.GetGroundHeight(vector + hitInfo.normal * 0.05f) + 0.1f;
		if (hitInfo.collider.gameObject.layer == 8 && hitInfo.normal.y == 0f)
		{
			if (Globals.m_PlayerController.m_CoverState != 0)
			{
				if (Vector3.Distance(hitInfo.normal, Globals.m_PlayerController.m_CoverNormal) > 0.01f)
				{
					return;
				}
				Vector3 vector2 = Vector3.Scale(hitInfo.point, hitInfo.normal);
				Vector3 vector3 = Vector3.Scale(Globals.m_PlayerController.m_CoverPoint, hitInfo.normal);
				if (Mathf.Abs(vector2.sqrMagnitude - vector3.sqrMagnitude) > 0.01f)
				{
					return;
				}
			}
			vector = hitInfo.point + hitInfo.normal * Globals.m_PlayerController.m_CoverDistFromWall;
			vector.y = Globals.m_PlayerController.GetGroundHeight(vector) + 0.05f;
			m_CoverMove = true;
			m_TapToCoverTargetObject.SetActiveRecursively(true);
			m_TapToCoverTargetObject.transform.position = hitInfo.point;
			Globals.m_HUD.TurnOnCoverButton(false);
			Globals.m_PlayerController.m_ForceCoverButtonInactive = true;
			m_TapToCoverTargetObject.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
		}
		else
		{
			if (Globals.m_PlayerController.m_CoverState != 0)
			{
				return;
			}
			m_CoverMove = false;
			m_TapToMoveTargetObject.SetActiveRecursively(true);
			m_TapToMoveTargetObject.transform.position = hitInfo.point;
		}
		m_TargetLocation = hitInfo.point;
		m_TargetNormal = hitInfo.normal;
		m_TargetCollider = hitInfo.collider;
		if (Globals.m_PlayerController.m_CoverState != 0)
		{
			m_TargetLocation += m_TargetNormal * Globals.m_PlayerController.m_CoverDistFromWall;
			m_TargetLocation.y = Globals.m_PlayerController.GetGroundHeight(m_TargetLocation);
			m_InCoverMove = true;
			m_TapToMoving = false;
			m_CoverMove = false;
			m_InCoverMoveDist = Vector3.Distance(m_TargetLocation, base.transform.position);
			m_InCoverMoveDir = m_TargetLocation - base.transform.position;
			m_InCoverMoveDir.Normalize();
		}
		else
		{
			m_InCoverMove = false;
			m_TapToMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			if (m_NavAgent.CalculatePath(vector, m_NavMeshPath))
			{
				m_NavAgent.SetPath(m_NavMeshPath);
				m_CurrentNavMeshCorner = 0;
				m_PathDistanceTravelled = 0f;
				m_PathLength = 0f;
				Vector3 vector4;
				Vector3 vector5;
				for (int i = 0; i < m_NavMeshPath.corners.Length - 1; i++)
				{
					vector4 = m_NavMeshPath.corners[i];
					vector4.y = 0f;
					vector5 = m_NavMeshPath.corners[i + 1];
					vector5.y = 0f;
					m_PathLength += (vector5 - vector4).magnitude;
				}
				m_CameraSpline.Reset();
				m_CameraSpline.AddPoint(base.transform.forward, base.transform.rotation, 0f, new Vector2(0f, 1f));
				vector4 = m_NavMeshPath.corners[0];
				vector4.y = 0f;
				vector5 = m_NavMeshPath.corners[1];
				vector5.y = 0f;
				Vector3 vector6 = vector5 - vector4;
				float num = vector6.magnitude;
				m_DistToNextCorner = vector6.magnitude;
				for (int j = 1; j < m_NavMeshPath.corners.Length; j++)
				{
					Vector3 vector7 = vector6;
					if (j < m_NavMeshPath.corners.Length - 1)
					{
						vector4 = m_NavMeshPath.corners[j];
						vector4.y = 0f;
						vector5 = m_NavMeshPath.corners[j + 1];
						vector5.y = 0f;
						vector7 = vector5 - vector4;
					}
					float magnitude = vector7.magnitude;
					vector7 /= magnitude;
					vector6 = vector7;
					Quaternion quat = Quaternion.LookRotation(vector7);
					float timeInSeconds = num / m_PathLength;
					m_CameraSpline.AddPoint(vector7, quat, timeInSeconds, new Vector2(0f, 1f));
					num += magnitude;
				}
			}
			else
			{
				CancelTapToMove();
				Debug.Log("No path in a tap to move attempt, movetopos: " + vector);
			}
		}
		m_CameraDraggedTimer = 0f;
	}

	public static void CameraDragged()
	{
		m_CameraDraggedTimer = m_CameraDragDelay;
	}

	private void UpdateStick()
	{
		if ((Globals.m_PlayerController.m_CoverState != 0 && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Inside && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Crouch && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Reloading) || !m_MovementPressed)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float value = (m_MovementHoldPosition.y - m_MovementPressPosition.y) / m_MovementPullRange;
		value = Mathf.Clamp(value, -1f, 1f);
		float value2 = (m_MovementHoldPosition.x - m_MovementPressPosition.x) / m_MovementPullRange;
		value2 = Mathf.Clamp(value2, -1f, 1f);
		if (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Outside)
		{
			if (Mathf.Abs(value2) >= m_DeadZoneHorizontal)
			{
				m_IsMoving = true;
				num = Globals.m_PlayerController.GetStrafeSpeed() * value2;
				if (Mathf.Abs(num) < Globals.m_PlayerController.GetMinSpeed())
				{
					num = ((!(value2 < 0f)) ? Globals.m_PlayerController.GetMinSpeed() : (0f - Globals.m_PlayerController.GetMinSpeed()));
				}
				m_CharacterMovementVector += base.transform.right * num;
			}
			if (!m_TapToMoving && Mathf.Abs(value) >= m_DeadZoneVertical)
			{
				m_IsMoving = true;
				num2 = Globals.m_PlayerController.GetRunSpeed() * value;
				if (Mathf.Abs(num2) < Globals.m_PlayerController.GetMinSpeed())
				{
					num2 = ((!(value < 0f)) ? Globals.m_PlayerController.GetMinSpeed() : (0f - Globals.m_PlayerController.GetMinSpeed()));
				}
				m_CharacterMovementVector += base.transform.forward * num2;
			}
			if (m_CharacterMovementVector.magnitude > Globals.m_PlayerController.GetRunSpeed())
			{
				m_CharacterMovementVector.Normalize();
				m_CharacterMovementVector *= Globals.m_PlayerController.GetRunSpeed();
			}
			m_CurrentMovementSpeed = m_CharacterMovementVector.magnitude;
			if (!m_TapToMoving)
			{
				return;
			}
			if (value < -0.9f)
			{
				CancelTapToMove();
			}
			if (value > m_DeadZoneVertical)
			{
				Vector3 rhs = m_NavAgent.steeringTarget - base.transform.position;
				rhs.Normalize();
				float num3 = Vector3.Dot(base.transform.forward, rhs);
				if (num3 < 0.7f)
				{
					CancelTapToMove();
				}
			}
		}
		else
		{
			if (!(Mathf.Abs(value2) >= m_DeadZoneHorizontal))
			{
				return;
			}
			float num4 = 1f;
			if (value2 < 0f)
			{
				num4 = -1f;
			}
			if (CoverMovementCollision(base.transform.right * m_CoverEdgeCollisionDist * num4))
			{
				return;
			}
			m_IsMoving = true;
			m_CurrentMovementSpeed = Globals.m_PlayerController.GetRunSpeed() * value2;
			if (Mathf.Abs(m_CurrentMovementSpeed) < Globals.m_PlayerController.GetMinSpeed())
			{
				if (value2 < 0f)
				{
					m_CurrentMovementSpeed = 0f - Globals.m_PlayerController.GetMinSpeed();
				}
				else
				{
					m_CurrentMovementSpeed = Globals.m_PlayerController.GetMinSpeed();
				}
			}
			m_CharacterMovementVector += base.transform.right * m_CurrentMovementSpeed;
		}
	}

	private bool CoverMovementCollision(Vector3 velocity)
	{
		Ray ray = new Ray(base.transform.position + velocity + new Vector3(0f, 0.1f, 0f), base.transform.forward);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 0.7f, 256))
		{
			return true;
		}
		ray = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), velocity);
		if (Physics.Raycast(ray, out hitInfo, m_CoverWallCollisionDist, 256))
		{
			return true;
		}
		return false;
	}

	public override void ButtonPress(Vector2 position, int id)
	{
		m_TapPressPosition = position;
		m_MovementTapID = id;
		if (m_MovementStickID == -1 && position.x < (float)(Screen.width / 2) && position.y < (float)(Screen.height / 2) && (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Outside || !m_InCoverMove))
		{
			m_StickTimer = Globals.m_TapTimeLimit;
			m_MovementPressPosition = position;
			m_MovementHoldPosition = position;
			m_MovementStickID = id;
			base.ButtonPress(position, id);
		}
	}

	public override void ButtonHeld(Vector2 position, int id)
	{
		if (m_MovementStickID == id && m_StickTimer > 0f)
		{
			m_StickTimer -= Time.deltaTime;
			if (m_StickTimer <= 0f || Vector2.Distance(m_MovementPressPosition, position) > 10f)
			{
				m_MovementPressed = true;
			}
		}
		if (m_MovementStickID != id || !m_MovementPressed)
		{
			return;
		}
		m_MovementHoldPosition = position;
		base.ButtonHeld(position, id);
		if (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Inside)
		{
			float value = (m_MovementHoldPosition.y - m_MovementPressPosition.y) / m_MovementPullRange;
			value = Mathf.Clamp(value, -1f, 1f);
			if (value < -0.9f)
			{
				Globals.m_PlayerController.ExitCover();
			}
		}
	}

	public override void ButtonRelease(Vector2 position, int id)
	{
		if (m_MovementTapID == id)
		{
			float num = Vector3.Distance(m_TapPressPosition, position);
			if (!Globals.m_DisableTapToMove && Globals.m_PlayerController.GetTapTime(id) < Globals.m_TapTimeLimit && num < m_TapMinRadius && Globals.m_PlayerController.m_PossibleEnemyTarget == null && !Globals.m_PlayerController.m_TappedInteractiveObject)
			{
				TapWorld(position);
			}
			m_MovementTapID = -1;
		}
		if (m_MovementStickID == id)
		{
			m_MovementStickID = -1;
			m_MovementPressed = false;
			m_StickTimer = 0f;
			base.ButtonRelease(position, id);
		}
	}

	public void CancelTapToMove()
	{
		m_MovementTapID = -1;
		m_TapToMoving = false;
		m_CoverMove = false;
		m_InCoverMove = false;
		m_NavAgent.Stop();
		m_NavAgent.ResetPath();
		m_TapToCoverTargetObject.SetActiveRecursively(false);
		m_TapToMoveTargetObject.SetActiveRecursively(false);
	}

	public override void CancelMovement()
	{
		CancelTapToMove();
		m_MovementStickID = -1;
		m_InCoverMove = false;
		m_MovementPressed = false;
		base.CancelMovement();
	}
}
