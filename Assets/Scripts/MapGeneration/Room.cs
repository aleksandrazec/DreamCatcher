using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static EnemyAI;

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
    public List<Door> activeDoors;
    [Header("Enemy Prefabs")]
    [SerializeField] private Enemy ghostPrefab;
    [SerializeField] private Enemy dressPrefab;
    [SerializeField] private Enemy batPrefab;
    [SerializeField] private Enemy wormPrefab;
    private Array enemyTypesArray;
    System.Random random = new System.Random();
    [SerializeField] private LayerMask groundMask;
    public List<Enemy> spawnedEnemies;
    Dictionary<RoomShape, int> roomEnemyRatio = new Dictionary<RoomShape, int>
    {
        {RoomShape.OneByOne, 6},
        {RoomShape.OneByTwo, 12 },
        {RoomShape.TwoByOne, 12 },
        {RoomShape.LShape, 18},
        {RoomShape.TwoByTwo, 24}
    };
    
    public void SetRoom(GameObject room, List<(int,int)> indexes,RoomType roomType, RoomShape roomShape)
    {
        this.room = room;
        Instantiate(room, this.transform);
        this.indexes = indexes;
        this.roomType= roomType;
        this.roomShape = roomShape;
        Debug.Log(roomShape);
        doors=GetComponentsInChildren<Door>();
        activeDoors=new List<Door> ();
        enemyTypesArray = Enum.GetValues(typeof(EnemyType));
        SetUpDoors();
        SetUpEnemies();
    }
    public void SetUpDoors()
    {
        var floorPlan = MapGenerator.instance.getFloorPlan;
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
                        activeDoors.Add(door);
                        break;
                    }
                }
            }
        }
    }
    public void SetUpEnemies()
    {
        if (roomType==RoomType.Regular)
        {
            for(int i = 0; i < roomEnemyRatio[roomShape]; i++)
            {
                EnemyType randomEnemy = (EnemyType) enemyTypesArray.GetValue(random.Next(enemyTypesArray.Length));
                Enemy enemy;
                switch (randomEnemy)
                {
                    case EnemyType.ghost:
                        enemy = Instantiate(ghostPrefab, this.transform);
                        break;
                    case EnemyType.worm:
                        enemy = Instantiate(wormPrefab, this.transform);
                        break;
                    case EnemyType.dress:
                        enemy = Instantiate(dressPrefab, this.transform);
                        break;
                    case EnemyType.bat:
                        enemy = Instantiate(batPrefab, this.transform);
                        break;
                    default:
                        enemy = Instantiate(ghostPrefab, this.transform);
                        break;
                }
                enemy.enemyAI.transform.position = FindSpawnPoint();                
                spawnedEnemies.Add(enemy);
            }
        }
    }
    public void SetUpParamsForEnemies(Camera cam, GameObject playerObj, PlayerHealthSystem playerHealthSystem)
    {
        foreach(Enemy enemy in spawnedEnemies)
        {
            enemy.enemyAI.playerTransform=playerObj.transform;
            enemy.enemyAI.playerHealthSystem = playerHealthSystem;
            enemy.enemyUI.cam = cam.transform;
        }
    }
  
    public Vector3 FindSpawnPoint()
    {
        float randomX = UnityEngine.Random.Range(-20, 20);
        float randomZ = UnityEngine.Random.Range(-20, 20);
        return new Vector3(transform.position.x+ randomX, transform.position.y, transform.position.z+randomZ); 
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
