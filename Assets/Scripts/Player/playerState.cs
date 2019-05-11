using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerState : MonoBehaviour
{
    public float hp = 100;              // 체력
    public float full = 10;             // 포만감
    public float scare = 10;            // 공포
    public int insight;                 // 통찰력

    private bool isSelected = false;    // 현재 선택되었는가 여부
    public bool IsSelected() { return isSelected; }

    void Update()
    {
        //몬스터가 공격시 hp 감소
        if(hp > 0)
            hp -= (0.1f * Time.deltaTime);
           //플레이어 사망 클래스만들고/ 클래스 소환. 
        //음식아이템 섭취시 stamina 증가
    }

    public void ChangeHP(float value)
    {
        // hp 확인 구간, value 값은 음수도 올 수 있는 것 유의할 것!!!!!!
        /*
         * if ((hp += value) > maxHP) hp = maxHP;
         * else if (hp <= 0) 체력 0 이하일 시 함수 호출
         */

        hp += value;
    }

    public void ChangeFull(float value)
    {
        // full 확인 구간, full 값은 음수도 가능!!!!!!!!!
        /*
         * if ((full += value) > maxFull) full = maxFull;
         * else if (full <= 0) 포만감 0 이하일 시 함수 호출
         */

        full += value;
    }
}
