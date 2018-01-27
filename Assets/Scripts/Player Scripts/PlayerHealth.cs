using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	public int health = 3, lives = 3, maxHealth = 3;

	public AudioClip struckSound, deathSound, pickUpHealth, pickUpFuel;

	private Vector3 slamDir;

	private bool fellDown;

	void Start () {
		GameplayController.instance.SetLife (health);
	}

	// Update is called once per frame
	void Update () {
		if (transform.position.y <= -30f) {
			if (!fellDown) {
				fellDown = true;

				health--;

				StartCoroutine (Die ());
			}
		}
	}

	void ApplyDamage (int damage) {
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

			string respawnPosName = "";

			Vector3 respawnPos = ClosestRespawnPoint (ref respawnPosName);

			Camera.main.transform.position = respawnPos - (transform.forward * 4) + Vector3.up;

			SendMessage ("HidePlayer");

			transform.position = respawnPos;

			yield return new WaitForSeconds (1.6f);

			SendMessage ("ShowPlayer");

			GameObject.Find (respawnPosName).GetComponent<Respawn> ().ActivateFireFX ();
		}
	}

	Vector3 ClosestRespawnPoint (ref string name) {
		Vector3 respawn0Pos = GameObject.Find ("Respawn0").transform.position,
		respawn1Pos = GameObject.Find ("Respawn1").transform.position,
		respawn2Pos = GameObject.Find ("Respawn2").transform.position,
		respawn3Pos = GameObject.Find ("Respawn3").transform.position,
		respawnPos = Vector3.zero;

		float respawn0Distance = Vector3.Distance (transform.position, respawn0Pos),
		respawn1Distance = Vector3.Distance (transform.position, respawn1Pos),
		respawn2Distance = Vector3.Distance (transform.position, respawn2Pos),
		respawn3Distance = Vector3.Distance (transform.position, respawn3Pos),
		minRespawnDistance = Mathf.Min (Mathf.Min (respawn0Distance, respawn1Distance), 
			Mathf.Min (respawn2Distance, respawn3Distance));

		if (Mathf.Approximately(minRespawnDistance, respawn0Distance)) {
			name = "Respawn0";

			return respawn0Pos;
		} else if (Mathf.Approximately(minRespawnDistance, respawn1Distance)) {
			name = "Respawn1";

			return respawn1Pos;
		} else if (Mathf.Approximately(minRespawnDistance, respawn2Distance)) {
			name = "Respawn2";

			return respawn2Pos;
		} else if (Mathf.Approximately(minRespawnDistance, respawn3Distance)) {
			name = "Respawn3";

			return respawn3Pos;
		}

		return respawn0Pos;
	}

	void SlamInfo (Vector3 direction) {
		slamDir = direction;
	}

	void OnTriggerEnter (Collider target) {
		if (target.tag == "Health") {
			Destroy (target.gameObject);

			health++;

			AudioSource.PlayClipAtPoint (pickUpHealth, transform.position);

			GameplayController.instance.SetLife (health);
		}

		if (target.tag == "Fuel") {
			Destroy (target.gameObject);

			GameplayController.instance.FuelCollected ();

			AudioSource.PlayClipAtPoint (pickUpFuel, transform.position);
		}
	}
}
