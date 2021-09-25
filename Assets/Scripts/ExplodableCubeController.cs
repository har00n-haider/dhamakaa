using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodableCubeController : MonoBehaviour {

	[SerializeField]
	private GameObject explodedCubePrefab;
	[SerializeField]
	private float explosionForce;
	[SerializeField]
	private float explosionRadius;
	[SerializeField]
	private float upwardsModifer;
	private GameObject explodedCubeInstance;
	

	void OnCollisionEnter(Collision collision){
		if(collision.contacts.Length > 0){
			ContactPoint contact = collision.contacts[0];

			if(contact.otherCollider.name == "Blade"){

				Debug.Log("Blade smack");
            	Debug.DrawRay(contact.point, contact.normal*10, Color.red);

				explodedCubeInstance = Instantiate(explodedCubePrefab, transform.position, transform.rotation);
			    Destroy(gameObject);

				Vector3 explosionPos = contact.point;
				Collider[] colliders = explodedCubeInstance.GetComponentsInChildren<Collider>();
				foreach (Collider hit in colliders)
				{
					Rigidbody rb = hit.GetComponent<Rigidbody>();
					if (rb != null){
						rb.AddForce(contact.normal*explosionForce, ForceMode.Impulse);
					}
				}
			}
		}
	}
}
