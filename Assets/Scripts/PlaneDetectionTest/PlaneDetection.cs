using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.ARFoundation;

public class PlaneDetection : MonoBehaviour
{
    private ARPlaneManager planeManager;
    private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

    // wall placement objects
    public GameObject targetIndicator;
    public GameObject targetObject;
    
    // inputs
    private MagicLeapInputs magicLeapInputs;
    private MagicLeapInputs.ControllerActions controllerActions;
    private bool isPlacing;

    private void Awake()
    {
        // subscribe to permission events
        permissionCallbacks.OnPermissionGranted += PermissionCallbacks_OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied += PermissionCallbacks_OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain += PermissionCallbacks_OnPermissionDenied;

    }
    
    
    private void OnDestroy()
    {
        // unsubscribe from permission events
        permissionCallbacks.OnPermissionGranted -= PermissionCallbacks_OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied -= PermissionCallbacks_OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= PermissionCallbacks_OnPermissionDenied;
    }
    
    
    private void Start()
    {
        // set wall objects as inactive
        targetIndicator.SetActive(false);
        targetObject.SetActive(false);
        
        // make sure the plane manager is disabled at the start of the scene before permissions are granted
        planeManager = FindObjectOfType<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("Failed to find ARPlaneManager in scene. Disabling Script");
            enabled = false;
        }
        else
        {
            planeManager.enabled = false;
        }

        // request spatial mapping permission for plane detection
        MLPermissions.RequestPermission(MLPermission.SpatialMapping, permissionCallbacks);
        
        magicLeapInputs = new MagicLeapInputs();
        magicLeapInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(magicLeapInputs);
        
        controllerActions.Trigger.performed += Trigger_performed;
        
    }
    
    private void Trigger_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Trigger pressed");
 
        if (targetIndicator.activeSelf)
        {
            isPlacing = false;
            targetObject.gameObject.SetActive(true);
            targetIndicator.SetActive(false);
            targetObject.transform.position = targetIndicator.transform.position;
            targetObject.transform.rotation = targetIndicator.transform.rotation;
        } 
    }
    
    // if permission denied, disable plane manager
    private void PermissionCallbacks_OnPermissionDenied(string permission)
    {
        Debug.LogError($"Failed to create Planes Subsystem due to missing or denied {MLPermission.SpatialMapping} permission. Please add to manifest. Disabling script.");
        planeManager.enabled = false;
    }

    // if permission granted, enable plane manager
    private void PermissionCallbacks_OnPermissionGranted(string permission)
    {
        if (permission == MLPermission.SpatialMapping)
        {
            planeManager.enabled = true;
            Debug.Log("Plane manager is active");
        }
    }
    
    
    private void Update()
    {
        if (planeManager.enabled)
        {
            PlanesSubsystem.Extensions.Query = new PlanesSubsystem.Extensions.PlanesQuery
            {
                BoundsCenter = Camera.main.transform.position,
                BoundsRotation = Camera.main.transform.rotation,
                BoundsExtents = Vector3.one * 20f,
                Flags = planeManager.requestedDetectionMode.ToMLQueryFlags() | PlanesSubsystem.Extensions.MLPlanesQueryFlags.Polygons | PlanesSubsystem.Extensions.MLPlanesQueryFlags.Semantic_Wall,
                MaxResults = 100,
                MinPlaneArea = 0.25f
            };
            
        }
        
        
        // raycast from the controller outward
        Ray raycastRay = new Ray(controllerActions.Position.ReadValue<Vector3>(), controllerActions.Rotation.ReadValue<Quaternion>() * Vector3.forward);

        // if ray hits an object on the Planes layer, position the indicator at the hit point and set it as active
        if (Physics.Raycast(raycastRay, out RaycastHit hitInfo, 100, LayerMask.GetMask("Planes")))
        {
            Debug.Log(hitInfo.transform);
            targetIndicator.transform.position = hitInfo.point;
            targetIndicator.transform.rotation = Quaternion.LookRotation(-hitInfo.normal);
            targetIndicator.gameObject.SetActive(true);
        }
    }
    
}
