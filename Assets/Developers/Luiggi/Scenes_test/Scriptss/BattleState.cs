using UnityEngine;
using TMPro;
using System.Collections;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    
    Unit playerUnit;
    Unit enemyUnit;
    public TMP_Text dialogueText;

    public BattleState state;

    private int lastEnemySpeechIndex = -1; // evita stessa frase consecutiva

    private bool isProcessing = false;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "Il cattivo " + enemyUnit.unitName + " si sta avvicinando...";
        yield return new WaitForSeconds(1f);

        dialogueText.text = "Pensavi di aver ripulito la RAM?";
        yield return new WaitForSeconds(1f);

        dialogueText.text = "Illuso.";
        yield return new WaitForSeconds(1f);

        dialogueText.text = "Finché io avrò il controllo della CPU,";
        yield return new WaitForSeconds(1f);

        dialogueText.text = "tu sarai solo un bit di scarto destinato all'overflow!";
        yield return new WaitForSeconds(1f);

        dialogueText.text = "[BOSS BATTLE: PROFESSOR BURLONE]";
        yield return new WaitForSeconds(1f);

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

IEnumerator PlayerAttack(int damageToDeal)
{
    bool isDead = enemyUnit.TakeDamage(damageToDeal);
    enemyHUD.SetHP(enemyUnit.currentHP);
    enemyUnit.GetComponent<EffettoVibrazione>()?.IniziaVibrazione();

    if (damageToDeal > playerUnit.damage)
        dialogueText.text = "COLPO CRITICO! Hai tolto " + damageToDeal + " HP di vita al " + enemyUnit.unitName + "!";
    else
        dialogueText.text = "Hai tolto " + damageToDeal + " HP di vita al " + enemyUnit.unitName + "!";

    yield return new WaitForSeconds(3f);
    isProcessing = false; // ← solo qui, dopo l'attesa

    if (isDead)
    {
        state = BattleState.WON;
        EndBattle();
    }
    else
    {
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
}

    IEnumerator EnemyTurn()
    {
        int speechIndex = GetRandomEnemySpeechIndex();

        if (speechIndex == 0)
        {
            dialogueText.text = "Pensi davvero di avere un input valido contro di me?!";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "La tua speranza di vittoria è appena passata per una porta NOT:";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "si è completamente invertita! Ora ti faccio VedeRAID io!";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "💥 ATTACCO: IMPATTO RAID-ZERO! 💥";
            yield return new WaitForSeconds(3f);
        }
        else if (speechIndex == 1)
        {
            dialogueText.text = "Ah! Un attacco a sorpresa? Che mossa instabile.";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "Il tuo coraggio fa FLIP, ma ti assicuro che la tua barra della vita farà FLOP!";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "Preparati al reset asincrono!";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "⚡ ATTACCO: RAGGIO FLIP-FLOP J-K! ⚡";
            yield return new WaitForSeconds(3f);
        }
         else
        {
            dialogueText.text = "Credi di essere un eroe ";
            yield return new WaitForSeconds(2f);

            dialogueText.text = "solo perché hai collegato due cavetti colorati su Logisim?!";
            yield return new WaitForSeconds(4f);

            dialogueText.text = "Qui siamo nel silicio vero, ragazzino!";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "Ora ti scompongo in fattori primi...";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "Un BIT alla volta!";
            yield return new WaitForSeconds(3f);

            dialogueText.text = "🔥 ATTACCO: OVERFLOW ARITMETICO LETALE! 🔥";
            yield return new WaitForSeconds(3f);
        }

        int damage = enemyUnit.damage;
        bool isDead = playerUnit.TakeDamage(damage);
        playerHUD.SetHP(playerUnit.currentHP);

        // --- NUOVA RIGA: FAI VIBRARE IL GIOCATORE ---
        playerUnit.GetComponent<EffettoVibrazione>()?.IniziaVibrazione();

        dialogueText.text = enemyUnit.unitName + " ti ha tolto " + damage + " HP di vita!";
        yield return new WaitForSeconds(3f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    int GetRandomEnemySpeechIndex()
    {
        int newIndex;

        if (lastEnemySpeechIndex == -1)
        {
            newIndex = Random.Range(0, 3);
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, 3);
            } while (newIndex == lastEnemySpeechIndex);
        }

        lastEnemySpeechIndex = newIndex;
        return newIndex;
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
            dialogueText.text = "Congratulazioni! Hai vinto e sconfitto " + enemyUnit.unitName;
        else if (state == BattleState.LOST)
            StartCoroutine(ShowLostMessageSequence());
    }

    IEnumerator ShowLostMessageSequence()
    {
        dialogueText.text = "Avvio Garbage Collector...";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "Spazio liberato.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "Addio, processo inutile.";
    }

    void PlayerTurn()
    {
        dialogueText.text = "Seleziona una mossa :";
    }

    
public void OnAttackButton()
{
    if (state != BattleState.PLAYERTURN || isProcessing) return;
    isProcessing = true;
    StartCoroutine(PlayerAttack(playerUnit.damage));
}

    public void PlayerSuperAttack()
{
    if (state != BattleState.PLAYERTURN || isProcessing) return;
    isProcessing = true;
    StartCoroutine(PlayerAttack(playerUnit.damage * 2));
}
}