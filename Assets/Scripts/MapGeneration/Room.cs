using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public Cell.LShapeType lShapeType;
    public Door[] doors;
    public List<Door> activeDoors;
    [Header("Enemy Prefabs")]
    [SerializeField] private Enemy ghostPrefab;
    [SerializeField] private Enemy dressPrefab;
    [SerializeField] private Enemy batPrefab;
    [SerializeField] private Enemy wormPrefab;
    [Header("Chest Prefabs")]
    [SerializeField] private Chest chestPrefab;
    [Header("Item Prefabs")]
    [SerializeField] private Item healPrefab;
    [SerializeField] private Item maxHealthPrefab;
    [SerializeField] private Item noDashCooldownPrefab;
    [SerializeField] private Item invincibleDashPrefab;
    [SerializeField] private Item damageCooldownPrefab;
    
    private List<Item.Type> types;
    private Array enemyTypesArray;
    private Vector3[] shopPositions =
    {
        new Vector3(0,10,0),
        new Vector3(12,10,-12),
        new Vector3(-12,10,12),
    };
    [SerializeField] private LayerMask groundMask;
    private List<GameObject> spawnPoints;
    public List<Enemy> spawnedEnemies;
    public GameController gameController;
    private RoomInformation roomInformation;
    private MapGenerator mapGenerator;
    System.Random random = new System.Random();
    Dictionary<RoomShape, int> roomEnemyRatio = new Dictionary<RoomShape, int>
    {
        {RoomShape.OneByOne, 4},
        {RoomShape.OneByTwo, 6 },
        {RoomShape.TwoByOne, 6 },
        {RoomShape.LShape, 8},
        {RoomShape.TwoByTwo, 12}
    };
    Dictionary<RoomShape, int> walkRadius = new Dictionary<RoomShape, int>
    {
        {RoomShape.OneByOne, 50},
        {RoomShape.OneByTwo, 50 },
        {RoomShape.TwoByOne, 50 },
        {RoomShape.LShape, 50},
        {RoomShape.TwoByTwo, 50}
    };

    public void SetRoom(GameObject room, List<(int,int)> indexes,RoomType roomType, RoomShape roomShape, GameController gameController, List<Item.Type> types, RoomInformation roomInformation, Cell.LShapeType lShapeType)
    {
        this.room = room;
        Instantiate(room, this.transform);
        this.indexes = indexes;
        this.roomType= roomType;
        this.roomShape = roomShape;
        this.roomInformation=roomInformation;
        this.lShapeType=lShapeType;
        doors = GetComponentsInChildren<Door>();
        this.spawnPoints = roomInformation.spawnPoints;
        activeDoors=new List<Door> ();
        enemyTypesArray = Enum.GetValues(typeof(EnemyAI.EnemyType));
        this.gameController = gameController;
        this.types = types;
        GameObject[] mapGeneratorObject = GameObject.FindGameObjectsWithTag("MapGenerator");
        mapGenerator = mapGeneratorObject[0].GetComponent<MapGenerator>();
        SetUpDoors();
        SetUpBasedOnRoomType();
    }
    public void SetUpDoors()
    {
        var floorPlan = mapGenerator.getFloorPlan;
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
    public void SetUpBasedOnRoomType()
    {
        switch (roomType)
        {
            case RoomType.Regular:
                SetUpEnemies();
                break;
            case RoomType.Item:
                SetUpItemRoom();
                break;
            case RoomType.Shop:
                SetUpShopRoom();
                break;
            case RoomType.Boss:
                SetUpBoss();
                break;
            default:
                break;
        }
    }
    private void SetUpItemRoom()
    {
        var chest = Instantiate(chestPrefab, this.transform);
        var itemType = types[0];
        Item item=GetItemPrefab(itemType);
        chest.SetChest(item, gameController.cam);
    }
    private Item GetItemPrefab(Item.Type itemType)
    {
        switch (itemType)
        {
            case Item.Type.Heal:
                return healPrefab;
            case Item.Type.InvincibleDash:
                return invincibleDashPrefab;
            case Item.Type.DamageCooldown:
                return damageCooldownPrefab;
            case Item.Type.NoDashCooldown:
                return noDashCooldownPrefab;
            case Item.Type.MaxHealth:
                return maxHealthPrefab;
            default:
                return invincibleDashPrefab;
        }
    }
    private void SetUpShopRoom()
    {
        for(int i=0;i< types.Count; i++)
        {
            var itemType = types[i];
            var item = GetItemPrefab(itemType);
            var itemObj = Instantiate(item, this.transform);
            itemObj.transform.position += shopPositions[i];
            itemObj.SetCost(100);
            itemObj.SetBillboardCamera(gameController.cam.gameObject);
        }
    }

    private void SetUpBoss()
    {
        EnemyAI.EnemyType randomEnemy = (EnemyAI.EnemyType)enemyTypesArray.GetValue(random.Next(enemyTypesArray.Length));
        var enemy = Instantiate(GetEnemyPrefab(randomEnemy), this.transform);;
        enemy.enemyAI.room = this;
        enemy.MakeBoss(30, 300, new Vector3(2f,2f,2f), 2);
        spawnedEnemies.Add(enemy);
    }
    private bool ReturnNavMeshHit(Vector3 sourcePosition)
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(sourcePosition, out closestHit, 500, 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SetUpEnemies()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            EnemyAI.EnemyType randomEnemy = (EnemyAI.EnemyType)enemyTypesArray.GetValue(random.Next(enemyTypesArray.Length));
            if (lShapeType == Cell.LShapeType.case0)
            {
                while (randomEnemy == EnemyAI.EnemyType.ghost)
                {
                    randomEnemy = (EnemyAI.EnemyType)enemyTypesArray.GetValue(random.Next(enemyTypesArray.Length));
                }
            }
            var spawnPosition = new Vector3(spawnPoints[i].gameObject.transform.position.x + this.transform.position.x, this.transform.position.y, spawnPoints[i].gameObject.transform.position.z + this.transform.position.z);
            Vector3 position;
            if (ReturnNavMeshHit(spawnPosition)){
                NavMeshHit closestHit;
                NavMesh.SamplePosition(spawnPosition, out closestHit, 500, 1);
                position = closestHit.position;
            }
            else
            {
                position=this.transform.position;
            }
            var enemy = Instantiate(GetEnemyPrefab(randomEnemy), position, Quaternion.identity);
            enemy.transform.parent= this.transform;
            enemy.enemyAI.room = this;
            spawnedEnemies.Add(enemy);
        }
    }
    private Enemy GetEnemyPrefab(EnemyAI.EnemyType type)
    {
        switch (type)
        {
            case EnemyAI.EnemyType.ghost:
                return ghostPrefab;
            case EnemyAI.EnemyType.worm:
                //worm breaks the thing for some reason
                return wormPrefab;
            case EnemyAI.EnemyType.dress:
                return dressPrefab;
            case EnemyAI.EnemyType.bat:
                return batPrefab;
            default:
                return ghostPrefab;
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
