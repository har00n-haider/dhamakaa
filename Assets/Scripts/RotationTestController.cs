using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTestController : MonoBehaviour {


	#region Properties

	public Vector3 offset;
	public Vector3 EulerAngles;
	public Vector3 CurrentPosition;
	private GameObject debugCube;
	private GameObject debugCubeCenter;
		
	#endregion


	// Use this for initialization
	void Start () {

		debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		debugCube.transform.position = new Vector3(10,1,0);

		debugCubeCenter = GameObject.CreatePrimitive(PrimitiveType.Cube);
		debugCubeCenter.transform.position = new Vector3(0,0,0);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		Quaternion lookRotation = Quaternion.LookRotation(offset);

		EulerAngles = lookRotation.eulerAngles;


		// Quaternion axisRotation1 = Quaternion.AngleAxis(EulerAngles.x, Vector3.up);
		// Quaternion axisRotation2 = Quaternion.AngleAxis(EulerAngles.y, Vector3.right);

		// debugCubeCenter.transform.rotation = axisRotation1 * axisRotation2;
 
 		Debug.DrawLine(Vector3.zero, offset, Color.red, 0.5f);

		//Debug.DrawRay(CurrentPosition, , Color.blue, 0.5f);
	}
}
