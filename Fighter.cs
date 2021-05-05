using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    //This contains all the variables a character in the game will have.
    public string Name;

    public float MaxHP;
    public float MaxMP;
    public float CurrentHP;
    public float CurrentMP;
    public float Strength;
    public float Intelligence;

    public bool IsDefending;

    public int FrozenTurns;
    public int TimedEarthTurns;
    public int BurnedTurns;
    public int PoisonedTurns;

    public Animator Anim;


    public List<string> CurrentStatuses;

    public void TakeDamage(float damage)
    {
        {
            CurrentHP -= damage;
        }
    }
}
