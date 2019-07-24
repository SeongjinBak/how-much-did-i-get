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
