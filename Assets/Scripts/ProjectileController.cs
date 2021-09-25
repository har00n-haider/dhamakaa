using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


		
	}

	void OnCollisionEnter(Collision c){

		if(c.gameObject.tag != "Level" &&
		   c.gameObject.name != "Enemy" &&
		   c.gameObject.name != "PlayerObject" ){

			Destroy(c.gameObject);
			Destroy(gameObject);

		}


	}
}
