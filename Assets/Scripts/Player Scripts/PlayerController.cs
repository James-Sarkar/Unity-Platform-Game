using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Public variables
	public float walkSpeed = 3f, trotSpeed = 5f, runSpeed = 8f, inAirControlAcceleration = 3f, jumpHeight = 0.5f,
	extraJumpHeight = 2.5f, gravity = 20f, controlledDescentGravity = 2f, speedSmoothing = 80f, rotateSpeed = 700f,
	trotAfterSeconds = 0.3f;

	// Private variables
	private bool canJump = true, canControlDescent = true, jumping, jumpingReachedApex, movingBack, isMoving,
	slammed, isControllable = true;

	private float jumpRepeatTime = 0.05f, jumpTimeout = 0.15f, groundedTimeout = 0.25f, lockCameraTimer = 0f,
	verticalSpeed = 0f, moveSpeed = 0f, lastJumpStartHeight = 0f, touchWallJumpTime = -1f, lastGroundedTime = 0f,
	walkTimeStart = 0f, lastJumpButtonTime = -10f, lastJumpTime = -1f;

	private Vector3 moveDirection = Vector3.zero, wallJumpContactNormal = Vector3.zero, inAirVelocity = Vector3.zero;

	private CollisionFlags collisionFlags;

	private CharacterController characterController;

	private Transform cameraTransform;

	private SkinnedMeshRenderer playerRenderer;

	void Awake () {
		characterController = GetComponent<CharacterController> ();

		moveDirection = transform.TransformDirection (transform.forward);

		cameraTransform = Camera.main.transform;

		playerRenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
	}

	// Update is called once per frame
	void Update () {
		if (!isControllable) {
			Input.ResetInputAxes ();
		}

		if (Input.GetButtonDown ("Jump")) {
			lastJumpButtonTime = Time.time;
		}

		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0f, verticalSpeed, 0f) + inAirVelocity;
		movement *= Time.deltaTime;

		collisionFlags = characterController.Move (movement);

		if (IsGrounded ()) {
			if (slammed) {
				slammed = false;

				characterController.height = 2f;

				transform.position = new Vector3 (transform.position.x, transform.position.y + 0.75f,
					transform.position.z);
			}

			transform.rotation = Quaternion.LookRotation (moveDirection);
		} else {
			if (!slammed) {
				Vector3 xzMove = movement;

				xzMove.y = 0f;

				if (xzMove.sqrMagnitude > 0.001f) {
					transform.rotation = Quaternion.LookRotation (xzMove);
				}
			}
		}

		if (IsGrounded ()) {
			lastGroundedTime = Time.time;

			inAirVelocity = Vector3.zero;

			if (jumping) {
				jumping = false;
				SendMessage ("DidLand");
			}
		}

		UpdateSmoothMovementDirection ();

		ApplyGravity ();

		ApplyJumping ();
	}

	void UpdateSmoothMovementDirection() {
		Vector3 forward = cameraTransform.TransformDirection (Vector3.forward);

		forward.y = 0f;

		forward = forward.normalized;

		Vector3 right = new Vector3 (forward.z, 0f, -forward.x);

		float v = Input.GetAxisRaw ("Vertical"), h = Input.GetAxisRaw ("Horizontal");

		movingBack = v < -0.2f ? true : false;

		bool wasMoving = isMoving;

		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;

		Vector3 targetDirection = h * right + v * forward;

		if (IsGrounded ()) {
			lockCameraTimer += Time.deltaTime;

			if (isMoving != wasMoving) {
				lockCameraTimer = 0f;
			}

			if (targetDirection != Vector3.zero) {
				if (moveSpeed < walkSpeed * 0.9f && IsGrounded ()) {
					moveDirection = targetDirection.normalized;
				} else {
					moveDirection = Vector3.RotateTowards (moveDirection, targetDirection,
						rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					moveDirection = moveDirection.normalized;
				}
			}

			float curSmooth = speedSmoothing * Time.deltaTime, targetSpeed = Mathf.Min (targetDirection.magnitude, 1.0f);

			if (Input.GetButton ("Fire3")) {
				targetSpeed *= runSpeed;
			} else if (Time.time - trotAfterSeconds > walkTimeStart) {
				targetSpeed *= trotSpeed;
			} else {
				targetSpeed *= walkSpeed;
			}

			moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);

			if (moveSpeed < walkSpeed * 0.3f) {
				walkTimeStart = Time.time;
			}
		} else {
			if (jumping) {
				lockCameraTimer = 0f;
			}

			if (isMoving) {
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
			}
		}
	}

	void ApplyGravity() {
		if (isControllable) {
			bool jumpButton = Input.GetButton ("Jump"),
			controlledDescent = canControlDescent && verticalSpeed <= 0.0f && jumpButton && jumping;

			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f) {
				jumpingReachedApex = true;
			}

			bool extraPowerJump = IsJumping () && verticalSpeed > 0.0f && jumpButton &&
				transform.position.y < lastJumpStartHeight + extraJumpHeight;

			if (controlledDescent) { 
				verticalSpeed -= controlledDescentGravity * Time.deltaTime;
			} else if (extraPowerJump) {
				return;
			} else if (IsGrounded ()) {
				verticalSpeed = 0f;
			} else {
				verticalSpeed -= gravity * Time.deltaTime;
			}
		}
	}

	void ApplyJumping() {
		if (lastJumpTime + jumpRepeatTime > Time.time) {
			return;
		}

		if (IsGrounded ()) {
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);

				DidJump ();
			}	
		}
	}

	void DidJump () {
		jumping = true;

		jumpingReachedApex = false;

		lastJumpTime = Time.time;

		lastJumpStartHeight = transform.position.y;

		lastJumpButtonTime = -10f;
	}

	bool IsGrounded() {
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}

	public bool IsJumping() {
		return jumping && !slammed;
	}

	float CalculateJumpVerticalSpeed(float targetJumpHeight) {
		return Mathf.Sqrt (2f * targetJumpHeight * gravity);
	}

	// These are for animation
	public float GetSpeed() {
		return moveSpeed;
	}

	public bool IsControlledDescent() {
		bool jumpBtn = Input.GetButton ("Jump");

		return canControlDescent && verticalSpeed <= 0.0f && jumpBtn && jumping;
	}

	public bool HasJumpReachedApex() {
		return jumpingReachedApex;
	}

	public bool IsGroundedWithTimer() {
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	// When the robots hit the player
	void Slam(Vector3 direction) {
		verticalSpeed = CalculateJumpVerticalSpeed (1f);

		inAirVelocity = direction * 6f;

		direction.y = 0.6f;

		Quaternion.LookRotation (-direction);

		characterController.height = 0.5f;

		slammed = true;

		collisionFlags = CollisionFlags.None;

		DidJump ();
	}

	public bool IsMoving() {
		return Mathf.Abs (Input.GetAxisRaw ("Vertical")) + 
			Mathf.Abs (Input.GetAxisRaw ("Horizontal")) > 0.5f;
	}

	public void SuperJump(float height) {
		verticalSpeed = CalculateJumpVerticalSpeed (height);

		collisionFlags = CollisionFlags.None;

		DidJump ();
	}

	// To hide the player
	void HidePlayer() {
		playerRenderer.enabled = false;

		isControllable = false;
	}

	void ShowPlayer() {
		playerRenderer.enabled = true;

		isControllable = true;
	}
} 
