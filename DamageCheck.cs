using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageCheck : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if(transform.root.GetComponent<playerCtrl>().isAttacking())
            {
                collision.gameObject.GetComponentInParent<mutantCtrl>().takeDamage(100);
                //Debug.Log("Enemy Hit !!");
            }

        }
    }
}
