using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CaserneUi : MonoBehaviour
{
    [SerializeField] VillageManager villageManager;
    [FormerlySerializedAs("CaserneUiPanel")] public GameObject caserneUiPanel;
    [FormerlySerializedAs("textInfo1")] public TMP_Text textInfoName;
    [FormerlySerializedAs("textInfo2")] public TMP_Text textInfoStats;
    [FormerlySerializedAs("textInfo3")] public TMP_Text textInfoCost;
    [SerializeField] Image unitSprite;

    private Vector2 mousePosition;
    private bool uIDisplay = false;
    
    [Space(10)]
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] List<UnitInfo> unitInfos;
    
    int selectedUnit;
    Vector2 unitSpawnPosition;
    void Start()
    {
        caserneUiPanel.SetActive(false);
        InputManager.instance.onMousePosition += GetMousePos;
        Caserne.onCaserneClick += OncaserneClick;
        InitButtons();
        OpenUnitInfo(0);
    }

    void OnDestroy()
    {
        Caserne.onCaserneClick -= OncaserneClick;
    }

    void OncaserneClick(Caserne _caserne)
    {
        caserneUiPanel.transform.position = _caserne.transform.position;
        caserneUiPanel.SetActive(true);
        uIDisplay = true;
        unitSpawnPosition = _caserne.transform.position;
    }

    private void GetMousePos(Vector2 _mousePosition)
    {
        mousePosition = _mousePosition;
    }
    
    public void CreateUnit()
    {
        Debug.Log("Creating new " + unitInfos[selectedUnit].unitName );
        if (CheckForUnitCost(unitInfos[selectedUnit].cost))
        {
            GameObject _newUnit = Instantiate(unitInfos[selectedUnit].unitPrefab);
            _newUnit.transform.position = unitSpawnPosition;
            RemoveUnitCost(unitInfos[selectedUnit].cost);
        }
    }
    public void Cross() 
    {
        uIDisplay = false;
        caserneUiPanel.SetActive(false);
    }

    void InitButtons()
    {
        for (int i = 0; i < unitInfos.Count; i++)
        {
            int _tempIndex = i;
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = unitInfos[i].unitName;
            foreach (Image _image in button.GetComponentsInChildren<Image>()) // Hacky way to not change the wrong image
            {
                if (_image.gameObject.CompareTag("buttonSprite"))
                    _image.sprite = unitInfos[i].unitSprite;
            }
            button.GetComponent<Button>().onClick.AddListener(delegate{OpenUnitInfo(_tempIndex);});
        }
    }

    void OpenUnitInfo(int _index)
    {
        selectedUnit = _index;
        textInfoName.text = unitInfos[_index].unitName;
        textInfoStats.text = "HP : " + unitInfos[_index].unitHp + " | ATK : " + unitInfos[_index].unitAtk;
        textInfoCost.text = GetUnitCost(_index);
        unitSprite.sprite = unitInfos[_index].unitSprite;
        
    }

    string GetUnitCost(int _index)
    {
        string _finalString = "";
        foreach (UnitCost _unitCost in unitInfos[_index].cost)
        {
            if (_unitCost.cost != 0)
            {
                _finalString += _unitCost.resourceType + " : " + _unitCost.cost + " | ";
            }
        }
        return _finalString;
    }

    bool CheckForUnitCost(List<UnitCost> _unitCost)
    {
        bool _canMake = true;

        foreach (UnitCost _cost in _unitCost)
        {
            var _resourceAmount = villageManager.GetResourceAmount(_cost.resourceType);
            if (_cost.cost > _resourceAmount)
            {
                Debug.Log("Not enough" + _cost.resourceType + " : " + _cost.cost + " needed but you got " + _resourceAmount);
                _canMake = false;
            }
        }
        return _canMake;
    }

    void RemoveUnitCost(List<UnitCost> _unitCost)
    {
        foreach (UnitCost _cost in _unitCost)
        {
            if (_cost.cost != 0)
                villageManager.AddResource(_cost.resourceType, -_cost.cost);
        }
    }
}

