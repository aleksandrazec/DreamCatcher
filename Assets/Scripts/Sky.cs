using System;
using System.Collections;
using UnityEngine;

public class Sky : MonoBehaviour
{
    private float startRange = -50;
    private float endRange = -10;
    private float oscilationRange;
    private float oscilationOffset;

    private void Start()
    {
        oscilationRange = (endRange - startRange) / 2;
        oscilationOffset = oscilationRange + startRange;
}
    private void Update()
    {
        float y = oscilationOffset + Mathf.Sin(Time.time) * oscilationRange*0.01f;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

}
