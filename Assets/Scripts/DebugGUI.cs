using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DebugGUI : MonoBehaviour {

	#region Properties
	public GUIStyle DebugGUIstyle; 
	public bool TouchInfo;
	public bool MouseInfo;
	public bool TimeInfo;
	public bool InertialInfo;
	public bool UIInfo;
	public Vector2 DebugConsoleOffset;
	public UIController uiController;



	#endregion

	#region Members

	private Vector2 mouseGetAxisMax = new Vector2(0,0);
	private Vector2 mouseGetAxisMin = new Vector2(0,0);	
	private Vector2 touchDeltaMax = new Vector2(0,0);
	private Vector2 touchDeltaMin = new Vector2(0,0);
	private DateTime storedTime;
	private int timesUpdateWasCalled;
	private float averageUpdateCallRate = -1;
	private int timesFixedUpdateWasCalled;
	private float averageFixedUpdateCallRate = -1;
	private int secondsOfFunctionCallHistory = 5;
	private Camera mainCamera;
	private Gyroscope gyro;
	private Vector2 debugConsoleDimensions;
	private Touch currentTouch;
	
		
	#endregion

	void Start()
	{
		storedTime		   		  = DateTime.Now;
		timesUpdateWasCalled 	  = 0;
		timesFixedUpdateWasCalled = 0;	
		mainCamera 				  = Camera.main;

		DebugGUIstyle.fontSize = (int)Screen.height/30;
		 
		debugConsoleDimensions.y = Screen.height;
		debugConsoleDimensions.x = Screen.width;

		gyro 					  = Input.gyro;
		gyro.enabled 		      = true;
	}


	void Update(){
		if(Input.touchCount > 0){
			currentTouch = Input.GetTouch(0);
		}
	}

	public void UpdateGUI()
	{
        Event currentEvent = Event.current;

        GUILayout.BeginArea(new Rect(DebugConsoleOffset.y,
		 							 DebugConsoleOffset.x, 	
								     debugConsoleDimensions.y, 
									 debugConsoleDimensions.x), 
									 "");

		if(MouseInfo){
			//Mouse GetAxis stats
			Vector2 currentAxis = new Vector2( Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			mouseGetAxisMax.x = mouseGetAxisMax.x > Input.GetAxis("Mouse X") ? mouseGetAxisMax.x : Input.GetAxis("Mouse X");
			mouseGetAxisMax.y = mouseGetAxisMax.y > Input.GetAxis("Mouse Y") ? mouseGetAxisMax.y : Input.GetAxis("Mouse Y");
			mouseGetAxisMin.x = mouseGetAxisMin.x < Input.GetAxis("Mouse X") ? mouseGetAxisMin.x : Input.GetAxis("Mouse X");
			mouseGetAxisMin.y = mouseGetAxisMin.y < Input.GetAxis("Mouse Y") ? mouseGetAxisMin.y : Input.GetAxis("Mouse Y");
			GUILayout.Label("Get axis mouse x: " + Input.GetAxis("Mouse X")  + " " + Input.GetAxis("Mouse Y"), DebugGUIstyle);
			GUILayout.Label("Get axis range for mouse x: " + mouseGetAxisMin.x  + " " + mouseGetAxisMax.x, DebugGUIstyle);
			GUILayout.Label("Get axis range for mouse y: " + mouseGetAxisMin.y  + " " + mouseGetAxisMax.y, DebugGUIstyle);
			GUILayout.Label("Mouse pos from event: " + currentEvent.mousePosition.x + " , " + currentEvent.mousePosition.y, DebugGUIstyle);
			GUILayout.Label("Mouse pos Input: " + new Vector2(Input.mousePosition.x, Input.mousePosition.y), DebugGUIstyle);
		}

		//Touch input
		if(TouchInfo){

			GUILayout.Label("Delta touch: " + currentTouch.deltaPosition, DebugGUIstyle);
			GUILayout.Label("Delta time: " + currentTouch.deltaTime, DebugGUIstyle);
			Vector2 currentTouchDelta = currentTouch.deltaPosition;
			touchDeltaMax.x = touchDeltaMax.x > currentTouchDelta.x ? touchDeltaMax.x : currentTouchDelta.x;
			touchDeltaMax.y = touchDeltaMax.y > currentTouchDelta.y ? touchDeltaMax.y : currentTouchDelta.y;
			touchDeltaMin.x = touchDeltaMin.x < currentTouchDelta.x ? touchDeltaMin.x : currentTouchDelta.x;
			touchDeltaMin.y = touchDeltaMin.y < currentTouchDelta.y ? touchDeltaMin.y : currentTouchDelta.y;

			GUILayout.Label("Screen pixels: " + mainCamera.pixelWidth + ":" + mainCamera.pixelHeight, DebugGUIstyle);
			GUILayout.Label("Touch position: " + currentTouch.position.x  + " " + currentTouch.position.y, DebugGUIstyle);
			GUILayout.Label("Delta touch x range: " + touchDeltaMin.x  + " " + touchDeltaMax.x, DebugGUIstyle);
			GUILayout.Label("Delta touch y range: " + touchDeltaMin.y  + " " + touchDeltaMin.y, DebugGUIstyle);
			
		}

		if(TimeInfo){
			//Time vales
			GUILayout.Label("deltaTime : " + Time.deltaTime, DebugGUIstyle);
			GUILayout.Label("deltaTime(unscaled): " + Time.unscaledDeltaTime, DebugGUIstyle);
			GUILayout.Label("Framerate : " + 1/Time.deltaTime, DebugGUIstyle);
			GUILayout.Label("fixedDeltaTime : " + Time.fixedDeltaTime, DebugGUIstyle);
			GUILayout.Label("fixedDeltaTime(unscaled) : " + Time.fixedUnscaledDeltaTime, DebugGUIstyle);
			GUILayout.Label("timeScale : " + Time.timeScale, DebugGUIstyle);
			GUILayout.Label("AverageRateUpdate() : " + averageUpdateCallRate, DebugGUIstyle);
			GUILayout.Label("AverageRateFixedUpdate() : " + averageFixedUpdateCallRate, DebugGUIstyle);
		}

		if(InertialInfo){
			GUILayout.Label("System support gyro : " + SystemInfo.supportsGyroscope, DebugGUIstyle);
			GUILayout.Label("Gyro Attitude : " + gyro.attitude.eulerAngles, DebugGUIstyle);
			GUILayout.Label("Gyro rate : " + gyro.rotationRate, DebugGUIstyle);
		}

		if(UIInfo){
			GUILayout.Label("Throw Vel: " + uiController.PoUiController.ThrowVelocity, DebugGUIstyle);
			GUILayout.Label("CurveAcc.: " + uiController.PoUiController.CurveAcceleration, DebugGUIstyle);
			GUILayout.Label("UI Acive : " + uiController.isUIActive, DebugGUIstyle);
		}

        GUILayout.EndArea();
	}

	//0 - Update
	//1 - Fixed Update
	public void LogFunctionCalls(int callerId){
		switch(callerId){
			case 0:
					//Debug.Log("Update called");
					if((DateTime.Now - storedTime).TotalSeconds > secondsOfFunctionCallHistory){
						timesUpdateWasCalled = 0;
						storedTime = DateTime.Now;
					}
					else {
						timesUpdateWasCalled++;
					}
					averageUpdateCallRate = timesUpdateWasCalled/secondsOfFunctionCallHistory;
				break;
			case 1:
					//Debug.Log("Update called");
					if((DateTime.Now - storedTime).TotalSeconds > secondsOfFunctionCallHistory){
						timesFixedUpdateWasCalled = 0;
						storedTime = DateTime.Now;
					}
					else {
						timesFixedUpdateWasCalled++;
					}
					averageFixedUpdateCallRate = timesFixedUpdateWasCalled/secondsOfFunctionCallHistory;
				break;
			default:
				return;
		}
	}

}
