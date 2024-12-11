using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;

public class CloudSaver : MonoBehaviour
{
    async void Start(){
        await UnityServices.InitializeAsync();
    }

    public async static Task SaveDataToCloud(string key, object saveData){
        
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            {
                key, saveData
            }};

        try {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("saved yipee");
        } catch {
            Debug.Log("no save.");
        }
    }

    public async static Task<string> LoadDataFromCloud(string key){

        //RETURNS DATA OF FROM THE CLOUD SAVE

        Dictionary<string, string> savedData = await 
            CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});

        string dataString = savedData[key];
        Debug.Log("loaded yipee");

        //BE SURE TO USE JsonUtility.FromJson<classhere>(variableName); to extract from data classes

        // T data = JsonUtility.FromJson<T>(dataString);
        return dataString;

    }
}