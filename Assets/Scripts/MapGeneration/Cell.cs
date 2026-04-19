using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;
    public int value;
    public Image image;
    public CellType typeOfCell=CellType.single;
    [SerializeField] public RectTransform rectTransform;
    public enum CellType{
        single,
        big0,
        big1,
        big2,
        big3,
        horizontal0,
        horizontal1,
        vertical0,
        vertical1,
        lshape0,
        lshape1,
        lshape2
    }
    public void SetRoomSprite(Sprite icon)
    {
        image.sprite = icon;
    }
    public void RotateSprite(float angle)
    {
        rectTransform.Rotate(new Vector3(0, 0, angle));
    }
}
