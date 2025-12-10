using UnityEngine;

public class GlorpResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        Debug.Log("Glorp feedback");
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(this);
    }
}