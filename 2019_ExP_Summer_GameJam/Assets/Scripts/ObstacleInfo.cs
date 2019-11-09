/*
 * Json 양식으로 받게 될 Obstacle의 명세 입니다.
 */

using UnityEngine;
[SerializeField]
public class ObstacleInfo 
{
    // 장애물의 이름
    public string obstacleName;

    // 스테이지 시작 몇초 후 부터 장애물 출력을 시작할지
    public float []startSoundSecond;

    // 몇초마다 이 장애물의 사운드를 출력할건지
    public float startSoundGap;

    // 이 장애물이 시간 간격마다 어느 확률로 출력이 될 것인지.
    public float startSoundProb;

    // 마지막 몇초 범위동안 소리가 안나올지
    public float[] endSoundSecond;


    // 확정된 것들 ( 사운드 시작 시간, 사운드 마감 시간)
    public float confirmedStartSoundSecond;
    public float confirmedEndSoundSecond;

}
