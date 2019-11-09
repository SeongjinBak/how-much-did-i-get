/*
 * 게임 처음 시작시 나오는 메인 화면 스크립트 입니다.
 * 아무 입력을 받게 되면, 스토리 씬으로 넘어가게 됩니다.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainTitle : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Story");
        }
        
    }
}
