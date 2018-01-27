using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float attackTurnTime = 0.7f, attackDistance = 17f, attackSpeed = 5f, attackRotateSpeed = 20f,
	rotateSpeed = 120f, extraRunTime = 2f, idleTime = 1.6f, punchRadius = 1.1f;

	public Vector3 punchPosition = new Vector3 (0.4f, 0f, 0.7f);

	public int damage = 1;

	public AudioClip idleSound, attackSound;

	private float attackAngle = 10f, lastPunchTime = 0f;

	private bool isAttacking;

	private Transform target;

	private CharacterController charController;

	private Animation anim;

	private AudioSource audioManager;

	private string idleClip = "idle", threatenClip = "threaten", turnJumpClip = "turnjump", gotHitClip = "gothit",
	attackRunClip = "attackrun";

	void Awake () {
		charController = GetComponent<CharacterController> ();

		target = GameObject.FindGameObjectWithTag ("Player").transform;

		anim = GetComponent<Animation> ();

		audioManager = GetComponent<AudioSource> ();

		anim.wrapMode = WrapMode.Loop;
		anim.Play (idleClip);

		anim [threatenClip].wrapMode = WrapMode.Once;
		anim [turnJumpClip].wrapMode = WrapMode.Once;
		anim [gotHitClip].wrapMode = WrapMode.Once;
		anim [gotHitClip].layer = 1;
	}

	void Start () {
		StartCoroutine (Idle ());
	}

	IEnumerator Idle () {
		if (audioManager.clip != idleSound) {
			PlaySoundClip (idleSound);
		}

		yield return new WaitForSeconds (idleTime);

		while (true) {
			charController.SimpleMove (Vector3.zero);

			yield return new WaitForSeconds (0.2f);

			Vector3 offset = transform.position - target.position;

			if (offset.magnitude < attackDistance) {
				StartCoroutine (Attack ());
			
				break;
			}
		}
	}

	IEnumerator Attack () {
		isAttacking = true;

		if (audioManager.clip != attackSound) {
			PlaySoundClip (attackSound);
		}

		anim.Play (attackRunClip);

		float angle = 180f, time = 0f;

		Vector3 direction;

		while (angle > 5 || time < attackTurnTime) {
			time += Time.deltaTime;

			angle = Mathf.Abs (RotateTowardsPosition(target.position, rotateSpeed));

			float move = Mathf.Clamp01 ((90f - angle) / 90f);

			anim [attackRunClip].weight = anim [attackRunClip].speed = move;

			direction = transform.TransformDirection (Vector3.forward * attackSpeed * move);

			charController.SimpleMove (direction);

			yield return null;
		}

		float timer = 0f;

		bool lostSight = false;

		while (time < extraRunTime) {
			angle = RotateTowardsPosition (target.position, attackRotateSpeed);

			if (Mathf.Abs (angle) > 40f) {
				lostSight = true;
			}

			if (lostSight) {
				timer += Time.deltaTime;
			}

			direction = transform.TransformDirection (Vector3.forward * attackSpeed);

			charController.SimpleMove (direction);

			Vector3 pos = transform.TransformPoint (punchPosition);

			if (Time.time > lastPunchTime + 0.3f && (pos - target.position).magnitude < punchRadius) {
				Vector3 slamDirection = transform.InverseTransformDirection (
					target.position - transform.position
				);

				slamDirection.y = 0f;
				slamDirection.z = 1f;

				if (slamDirection.x >= 0f) {
					slamDirection.x = 0f;
				} else {
					slamDirection.x = -1f;
				}

				target.SendMessage ("SlamInfo", transform.TransformDirection(slamDirection));
				target.SendMessage ("ApplyDamage", damage);

				lastPunchTime = Time.time;
			}

			if (charController.velocity.magnitude < attackSpeed * 0.3f) {
				StartCoroutine (Idle ());

				isAttacking = false;

				anim.CrossFade (idleClip);

				break;
			}

			yield return null;
		}

		isAttacking = false;

		anim.CrossFade (idleClip);
	}

	void PlaySoundClip (AudioClip sound) {
		audioManager.Stop ();
		audioManager.clip = sound;
		audioManager.loop = true;
		audioManager.Play ();
	}

	float RotateTowardsPosition(Vector3 targetPos, float rotateSpeed) {
		Vector3 relative = transform.InverseTransformPoint (targetPos);

		float angle = Mathf.Atan2 (relative.x, relative.z) * Mathf.Rad2Deg, maxRotation = rotateSpeed * Time.deltaTime,
		clampedAngle = Mathf.Clamp (angle, -maxRotation, maxRotation);

		transform.Rotate (0f, clampedAngle, 0f);

		return angle;
	}
}
