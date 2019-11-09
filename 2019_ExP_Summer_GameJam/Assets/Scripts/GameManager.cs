using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글턴 구현
    public static GameManager instance = null; 
    private void Awake()
    {
        
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        Screen.SetResolution(1080, 1920,true);
    }

    // 현재 배경의 이름
    public string backgroundName;

    // bgm
    public AudioSource bgm;

    // 플레이어 이름
    public string playerName;
 
    // 현재 스테이지
    public int nowStage = 0;
    
    // 장애물들 배열
    public string [] obstacles = new string[] { "None", "Chicken", "Car" };
    
    // 씬을 클리어 한 상태인지(T/F)
    public bool isClear;

    // Start is called before the first frame update
    void Start()
    {
        isClear = false;
    }

    // 진행 데이터 초기화
    public void ResetData()
    {
        nowStage = 0;
    }

    // 게임매니저의 nowStage 에 따라 json 파일을 불러와서 stageManager에 저장하는 함수.
    public void LoadStageInfo(ref StageInfo info)
    {
        string fileName = "Stage" + GameManager.instance.nowStage;
        string jsonFile = Resources.Load<TextAsset>(fileName).ToString();
       
        info = JsonUtility.FromJson<StageInfo>(jsonFile);

        // 사운드 장애물 json 파일
        ObstacleInfo ob;
        for (int i = 0; i < info.stageObstacles.Length; i++)
        {
            fileName = "Stage" + GameManager.instance.nowStage + "_Obstacle" + (i+1).ToString();
            jsonFile = Resources.Load<TextAsset>(fileName).ToString();
            ob = JsonUtility.FromJson<ObstacleInfo>(jsonFile);
            info.obstaclesInfo[i] = ob;
        }
       


    }
}
