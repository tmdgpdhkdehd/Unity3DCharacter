using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using itemSystem;

[CreateAssetMenu(fileName = "HP_Item", menuName = "HP_Item")]
public class itemHP : Item
{
    public float HP;

    public override void Use()
    {
        base.Use();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            player.GetComponent<playerCtrl>().takeDamage(-300); // 마이너스 값은 healing 

    }


}
