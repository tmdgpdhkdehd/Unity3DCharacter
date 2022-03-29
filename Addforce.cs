using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addforce : MonoBehaviour
{
    Rigidbody rb;
    float act = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        // 힘을 가한다.
        if (act == 0)
        {
            Vector3 vDir;
            vDir.x = (float)Random.Range(-10, 10);
            vDir.y = (float)Random.Range(0, 20);
            vDir.z = (float)Random.Range(-10, 10);
            rb.AddForce(vDir.normalized * (float)Random.Range(10, 2000));
        }

        act += Time.deltaTime;
        if (act > 3.0) 
            Destroy(this.gameObject);

    }
}
