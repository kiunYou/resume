using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamblingItemDrop : MonoBehaviour
{
    public static GamblingItemDrop instansce;
    public List<GameObject> itemPrefabs; // 아이템 프리팹 리스트
    public Transform dropPoint;
    [HideInInspector]
    public GameObject spawnedItem;

    [System.Serializable]
    public class ItemGradeProbability
    {
        public ItemGrade.Grade grade;
        public float probability;
    }
    public List<ItemGradeProbability> gradeProbabilities;

    public Transform npcHead; // NPC 머리 위치
    public SpriteRenderer gradeSpriteRenderer; // 스프라이트 렌더러
    public Sprite gradeA; // A 등급 스프라이트
    public Sprite gradeS; // S 등급 스프라이트
    public Sprite gradeSS; // SS 등급 스프라이트
    public Sprite gradeFail; // 꽝 스프라이트

    private void Awake()
    {
        if (instansce == null)
        {
            instansce = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        spawnedItem = null;
        gradeSpriteRenderer.enabled = false; // 초기에 스프라이트 숨김
    }

    public void GamblingDropItem()
    {
        gradeSpriteRenderer.enabled = false; // 스프라이트 숨김
        if (itemPrefabs.Count > 0 && dropPoint != null)
        {
            if (spawnedItem == null)
            {
                float randomValue = Random.value;
                GameObject selectedItem = null;

                foreach (GameObject itemPrefab in itemPrefabs)
                {
                    ItemGrade itemGrade = itemPrefab.GetComponent<ItemGrade>();

                    if (itemGrade != null)
                    {
                        bool itemSelected = false;

                        foreach (var gradeProbability in gradeProbabilities)
                        {
                            if (itemGrade.itemGrade == gradeProbability.grade && randomValue <= gradeProbability.probability)
                            {
                                selectedItem = itemPrefab;
                                itemSelected = true;
                                ShowGradeSprite(gradeProbability.grade); // 스프라이트 표시 함수 호출
                                break;
                            }
                        }

                        if (itemSelected)
                        {
                            spawnedItem = Instantiate(selectedItem, dropPoint.position, Quaternion.identity);
                            spawnedItem.SetActive(false);
                            break;
                        }
                    }
                }
            }
            if (spawnedItem == null)
            {
                Debug.Log("꽝");
                ShowGradeSprite(ItemGrade.Grade.Fail); // 꽝 스프라이트 표시 함수 호출
            }
            if (spawnedItem != null) // 생성된 아이템이 있으면 활성화
            {
                spawnedItem.SetActive(true);
            }
        }
    }

    public void ShowGradeSprite(ItemGrade.Grade grade)
    {
        gradeSpriteRenderer.enabled = true; // 스프라이트 활성화
        switch (grade)
        {
            case ItemGrade.Grade.A:
                gradeSpriteRenderer.sprite = gradeA;
                break;
            case ItemGrade.Grade.S:
                gradeSpriteRenderer.sprite = gradeS;
                break;
            case ItemGrade.Grade.SS:
                gradeSpriteRenderer.sprite = gradeSS;
                break;
            case ItemGrade.Grade.Fail:
                gradeSpriteRenderer.sprite = gradeFail;
                break;
            default:
                gradeSpriteRenderer.enabled = false;
                break;
        }
        gradeSpriteRenderer.transform.position = npcHead.position; // NPC 머리 위에 스프라이트 위치 설정
        StartCoroutine(DisableSpriteAfterDelay(2.0f));
    }
    public IEnumerator DisableSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 설정한 시간 동안 대기합니다.
        gradeSpriteRenderer.enabled = false; // 스프라이트 비활성화합니다.
    }
}
