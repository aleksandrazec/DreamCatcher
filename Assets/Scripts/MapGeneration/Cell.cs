using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;
    public int value;
    public Image image;
    public void SetSpecialRoomSprite(Sprite icon)
    {
        image.sprite = icon;
    }

}
