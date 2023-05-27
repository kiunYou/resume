using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    [SerializeField]
    [Tooltip("최대 4줄이며 16글자로 구성할 수 있습니다")]
    public Dialogue dialogue;
    [HideInInspector]
    public DialogueManager theDM;
    [HideInInspector]
    public bool isInRange = false;


    public bool dialogueEnded = false;

    public Queue<string> queue;                 // 큐 스트링


    static public TestDialogue instance;       // 인스턴스 선언

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

        theDM = FindObjectOfType<DialogueManager>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            isInRange = false;
        }
    }



    void Update()
    {
        //if (!dialogueEnded && isInRange)
        //{
        //    theDM.testDialogue = this; // 현재 대화 스크립트 인스턴스를 DialogueManager에 전달합니다.
        //    theDM.ShowDialogue(dialogue);
        //}
    }
}
