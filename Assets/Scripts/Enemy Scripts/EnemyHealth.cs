using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	public int health = 3;

	public GameObject explosionPrefab, deadModelPrefab, healthCollectable;

	void ApplyDamage (int damage) {
		health -= damage;

		if (health <= 0) {
			Die ();
		}
	}

	void Die() {
		GameObject deadModel = Instantiate (deadModelPrefab, transform.position, Quaternion.identity),
		explosionFX = Instantiate (explosionPrefab, transform.position, Quaternion.identity);

		Vector3 temp = transform.position;
		temp.x += 2f;
		temp.y += 1f;

		GameObject obj = Instantiate (healthCollectable, temp, transform.rotation);

		Rigidbody deadModelBody = deadModel.GetComponent<Rigidbody> ();

		Vector3 relativePlayerPosition = transform.InverseTransformPoint (Camera.main.transform.position);

		deadModelBody.AddTorque (Vector3.up * 7f);

		if (relativePlayerPosition.z > 0f) {
			deadModelBody.AddForceAtPosition (-transform.forward * 2f, transform.position +
				(transform.up * 5f), ForceMode.Impulse);
		} else {
			deadModelBody.AddForceAtPosition (transform.forward * 2f, transform.position +
				(transform.up * 2f), ForceMode.Impulse);
		}

		Destroy (gameObject);
	}
}
