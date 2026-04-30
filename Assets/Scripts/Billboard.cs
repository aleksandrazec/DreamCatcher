using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    // Update is called once per frame
    void LateUpdate()
    {
        if(cam != null)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
