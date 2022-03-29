using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour
{

    public Transform Img;   

    private Image EmptyImg; 
    private Slot slot;      // slot script

    void Start()
    {
        // get script
        slot = GetComponent<Slot>();
        // get drag image
        Img = GameObject.FindGameObjectWithTag("DragImg").transform;
        // get image component
        EmptyImg = Img.GetComponent<Image>();
    }

    public void Down()
    {
        // empty slot , do nothing
        if (!slot.isUsing())
            return;

        // 오른 마우스, item use
        if (Input.GetMouseButtonDown(1))
        {
            slot.ItemUse();
            return;
        }

        // 보이게 한다.
        Img.gameObject.SetActive(true);

        // size setting
        float Size = slot.transform.GetComponent<RectTransform>().sizeDelta.x;
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size);
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size);

        // get image from cur item
        EmptyImg.sprite = slot.getCurItem().DefaultImg;
        // 마우스 따라 다니게
        Img.transform.position = Input.mousePosition;
        // 인벤 슬롯을 투명으로
        slot.UpdateInfo(true, slot.DefaultImg);
        // 인벤 슬롯 글자 없어지게
        slot.text.text = "";
    }

    public void Drag()
    {
        // isUsing 아니면 아이템이 존재하지 않는 slot
        if (!slot.isUsing())
            return;

        Img.transform.position = Input.mousePosition;
    }

    public void DragEnd()
    {
        // isUsing 아니면 아이템이 존재하지 않는 slot
        if (!slot.isUsing())
            return;

        // IV 는 singleton, inventory  , swap 실행 
        Inventory.IV.Swap(slot, Img.transform.position);
    }

    public void Up()
    {
        // isUsing 아니면 아이템이 존재하지 않는 slot
        if (!slot.isUsing())
            return;

        // 커서 따라다니는 이미지 끔 
        Img.gameObject.SetActive(false);

        // slot default image 로 변경
        slot.UpdateInfo(true, slot.itemStack.Peek().DefaultImg);
    }
}
