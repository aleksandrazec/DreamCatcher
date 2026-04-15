using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    private int[,] floorPlan;
    private int numOfRows;
    private int numOfColumns;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<(int,int)> endRooms;

    private (int, int) bossRoomIndex;
    private (int, int) shopRoomIndex;
    private (int, int) itemRoomIndex;

    public Cell cellPrefab;
    public GameObject emptyCellPrefab;
    private Queue<(int,int)> cellQueue;
    private List<Cell> spawnedCells;
    private List<GameObject> spawnedEmpties;

    [SerializeField] private Canvas canvas;

    [Header("Sprite References")]
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite shop;
    [SerializeField] private Sprite boss;

    private InputSystem_Actions _inputActions;
    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        _inputActions.UI.Enable();
        _inputActions.UI.GenerateMap.started += GenerateMap;
    }
    private void OnDisable()
    {
        _inputActions.UI.Disable();
        _inputActions.UI.GenerateMap.started -= GenerateMap;

    }
    private void GenerateMap(InputAction.CallbackContext obj)
    {
       SetupDungeon();
    }
    void Start()
    {
        numOfRows = 10;
        numOfColumns = 10;
        minRooms = 10;
        maxRooms = 15;
        spawnedCells = new();
        spawnedEmpties = new();
        SetupDungeon();
    }
    void SetupDungeon()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Destroy(spawnedCells[i].gameObject);
        }
        for (int i = 0; i < spawnedEmpties.Count; i++)
        {
            Destroy(spawnedEmpties[i].gameObject);
        }
        spawnedEmpties.Clear();
        spawnedCells.Clear();

        floorPlan = new int[numOfRows, numOfColumns];
        floorPlanCount = 0;
        cellQueue = new Queue<(int, int)>();
        endRooms = new List<(int, int)>();

        VisitCell(4, 5);
        GenerateDungeon();
    } 
    void GenerateDungeon()
    {
        while (cellQueue.Count > 0)
        {
            (int,int) index= cellQueue.Dequeue();
            int rowIndex = index.Item1;
            int columnIndex = index.Item2;
            bool created = false;
            if (columnIndex > 1) created |= VisitCell(rowIndex, columnIndex - 1);
            if(columnIndex < 9) created |= VisitCell(rowIndex, columnIndex + 1);
            if (rowIndex > 2) created |= VisitCell(rowIndex - 1, columnIndex);
            if (rowIndex < 7) created|= VisitCell(rowIndex + 1, columnIndex);

            if (created == false)
            {
                endRooms.Add((rowIndex, columnIndex));
            }
        }
        if (floorPlanCount< minRooms)
        {
            SetupDungeon();
            return;
        }
        SetupSpecialRooms();
    }
    void SetupSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : (-1, -1);
        if (bossRoomIndex != (-1, -1))
        {
            endRooms.RemoveAt(endRooms.Count - 1);
        }
        itemRoomIndex = RandomEndRoom();
        shopRoomIndex = RandomEndRoom();
        if (itemRoomIndex == (-1, -1) || shopRoomIndex == (-1, -1) || bossRoomIndex == (-1, -1))
        {
            SetupDungeon();
            return;
        }
        SetUpVisuals();
        UpdateSpecialRoomVisuals();

    }
    void UpdateSpecialRoomVisuals()
    {
        foreach (var cell in spawnedCells)
        {
            if ((cell.rowIndex, cell.columnIndex) == itemRoomIndex)
            {
                cell.SetSpecialRoomSprite(item);
            }
            if ((cell.rowIndex, cell.columnIndex) == shopRoomIndex)
            {
                cell.SetSpecialRoomSprite(shop);
            }
            if ((cell.rowIndex, cell.columnIndex) == bossRoomIndex)
            {
                cell.SetSpecialRoomSprite(boss);
            }
        }
    }
    (int,int) RandomEndRoom()
    {
        if(endRooms.Count == 0) return (-1,-1);
        int randomRoom = UnityEngine.Random.Range(0,endRooms.Count);
        (int, int) index = endRooms[randomRoom];
        endRooms.RemoveAt(randomRoom);
        return index;
    }
    private int GetNeighbourCount(int rowIndex, int columnIndex)
    {
        int down = 0;
        int left = 0;
        int right = 0;
        int up = 0;
        try { down = floorPlan[rowIndex - 1, columnIndex]; } catch { }
        try { up= floorPlan[rowIndex + 1, columnIndex]; } catch { }
        try { left = floorPlan[rowIndex, columnIndex - 1]; } catch { }
        try { right = floorPlan[rowIndex, columnIndex + 1]; } catch { }
        return left+right+up+down;
    }
    private bool VisitCell(int rowIndex, int columnIndex)
    {
        if (floorPlan[rowIndex, columnIndex] != 0 || GetNeighbourCount(rowIndex, columnIndex) > 1 || floorPlanCount > maxRooms || UnityEngine.Random.value < 0.5f)
        {
            return false;
        }
        cellQueue.Enqueue((rowIndex,columnIndex));
        floorPlan[rowIndex, columnIndex] = 1;
        floorPlanCount++;
        return true;
    }
    private void SpawnEmpty()
    {
        GameObject newEmpty = Instantiate(emptyCellPrefab, this.transform);
        spawnedEmpties.Add(newEmpty);
    }
    private void SpawnRoom(int rowIndex, int columnIndex)
    {
        Cell newCell = Instantiate(cellPrefab, this.transform);
        newCell.value = 1;
        newCell.rowIndex= rowIndex;
        newCell.columnIndex= columnIndex;

        spawnedCells.Add(newCell);
    }
    private void SetUpVisuals()
    {
        int[] emptyArray= new int[numOfRows];
        for (int i = 0; i < floorPlan.GetLength(0); i++)
        {
            for (int j = 0; j < floorPlan.GetLength(1); j++)
            {
                emptyArray[i]|=floorPlan[i,j];
            }
        }
        for (int i = 0; i < floorPlan.GetLength(0); i++)
        {
            if (emptyArray[i] == 0)
            {
                continue;
            }
            for (int j = 0; j < floorPlan.GetLength(1); j++)
            {
                if(floorPlan[i, j] != 0) {
                    SpawnRoom(i, j);
                }else 
                {
                    SpawnEmpty();
                }
            }
        }
    }
}
