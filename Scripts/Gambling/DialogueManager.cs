using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    #region Singleton
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;

        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion Singleton

    public Text text;
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;
    [HideInInspector]
    public List<string> listSentences;
    [HideInInspector]
    public List<Sprite> listSprites;
    [HideInInspector]
    public List<Sprite> listDialogueWindows;
    [HideInInspector]
    public int count; // 대화 진행 사항 카운트

    public Animator animSprite;
    public Animator animDialogueWindow;

    private PlayerControl playerControl; // 플레이어 컨트롤러에 대한 참조 추가

    [HideInInspector]
    public AudioManager theAudio; // 사운드 나오게 하기 위한 오디오 메니저 클래스 오버로딩
    public bool talking = false;
    public TestDialogue testDialogue;

    public Button yesButton; // 클래스에 YES버튼을 참조할 변수 추가
    public Button noButton;  // 클래스에 NO버튼을 참조할 변수 추가

    [HideInInspector]
    public bool isDialogueEnded = false; // 대화가 끝났는지 확인하는 변수
    [HideInInspector]
    public bool isYesNoButtonsActive = false;
    [HideInInspector]
    public GamblingItemDrop itemDrop;


    public delegate void DialogueEndEvent();
    public event DialogueEndEvent OnDialogueEnd;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>(); // 사진
        listDialogueWindows = new List<Sprite>();
        theAudio = FindObjectOfType<AudioManager>();
        playerControl = FindObjectOfType<PlayerControl>(); // 플레이어 컨트롤러를 찾고 할당
        testDialogue = null;
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        rendererSprite.gameObject.SetActive(false);
        rendererDialogueWindow.gameObject.SetActive(false);



    }
    public void ShowDialogue(Dialogue dialogue)
    {
        rendererSprite.gameObject.SetActive(true);
        rendererDialogueWindow.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        if (dialogue.sentences.Length != dialogue.sprites.Length || dialogue.sentences.Length != dialogue.dialogueWindows.Length)
        {
            Debug.LogError("모든 대화 목록(문장, 스프라이트, 대화 창)의 길이는 같아야 합니다.");
            return;
        }
        talking = true;
        count = 0;
        playerControl.isChat = true; // 플레이어 이동 정지
        listSentences.Clear(); // 리스트 초기화 추가
        listSprites.Clear();
        listDialogueWindows.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]); // 사진
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        animSprite.SetBool("Appear", true); // 사진
        animDialogueWindow.SetBool("Appear", true);

        StopAllCoroutines();
        StartCoroutine(StartDialogueCoroutine());
    }
    public void ExitDialogue()
    {
        text.text = "";
        rendererSprite.GetComponent<SpriteRenderer>().sprite = null; //이미지를 지웁니다.
        rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = null; //이미지를 지웁니다.
        count = 0;
        rendererSprite.enabled = true;
        rendererDialogueWindow.enabled = true;
        listSentences.Clear();
        listSprites.Clear();// 사진
        listDialogueWindows.Clear();

        animSprite.SetBool("Appear", false); // 사진
        animDialogueWindow.SetBool("Appear", false);
        talking = false;
        playerControl.isChat = false;
        testDialogue.dialogueEnded = true;
        rendererSprite.gameObject.SetActive(false);
        rendererDialogueWindow.gameObject.SetActive(false);
        if (OnDialogueEnd != null)
        {
            OnDialogueEnd();
        }
    }
    IEnumerator HideDialogueWindow()
    {
        animSprite.SetBool("Appear", false); // 사진
        animDialogueWindow.SetBool("Appear", false);
        yield return new WaitForSeconds(0.5f);
        ExitDialogue();
    }
    public IEnumerator StartDialogueCoroutine()
    {
        if (count >= listSentences.Count || count < 0)
        {
            Debug.Log("잘못된 개수 값: " + count);
            yield break;
        }
        if (count >= listSprites.Count || count >= listDialogueWindows.Count)
        {
            Debug.LogError("목록 크기가 개수 값과 일치하지 않습니다. 모든 목록이 올바르게 채워졌는지 확인합니다.");
            yield break;
        }
        if (count >= listSentences.Count)
        {
            // 리스트의 범위를 벗어난 인덱스를 사용한 경우에 대한 예외 처리
            Debug.LogError("잘못된 인덱스: " + count);
            yield break;
        }
        if (count == 1)
        {
            theAudio.Play("gambling_Dialogue_Sound1");
        }
        else if (count == 2)
        {
            theAudio.Play("gambling_Dialogue_Sound2");
        }
        else if (count == 3)
        {
            theAudio.Play("gambling_Dialogue_Sound3");
        }
        else if (count == 4)
        {
            theAudio.Play("gambling_Dialogue_Sound4");
        }
        string currentSentence = listSentences[count];
        if (count > 0)
        {
            if (listDialogueWindows[count] != listDialogueWindows[count - 1])
            {
                animSprite.SetBool("Change", true);
                animDialogueWindow.SetBool("Appear", false);
                yield return new WaitForSeconds(0.02f);
                rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
                rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                animDialogueWindow.SetBool("Appear", true);
                animSprite.SetBool("Change", false);
            }
            else
            {
                if (listSprites[count] != listSprites[count - 1])
                {
                    animSprite.SetBool("Change", true);
                    yield return new WaitForSeconds(0.1f);
                    rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                    animSprite.SetBool("Change", false);
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        else
        {
            rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
            rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];

        }
        if (count < listSentences.Count)
        {

            for (int j = 0; j < currentSentence.Length; j++)
            {

                char currentChar = currentSentence[j];
                text.text += listSentences[count][j]; // 1글자씩 출력



                yield return new WaitForSeconds(0.01f);
            }
        }
        if (count == listSentences.Count - 1) // 마지막 대화일 경우
        {
            isDialogueEnded = true; // 대화가 끝났음을 표시

            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
        else
        {
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
    }
    public void CloseDialogueWithButton() // 버튼을 클릭했을 때 수행할 작업을 여기에 추가합니다.
    {
        count = 0; //count 초기화 추가

        listSentences.Clear(); // 리스트 초기화 추가
        listSprites.Clear();
        listDialogueWindows.Clear();

        isDialogueEnded = false;
        ExitDialogue(); // 대화창을 닫습니다.
    }
    public void OnYesButtonClick() // 예 버튼을 클릭했을 때 수행할 작업을 여기에 추가합니다.
    {
        if (count < listSentences.Count - 1) // 대화가 끝나지 않았을 때
        {
            count++;
            text.text = "";
            theAudio.Play("enter_sound");
            StopAllCoroutines();
            StartCoroutine(StartDialogueCoroutine());
        }
        else // 마지막 대화에 도달했을 때
        {
            itemDrop.GamblingDropItem();

            theAudio.Play("gamblingSound");
            count = 0; //count 초기화 추가
            listSentences.Clear(); // 리스트 초기화 추가
            listSprites.Clear();
            listDialogueWindows.Clear();
            ExitDialogue(); // 대화창을 닫습니다.
        }
    }
    public void OnNoButtonClick() //아니오 버튼을 클릭했을 때 수행할 작업을 여기에 추가
    {
        count = 0; //count 초기화 추가

        listSentences.Clear(); // 리스트 초기화 추가
        listSprites.Clear();
        listDialogueWindows.Clear();

        ExitDialogue(); // 대화창을 닫습니다.

    }
    // Update is called once per frame
    void Update()
    {
        itemDrop = FindObjectOfType<GamblingItemDrop>();

        if (Input.GetMouseButtonDown(0) && !isYesNoButtonsActive && !isDialogueEnded)
        {
            if (talking)
            {
                if (count < listSentences.Count - 1)
                {
                    count++;
                    text.text = "";
                    theAudio.Play("enter_sound");
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
                else
                {
                    CloseDialogueWithButton();
                }
            }
        }

    }
}