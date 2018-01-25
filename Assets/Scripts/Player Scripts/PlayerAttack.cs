using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	public float punchSpeed = 1f, punchHitTime = 0.15f, punchTime = 0.25f, punchRadius = 1.3f, damage = 1f;

	public Vector3 punchPosition = new Vector3 (0f, 0f, 0.8f);

	public AudioClip punchSound;

	private AudioSource audioManager;

	private Animation anim;

	private PlayerController playerController;

	private bool busy;

	private string punchClip = "punch";

	void Awake () {
		audioManager = GetComponent<AudioSource> ();

		anim = GetComponent<Animation> ();

		playerController = GetComponent<PlayerController> ();
	}

	void Start() {
		anim [punchClip].speed = punchSpeed;
	}

	void Update () {
		if (!busy && Input.GetButton ("Fire1") && playerController.IsGroundedWithTimer ()
			&& !playerController.IsMoving ()) {
			StartCoroutine (DidPunch ());

			busy = true;
		}
	}

	IEnumerator DidPunch() {
		anim.CrossFadeQueued (punchClip, 0.1f, QueueMode.PlayNow);

		yield return new WaitForSeconds (punchHitTime);

		Vector3 pos = transform.TransformPoint (punchPosition);

		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");

		foreach (GameObject go in enemies) {
			if (Vector3.Distance (go.transform.position, pos) < punchRadius) {
				go.SendMessage ("ApplyDamage", damage);

				AudioSource.PlayClipAtPoint (punchSound, transform.position);
			}
		}

		yield return new WaitForSeconds (punchTime - punchHitTime);

		busy = false;
	}
}
