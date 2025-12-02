
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    
    public void OnMiddleMouseButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            onMiddleMouseButtonPressStarted?.Invoke();
        }
        else if (_context.canceled)
        {
            onMiddleMouseButtonPressCanceled?.Invoke();
        }
    }
    
    public void OnMiddleMouseScroll(InputAction.CallbackContext _context)
    {
        float _scrollValue = _context.ReadValue<Vector2>().y;
        onMiddleMousseScroll?.Invoke(_scrollValue);
    }
    
    public void OnLeftMouseButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            onLeftMouseButtonPressStarted?.Invoke();
        }
        else if (_context.canceled)
        {
            onLeftMouseButtonPressCanceled?.Invoke();
        }
    }
    
    public void OnRightMouseButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            onRightMouseButtonPressStarted?.Invoke();
        }
        else if (_context.canceled)
        {
            onRightMouseButtonPressCanceled?.Invoke();
        }
    }
    
    public void OnEscapeButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            onEscapeButtonPressStarted?.Invoke();
        }
    }
}