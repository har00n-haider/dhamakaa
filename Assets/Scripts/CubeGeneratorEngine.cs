using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGeneratorEngine : MonoBehaviour {

	[SerializeField]
	private GameObject targetCubePrefab;
	[SerializeField]
	private int cubesPerHorEdge = 30;	
	[SerializeField]
	private int cubesPerVertEdge = 30;

	private Vector3 cubeScale;
	private Vector3 cubeSize;


	// Use this for initialization
	void Start () {


		//Could also do:
		//cubeSize = targetCubePrefab.GetComponent<Renderer>().bounds.size;
		cubeSize = targetCubePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size;


		CreateWallAtPosition(new Vector3(gameObject.transform.position.x,
										 gameObject.transform.position.y + cubeScale.y/2, 
										 gameObject.transform.position.z));
	}

	

		


	// Update is called once per frame
	void Update () {
		
	}

	void CreateWallAtPosition(Vector3 position){

		int xLimit = cubesPerHorEdge;
		int yLimit = cubesPerVertEdge;

		float verticalOffset = cubeSize.y/2f;

		for(int counterX = 0; counterX < xLimit ; counterX++){
			for(int counterY = 0; counterY < yLimit ; counterY++){

				GameObject targetCube = Instantiate(targetCubePrefab, 
													new Vector3(position.x + counterX*cubeSize.x, 
																position.y + counterY*cubeSize.y + verticalOffset, 
																position.z),
													new Quaternion());

				targetCube.transform.SetParent(transform);
			}
		}
	}
}