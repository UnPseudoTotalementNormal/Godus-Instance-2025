using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class CaserneUi : MonoBehaviour
{

    public GameObject CaserneUiPanel;
    public TMP_Text textInfo1;
    public TMP_Text textInfo2;
    public TMP_Text textInfo3;

    private Vector2 mousePosition;
    public UnityEngine.Camera mainCamera;
    private bool UIDisplay = false;

    void Start()
    {
        CaserneUiPanel.SetActive(false);
        InputManager.instance.onMousePosition += GetMousePos;
        Caserne.OnCaserneClick += OncaserneClick;
    }

    void OnDestroy()
    {
        Caserne.OnCaserneClick -= OncaserneClick;
    }

    void OncaserneClick(Caserne _caserne)
    {
        Vector3 _posMouse = GetMousePosOnGame();
        {
            CaserneUiPanel.transform.position = _caserne.transform.position;
            CaserneUiPanel.SetActive(true);
            UIDisplay = true;
        }
    }

    private void GetMousePos(Vector2 _mousePosition)
    {
        mousePosition = _mousePosition;
    }
    private Vector3 GetMousePosOnGame()
    {
        Vector2Int _newMousePosInt = Vector2Int.RoundToInt(mousePosition);
        Vector3 _posMouse = mainCamera
            .ScreenToWorldPoint(new Vector3(_newMousePosInt.x, _newMousePosInt.y));
        _posMouse += Vector3.one * 0.5f;
        return _posMouse;
    }

    public void Unit1()
    {
        textInfo1.text = "info1";
        textInfo2.text = "info1";
        textInfo3.text = "info1";
    }

    public void Unit2()
    {
        textInfo1.text = "info2";
        textInfo2.text = "info2";
        textInfo3.text = "info2";
    }

    public void Unit3()
    {
        textInfo1.text = "info3";
        textInfo2.text = "info3";
        textInfo3.text = "info3";
    }

    public void Unit4()
    {
        textInfo1.text = "info4";
        textInfo2.text = "info4";
        textInfo3.text = "info4";
    }
    public void CreateUnit()
    {
    }
    public void Cross() 
    {
        UIDisplay = false;
        CaserneUiPanel.SetActive(false);
    }
}

