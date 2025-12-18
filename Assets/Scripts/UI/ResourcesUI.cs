using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [Header("Meat")]
    [SerializeField] private TMP_Text meatValueText;
    
    [Header("Wood")]
    [SerializeField] private TMP_Text woodValueText;

    [Header("Stone")]
    [SerializeField] private TMP_Text stoneValueText;

    [Header("Iron")]
    [SerializeField] private TMP_Text ironValueText;

    [Header("Glorp")]
    [SerializeField] private TMP_Text glorpValueText;

    private void OnEnable()
    {
        GameEvents.onResourceValueRefreshed     += OnResourceValueRefreshed;
    }

    private void OnDisable()
    {
        GameEvents.onResourceValueRefreshed     -= OnResourceValueRefreshed;
    }

    private void OnResourceValueRefreshed(ResourceType _type, int _value, int _max)
    {
        switch (_type)
        {
            case ResourceType.Meat:
                meatValueText.text = _value + " / " + _max;
                break;
            
            case ResourceType.Wood:
                woodValueText.text = _value + " / " + _max;
                break;
            
            case ResourceType.Stone:
                stoneValueText.text = _value + " / " + _max;
                break;
            
            case ResourceType.Iron:
                ironValueText.text = _value + " / " + _max;
                break;
            
            case ResourceType.Glorp:
                glorpValueText.text = _value + " / " + _max;
                break;
        }
    }
}
