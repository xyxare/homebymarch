

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    public string playerName;
    public int health{get; private set;}
    public int attack{get; private set;}
    public int defense{get; private set;}
    public int gold{get; private set;}
    public double attackSpeed{get; private set;}

    private string playerDataJsonFilePath;


    public PlayerData()
    {
        // Default values for a new player
        playerName = "New Player";
        health = 100;
        attack = 10;
        defense = 5;
        gold = 0;
        attackSpeed = 2;
        
    }
    void Start(){
        playerDataJsonFilePath = Application.persistentDataPath + "/playerData.json";
        LoadPlayerData();
        
    }

    public void OnApplicationClose(){
        SavePlayerData();
    }

    // Method to level up the player
    public void LevelUp()
    {

        health += 10;
        attack += 5;
        defense += 3;
        attackSpeed = attackSpeed / 0.995;
        SavePlayerData();
        

    }

    public void AddGold(int amount){
        gold += amount;
        SavePlayerData();
    }

    public void SubtractGold(int amount){
        gold -= amount;
    }

    public void SavePlayerData(){

        PlayerDataSaver data = new PlayerDataSaver();
        data.health = health;
        data.attack = attack;
        data.defense = defense;
        data.gold = gold;
        data.attackSpeed = attackSpeed;

        
        string playerDataJson = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(playerDataJsonFilePath, playerDataJson);
        Debug.Log("Saved Player Data");

    }

    public void LoadPlayerData(){

        if(System.IO.File.Exists(playerDataJsonFilePath)){
            string playerDataJson = System.IO.File.ReadAllText(playerDataJsonFilePath);
            health = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson).health;
            attack = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson).attack;
            defense = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson).defense;
            gold = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson).gold;
            attackSpeed = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson).attackSpeed;

            Debug.Log("Player Data Loaded");


        }

    }


    



}
// }