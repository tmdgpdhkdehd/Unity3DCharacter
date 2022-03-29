using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elementAdd : MonoBehaviour
{
    [SerializeField]
    private GameObject element = null;

    [SerializeField]
    private Transform content = null;

    public void AddElement()
    {
        var instance = Instantiate(element);
        instance.transform.SetParent(content);
    }
}
