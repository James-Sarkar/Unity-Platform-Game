using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	public int health = 3, lives = 3, maxHealth = 3;

	public AudioClip struckSound, deathSound, pickUpHealth, pickUpFuel;

	private Vector3 slamDir;

	private bool fellDown;

	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (transform.position.y <= -30f) {
			if (!fellDown) {
				fellDown = true;

				health--;
			}
		}
	}

	void ApplyDamage(int damage) {
		AudioSource.PlayClipAtPoint (struckSound, transform.position);

		health -= damage;

		if (health <= 0) {
			StartCoroutine (Die ());
		} else {
			SendMessage ("Slam", transform.TransformDirection (slamDir));
		}
	}

	IEnumerator Die () {
		AudioSource.PlayClipAtPoint (deathSound, transform.position);

		lives--;

		if (lives < 0) {
			// TODO: End the game
		} else {
			fellDown = false;

			health = maxHealth;

			Vector3 respawnPos = GameObject.Find ("Respawn0").transform.position;

			Camera.main.transform.position = respawnPos - (transform.forward * 4) + Vector3.up;

			SendMessage ("HidePlayer");

			transform.position = respawnPos;

			yield return new WaitForSeconds (1.6f);

			SendMessage ("ShowPlayer");
		}
	}

	void SlamInfo(Vector3 direction) {
		slamDir = direction;
	}
}
