using System;
using Feedback.Health;
using UnityEngine;

public abstract class ResourceComponent : MonoBehaviour
{
    public ResourceType resourceType;
    public float collectionDelay;
    public int collectionQuantity;
    [SerializeField] protected int collectionsLeft;
    public bool collectible;
    public Action callback;

    FeedbackHealthAi dmgFeedback;

    void Start()
    {
        dmgFeedback = GetComponent<FeedbackHealthAi>();
    }
    public void OnCollect()
    {
        OnCollectFeedback();
        dmgFeedback.CosmeticDamage(); // Temp implementation of feedback
        collectionsLeft--;
        if (collectionsLeft == 0)
            OnExhausted();
    }
    
    void OnExhausted()
    {
        callback.Invoke();
        OnExhaustedFeedback();
    }

    protected abstract void OnCollectFeedback(); // Use this for any resource specific feedback needed
    protected abstract void OnExhaustedFeedback();
}
