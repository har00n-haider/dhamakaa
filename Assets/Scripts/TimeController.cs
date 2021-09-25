using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {

    //the factor used to slow down time  
    private float slowFactor = 5f;  
    //the new time scale  
    private float newTimeScale;  
  
    // Called when this script starts  
    void Start()  
    {  
        //calculate the new time scale  
        newTimeScale = Time.timeScale/slowFactor;  
    } 


	public void SlowTime(){

		//if the game is running normally  
		if (Time.timeScale == 1.0f)  
		{  
		    //assign the 'newTimeScale' to the current 'timeScale'  
		    Time.timeScale = newTimeScale;  
		    //proportionally reduce the 'fixedDeltaTime', so that the Rigidbody simulation can react correctly  
		    Time.fixedDeltaTime = Time.fixedDeltaTime/slowFactor;  
		    //The maximum amount of time of a single frame  
		    Time.maximumDeltaTime = Time.maximumDeltaTime/slowFactor;  
		}  
		else if (Time.timeScale == newTimeScale) //the game is running in slow motion  
		{  
		    //reset the values  
		    Time.timeScale = 1.0f;  
		    Time.fixedDeltaTime = Time.fixedDeltaTime*slowFactor;  
		    Time.maximumDeltaTime = Time.maximumDeltaTime*slowFactor;  
		}  

	}
	

}
