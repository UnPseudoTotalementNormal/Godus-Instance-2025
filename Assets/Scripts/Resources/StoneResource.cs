using UnityEngine;

public class StoneResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        Debug.Log("Stone feedback");
    }

    protected override void OnExhaustedFeedback()
    {
        Destroy(this);
    }
}
