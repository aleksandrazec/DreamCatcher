using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{
    private int[,] floorPlan;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<(int,int)> endRooms;

    private (int, int) bossRoomIndex;
    private (int, int) shopRoomIndex;
    private (int, int) itemRoomIndex;

    public Cell cellPrefab;
    private float cellSize;
    private float cellXOffset;
    private float cellYOffset;
    private Queue<(int,int)> cellQueue;
    private List<Cell> spawnedCells;

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
       Debug.Log("called");
       SetupDungeon();
    }
    void Start()
    {
        minRooms = 10;
        maxRooms = 15;
        cellSize = 15f;
        cellXOffset = 400f;
        cellYOffset = 0f;
        spawnedCells = new();
        SetupDungeon();
    }
    void SetupDungeon()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Destroy(spawnedCells[i].gameObject);
        }
        spawnedCells.Clear();

        floorPlan = new int[10, 10];
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
        int randomRoom = Random.Range(0,endRooms.Count);
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
        if (floorPlan[rowIndex, columnIndex] != 0 || GetNeighbourCount(rowIndex, columnIndex) > 1 || floorPlanCount > maxRooms || Random.value < 0.5f)
        {
            return false;
        }
        cellQueue.Enqueue((rowIndex,columnIndex));
        floorPlan[rowIndex, columnIndex] = 1;
        floorPlanCount++;
        SpawnRoom(rowIndex, columnIndex);
        return true;
    }
    private void SpawnRoom(int rowIndex, int columnIndex)
    {
        Vector2 position = new Vector2(columnIndex*cellSize+cellYOffset, -rowIndex*cellSize+cellXOffset);
        Cell newCell = Instantiate(cellPrefab, position, Quaternion.identity, canvas.transform);
        newCell.value = 1;
        newCell.rowIndex= rowIndex;
        newCell.columnIndex= columnIndex;

        spawnedCells.Add(newCell);
    }
}
