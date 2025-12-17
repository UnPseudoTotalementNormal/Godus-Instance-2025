using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum CreditsRole
{
    ProjectManager,
    DirectorArtist,
    LeadDeveloper,
    Developer,
    Artist,
    MusicComposer,
    Tester,
    SpecialThanks,
}

public class Credits : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Credit Info")]
    [SerializeField] private string personName;
    [SerializeField] private CreditsRole role;
    [SerializeField] private string link;
    [SerializeField] private Sprite avatar;
    
    [Header("UI References")]
    [SerializeField] private GameObject overPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI roleText;
    [SerializeField] private Image avatarImage;
    
    private void Start()
    {
        nameText.text = personName;
        roleText.text = role.ToString().Replace("_", " ");
        avatarImage.sprite = avatar;
        overPanel.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        overPanel.SetActive(true);
        
        CanvasGroup canvasGroup = overPanel.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = overPanel.AddComponent<CanvasGroup>();
        
        // TODO : feedback avec dotween fade in du canvasGroup de `overPanel`
        // TODO : Mettre le preserveAspect à true sur l'Image avatarImage
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        CanvasGroup canvasGroup = overPanel.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = overPanel.AddComponent<CanvasGroup>();
        
        // TODO : feedback avec dotween fade out du canvasGroup de `overPanel` 
        overPanel.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(link);
    }
}