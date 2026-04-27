using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public LayerMask mask;
    [SerializeField] public InteractableObjectType objectType;
    [SerializeField] public GameObject gameObj;
    [SerializeField] public GameObject text;
    public enum InteractableObjectType
    {
        door,
        window,
        bed,
        desk
    }

    public void SetTextVisible()
    {
        text.SetActive(true);
    }
    public void SetTextInvisible()
    {
        text.SetActive(false);
    }
}
