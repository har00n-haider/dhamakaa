using System.Collections;
using System.Collections.Generic;
using Dhamakaa.Enums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerObjectUIController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {


	#region Control properties

	public PlayerObjectController POController;
	public float ThrowVelocity { get; private set;}
	public float CurveAcceleration { get; private set;}
	public Quaternion AimDirection { get; private set;}

		
	#endregion

	public LinearControlStageElement VelControlElement;
	public LinearControlStageElement AimControlElement;
	public CurvedControlStageElement CurvedControlElement;

	[SerializeField]
	private ParticleController particleController;

	public float DefaultThrowVelocity;
	public float DefaultAimAngle;
	public float DefaultCurveAcceleration;
	

	private UIController uiController;
	private Color poLocationImageColor;
	private Image poLocationImage;
	private ControlStage currentStage;
	private Camera mainCamera;
	private Vector2 centerScreen;
	private System.Diagnostics.Stopwatch stopWatch;
	private bool velocityStageEntered;
	private bool aimEnabled;

	void Awake () {
		//Set internal references.
		uiController 		   = GetComponentInParent<UIController>();
		poLocationImage 	   = GetComponent<Image>();
		poLocationImageColor   = poLocationImage.color;
		mainCamera 			   = Camera.main;
		centerScreen 		   = new Vector2(Screen.width/2f, Screen.height/2); 

		//Calculate the canvas scaling factor required to manually adjust elements
		CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
		float widthScaleFactor    = canvasScaler.referenceResolution.x/Screen.width;
		float heightScaleFactor   = canvasScaler.referenceResolution.y/Screen.height;
		float actualScaleFactor   = Mathf.Lerp(widthScaleFactor, heightScaleFactor,canvasScaler.matchWidthOrHeight);

		VelControlElement.CanvasScalingFactor = actualScaleFactor;
		AimControlElement.CanvasScalingFactor = actualScaleFactor;
		VelControlElement.CenterScreen        = centerScreen;
		AimControlElement.CenterScreen        = centerScreen;
		CurvedControlElement.CurveControlElement = GetComponentInChildren<CurveBarController>();

		stopWatch = new System.Diagnostics.Stopwatch();

		ResetUI();
	}

	// Update is called once per frame
	void Update () {
		if(swipeCheckEnabled){
			SwipeCheck();
		}
	}

	private Touch startTouch;
	private float swipeDeltaX;
	private float touchDeltaX = 0f;
	[SerializeField]
	private float touchDeltaXThreshold = 20f;	
	[SerializeField]
	private float swipeDeltaXThreshold = 20f;
	private bool swipeLeft;
	private bool swipeRight;
	private bool swipeCheckEnabled;
	private Coroutine updateUIActiveCoroutine;
	private Coroutine updateEnableAimCoroutine;

	/// <summary>
	/// Function to check whether a swipe has occured
	/// </summary>
	private void SwipeCheck(){

		if(Input.touchCount > 0){

			Touch currentTouch = Input.GetTouch(0);

			switch(currentTouch.phase){
				case TouchPhase.Began:
					startTouch = currentTouch;
					break;
				case TouchPhase.Moved:
					//Figure out if thresholds for a swipe have been crossed
					touchDeltaX = Mathf.Abs(currentTouch.deltaPosition.x) > touchDeltaX ? 
								  Mathf.Abs(currentTouch.deltaPosition.x) : 
								  touchDeltaX;
				    float currentSwipeDeltaX = currentTouch.position.x - startTouch.position.x;
					swipeDeltaX =  Mathf.Abs(currentSwipeDeltaX) > swipeDeltaX ? 
								   Mathf.Abs(currentSwipeDeltaX) : 
								   swipeDeltaX;

					if(touchDeltaX > touchDeltaXThreshold && swipeDeltaX > swipeDeltaXThreshold){
						if(currentSwipeDeltaX > 0){
							swipeRight = true;
						}
						else{
							swipeLeft = true;
						}
					}
					break;
				case TouchPhase.Ended:

					break;
				default:
					break;
			}
		}
	}

	/// <summary>
	/// Post result of swipe check and reset values
	/// </summary>
	private void SwipeResult(){
		if(swipeLeft){
			Debug.Log("swiped left");
			POController.Juke(JukeDirection.Left);
		}
		if(swipeRight){
			Debug.Log("swiped right");
			POController.Juke(JukeDirection.Right);
		}

		startTouch  = new Touch();
		swipeDeltaX = 0f;
		touchDeltaX = 0f;
		swipeLeft   = false;
		swipeRight  = false;
	}

    public virtual void OnPointerDown(PointerEventData pointerED){
		if(updateUIActiveCoroutine != null){
			StopCoroutine(updateUIActiveCoroutine);
		}

		//Need to have a delay here to stop the camera script from updating to 
		//early and picking up false camera updates
		updateEnableAimCoroutine = StartCoroutine(UpdateAimFlagWithDelay(0.2f, true));

		ResetValues();
		//Show/enable the UI elements of the control
		poLocationImage.color   = poLocationImageColor;
		currentStage 			= ControlStage.VelocityControl;
		uiController.isUIActive = true;
		swipeCheckEnabled 	    = true;

		stopWatch.Start();
    }

    public virtual void OnPointerUp(PointerEventData pointerED){
		if(velocityStageEntered){
			POController.Fire();
		}

		if(updateEnableAimCoroutine != null){
			StopCoroutine(updateEnableAimCoroutine);
		}

		velocityStageEntered = false;
		swipeCheckEnabled    = false;
		aimEnabled 			 = false;

		//Need to have a delay here to stop the camera script from updating to 
		//early and picking up false camera updates
		updateUIActiveCoroutine = StartCoroutine(UpdateUIFlagWithDelay(0.2f, false));

		stopWatch.Stop();
		stopWatch.Reset();
		SwipeResult();
		ResetUI();
    }

	IEnumerator UpdateUIFlagWithDelay(float delay, bool flagStatus){
		yield return new WaitForSeconds(delay);
		uiController.isUIActive = flagStatus;		
	}

	IEnumerator UpdateAimFlagWithDelay(float delay, bool flagStatus){
		yield return new WaitForSeconds(delay);
		aimEnabled = flagStatus;		
	}


    public virtual void OnDrag(PointerEventData pointerED)
    {	
		//Only start aiming if a swipe has not been detected
		if(aimEnabled){
			UpdateAim(pointerED);
		}
	}

	void UpdateAim(PointerEventData pointerED){
		switch (currentStage){
			case ControlStage.VelocityControl:
				velocityStageEntered = true;
				VelControlElement.Enable();
				VelControlElement.Update(pointerED.position, currentStage);
				float screenNormalizedVelValue = (VelControlElement.Position.y - centerScreen.y)/Screen.height;
				ThrowVelocity = Mathf.Abs((screenNormalizedVelValue))*VelControlElement.Sensitivity;
				if(stopWatch.Elapsed.TotalSeconds > VelControlElement.TransitionTime){
					currentStage = ControlStage.AimControl;
					VelControlElement.Fade();
					particleController.ShowParticles(1, VelControlElement.Position);
					stopWatch.Reset();
				}
				break;
			case ControlStage.AimControl:
				AimControlElement.Enable();
				AimControlElement.Update(pointerED.position, currentStage);
				AimDirection = GetAimDirection(pointerED.position);
				if(stopWatch.Elapsed.TotalSeconds > AimControlElement.TransitionTime){
					currentStage = ControlStage.CurveControl;
					AimControlElement.Fade();
					particleController.ShowParticles(1, AimControlElement.Position);
					stopWatch.Reset();
				}
				break;			
			case ControlStage.CurveControl:
				CurvedControlElement.Enable();
				float screenNormalizedCurveValue = (pointerED.position.x - Screen.width/2f)/Screen.width;
				CurveAcceleration = CurvedControlElement.CurrentValue 
								= screenNormalizedCurveValue*CurvedControlElement.Sensitivity;
				break;
			default:
				break;
		}

		if(velocityStageEntered){
			POController.UpdateThrowPreview();
		}
	}

	void ResetUI(){
		poLocationImage.color = Color.clear;
		VelControlElement.Disable();
		AimControlElement.Disable();
		CurvedControlElement.Disable();
		currentStage = ControlStage.None;
	}

	void ResetValues(){
		//Reset control values to default
		AimDirection 	  = mainCamera.transform.rotation * Quaternion.AngleAxis(-DefaultAimAngle,Vector3.right);
		ThrowVelocity 	  = DefaultThrowVelocity;
		CurveAcceleration = DefaultCurveAcceleration;
	}
	

	/// <summary>
	/// Convert a screen touch point to a rotation that can be applied to the throwPointer.
	/// </summary>
	/// <param name="currentTouchPos"></param>
	/// <returns></returns>
	private Quaternion GetAimDirection(Vector2 pointerPosition){

		//Convert the X/Y coordinate to angles around two orthogonal axes, taking into account current camera orientation 
		float currentCameraZAngle 	   = mainCamera.transform.rotation.eulerAngles.y;
		float angleAroundUp 		   = (90f/(Screen.width/2f)) * (Screen.width - pointerPosition.x) - 90f;
		float angleAroundRight 		   = (90f/(Screen.height/2f)) * pointerPosition.y - 90f;
		Quaternion rotationAroundUp    = Quaternion.AngleAxis(angleAroundUp + currentCameraZAngle, Vector3.up);
		Quaternion rotationAroundRight = Quaternion.AngleAxis(angleAroundRight, Vector3.right);
		Quaternion calcRotation 	   = rotationAroundUp*rotationAroundRight;

		return Quaternion.Lerp(AimDirection, calcRotation, AimControlElement.Sensitivity);
	}


}

public enum ControlStage{
	VelocityControl,
	AimControl,
	CurveControl,
	None,
}


/// <summary>
/// Class to the house the functionality and properties of a linear handle on the UI
/// </summary>
[System.Serializable]
public class LinearControlStageElement{

	public Vector2 Position { get { return position;} }

	public float Sensitivity;
	public float TransitionTime;
	public Image EndPointImage; 
	public Image LineImage; 
	public float CanvasScalingFactor { get; set; }

	public Vector2 CenterScreen { get; set; }

	private float connectorWidth = 5f;
	private Vector2 position;


	public void Enable(){
		EndPointImage.enabled = true;
		LineImage.enabled     = true;
		EndPointImage.color = new Color(EndPointImage.color.r,
									 	EndPointImage.color.g,
									 	EndPointImage.color.b,
									 	1f);
		LineImage.color = new Color(LineImage.color.r,
									LineImage.color.g,
									LineImage.color.b,
									1f);
	}

	public void Disable(){
		//Reset positions
		EndPointImage.transform.position   = CenterScreen;
		LineImage.rectTransform.sizeDelta = Vector2.zero;

		//Disable GUI elements 
		EndPointImage.enabled = false;
		LineImage.enabled 	  = false;
	}

	public void Update(Vector2 pointerPosition, ControlStage currentStage){
		//Special considerations for each case
		switch (currentStage){
			case ControlStage.VelocityControl:
				pointerPosition = new Vector2(CenterScreen.x, pointerPosition.y);
				break;
			case ControlStage.AimControl:
				break;
			default:
				break;
		}

		position						   = pointerPosition;
		EndPointImage.transform.position   = pointerPosition;
		//Position and rotate the connecting line
		Vector3 differenceVector    	   = pointerPosition - CenterScreen;
		float angle 			    	   = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
		LineImage.rectTransform.sizeDelta  = new Vector2(differenceVector.magnitude*CanvasScalingFactor, connectorWidth);
		LineImage.rectTransform.rotation   = Quaternion.Euler(0,0, angle);
	}

	public void Fade(){
		EndPointImage.color = new Color(EndPointImage.color.r,
										EndPointImage.color.g,
										EndPointImage.color.b,
										0.2f);
		LineImage.color = new Color(LineImage.color.r,
									LineImage.color.g,
									LineImage.color.b,
									0.2f);
	}

}

/// <summary>
/// Class to the house the functionality and properties of a curved handle on the UI
/// </summary>
[System.Serializable]
public class CurvedControlStageElement{

	public float CurrentValue { get { return currentValue;} set { currentValue = CurveControlElement.CurveValue = value;}}
	private float currentValue;

	public float Sensitivity;
	public Image EndPointImage; 
	public Image LineImage; 
	public Image LineBackImage; 
	public CurveBarController CurveControlElement;

	public Vector2 CenterScreen { get; set; }

	private float connectorWidth = 5f;

	public void Enable(){
		EndPointImage.enabled = true;
		LineImage.enabled 	  = true;
		LineBackImage.enabled = true;
	}

	public void Disable(){
		EndPointImage.enabled = false;
		LineImage.enabled 	  = false;
		LineBackImage.enabled = false;
	}


}