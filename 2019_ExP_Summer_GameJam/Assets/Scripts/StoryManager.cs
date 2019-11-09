/*
 * 사운드를 듣는 게임화면이 아닌 대화창 출력 및 라운드 정보 표현을 위한 클래스 입니다.
 * 백그라운드 이미지, 애니메이션, 대화창 출력을 담당합니다.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{

    // 이름 입력 창 2개(인풋필드, 버튼)
    public GameObject inputField, confirmButton;

    // 제 몇회인지
    public Text episode;
    // 백그라운드 이미지
    public SpriteRenderer backgroundSr;
    public Image backgroundImage;

    // 플레이어 이름 텍스트
    public Text playerNameInput;

    // 대화창 출력 텍스트필드
    public Text dialogueText;
    
    // 대화창 출력 화자의 이름
    public Text dialogueSpeaker;

    // 대화창 출력 배경 이미지
    public Image dialogueImage;

    // 대화창에 출력할 문자열들
    public List<string> myDialogues;
    // 화자는 누구인가

    // 말할때마다 나오게 할 애니메이션
    public GameObject[] male;
    public GameObject[] female;

    // 페이드아웃
    public SpriteRenderer fadeOut;

    // 오답인지, 정답인지 지정
    private bool finish;

    private void Start()
    {
        // 화면에 에피소드 출력
        if(GameManager.instance.nowStage == 0)
            episode.text = (GameManager.instance.nowStage + 1).ToString();
        else
        {
            episode.text = (GameManager.instance.nowStage).ToString();
        }

        if(GameManager.instance.nowStage == 15)
        {
            female[0].SetActive(false);
            female[1].SetActive(false);
            episode.text = (14).ToString();
        }

        // 백그라운드 이미지
        int randomBackGround = Random.Range(0, 2);
        if(randomBackGround == 0)
        {
            GameManager.instance.backgroundName = "InBank";
            backgroundImage.sprite = Resources.Load<Sprite>("InBank_A");
        }
        else
        {
            GameManager.instance.backgroundName = "OutBank";
            backgroundImage.sprite = Resources.Load<Sprite>("OutBank_A");
        }


        inputField.SetActive(false);
        confirmButton.SetActive(false);
        finish = false;

        // 스테이지에 따라서 출력할 문자열들을 정해줌.
        // 스테이지가 많지않고, 비슷한 내용의 대화를 하므로 하드코딩 하였음.
        // 게임잼 특성상 마무리단계에서 시간을 많이 투자할 수 없었음.
        if(GameManager.instance.nowStage == 0)
        {
            myDialogues.Add("안녕하세요 시청자 여러분. 제가 지금 서 있는 곳은 서울의 한 은행입니다.");
            myDialogues.Add("오늘 만나볼 달인은 은행 경력 23년에 달하는 돈 세기의 달인이신데요. 인사 한번 부탁드려요.");
            myDialogues.Add("안녕하세요~");
            myDialogues.Add("소문에 의하면 달인 님은 은행 내의 모든 돈 세는 소리를 들을 수 있다던데 사실인가요?");
            myDialogues.Add("네~ 맞습니다. 아무리 주변이 시끄러워도 돈 세는 소리만큼은 ASMR처럼 선명합니다.");
            myDialogues.Add("그래서~ 저희가 준비했습니다. 오늘의 달인 미션!");
            myDialogues.Add("???!");
            myDialogues.Add("미션은 간단합니다. 달인 님께선 이제 안대를 쓰시고 저희가 세는 지폐의 수를 맞춰주시면 됩니다.");
            myDialogues.Add("그런건 제 전문이죠. 맡겨만 주세요~");
            myDialogues.Add("네~ 그러면 안대 씌워드리겠습니다! 저희가 지폐를 다 세면, 몇장을 셌는지 맞추어 주시면 됩니다~");
            myDialogues.Add("이어폰이나 헤드셋 착용을 권장합니다. 그럼 미션 시작하겠습니다!");
        }
        else if(GameManager.instance.nowStage > 14)
        {
            myDialogues.Add("역시 달인이십니다! 달인을 진정한 돈 세기의 달인으로 임명해드리겠습니다! 축하합니다!");
            myDialogues.Add("그럼 저는 이제 이 작가를 좀 말리러 가보겠습니다! 오늘 방송 고생 많으셨습니다!");
        }
        else
        {
            if(GameManager.instance.isClear == true)
            {
                myDialogues.Add("정답입니다! 그럼 "+GameManager.instance.nowStage+ "번째 미션을 시작하겠습니다~ 다시 안대를 씌워주세요.");						
            }
            else
            {
                finish = true;
                myDialogues.Add("오답입니다! 오늘 달인 방송은 여기까집니다. 시청자 여러분 다음 주에 만나요~");
                myDialogues.Add("자..잠깐만요..!");
            }
        }

        // 대화 코루틴 시작
        StartCoroutine(StartDialogue(GameManager.instance.nowStage));
    }

    // 대화 출력하는 코루틴
    private IEnumerator StartDialogue(int stage)
    {
        yield return new WaitForSeconds(0.2f);
        dialogueText.text = "";

        int cnt = 0;
        // 대화창 두번 눌림을 방지하기 위함
        bool flag = false;
        while (cnt < myDialogues.Count && flag == false)
        {
            if(GameManager.instance.nowStage == 0)
            {
                if (cnt == 0 || cnt == 3 || cnt == 7 || cnt == 10)
                    dialogueSpeaker.text = "장PD";
                else if (cnt == 1 || cnt == 5 || cnt == 9)
                    dialogueSpeaker.text = "이작가";
                else
                    dialogueSpeaker.text = "나";
            }
            else if(GameManager.instance.nowStage > 14)
            {
                dialogueSpeaker.text = "장PD";
            }
            else
            {
                if(GameManager.instance.isClear == true)
                {
                    dialogueSpeaker.text = "장PD";
                    GameManager.instance.isClear = false;
                }
                else
                {
                    if (cnt == 0)
                        dialogueSpeaker.text = "이작가";
                    else
                    {
                        dialogueSpeaker.text = "나";
                    }
                }
            }

            // 화자에 따라 이미지가 바뀐다.
            if(dialogueSpeaker.text == "장PD")
            {
                male[1].SetActive(true);
                male[0].SetActive(false);
                female[1].SetActive(false);
                female[0].SetActive(true);
                if (GameManager.instance.nowStage == 15)
                {
                    female[0].SetActive(false);
                    female[1].SetActive(false);
                }
            }
            else if(dialogueSpeaker.text == "이작가")
            {
                male[1].SetActive(false);
                male[0].SetActive(true);
                female[1].SetActive(true);
                female[0].SetActive(false);
            }
            else
            {
                male[1].SetActive(false);
                male[0].SetActive(true);
                female[1].SetActive(false);
                female[0].SetActive(true);
            }
           
            for(int i = 0; i < myDialogues[cnt].Length; i++)
            {
                if (i%16 ==0 && i != 0) dialogueText.text += '\n';
                dialogueText.text += myDialogues[cnt][i];
                yield return new WaitForSeconds(0.02f);
            }
            flag = true;
            while (!Input.anyKey)
            {
                yield return new WaitForSeconds(0.02f);

            }
            dialogueText.text = "";
            flag = false;
            cnt++;
            yield return new WaitForSeconds(0.1f);
        }

        // 마지막엔 대화 문자열들 클리어
        myDialogues.Clear();

        dialogueImage.enabled = false;
        dialogueSpeaker.enabled = false;
        dialogueText.enabled = false;
        if(GameManager.instance.nowStage == 0)
            GameManager.instance.nowStage++;
        StartCoroutine(Blinder());
    }
    
    // Fade Out coroutine
    IEnumerator Blinder()
    {
        
        Color tmp = new Color(0f, 0f, 0f,0f);
        while (tmp.a< 1)
        {
            tmp.a += 0.01f;
            fadeOut.color = tmp;
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(0.7f);
        if (finish)
        {
            GameManager.instance.ResetData();
            SceneManager.LoadScene("Main");
        }
        else
        {
            if(GameManager.instance.nowStage > 14)
            {
                // 크레딧 화면으로 이동.
                SceneManager.LoadScene("Credit");
            }
            else
                SceneManager.LoadScene("GameStage");
        }
            

    }

    public void NameClicked()
    {
        GameManager.instance.playerName = playerNameInput.text;
    }
}
