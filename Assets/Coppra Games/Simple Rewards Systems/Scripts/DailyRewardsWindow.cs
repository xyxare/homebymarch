using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            // public ItemSO itemSO; add item here
        }

        public GameObject ResultPanel;
        public Image ResultIcon;
        public TextMeshProUGUI ResultCount;

        public Button ClaimButton;
        // public InventoryObject playerInventory; add inventory here

        public RewardData[] rewards;
        public DailyRewardItem[] rewardItemComponents;

        void Awake()
        {
            HideResult();
            //Init();


        }

        public void Init()
        {
            if(!IsYesterdayRewardCollected() | GetDaysSinceLastReset() == 7){
                ResetDailyRewards();
            }
            ApplyValues();

        }

        public void Close()
        {
            Main.instance.ShowDailyRewardsWindow(false);
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
            PlayerPrefs.SetString("last_reset_time", resetTime.ToString());

            for (int i = 0; i < rewards.Length; i++){
                string key = "reward_claimed_" + (i + 1);
                PlayerPrefs.SetInt(key, 0);
                Debug.Log("skibidi reward reset" + i);
            }

        }

        public int GetDaysSinceLastReset()
        {
            DateTime current = DateTime.Now;
            DateTime resetTime;

            string resetTimeString = PlayerPrefs.GetString("last_reset_time");
            if (string.IsNullOrEmpty(resetTimeString))
            {
                resetTime = DateTime.Now;
                PlayerPrefs.SetString("last_reset_time", resetTime.ToString());
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
            string key = "reward_claimed_" + (lastReset); // key for yesterday's claim

            return(PlayerPrefs.GetInt(key, 0) == 1 | lastReset == 0);


        }


        public bool IsDailyRewardReadyToCollect(int day)
        {
            int loginDay = GetDaysSinceLastReset();
            return (loginDay >= day - 1);
        }

        public bool IsDailyRewardClaimed(int day)
        {
            string key = "reward_claimed_" + day;
            return (PlayerPrefs.GetInt(key, 0) == 1);
        }

        public void ClaimDailyReward(int day)
        {
            string key = "reward_claimed_" + day;
            PlayerPrefs.SetInt(key, 1);
            

            QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.COLLECT_DAILY_REWARDS);
        }

        public void ShowResult(int resultIndex)
        {
            StartCoroutine(_ShowResult(resultIndex));
            SoundController.instance.PlaySoundEffect("collection", false, 1);
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
    }
}
