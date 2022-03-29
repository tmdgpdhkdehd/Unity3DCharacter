using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    public ParticleSystem bulletParticle;

    Rigidbody rb;

    float time = 0;
 
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        time = 0;
    }

    private void OnEnable()
    {
        time = 0;

        // 물리를 켠다.
        rb.WakeUp();
    }
    private void OnDisable()
    {
        CancelInvoke();

        // 물리를 꺼준다.
        rb.Sleep();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 5.0f) // 너무 오래됨 
        { 
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint pt = collision.GetContact(0);

        if (collision.collider.gameObject.tag =="PlayerTank")
        {
            var myParticle = Instantiate(bulletParticle, pt.point + 2 * Vector3.up, Quaternion.identity); // 2 = 약간 위 
            collision.collider.gameObject.GetComponent<tankController>().takeDamage(100.0f);
        }
        else if (collision.collider.gameObject.tag == "EnemyTank")
        {
            var myParticle = Instantiate(bulletParticle, pt.point + 2 * Vector3.up, Quaternion.identity); // 2 = 약간 위 
            collision.collider.gameObject.GetComponent<AIController>().takeDamage(100.0f);
        }
        else if (collision.collider.gameObject.tag == "EnemySoldier")
        {
            var myParticle = Instantiate(bulletParticle, pt.point + 2 * Vector3.up, Quaternion.identity); // 2 = 약간 위 
            collision.collider.gameObject.GetComponent<ragDoll>().takeDamage(300.0f);
        }


        // active 끄기
        gameObject.SetActive(false);
    }

}
