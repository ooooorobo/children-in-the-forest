using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combine : MonoBehaviour
{
    private DataManager dm;

    private List<Dictionary<string, object>> recipeList;
    private Dictionary<string, object> recipe;

    public GameObject recipePrefab;
    public GameObject recipeParent;

    public GameObject combinePanel;
    public GameObject itemInfo;
    public GameObject costInfo;
    public Button btnCombine;
    private bool combinable;

    public List<playerState> playerStates; // 임시로 플레이어 상태 가져오기 위해 리스트 선언!!!

    private int selectedChar; // 현재 선택된 캐릭터 넘버, 미선택 시 -1

    private void Start()
    {
        dm = DataManager.dataManager;
        recipeList = dm.GetRecipe();

        InitCombiner();

        // 레시피 리스트 개수만큼 버튼 생성
        for (int i = 0; i < recipeList.Count; i++)
        {
            Dictionary<string, object> recipe = recipeList[i];
            MakeBtnRecipe(recipe);
        }

        // 다른 일 하고 있는 캐릭터 있는지 체크하고 있다면 다른일 리본 띄우기
    }

    public void InitCombiner()
    {
        // 변수 초기화
        recipe = null;
        selectedChar = -1;
        btnCombine.interactable = false;
        combinable = false;
        // 레시피 획득 여부 확인하고 isGot에 1 들어간 애들 활성화 시켜줘
    }

    private void MakeBtnRecipe(Dictionary<string, object> recipe)
    {
        GameObject newRecipe = Instantiate(recipePrefab, recipeParent.transform); // 버튼 인스턴스 생성
        newRecipe.name = System.Convert.ToString(recipe["comID"]);

        // 아이템 이미지 설정
        newRecipe.transform.GetChild(0).GetComponent<Image>().sprite = dm.findSprite(newRecipe.name);
        // 아이템 이름 설정
        newRecipe.transform.GetChild(1).GetComponent<Text>().text = dm.getName(newRecipe.name);

        if (System.Convert.ToInt16(recipe["isGot"]) == 0)
        { // 레시피 획득하지 않은 경우 버튼 선택 비활성화
            newRecipe.GetComponent<Button>().interactable = false;
        }
        else
        {
            newRecipe.GetComponent<Button>().interactable = true;
            newRecipe.GetComponent<Button>().onClick.AddListener(() => onClickRecipe(recipe));
        }
    }

    private void onClickRecipe(Dictionary<string, object> recipe)
    {
        string id = System.Convert.ToString(recipe["comID"]);
        this.recipe = recipe;
        Item item = dm.GetItem(id);


                /* 선택 아이템 정보 창 설정 */

        itemInfo.SetActive(true);
            // 스프라이트 설정
        itemInfo.transform.GetChild(0).GetComponent<Image>().sprite = dm.findSprite(id);
            // 아이템 이름 설정
        itemInfo.transform.GetChild(1).GetComponent<Text>().text = item.name;
            // 아이템 종류 설정
        itemInfo.transform.GetChild(2).GetComponent<Text>().text = item.type.ToString();
            // 아이템 설명 설정
        itemInfo.transform.GetChild(3).GetComponent<Text>().text = item.effect;


                /* 아이템 제작 정보 창 설정 */

        costInfo.SetActive(true);

            // 재료 설정
        int matCount = System.Convert.ToInt16(recipe["matCount"]);

        for (int i = 0; i < matCount; i++)
        {
            costInfo.transform.GetChild(i).gameObject.SetActive(true);

            Item item_mat = dm.GetItem(System.Convert.ToString(recipe["mat" + (i + 1) + "ID"]));
            Transform mat = costInfo.transform.GetChild(i);
            mat.gameObject.name = item_mat.ID;
            mat.GetChild(0).GetComponent<Image>().sprite = dm.findSprite(item_mat.ID);      // 재료 스프라이트
            mat.GetChild(2).GetComponent<Text>().text = " / " +                             // 필요 재료 수
                System.Convert.ToString(recipe["mat" + (i + 1) + "Need"]);
        }
        for (int i = matCount; i < 2; i++)
        {
            costInfo.transform.GetChild(i).gameObject.SetActive(false);
        }

        // 제작 시간 설정
        costInfo.transform.GetChild(2).GetComponent<Text>().text = "제작 시간 : " +
            System.Convert.ToString(recipe["time"]) + "초";

        // 필요 통찰력 설정
        costInfo.transform.GetChild(3).GetChild(1).GetComponent<Text>().text 
            = " / " + System.Convert.ToInt16(recipe["insight"]).ToString();

        setCharInfo(); // 현재 캐릭터별 소지 재료 수와 통찰력 설정
    }

    private void setCharInfo()
    {
        combinable = true;

        // 통찰력 설정
        if (selectedChar == -1)     // 선택되지 않았을 경우
        {
            combinable = false;

            Text nowInsight = costInfo.transform.GetChild(3).GetChild(0).GetComponent<Text>();
            nowInsight.text = "?";
            nowInsight.color = Color.red;
        }
        else                        // 플레이어 선택했을 경우
        {
            Text nowInsight = costInfo.transform.GetChild(3).GetChild(0).GetComponent<Text>();
            int insight = playerStates[selectedChar].insight;
            nowInsight.text = insight.ToString();

            if (System.Convert.ToInt16(recipe["insight"]) > insight)
            {
                nowInsight.color = Color.red;
                combinable = false;
            }
            else nowInsight.color = Color.black;
        }

        // 소지 재료 수 설정
        for (int i = 0; i < System.Convert.ToInt16(recipe["matCount"]); i++)
        {
            Item item_mat = dm.GetItem(System.Convert.ToString(recipe["mat" + (i + 1) + "ID"]));
            Transform mat = costInfo.transform.GetChild(i);
            Text txtNowMat = mat.GetChild(1).GetComponent<Text>();

            int nowMat;

            if (selectedChar == -1) nowMat = 0;
            else nowMat = ObjManager.objManager.inventory.getItemCount(item_mat.ID, selectedChar);

            txtNowMat.text = nowMat.ToString();

            if (System.Convert.ToInt16(recipe["mat" + (i + 1) + "Need"]) > nowMat)
            {
                txtNowMat.color = Color.red;
                combinable = false;
            }
            else
            {
                txtNowMat.color = Color.black;
            }
        }

        // 제작 가능 여부 결정
        checkCombinable();
    }

    private void checkCombinable()
    {
        if (combinable) btnCombine.interactable = true;
        else btnCombine.interactable = false;
    }

    public void SelectChild(int num)
    {
        selectedChar = num;

        // 제작 아이템 정보창 업데이트
        if (itemInfo.activeInHierarchy) setCharInfo();
    }

    public void StartCombine()
    {
        Debug.Log("조합 시작!");


        // 현재 선택된 캐릭터 다른 일 중으로 스테이트 변경
        // nn초 뒤에 완성되어 인벤토리에 들어가도록 하기
        // 아무튼 조합 시작하기
        // 재료 사용한 거 소모시키기

    }
}
