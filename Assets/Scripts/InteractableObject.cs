using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] public InteractableObjectType objectType;
    [SerializeField] public GameObject gameObj;
    [SerializeField] public GameObject text;
    [SerializeField] public Item item;
    [SerializeField] public Chest chest;
    public GameController gameController;
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
                GoToDreamWorld();
                break;
            case InteractableObjectType.desk:
                OpenDiary();
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
    private void OpenDiary()
    {
        if (gameController == null)
        {
            GameObject[] gameControllerObj = GameObject.FindGameObjectsWithTag("GameController");
            gameController = gameControllerObj[0].GetComponent<GameController>();
        }
        gameController.OpenDiary();
    }
    private void GoToDreamWorld()
    {
        if (gameController == null)
        {
            GameObject[] gameControllerObj = GameObject.FindGameObjectsWithTag("GameController");
            gameController = gameControllerObj[0].GetComponent<GameController>();
        }
        StartCoroutine(DreamWorldRoutine());    
    }
    private IEnumerator DreamWorldRoutine()
    {
        gameController.PrepareToGoToDreamWorld();
        yield return new WaitForSeconds(3);
        gameController.GoToDreamWorld();
    }
}
