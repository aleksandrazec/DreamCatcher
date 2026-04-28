using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/Room")]
public class RoomScriptable : ScriptableObject
{
    public RoomType roomType;
    public RoomShape roomShape;

    public int[] occupiedTilesRow;
    public int[] occupiedTilesColumn;
    public GameObject room;

}
