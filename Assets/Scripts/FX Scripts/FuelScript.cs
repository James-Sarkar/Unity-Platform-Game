using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelScript : MonoBehaviour {

	void Start () {
		GameplayController.instance.fuelCount++;
	}
}
