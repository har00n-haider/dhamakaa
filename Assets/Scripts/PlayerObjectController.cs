using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhamakaa.Enums;

public class PlayerObjectController : MonoBehaviour {

	public bool Fired { get; private set;}

	[SerializeField]
	private UIController uiController;

	[SerializeField]
	private GameObject cameraPivot;
	[SerializeField]
	private Material standardMaterial;
	[SerializeField]
	private Material explosionMaterial;

	#region Throw preview

	[SerializeField]
	private GameObject throwPointer;
	[SerializeField]
	private int previewArcLength;
	[SerializeField]
	private float previewArcResolution;

	[SerializeField]
	private bool previewActive;

	#endregion

	#region ExplosionParameters

	[SerializeField]
	private float explosionForce;
	[SerializeField]
	private float explosionRadius;
	[SerializeField]
	public float upwardsModifer;

	#endregion

	#region Members
	private Camera mainCamera;
	private bool touchStartedOnPO;
	private CameraController cameraController;
	private LineRenderer lineRenderer;
	private Vector3 velocityVector;
	private Renderer renderer;
	private Rigidbody rigidbody;
	private Quaternion fireDirection;
	private Vector3 radianAngles;
	private float gravity;
	private bool inFlight;
	private Vector3 launchLocation;
	private Gyroscope gyro;
	private float throwVelocity;
	private float lateralAcceleration ; 
		
	#endregion

	// Use this for initialization
	void Start () {

		//Starting values for the PO
		Fired  	       = false;
		inFlight 	   = false;
		renderer       = GetComponent<Renderer>();
		lineRenderer   = GetComponent<LineRenderer>();
		rigidbody      = GetComponent<Rigidbody>();
		velocityVector = new Vector3(0,0,0);
		mainCamera     = Camera.main;
		previewActive  = false;
		gravity 	   = -Physics.gravity.y;

		//Set the camera variables
		mainCamera 		   = Camera.main;
		touchStartedOnPO   = false;
		cameraController   = cameraPivot.GetComponent<CameraController>();

		gyro = Input.gyro;
		gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(inFlight){
			lineRenderer.enabled = false;
		}
	}

	void OnCollisionEnter (Collision col)
	{
		inFlight = false;
		Quaternion currentRotation = transform.rotation;
		ResetPO();
		transform.rotation = currentRotation;
		rigidbody.useGravity = false;
	}


	
	void FixedUpdate(){
		if(inFlight){
			Vector2 distanceCovered = new Vector2(transform.position.z - launchLocation.z, 
												  transform.position.x - launchLocation.x);

			//float forceFactor = Mathf.Sin(Mathf.Clamp(distanceCovered.magnitude/100, 0, Mathf.PI/4f));								  
			float forceFactor = 1f;								  

			//Debug.Log("force factor" + forceFactor);
			
			Vector3 curveVelocity =  new Vector3( forceFactor*lateralAcceleration*Mathf.Cos(radianAngles.y)*Time.fixedDeltaTime,
											      0f,
											     -forceFactor*lateralAcceleration*Mathf.Sin(radianAngles.y)*Time.fixedDeltaTime);	

			//Debug
			Vector3 throwDirection = new Vector3(throwVelocity*Mathf.Sin(radianAngles.y), 
		    									 0f,
												 throwVelocity*Mathf.Cos(radianAngles.y));
			// Debug.DrawRay(Vector3.zero, throwDirection.normalized*100, Color.blue,3f);
			// Debug.DrawRay(rigidbody.transform.position, curveVelocity.normalized*10, Color.red, 3f);

			rigidbody.velocity += curveVelocity;

			throwPointer.transform.rotation = Quaternion.LookRotation(rigidbody.velocity); 
		}
	}

	public void Juke(JukeDirection direction){
		if(direction == JukeDirection.Left){
			transform.Translate(transform.right*-10);
		}
		else if (direction == JukeDirection.Right){
			transform.Translate(transform.right*10);
		}
	}

	public void Explode(){
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null){
				rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardsModifer);
			}
		}
		renderer.material = standardMaterial;
	}

	public void Fire(){
		ResetPO();
		rigidbody.useGravity = true;
		launchLocation 		 = rigidbody.position;
		rigidbody.velocity   = velocityVector;
		renderer.material    = explosionMaterial;
		inFlight 			 = true;
		Fired 				 = true;	
	}

	
	public void ResetPO(){
		//Reset the motion of the PO to zero
		rigidbody.velocity 	      = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.useGravity 	  = false;
	}

	public void UpdateThrowPreview()
	{
		if(lineRenderer.enabled == false ){ lineRenderer.enabled = true;}

		throwVelocity 		= uiController.PoUiController.ThrowVelocity;
		lateralAcceleration = uiController.PoUiController.CurveAcceleration;
		fireDirection 		= uiController.PoUiController.AimDirection;

		//Update throw pointer
		throwPointer.transform.position = transform.position;
		throwPointer.transform.rotation = fireDirection;

		List<Vector3> points = new List<Vector3>();

		//Adapted with -ve sign on x axis due to unity axis handedness (angle = 360 - angle = -angle)
		radianAngles = new Vector3(-fireDirection.eulerAngles.x*Mathf.Deg2Rad,
								    fireDirection.eulerAngles.y*Mathf.Deg2Rad,
								    fireDirection.eulerAngles.z*Mathf.Deg2Rad);

		velocityVector = new Vector3(throwVelocity*Mathf.Cos(radianAngles.x)*Mathf.Sin(radianAngles.y),
					                 throwVelocity*Mathf.Sin(radianAngles.x),
					                 throwVelocity*Mathf.Cos(radianAngles.x)*Mathf.Cos(radianAngles.y));

		//Debug.DrawLine(new Vector3(0,0,0),velocityVector.normalized*3,Color.red,0.1f);

		//Iterate over points along the Sxz line
		for(float pointCount = 0 ; pointCount <= previewArcLength ; pointCount += previewArcResolution)
		{
			//points.Add(GetPreviewCoorinate(pointCount, radianAngles, velocityVector)); 
			points.Add(GetPreviewCoorinateWithLateralAcc(pointCount, radianAngles, velocityVector)); 
		}

		//Update the throw preview
		lineRenderer.positionCount = points.Count;
		lineRenderer.SetPositions(points.ToArray());
	}

	private Vector3 GetPreviewCoorinate(float sxzPoint, Vector3 radianAngles, Vector3 velocityVector){

		//Time at that point, assuming point is a point on the Sxz line
		float time = ((float)sxzPoint)/(throwVelocity*Mathf.Cos(radianAngles.x));

		//Sx, Sy, Sz at that time
		float xCoord = throwVelocity*Mathf.Cos(radianAngles.x)*Mathf.Sin(radianAngles.y)*time;
		float yCoord = throwVelocity*Mathf.Sin(radianAngles.x)*time - (gravity/2f)*(time*time);
		float zCoord = throwVelocity*Mathf.Cos(radianAngles.x)*Mathf.Cos(radianAngles.y)*time;
		
		return new Vector3(transform.position.x + xCoord,
			 			   transform.position.y + yCoord, 
						   transform.position.z + zCoord);

	}
	
	private Vector3 GetPreviewCoorinateWithLateralAcc(float sxzPoint, Vector3 radianAngles, Vector3 velocityVector){
		
		// //Attempt to solve for time with the quadratic formula
		// float a = LateralAcceleration*Mathf.Sin(radianAngles.y); 
		// float b = ThrowVelocity*Mathf.Cos(radianAngles.x)*Mathf.Cos(radianAngles.y); 
		// float c = -Mathf.Cos(radianAngles.y)*sxzPoint; 

		// //Under the root
		// float underRoot = b*b - 4f*a*c;

		// //Bail out if the underoot is negative
		// if(underRoot < 0) { return new Vector3();}

		// float numeratorFromAddition = -b + Mathf.Sqrt(underRoot);
		// //float numeratorFromSubtraction = -b - Mathf.Sqrt(underRoot);

		// //Debug.Log("underoot:" + underRoot + " sqrt: " +  Mathf.Sqrt(underRoot) + " b: " + b + "from add: " + numeratorFromAddition + " from sub: " + numeratorFromSubtraction);

		// //float chosenNumerator = numeratorFromAddition > 0 ? numeratorFromAddition : numeratorFromSubtraction;
		// float time = numeratorFromAddition/2f*a;

		
		float time = sxzPoint;

		//Sx, Sy, Sz at that time
		//Coordinate   Component of the initial velocity                                        Additional components (Gravity, curve force)
		float xCoord = throwVelocity*Mathf.Cos(radianAngles.x)*Mathf.Sin(radianAngles.y)*time + (lateralAcceleration/2f)*Mathf.Cos(radianAngles.y)*time*time;
		float yCoord = throwVelocity*Mathf.Sin(radianAngles.x)*time 						  - (gravity/2f)*(time*time);
		float zCoord = throwVelocity*Mathf.Cos(radianAngles.x)*Mathf.Cos(radianAngles.y)*time - (lateralAcceleration/2f)*Mathf.Sin(radianAngles.y)*time*time;
		
		return new Vector3(transform.position.x + xCoord,
						   transform.position.y + yCoord, 
						   transform.position.z + zCoord);
			

	}

	public bool touchStartedOnPOCheck(Touch currentTouch){

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(currentTouch.position.x, currentTouch.position.y,0));

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.gameObject;
			if(objectHit.name == "PlayerObject"){
				return true;
			}
			else{
				return false;
			}
        }
		else{
			return false;
		}
	}

}
