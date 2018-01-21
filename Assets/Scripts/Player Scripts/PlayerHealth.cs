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

		} else {
			SendMessage ("Slam", transform.TransformDirection (slamDir));
		}
	}

	void SlamInfo(Vector3 direction) {
		slamDir = direction;
	}
}
