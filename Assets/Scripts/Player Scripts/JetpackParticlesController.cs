using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackParticlesController : MonoBehaviour {

	private PlayerController playerController;

	private AudioSource audioManager;

	private Component[] particles;

	private Light jetLight;

	private float litAmount = 0f;

	void Awake () {
		playerController = GetComponent<PlayerController> ();

		audioManager = GetComponent<AudioSource> ();

		particles = GetComponentsInChildren<ParticleEmitter> ();

		jetLight = GetComponentInChildren<Light> ();

		foreach (ParticleEmitter particle in particles) {
			particle.emit = false;
		}

		jetLight.enabled = false;
	}

	void Start () {
		StartCoroutine (StartEmission ());
	}

	IEnumerator StartEmission () {
		while (true) {
			if (playerController.IsJumping ()) {
				if (!audioManager.isPlaying) {
					audioManager.Play ();
				}
			} else {
				audioManager.Stop ();
			}

			foreach (ParticleEmitter particle in particles) {
				particle.emit = playerController.IsJumping ();
			}

			if (playerController.IsJumping ()) {
				litAmount = Mathf.Clamp01 (litAmount + Time.deltaTime * 2f);
			} else {
				litAmount = Mathf.Clamp01 (litAmount - Time.deltaTime * 2f);
			}

			jetLight.enabled = playerController.IsJumping ();
			jetLight.intensity = litAmount * 2f;

			yield return null;
		}
	}
}
