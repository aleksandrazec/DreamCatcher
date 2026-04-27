using Unity.VisualScripting;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public LayerMask mask;
    public GameObject lookingAt;
    public float distance = 15f;
    public InteractableObject currentObject;
    public InteractableObject previousObject;
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
            if (!currentObject.text.activeInHierarchy)
            {
                currentObject.SetTextVisible();
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
