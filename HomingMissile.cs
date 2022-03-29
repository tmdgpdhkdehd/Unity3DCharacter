using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class homingMissile : MonoBehaviour
{
    float time = 0;

    public ParticleSystem bulletParticle;

    public GameObject targetObj;

    Rigidbody rb;

    Vector3 vDirToTarget_Pre;

    float velocity = 20.0f;
    float accel = 15.0f;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();        
    }
    // Start is called before the first frame update
    void Start()
    {
        vDirToTarget_Pre = targetObj.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vDirToTarget;
        if (targetObj != null)
            vDirToTarget = targetObj.transform.position - transform.position;
        else              
            vDirToTarget = vDirToTarget_Pre;  

        vDirToTarget_Pre = vDirToTarget;

            // 목표물에 도착함 
        if (vDirToTarget.magnitude < 1.0) return;

        // 내적(dot)을 통해 각도를 구함. (Acos로 나온 각도는 방향을 알 수가 없음)
        //float dot = Vector3.Dot(transform.forward, vDirToTarget.normalized);
        //if (dot < 1.0f)
        {
            //float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            //// 외적을 통해 각도의 방향을 판별.
            //Vector3 cross = Vector3.Cross(transform.forward, vDirToTarget.normalized);
            //// 외적 결과 값에 따라 각도 반영
            //float deltaAngle = 0.0f;
            //if (cross.y < 0)
            //{
            //    deltaAngle = -2.0f * angle * Time.deltaTime;
            //}
            //else
            //{
            //    deltaAngle = 2.0f * angle * Time.deltaTime;
            //}

            // 방법 #2
            Vector3 vDirInterpolate =
                transform.position + transform.forward + 0.05f * Time.deltaTime * (vDirToTarget - transform.forward);

            transform.LookAt(vDirInterpolate);

            // 무조건 앞 방향 이동 
            velocity += accel * Time.deltaTime; // 가속 
            Vector3 vMov = velocity * Time.deltaTime * transform.forward;
            transform.Translate(vMov.x, vMov.y, vMov.z, Space.World);
        }
     

        time += Time.deltaTime;
        if (time > 10.0f)  // 너무 오래 지나면 소멸 
        {
            //Destroy(this.gameObject);
            gameObject.SetActive(false);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint pt = collision.GetContact(0);
        var myParticle = Instantiate(bulletParticle, pt.point + 2 * Vector3.up, Quaternion.identity); // 2 = 약간 위 
   
        if (collision.collider.gameObject.tag == "EnemyTank")
        {
            collision.collider.gameObject.GetComponent<AIController>().takeDamage(100.0f);
        }

        //Destroy(this.gameObject); // 충돌하면 소멸 
        gameObject.SetActive(false);

    }


    private void OnEnable()
    {
        // 물리를 켠다.
        rb.WakeUp();
    }
    private void OnDisable()
    {
        CancelInvoke();

        // 물리를 꺼준다.
        rb.Sleep();
        rb.velocity = Vector3.zero;
        
        // 변수 초기화 
        time = 0;
        velocity = 20.0f; // 초기 속도 
    }
}
