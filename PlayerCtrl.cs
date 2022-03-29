using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class playerCtrl : MonoBehaviour
{
    // 애니메이터
    Animator anim;

    float horiz, verti;

    // 컴포넌트
    Transform transComp;
    Rigidbody rigidBD;
    NavMeshAgent agent;

    Image HP_Bar;
    Image MP_Bar;
    GameObject invenUI;

    public float curHP = 3000f;
    public float maxHP = 3000f; 
    
    public float curMP = 3000f;
    public float maxMP = 3000f;

    enum _STATE_
    {
        _IDLE_,
        _RUN_,
        _ATTACK_,
        _DEAD_,
    };

    _STATE_ st = _STATE_._IDLE_;


    // 공격중 ??
    bool isDuringAttack = false;


    public bool isAttacking() {
        //Debug.LogFormat("isDuringAttack {0}", isDuringAttack);
        return isDuringAttack;  
    }

    public void setAttackEnd() {
        isDuringAttack = false;
        //Debug.Log("setAttackEnd");
    }

    float actDead = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        transComp = GetComponent<Transform>();
        rigidBD = GetComponent<Rigidbody>();
        HP_Bar = GameObject.Find("Canvas_PlayerHP").transform.Find("Image_HP").GetComponent<Image>();
        MP_Bar = GameObject.Find("Canvas_PlayerHP").transform.Find("Image_MP").GetComponent<Image>();

        invenUI = GameObject.Find("Canvas_PlayerHP").transform.Find("Inventory").gameObject;

        takeDamage(0); // health bar update 
        changeMP(0); // Mana Point bar update 
    }

    // Update is called once per frame
    void Update()
    {
        // toggle, 인벤토리, i key
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            if(invenUI.activeSelf)
                invenUI.SetActive(false);
            else
                invenUI.SetActive(true);
        }
            
        
        // 공격 시작 요청 
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(1))
        {
            isDuringAttack = true;
        }

        
        switch (st)
        {
            case _STATE_._IDLE_:
                {
                    // 클릭하여 이동 시작
                    RaycastHit hit;
                    if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
                    {
                        agent.isStopped = false;

                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider.gameObject.tag == "Enemy")
                            {
                                Vector3 vDir = hit.point - transform.position;
                                Vector3 vDestPos;
                                if (vDir.magnitude > 1.0f)
                                    // 때리기 좋은 위치, 적군 위치에서 1 떨어진 곳
                                    vDestPos = hit.point - vDir.normalized;
                                else
                                    // 이미 가까움 , 그냥 사용 
                                    vDestPos = hit.point;

                                agent.SetDestination(vDestPos);

                                Debug.Log("Enemy Click !!");
                            }
                            else
                            {
                                agent.SetDestination(hit.point);
                            }

                            anim.SetTrigger("run");
                            st = _STATE_._RUN_;
                        }
                    }

                    // attack 클릭
                    if (isDuringAttack)
                    {
                        agent.ResetPath();
                        agent.velocity = Vector3.zero;
                        agent.isStopped = true;

                        anim.SetTrigger("attack");
                        st = _STATE_._ATTACK_;
                    }    
                }
                break;
            case _STATE_._RUN_:
                {
                    // 중도에 위치 바꿀 경우
                    if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        { 
                            agent.SetDestination(hit.point);
                        }
                    }

                    // 클릭 위치에 도달했는지 .. 
                    if (!agent.pathPending)
                    {
                        if (agent.remainingDistance <= agent.stoppingDistance)
                        {
                            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                            {
                                anim.SetTrigger("idle");
                                st = _STATE_._IDLE_;
                            }
                        }
                    }

                    // 이동 중 공격 클릭 
                    if(isDuringAttack)
                    {
                        // 목표 지점을 현재로
                        agent.SetDestination(this.transform.position); 
                    }
                }
                break;
            case _STATE_._ATTACK_:
                {
                    if(isDuringAttack==false) // 애니 끝났으면 .. 
                    {
                        st = _STATE_._IDLE_;
                    }
                }
                break;

            case _STATE_._DEAD_:
                {
                    actDead += Time.deltaTime;
                    if (actDead > 2.2f)
                    {
                        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                        foreach (var enm in enemies)
                            enm.transform.parent.GetComponent<mutantCtrl>().returnToIdle();

                        transform.Find("Main Camera").parent = GameObject.Find("sceneMgr").transform;
                        this.gameObject.SetActive(false);
                    }
                }
                break;
        }

        // hp = 0, dead 
        if (st != _STATE_._DEAD_ && curHP == 0)
        {
            anim.SetTrigger("dead");
            st = _STATE_._DEAD_;
        }

        //Debug.LogFormat("{0}", st);

    }


    public void takeDamage(float amount)
    {

        curHP -= amount;

        float fBarPos = curHP / maxHP;
        // update HP bar
        HP_Bar.fillAmount = fBarPos;

        if (curHP <= 0)
        {
            curHP = 0;

            return;
        }
    }
    public void changeMP(float amount)
    {

        curMP += amount;

        float fBarPos = curMP / maxMP;
        // update HP bar
        MP_Bar.fillAmount = fBarPos;

        if (curMP <= 0)
        {
            curMP = 0;

            return;
        }
    }
}
