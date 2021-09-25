using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	#region Properties

	public DebugGUI DebugGUI;

	#endregion

	#region Members


	#endregion

	//Initialisation of properties at start of scene
	void Start () {
		SystemSetup();
	}

	// Update is called once per frame
	void Update () {
		DebugGUI.LogFunctionCalls(0);
	}

	void FixedUpdate(){
		DebugGUI.LogFunctionCalls(1);
	}
	
	//This is called when the scene is finished rendering. Camera contol stuff should happen here
	//TODO: Test why this is the case?
    void LateUpdate() {
		DebugGUI.LogFunctionCalls(3);
    }

	void OnGUI(){
		DebugGUI.UpdateGUI();
	}

	private void SystemSetup(){
		// Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

}