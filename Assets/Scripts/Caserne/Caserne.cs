using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Caserne : MonoBehaviour, IPointerClickHandler
{

    public static event Action<Caserne> OnCaserneClick;
    public void OnPointerClick(PointerEventData Point)
    {
        OnCaserneClick?.Invoke(this);
    }
}
