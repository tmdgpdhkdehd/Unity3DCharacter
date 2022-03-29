using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AIController : MonoBehaviour
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
    public GameObject muzzle;

    public GameObject fragExplode;

    public GameObject playerPos;
    public GameObject movPos;

    objectPool objectPoolScript;

    float curHP = 500f;
    float maxHP = 500f;

    enum _AI_STATE_
    {
        _NONE_,
        _MOVE_TO_WAY_POINT_,
        _ATTACK_,
        _IDLE_,
    }

    _AI_STATE_ aiState;

    // 공격 제어 , 일정 시간마다 공격 
    float actAttack = 0;

    // Start is called before the first frame update
    void Start()
    {
        aiState = _AI_STATE_._MOVE_TO_WAY_POINT_;
        // HP bar - 값 채우기 
        transform.Find("Canvas_HP").transform.Find("ImageHP").GetComponent<Image>().fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {

        float amtToMove = speed * Time.deltaTime;    // 거리
        float amtToRot = rotSpeed * Time.deltaTime;  // 회전     
        float amtToRotTop = rotSpeedTop * Time.deltaTime;
        float amtToRotCannon = rotSpeedCannon * Time.deltaTime;

        // HP bar - 플레이어를 바라보게하기 
        GameObject.Find("Canvas_HP").transform.LookAt(playerPos.transform, Vector3.up);

        switch (aiState)
        {
            case _AI_STATE_._MOVE_TO_WAY_POINT_:
                {
                    Vector3 vDir = movPos.transform.position - this.transform.position ;
                    float fDistance = vDir.magnitude;

                    if (fDistance <= 1.0f) // 도착
                        aiState = _AI_STATE_._ATTACK_; // 공격모드

                    // way point 쪽으로 서서히 회전 
                    Quaternion targetRot = Quaternion.LookRotation(vDir.normalized);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime*2.0f);

                    // 탱크 회전
                    //transform.Rotate(0, rot_Key * amtToRot, 0);

                    // 탱크 전후진
                    transform.Translate(Vector3.forward * amtToMove);
                    
                    // 바퀴 회전 
                    wheel01.transform.localRotation *= Quaternion.Euler(amtToRot, 0, 0);
                    wheel02.transform.localRotation *= Quaternion.Euler(amtToRot, 0, 0);
                    wheel03.transform.localRotation *= Quaternion.Euler(amtToRot, 0, 0);
                    wheel04.transform.localRotation *= Quaternion.Euler(amtToRot, 0, 0);
                }
                break;

            case _AI_STATE_._ATTACK_:
                {
                    Vector3 vDir = playerPos.transform.position - this.transform.position;
                    float fDistance = vDir.magnitude;

                    // 서서히 회전 player 바라보기
                    Quaternion targetRot = Quaternion.LookRotation(vDir.normalized);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 3.0f);

                    // 포탄 발사하기 
                    Vector3 vDeltaRot = transform.rotation.eulerAngles - targetRot.eulerAngles;

                    if(vDeltaRot.magnitude<0.3f)
                    {
                        // bullet 을 하나 고른다.
                        var myBullet = objectPool.getBullet();
                        if (myBullet == null)
                            return;

                        // active = true, pos, rot 세팅 
                        myBullet.SetActive(true);
                        myBullet.transform.position = muzzle.transform.position + 2 * muzzle.transform.forward;
                        myBullet.transform.rotation = muzzle.transform.rotation;

                        Rigidbody rb = myBullet.GetComponent<Rigidbody>();

                        bullet_power = 1000f * fDistance / 37.0f;  
                        bullet_power = Mathf.Max(300, bullet_power);
                        rb.AddForce(myBullet.transform.forward * bullet_power);

                        // light on, off 
                        Light pointLight = pointLightObject.GetComponent<Light>();
                        pointLight.intensity = 3;

                        StartCoroutine("turnOffLight");

                        aiState = _AI_STATE_._IDLE_;
                    }
                }
                break;

            case _AI_STATE_._IDLE_:
                {
                    actAttack += Time.deltaTime;
                    if(actAttack>3.0f)
                    {
                        actAttack = 0;
                        aiState = _AI_STATE_._ATTACK_; // 쉬었으니 다시 공격으로 .. 
                    }
                }
                break;
        }

        //Debug.LogFormat("{0}", amtToRotTop);

        if (bCannonMov)
        {
            float Horizontal_Mouse = 0;
            float moveVertical_Mouse = 0;

            // 포탑(top) 회전
            top.transform.localRotation *= Quaternion.Euler(0, Horizontal_Mouse * amtToRotTop, 0);

            // 포신(cannonRoot): up, down )
            Quaternion qtDelta = Quaternion.Euler(0, 0, moveVertical_Mouse * amtToRotCannon);
            Quaternion qtOld = cannonRoot.transform.localRotation;
            Quaternion qtNow = qtOld * qtDelta;

            // 포신(cannonRoot) : 회전 각도 제약 걸기
            float zAngleLocal = qtNow.eulerAngles.z;
            if (!(zAngleLocal > 20 && zAngleLocal < 340))
                // 포신(cannonRoot) : 회전
                cannonRoot.transform.localRotation *= Quaternion.Euler(0, 0, moveVertical_Mouse * amtToRotCannon);
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

        float fBarPos = curHP / maxHP;
        // update HP bar
        transform.Find("Canvas_HP").transform.Find("ImageHP").GetComponent<Image>().fillAmount = fBarPos;

        if (curHP <= 0)
        {
            curHP = 0;
            var myBullet = Instantiate(fragExplode, muzzle.transform.position, Quaternion.identity);
            
            // 탱크 소멸 
            Destroy(this.gameObject);

            return;
        }
    }
}
