using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Script : MonoBehaviour
{
    public Text PlayerName;
    public Text PlayerHP;
    public Text PlayerMP;
    public Slider HPBar;
    public Slider MPBar;
    
    public void StartHUD(Fighter fighter)

    {
        PlayerName.text = fighter.Name;

        HPBar.maxValue = fighter.MaxHP;
        HPBar.value = fighter.CurrentHP;
        PlayerHP.text = fighter.CurrentHP + "/" + fighter.MaxHP;

        MPBar.maxValue = fighter.MaxMP;
        MPBar.value = fighter.CurrentMP;
        PlayerMP.text = fighter.CurrentMP + "/" + fighter.MaxMP;
    }

    public void UpdateHP(float HP)
    {
        HPBar.value = HP;
    }
    public void UpdateMP(float MP)
    {
        MPBar.value = MP;
    }

    public void UpdateHUD(Fighter fighter) 
    {
        PlayerHP.text = fighter.CurrentHP + "/" + fighter.MaxHP;
        PlayerMP.text = fighter.CurrentMP + "/" + fighter.MaxMP;
    }
}