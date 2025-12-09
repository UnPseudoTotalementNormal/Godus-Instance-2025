using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Caserne : MonoBehaviour, IPointerClickHandler
{

    public static event Action<Caserne> onCaserneClick;
    public void OnPointerClick(PointerEventData _point)
    {
        onCaserneClick?.Invoke(this);
    }
}
