using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    /* 가져올 게임 오브젝트들 */
    public GameObject SlotsParent; // 슬롯 Parent
    public GameObject CharactersParent; // 플레이어 캐릭터들 Parent
    public Transform InfoSlot; // 인벤토리의 설명 슬롯

    private Button checkBtn; // 설명 슬롯의 조사 버튼
    private Button useBtn; // 설명 슬롯의 사용 버튼
    private Button trashBtn; // 설명 슬롯의 버리기 버튼

    public List<GameObject> slots; // 게임 오브젝트의 슬롯들
    public List<Item> items0; // 0번 애기의 아이템 저장 리스트
    public List<Item> items1; // 1번 애기의 아이템 저장 리스트
    public List<Item> items2; // 2번 애기의 아이템 저장 리스트


    public int CharMax = 3; // 애기 몇명
    public int InvenMax = 6; // 애기 당 최대 인벤토리 슬롯 수

    /* 
     * 0 ~ 5 0번 애기
     * 6 ~ 11 1번 애기
     * 12 ~ 17 2번 애기
     */


    private void Start() // 인벤토리 상황 불러와서 UI 설정하기
    {

        /* 설명 슬롯 버튼 가져오기 */
        checkBtn = InfoSlot.Find("BtnCheck").GetComponent<Button>();
        useBtn = InfoSlot.Find("BtnUse").GetComponent<Button>();
        trashBtn = InfoSlot.Find("BtnThrow").GetComponent<Button>();

        /* 슬롯들 리스트에 저장하기 */
        for (int i = 0; i < SlotsParent.transform.childCount; i++)
        {
            slots.Add(SlotsParent.transform.GetChild(i).gameObject);
            slots[i].GetComponent<Slot>().SetChildNum(i / 6);
        }

        /* xml에서 아이템 데이터 가져오기 */
        Item[] items = ItemIO.LoadData();

        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Slot slot = slots[i].GetComponent<Slot>();

                try
                {
                    slot.AddItem(items[i]);
                }
                catch (Exception ex) { }
            }
            RenewInventory();
        }
    }

    public void RenewInfo(Slot slot) // 아이템 상세정보 띄우기
    {
        InfoSlot.gameObject.SetActive(true);
        Item item = slot.ItemReturn();

        if (DataManager.dataManager.isGot(item.ID) == 0) // 처음 줍거나 써본 적 없을 때
        {
            checkBtn.interactable = true; // 체크 버튼 사용 가능

            /* 설명 슬롯 설정 */
            InfoSlot.GetChild(0).GetComponent<Text>().text = "???";
            Image image = InfoSlot.GetChild(1).GetComponent<Image>();
            image.sprite = item.sprite;
            image.color = Color.black;
            InfoSlot.GetChild(3).GetComponent<Text>().text = "???";

            /* 기존 버튼 리스너 모두 제거 */
            checkBtn.onClick.RemoveAllListeners();
            useBtn.onClick.RemoveAllListeners();
            trashBtn.onClick.RemoveAllListeners();

            /* 버튼 리스너 붙이기 */
            checkBtn.onClick.AddListener(() => slot.CheckItem()); // 아이템 조사 함수 리스너 등록
            useBtn.onClick.AddListener(() => slot.UseItem()); // 아이템 사용 함수 리스너 등록
            trashBtn.onClick.AddListener(() => slot.ThrowItem()); // 아이템 버리기 함수 리스너 등록
        }
        else // 조사했거나 사용해 봤을 때
        {
            checkBtn.interactable = false; // 조사 버튼 사용 불가능

            /* 설명 슬롯 설정 */
            InfoSlot.GetChild(0).GetComponent<Text>().text = item.name;
            Image image = InfoSlot.GetChild(1).GetComponent<Image>();
            image.sprite = item.sprite;
            image.color = Color.white;
            InfoSlot.GetChild(3).GetComponent<Text>().text = item.effect;

            /* 기존 버튼 리스너 모두 제거 */
            useBtn.onClick.RemoveAllListeners();
            trashBtn.onClick.RemoveAllListeners();

            /* 버튼 리스너 붙이기 */
            useBtn.onClick.AddListener(() => slot.UseItem()); // 아이템 사용 함수 리스너 등록
            trashBtn.onClick.AddListener(() => slot.ThrowItem()); // 아이템 버리기 함수 리스너 등록
        }
    }

    public void RenewInfo() // 설명 슬롯 끄기
    {
        InfoSlot.gameObject.SetActive(false);
    }

    public bool AddItem(int ChildNum, Item item) // 슬롯 차있는지 확인 후 슬롯에 addItem
    {
        int startSlotNum = ChildNum * 6; // 해당 번호 애기 인벤토리 시작 인덱스
        
        for (int i = 0; i < 6; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();

            if (slot.isSlots()) // 슬롯이 비어 있으면 아래 줄 실행
                continue; // 차있으면(true) i++

            slot.AddItem(item);
            return true;
        }
        
        return false;
    }

    public void Swap(Slot slot, Vector3 pos) // 선택한 slot과 드롭한 마우스 위치로 스왑함
    {
        // firSlot --> 현재 마우스 포인트에 가장 가까이 있는 슬롯
        Slot firSlot = NearDisSlot(pos);

        if (slot == firSlot || firSlot == null)
        { // 선택한 슬롯이 비어있거나 드래그 끝난 위치가 처음 위치와 같을 때
            slot.UpdateInfo(true, slot.ItemReturn().sprite);
            return;
        }

        if (!firSlot.isSlots()) // 슬롯이 비었으면
        {
            slot.GetComponent<Image>().color = Color.white;
            Swap(firSlot, slot);
        }
        else // 슬롯이 차있으면
        {
            Item item = slot.ItemReturn(); 
            slot.emptyItem(); // 선택한 슬롯의 아이템 기억

            Swap(slot, firSlot); // firSlot의 아이템을 slot에 넣음

            firSlot.AddItem(item); // 기억한 아이템을 firSlot에 넣음
            firSlot.UpdateInfo(true, item.sprite);
        }
    }

    public void Swap(Slot xFirst, Slot oSecond) // (swap 보조함수) xFirst에 oSecond가 가지고 있던 아이템 넣고 oSecond 비우기
    { 
        // 두 슬롯의 인벤토리 주인 다를 경우 빠꾸먹이는 부분 추가

        Item item = oSecond.ItemReturn();

        if (xFirst != null)
        {
            xFirst.AddItem(item);
            xFirst.UpdateInfo(true, oSecond.ItemReturn().sprite);
        }

        oSecond.emptyItem();
        oSecond.UpdateInfo(false, oSecond.DefaultImg);
    }

    public Slot NearDisSlot(Vector3 pos) // 해당 위치 근처에 슬롯 있는지 확인
    {
        float min = 50000f;
        int index = -1;

        int count = slots.Count;

        for (int i = 0; i < count; i++) // 슬롯 중에 현재 위치와 가장 가까운 슬롯 찾기
        {
            Vector2 sPos = slots[i].transform.GetChild(0).position;
            float dis = Vector2.Distance(sPos, pos);

            if (dis < min)
            {
                min = dis;
                index = i;
            }
        }

        return slots[index].GetComponent<Slot>();
    }

    public void RenewInventory() // 아이템 재정렬 (아이템 사이에 빈칸 없도록)
    {
        for (int i = 0; i < CharactersParent.transform.childCount; i++)
        {
            List<Item> items = new List<Item>();

            for (int j = i * 6; j < (i + 1) * 6; j++)
            {
                Slot slot = slots[j].GetComponent<Slot>();
                slot.CheckGotItem();

                if (slot.isSlots())
                {
                    Item item = slot.ItemReturn();
                    items.Add(item);
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            int k = i * 6;

            for (int j = i * 6; j < (i + 1) * 6; j++)
            {

                if (slots[j].GetComponent<Slot>().isSlots())
                {
                    if (k != j)
                    {
                        Swap(slots[k].GetComponent<Slot>(), slots[j].GetComponent<Slot>());
                    }

                    k++;
                }
            }
        }
    }

    public int getItemCount(string id, int num)
    {
        int count = 0;

        for (int i = 6 * num; i < 6 * (num + 1) ; i++)
        {
            if (!slots[i].GetComponent<Slot>().isSlots())
                break;
            if (slots[i].GetComponent<Slot>().item.ID == id)
                count++;
        }
        return count;
    }
}
