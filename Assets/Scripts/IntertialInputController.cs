using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntertialInputController : MonoBehaviour {

	public GameObject testRect;
	private Gyroscope gyro;

	// Use this for initialization
	void Start () {
		gyro = Input.gyro;
		gyro.enabled = true;

	}
	
	// Update is called once per frame
	void Update () {
	}
}
