using UnityEngine;
using System.Collections.Generic;
using System.Linq; // <-- Obbligatorio per ordinare la lista!

public class LeaderboardManager : MonoBehaviour
{
    // Riferimento allo script che contiene la tua UserList (UserManager)
    public UserManager userManager;

    public List<User> GetTopScores(int limit = 10)
    {
        if (userManager == null || userManager.userList.users == null)
        {
            Debug.LogWarning("Lista utenti non trovata!");
            return new List<User>();
        }

        // 1. Prendiamo la lista originale
        // 2. OrderByDescending mette il punteggio più alto in cima
        // 3. Take(limit) prende solo i primi X (es. i primi 10)
        // 4. ToList() converte il risultato in una nuova lista
        return userManager.userList.users
            .OrderByDescending(u => u.score)
            .Take(limit)
            .ToList();
    }

    public void MostraClassificaInConsole()
    {
        List<User> topUsers = GetTopScores(5);
        
        Debug.Log("--- TOP 5 PLAYERS ---");
        for (int i = 0; i < topUsers.Count; i++)
        {
            Debug.Log($"{i + 1}. {topUsers[i].username}: {topUsers[i].score} punti");
        }
    }
}
