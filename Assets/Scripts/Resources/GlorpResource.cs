using UnityEngine;

public class GlorpResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(gameObject);
    }
}