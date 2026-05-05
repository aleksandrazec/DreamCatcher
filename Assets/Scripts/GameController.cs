using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameController: MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;
    [SerializeField] private PlayerMoneySystem playerMoneySystem;
    [SerializeField] private UnityEngine.Canvas mainMenu;
    [SerializeField] private UnityEngine.Canvas pausedMenu;
    [SerializeField] private UnityEngine.Canvas diaryMenu;
    [SerializeField] private UnityEngine.Canvas sky;
    [SerializeField] private SkyCanvas skyCanvas;
    [SerializeField] private Blink eyes;
    [SerializeField] public Camera cam;
    [SerializeField] public Camera cam2;
    [SerializeField] public GameObject directionalLight;
    [SerializeField] public GameObject newGameButton;
    [SerializeField] public GameObject helpButton;
    [SerializeField] public GameObject speedUpgradeButton;
    [SerializeField] public GameObject healthUpgradeButton;
    [SerializeField] public GameObject attackUpgradeButton;
    [SerializeField] public GameObject backUpgradeButton;
    [SerializeField] public GameObject backButton;
    [SerializeField] public EventSystem eventSystem;
    private List<Room> rooms = new List<Room>();
    private int[,] floorPlan;
    private (int, int) playerPosition;
    private Room currentRoom=null;
    private bool readyForBoss = false;
    private bool bossBeaten=false;
    private bool inDreamWorld = true;

    private MapGenerator mapGenerator;
    private RoomManager roomManager;

    public int mapGeneratorScene=1;
    public int irlRoomScene=2;
    public int currentScene = -1;

    private InputSystem_Actions _uiInputActions;

    private void Awake()
    {
        playerHealthSystem = player.healthSystem;
        playerMoneySystem=player.moneySystem;
        playerObj= player.gameObject;
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible = false;
        _uiInputActions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        _uiInputActions.UI.Enable();
        _uiInputActions.UI.Cancel.started += Cancel;
    }
    private void OnDisable()
    {
        _uiInputActions.UI.Disable();
        _uiInputActions.UI.Cancel.started -= Cancel;
    }
    private void Cancel(InputAction.CallbackContext obj)
    {
        if (mainMenu.enabled) { return; }
        if (sky.enabled)
        {
            sky.enabled = false;
            return;
        }
        if (pausedMenu.enabled) {
            pausedMenu.enabled = false;
            Time.timeScale = 1;
        } 
        else
        {
            Time.timeScale = 0;
            pausedMenu.enabled = true;
            if (helpButton.gameObject.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(helpButton, new BaseEventData(eventSystem));
            }
            else
            {
                eventSystem.SetSelectedGameObject(backButton, new BaseEventData(eventSystem));
            }
        }
    }
    public void SetBackButton()
    {
        eventSystem.SetSelectedGameObject(backButton, new BaseEventData(eventSystem));
    }
    public void SetPausedButton()
    {
        eventSystem.SetSelectedGameObject(helpButton, new BaseEventData(eventSystem));
    }
    public void LookAtTheSky()
    {
        skyCanvas.SetSkyToStart();
        sky.enabled= true;
        skyCanvas.AnimateSky();
    }
    public void OpenDiary()
    {
        diaryMenu.enabled = true;
        eventSystem.SetSelectedGameObject(backUpgradeButton, new BaseEventData(eventSystem));
    }
    public void SpeedUpgrade() {
        var button = speedUpgradeButton.GetComponent<Button>();
        button.interactable = false;
        player.speedUpgrades++;
        SaveSystem.SavePlayer(player);
        eventSystem.SetSelectedGameObject(backUpgradeButton, new BaseEventData(eventSystem));
    }
    public void AttackUpgrade()
    {
        var button = attackUpgradeButton.GetComponent<Button>();
        button.interactable = false;
        player.attackUpgrades++;
        SaveSystem.SavePlayer(player);
        eventSystem.SetSelectedGameObject(backUpgradeButton, new BaseEventData(eventSystem));
    }
    public void HealthUpgrade()
    {
        var button = healthUpgradeButton.GetComponent<Button>();
        button.interactable = false;
        player.healthUpgrades++;
        SaveSystem.SavePlayer(player);
        eventSystem.SetSelectedGameObject(backUpgradeButton, new BaseEventData(eventSystem));
    }

    private void Update()
    {
        if (bossBeaten) {
            bossBeaten = false;
            StartCoroutine(WinRoutine()); 
        }
        if (inDreamWorld)
        {
            if (currentRoom != null)
            {
                if (currentRoom.spawnedEnemies.Count == 0)
                {
                    if (readyForBoss)
                    {
                        currentRoom.OpenBossAndNormalDoors();
                        CheckIfAllRoomsAreDone();
                    }
                    else
                    {
                        currentRoom.OpenDoors();
                        CheckIfAllRoomsAreDone();
                    }
                }
            }
        }
    }
    
    public void CheckIfAllRoomsAreDone()
    {
        var numOfRooms = rooms.Count;
        foreach (var room in rooms)
        {
            if (room.spawnedEnemies.Count == 0)
            {
                numOfRooms--;
            }
        }
        if (numOfRooms == 1)
        {
            readyForBoss = true;
        }else if (numOfRooms == 0)
        {
            bossBeaten = true;
        }
    }

    public void StartNewGame()
    {
        inDreamWorld = true;
        var button = newGameButton.GetComponent<Button>();
        button.interactable = false;
        Time.timeScale = 0;
        StartCoroutine(LoadScene(mapGeneratorScene));
        StartCoroutine(WaitForRooms());
        button.interactable = true;
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
        currentScene = number;
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
        if(currentScene== number)
        {
            currentScene = -1;
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
                mapGenerator.MarkCurrentRoom(room.indexes);
            }
            else
            {
                room.gameObject.SetActive(false);
            }
        }
    }
    public void GoToMainMenu()
    {
        mainMenu.enabled = true;
        eventSystem.SetSelectedGameObject(newGameButton, new BaseEventData(eventSystem));
        pausedMenu.enabled = false;
        StartCoroutine(UnloadScene(currentScene));
        SaveSystem.SavePlayer(player);
    }
    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(3);
        PrepareToGoToRealWorld();
        GoToRealWorld(true);
    }
    public void PrepareToGoToRealWorld()
    {
        //StartCoroutine(DeleteRooms());
        StartCoroutine(UnloadScene(mapGeneratorScene));
        SaveSystem.SavePlayer(player);
    }
    public void GoToRealWorld(bool win)
    {
        inDreamWorld = false;
        directionalLight.SetActive(win);
        StartCoroutine(LoadScene(irlRoomScene));
        var button = healthUpgradeButton.GetComponent<Button>();
        button.interactable = true;
        button = attackUpgradeButton.GetComponent<Button>();
        button.interactable = true;
        button = speedUpgradeButton.GetComponent<Button>();
        button.interactable = true;
        healthUpgradeButton.SetActive(true);
        attackUpgradeButton.SetActive(true);
        speedUpgradeButton.SetActive(true);
        StartCoroutine(AwaitCamera());
        player.cam.enabled = false;
        playerObj.SetActive(false);
        player.isometricCam.SetActive(false);
        //StartCoroutine(DeleteRooms());
        StartCoroutine(WaitAndOpenEyes());
    }
    public IEnumerator AwaitCamera()
    {
        GameObject[] cameraObj = GameObject.FindGameObjectsWithTag("MainCamera");
        while(cameraObj.Length != 2)
        {
            yield return new WaitForSeconds(0.5f);
            cameraObj = GameObject.FindGameObjectsWithTag("MainCamera");
        }
    }
    public IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(1);
    }
    public IEnumerator WaitAndOpenEyes()
    {
        yield return new WaitForSeconds(1);
        eyes.OpenEyes();
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
        var playerData = SaveSystem.LoadPlayer();
        playerData.SetUpPlayer(player);
    }
    public void GoToDreamWorld()
    {
        bossBeaten = false;
        readyForBoss= false;
        inDreamWorld = true;
        StartCoroutine(UnloadScene(irlRoomScene));
        playerObj.SetActive(true);
        player.cam.enabled = true;
        player.isometricCam.SetActive(true);
        directionalLight.SetActive(true);
        Debug.Log("before loading mapgenscene");
        StartCoroutine(LoadScene(mapGeneratorScene));
        StartCoroutine(WaitForRooms());
        StartCoroutine(WaitAndOpenEyes());
    }
    public void LoadGame()
    {
        try
        {
            var playerData = SaveSystem.LoadPlayer();
            playerData.SetUpPlayer(player);
        }
        catch
        {
            Debug.Log("No save file");
            return;
        }
        mainMenu.enabled = false;
        GoToRealWorld(false);
    }
}
