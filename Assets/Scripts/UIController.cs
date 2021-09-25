using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	public bool isUIActive { get; set; }

	public PlayerObjectUIController PoUiController { get { return poUiController;} }
	private PlayerObjectUIController poUiController;

	public BladeUIController BladeUIController { get { return bladeUIController;} }
	private BladeUIController bladeUIController;



	// Use this for initialization
	void Awake () {

		poUiController 	  = GetComponentInChildren<PlayerObjectUIController>();
		bladeUIController = GetComponentInChildren<BladeUIController>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
