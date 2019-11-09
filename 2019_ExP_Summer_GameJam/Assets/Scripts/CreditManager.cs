/*
 *  크레딧 씬에서 화면이 터치될 때 까지 크레딧 화면을 보여주는 스크립트 입니다.
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            GameManager.instance.ResetData();
            SceneManager.LoadScene("Main");
        }
    }
}
