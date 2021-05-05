using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CombatState { START, PLAYERTURN, ENEMYTURN, VICTORY, DEFEAT}

public class Combat_System : MonoBehaviour
{
    public CombatState state;

    public GameObject PlayerPrefab;
    public GameObject PlayerMenu;
    public GameObject PlayerSpellMenu;
    public GameObject EnemyPrefab;
    public GameObject PlayerTurnIcon;
    public GameObject EnemyTurnIcon;
    public GameObject DamageTaken;
    public GameObject NotEnoughMana;
    public GameObject Ice;
    public GameObject Earth;
    public GameObject EarthTimer;
    public GameObject Fire;
    public GameObject Poison;
    public GameObject EndBattlePanel;


    public Transform PlayerGround;
    public Transform EnemyGround;
    public Transform PlayerTurnPositon;
    public Transform EnemyTurnPosition;
    public Transform EnemyDamagePosition;
    public Transform PlayerDamagePosition;

    public AudioSource AudioPlayer;
    public AudioClip PlayerAttackSound;
    public AudioClip PlayerHurtSound;
    public AudioClip EnemyAttackSound;
    public AudioClip EnemyHurtSound;

    public HUD_Script PlayerHUD;
    public HUD_Script EnemyHUD;

    bool checkingStatus;
    bool endTurnFromStatus;

    float EnemyTrueStr;

    Fighter Player;
    Fighter Enemy;

    public string[] Statuses; // 0 = Frozen, 1 = RockOverHead, 2 = Burned, 3 = Poisoned

    void Start()    // Start is called before the first frame update
    {
        AudioPlayer = GetComponent<AudioSource>();  // Sets Audioplayer to be able to play sounds.
        state = CombatState.START;  //Sets the combat state.
        StartBattle(); // Starts the StartBattle function and allows the use of a pause.
    }

    void StartBattle()
    {
        GameObject PlayerStart = Instantiate(PlayerPrefab, PlayerGround); // Spawns the player and enemy on their ground sprite and allows them to access their stats.
        Player = PlayerStart.GetComponent<Fighter>();

        GameObject EnemyStart = Instantiate(EnemyPrefab, EnemyGround);
        Enemy = EnemyStart.GetComponent<Fighter>();

        PlayerHUD.StartHUD(Player); // Spawns the HUD for the player and enemy.
        EnemyHUD.StartHUD(Enemy);

        state = CombatState.PLAYERTURN; // Begins player's turn.
        EnemyTrueStr = Enemy.Strength;  // Makes sure we save the set strength of the enemy so we can restore it once the burn effect is done.
        PlayerTurn();
    }

    void PlayerTurn()
    {
        Instantiate(PlayerTurnIcon, PlayerTurnPositon); // Spawns in player turn icon.
        Destroy(GameObject.Find("Enemy Turn Icon(Clone)")); // Destroy the enemy icon.
        PlayerMenu.SetActive(true); // Enables the menu to be visbile and interactable.
        Player.Anim.SetBool("Attacking", false);
    }

    IEnumerator EnemyTurn() // Operates more or less the same as PlayerAttack.
    {
        HidePlayerMenus();
        Instantiate(EnemyTurnIcon, EnemyTurnPosition);
        Destroy(GameObject.Find("Player Turn Icon(Clone)")); // Looks for player icon and destroys it.

        StartCoroutine(CheckStatus());

        while (checkingStatus) 
        {
            yield return null;
        }

        if (endTurnFromStatus) 
        {
            endTurnFromStatus = false;
            yield break;
        }

        Enemy.Anim.SetBool("Attacking", true);

        if (Player.IsDefending) // If the player is defending, run TakeDamage with half of the Enemy Strength, if not, full strength.
        {
            Player.TakeDamage (Mathf.Ceil (Enemy.Strength / 2));
            DisplayPlayerDamage (Mathf.Ceil (Enemy.Strength / 2));
            Player.IsDefending = false;
        }
        else
        {
            Player.TakeDamage (Mathf.Ceil (Enemy.Strength));
            DisplayPlayerDamage (Mathf.Ceil (Enemy.Strength));
        }

        AudioPlayer.PlayOneShot(EnemyAttackSound);
        AudioPlayer.PlayOneShot(PlayerHurtSound);
        UpdateAllHUDs();

        yield return new WaitForSeconds(2f);

        Enemy.Anim.SetBool("Attacking", false);
        CheckPlayerDead();
    }

    void EndBattle()
    {
        if (state == CombatState.VICTORY)
        {
            EndBattlePanel.SetActive(true);
            EndBattlePanel.transform.GetChild(0).GetComponent<Text>().text = "You won!";
            Time.timeScale = 0f;
        }
        if (state == CombatState.DEFEAT)
        {
            EndBattlePanel.SetActive(true);
            EndBattlePanel.transform.GetChild(0).GetComponent<Text>().text = "You lost.";
            Time.timeScale = 0f;
        }
    }

    IEnumerator PlayerAttack() // Attack button.
    {
        PlayerMenu.SetActive(false);    // Removes the player menu temporarly.

        EnemyHurtSounds();
        Player.Anim.SetBool("Attacking", true);
        Enemy.TakeDamage(Player.Strength);
        DisplayEnemyDamage(Player.Strength);

        UpdateAllHUDs();

        yield return new WaitForSeconds(2f);

        CheckEnemyDead();
        SetEnemyTurn();
    }

    IEnumerator PlayerSpellsMenu()
    {
        PlayerMenu.SetActive(false);
        PlayerSpellMenu.SetActive(true);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator PlayerDefend()
    {
        HidePlayerMenus();
        Player.IsDefending = true;
        yield return new WaitForSeconds(2f);
        SetEnemyTurn();
    }

    IEnumerator PlayerFlee()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("StartMenu");
    }

    IEnumerator PlayerFreezeRay()
    {
        HidePlayerMenus();
        Player.Anim.SetBool("Attacking", true);
        EnemyHurtSounds();

        Player.CurrentMP -= 1100;
        Enemy.TakeDamage(Mathf.Ceil(Player.Intelligence * 0.8f)); // Runs the TakeDamage function with a float attached that is the result of the equation.
        DisplayEnemyDamage(Mathf.Ceil(Player.Intelligence * 0.8f)); // ^ Same but for the UI.
        Enemy.CurrentStatuses.Add(Statuses[0]); // Adds Frozen status and sets it to 1.
        Enemy.FrozenTurns = 1;
        Ice.SetActive(true);

        UpdateAllHUDs();

        yield return new WaitForSeconds(2f);
        CheckEnemyDead();
        SetEnemyTurn();
    }

    IEnumerator PlayerTimedEarth() 
    {
        HidePlayerMenus();
        Player.Anim.SetBool("Attacking", true);
        EnemyHurtSounds();

        Player.CurrentMP -= 300;
        UpdateAllHUDs();
        Enemy.CurrentStatuses.Add(Statuses[1]); 
        Enemy.TimedEarthTurns = 1;
        Earth.SetActive(true);
        EarthTimer.SetActive(true);

        yield return new WaitForSeconds(2f);

        SetEnemyTurn();
    }

    IEnumerator PlayerFlameSphere()
    {
        HidePlayerMenus();
        Fire.SetActive(true);
        Player.Anim.SetBool("Attacking", true);
        EnemyHurtSounds();

        Player.CurrentMP -= 200;
        Enemy.CurrentStatuses.Add(Statuses[2]);
        Enemy.BurnedTurns = 3;
        Enemy.TakeDamage(Mathf.Ceil(Player.Intelligence * 0.4f));
        DisplayEnemyDamage(Mathf.Ceil(Player.Intelligence * 0.4f));

        UpdateAllHUDs();
        CheckEnemyDead();

        yield return new WaitForSeconds(2f);

        Fire.SetActive(false);
        SetEnemyTurn();
    }

    IEnumerator PlayerPoisonDart()
    {
        HidePlayerMenus();
        Player.Anim.SetBool("Attacking", true);
        EnemyHurtSounds();
        Poison.SetActive(true);

        Player.CurrentMP -= 100;
        Enemy.CurrentStatuses.Add(Statuses[3]);
        Enemy.PoisonedTurns = 5;
        Enemy.TakeDamage(Mathf.Ceil(Player.Intelligence * 0.1f));
        DisplayEnemyDamage(Mathf.Ceil(Player.Intelligence * 0.1f));

        UpdateAllHUDs();
        CheckEnemyDead();

        yield return new WaitForSeconds(2f);

        SetEnemyTurn();
    }

    IEnumerator CheckStatus()
    {
        checkingStatus = true;

        if (Enemy.CurrentStatuses.Contains("Burned"))
        {
            if (Enemy.BurnedTurns > 0)
            {
                Enemy.Strength = Mathf.Ceil(EnemyTrueStr / 1.5f);   // Looks at the original strength, divides it by 1.5 and then sets the current strength to that.
                UpdateAllHUDs();
                Enemy.BurnedTurns -= 1;
            }
            else
            {
                Enemy.Strength = EnemyTrueStr;  //  Resets the strength to the original amount.
                Enemy.CurrentStatuses.Remove("Burned");
            }
        }

        if (Enemy.CurrentStatuses.Contains("RockOverHead")) // Checks for the RockOverHead status set by Timed Earth.
        {
            if (Enemy.TimedEarthTurns > 0)  // If Timed Earth has turns to spend, take away 1, continue checking other statuses.
            {
                Enemy.TimedEarthTurns--;
            }
            else
            {
                Enemy.CurrentStatuses.Remove("RockOverHead");   // As RockOverHead is now executing it's purpose, the enemy no longer needs that status.
                if (Enemy.CurrentStatuses.Contains("Frozen"))   // If the eneny is also frozen, deal double damage, if not, do normal damage.
                {
                    Enemy.TakeDamage(Mathf.Ceil(Player.Intelligence * 2f));  // If the enemy is frozen, do bonus damage.
                    DisplayEnemyDamage(Mathf.Ceil(Player.Intelligence * 2f));   // Display said damage.
                    AudioPlayer.PlayOneShot(EnemyHurtSound);
                }
                else
                {
                    Enemy.TakeDamage(Mathf.Ceil(Player.Intelligence));
                    DisplayEnemyDamage(Mathf.Ceil(Player.Intelligence));
                    AudioPlayer.PlayOneShot(EnemyHurtSound);
                }
                Earth.SetActive(false);
                EarthTimer.SetActive(false);
                CheckEnemyDead();   // Since this status can kill, it needs to check if the enemy is dead.
            }
        }

        if (Enemy.CurrentStatuses.Contains("Poisoned"))
        {
            if (Enemy.PoisonedTurns > 0)
            {
                Enemy.TakeDamage(Mathf.Ceil(Player.Intelligence / 3f));
                DisplayEnemyDamage(Mathf.Ceil(Player.Intelligence / 3f));
                AudioPlayer.PlayOneShot(EnemyHurtSound);
                UpdateAllHUDs();
                Enemy.PoisonedTurns -= 1;
            }
            else
            {
                Enemy.CurrentStatuses.Remove("Poisoned");
                Poison.SetActive(false);
            }
            CheckEnemyDead();
        }

        if (Enemy.CurrentStatuses.Contains("Frozen")) //Check if Frozen.
        {
            if (Enemy.FrozenTurns > 0)
            {
                Enemy.FrozenTurns -= 1;                 //Count down frozen turns.              
                UpdateAllHUDs();                        //Update the huds.
                yield return new WaitForSeconds(2f);
                state = CombatState.PLAYERTURN;         //Change play state.
                PlayerTurn();                           //Load player turn.
                endTurnFromStatus = true;
                yield break;                            //Exits the coroutine.
            }
            else
            {
                Enemy.CurrentStatuses.Remove("Frozen");
                Ice.SetActive(false);
            }
        }
        checkingStatus = false;
    }

    void DisplayPlayerDamage(float Dmg)
    {
        GameObject DamageDisplay = Instantiate(DamageTaken, PlayerDamagePosition) as GameObject;
        DamageDisplay.transform.GetChild(0).GetComponent<TextMesh>().text = Dmg + "";   // Spawns the damage text at the damage position and changes the text to match the damage of the enemy.
    }

    void DisplayEnemyDamage(float Dmg)
    {
        GameObject DamageDisplay = Instantiate(DamageTaken, EnemyDamagePosition) as GameObject;
        DamageDisplay.transform.GetChild(0).GetComponent<TextMesh>().text = Dmg + "";
    }
    
    void CheckPlayerDead()
    {
        if (Player.CurrentHP <= 0)
        {
            state = CombatState.DEFEAT;
            EndBattle();
        }
        else
        {
            state = CombatState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void CheckEnemyDead()
    {
        if (Enemy.CurrentHP <= 0)  // If the enemy dies, player wins. If the enemy is still alive, move to enemies turn.
        {
            state = CombatState.VICTORY;
            EndBattle();
        }
        else
        {
            return;
        }
    }

    void SetEnemyTurn()
    {
        state = CombatState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    void EnemyHurtSounds() // Plays two sounds at the same time.
    {
        AudioPlayer.PlayOneShot(PlayerAttackSound);
        AudioPlayer.PlayOneShot(EnemyHurtSound);
    }

    void UpdateAllHUDs()
    {
        PlayerHUD.UpdateHP(Player.CurrentHP);
        EnemyHUD.UpdateHP(Enemy.CurrentHP);
        PlayerHUD.UpdateMP(Player.CurrentMP);
        EnemyHUD.UpdateMP(Enemy.CurrentMP);
        PlayerHUD.UpdateHUD(Player);
        EnemyHUD.UpdateHUD(Enemy);
    }

    void HidePlayerMenus()
    {
        PlayerMenu.SetActive(false);
        PlayerSpellMenu.SetActive(false);
    }

    public void AttackButton() //Allows the Attack button to begin PlayerAttack.
    {
        StartCoroutine(PlayerAttack());
    }

    public void SpellsButton()
    {
        StartCoroutine(PlayerSpellsMenu());
    }

    public void DefendButton()
    {
        StartCoroutine(PlayerDefend());
    }

    public void FleeButton()
    {
        StartCoroutine(PlayerFlee());
    }

    public void FreezeRayButton() 
    {
        if (Player.CurrentMP < 600)
        {
            NotEnoughMana.SetActive(true);
            return;
        }
        StartCoroutine(PlayerFreezeRay());
    }

    public void TimedEarthButton()
    {
        if (Player.CurrentMP < 300)
        {
            NotEnoughMana.SetActive(true);
            return;
        }
        StartCoroutine(PlayerTimedEarth());
    }

    public void FlameSphereButton()
    {
        if (Player.CurrentMP < 200)
        {
            NotEnoughMana.SetActive(true);
            return;
        }
        StartCoroutine(PlayerFlameSphere());
    }

    public void PoisonDartButton()
    {
        if (Player.CurrentMP < 100)
        {
            NotEnoughMana.SetActive(true);
            return;
        }
        StartCoroutine(PlayerPoisonDart());
    }

    public void ReturnButton()
    {
        PlayerSpellMenu.SetActive(false);
        PlayerMenu.SetActive(true);
    }

    public void BackToTheStartButton()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1f;
    }
}