using System;
using UnityEngine;

public class EnableOnStart : MonoBehaviour
{
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
