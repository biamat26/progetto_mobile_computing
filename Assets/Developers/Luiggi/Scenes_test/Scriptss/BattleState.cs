using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public enum BattleState{START, PLAYERTURN , ENEMYTURN , WON, LOST}

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle(){
        GameObject playerGO = Instantiate(playerPrefab,playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        
        GameObject enemyGO = Instantiate(enemyPrefab,enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "Il cattivo " + enemyUnit.unitName + " si sta avvicinando...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();


    }

    IEnumerator PlayerAttack(int damageToDeal){
        

        bool isDead = enemyUnit.TakeDamage(damageToDeal);

        enemyHUD.SetHP(enemyUnit.currentHP);
        if(damageToDeal > playerUnit.damage){
            dialogueText.text = "COLPO CRITICOO! " + damageToDeal + " di DANNO!";
        }
        else{
            dialogueText.text = "Attacco formidabile!";
        }

        yield return new WaitForSeconds(2f);

        if(isDead){
            state = BattleState.WON;
            EndBattle();
        }
        else{
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn(){

        dialogueText.text = enemyUnit.unitName + " ti sta attaccando !";
        yield return new WaitForSeconds(2f);
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if(isDead){
            state = BattleState.LOST;
            EndBattle();
        }
        else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle(){
        if(state == BattleState.WON){
            dialogueText.text = "Congratulazioni! Hai vinto e sconfitto " + enemyUnit.unitName;
        }
        else if (state == BattleState.LOST){
            dialogueText.text = "Ti hanno spaccato! Hahahahah godo";
        }
    }


    void PlayerTurn(){
        dialogueText.text = "Seleziona una mossa :";

    }

    public void OnAttackButton(){
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(PlayerAttack(playerUnit.damage));
    }

    public void PlayerSuperAttack(){
        if(state!=BattleState.PLAYERTURN){
            return;
        }
        StartCoroutine(PlayerAttack(playerUnit.damage*2));
    }


}
