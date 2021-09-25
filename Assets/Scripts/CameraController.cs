using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private UIController uiController;

	#region CameraControl
 
	public GameObject ObjectInFocus;
	public float OrbitSensitivity;
	public float OrbitDampening;
	public float CameraDecelerationRate;
	public float ElevationMask;
	public bool CameraPivotDisabled = false;
	
	#endregion

	public Material DefaultMaterial;
	public Material TransparentMaterial;

	private Transform cameraTransform;
	private Vector2 localRotation;
	private Camera attachedCamera;
	private GameObject alteredGameObject;
	private Vector2 inputDelta;
	private float decelerationFactor;




	// Use this for initialization
	void Awake () {
        cameraTransform   = Camera.main.transform;
		attachedCamera    = Camera.main;
		inputDelta	      = Vector2.zero;
		decelerationFactor = 1f;

	}

	void Update(){
	}

	
	/// <summary>
	/// Function called once a frame is rendered, camera manipulation should generally occur here.
	/// </summary>
	void LateUpdate(){
		//Keep the camera on the player object
		transform.position = ObjectInFocus.transform.position;

		if(!uiController.isUIActive){
			UpdateCamera();
		}
	}
	
	public void UpdateCamera(){

		//Bail out if camera is disabled
		if(CameraPivotDisabled){ return;}

		if(Input.touchCount > 0){
			Touch currentTouch = Input.GetTouch(0);
			Vector2 abstractedDeltaPos = new Vector2(currentTouch.deltaPosition.x/Screen.width,
													 currentTouch.deltaPosition.y/Screen.height);
			inputDelta = abstractedDeltaPos;
			decelerationFactor = 1f;
		}

		//Apply calculate the deceleration factor
		decelerationFactor = (decelerationFactor - CameraDecelerationRate) < 0 ? 0 : decelerationFactor - CameraDecelerationRate;
		if(decelerationFactor == 0){ inputDelta = Vector2.zero; }

		//Apply input delta to local rotation
		if (inputDelta.x != 0 || inputDelta.y != 0)
		{
			localRotation.x += inputDelta.x * OrbitSensitivity * decelerationFactor;
			localRotation.y -= inputDelta.y * OrbitSensitivity * decelerationFactor;	

			if (localRotation.y < ElevationMask){
				localRotation.y = ElevationMask;		
			}
			// Horizon limit on the camera
			else if (localRotation.y > 90f){
				localRotation.y = 90f;		
			}                    
		}
		
		//Apply rotations
		Quaternion QT 	   = Quaternion.Euler(localRotation.y, localRotation.x, 0);
		transform.rotation = Quaternion.Lerp(transform.rotation, QT, Time.unscaledDeltaTime * OrbitDampening);

	}

	private void GetZoomFromTouch(){
	    //public float ScrollDampening = 6f;
	    //public float ScrollSensitvity = 2f;
	    //protected float cameraDistance = 10f;

		// //Zooming Input from our Mouse Scroll Wheel
		// if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		// {
		//  float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

		//  ScrollAmount *= (cameraDistance * 0.3f);

		//  cameraDistance += ScrollAmount * -1f;

		//  cameraDistance = Mathf.Clamp(cameraDistance, 1.5f, 100f);
		// }
		 
		// if ( cameraTransform.localPosition.z != cameraDistance * -1f )
		// {
		// 	cameraTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(cameraTransform.localPosition.z, cameraDistance * -1f, Time.deltaTime * ScrollDampening));
		// }
	}

}
