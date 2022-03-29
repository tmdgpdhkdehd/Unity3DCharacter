using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageCheckMutant : MonoBehaviour
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
        if (collision.gameObject.tag == "Player")
        {
            if (transform.root.GetComponent<mutantCtrl>().isAttacking())
            {
                collision.gameObject.GetComponentInParent<playerCtrl>().takeDamage(100);
                //Debug.Log("Player Hit !!");
            }

        }
    }
}
