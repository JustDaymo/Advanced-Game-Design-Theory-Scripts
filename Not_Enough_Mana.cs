using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Not_Enough_Mana : MonoBehaviour
{
    public GameObject NotEnoughMana;

    // Update is called once per frame
    void Update()
    {
        if (NotEnoughMana.activeSelf)
        {
            StartCoroutine(TurnSelfOff());
        }
    }

    IEnumerator TurnSelfOff()
    {
        yield return new WaitForSeconds(3f);
        NotEnoughMana.SetActive(false);
    }
}
