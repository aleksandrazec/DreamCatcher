using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private MapGenerator mapGenerator;
    private RoomManager roomManager;

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
        Debug.Log("loading scene " + number);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(number,LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }
        Debug.Log("loaded scene " + number);
    }
    public IEnumerator UnloadScene(int number)
    {
        Debug.Log("unloading scene " + number);
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(number);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
        Debug.Log("unloaded scene " + number);
    }
    public IEnumerator WaitForRooms()
    {
        bool notReady = true;
        while (notReady)
        {
            try
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(mapGeneratorScene));
                notReady = false;
            }
            catch
            {

            }
            yield return null;
        }
        if (mapGenerator == null)
        {
            GameObject[] mapGeneratorObject = GameObject.FindGameObjectsWithTag("MapGenerator");
            mapGenerator = mapGeneratorObject[0].GetComponent<MapGenerator>();
            roomManager = mapGeneratorObject[0].GetComponent<RoomManager>();
        }
        mapGenerator.GenerateMap();
        while (roomManager.getCreatedRooms == rooms)
        {
            yield return null;
        }
        rooms = roomManager.getCreatedRooms;
        floorPlan = mapGenerator.getFloorPlan;
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
        //eyes.CloseEyes();
        //StartCoroutine(DeleteRooms());
        StartCoroutine(UnloadScene(mapGeneratorScene));
        SaveSystem.SavePlayer(player);
    }
    public void GoToRealWorld()
    {
        directionalLight.SetActive(false);
        playerObj.SetActive(false);
        StartCoroutine(LoadScene(irlRoomScene));
        player.cam.enabled = false;
        player.isometricCam.SetActive(false);
        //eyes.OpenEyes();
        //StartCoroutine(DeleteRooms());
    }
    public IEnumerator DeleteRooms()
    {
        yield return new WaitForSeconds(3);
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }
        rooms.Clear();
        roomManager.ResetCreatedRooms();
    }
    public void PrepareToGoToDreamWorld()
    {
        //eyes.CloseEyes();
        var playerData = SaveSystem.LoadPlayer();
        playerData.SetUpPlayer(player);
    }
    public void GoToDreamWorld()
    {
        //Time.timeScale = 0;
        StartCoroutine(UnloadScene(irlRoomScene));
        playerObj.SetActive(true);
        player.cam.enabled = true;
        player.isometricCam.SetActive(true);
        directionalLight.SetActive(true);
        Debug.Log("before loading mapgenscene");
        StartCoroutine(LoadScene(mapGeneratorScene));
        StartCoroutine(WaitForRooms());
        //eyes.OpenEyes();
    }
}
