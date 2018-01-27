using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpad : MonoBehaviour {

	public float jumpHeight = 5f;

	private AudioSource audioManager;

	void Awake() {
		audioManager = GetComponent<AudioSource> ();
	}

	void OnTriggerEnter (Collider target) {
		if (target.tag == "Player") {
			audioManager.Play ();

			target.gameObject.GetComponent<PlayerController> ().SuperJump (jumpHeight);
		}
	}
}
