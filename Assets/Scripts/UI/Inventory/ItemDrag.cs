using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour {

    public Transform img; // 드래그이미지
    private Image emptyImg; // 드래그이미지의 이미지 컴포넌트
    private Slot slot;

    /* 아이템 드래그 확인 */
    private Vector2 firMousePos;
    private float min = 20f;
    private bool isDrag = false;

    private void Start()
    {
        slot = GetComponent<Slot>();
        img = GameObject.FindGameObjectWithTag("DragImg").transform;
        emptyImg = img.GetComponent<Image>();
    }

    public void Down()
    {
        firMousePos = Input.mousePosition;
        isDrag = false;

        if (!slot.isSlots())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            slot.emptyItem(); // 아이템 클릭 메소드
            return;
        }
        
    }

    public void Drag()
    {
        if (!slot.isSlots())
            return;

        if (!isDrag && Vector2.Distance((Vector2)Input.mousePosition, firMousePos) >= min)
        { // 처음에 드래그 일정 거리 이상 했을 경우
            isDrag = true;
            ObjManager.objManager.inventory.RenewInfo();

            img.gameObject.SetActive(true);
            emptyImg.sprite = slot.ItemReturn().sprite;
            slot.UpdateInfo(true, slot.DefaultImg);
            gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;

            if (DataManager.dataManager.isGot(slot.ItemReturn().ID) == 0)
            {
                emptyImg.color = Color.black;
            }
            else
            {
                emptyImg.color = Color.white;
            }
        }

        if (img.gameObject.activeInHierarchy)
        {
            img.transform.position = Input.mousePosition;
        }
    }

    public void DragEnd()
    {

        if (!slot.isSlots())
            return;

        ObjManager.objManager.inventory.Swap(slot, img.transform.position);
        ObjManager.objManager.inventory.RenewInventory();
    }

    public void Up()
    {

        if (!slot.isSlots())
            return;

        img.gameObject.SetActive(false);
        slot.UpdateInfo(true, slot.ItemReturn().sprite);

        if (!isDrag) // 드래그 하지 않았을 때
            ObjManager.objManager.inventory.RenewInfo(gameObject.GetComponent<Slot>()); // 아이템 설명 띄우기
    }
}
