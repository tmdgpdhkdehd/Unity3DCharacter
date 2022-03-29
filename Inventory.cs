using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using itemSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory IV;

    public List<GameObject> SlotList;    
    public RectTransform InvenRect;     
    public GameObject OriginSlot;       

    public float slotSize;              
    public float slotGap;               
    public float slotCountX;            
    public float slotCountY;            

    private float InvenWidth;           
    private float InvenHeight;          
                                        

    void Awake()
    {
        IV = this;

        // inventory image size , width , height
        InvenWidth = (slotCountX * slotSize) + (slotCountX * slotGap) + slotGap;
        InvenHeight = (slotCountY * slotSize) + (slotCountY * slotGap) + slotGap;

        // inventory size
        InvenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, InvenWidth); 
        InvenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InvenHeight);  

        // 가로X * 세로Y 
        for (int y = 0; y < slotCountY; y++)
        {
            for (int x = 0; x < slotCountX; x++)
            {
                // instantiate 
                GameObject slot = Instantiate(OriginSlot) ;
                // slot RectTransform
                RectTransform slotRect = slot.GetComponent<RectTransform>();
                // tranparent image (child) RectTransform 
                RectTransform item = slot.transform.GetChild(0).GetComponent<RectTransform>();

                slot.name = "slot_" + y + "_" + x;   // slot name
                slot.transform.SetParent(transform); // slot parent => Inventory

                // set slot position
                slotRect.localPosition = new Vector3((slotSize * x) + (slotGap * (x + 1)),
                                                   -((slotSize * y) + (slotGap * (y + 1))),
                                                      0);

                // slot size
                slotRect.localScale = Vector3.one;
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize); // width
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize);   // height

                // transparent image size
                item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize - slotSize * 0.3f); // width
                item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize - slotSize * 0.3f);   // height

                // add to list
                SlotList.Add(slot);
            }
        }

        Invoke("Init", 0.1f);
    }

    void Init()
    {
        // to do list - item load
    }

    // check all slots to add item
    public bool AddItem(itemContainer item)
    {
        
        int slotCount = SlotList.Count;

        // check if already exists
        for (int i = 0; i < slotCount; i++)
        {
           
            Slot slot = SlotList[i].GetComponent<Slot>();

            // if not using , skip
            if (!slot.isUsing())
                continue;

            // if same type and 
            // if less than max count
            if (slot.getCurItem().type == item.item01.type && slot.getItemMaxCnt(item.item01))
            {
                // add
                slot.AddItem(item.item01);
                return true;
            }
        }

        // check empty slots
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = SlotList[i].GetComponent<Slot>();

            // if using , skip
            if (slot.isUsing())
                continue;

            slot.AddItem(item.item01); // add
            return true;
        }

        // some problem , can not add item
        return false;
    }

    // find closest slot
    public Slot ClosestSlot(Vector3 Pos)
    {
        float Min = 10000f;
        int Index = -1;

        int Count = SlotList.Count;
        for (int i = 0; i < Count; i++)
        {
            Vector2 sPos = SlotList[i].transform.GetChild(0).position;
            float Dis = Vector2.Distance(sPos, Pos);

            if (Dis < Min)
            {
                Min = Dis;
                Index = i;
            }
        }

        if (Min > slotSize)
            return null;

        return SlotList[Index].GetComponent<Slot>();
    }

    // item swap
    public void Swap(Slot slot, Vector3 Pos)
    {
        Slot FirstSlot = ClosestSlot(Pos);

        // if same slot, return 
        if (slot == FirstSlot || FirstSlot == null)
        {
            slot.UpdateInfo(true, slot.itemStack.Peek().DefaultImg);
            return;
        }

        // if not using , swap
        if (!FirstSlot.isUsing())
        {
            Swap(FirstSlot, slot);
        }
        else
        {
            int Count = slot.itemStack.Count;
            Item item = slot.itemStack.Peek();
            Stack<Item> temp = new Stack<Item>();

            {
                for (int i = 0; i < Count; i++)
                    temp.Push(item);

                slot.itemStack.Clear();
            }

            Swap(slot, FirstSlot);

            {
                Count = temp.Count;
                item = temp.Peek();

                for (int i = 0; i < Count; i++)
                    FirstSlot.itemStack.Push(item);

                FirstSlot.UpdateInfo(true, temp.Peek().DefaultImg);
            }
        }
    }

    // emptySlot : empty slot, itemSlot : slot with items
    void Swap(Slot emptySlot, Slot itemSlot)
    {
        int Count = itemSlot.itemStack.Count;
        Item item = itemSlot.itemStack.Peek();

        for (int i = 0; i < Count; i++)
        {
            if (emptySlot != null)
                emptySlot.itemStack.Push(item);
        }

        if (emptySlot != null)
            emptySlot.UpdateInfo(true, itemSlot.getCurItem().DefaultImg);

        itemSlot.itemStack.Clear();
        itemSlot.UpdateInfo(false, itemSlot.DefaultImg);
    }
}
