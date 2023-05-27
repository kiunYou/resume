using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerControl;
//using static UnityEditor.PlayerSettings;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Collections;

public class PlayerControlUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum State
    {
        up, down, left, right, atk, interact
    }

    public State state;

    private GameObject player; // 플레이어 게임 오브젝트 참조

    private PlayerControl playerControl;

    GameManager gameManager;            // 게임 매니저 스크립트를 불러올 변수

    public DialogueManager dialogueManager;

    public TestDialogue testDialogue;

    DialogueManager theDM;

    public GameObject Controlset;

    TextTrigger textTrigger;

    DatabaseManager databaseManager;

    private bool isPressingUI = false;

    private IEnumerator PlayerMoveCoroutine;

    public AudioManager audioManager;

    private IEnumerator MovePlayerEverySecond()
    {
        while (isPressingUI)
        {
            Vector3 moveDirection = Vector3.zero;

            switch (state)
            {
                case State.up:

                    moveDirection = Vector3.up;

                    break;

                case State.down:

                    moveDirection = Vector3.down;

                    break;

                case State.left:

                    moveDirection = Vector3.left;

                    break;

                case State.right:

                    moveDirection = Vector3.right;

                    break;

            }

            if (Mathf.Abs(moveDirection.y) > 0.5f && !playerControl.isMoving && !playerControl.isAtk && !playerControl.isChat)       // moveVertical이 0.5f보다 클 때, 이동 중이 아닐 때, 공격 중이 아닐 때, 대화중이 아닐 때를 체크하는 조건문이다.
                                                                                                                                     // Mathf.Abs함수는 절대값으로 배출한다. (ex: -100일 때 100, 100일 때 100) 즉 입력키를 절대값으로 반환하고, 입력했는지를 판단한다.
            {

                Vector3 newPosition = player.transform.position + new Vector3(0, moveDirection.y, 0) * playerControl.moveAmount;        // 새로운 변수 newPosition을 설정하고, 기존의 트랜스폼 포지션 + 세로 입력 * 이동 량을 곱해 newPosition에 대입하는 구문이다.

                playerControl.pos.position = player.transform.position + new Vector3(0, moveDirection.y, 0) * playerControl.moveAmount;               // 공격하는 범위가 캐릭터 이동에 따라 변화하기 위해 pos의 포지션도 입력값 만큼 이동 시키고 대입하는 구문이다.
                                                                                                                                                      // (여기서 이미 자식클래스로 상속되어있기 때문에, 추가로 이동을 시키지 않아도 플레이어의 이동 방향에 맞게 앞에 배치된다.)

                playerControl.TrapMove();

                if (!IsWallBetween(player.transform.position, newPosition))    // IsWallBetween(start, end)고 인자 값은 Vector3이다. 출발점과 도착점을 책정하고, 위에 정의된 WallLayer가 존재 할 시 이동하지 않기 위해 작성된 조건문이다.
                {


                    if (moveDirection.y > 0)                               // moveVertical의 입력값이 0보다 클 때, 즉 상향 방향키를 입력했을 경우에 대한 조건문
                    {

                        playerControl.animator.SetTrigger("player_Move_Up");          // player_Move_Up의 이름을 가진 애니메이터의 트리거를 킨다

                        playerControl.animator.SetBool("isMove", false);              // 애니메이터에서 isMove라는 이름을 가진 bool타입 파라미터를 false로 한다. 여기서 isMove는 무브가 종료되었을 시 Idle 애니메이션으로 자동 전환하기 위한 파라미터이다.

                        playerControl.theAudio.Play(playerControl.moveSound);                       // 사운드 틀어라!

                        playerControl.copyPosition = Vector3.up;                      // 이동 체크 후 대입

                    }
                    if (moveDirection.y < 0)                               // moveVertical의 입력값이 0보다 작을 때, 즉 하향 방향키를 입력했을 경우에 대한 조건문
                    {

                        playerControl.animator.SetTrigger("player_Move_Down");        // player_Move_Down의 이름을 가진 애니메이터의 트리거를 킨다.

                        playerControl.animator.SetBool("isMove", false);              // 애니메이터에서 isMove라는 이름을 가진 bool타입 파라미터를 false로 한다. 여기서 isMove는 무브가 종료되었을 시 Idle 애니메이션으로 자동 전환하기 위한 파라미터이다.

                        playerControl.theAudio.Play(playerControl.moveSound);                       // 사운드 틀어라!

                        playerControl.copyPosition = Vector3.down;                    // 이동 체크 후 대입

                    }
                    playerControl.targetPosition = newPosition;                       // 입력에 따라 변화한 newPosition을 targetPosition에 대입하는 구문

                    playerControl.isMoving = true;                                    // isMoving을 true로 해 이동중인 것을 판단한다.
                }
            }

            if (Mathf.Abs(moveDirection.x) > 0.5f && !playerControl.isMoving && !playerControl.isAtk && !playerControl.isChat)      // moveHorizontal이 0.5f보다 클 때, 이동 중이 아닐 때, 공격 중이 아닐 때, 대화중이 아닐 때를 체크하는 조건문이다.
            {

                Vector3 newPosition = player.transform.position + new Vector3(moveDirection.x, 0, 0) * playerControl.moveAmount;      // newPosition을 설정하고, 기존의 트랜스폼 포지션 + 세로 입력 * 이동 량을 곱해 newPosition에 대입하는 구문이다.

                playerControl.pos.position = player.transform.position + new Vector3(moveDirection.x, 0, 0) * playerControl.moveAmount;             // 공격하는 범위가 캐릭터 이동에 따라 변화하기 위해 pos의 포지션도 입력값 만큼 이동 시키고 대입하는 구문이다.

                playerControl.TrapMove();

                if (!IsWallBetween(player.transform.position, newPosition))    // IsWallBetween(start, end)고 인자 값은 Vector3이다. 출발점과 도착점을 책정하고, 위에 정의된 WallLayer가 존재 할 시 이동하지 않기 위해 작성된 조건문이다.
                {


                    if (moveDirection.x > 0)                             // moveHorizontal의 입력값이 0보다 클 때, 즉 오른쪽 방향키를 입력했을 경우에 대한 조건문
                    {

                        playerControl.animator.SetTrigger("player_Move_Right");       // player_Move_Right의 이름을 가진 애니메이터의 트리거를 킨다.

                        playerControl.animator.SetBool("isMove", false);              // 애니메이터에서 isMove라는 이름을 가진 bool타입 파라미터를 false로 한다. 여기서 isMove는 무브가 종료되었을 시 Idle 애니메이션으로 자동 전환하기 위한 파라미터이다.

                        playerControl.theAudio.Play(playerControl.moveSound);                       // 사운드 틀어라!

                        playerControl.copyPosition = Vector3.right;                   // 이동 체크 후 대입

                    }
                    if (moveDirection.x < 0)                             // moveHorizontal의 입력값이 0보다 클 때, 즉 왼쪽 방향키를 입력했을 경우에 대한 조건문
                    {

                        playerControl.animator.SetTrigger("player_Move_Left");        // player_Move_Left의 이름을 가진 애니메이터의 트리거를 킨다.

                        playerControl.animator.SetBool("isMove", false);              // 애니메이터에서 isMove라는 이름을 가진 bool타입 파라미터를 false로 한다. 여기서 isMove는 무브가 종료되었을 시 Idle 애니메이션으로 자동 전환하기 위한 파라미터이다.

                        playerControl.theAudio.Play(playerControl.moveSound);                       // 사운드 틀어라!

                        playerControl.copyPosition = Vector3.left;                    // 이동 체크 후 대입

                    }

                    playerControl.targetPosition = newPosition;                       // 입력에 따라 변화한 newPosition을 targetPosition에 대입하는 구문

                    playerControl.isMoving = true;                                    // isMoving을 true로 해 이동중인 것을 판단한다.

                }

            }
            yield return new WaitForSeconds(0.05f);
        }



    }

    //public Vector3 targetPosition;     // 이동 구문에서 포지션을 관리하는 변수

    //public bool isMoving = false;      // 이동 중인지 판단하는 불 값 변수 - true : 이동 상태, false : Idle 상태 (이동, 공격을 동시에 못하게 차단하는 역할을 함)

    //public Transform playerPos;

    //public float moveAmount = 1.0f;     // 입력 시 얼마나 이동할지에 대한 변수

    //public float moveSpeed = 15.0f;      // 입력키에 따라 이동할 때 속도를 정의하는 변수
    private void Awake()
    {
        theDM = FindObjectOfType<DialogueManager>();
        theDM.OnDialogueEnd += HandleDialogueEnd; // 이벤트 등록
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerControl.isPlayerDead) return;

        isPressingUI = true;
        PlayerMoveCoroutine = MovePlayerEverySecond();
        StartCoroutine(PlayerMoveCoroutine);

        bool atkButton = false;
        bool interactButton = false;


        switch (state)
        {

            case State.atk:

                atkButton = true;

                break;

            case State.interact:

                interactButton = true;

                break;

        }


        //공격 구문

        if (playerControl.curTime <= 0 && !playerControl.isMoving && !playerControl.skill1 && !playerControl.isAtk)                                              // 쿨타임이 0보다 작거나 0일때, 즉 공격을 발동할 수 있을때, 이동중이 아닐때에 대한 조건문
        {
            if (atkButton)                                            // Z키를 눌렀을 때에 대한 조건문
            {

                if (playerControl.playerAtkCount > 0)                                             // 공격 카운트가 0보다 클 때에 대한 조건문
                {

                    Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(playerControl.pos.position, playerControl.boxSize, 0);       // Physics2D.OverlapBoxAll(중심 위치, 사이즈, 회전값);은 콜라이더 상자의 위치와 사이즈, 회전값을 정의하고 콜라이더 상자안에 검출된 모든 오브젝트를 검출하는 코드
                                                                                                                                    // 모든 오브젝트를 검출해 배열 형태로 출력하기 때문에 배열문은 Collider2D[] collider2Ds에 대입시킨다.

                    foreach (Collider2D collider in collider2Ds)    // 배열의 처음과 끝까지 검출하는 코드. 검출된 모든 오브젝트를 처음부터 끝까지 검출한다.
                    {
                        if (collider.tag == "Enemy")                // 검출된 오브젝트의 태그가 Enemy가 달려있을 경우에 대한 조건문
                        {
                            collider.GetComponent<Enemy>().TakeDamage(1);       // 충돌된 오브젝트가 가지고 있는 Enemy라는 스크립트에서 TakeDamage함수를 호출 시켜라. TakeDamage함수의 매개변수는 int형이다! 궁금하면 Enemy함수 가보세요!
                                                                                // 충돌된 오브젝트가 Enemy일 경우에 대해 이미 검출 작업을 거쳤기 때문에 추가 참조 구문이 필요 없이 바로 가져다 쓸 수 있다.
                        }
                    }
                    playerControl.animator.SetTrigger("player_Atk");                          // 애니메이터의 player_Atk 트리거를 켜라! 

                    playerControl.theAudio.Play(playerControl.atkSound);

                    gameManager.atkCount--;                         // 공격을 했기 때문에 게임 매니저가 관리하고있는 atkCount를 -1 해라.                                     

                    Debug.Log("공격중입니다");                      // 공격 했는지 확인용!

                    playerControl.curTime = playerControl.coolTime;                             // 쿨타임에 위에 정의한 쿨타임을 대입시켜라!

                    playerControl.isAtk = true;                                   // 공격 중일 때에 대한 변수를 true로 해라. 즉 

                }
            }
        }

        if (interactButton && !playerControl.isCasting)          // Q 입력 하거나 IsCasting 상태가 false 일 때
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(playerControl.playerCollider.bounds.center, playerControl.playerCollider.bounds.size, 0);  // 플레이어 콜라이더의 범위 만큼 검출

            foreach (Collider2D collider in colliders)          // 검출한 오브젝트 다시 재검출
            {
                if (collider.CompareTag("Box"))                 // Box 태그 달고 있으면
                {
                    playerControl.isCasting = true;                           // 캐스팅 상태로 만들고

                    StartCoroutine(playerControl.ShowProgress());             // UI 반영을 위한 코루틴을 실행해라

                    break;                                      // 종료
                }

                if (collider.CompareTag("Lever"))
                {
                    playerControl.isCasting = true;

                    StartCoroutine(playerControl.ShowProgress());

                    break;
                }

                if (collider.CompareTag("Dust"))
                {
                    playerControl.isCasting = true;

                    StartCoroutine(playerControl.ShowProgress());

                    break;
                }

                if (collider.CompareTag("Reward"))
                {
                    playerControl.isCasting = true;
                    audioManager.Play("clear_Box");
                    StartCoroutine(playerControl.ShowProgress());

                    break;
                }

                else if (collider.CompareTag("NPC")) //NPC 콜라이더 내에서만 텍스트 창 활성화 시키겠다!
                {
                    collider.GetComponent<TextTrigger>().Trigger();
                }
                //else if (collider.CompareTag("FireBox")) //박스할거면 ㄱㄱ
                //{
                //    collider.GetComponent<TextTrigger3>().Trigger();
                //}

                /*else if (collider.CompareTag("NPC1")) //NPC 콜라이더 내에서만 텍스트 창 활성화 시키겠다!
                {
                    collider.GetComponent<QuestTest>().ACorou();
                }*/

                else if (collider.CompareTag("NPC2")) //NPC 콜라이더 내에서만 텍스트 창 활성화 시키겠다!
                {
                    collider.GetComponent<TextSelectTrigger>().Trigger5();
                }

                else if (collider.CompareTag("Secret")) //NPC 콜라이더 내에서만 텍스트 창 활성화 시키겠다!
                {
                    collider.GetComponent<SecretMemoTrigger>().Trigger();
                }

                if (collider.CompareTag("PotionFream") && !playerControl.isInteractive)
                {
                    //collider.GetComponent<PotionFrame>().PotionOpenUI();
                    audioManager.Play("cau_Sound");

                    GameObject.Find("Potion").GetComponent<PotionFrame>().PotionOpenUI();

                    playerControl.isChat = true;

                    playerControl.isInteractive = true;

                    Debug.Log("실행");
                    break;

                }


            }
            if (testDialogue != null && !testDialogue.dialogueEnded && testDialogue.isInRange && interactButton)
            {
                if (databaseManager.GamblingUsed == true)
                {
                    Controlset.SetActive(false);

                    theDM.testDialogue = testDialogue; // 현재 대화 스크립트 인스턴스를 DialogueManager에 전달합니다.

                    theDM.ShowDialogue(testDialogue.dialogue);
                }

            }

            if (!theDM.isYesNoButtonsActive)
            {

                if (!theDM.isDialogueEnded)
                {

                    if (theDM.count < theDM.listSentences.Count - 1) // 대화가 끝나지 않았을 때
                    {
                        theDM.text.text = "";
                        theDM.theAudio.Play("enterSound");
                        StopAllCoroutines();
                        if (gameObject.activeInHierarchy)
                        {
                            Controlset.SetActive(true);
                            StartCoroutine(theDM.StartDialogueCoroutine());
                        }


                    }
                }
            }

        }

    }

    void Update()
    {
        if (playerControl.isPlayerDead) return;



        if (playerControl.isMoving)       // 이동중일 때! isMoving이 True일 때!
        {

            player.transform.position = Vector3.Lerp(player.transform.position, playerControl.targetPosition, playerControl.moveSpeed * Time.deltaTime);      // transform.position에서 targetPosition까지 moveSpeed*Time.deltaTime만큼 서서히 이동하는 보간 함수이고, 그 값을 transform.position에 대입해라.
                                                                                                                                                              // Vector3.Lerp(시작점, 끝점, 결과값)은 시작점과 끝점사이의 거리를 결과값을 바탕으로 평균화 시키는 보간 함수이다.
                                                                                                                                                              // 캐릭터의 현재 위치, 한 칸 이동까지의 거리를 서서히 이동시키는 구문이라고 이해하자!

            if (Vector3.Distance(player.transform.position, playerControl.targetPosition) < 0.01f)       // 거리를 비교하는 Distance (캐릭터의 현재 위치, 이동했을 시의 위치)가 0.01보다 작을 경우. 즉 곧 이동이 종료될 경우                              
            {

                player.transform.position = playerControl.targetPosition;                                // transform.position에 targetPosition을 대입하면서 종료시켜라.

                playerControl.isMoving = false;                                                   // 이동이 종료됐으니 이동중 변수인 isMoving을 false로 만들어라.

                playerControl.animator.SetBool("isMove", true);                                   // 애니메이터의 isMove 파라미터를 true로 한다. 여기서 true가 되면 연결된 방향의 Idle로 전환된다.

            }

        }

        if (playerControl.curTime >= 0)
        {
            playerControl.curTime -= Time.deltaTime;                          // 쿨타임에 Time.delTaTime 값을 빼라. 즉 서서히 감소시켜라!
        }

        if (playerControl.isCasting)                                          // 캐스팅 상태 일때
        {
            playerControl.castingTimer += Time.deltaTime;                     // 캐스팅 시간은 시간의 흐름에 따라 + 현재 시간이 누적

            float progress = playerControl.castingTimer / playerControl.castingTime;        // progress = 경과된 시간 / 캐스팅 종료 시간

            playerControl.progressUI.SetProgress(progress);                   // 경과 시간을 UI 프로그레스로 표기

            playerControl.progressUI.SetTimeRemaining(playerControl.castingTime - playerControl.castingTimer);        // 경과 시간을 Txt로 표기

            if (playerControl.castingTimer >= playerControl.castingTime)                    // 현재 시간이 캐스팅 시간보다 길어졌을 때 ( 2초 지났을 때 )
            {
                playerControl.isCasting = false;                              // 캐스팅 종료

                playerControl.castingTimer = 0f;                              // 다시 0으로 대입

                Collider2D[] colliders = Physics2D.OverlapBoxAll(playerControl.playerCollider.bounds.center, playerControl.playerCollider.bounds.size, 0);      // 검출해서

                foreach (Collider2D collider in colliders)      // 돌리고~
                {
                    if (collider.CompareTag("Box"))             // 박스면 - 이제 여기서 Update라 계속 플레이어가 접촉해있는지 확인하고, 안해있으면 종료시킴
                    {
                        collider.gameObject.GetComponent<GiveItem>().ItemDrop();        // 아이템 드랍해!

                        break;
                    }

                    else if (collider.CompareTag("Lever"))
                    {
                        collider.gameObject.GetComponent<Lever1>().DoorDestroy();

                        break;
                    }

                    else if (collider.CompareTag("Dust"))
                    {
                        collider.gameObject.GetComponent<GiveDust>().DustDrop();
                    }


                    else if (collider.CompareTag("Reward"))
                    {

                        collider.gameObject.GetComponent<StageClear>().StageEnd();
                    }

                }
                playerControl.progressUI.Hide();                              // 종료 시 UI 숨겨!
            }
        }
        theDM = FindObjectOfType<DialogueManager>();
        testDialogue = FindObjectOfType<TestDialogue>();
    }

    public void EnableControlSet()
    {
        Controlset.SetActive(true);
    }
    private void HandleDialogueEnd()
    {
        if (theDM != null)
        {
            Controlset.SetActive(true);
        }
    }
    private bool IsWallBetween(Vector3 start, Vector3 end)      // IsWallBetween(start, end) 함수. 매개 변수는 Vector3이고 (출발점, 도착점) 이다. 두개의 벡터 사이에 장애물이 있는지 판단하는 구문.
    {
        RaycastHit2D hit = Physics2D.Linecast(start, end, playerControl.wallLayer);       // 출발점과 도착점 사이의 선을 그리고 도착점이 wallLayer인지 체크해라. 그리고 그걸 hit이라는 변수에 반환해라. 여기서 장애물이 없다면 Null로 반환된다.

        return hit.collider != null;        // 검출된 콜라이더가 null이 아닐때, 즉 장애물이 있을 때 isWallBetween을 true로 반환하고, 없다면 false로 반환해라. false일 시만 이동할 수 있게 위에 조건문을 달아놨다!
    }



    void Start()
    {
        player = GameObject.Find("Player");
        playerControl = player.GetComponent<PlayerControl>();
        //targetPosition = player.transform.position;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        theDM.OnDialogueEnd += HandleDialogueEnd;
        Controlset = GameObject.Find("ControlSet");
        databaseManager = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressingUI = false;
        if (PlayerMoveCoroutine != null)
        {
            StopCoroutine(PlayerMoveCoroutine);

        }
    }
}

