using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // 정적 인스턴스 선언

    public int atkCount;        // 플레이어 어택 카운트 변수

    public TextMeshProUGUI atkCountText;

    public Queue<string> queue;                 // 큐 스트링

    private Bound[] bounds;

    private PlayerControl theplayer;

    private CameraManager theCamera;

    public GameObject[] npcPrefabs; //여러개의 NPC 프리팹 참조를 위한 배열 추가

    public Transform[] npcSpawnPoints; //NPC 생성 위치를 저장할 배열

    private List<GameObject> npcs; //생성된 NPC들을 저장할 리스트

    public ProgressUI progressUI;

    private void UpdateAtkCountText()
    {
        atkCountText.text = atkCount.ToString();
    }

    public void UpdateAtkCount(int Count)
    {
        Count = atkCount;

        UpdateAtkCountText();
    }

    public void LoadStart()
    {
        StartCoroutine(LoadWaitCoroutine());
    }

    IEnumerator LoadWaitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        theplayer = FindObjectOfType<PlayerControl>();
        bounds = FindObjectsOfType<Bound>();
        theCamera = FindObjectOfType<CameraManager>();

        // theCamera.target = GameObject.Find("Player");


        /*for(int i = 0; i< bounds.Length; i++)
         {
             if(bounds[i].boundName == theplayer.currentMapName)
             {
                 bounds[i].SetBound();

             }
         }*/

    }

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<string>();

        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);

            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        npcs = new List<GameObject>(); //NPC 리스트 초기화

        if (SceneManager.GetActiveScene().name == "Stage_2")
        {
            SpawnNPCs(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAtkCount(atkCount);
    }

    private void SpawnNPCs(int count)
    {
        int randomIndex = Random.Range(0, npcPrefabs.Length); // 랜덤한 인덱스를 루프 밖에서 선택
        for (int i = 0; i < count; i++)
        {

            if (randomIndex >= npcSpawnPoints.Length || i >= npcSpawnPoints.Length)
            {
                return;
            }
            if (i == randomIndex)
            {
                GameObject npc = Instantiate(npcPrefabs[i], npcSpawnPoints[i].position, npcSpawnPoints[i].rotation);
                npcs.Add(npc);
                break;
            }
        }

    }
}