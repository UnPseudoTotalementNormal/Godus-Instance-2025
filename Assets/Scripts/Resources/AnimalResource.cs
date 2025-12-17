using UnityEngine;

public class AnimalResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        
    }

    protected override void OnExhaustedFeedback()
    {
        
        Destroy(gameObject);
    }
}
