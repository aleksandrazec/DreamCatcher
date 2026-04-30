using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
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
    public GameController gameController;
    Dictionary<RoomShape, int> roomEnemyRatio = new Dictionary<RoomShape, int>
    {
        {RoomShape.OneByOne, 1},
        {RoomShape.OneByTwo, 6 },
        {RoomShape.TwoByOne, 6 },
        {RoomShape.LShape, 8},
        {RoomShape.TwoByTwo, 12}
    };
    Dictionary<RoomShape, int> walkRadius = new Dictionary<RoomShape, int>
    {
        {RoomShape.OneByOne, 50},
        {RoomShape.OneByTwo, 100 },
        {RoomShape.TwoByOne, 100 },
        {RoomShape.LShape, 100},
        {RoomShape.TwoByTwo, 100}
    };

    public void SetRoom(GameObject room, List<(int,int)> indexes,RoomType roomType, RoomShape roomShape, GameController gameController)
    {
        this.room = room;
        Instantiate(room, this.transform);
        this.indexes = indexes;
        this.roomType= roomType;
        this.roomShape = roomShape;
        doors=GetComponentsInChildren<Door>();
        activeDoors=new List<Door> ();
        enemyTypesArray = Enum.GetValues(typeof(EnemyType));
        this.gameController = gameController;
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
                        door.room = this;
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
                        //worm breaks the thing for some reason
                        enemy = Instantiate(ghostPrefab, this.transform);
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
                enemy.transform.position = FindSpawnPoint();
                enemy.enemyAI.room = this;
                spawnedEnemies.Add(enemy);
            }
        }
    }
    public void SetUpParamsForEnemies(Camera cam, GameObject playerObj, PlayerHealthSystem playerHealthSystem, PlayerMoneySystem playerMoneySystem)
    {
        foreach(Enemy enemy in spawnedEnemies)
        {
            enemy.enemyAI.playerTransform=playerObj.transform;
            enemy.enemyAI.playerHealthSystem = playerHealthSystem;
            enemy.enemyAI.playerMoneySystem = playerMoneySystem;
            enemy.enemyUI.cam = cam.transform;
        }
    }
    public void OpenDoors()
    {
        foreach(Door door in activeDoors)
        {
            door.OpenDoor();
        }
    }
    public Vector3 FindSpawnPoint()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius[roomShape];
        randomDirection = new Vector3(transform.position.x+randomDirection.x,transform.position.y, transform.position.z+randomDirection.z);
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius[roomShape], 1);
        return new Vector3(hit.position.x, 0, hit.position.z);
    }

    public void RemoveAllEnemies()
    {
        for(int i = 0; i < spawnedEnemies.Count; i++)
        {
            Destroy(spawnedEnemies[i].gameObject);
        }
        spawnedEnemies.Clear();
    }

    public (int, int) GetOffset(EdgeDirection direction)
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
