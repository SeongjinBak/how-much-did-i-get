using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class StageInfo 
{
    // 이 스테이지에서 사용할 장애물 배열
    public string[] stageObstacles;
    // 이 스테이지에서 나올 종잇장의 갯수, randMin, randMax
    public int[] billRange;
    // 종잇장을 세는 속도( 단위는 1초단위, 이 값은 1초에 x번 세는것) 
    public float[] billSpeed;
    // 장애물 구조체의 배열
    public ObstacleInfo[] obstaclesInfo = new ObstacleInfo[8];
}
