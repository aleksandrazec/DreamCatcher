using System.Collections;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public GameObject upper;
    public GameObject lower;
    public Transform upperStartTransform;
    public Transform lowerStartTransform;
    public Transform upperEndTransform;
    public Transform lowerEndTransform;
    public float speed = 0.05f; 
    public void CloseEyes()
    {
        StartCoroutine(CloseEyesRoutine());
    }
    public void OpenEyes()
    {
        StartCoroutine(OpenEyesRoutine());
    }
    public IEnumerator CloseEyesRoutine()
    {
        while (upper.transform.position != upperEndTransform.position && lower.transform.position != lowerEndTransform.position)
        {
            upper.transform.position = Vector3.MoveTowards(upper.transform.position, upperEndTransform.position, speed * Time.deltaTime);
            lower.transform.position = Vector3.MoveTowards(lower.transform.position, lowerEndTransform.position, speed * Time.deltaTime);
            Debug.Log("closing eyes");
        }

        yield return null;
        
    }
    public IEnumerator OpenEyesRoutine()
    {
        while (upper.transform.position != upperStartTransform.position && lower.transform.position != lowerStartTransform.position)
        {
            upper.transform.position = Vector3.MoveTowards(upper.transform.position, upperStartTransform.position, speed * Time.deltaTime);
            lower.transform.position = Vector3.MoveTowards(lower.transform.position, lowerStartTransform.position, speed * Time.deltaTime);
            Debug.Log("opening eyes");
        }
        yield return null;
    }
}
