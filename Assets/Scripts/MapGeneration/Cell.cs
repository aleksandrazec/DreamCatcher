using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum RoomType
{
    Regular,
    Item,
    Boss,
    Shop
}
public enum RoomShape
{
    OneByOne,
    OneByTwo,
    TwoByOne,
    TwoByTwo,
    LShape
}
public class Cell : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;
    public int value;
    public Image image;
    public CellType typeOfCell=CellType.single;
    public RoomType roomType=RoomType.Regular;
    public RoomShape roomShape=RoomShape.OneByOne;
    [SerializeField] public RectTransform rectTransform;
    public List<(int,int)> cellList=new List<(int, int)> ();
    public LShapeType lShapeType = LShapeType.notLShape;
    public enum LShapeType
    {
        notLShape,
        case0,
        case1,
        case2,
        case3
    }
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
