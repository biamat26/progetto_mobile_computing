using UnityEngine;
using System.Collections.Generic;

public class SignUp : MonoBehaviour 
{
    public UserList userList; 

    public void Register(string username, string password)
    {
        if (userList == null || userList.users == null)
        {
            Debug.LogError("UserList non configurata nel componente SignUp!");
            return;
        }

        foreach (User u in userList.users)
        {
            if (u.username == username)
            {
                Debug.Log("Utente già esistente: " + username);
                return;
            }
        }

        User newUser = new User(username, password);
        userList.users.Add(newUser);
        
        Debug.Log("Utente registrato con successo: " + username);
        
        SaveUsers();
    }

    private void SaveUsers()
    {
        Debug.Log("Salvataggio dati in corso...");
    }
}