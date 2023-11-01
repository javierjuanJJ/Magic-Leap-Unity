using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    private MagicLeapInputs magicLeapInputs;
     private MagicLeapInputs.ControllerActions controllerActions;
    
        public GameObject newCube;
        public Transform controllerTransform;
        private GameObject currentCube;
    
    
        private void Start()
        {
            magicLeapInputs = new MagicLeapInputs();
            magicLeapInputs.Enable();
            controllerActions = new MagicLeapInputs.ControllerActions(magicLeapInputs);
    
            // subscribe to the controller events
            controllerActions.Bumper.performed += Bumper_performed;
            controllerActions.TouchpadPosition.performed += TouchpadPositionOnperformed;
        }
    
        // when bumper is pressed, instantiate a new cube
        private void Bumper_performed(InputAction.CallbackContext obj)
        {
            currentCube = Instantiate(newCube, controllerTransform.position, controllerTransform.rotation);
        }
    
        // when the touchpad is touched, set the local scale of the current cube
        private void TouchpadPositionOnperformed(InputAction.CallbackContext obj)
        {
            var touchPosition = controllerActions.TouchpadPosition.ReadValue<Vector2>();
            var touchValue = Mathf.Clamp((touchPosition.y + 1) / (1.8f), 0, 1);
            currentCube.transform.localScale = new Vector3(touchValue, touchValue, touchValue);
    
        }
}
