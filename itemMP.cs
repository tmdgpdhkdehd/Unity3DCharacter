using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using itemSystem;

[CreateAssetMenu(fileName = "MP_Item", menuName = "MP_Item")]
public class itemMP : Item
{
    public float MP;
    
    public override void Use()
    {
        base.Use();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            player.GetComponent<playerCtrl>().changeMP(MP); // 마나 회복  

    }
}
