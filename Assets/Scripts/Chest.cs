using UnityEngine;

public class Chest : MonoBehaviour
{
    public Item item;
    public Camera cam;
    public void SetChest(Item item, Camera cam)
    {
        this.item = item;
        this.cam = cam;
    }
    public void OpenChest()
    {
        var itemObj =Instantiate(item,gameObject.transform.position,Quaternion.identity);
        itemObj.transform.position += new Vector3(0, 10, 0);
        itemObj.SetBillboardCamera(cam.gameObject);
        Destroy(gameObject);
    }

}
