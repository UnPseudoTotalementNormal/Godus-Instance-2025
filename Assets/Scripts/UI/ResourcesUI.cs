using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [Header("Meat")]
    [SerializeField] private TMP_Text meatValueText;
    [SerializeField] private TMP_Text meatMaxText;
    
    [Header("Wood")]
    [SerializeField] private TMP_Text woodValueText;
    [SerializeField] private TMP_Text woodMaxText;

    [Header("Stone")]
    [SerializeField] private TMP_Text stoneValueText;
    [SerializeField] private TMP_Text stoneMaxText;

    [Header("Iron")]
    [SerializeField] private TMP_Text ironValueText;
    [SerializeField] private TMP_Text ironMaxText;

    [Header("Glorp")]
    [SerializeField] private TMP_Text glorpValueText;
    [SerializeField] private TMP_Text glorpMaxText;

    private void OnEnable()
    {
        GameEvents.onResourceValueRefreshed     += OnResourceValueRefreshed;
        GameEvents.onResourceMaxValueRefreshed  += OnResourceMaxValueRefreshed;
    }

    private void OnDisable()
    {
        GameEvents.onResourceValueRefreshed     -= OnResourceValueRefreshed;
        GameEvents.onResourceMaxValueRefreshed  -= OnResourceMaxValueRefreshed;
    }

    private void OnResourceValueRefreshed(ResourceType _type, int _value)
    {
        switch (_type)
        {
            case ResourceType.Meat:
                meatValueText.text = _value + " /";
                break;
            
            case ResourceType.Wood:
                woodValueText.text = _value + " /";
                break;
            
            case ResourceType.Stone:
                stoneValueText.text = _value + " /";
                break;
            
            case ResourceType.Iron:
                ironValueText.text = _value + " /";
                break;
            
            case ResourceType.Glorp:
                glorpValueText.text = _value + " /";
                break;
        }
    }

    private void OnResourceMaxValueRefreshed(ResourceType _type, int _maxValue)
    {
        switch (_type)
        {
            case ResourceType.Meat:
                meatMaxText.text = _maxValue.ToString();
                break;
            
            case ResourceType.Wood:
                woodMaxText.text = _maxValue.ToString();
                break;
            
            case ResourceType.Stone:
                stoneMaxText.text = _maxValue.ToString();
                break;
            
            case ResourceType.Iron:
                ironMaxText.text = _maxValue.ToString();
                break;
            
            case ResourceType.Glorp:
                glorpMaxText.text = _maxValue.ToString();
                break;
        }
    }
}
