using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

	public AudioClip sfxPlayerRespawn, sfxRespawnActivate, sfxRespawnActiveLoop;
	public float sfxVolume = 1f;

	private ParticleEmitter activeEmitter, inactiveEmitter, respawnEmitter0, respawnEmitter1, respawnEmitter2, respawnEmitter3;

	private Light respawnLight;

	private AudioSource audioManager;

	void Awake () {
		audioManager = GetComponent<AudioSource> ();

		activeEmitter = transform.Find ("RSParticlesActive").GetComponent<ParticleEmitter> ();

		inactiveEmitter = transform.Find ("RSParticlesInactive").GetComponent<ParticleEmitter> ();

		respawnEmitter0 = transform.Find ("RSParticlesRespawn1").GetComponent<ParticleEmitter> ();

		respawnEmitter1 = transform.Find ("RSParticlesRespawn1").GetComponent<ParticleEmitter> ();

		respawnEmitter2 = transform.Find ("RSParticlesRespawn2").GetComponent<ParticleEmitter> ();

		respawnEmitter3 = transform.Find ("RSParticlesRespawn3").GetComponent<ParticleEmitter> ();

		respawnLight = transform.Find ("RSSpotlight").GetComponent<Light> ();
	}

	void Start() {
		SetActive ();
	}

	void SetActive() {
		activeEmitter.emit = true;

		inactiveEmitter.emit = false;

		respawnLight.intensity = 1.5f;

		audioManager.clip = sfxRespawnActiveLoop;
		audioManager.loop = true;
		audioManager.Play ();
	}

	void SetInactive() {
		activeEmitter.emit = false;

		inactiveEmitter.emit = true;

		respawnLight.intensity = 1.5f;

		audioManager.Stop ();
	}

	public void ActivateFireFX() {
		StartCoroutine (FireEffect ());
	}

	IEnumerator FireEffect() {
		respawnEmitter0.Emit ();

		respawnEmitter1.Emit ();

		respawnEmitter2.Emit ();

		respawnEmitter3.Emit ();

		respawnLight.intensity = 3.5f;

		AudioSource.PlayClipAtPoint (sfxPlayerRespawn, transform.position, sfxVolume);

		yield return new WaitForSeconds (2f);

		respawnLight.intensity = 2f;
	}

	void OnTriggerEnter(Collider target) {
		if (target.tag == "Player") {
			AudioSource.PlayClipAtPoint (sfxRespawnActivate, transform.position, sfxVolume);

			SetActive ();
		}
	}
}
