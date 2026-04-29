using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EdgeDirection
{
    Up, Down, Left, Right
}
public class Room : MonoBehaviour
{
    public GameObject room;
    public List<(int,int)> indexes;
    public RoomType roomType;
    public RoomShape roomShape;
    public Door[] doors;
    public void SetRoom(GameObject room, List<(int,int)> indexes,RoomType roomType, RoomShape roomShape)
    {
        this.room = room;
        Instantiate(room, this.transform);
        this.indexes = indexes;
        this.roomType= roomType;
        this.roomShape = roomShape;
        doors=GetComponentsInChildren<Door>();
        SetUpDoors();
    }
    public void SetUpDoors()
    {
        var floorPlan = MapGenerator.instance.getFloorPlan;
        switch (roomShape)
        {
            case RoomShape.OneByOne:
                SetUpOneByOne(floorPlan);
                break;
            case RoomShape.TwoByOne:
                SetUpTwoByOne(floorPlan);
                break;
            case RoomShape.OneByTwo:
                SetUpOneByTwo(floorPlan);
                break;
            case RoomShape.TwoByTwo:
                SetUpTwoByTwo(floorPlan);
                break;
            case RoomShape.LShape:
                SetUpLShape(floorPlan);
                break;
            default:
                break;
        }
    }
    public void SetUpOneByOne(int[,] floorPlan)
    {
        foreach (EdgeDirection d in Enum.GetValues(typeof(EdgeDirection)))
        {
            (int, int) neighborIndex = (indexes[0].Item1 + GetOffset(d).Item1, indexes[0].Item2 + GetOffset(d).Item2);
            if (neighborIndex.Item1 < 0 || neighborIndex.Item2 < 0 || neighborIndex.Item1 >= floorPlan.GetLength(0) || neighborIndex.Item2 >= floorPlan.GetLength(1))
            {
                continue;
            }
            if (floorPlan[neighborIndex.Item1, neighborIndex.Item2] != 1)
            {
                continue;
            } 
            foreach(Door door in doors)
            {
                if (door.direction == d)
                {
                    door.MakeDoorActive();
                    break;
                }
            }
        }
    }
    private void SetUpTwoByOne(int[,] floorPlan)
    {
        for (int i = 0; i < indexes.Count; i++) {
            foreach (EdgeDirection d in Enum.GetValues(typeof(EdgeDirection)))
            {
                (int, int) neighborIndex = (indexes[i].Item1 + GetOffset(d).Item1, indexes[i].Item2 + GetOffset(d).Item2);
                if (neighborIndex.Item1 < 0 || neighborIndex.Item2 < 0 || neighborIndex.Item1 >= floorPlan.GetLength(0) || neighborIndex.Item2 >= floorPlan.GetLength(1))
                {
                    continue;
                }
                if (floorPlan[neighborIndex.Item1, neighborIndex.Item2] != 1 || indexes.Contains(neighborIndex))
                {
                    continue;
                }
                foreach (Door door in doors)
                {
                    if (door.direction == d && door.subRoomIndex==i)
                    {
                        door.MakeDoorActive();
                        break;
                    }
                }
            }
        }
    }
    private void SetUpOneByTwo(int[,] floorPlan)
    {
        for (int i = 0; i < indexes.Count; i++)
        {
            foreach (EdgeDirection d in Enum.GetValues(typeof(EdgeDirection)))
            {
                (int, int) neighborIndex = (indexes[i].Item1 + GetOffset(d).Item1, indexes[i].Item2 + GetOffset(d).Item2);
                if (neighborIndex.Item1 < 0 || neighborIndex.Item2 < 0 || neighborIndex.Item1 >= floorPlan.GetLength(0) || neighborIndex.Item2 >= floorPlan.GetLength(1))
                {
                    continue;
                }
                if (floorPlan[neighborIndex.Item1, neighborIndex.Item2] != 1 || indexes.Contains(neighborIndex))
                {
                    continue;
                }
                foreach (Door door in doors)
                {
                    if (door.direction == d && door.subRoomIndex == i)
                    {
                        door.MakeDoorActive();
                        break;
                    }
                }
            }
        }
    }
    private void SetUpTwoByTwo(int[,] floorPlan)
    {
        for (int i = 0; i < indexes.Count; i++)
        {
            foreach (EdgeDirection d in Enum.GetValues(typeof(EdgeDirection)))
            {
                (int, int) neighborIndex = (indexes[i].Item1 + GetOffset(d).Item1, indexes[i].Item2 + GetOffset(d).Item2);
                if (neighborIndex.Item1 < 0 || neighborIndex.Item2 < 0 || neighborIndex.Item1 >= floorPlan.GetLength(0) || neighborIndex.Item2 >= floorPlan.GetLength(1))
                {
                    continue;
                }
                if (floorPlan[neighborIndex.Item1, neighborIndex.Item2] != 1 || indexes.Contains(neighborIndex))
                {
                    continue;
                }
                foreach (Door door in doors)
                {
                    if (door.direction == d && door.subRoomIndex == i)
                    {
                        door.MakeDoorActive();
                        break;
                    }
                }
            }
        }
    }
    private void SetUpLShape(int[,] floorPlan)
    {
        for (int i = 0; i < indexes.Count; i++)
        {
            foreach (EdgeDirection d in Enum.GetValues(typeof(EdgeDirection)))
            {
                (int, int) neighborIndex = (indexes[i].Item1 + GetOffset(d).Item1, indexes[i].Item2 + GetOffset(d).Item2);
                if (neighborIndex.Item1 < 0 || neighborIndex.Item2 < 0 || neighborIndex.Item1 >= floorPlan.GetLength(0) || neighborIndex.Item2 >= floorPlan.GetLength(1))
                {
                    continue;
                }
                if (floorPlan[neighborIndex.Item1, neighborIndex.Item2] != 1 || indexes.Contains(neighborIndex))
                {
                    continue;
                }
                foreach (Door door in doors)
                {
                    if (door.direction == d && door.subRoomIndex == i)
                    {
                        door.MakeDoorActive();
                        break;
                    }
                }
            }
        }
    }








    
    //public void SetupRoom(Cell currentCell, RoomScriptable room)
    //{
    //    var floorplan = MapGenerator.instance.getFloorPlan;
    //    var cellList = MapGenerator.instance.getSpawnedCells;

    //    switch (currentCell.roomShape)
    //    {
    //        case RoomShape.OneByOne:
    //            SetupOneByOne(currentCell, floorplan, cellList); 
    //            break;
    //        case RoomShape.TwoByOne:
    //            SetupTwoByOne(currentCell, floorplan, cellList);
    //            break;
    //        case RoomShape.OneByTwo:
    //            SetupOneByTwo(currentCell, floorplan, cellList); 
    //            break;
    //        case RoomShape.TwoByTwo:
    //            SetupTwoByTwo(currentCell, floorplan, cellList);
    //            break;
    //        case RoomShape.LShape:
    //            SetupLShape(currentCell, floorplan, cellList);
    //            break;
    //        default:
    //            break;
    //    }
    //}
    ////for each of these call each possible door to check if it should be active
    //public void SetupOneByOne(Cell cell, int[,] floorPlan, List<Cell> cellList)
    //{
    //    var currentCell=cell.cellList[0];
    //    TryActivateDoor(currentCell, EdgeDirection.Up, floorPlan, cellList, cell);
    //    TryActivateDoor(currentCell, EdgeDirection.Down, floorPlan, cellList, cell);
    //    TryActivateDoor(currentCell, EdgeDirection.Left, floorPlan, cellList, cell);
    //    TryActivateDoor(currentCell, EdgeDirection.Right, floorPlan, cellList, cell);
    //}
    //public void SetupTwoByOne(Cell cell, int[,] floorPlan, List<Cell> cellList)
    //{
    //    var cell0 = cell.cellList[0];
    //    var cell1 = cell.cellList[1];
    //    TryActivateDoor(cell0, EdgeDirection.Up, floorPlan, cellList, cell);
    //    TryActivateDoor(cell0, EdgeDirection.Left, floorPlan, cellList, cell);
    //    TryActivateDoor(cell0, EdgeDirection.Down, floorPlan, cellList, cell);

    //    TryActivateDoor(cell1, EdgeDirection.Up, floorPlan, cellList, cell);
    //    TryActivateDoor(cell1, EdgeDirection.Down, floorPlan, cellList, cell);
    //    TryActivateDoor(cell1, EdgeDirection.Right, floorPlan, cellList, cell);
    //}
    //public void SetupOneByTwo(Cell cell, int[,] floorPlan, List<Cell> cellList)
    //{
    //    var cell0 = cell.cellList[0];
    //    var cell1 = cell.cellList[1];
    //    TryActivateDoor(cell0, EdgeDirection.Up, floorPlan, cellList, cell);
    //    TryActivateDoor(cell0, EdgeDirection.Left, floorPlan, cellList, cell);
    //    TryActivateDoor(cell0, EdgeDirection.Right, floorPlan, cellList, cell);

    //    TryActivateDoor(cell1, EdgeDirection.Down, floorPlan, cellList, cell);
    //    TryActivateDoor(cell1, EdgeDirection.Left, floorPlan, cellList, cell);
    //    TryActivateDoor(cell1, EdgeDirection.Right, floorPlan, cellList, cell);
    //}
    //public void SetupTwoByTwo(Cell cell, int[,] floorPlan, List<Cell> cellList)
    //{
    //    var cell0 = cell.cellList[0];
    //    var cell1 = cell.cellList[1];
    //    var cell2 = cell.cellList[2];
    //    var cell3 = cell.cellList[3];

    //    TryActivateDoor(cell0, EdgeDirection.Up, floorPlan, cellList, cell);
    //    TryActivateDoor(cell1, EdgeDirection.Up, floorPlan, cellList, cell);

    //    TryActivateDoor(cell0, EdgeDirection.Left, floorPlan, cellList, cell);
    //    TryActivateDoor(cell2, EdgeDirection.Left, floorPlan, cellList, cell);

    //    TryActivateDoor(cell1, EdgeDirection.Right, floorPlan, cellList, cell);
    //    TryActivateDoor(cell3, EdgeDirection.Right, floorPlan, cellList, cell);

    //    TryActivateDoor(cell2, EdgeDirection.Down, floorPlan, cellList, cell);
    //    TryActivateDoor(cell3, EdgeDirection.Down, floorPlan, cellList, cell);
    //}
    //public void SetupLShape(Cell cell, int[,] floorplan, List<Cell> cellList)
    //{

    //    var cell0 = cell.cellList[0];
    //    var cell1 = cell.cellList[1];
    //    var cell2 = cell.cellList[2];

    //    if ((cell0.Item1, cell0.Item2+1) == cell1 && (cell0.Item1 + 1,cell0.Item2) == cell2)
    //    {
    //        TryActivateDoor(cell0, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Left, floorplan, cellList, cell);

    //        TryActivateDoor(cell1, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell1, EdgeDirection.Right, floorplan, cellList, cell);
    //        TryActivateDoor(cell1, EdgeDirection.Down, floorplan, cellList, cell);

    //        TryActivateDoor(cell2, EdgeDirection.Down, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Right, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Left, floorplan, cellList, cell);
    //    }
    //    else if((cell0.Item1, cell0.Item2 + 1) == cell1 && (cell1.Item1 + 1, cell1.Item2) == cell2)
    //    {
    //        TryActivateDoor(cell0, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Left, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Down, floorplan, cellList, cell);

    //        TryActivateDoor(cell1, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell1, EdgeDirection.Right, floorplan, cellList, cell);

    //        TryActivateDoor(cell2, EdgeDirection.Down, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Right, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Left, floorplan, cellList, cell);
    //    }
    //    else if ((cell0.Item1 + 1, cell0.Item2) == cell1)
    //    {
    //        TryActivateDoor(cell0, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Left, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Right, floorplan, cellList, cell);

    //        TryActivateDoor(cell1, EdgeDirection.Down, floorplan, cellList, cell);
    //        TryActivateDoor(cell1, EdgeDirection.Left, floorplan, cellList, cell);

    //        TryActivateDoor(cell2, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Down, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Right, floorplan, cellList, cell);
    //    }
    //    else if ((cell0.Item1 + 1,cell0.Item2) == cell2)
    //    {
    //        TryActivateDoor(cell0, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Left, floorplan, cellList, cell);
    //        TryActivateDoor(cell0, EdgeDirection.Right, floorplan, cellList, cell);

    //        TryActivateDoor(cell1, EdgeDirection.Up, floorplan, cellList, cell);
    //        TryActivateDoor(cell1, EdgeDirection.Down, floorplan, cellList, cell);
    //        TryActivateDoor(cell1, EdgeDirection.Left, floorplan, cellList, cell);

    //        TryActivateDoor(cell2, EdgeDirection.Down, floorplan, cellList, cell);
    //        TryActivateDoor(cell2, EdgeDirection.Right, floorplan, cellList, cell);
    //    }
    //}
    private void TryActivateDoor((int, int) fromIndex, EdgeDirection direction, int[,] floorplan, List<Cell> cellList, Cell currentCell)
    {
        (int, int) neighborIndex = (fromIndex.Item1 + GetOffset(direction).Item1, fromIndex.Item2 + GetOffset(direction).Item2);
        if (neighborIndex.Item1 < 0 || neighborIndex.Item2 < 0 || neighborIndex.Item1 > floorplan.GetLength(0) || neighborIndex.Item2 > floorplan.GetLength(1))
        {
            return;
        }
        if (floorplan[neighborIndex.Item1, neighborIndex.Item2] != 1) return;
        var foundCell = cellList.FirstOrDefault(x => x.cellList.Contains(neighborIndex));
        //somehow find the door and set it active
    }
    private (int, int) GetOffset(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                return (-1, 0);
            case EdgeDirection.Down:
                return (1, 0);
            case EdgeDirection.Right:
                return (0, 1);
            case EdgeDirection.Left:
                return (0, -1);
        }
        return (0, 0);
    }
}
