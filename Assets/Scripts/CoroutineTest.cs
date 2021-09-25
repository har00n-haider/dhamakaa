using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Coroutine routine = StartCoroutine(Move(500));


	}
	
	// Update is called once per frame
	void Update () {
		MoveCheck();
		
	}

	IEnumerator Timer(float seconds){
		yield return new WaitForSeconds(seconds);
		Debug.Log(seconds + " passed");

	}

	IEnumerator Countdown (int seconds) {
		int counter = seconds;
		while (counter > 0) {
			yield return new WaitForSeconds (1f);
			Debug.Log("1 second passed");
			counter--;
		}
 	}

	IEnumerator Move(int moveAmount){
		while (moveAmount > 0) {



			yield return new WaitForSeconds (0.1f);
			moveAmount--;
		}
	}

	private void MoveCheck(){

		RaycastHit lineCast;

		Vector3 lineCastEnd = new Vector3(transform.position.x,
										  transform.position.y,
										  transform.position.z + 5);

										  

		//Check if there is an obstruction
		bool didHit = Physics.Linecast(transform.position, 
									   lineCastEnd, 
									   out lineCast);

		Debug.DrawLine(transform.position, lineCastEnd, Color.red);

		if(didHit)
		{
			Debug.Log(lineCast.transform.gameObject.tag);

			if(lineCast.transform.gameObject.tag != "Level"){

				transform.Translate(new Vector3(0f,0f,0.1f));
			}
		}
		else{
				transform.Translate(new Vector3(0f,0f,0.1f));

		}
	}
}