using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class mutantCtrl : MonoBehaviour
{
    enum _STATE_
    {
        _NONE_,
        _IDLE_,
        _FOLLOW_,
        _ATTACK_,
        _DEAD_,
    };

    _STATE_ st = _STATE_._IDLE_;


    // 공격중 ??
    bool isDuringAttack = false;

    public bool isAttacking()
    {
        //Debug.LogFormat("isDuringAttack {0}, {1}", isDuringAttack, transform.root.name);
        return isDuringAttack;
    }

    //public void setAttackEnd()
    //{
    //    isDuringAttack = false;
    //    Debug.Log("setAttackEnd");
    //}


    Transform transComp;
    Rigidbody rigidBD;
    NavMeshAgent agent;

    Animator anim;

    float curHP = 500f;
    float maxHP = 500f;

    Vector3 lookatPos;
    Transform playerTransform;
    Transform canvasTransform;
    Transform cameraTransform;

    float distanceToPlayer = 1000000;
    
    public float distFollow = 6;
    public float attackRange = 2.0f;

    float actDead = 0;

    public void returnToIdle()
    {
        st = _STATE_._NONE_;
        anim.SetTrigger("returnToIdle");
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        transComp = GetComponent<Transform>();
        rigidBD = GetComponent<Rigidbody>();

        // HP bar - 값 채우기 
        transform.Find("Canvas_HP").transform.Find("Image_HP").GetComponent<Image>().fillAmount = 1f;


        playerTransform = GameObject.Find("Player").transform;
        canvasTransform = transform.Find("Canvas_HP").transform;

        cameraTransform = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame

    void Update()
    {
        // HP bar - 카메라를 바라보게하기 
        //          HP bar 가 쳐다볼 위치 = player 위치에서 캔버스 와 동일 높이
        lookatPos = cameraTransform.position;
        lookatPos.y = canvasTransform.position.y;
        canvasTransform.LookAt(lookatPos, Vector3.up);

        Vector3 vDir = playerTransform.position - transform.position;
        distanceToPlayer = vDir.magnitude;



        switch (st)
        {
            case _STATE_._FOLLOW_:
                {
                    
                    if (distanceToPlayer < attackRange)
                    {
                        st = _STATE_._ATTACK_;

                        agent.ResetPath();
                        agent.velocity = Vector3.zero;
                        agent.isStopped = true;
                        agent.SetDestination(transform.position);

                        anim.SetTrigger("attack");
                    }
                    else
                        agent.SetDestination(playerTransform.position);
                }
                break;


            case _STATE_._IDLE_:
                {
                    if (distanceToPlayer < distFollow)
                    {
                        st = _STATE_._FOLLOW_;
                        
                        agent.isStopped = false;
                        anim.SetTrigger("run");
                    }
                }
                break;

            case _STATE_._ATTACK_:
                {

                    if (distanceToPlayer > attackRange)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(playerTransform.position);
                        anim.SetTrigger("run");
                        st = _STATE_._FOLLOW_;
                    }

                    //Debug.LogFormat("{0},{1}", distanceToPlayer, attackRange);
                }
                break;

            case _STATE_._DEAD_:
                {
                    actDead += Time.deltaTime;
                    if(actDead>3.5f)
                        Destroy(this.gameObject);
                }
                break;

        }


        // hp = 0, dead 
        if (st != _STATE_._DEAD_ && curHP == 0)
        {
            anim.SetTrigger("dead");
            st = _STATE_._DEAD_;
        }

        if (st == _STATE_._ATTACK_)
        {
            transform.LookAt(playerTransform.position);
            isDuringAttack = true;
        }
        else
            isDuringAttack = false;

        //Debug.LogFormat("isDuringAttack {0}, {1}", isDuringAttack, transform.name);

    }

    public void takeDamage(float amount)
    {
        curHP -= amount;

        float fBarPos = curHP / maxHP;
        // update HP bar
        transform.Find("Canvas_HP").transform.Find("Image_HP").GetComponent<Image>().fillAmount = fBarPos;

        if (curHP <= 0)
        {
            curHP = 0;
            
            return;
        }
    }
}
