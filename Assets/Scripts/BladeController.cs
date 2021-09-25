using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider){
		switch(collider.tag){
			case "Level":
				break;
			case "Player":
				break;
			default:
				//Destroy(collider.gameObject);
				break;
		}
	}

}
