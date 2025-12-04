using NUnit.Framework.Constraints;
using System;
using TileSystemSpace;
using UnityEditor.Rendering;
using UnityEngine;

public class PowerRange : MonoBehaviour
{
    bool Poweractivated = false;
    int Range = 0;
    int HeigtLimit = 0;


    private void Start()
    {
        InputManager.instance.onMousePosition += Test;
    }

    private void Test(Vector2 mousePosition)
    {
        Vector2Int newMousePosInt = Vector2Int.RoundToInt(mousePosition);
        Vector3 PosMouse=UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(newMousePosInt.x, newMousePosInt.y));

        Tile tile = TileSystem.instance.GetTile(new Vector2Int ((int)PosMouse.x,(int)PosMouse.y));


        if (tile !=null)
        {
            Debug.Log($"mouse position x : {(int)PosMouse.x}  -  mouse position y : {(int)PosMouse.y}");
        }
    }
}

