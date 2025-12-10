using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    [Header("Paramètres de croissance")]
    public float growthDuration = 90f; // Temps total de croissance
    public Vector2 finalScale = new(2f, 2f); // Taille finale

    private Vector2 initialScale;
    private float growthTimer = 0f;
    public bool canBeCollected => growthTimer >= growthDuration;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (growthTimer < growthDuration)
        {
            growthTimer += Time.deltaTime;

            float _t = growthTimer / growthDuration;

            transform.localScale = Vector2.Lerp(initialScale, finalScale, _t);
        }
    }
}
