using UnityEngine;

public class IronResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(gameObject);
    }
}
