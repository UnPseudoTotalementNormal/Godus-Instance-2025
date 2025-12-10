using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MaterialInstancer : MonoBehaviour
{
    [SerializeField] private Renderer[] rendererToInstance;
    
    private void Awake()
    {
        foreach (Renderer _renderer in rendererToInstance)
        {
            _renderer.material = Instantiate(_renderer.material);
        }
    }
}
