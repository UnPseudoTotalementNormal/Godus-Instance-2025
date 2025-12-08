using System;
using UnityEngine;

public abstract class ResourceComponent : MonoBehaviour
{
    public ResourceType resourceType;
    public float collectionDelay;
    public int collectionQuantity;
    [SerializeField] protected int collectionsLeft;

    public Action callback;
    
    public void OnCollect()
    {
        OnCollectFeedback();
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
