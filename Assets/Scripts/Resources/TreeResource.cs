using UnityEngine;

public class TreeResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(gameObject);
    }
}
