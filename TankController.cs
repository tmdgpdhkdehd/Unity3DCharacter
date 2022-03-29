using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tankController : MonoBehaviour
{
    public GameObject topCannonRoot = null;
    public GameObject top = null;
    public GameObject cannonRoot = null;
    public GameObject wheel01 = null;
    public GameObject wheel02 = null;
    public GameObject wheel03 = null;
    public GameObject wheel04 = null;
    public GameObject pointLightObject = null;

    public float speed = 5;                           // 이동 속도(m/s)
    public float rotSpeed = 120;                      // 회전 속도(각도/s)
    public float rotSpeedTop = 200;
    public float rotSpeedCannon = 200;

    public float bullet_power = 600;

    bool bCannonMov = false;

    public GameObject bullet;
    public GameObject missile;
    public GameObject muzzle;

    float curHP = 1000f;
    float maxHP = 1000f;

    float timePassedMissile = 0;

    // Start is called before the first frame update
    void Start()
    {
        // HP bar - 값 채우기 
        GameObject.Find("Canvas_Player_HP").transform.Find("ImageHP").GetComponent<Image>().fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            bCannonMov = true;
        if (Input.GetMouseButtonUp(0))
            bCannonMov = false;

           
        float front_Key = Input.GetAxis("Vertical");                  // 전후진
        float rot_Key = Input.GetAxis("Horizontal");                  // 좌우 회전 방향
    
        float Horizontal_Mouse = Input.GetAxis("Mouse X");
        float moveVertical_Mouse = Input.GetAxis("Mouse Y");

        float amtToMove = speed * Time.deltaTime;    // 거리
        float amtToRot = rotSpeed * Time.deltaTime;  // 회전     
        float amtToRotTop = rotSpeedTop * Time.deltaTime;
        float amtToRotCannon = rotSpeedCannon * Time.deltaTime;

        //Debug.LogFormat("{0}", amtToRotTop);

        // 탱크 전후진
        transform.Translate(Vector3.forward * front_Key * amtToMove);  
        // 탱크 회전
        transform.Rotate(0, rot_Key * amtToRot, 0);
        // 바퀴 회전 
        wheel01.transform.localRotation *= Quaternion.Euler(front_Key * amtToRot, 0, 0);
        wheel02.transform.localRotation *= Quaternion.Euler(front_Key * amtToRot, 0, 0);
        wheel03.transform.localRotation *= Quaternion.Euler(front_Key * amtToRot, 0, 0);
        wheel04.transform.localRotation *= Quaternion.Euler(front_Key * amtToRot, 0, 0);


        if (bCannonMov)
        {
            // 포탑(top) 회전
            //top.transform.localRotation *= Quaternion.Euler(0, Horizontal_Mouse * amtToRotTop, 0);
            topCannonRoot.transform.localRotation *= Quaternion.Euler(0, Horizontal_Mouse * amtToRotTop, 0);

            // 포신(cannonRoot): up, down )
            Quaternion qtDelta = Quaternion.Euler(0, 0, moveVertical_Mouse * amtToRotCannon);
            Quaternion qtOld = cannonRoot.transform.localRotation;
            Quaternion qtNow = qtOld * qtDelta;

            // 포신(cannonRoot) : 회전 각도 제약 걸기
            float zAngleLocal = qtNow.eulerAngles.z;
            //Debug.LogFormat("{0}", zAngleLocal);
            if (!(zAngleLocal > 20 && zAngleLocal < 340))
                // 포신(cannonRoot) : 회전
                cannonRoot.transform.localRotation *= Quaternion.Euler(0, 0, moveVertical_Mouse * amtToRotCannon);
       
        }

        // bullet 
        if (Input.GetButtonDown("Fire1"))
        {
            // bullet
            var myBullet = objectPool.getBullet();
            if (myBullet == null)
                return;
         
            myBullet.SetActive(true);
            myBullet.transform.position = muzzle.transform.position + 2 * muzzle.transform.forward;
            myBullet.transform.rotation = muzzle.transform.rotation;

            Rigidbody rb = myBullet.GetComponent<Rigidbody>();
            rb.AddForce(myBullet.transform.forward * bullet_power);

            // light on, off 
            Light pointLight = pointLightObject.transform.GetComponent<Light>();
            pointLight.intensity = 3;

            StartCoroutine("turnOffLight");
        }

        // missile
        timePassedMissile += Time.deltaTime;

        if ( timePassedMissile>1.2f &&
            Input.GetButtonDown("Fire2"))
        {
            timePassedMissile = 0;

            // missile
            GameObject[] enemyTanks = GameObject.FindGameObjectsWithTag("EnemyTank"); // 현재 scene 에 있는 탱크들 
            if (enemyTanks.Length > 0)
            {
                var myMissile = objectPool.getMissile(); // object pool 에서 가져오기
                myMissile.SetActive(true);
                myMissile.transform.position = muzzle.transform.position + 2 * muzzle.transform.forward;
                myMissile.transform.rotation = muzzle.transform.rotation;

                
                myMissile.GetComponent<homingMissile>().targetObj = enemyTanks[0];
                myMissile.SetActive(true);

                // light on, off 
                Light pointLight = pointLightObject.transform.GetComponent<Light>();
                pointLight.intensity = 3;

                StartCoroutine("turnOffLight");
            }

        }


    }

    IEnumerator turnOffLight()
    {
        Light pointLight = pointLightObject.transform.GetComponent<Light>();
        for (float i = 3f; i >= 0f; i -= 0.1f)
        {
            pointLight.intensity = i;
            yield return null;
        }
    }

    public void takeDamage(float amount)
    {
        curHP -= amount;
        
        if (curHP < 0) 
            curHP = 0;

        float fBarPos = curHP / maxHP;
        //Debug.LogFormat("{0}", fBarPos);

        // update HP bar
        GameObject.Find("Canvas_Player_HP").transform.Find("ImageHP").GetComponent<Image>().fillAmount = fBarPos;
    }

}
