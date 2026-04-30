using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController: MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Camera cam;
    
    public void StartNewGame()
    {  
        StartCoroutine(LoadScene());
        StartCoroutine(WaitForRooms());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public IEnumerator LoadScene()
    {
        yield return SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);
    }
    public IEnumerator WaitForRooms()
    {
        yield return new WaitForSeconds(1);
        var floorPlan = MapGenerator.instance.getFloorPlan;
        var rooms = RoomManager.instance.getCreatedRooms;
        foreach (var room in rooms)
        {
            room.SetUpParamsForEnemies(cam, playerObj, playerHealthSystem);
            if (room.indexes.Contains((4, 5)))
            {
                playerObj.transform.position = room.transform.position;
            }
        }
        mainMenu.enabled = false;
    }

}
