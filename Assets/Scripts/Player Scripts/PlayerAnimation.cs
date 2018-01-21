using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	private Animation anim;

	private float runSpeedScale = 1f, walkSpeedScale = 1f;

	private string runClip = "run", walkClip = "walk", idleClip = "idle", ledgefallClip = "ledgefall", jumpClip = "jump",
	jumpFallClip = "jumpfall", jumpLandClip = "jumpland", jetpackJumpClip = "jetpackjump", wallJumpClip = "walljump",
	buttstompClip = "buttstomp", punchClip = "punch";

	private PlayerController playerController;

	void Awake () {
		anim = GetComponent<Animation> ();

		playerController = GetComponent<PlayerController> ();
	}

	void Start() {
		anim.wrapMode = WrapMode.Loop;

		anim [runClip].layer = -1;
		anim [walkClip].layer = -1;
		anim [idleClip].layer = -2;

		anim.SyncLayer (-1);

		anim [ledgefallClip].layer = 9;
		anim [ledgefallClip].wrapMode = WrapMode.Loop;

		anim [jumpClip].layer = 10;
		anim [jumpClip].wrapMode = WrapMode.ClampForever;

		anim [jumpFallClip].layer = 10;
		anim [jumpFallClip].wrapMode = WrapMode.ClampForever;

		anim [jetpackJumpClip].layer = 10;
		anim [jetpackJumpClip].wrapMode = WrapMode.ClampForever;

		anim [jumpLandClip].layer = 10;
		anim [jumpLandClip].wrapMode = WrapMode.Once;

		anim [buttstompClip].layer = 20;
		anim [buttstompClip].speed = 0.15f;
		anim [buttstompClip].wrapMode = WrapMode.Once;

		anim [punchClip].wrapMode = WrapMode.Once;

		anim.Stop ();
		anim.Play (idleClip);
	}

	// Update is called once per frame
	void Update () {
		if (playerController.GetSpeed () > playerController.walkSpeed) {
			anim.CrossFade (runClip);
			anim.Blend (jetpackJumpClip, 0f);
		} else if (playerController.GetSpeed () > 0.1f) {
			anim.CrossFade (walkClip);
			anim.Blend (jumpLandClip, 0f);
		} else {
			anim.Blend (walkClip, 0f, 0.3f);
			anim.Blend (runClip, 0f, 0.3f);
			anim.Blend (runClip, 0f, 0.3f);
		}

		anim [runClip].normalizedSpeed = runSpeedScale;
		anim [walkClip].normalizedSpeed = walkSpeedScale;

		if (playerController.IsJumping ()) {
			if (playerController.IsControlledDescent ()) {
				anim.CrossFade (jetpackJumpClip, 0.2f);
			} else if (playerController.HasJumpReachedApex ()) {
				anim.CrossFade (jumpFallClip, 0.2f);
			} else {
				anim.CrossFade (jumpClip, 0.2f);
			}
		} else if (!playerController.IsGroundedWithTimer ()) {
			anim.CrossFade (ledgefallClip, 0.2f);
		} else {
			anim.Blend (ledgefallClip, 0f, 0.2f);
		}
	}

	void DidLand() {
		anim.Play (jetpackJumpClip);
	}

	void DidButtStomp() {
		anim.CrossFade (buttstompClip, 0.1f);
		anim.CrossFadeQueued (jumpLandClip, 0.2f);
	}
}
