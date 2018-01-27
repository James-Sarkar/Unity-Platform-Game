using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;

	private Text lifeText, fuelText;

	[HideInInspector]
	public int fuelCount;

	void Awake () {
		MakeInstance ();

		lifeText = GameObject.Find ("Life Text").GetComponent<Text> ();

		fuelText = GameObject.Find ("Fuel Text").GetComponent<Text> ();
	}

	void Start () {
		fuelText.text = fuelCount.ToString ();
	}

	void OnDisable () {
		instance = null;
	}

	void MakeInstance () {
		if (instance == null) {
			instance = this;
		} 
	}

	public void SetLife (int life) {
		lifeText.text = life.ToString ();
	}

	public void FuelCollected () {
		fuelCount--;

		fuelText.text = (fuelCount < 0 ? 0 : fuelCount).ToString ();

		if (fuelCount <= 0) {
			// TODO: End the game
		}
	}
}
