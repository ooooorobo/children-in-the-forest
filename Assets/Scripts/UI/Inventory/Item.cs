using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string name; // 아이템 이름
    public string ID; // 아이템 식별 번호
    public string des; // 아이템 설명
    public string tool; // 채집에 필요한 도구
    public Sprite sprite; // 아이템 이미지
    public ItemType type; // 아이템 종류
    public string effect; // 사용 효과
    
    public enum ItemType // 아이템 종류
    {
        food,       // 음식
        tool,       // 도구
        material,   // 합성 재료
        memo        // 메모
    }

    private Inventory iv;

    private void Start()
    {
        iv = ObjManager.objManager.inventory;
        sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void AddItem() // 인벤토리에 해당 아이템 넣기
    {
        /* 어떤 애기가 먹었는지 가져와야 함 */

        if (!iv.AddItem(0, this)) // 해당 애기의 인벤토리 자리 없을 경우
        {
            Debug.Log("가득참");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}