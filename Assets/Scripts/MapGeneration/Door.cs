using UnityEngine;

public class Door : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public int subRoomIndex;
    public int subRoomCount;
    public EdgeDirection direction;
    public bool active = false;
    public bool open = false;
    public Room room;
    public GameObject path;
    public void MakeDoorActive()
    {
        meshRenderer.enabled = true;
        meshCollider.enabled = true;
        active = true;
        if (path != null)
        {
            path.SetActive(true);
        }
    }
    public void OpenDoor()
    {
        meshRenderer.enabled = false;
        meshCollider.enabled = false;
        if (path != null)
        {
            path.SetActive(true);
        }
        open = true;
    }
    public void CloseDoor()
    {
        meshRenderer.enabled = true;
        meshCollider.enabled = true;
        open = false;
    }
}
