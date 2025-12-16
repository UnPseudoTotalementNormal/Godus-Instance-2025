using System;
using TileSystemSpace;
using TMPro;
using UnityEngine;

public class CamerBoundSize : MonoBehaviour
{
   public Collider2D boundsCollider;
   public GameObject BoundBox;

   private void Start()
   {
      var vector3 = boundsCollider.bounds.size;
      vector3.x = TileSystem.instance.GetSize().x;
      vector3.y = TileSystem.instance.GetSize().y;
      vector3.z = 1;
      BoundBox.transform.localScale = vector3;
   }
}
