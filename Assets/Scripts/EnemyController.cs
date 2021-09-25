using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	[SerializeField]
	private GameObject playerObject;
	[SerializeField]
	private float approachSpeed;
	[SerializeField]
	private GameObject projectilePrefab;
	[SerializeField]
	private float projectileForce;
	[SerializeField]
	private int fireRate;
	private float timer;

	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(playerObject.transform.position);

		transform.position = Vector3.Lerp(transform.position, 
										  playerObject.transform.position,
										  approachSpeed*Time.deltaTime);

		if(timer > fireRate){
			
			Fire();

			timer = 0;

		}

		timer += Time.deltaTime;
		
	}


	void Fire(){

		Vector3 fireLoc = transform.TransformPoint(Vector3.forward * 1);

		GameObject proj = Instantiate(projectilePrefab, 
									  fireLoc, 
									  transform.localRotation);

		proj.GetComponent<Rigidbody>().velocity = (transform.rotation*Vector3.forward)*projectileForce;


	}
}
