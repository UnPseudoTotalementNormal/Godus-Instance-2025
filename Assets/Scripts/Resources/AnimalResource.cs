using UnityEngine;

public class AnimalResource : ResourceComponent
{
    protected override void OnCollectFeedback()
    {
        Debug.Log("Animal Feedback");
    }

    protected override void OnExhaustedFeedback()
    {
        Debug.Log("Animal Feedback");
        Destroy(gameObject);
    }
}
