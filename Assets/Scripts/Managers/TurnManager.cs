using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState { Player, Enemy }
    public TurnState currentState;

    public void EndTurn()
    {
        if (currentState == TurnState.Player)
        {
            currentState = TurnState.Enemy;
            Debug.Log("Player turn ended, now enemy turn");
        }
    }

    public void ResetPlayerTurn()
    {
        currentState = TurnState.Player;
        Debug.Log("Now player turn");
    }
}
