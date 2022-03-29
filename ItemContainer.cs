using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using itemSystem;

public class itemContainer : MonoBehaviour
{
 
    public Item item01;  // 스크립터블 오브젝트 아이템 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,100.0f*Time.deltaTime,0); // 회전
    }
    void AddItemToInven()
    {
        if (!Inventory.IV.AddItem(this)) // IV : inventory Singleton
            Debug.Log("inventory full");
        else
            gameObject.SetActive(false); // 아이템을 안보이게 
    }

    // 충돌체크
    void OnTriggerEnter(Collider col)
    {
        // 상대방이 플레이어이면 .. 
        if (col.gameObject.tag == "Player")
            AddItemToInven();
    }
}
