using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    private int[,] floorPlan;
    
    public int[,] getFloorPlan => floorPlan;

    private int numOfRows;
    private int numOfColumns;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<(int, int)> endRooms;

    private (int, int) bossRoomIndex;
    private (int, int) shopRoomIndex;
    private (int, int) itemRoomIndex;

    public Cell cellPrefab;
    public GameObject emptyCellPrefab;
    private Queue<(int, int)> cellQueue;
    private List<Cell> spawnedCells;
    private List<GameObject> spawnedEmpties;

    public List<Cell> getSpawnedCells => spawnedCells;

    [SerializeField] private Canvas canvas;

    private List<(int, int)> bigRoomIndexes;
    private List<List<(int, int)>> bigRooms;

    [Header("Sprite References")]
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite shop;
    [SerializeField] private Sprite boss;

    [Header("Room Variations")]
    [SerializeField] private Sprite big0;
    [SerializeField] private Sprite big1;
    [SerializeField] private Sprite big2;
    [SerializeField] private Sprite big3;
    [SerializeField] private Sprite vertical0;
    [SerializeField] private Sprite vertical1;
    [SerializeField] private Sprite horizontal0;
    [SerializeField] private Sprite horizontal1;
    [SerializeField] private Sprite lShape0;
    [SerializeField] private Sprite lShape1;
    [SerializeField] private Sprite lShape2;

    public static MapGenerator instance;

    private InputSystem_Actions _inputActions;

    private static readonly List<(int, int)[]> roomShapes = new() {
        new (int, int)[] { (0,-1) },
        new (int, int)[] { (0,1) },

        new (int, int)[] { (1,0) },
        new (int, int)[] { (-1,0) },

        new (int, int)[] { (0,1), (1,0) },
        new (int, int)[] { (0,1), (1,1) },
        new (int, int)[] { (1,0), (1,1) },

        new (int, int)[] { (1,-1), (1,0) },
        new (int, int)[] { (0,-1), (1,-1) },
        new (int, int)[] { (0,-1), (1,0) },

        new (int, int)[] { (0,1), (-1,0) },
        new (int, int)[] { (0,1), (-1,1) },
        new (int, int)[] { (-1,1), (-1,0) },

        new (int, int)[] { (0,-1), (-1,0) },
        new (int, int)[] { (0,-1), (-1,-1) },
        new (int, int)[] { (-1,0), (-1,-1) },

        new (int, int)[] { (0,1), (1,0), (1,1) },
        new (int, int)[] { (0,1), (-1,1), (-1,0) },
        new (int, int)[] { (0,-1), (1,-1), (1,0) },
        new (int, int)[] { (0,-1), (-1,0), (-1,-1) },
    };

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
        instance = this;

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
        bigRoomIndexes = new List<(int, int)>();
        bigRooms = new List<List<(int, int)>>();

        VisitCell(4, 5);
        GenerateDungeon();
    }
    void GenerateDungeon()
    {
        while (cellQueue.Count > 0)
        {
            (int, int) index = cellQueue.Dequeue();
            int rowIndex = index.Item1;
            int columnIndex = index.Item2;
            bool created = false;
            if (columnIndex > 1) created |= VisitCell(rowIndex, columnIndex - 1);
            if (columnIndex < 9) created |= VisitCell(rowIndex, columnIndex + 1);
            if (rowIndex > 2) created |= VisitCell(rowIndex - 1, columnIndex);
            if (rowIndex < 7) created |= VisitCell(rowIndex + 1, columnIndex);

            if (created == false)
            {
                endRooms.Add((rowIndex, columnIndex));
            }
        }
        if (floorPlanCount < minRooms)
        {
            SetupDungeon();
            return;
        }
        CleanEndRoomsList();
        SetUpVisuals();
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
        RoomManager.instance.SetupRooms(spawnedCells);
    }
    void CleanEndRoomsList()
    {
        endRooms.RemoveAll(item => bigRoomIndexes.Contains(item) || GetNeighbourCount(item.Item1, item.Item2) > 1);
    }

    void UpdateSpecialRoomVisuals()
    {
        foreach (var cell in spawnedCells)
        {
            if ((cell.rowIndex, cell.columnIndex) == itemRoomIndex)
            {
                cell.SetRoomSprite(item);
                cell.roomType = RoomType.Item;
            }
            if ((cell.rowIndex, cell.columnIndex) == shopRoomIndex)
            {
                cell.SetRoomSprite(shop);
                cell.roomType = RoomType.Shop;
            }
            if ((cell.rowIndex, cell.columnIndex) == bossRoomIndex)
            {
                cell.SetRoomSprite(boss);
                cell.roomType = RoomType.Boss;
            }
        }
    }
    (int, int) RandomEndRoom()
    {
        if (endRooms.Count == 0) return (-1, -1);
        int randomRoom = UnityEngine.Random.Range(0, endRooms.Count);
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
        try { up = floorPlan[rowIndex + 1, columnIndex]; } catch { }
        try { left = floorPlan[rowIndex, columnIndex - 1]; } catch { }
        try { right = floorPlan[rowIndex, columnIndex + 1]; } catch { }
        return left + right + up + down;
    }
    private bool VisitCell(int rowIndex, int columnIndex)
    {
        if (floorPlan[rowIndex, columnIndex] != 0 || GetNeighbourCount(rowIndex, columnIndex) > 1 || floorPlanCount > maxRooms || UnityEngine.Random.value < 0.5f)
        {
            return false;
        }
        if (Random.value < 0.3f && !(rowIndex == 4 && columnIndex == 5))
        {
            foreach (var shape in roomShapes.OrderBy(_ => UnityEngine.Random.value))
            {
                if (TryPlaceRoom(rowIndex, columnIndex, shape))
                {
                    return true;
                }
            }
        }
        cellQueue.Enqueue((rowIndex, columnIndex));
        floorPlan[rowIndex, columnIndex] = 1;
        floorPlanCount++;
        return true;
    }
    private bool TryPlaceRoom(int rowOrigin, int columnOrigin, (int, int)[] offsets)
    {
        List<(int, int)> currentRoomIndexes = new List<(int, int)>() { (rowOrigin, columnOrigin) };
        foreach (var offset in offsets)
        {
            int currentRowIndexChecked = rowOrigin + offset.Item1;
            int currentColumnIndexChecked = columnOrigin + offset.Item2;

            if (currentRowIndexChecked - 1 < 0 || currentRowIndexChecked + 1 >= floorPlan.Length)
            {
                return false;
            }
            if (currentColumnIndexChecked - 1 < 0 || currentColumnIndexChecked + 1 >= floorPlan.Length)
            {
                return false;
            }
            try
            {
                if (floorPlan[currentRowIndexChecked, currentColumnIndexChecked] != 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            if (currentRowIndexChecked == rowOrigin && currentColumnIndexChecked == columnOrigin) continue;
            if (currentColumnIndexChecked == 0) continue;

            currentRoomIndexes.Add((currentRowIndexChecked, currentColumnIndexChecked));

        }
        if (currentRoomIndexes.Count == 1) return false;

        var newList = new List<(int, int)>();

        foreach ((int, int) index in currentRoomIndexes)
        {
            floorPlan[index.Item1, index.Item2] = 1;
            floorPlanCount++;
            cellQueue.Enqueue(index);

            bigRoomIndexes.Add(index);
            newList.Add(index);
        }
        bigRooms.Add(newList);

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
        if (bigRoomIndexes.Contains((rowIndex, columnIndex)))
        {
            List<(int, int)> temp = null;
            foreach (List<(int, int)> indexesList in bigRooms)
            {
                if (indexesList.Contains((rowIndex, columnIndex)))
                {
                    temp = indexesList;
                    break;
                }
            }
            newCell.cellList= temp;
            switch (temp.Count)
            {
                case 4:
                    newCell.roomShape = RoomShape.TwoByTwo;
                    switch (temp.IndexOf((rowIndex, columnIndex)))
                    {
                        case 0:
                            newCell.typeOfCell = Cell.CellType.big0;
                            break;
                        case 1:
                            newCell.typeOfCell = Cell.CellType.big1;
                            break;
                        case 2:
                            newCell.typeOfCell = Cell.CellType.big2;
                            break;
                        case 3:
                            newCell.typeOfCell = Cell.CellType.big3;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    newCell.roomShape = RoomShape.LShape;
                    if (temp[1] == (temp[0].Item1 + 1, temp[0].Item2) && temp[2] == (temp[0].Item1 + 1, temp[0].Item2 + 1))
                    {
                        newCell.lShapeType = Cell.LShapeType.case0;
                        switch (temp.IndexOf((rowIndex, columnIndex)))
                        {
                            case 0:
                                newCell.typeOfCell = Cell.CellType.lshape0;
                                break;
                            case 1:
                                newCell.typeOfCell = Cell.CellType.lshape1;
                                break;
                            case 2:
                                newCell.typeOfCell = Cell.CellType.lshape2;
                                break;
                            default:
                                break;
                        }
                    }else if (temp[1] == (temp[0].Item1, temp[0].Item2+1) && temp[2] == (temp[0].Item1 + 1, temp[0].Item2))
                    {
                        newCell.lShapeType = Cell.LShapeType.case1;
                        switch (temp.IndexOf((rowIndex, columnIndex)))
                        {
                            case 0:
                                newCell.typeOfCell = Cell.CellType.lshape1;
                                break;
                            case 1:
                                newCell.typeOfCell = Cell.CellType.lshape0;
                                break;
                            case 2:
                                newCell.typeOfCell = Cell.CellType.lshape2;
                                break;
                            default:
                                break;
                        }
                        newCell.RotateSprite(-90);
                    }else if (temp[1] == (temp[0].Item1, temp[0].Item2 + 1) && temp[2] == (temp[0].Item1 + 1, temp[0].Item2+1))
                    {
                        newCell.lShapeType = Cell.LShapeType.case2;
                        switch (temp.IndexOf((rowIndex, columnIndex)))
                        {
                            case 0:
                                newCell.typeOfCell = Cell.CellType.lshape2;
                                break;
                            case 1:
                                newCell.typeOfCell = Cell.CellType.lshape1;
                                break;
                            case 2:
                                newCell.typeOfCell = Cell.CellType.lshape0;
                                break;
                            default:
                                break;
                        }
                        newCell.RotateSprite(-180);
                    }else
                    {
                        newCell.lShapeType = Cell.LShapeType.case3;
                        switch (temp.IndexOf((rowIndex, columnIndex)))
                        {
                            case 0:
                                newCell.typeOfCell = Cell.CellType.lshape2;
                                break;
                            case 1:
                                newCell.typeOfCell = Cell.CellType.lshape0;
                                break;
                            case 2:
                                newCell.typeOfCell = Cell.CellType.lshape1;
                                break;
                            default:
                                break;
                        }
                        newCell.RotateSprite(-270);
                    }

                    break;
                case 2:
                    if (temp[0].Item1 + 1 == temp[1].Item1 || temp[0].Item1 - 1 == temp[1].Item1)
                    {
                        newCell.roomShape = RoomShape.TwoByOne;
                        switch (temp.IndexOf((rowIndex, columnIndex)))
                        {
                            case 0:
                                newCell.typeOfCell = Cell.CellType.vertical0;
                                break;
                            case 1:
                                newCell.typeOfCell = Cell.CellType.vertical1;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        newCell.roomShape = RoomShape.OneByTwo;
                        switch (temp.IndexOf((rowIndex, columnIndex)))
                        {
                            case 0:
                                newCell.typeOfCell = Cell.CellType.horizontal0;
                                break;
                            case 1:
                                newCell.typeOfCell = Cell.CellType.horizontal1;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default: break;
            }
        }

        newCell.value = 1;
        newCell.rowIndex = rowIndex;
        newCell.columnIndex = columnIndex;

        spawnedCells.Add(newCell);
    }
    private void SetUpVisuals()
    {
        foreach (List<(int, int)> bigRoom in bigRooms)
        {
            bigRoom.Sort((x, y) =>
            {
                int result = x.Item1.CompareTo(y.Item1);
                return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
            });
        }

        int[] emptyArray = new int[numOfRows];
        for (int i = 0; i < floorPlan.GetLength(0); i++)
        {
            for (int j = 0; j < floorPlan.GetLength(1); j++)
            {
                emptyArray[i] |= floorPlan[i, j];
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
                if (floorPlan[i, j] != 0)
                {
                    SpawnRoom(i, j);
                }
                else
                {
                    SpawnEmpty();
                }
            }
        }
        UpdateLargeRoomVisuals();
    }
    void UpdateLargeRoomVisuals()
    {
        foreach (var cell in spawnedCells)
        {
            switch (cell.typeOfCell)
            {
                case Cell.CellType.big0:
                    cell.SetRoomSprite(big0);
                    break;
                case Cell.CellType.big1:
                    cell.SetRoomSprite(big1);
                    break;
                case Cell.CellType.big2:
                    cell.SetRoomSprite(big2);
                    break;
                case Cell.CellType.big3:
                    cell.SetRoomSprite(big3);
                    break;
                case Cell.CellType.lshape0:
                    cell.SetRoomSprite(lShape0);
                    break;
                case Cell.CellType.lshape1:
                    cell.SetRoomSprite(lShape1);
                    break;
                case Cell.CellType.lshape2:
                    cell.SetRoomSprite(lShape2);
                    break;
                case Cell.CellType.horizontal0:
                    cell.SetRoomSprite(horizontal0);
                    break;
                case Cell.CellType.horizontal1:
                    cell.SetRoomSprite(horizontal1);
                    break;
                case Cell.CellType.vertical0:
                    cell.SetRoomSprite(vertical0);
                    break;
                case Cell.CellType.vertical1:
                    cell.SetRoomSprite(vertical1);
                    break;
                default:
                    break;
            }
        }
    }
}