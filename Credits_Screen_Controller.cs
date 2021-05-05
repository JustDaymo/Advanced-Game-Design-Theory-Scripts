using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits_Screen_Controller : MonoBehaviour
{
    public void BackButton()
    {
        SceneManager.LoadScene("StartMenu");
    }
}