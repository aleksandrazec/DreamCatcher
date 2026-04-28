using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private List<Room> createdRooms;
    //private List<GameObject> createdRooms;
    [Header("Offset Variables")]
    public float offsetX;
    public float offsetZ;
    [Header("Prefab References")]
    public Room roomPrefab;
    public Door doorPrefab;
    [Header("Scriptable Object References")]
    public RoomScriptable[] rooms;

    public static RoomManager instance;
    private List<(int,int)> bigRoomCells=new List<(int,int)> ();

    private void Awake()
    {
        instance = this;
        createdRooms = new List<Room>();
        //createdRooms = new List<GameObject>();
    }

    public void SetupRooms(List<Cell> spawnedCells)
    {
        for (int i = createdRooms.Count - 1; i >= 0; i--)
        {
            Destroy(createdRooms[i].gameObject);
        }
        createdRooms.Clear();
        bigRoomCells.Clear();
        foreach (var currentCell in spawnedCells)
        {
            if (bigRoomCells.Contains((currentCell.rowIndex, currentCell.columnIndex)))
            {
                continue;
            }
            var foundRoom = rooms.FirstOrDefault(x => x.roomShape == currentCell.roomShape);
            //var currentPosition = currentCell.transform.position;
            if (currentCell.roomShape != RoomShape.OneByOne)
            {
                SpawnBigRooms(currentCell);
            }
                var convertedPosition = CalculatePosition(currentCell);
            var rotation = Quaternion.identity;
            rotation.eulerAngles=CalculateRotation(currentCell);
            var spawnedRoom = Instantiate(roomPrefab, convertedPosition, rotation);
            var indexes = currentCell.cellList;
            if (currentCell.roomShape == RoomShape.OneByOne)
            {
                indexes.Add((currentCell.rowIndex, currentCell.columnIndex));
            }
            spawnedRoom.SetRoom(foundRoom.room,indexes, currentCell.roomType, currentCell.roomShape);
            //var spawnedRoom = Instantiate(foundRoom.room, convertedPosition, rotation);
            //var spawnedRoom = Instantiate(roomPrefab, convertedPosition, Quaternion.identity);
            //spawnedRoom.SetupRoom(currentCell, foundRoom);
            createdRooms.Add(spawnedRoom);
        }
    }
    private Vector3 CalculatePosition(Cell currentCell)
    {
        if (currentCell.roomShape == RoomShape.OneByOne)
        {
           return new Vector3(currentCell.rowIndex * offsetX, 0, currentCell.columnIndex * offsetZ);
        }
        if(currentCell.roomShape == RoomShape.OneByTwo)
        {
           return new Vector3(currentCell.rowIndex * offsetX, 0, currentCell.columnIndex * offsetZ+100);   
        }
        if(currentCell.roomShape == RoomShape.TwoByOne)
        {
            return new Vector3(currentCell.cellList[1].Item1*offsetX-100, 0, currentCell.columnIndex*offsetZ);
        }
        if(currentCell.roomShape == RoomShape.TwoByTwo)
        {
            return new Vector3(currentCell.rowIndex * offsetX+100, 0, currentCell.columnIndex * offsetZ+100);
        }
        if(currentCell.roomShape == RoomShape.LShape)
        {
            if (currentCell.lShapeType == Cell.LShapeType.case2)
            {
                return new Vector3(currentCell.cellList[1].Item1 * offsetX, 0, currentCell.cellList[1].Item2 * offsetZ);
            }
            return new Vector3(currentCell.cellList[1].Item1 * offsetX, 0, currentCell.columnIndex * offsetZ);
        }
        return new Vector3(2000,0,0);
    }
    private Vector3 CalculateRotation(Cell currentCell)
    {
        if (currentCell.roomShape == RoomShape.OneByTwo)
        {
            return new Vector3(0, 90, 0);
        }
        if (currentCell.roomShape == RoomShape.LShape)
        {
            switch (currentCell.lShapeType)
            {
                case Cell.LShapeType.case0:
                    return new Vector3(0, -180, 0);
                case Cell.LShapeType.case1:
                    return new Vector3(0, -90, 0);
                case Cell.LShapeType.case2:
                    return new Vector3(0, 0, 0);
                case Cell.LShapeType.case3:
                    return new Vector3(0, -270, 0);
                default:
                    break;
            }
        }
        return new Vector3(0, 0, 0);
    }
    private void SpawnBigRooms(Cell cell){
        foreach(var adjacentCell in cell.cellList)
        {
            bigRoomCells.Add(adjacentCell);
        }
    }
    private bool DoesTileMatchCell(int[] occupiedTilesRow, int[] occupiedTilesColumn, Cell cell)
    {
        Debug.Log("Cell list length" + cell.cellList.Count);
        Debug.Log("Occupied Tiles Length" + occupiedTilesColumn.Length);
        if (occupiedTilesColumn.Length != cell.cellList.Count)
        {
            return false;
        }
        if (cell.cellList.Count == 0)
        {
            cell.cellList.Add((cell.rowIndex, cell.columnIndex));
        }

        cell.cellList.Sort((x, y) =>
        {
            int result = x.Item1.CompareTo(y.Item1);
            return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
        });
        int minIndexRow = cell.cellList[0].Item1;
        int minIndexCol = cell.cellList[0].Item2;


        Debug.Log(minIndexRow);

        List<(int, int)> normalizedCell = new List<(int, int)>();
        foreach ((int, int) index in cell.cellList)
        {
            int row = index.Item1 - minIndexRow;
            int col = index.Item2 - minIndexCol;
            normalizedCell.Add((row, col));
        }
        normalizedCell.Sort((x, y) =>
        {
            int result = x.Item1.CompareTo(y.Item1);
            return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
        });
        List<(int, int)> occupiedTiles = new List<(int, int)>();
        for (int i = 0; i < occupiedTilesRow.Length; i++)
        {
            occupiedTiles[i] = (occupiedTilesRow[i], occupiedTilesColumn[i]);
        }
        occupiedTiles.Sort((x, y) =>
        {
            int result = x.Item1.CompareTo(y.Item1);
            return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
        });

        return normalizedCell.SequenceEqual(occupiedTiles);
    }
}
