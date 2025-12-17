using TMPro;
using UnityEngine;

public class StartMainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject textAnyKey;
    private bool mainMenuIsShownForTheFirstTime = false;
    
    void Start()
    {
        InputManager.instance.onAnyKeyPressStarted += ShowMainMenu;
    }
    
    private void ShowMainMenu()
    {
        if (!mainMenuIsShownForTheFirstTime)
        {
            textAnyKey.SetActive(false);
            mainMenu.SetActive(true);
            mainMenuIsShownForTheFirstTime = true;
        }
        
    }
}
