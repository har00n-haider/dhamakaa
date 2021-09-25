using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BladeUIController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler  {


    [SerializeField]
	private GameObject blade;
    [SerializeField]
	private float bladeColorAlpha;
    [SerializeField]
    private float nearClipPlaneOffset;
    [SerializeField]
    private GameObject bladeTrailPrefab;

	private Image bladeDragPointImage;
	private RectTransform bladeDragPointImageRect;
    private UIController uiController;
    private Rigidbody bladeRigidBody;
    private Camera mainCamera;
    private GameObject currentBladeTrail;
    private CapsuleCollider bladeCollider;
    private bool isCutting;

	
    // Use this for initialization
    void Awake () {

        bladeDragPointImage     = GetComponent<Image>();
		bladeDragPointImageRect = GetComponent<RectTransform>();
        uiController            = GetComponentInParent<UIController>();
        bladeRigidBody          = blade.GetComponent<Rigidbody>();  
        mainCamera              = Camera.main; 
        bladeCollider           = blade.GetComponent<CapsuleCollider>();
        bladeCollider.enabled   = false;
	}


    public void OnPointerDown(PointerEventData eventData)
    {
        uiController.isUIActive = true;
        isCutting = true;
        bladeCollider.enabled = true;

        //Make the icon dissapear
        bladeDragPointImage.color = new Color(bladeDragPointImage.color.r,
                                              bladeDragPointImage.color.g,
                                              bladeDragPointImage.color.b,
                                              0);

        currentBladeTrail = Instantiate(bladeTrailPrefab, blade.transform);

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
		bladeDragPointImageRect.position = eventData.position;

        bladeRigidBody.position = mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, 
                                                                            eventData.position.y, 
                                                                            mainCamera.nearClipPlane + nearClipPlaneOffset));
        Ray pointerRay = mainCamera.ScreenPointToRay(eventData.position);

        Debug.DrawRay(pointerRay.origin, pointerRay.direction*10, Color.red, 1);

        bladeRigidBody.rotation = Quaternion.LookRotation(pointerRay.direction);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        uiController.isUIActive = false;
        isCutting = false;
        bladeCollider.enabled = false;


        //Make the icon reppear
        bladeDragPointImage.color = new Color(bladeDragPointImage.color.r,
                                              bladeDragPointImage.color.g,
                                              bladeDragPointImage.color.b,
                                              bladeColorAlpha);

        currentBladeTrail.transform.SetParent(null);
        Destroy(currentBladeTrail, 1);
    }
}
