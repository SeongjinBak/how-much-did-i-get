/*
 * 
 * 스테이지 씬을 관리하는 매니저 클래스 입니다.
 * 게임 초기정보 세팅, 매 라운드 별 사운드 재생, 답 후보 제시 등을 담당합니다.
 * 
 * 스테이지 정보와 그 스테이지에서 출력할 사운드, 종잇장 넘기는 횟수, 횟수의 시간 차,
 * 장애물 사운드, 그 장애물 사운드의 출력 주기 등을 Json 포맷으로 받아 와 각 변수에 배정 합니다.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StageManager : MonoBehaviour
{
    // 스테이지 정보
    [SerializeField]
    public StageInfo stageInfo;
    
    [SerializeField]
    // 랜덤값으로 확정된 종잇장의 갯수
    private int confirmedBillNumber;
    [SerializeField]
    // 랜덤값으로 확정된 종잇장을 세는 속도(1초단위)
    private float confirmedBillSpeed;
    [SerializeField]
    // 절대시간 타이머
    private float timer;
    // 돈 세는소리 오디오소스
    private AudioSource billAudioSource;
    // 돈 세는소리 오디오클립
    private AudioClip billSound;
    [SerializeField]
    // 장애물 오디오소스 배열
    private AudioSource[] obstacleSource = new AudioSource[8];
    // 장애물 오디오클립 배열
    private AudioClip[] obstacleSound = new AudioClip[11];


    // 장애물 각각이 가지는 gap timer
    private List<float> gapTimerList = new List<float>();

    // 최소 한번은 출력 되었는지(장애물이)
    private List<bool> gapInitialOutput = new List<bool>();

    // 답 후보 4개
    public GameObject[] substitutes = new GameObject[4];
    public Text[] texts = new Text[4];
    // 정답은 무엇일까요 이미지
    public GameObject theAnswerIs;
    int answerNum;

    // 까만 배경 이미지(안대)
    public Image blinder;
    // 백그라운드
    public SpriteRenderer backgroundSr;
    public Image backgroundImage;

    // 안대 쓴 상태(블라인더 상태)에서는 캐릭터들이 안보이게끔 한다.
    public GameObject male;
    // 여자 캐릭터 없애버림
    public GameObject female;

    // 몇회인지 알려주는 텍스트
    public Text episode;

    [SerializeField]
    // 14스테이지에서 몇번 세었는지를 저장하는 변수 (하이라이트 부분이므로 하드코딩)
    private int countFor14 =0;

    private void OnEnable()
    {

        // 스크립트 처음 시작시 여자 남자 캐릭터 꺼놓는다.
        male.SetActive(false);
        female.SetActive(false);
        // 몇 회인지 UI에 출력.
        episode.text = GameManager.instance.nowStage.ToString();
        // 게임 시작시 bgm 끈다
        GameManager.instance.bgm.enabled = false;
        // 정답들 이미지들 끈다.
        for(int i = 0; i < 4; i++)
        {
            substitutes[i].SetActive(false);
        }
        theAnswerIs.SetActive(false);


        // StageInfo 클래스를 생성한다.
        stageInfo = new StageInfo();
        if(GameManager.instance == null)
            Debug.Log("Null GameManager error.");
        
        // LoadStageInfo()로, 해당 스테이지의 Json 파일을 받아오고, 반영한다.
        GameManager.instance.LoadStageInfo(ref stageInfo);

       
        // 장애물의 오디오소스 지정. Json으로 받아온 장애물 사운드의 개수만큼 오디오 소스를 지정.
        for (int audio = 0; audio < stageInfo.stageObstacles.Length; audio++)
        {
            obstacleSource[audio] = GameObject.Find("Obstacle " + "(" + (audio + 1).ToString() + ")").GetComponent<AudioSource>();
        }

        // 장애물의 Json으로 받아온 장애물 사운드 오디오클립 지정
        for (int audio = 0; audio < stageInfo.stageObstacles.Length; audio++)
        {
            string tmpName = stageInfo.obstaclesInfo[audio].obstacleName;

            if (tmpName != "None")
                obstacleSound[audio] = Resources.Load<AudioClip>(tmpName);
        }

        // 돈 세는소리 오디오소스 지정
        billAudioSource = GameObject.Find("BillSound").GetComponent<AudioSource>();
        // 돈 세는소리 오디오클립
        billSound = Resources.Load<AudioClip>("Paper");

        // Json으로 받아와, 종잇장의 갯수 확정
        confirmedBillNumber = Random.Range(stageInfo.billRange[0], stageInfo.billRange[1]);
        // Json으로 받아와, 종잇장 세는 속도 확정
        confirmedBillSpeed = 1 / Random.Range(stageInfo.billSpeed[0], stageInfo.billSpeed[1]);
        if (GameManager.instance.nowStage == 14)
        {
            confirmedBillSpeed = 55f/confirmedBillNumber ;
        }

        // Json으로 받아와,장애물들의 값을 확정시키는 것.
        for (int i = 0; i < stageInfo.stageObstacles.Length; i++)
        {
            stageInfo.obstaclesInfo[i].confirmedStartSoundSecond = Random.Range(stageInfo.obstaclesInfo[i].startSoundSecond[0], stageInfo.obstaclesInfo[i].startSoundSecond[1]);
            stageInfo.obstaclesInfo[i].confirmedEndSoundSecond = Random.Range(stageInfo.obstaclesInfo[i].endSoundSecond[0], stageInfo.obstaclesInfo[i].endSoundSecond[1]);
            // 각각의 갭 타이머 추가
            gapTimerList.Add(150f);
            // 최소한번 출력되었는지 판단변수
            gapInitialOutput.Add(false);
        }
        
        // 타이머는 0.0f로 시작.
        timer = 0.0f;
        // 게임 실행 코루틴 시작
        StartCoroutine(PlayStage());
    }

    private IEnumerator PlayStage()
    {
        // 시작 2초는 무조건 기다린다.
        while (timer <= 2f)
        {
            yield return new WaitForSeconds(0.02f);
        }

        int currentBillNumber = 0;
        float localBillTimer = 0f;
        float gapTime = timer;
        // 현재 종잇장의 개수가 확정된 종잇장의 개수보다 작을때
        // 즉, 종잇장 세는 소리 들릴 때.
        while (GameManager.instance.nowStage == 14 || currentBillNumber < confirmedBillNumber)
        {
            if (GameManager.instance.nowStage == 14)
            {
                if (timer >= 53f)
                {
                    break;
                }
            }
            localBillTimer += Time.deltaTime;
            
            // 확정된 종잇장의 매 속도마다 종잇장 세기 출력
            if (localBillTimer >= confirmedBillSpeed)
            {
                currentBillNumber++;
                localBillTimer = 0f;
                if(GameManager.instance.nowStage == 14)
                {
                    billAudioSource.PlayOneShot(billSound, 0.9f);
                }
                else
                    billAudioSource.PlayOneShot(billSound);
             
                // 종잇장 세는 속도 다시 확정
                confirmedBillSpeed = 1 / Random.Range(stageInfo.billSpeed[0], stageInfo.billSpeed[1]);
                if (GameManager.instance.nowStage == 14)
                {
                    countFor14++;
                    confirmedBillSpeed = 53f / confirmedBillNumber;
                }
            }

            // 장애물이 출력 되었는지 확인하는 플래그
            bool isPlayed = false;
            float probability = Random.Range(0f, 1f);
            // 장애물의 출력
            for (int i = 0; i < stageInfo.stageObstacles.Length; i++)
            {
                // 최소 대기시간을 넘겼을 때부터 장애물 소리가 출력 되게끔 분기 설정
                // endsoundsecond 이후부터는(timer 가 confirmedEndSecond 보다 커진다면) 전혀 소리가 나면 안된다.
                if (stageInfo.obstaclesInfo[i].confirmedStartSoundSecond < timer && timer < stageInfo.obstaclesInfo[i].confirmedEndSoundSecond)
                {  
                    // 난수로 생성한 확률변수가, 정해놓았던 확률보다 작거나 같을경우에만 사운드 출력, 갭 타이머 마다 확률계산함.
                    if (probability <= stageInfo.obstaclesInfo[i].startSoundProb)
                    {
                        if (gapTimerList[i] > stageInfo.obstaclesInfo[i].startSoundGap)
                        {
                            // Start Obstacle Sound..
                            isPlayed = true;
                            // 장애물의 사운드 출력
                            if (stageInfo.obstaclesInfo[i].obstacleName != "None")
                            {
                                if (GameManager.instance.nowStage == 10)
                                    obstacleSource[i].PlayOneShot(obstacleSound[i], 0.85f);
                                else
                                    obstacleSource[i].PlayOneShot(obstacleSound[i]);
                                // 각 장애물별 갭타임 초기화
                                gapTimerList[i] = 0f;
                                // 출력되었음을 알림
                                gapInitialOutput[i] = true;
                            }
                        }
                    }
                }
            } 
            
            // 장애물의 출력 반복문 종료
            if (isPlayed == true)
            {
                gapTime = timer;
                isPlayed = false;
            }

            yield return new WaitForSeconds(0.02f);
            localBillTimer += 0.02f;
        }


        timer = 0.0f;
        if (GameManager.instance.nowStage == 14)
        {
            while (obstacleSource[0].isPlaying)
            {
                yield return new WaitForSeconds(0.02f);
            }
        }
        else
        {
            // 마지막 2초는 무조건 기다린다.
            while (timer <= 2f)
            {
                yield return new WaitForSeconds(0.02f);
            }
        }
       
        
        // 게임 종료시 작동할 코드
        // 재생 종료
        for (int audio = 0; audio < stageInfo.stageObstacles.Length; audio++)
        {
            obstacleSource[audio].Stop();
        }
        // 정답 출력하는 코드
        StartCoroutine(CallCheckBoard());
    }

    IEnumerator CallCheckBoard()
    {

        yield return new WaitForSeconds(0.01f);
        // 게임 종료시 bgm 켠다
        GameManager.instance.bgm.enabled = true;
        if (GameManager.instance.backgroundName == "InBank")
        {
           backgroundImage.sprite = Resources.Load<Sprite>("InBank_B");
        }
        else
        {
            backgroundImage.sprite = Resources.Load<Sprite>("OutBank_B");
        }

        // 답 맞추는 경우 남여 캐릭터 켬
        male.SetActive(true);
        answerNum = Random.Range(0, 4);
        int[] answerArray = new int[4];
        if (GameManager.instance.nowStage == 14)
        {
            confirmedBillNumber = countFor14;
            female.SetActive(false);
        }
        else
        {
            female.SetActive(true);
        }
        
        // 4지선다 중 하나에 정답을 저장.
        answerArray[answerNum] = confirmedBillNumber;
       
        // 정답과 이웃한 수로 가짜답 생성한다.
        for (int i = 0; i < 4; i++)
        {
            if(i != answerNum)
            {
                if(i < answerNum)
                {
                    answerArray[i] = confirmedBillNumber - (answerNum - i); 
                }
                else
                {
                    answerArray[i] = confirmedBillNumber + (i - answerNum);  
                }
            }
        }

        // 답지용 이미지들 전부 켠다.
        for (int i = 0; i < 4; i++)
        {
            substitutes[i].SetActive(true);
            texts[i].text = answerArray[i].ToString();
        }
        theAnswerIs.SetActive(true);
        blinder.enabled = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        // 매 Update문마다, 타이머 값 증가.
        timer += Time.deltaTime;
        for (int i = 0; i < stageInfo.stageObstacles.Length; i++)
        {
            if(gapInitialOutput[i] != false)
            {
                gapTimerList[i] += Time.deltaTime;
            }
        }
    }


    // 버튼 클릭용 함수.
    public void Clicked_0()
    {
        if(0 == answerNum)
        {
            // 스테이지 1 증가
            GameManager.instance.nowStage++;
            GameManager.instance.isClear = true;
            SceneManager.LoadScene("Story");
        }
        else
        {
            SceneManager.LoadScene("Story");
        }
    }
    public void Clicked_1()
    {
        if (1 == answerNum)
        {
            // 스테이지 1 증가
            GameManager.instance.nowStage++;
            GameManager.instance.isClear = true;
        }
        else
        {
            SceneManager.LoadScene("Story");
        }
    }
    public void Clicked_2()
    {
        if (2 == answerNum)
        {
            // 스테이지 1 증가
            GameManager.instance.nowStage++;
            GameManager.instance.isClear = true;
        }
        else
        {
            SceneManager.LoadScene("Story");
        }
    }
    public void Clicked_3()
    {
        if (3 == answerNum)
        {
            // 스테이지 1 증가
            GameManager.instance.nowStage++;
            GameManager.instance.isClear = true;
            SceneManager.LoadScene("Story");
        }
        else
        {
            SceneManager.LoadScene("Story");
        }
    }

}
