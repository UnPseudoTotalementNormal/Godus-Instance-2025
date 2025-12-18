using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    
    private CanvasGroup overPanelCanvasGroup;
    private Tween fadeTween;
    
    private void Start()
    {
        nameText.text = personName;
        roleText.text = FormatRoleText(role.ToString());
        avatarImage.sprite = avatar;
        avatarImage.preserveAspect = true;
        
        // Get or add CanvasGroup component
        overPanelCanvasGroup = overPanel.GetComponent<CanvasGroup>();
        if (!overPanelCanvasGroup)
            overPanelCanvasGroup = overPanel.AddComponent<CanvasGroup>();
        
        overPanelCanvasGroup.alpha = 0;
        overPanel.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        fadeTween?.Kill();
        
        overPanel.SetActive(true);
        overPanelCanvasGroup.alpha = 0;
        
        fadeTween = overPanelCanvasGroup.DOFade(1, fadeDuration).SetEase(Ease.OutQuad).SetAutoKill(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        fadeTween?.Kill();
        
        fadeTween = overPanelCanvasGroup.DOFade(0, fadeDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => overPanel.SetActive(false));
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(link);
    }
    
    private void OnDestroy()
    {
        fadeTween?.Kill();
    }
    
    private string FormatRoleText(string roleText)
    {
        roleText = roleText.Replace("_", " ");
        
        string result = "";
        for (int i = 0; i < roleText.Length; i++)
        {
            if (i > 0 && char.IsUpper(roleText[i]) && !char.IsWhiteSpace(roleText[i - 1]))
            {
                result += " ";
            }
            result += roleText[i];
        }
        
        return result;
    }
}