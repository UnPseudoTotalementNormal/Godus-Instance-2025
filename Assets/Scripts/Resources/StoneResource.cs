using UnityEngine;

public class StoneResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(gameObject);
    }
}
