using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string link;

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(link);
    }
}