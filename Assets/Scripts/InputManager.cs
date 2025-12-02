
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event Action onMiddleMouseButtonPressStarted;
    public event Action onMiddleMouseButtonPressCanceled;

    public event Action<float> onMiddleMousseScroll;
    
    public event Action onLeftMouseButtonPressStarted;
    public event Action onLeftMouseButtonPressCanceled;
    
    public event Action onRightMouseButtonPressStarted;
    public event Action onRightMouseButtonPressCanceled;
    
    public event Action onEscapeButtonPressStarted;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void OnMiddleMouseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onMiddleMouseButtonPressStarted?.Invoke();
        }
        else if (context.canceled)
        {
            onMiddleMouseButtonPressCanceled?.Invoke();
        }
    }
    
    public void OnMiddleMouseScroll(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        onMiddleMousseScroll?.Invoke(scrollValue);
    }
    
    public void OnLeftMouseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onLeftMouseButtonPressStarted?.Invoke();
        }
        else if (context.canceled)
        {
            onLeftMouseButtonPressCanceled?.Invoke();
        }
    }
    
    public void OnRightMouseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onRightMouseButtonPressStarted?.Invoke();
        }
        else if (context.canceled)
        {
            onRightMouseButtonPressCanceled?.Invoke();
        }
    }
    
    public void OnEscapeButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onEscapeButtonPressStarted?.Invoke();
        }
    }
}