using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private List<Room> createdRooms;
    public List<Room> getCreatedRooms => createdRooms;
    //private List<GameObject> createdRooms;
    [Header("Offset Variables")]
    public float offsetX;
    public float offsetZ;
    [Header("Prefab References")]
    public Room roomPrefab;
    //public Door doorPrefab;
    [Header("Scriptable Object References")]
    public RoomScriptable[] rooms;

    public static RoomManager instance;
    private List<(int,int)> bigRoomCells=new List<(int,int)> ();
    private GameController gameController;
    private void Awake()
    {
        instance = this;
        createdRooms = new List<Room>();
        GameObject[] gameControllerObject= GameObject.FindGameObjectsWithTag("GameController");
        gameController = gameControllerObject[0].GetComponent<GameController>();
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
            RoomScriptable foundRoom;
            if (currentCell.lShapeType == Cell.LShapeType.notLShape)
            {
                foundRoom = rooms.FirstOrDefault(x => x.roomShape == currentCell.roomShape && x.roomType==currentCell.roomType);
            }
            else
            {
                foundRoom = rooms.FirstOrDefault(x => x.roomShape == currentCell.roomShape && x.lShapeType==currentCell.lShapeType);
            }
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
            spawnedRoom.SetRoom(foundRoom.room,indexes, currentCell.roomType, currentCell.roomShape, gameController);
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
        if (currentCell.roomShape == RoomShape.OneByTwo || currentCell.roomShape == RoomShape.TwoByTwo)
        {
            return new Vector3(0, 90, 0);
        }
        if (currentCell.roomShape == RoomShape.TwoByOne)
        {
            return new Vector3(0, 180, 0);
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
}
