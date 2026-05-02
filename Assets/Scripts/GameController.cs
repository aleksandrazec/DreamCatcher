using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
public class GameController: MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;
    [SerializeField] private PlayerMoneySystem playerMoneySystem;
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Blink eyes;
    [SerializeField] public Camera cam;
    [SerializeField] public GameObject directionalLight;
    private List<Room> rooms = new List<Room>();
    private int[,] floorPlan;
    private (int, int) playerPosition;
    private Room currentRoom=null;

    public int mapGeneratorScene=1;
    public int irlRoomScene=2;
    private void Awake()
    {
        playerHealthSystem = player.healthSystem;
        playerMoneySystem=player.moneySystem;
        playerObj= player.gameObject;
    }
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
        StartCoroutine(LoadScene(mapGeneratorScene));
        StartCoroutine(WaitForRooms());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public IEnumerator LoadScene(int number)
    {
        yield return SceneManager.LoadSceneAsync(number,LoadSceneMode.Additive);
    }
    public IEnumerator UnloadScene(int number)
    {
        yield return SceneManager.UnloadSceneAsync(number);
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
    public void PrepareToGoToRealWorld()
    {
        eyes.CloseEyes();
        StartCoroutine(UnloadScene(mapGeneratorScene));
        SaveSystem.SavePlayer(player);
    }
    public void GoToRealWorld()
    {
        currentRoom.room.SetActive(false);
        directionalLight.SetActive(false);
        playerObj.SetActive(false);
        StartCoroutine(LoadScene(irlRoomScene));
        player.cam.enabled = false;
        player.isometricCam.SetActive(false);
        eyes.OpenEyes();
        StartCoroutine(DeleteRooms());
    }
    public IEnumerator DeleteRooms()
    {
        yield return new WaitForSeconds(4);
        foreach(var room in rooms)
        {
            Destroy(room.gameObject);
        }
        rooms.Clear();
    }
    public void PrepareToGoToDreamWorld()
    {
        eyes.CloseEyes();
        StartCoroutine(UnloadScene(irlRoomScene));
        var playerData = SaveSystem.LoadPlayer();
        playerData.SetUpPlayer(player);
    }
    public void GoToDreamWorld()
    {
        Time.timeScale = 0;
        playerObj.SetActive(true);
        player.cam.enabled = true;
        player.isometricCam.SetActive(true);
        StartCoroutine(LoadScene(mapGeneratorScene));
        StartCoroutine(WaitForRooms());
        eyes.OpenEyes();
    }
}
