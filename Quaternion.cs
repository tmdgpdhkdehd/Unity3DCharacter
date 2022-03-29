using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quaternion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angle = 30 * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(angle, new Vector3(1,0,1));
    }
}
