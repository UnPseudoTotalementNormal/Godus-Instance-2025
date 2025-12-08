using UnityEngine;

public class TreeResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        Debug.Log("Tree feedback");
    }

    protected override void OnExhaustedFeedback()
    {
        Debug.Log("Tree down !");
        Destroy(this);
    }
}
