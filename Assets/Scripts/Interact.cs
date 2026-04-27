using UnityEngine;

public class Interact : MonoBehaviour
{
    public LayerMask mask;

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity, mask))
        {
            var obj = hit.collider.gameObject;

            Debug.Log($"looking at {obj.name}", this);
        }
    }
}
