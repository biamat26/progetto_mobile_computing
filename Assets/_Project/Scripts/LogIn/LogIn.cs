using UnityEngine;
using System.Collections.Generic;

public class LogIn : MonoBehaviour 
{
    public UserList userList;

    public void Authenticate(string username, string password)
    {
        if (userList == null || userList.users == null)
        {
            Debug.LogError("Errore: UserList non assegnata nel componente Login!");
            return;
        }

        foreach (User u in userList.users)
        {
            if (u.username == username)
            {
                if (u.password == password)
                {
                    Debug.Log("Login effettuato con successo! Benvenuto " + username);
                    return;
                }
                else
                {
                    Debug.LogWarning("Password errata per l'utente: " + username);
                    return;
                }
            }
        }
        Debug.LogError("Utente non trovato!");
    }
}