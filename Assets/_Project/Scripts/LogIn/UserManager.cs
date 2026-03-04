using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class UserManager : MonoBehaviour
{
    // 1. Qui ci sono le variabili
    public static UserManager Instance;
    private string filePath;
    public UserList userList = new UserList();
    public User currentUser;

    // 2. Qui ci sono le funzioni esistenti
    private void Awake()
    {
        Instance = this;
        filePath = Application.persistentDataPath + "/users.json";
        LoadUsers();
    }

    // --- METTI LA FUNZIONE QUI ---
    // (Dopo la chiusura di Awake, ma prima di SaveUsers)
    public void UpdatePlayerScore(string name, int newPoints)
    {
        User u = userList.users.Find(user => user.username == name);
        if (u != null)
        {
            if (newPoints > u.score)
            {
                u.score = newPoints;
                SaveUsers();
                Debug.Log($"Nuovo Record per {name}: {newPoints}!");
            }
        }
    }

    public void SaveUsers()
    {
        string json = JsonUtility.ToJson(userList, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadUsers()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            userList = JsonUtility.FromJson<UserList>(json);
        }
    }
} // <--- QUESTA parentesi chiude la classe. NON scrivere nulla dopo di lei.