using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController: MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;
    [SerializeField] private PlayerMoneySystem playerMoneySystem;
    [SerializeField] private Canvas mainMenu;
    [SerializeField] public Camera cam;
    private List<Room> rooms = new List<Room>();
    private int[,] floorPlan;
    private (int, int) playerPosition;
    private Room currentRoom=null;
    private void Update()
    {
        if (currentRoom != null)
        {
            if (currentRoom.spawnedEnemies.Count == 0)
            {
                currentRoom.OpenDoors();
            }
        }
    }
    public void StartNewGame()
    {
        Time.timeScale = 0;
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
        bool notReady = true;
        while (notReady)
        {
            try{
                rooms = RoomManager.instance.getCreatedRooms;
                notReady = false;
            }
            catch
            {

            }
            yield return null;
        }
        floorPlan = MapGenerator.instance.getFloorPlan;
        foreach (var room in rooms)
        {
            if (room.indexes.Contains((4, 5)))
            {
                playerObj.transform.position = room.transform.position;
                ChangePlayerPosition((4, 5));
                room.RemoveAllEnemies();
            }
            else
            {
                room.SetUpParamsForEnemies(cam, playerObj, playerHealthSystem, playerMoneySystem);
            }
        }
        mainMenu.enabled = false;
        Time.timeScale = 1;
    }
    public void ChangePlayerPosition((int,int) newPosition)
    {
        playerPosition = newPosition;
        UpdateActiveRooms();
    }
    public void UpdateActiveRooms()
    {
        foreach (var room in rooms)
        {
            if (room.indexes.Contains(playerPosition))
            {
                room.gameObject.SetActive(true);
                currentRoom = room;
            }
            else
            {
                room.gameObject.SetActive(false);
            }
        }
    }
}
