using Unity.AppUI.MVVM;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    public LayerMask mask;
    public GameObject lookingAt;
    public float distance = 15f;
    public InteractableObject currentObject;
    public InteractableObject previousObject;

    private InputSystem_Actions _playerInputActions;
    private Vector3 _input;
    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Interact.started += InteractAction;
    }
    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player.Interact.started -= InteractAction;

    }
    private void InteractAction(InputAction.CallbackContext obj)
    {
        if (lookingAt != null)
        {
            currentObject.Interact();
        }
    }
    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, distance, mask))
        {
            lookingAt = hit.collider.gameObject;
            currentObject = lookingAt.GetComponent<InteractableObject>();
            if(previousObject != null && currentObject != previousObject)
            {
                previousObject.SetTextInvisible();
            }
            if (currentObject != null && currentObject.text != null) 
            {
                if (!currentObject.text.activeInHierarchy)
                {
                    currentObject.SetTextVisible();
                }
            }
        }
        else
        {
            lookingAt = null;
            if (previousObject != null)
            {
                previousObject.SetTextInvisible();
            }
        }
        previousObject = currentObject;
    }
    
}
