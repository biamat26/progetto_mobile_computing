using Unity.VisualScripting;
using UnityEngine;

public enum BattleState{START, PLAYERTURN , ENEMYTURN , WON, LOST}

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;
    public BattleState state;

    private 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = BattleState.START;
        SetUpBattle();
    }

    void SetUpBattle(){
        GameObject playerGO = Instantiate(playerPrefab,playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        
        GameObject enemyGO = Instantiate(enemyPrefab,enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
    }

}
