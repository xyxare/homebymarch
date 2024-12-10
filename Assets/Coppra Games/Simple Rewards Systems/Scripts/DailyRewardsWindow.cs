using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Repforge.StepCounterPro;

namespace CoppraGames
{
    public class DailyRewardsWindow : MonoBehaviour
    {
        [System.Serializable]
        public class RewardData
        {
            public Sprite icon;
            public int count;
            public int day;
            public Item itemSO; // add item here
        }

        public GameObject ResultPanel;
        public Image ResultIcon;
        public TextMeshProUGUI ResultCount;
        public TextMeshProUGUI progressText;

        public Button ClaimButton;
        public InventoryObject playerInventory;

        public RewardData[] rewards;
        public DailyRewardItem[] rewardItemComponents;
        public DailyQuestProgress dailyQuestProgress;
        public string dailyQuestJsonFilePath;
        public int currentDailySteps;
        public int requiredDailySteps;
        public PlayerData playerData;


        void Awake()
        {
            HideResult();
            //Init();


        }

        public void Init()
        { 
            dailyQuestJsonFilePath = Application.persistentDataPath + "/playerDailyQuestData.json";





            if (System.IO.File.Exists(dailyQuestJsonFilePath)){
                LoadDailyQuestData();
            } else {
                dailyQuestProgress = new DailyQuestProgress(rewards.Length);
            }
            if(!IsYesterdayRewardCollected() | GetDaysSinceLastReset() == 7){
                ResetDailyRewards();
            }

            LoadDailyStepCount();

            Debug.Log("file path: " + dailyQuestJsonFilePath);
            Debug.Log("quests claimed array: " + dailyQuestProgress.areDailyQuestsClaimed);
            Debug.Log("last reset date: " + dailyQuestProgress.lastResetDate);
            Debug.Log("days since last: " + GetDaysSinceLastReset());
            Debug.Log("current steps: " + currentDailySteps);
            Debug.Log("current > required: " + (currentDailySteps > requiredDailySteps));
            Debug.Log("required steps: " + (requiredDailySteps));
            ApplyValues();

        }

        public void Close()
        {
            Main.instance.ShowDailyRewardsWindow(false);
            SaveDailyQuestData();
        }

        public void ApplyValues()
        {
            int index = 0;
            foreach (var r in rewards)
            {
                if (rewardItemComponents.Length > index)
                {
                    rewardItemComponents[index].SetData(r);
                }

                index++;
            }

            RefreshClaimButton();
        }

        public void ResetDailyRewards(){        

            DateTime resetTime;

            resetTime = DateTime.Now;
            dailyQuestProgress.lastResetDate = resetTime.ToString();
            // PlayerPrefs.SetString("last_reset_time", resetTime.ToString());

            for (int i = 0; i < rewards.Length; i++){
                string key = "reward_claimed_" + (i + 1);
                // PlayerPrefs.SetInt(key, 0);
                dailyQuestProgress.areDailyQuestsClaimed[i] = false;
            }

        }

        public void LoadDailyStepCount(){
            StepCounterRequest request = new StepCounterRequest();
            request.Since(DateTime.Today).OnQuerySuccess((stepCount) => currentDailySteps = stepCount).Execute();
            UpdateDailyQuestProgress();
        }

        public void UpdateDailyQuestProgress(){
            progressText.text = $"{currentDailySteps} / {requiredDailySteps} steps";
        }

        public int GetDaysSinceLastReset()
        {
            DateTime current = DateTime.Now;
            DateTime resetTime;

            string resetTimeString = dailyQuestProgress.lastResetDate;
            if (string.IsNullOrEmpty(resetTimeString))
            {
                resetTime = DateTime.Now;
                dailyQuestProgress.lastResetDate = resetTime.ToString();
                // PlayerPrefs.SetString("last_reset_time", resetTime.ToString());
            }
            else
            {
                if (!DateTime.TryParse(resetTimeString, out resetTime))
                {
                    resetTime = DateTime.Now;
                }
            }

            TimeSpan timeSpan = current - resetTime;
            return timeSpan.Days;
        }

        public bool IsYesterdayRewardCollected(){
            int lastReset = GetDaysSinceLastReset();
            // string key = "reward_claimed_" + (lastReset); // key for yesterday's claim
            Debug.Log("days since last reset:" + lastReset);

            if (lastReset == 0){
                return true;
            } else {
            return(dailyQuestProgress.areDailyQuestsClaimed[lastReset - 1]);
            }

            // return(PlayerPrefs.GetInt(key, 0) == 1 | lastReset == 0);


        }


        public bool IsDailyRewardReadyToCollect(int day)
        {
            int loginDay = GetDaysSinceLastReset();
            return (loginDay >= day - 1 && currentDailySteps > requiredDailySteps);
        }

        public bool IsDailyRewardClaimed(int day)
        {
            // string key = "reward_claimed_" + day;
            // return (PlayerPrefs.GetInt(key, 0) == 1);
            return dailyQuestProgress.areDailyQuestsClaimed[day - 1];
        }

        public void ClaimDailyReward(int day)
        {
            RewardData reward = rewards[day - 1];
            // string key = "reward_claimed_" + day;
            dailyQuestProgress.areDailyQuestsClaimed[day - 1] = true;
            Debug.Log("claimed. new array:" + dailyQuestProgress.areDailyQuestsClaimed[day - 1]);

            if(reward.itemSO.Name != ""){
                playerInventory.AddItem(reward.itemSO, reward.count);
            } else {
                playerData.AddGold(reward.count);
            }
            SaveDailyQuestData();
            // PlayerPrefs.SetInt(key, 1);
            

            // QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.COLLECT_DAILY_REWARDS);
        }

        public void ShowResult(int resultIndex)
        {
            StartCoroutine(_ShowResult(resultIndex));
            // SoundController.instance.PlaySoundEffect("collection", false, 1);
        }


        private IEnumerator _ShowResult(int resultIndex)
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(true);

                if (rewards.Length > resultIndex)
                {

                    RewardData reward = rewards[resultIndex];
                    ResultIcon.sprite = reward.icon;
                    ResultCount.text = "x" + reward.count.ToString();
                    // playerInventory.AddItem(reward.itemSO, reward.count); inventory code goes here

                }

                ResultPanel.GetComponent<Animator>().Play("clip");
            }
            yield return new WaitForSeconds(3.3f);
            HideResult();
        }

        public void HideResult()
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(false);
            }
        }

        public void OnClickClaimButton()
        {
            if (DailyRewardItem.selectedItem != null)
            {
                int day = DailyRewardItem.selectedItem.GetDay();
                if (!IsDailyRewardClaimed(day))
                {
                    ClaimDailyReward(day);
                    ShowResult(DailyRewardItem.selectedItem.GetDay() - 1);

                    Init();
                }
            }
        }

        public void RefreshClaimButton()
        {
            if (DailyRewardItem.selectedItem != null)
            {
                int day = DailyRewardItem.selectedItem.GetDay();
                bool isClaimed = IsDailyRewardClaimed(day);
                bool isReadyToCollect = IsDailyRewardReadyToCollect(day);

                

                this.ClaimButton.interactable = !isClaimed && isReadyToCollect;
            }
            else
                this.ClaimButton.interactable = false;
        }

        public void OnClickCloseButton()
        {
            this.Close();
        }

        public void SaveDailyQuestData(){

            string dailyQuestJson = JsonUtility.ToJson(dailyQuestProgress);
            
            System.IO.File.WriteAllText(dailyQuestJsonFilePath, dailyQuestJson);
            Debug.Log("daily quest data saved");

        }

        public void LoadDailyQuestData(){

        string dailyQuestJson = System.IO.File.ReadAllText(dailyQuestJsonFilePath);
        dailyQuestProgress = new DailyQuestProgress(rewards.Length);

        Debug.Log(dailyQuestJson);

        if(dailyQuestJson != ""){

        Debug.Log("json utilty stuff: " + JsonUtility.FromJson<DailyQuestProgress>(dailyQuestJson).areDailyQuestsClaimed[0]);

        dailyQuestProgress.areDailyQuestsClaimed = JsonUtility.FromJson<DailyQuestProgress>(dailyQuestJson).areDailyQuestsClaimed;
        dailyQuestProgress.lastResetDate = JsonUtility.FromJson<DailyQuestProgress>(dailyQuestJson).lastResetDate;
        } else {
            dailyQuestProgress = new DailyQuestProgress(rewards.Length);
        }

        Debug.Log("daily quest data loaded");

            
        }
    }
}
