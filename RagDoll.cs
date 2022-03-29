using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ragDoll : MonoBehaviour
{
    float act = 0;
    Animator anim;

    private Rigidbody[] bodyparts;
    private CapsuleCollider coll;

    public GameObject playerPos;

    float curHP = 500f;
    float maxHP = 500f;

    // Start is called before the first frame update
    void Start()
    {
        // HP bar - 값 채우기 
        transform.Find("Canvas_HP").transform.Find("ImageHP").GetComponent<Image>().fillAmount = 1f;


        anim = gameObject.GetComponent<Animator>();
        coll = gameObject.GetComponent<CapsuleCollider>();

        bodyparts = GetComponentsInChildren<Rigidbody>();

        // isKinematic 켜 놓는다.
        foreach (Rigidbody rb in bodyparts)
        {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // HP bar - 플레이어를 바라보게하기 
        GameObject.Find("Canvas_HP").transform.LookAt(playerPos.transform, Vector3.up);

    }

    public void takeDamage(float amount)
    {
        curHP -= amount;

        float fBarPos = curHP / maxHP;
        // update HP bar
        transform.Find("Canvas_HP").transform.Find("ImageHP").GetComponent<Image>().fillAmount = fBarPos;

        if (curHP <= 0)
        {
            // 애니메이터 끈다.
            anim.enabled = false;
            coll.enabled = false;

            // isKinematic 끈다.
            foreach (Rigidbody rb in bodyparts)
            {
                rb.isKinematic = false;
            }

            this.enabled = false;

            return;
        }
    }
}
