using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public float distance = 5f, height = 1.2f, smoothLag = 0.3f, maxSpeed = 12f, snapLag = 0.3f,
	clampHeadPositionScreenSpace = 0.75f;

	private bool isSnapping;

	private Vector3 headOffset = Vector3.zero, centerOffset = Vector3.zero, velocity = Vector3.zero;

	private float targetHeight;

	private Transform target;

	private PlayerController playerController;

	private Camera camera;

	void Awake () {
		camera = GetComponent<Camera> ();

		target = GameObject.FindGameObjectWithTag ("Player").transform;

		CharacterController charController = target.GetComponent<CharacterController> ();

		centerOffset = charController.bounds.center - target.position;

		headOffset = centerOffset;
		headOffset.y = charController.bounds.max.y - target.position.y;

		playerController = target.GetComponent<PlayerController> ();
	}

	void LateUpdate () {
		Vector3 targetCenter = target.position + centerOffset, targetHead = target.position + headOffset;

		if (playerController.IsJumping ()) {
			float newTargetHeight = targetCenter.y + height;	

			if (newTargetHeight < targetHeight || newTargetHeight - targetHeight > 5) {
				targetHeight = targetCenter.y + height;
			}
		} else {
			targetHeight = targetCenter.y + height;
		}

		if (Input.GetButton ("Fire2") && !isSnapping) {
			velocity = Vector3.zero;

			isSnapping = true;
		}

		if (isSnapping) {
			ApplySnapping (targetCenter);
		} else {
			ApplyPositionDamping (new Vector3(targetCenter.x, targetHeight, targetCenter.z));
		}

		SetUpRotation (targetCenter);
	}

	void ApplyPositionDamping (Vector3 targetCenter) {
		Vector3 offset = transform.position - targetCenter;
		offset.y = 0f;

		Vector3 newTargetPos = offset.normalized * distance + targetCenter, newPosition;

		newPosition.x = Mathf.SmoothDamp (transform.position.x, newTargetPos.x, ref velocity.x,
			smoothLag, maxSpeed);
		newPosition.z = Mathf.SmoothDamp (transform.position.z, newTargetPos.z, ref velocity.z,
			smoothLag, maxSpeed);
		newPosition.y = Mathf.SmoothDamp (transform.position.y, newTargetPos.y, ref velocity.y,
			smoothLag, maxSpeed);

		transform.position = newPosition;
	}

	void SetUpRotation (Vector3 centerPos) {
		Vector3 offsetToCenter = centerPos - camera.transform.position;

		Quaternion yRotation = Quaternion.LookRotation (new Vector3(offsetToCenter.x, 0f, offsetToCenter.z));

		Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
		transform.rotation = yRotation * Quaternion.LookRotation (relativeOffset);

		Ray centerRay = camera.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 1f)),
		topRay = camera.ViewportPointToRay (new Vector3 (0.5f, clampHeadPositionScreenSpace, 1f));

		Vector3 centerRayPos = centerRay.GetPoint (distance), topRayPos = topRay.GetPoint (distance);

		float centerToTopAngle = Vector3.Angle (centerRay.direction, topRay.direction),
		heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y),
		extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);

		if (extraLookAngle < centerToTopAngle) {
			extraLookAngle = 0f;
		} else {
			extraLookAngle -= centerToTopAngle;

			transform.rotation *= Quaternion.Euler (-extraLookAngle, 0f, 0f);
		}
	}

	void ApplySnapping (Vector3 targetCenter) {
		Vector3 offset = transform.position - targetCenter;

		offset.y = 0f;

		float currentDistance = offset.magnitude, targetAngle = target.eulerAngles.y,
		currentAngle = transform.eulerAngles.y;

		currentAngle = Mathf.SmoothDampAngle (currentAngle, targetAngle, ref velocity.x, snapLag);

		currentDistance = Mathf.SmoothDampAngle (currentDistance, distance, ref velocity.z, snapLag);

		Vector3 newPosition = targetCenter;

		newPosition += Quaternion.Euler (0f, currentAngle, 0f) * Vector3.back * currentDistance;

		newPosition.y = Mathf.SmoothDamp (transform.position.y, targetCenter.y + height, ref velocity.y, 
			smoothLag, maxSpeed);

		transform.position = newPosition;

		if (AngleDistance (currentAngle, targetAngle) < 3f) {
			isSnapping = false;

			velocity = Vector3.zero;
		}
	}

	float AngleDistance (float a, float b) {
		return Mathf.Abs(b - a);
	}
}
