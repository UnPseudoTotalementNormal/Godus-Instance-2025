using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    [Header("Paramètres de croissance")]
    [SerializeField] private float growthDuration = 90f;
    [SerializeField] private Vector2 finalScale = new(2f, 2f);

    private Vector2 initialScale;
    private float growthTimer = 0f;
    public float growthSpeedMultiplier = 1f;
    public bool canBeCollected => growthTimer >= growthDuration;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (growthTimer < growthDuration)
        {
            growthTimer += Time.deltaTime * growthSpeedMultiplier;

            float _t = growthTimer / growthDuration;

            transform.localScale = Vector2.Lerp(initialScale, finalScale, _t);
        }
    }
    
    public void GrowFaster(float _multiplier)
    {
        growthSpeedMultiplier = _multiplier;
    }

}
