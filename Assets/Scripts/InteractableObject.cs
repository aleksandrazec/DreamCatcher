using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] public InteractableObjectType objectType;
    [SerializeField] public GameObject gameObj;
    [SerializeField] public GameObject text;
    [SerializeField] public Item item;
    [SerializeField] public Chest chest;
    public enum InteractableObjectType
    {
        door,
        window,
        bed,
        desk,
        item,
        chest
    }

    public void SetTextVisible()
    {
        text.SetActive(true);
    }
    public void SetTextInvisible()
    {
        text.SetActive(false);

    }
    public void Interact()
    {
        switch (objectType)
        {
            case InteractableObjectType.door:
                break;
            case InteractableObjectType.window: 
                break;
            case InteractableObjectType.bed: 
                break;
            case InteractableObjectType.desk: 
                break;
            case InteractableObjectType.item:
                item.UseItem();
                break;
            case InteractableObjectType.chest:
                chest.OpenChest(); 
                break;
            default:
                break;
        }
    }
}
