using UnityEngine;

public class IronResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        Debug.Log("Iron feedback");
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(this);
    }
}
