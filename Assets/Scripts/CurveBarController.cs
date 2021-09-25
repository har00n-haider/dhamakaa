using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurveBarController : MonoBehaviour {

	public Image CurveBar;
	public RectTransform CurvePointRect;
	
	public float CurveValue { get { return curveValue; } set { curveValue = value; }}
	[SerializeField] private float curveValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		CurveChange(Mathf.Clamp(CurveValue, -50,50));
	}
 
    /// <summary>
    /// Adjust the bar fill base in the input curve value
    /// </summary>
    /// <param name="curveValue"></param>
    void CurveChange(float curveValue){
		CurveBar.fillClockwise     = curveValue < 0;
        float amount		       = (-curveValue/100.0f) * 180.0f/360;
        CurveBar.fillAmount 	   = Mathf.Abs(amount);
        float buttonAngle 		   = amount * 360;
        CurvePointRect.eulerAngles = new Vector3(0, 0, -buttonAngle);
    }
}
