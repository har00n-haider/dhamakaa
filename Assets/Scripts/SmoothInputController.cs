using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType{
	absolutePos,
	deltaPos,
	force
}

public class SmoothInputController : MonoBehaviour {

	public GameObject Cube;

	public float Sensitivity;

	public MovementType Switch;


	private Vector3 cubeVel;
	private Rigidbody cubeRb;

	// Use this for initialization
	void Awake () {

		cubeRb = Cube.GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown("space")){
			//MoveCube();
		};
		MoveCube();
		
	}

	void FixedUpdate(){
		

	}

	private void MoveCube(){
		
		// float moveHorizontal = Input.GetAxis ("Horizontal");
		// float moveVertical = Input.GetAxis ("Vertical");
         
		// //Movement
		// Cube.transform.position += (Vector3.right * moveHorizontal * speed * Time.deltaTime);
		// Cube.transform.position += (Vector3.forward * moveVertical * speed * Time.deltaTime);

		// Vector3 goal = new Vector3(3f, 0.5f, 0f);

		// Cube.transform.position = Vector3.Lerp(Cube.transform.position, goal, speed);
		// Debug.Log(Cube.transform.position);

		if(Input.touchCount > 0){

			Touch currentTouch = Input.GetTouch(0);
			Vector2 touchDelta = currentTouch.deltaPosition;


			if(Switch == MovementType.absolutePos){
				//Target pos from absolute position of touch
				float x = (currentTouch.position.x - Screen.width/2f)/10f; 
				float y = (currentTouch.position.y - Screen.height/2f)/10f;
				Vector3 targetPos = new Vector3(x,0.5f,y);
				Cube.transform.position = Vector3.Lerp(Cube.transform.position, targetPos, Sensitivity);
			}
			else if (Switch == MovementType.deltaPos){
				//Target pos from change in pos
				float x = (currentTouch.deltaPosition.x); 
				float y = (currentTouch.deltaPosition.y);
				Vector3 targetPos = Cube.transform.position + new Vector3(x,0f,y);
				Cube.transform.position = Vector3.Lerp(Cube.transform.position, targetPos, Sensitivity);

			}
			else if (Switch == MovementType.force){
				//Target pos from change in pos
				float x = (currentTouch.deltaPosition.x); 
				float y = (currentTouch.deltaPosition.y);
				Vector3 direction = new Vector3(x,0.5f,y);

				cubeRb.AddForce(direction);

			}
		
		}
		else{
			cubeRb.velocity = Vector3.zero;
			cubeRb.angularVelocity = Vector3.zero;
		}
		
	}
}
