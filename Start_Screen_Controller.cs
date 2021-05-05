using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Screen_Controller : MonoBehaviour
{
    public Camera Cam;
    Color BackGroundColor;
    float R;
    float G;
    float B;

    void Start()    // Start is called before the first frame update
    {
        BackGroundColor = new Color(R, G, B, 1);
        Cam.backgroundColor = BackGroundColor;
        //StartCoroutine(UpdateColor());
    }

 /*   IEnumerator UpdateColor()
    {
        R += 0.09f;
        G += 0.06f;
        B += 0.03f;

        if (R >= 0.5)
        {
            R = 0;
        }
        if (G >= 0.5)
        {
            G = 0;
        }
        if (B >= 0.5)
        {
            B = 0;
        }

        BackGroundColor = new Color(R, G, B);

        Cam.backgroundColor = BackGroundColor;

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(UpdateColor());
    }   */
    public void FightButton()
    {
        SceneManager.LoadScene("Battle");
    }

    public void OptionsButton()
    {
        SceneManager.LoadScene("Options");
    }

    public void CreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }
}