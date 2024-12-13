

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;


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
    public PlayerDataSaver data;


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
    void Awake(){
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

        data = new PlayerDataSaver();
        data.playerName = playerName;
        data.health = health;
        data.attack = attack;
        data.defense = defense;
        data.gold = gold;
        data.attackSpeed = attackSpeed;

        
        string playerDataJson = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(playerDataJsonFilePath, playerDataJson);

        Debug.Log("Saved Player Data" + data.playerName);


    }

    public void LoadPlayerData(){

        if(System.IO.File.Exists(playerDataJsonFilePath)){
            string playerDataJson = System.IO.File.ReadAllText(playerDataJsonFilePath);
            data = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson);

            SetPlayerStats(data);

            Debug.Log("Player Data Loaded");


        }

    }

    public async void SavePlayerDataToCloud(){

        CloudSaver.SaveDataToCloud("playerData", data);
        

    }

    public async void LoadPlayerDataFromCloud(){
        string playerDataJson = await CloudSaver.LoadDataFromCloud("playerData");
        data = JsonUtility.FromJson<PlayerDataSaver>(playerDataJson);
        Debug.Log(data.playerName);
        

        SetPlayerStats(data);
        SavePlayerData();

    }

    public void SetPlayerStats(PlayerDataSaver playerData){
        playerName = playerData.playerName;
        health = playerData.health;
        attack = playerData.attack;
        defense = playerData.defense;
        gold = playerData.gold;
        attackSpeed = playerData.attackSpeed;

        Debug.Log("player stats set!");
    }

    public void ChangePlayerName(string name){
        playerName = name;
        SavePlayerData();
    }





}