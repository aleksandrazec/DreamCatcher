using UnityEngine;

public class ChangingRoom : MonoBehaviour
{
    [SerializeField] private Door door;
    private void OnTriggerEnter(Collider other)
    {
        door.room.gameController.ChangePlayerPosition((door.room.indexes[door.subRoomIndex].Item1+door.room.GetOffset(door.direction).Item1, door.room.indexes[door.subRoomIndex].Item2 + door.room.GetOffset(door.direction).Item2));
    }
}
