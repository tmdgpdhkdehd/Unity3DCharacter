using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using itemSystem;

public class Slot : MonoBehaviour
{

    public Stack<Item> itemStack;       // stack 
    public Text text;       // number of items
    public Sprite DefaultImg; //  default image [ no item ]

    private Image ItemImg;
    private bool isSlotUsing;     // empty slot ?

    public Item getCurItem() { return itemStack.Peek(); } // current item 
    public bool getItemMaxCnt(Item item) { return getCurItem().MaxCount > itemStack.Count; } // 아이템 겹침 최대치   
    public bool isUsing() { return isSlotUsing; } // empty slot ?
    public void SetSlotUsing(bool isSlotUsing) { this.isSlotUsing = isSlotUsing; }

    void Start()
    {
        // 스택 메모리 할당.
        itemStack = new Stack<Item>();

        // 맨 처음엔 슬롯이 비어있다.
        isSlotUsing = false;

   
        // calc font size
        float Size = text.gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        text.fontSize = (int)(Size * 0.5f);

        ItemImg = transform.GetChild(0).GetComponent<Image>();
    }

    public void AddItem(Item item)
    {
        // 아이템 stack 에 저장 
        itemStack.Push(item);
        UpdateInfo(true, item.DefaultImg);
    }

    // 아이템 사용.
    public void ItemUse()
    {
        if (!isSlotUsing)
            return;

        // item == 1 개 ?
        if (itemStack.Count == 1)
        {
            Item curItem01 = itemStack.Peek();
            curItem01.Use();

            itemStack.Clear(); // 1개 일 때 사용하면 0 이 되므로 ..
            
            UpdateInfo(false, DefaultImg);
            return;
        }

        Item curItem = itemStack.Pop();
        curItem.Use(); // 실제 체력, 마나 올려줌

        UpdateInfo(isSlotUsing, ItemImg.sprite);
    }

    // 슬롯에 대한 각종 정보 업데이트.
    public void UpdateInfo(bool isSlotUsing, Sprite sprite)
    {
        SetSlotUsing(isSlotUsing);                                          
        ItemImg.sprite = sprite;                                   
        text.text = itemStack.Count > 1 ? itemStack.Count.ToString() : "";   // text, number of items

        // to do list - save item using json                                          
    }
}
