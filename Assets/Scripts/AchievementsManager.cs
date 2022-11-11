using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Achievements
{
    public class AchievementsManager : MonoBehaviour
    {

        public enum Achievement : int
        {
            DiscoverBrightmaw = 0,
            DefeatBrightmaw = 1,
            DiscoverBonestag = 2,
            DefeatBonestag = 3,
            DiscoverWailingTree = 4,
            DefeatWailingTree = 5,
            DiscoverOmen = 6,
            DefeatOmen = 7,
            DiscoverPrimaeval = 8,
            DefeatPrimaeval = 9,
            FullyUpgradeBank = 10,
            PurchaseAllWeapons = 11,
            DrinkGrumbo = 12,
            DefeatPossessedDummy = 13,
            HuntSquib = 14,
            JudgeJuryExecutioner = 15,
            DieInTutorial = 16,
            DieXFifty = 17,
            Die = 18,
            AllLoreBrightmoles = 19,
            AllLorePlagueBeetles = 20,
            AllLoreCorpsePuppets = 21,
            AllLoreAfflicted = 22,
            AllLoreCompleted = 23,
            CompleteEasyBounty = 24,
            CompleteMediumBounty = 25,
            CompleteHardBounty = 26,
            BuyAllWeapons = 27,
            W_Executioner = 28,
            W_DavyJones = 29,
            W_Obituary = 30,
            W_Offering = 31,
            W_Rattlesnake = 32,
            W_WitchHunter = 33,
            W_GraveRobbing = 34,
            W_CuttingWords = 35,
            W_Buzzard = 36,
            W_Taboo = 37,
            W_CaveIn = 38,
            W_Dustman = 39,
            W_Judge = 40,
            W_Jury = 41,
            CollectBrightmaw = 42,
            CollectBonestag = 43,
            CollectWailingTree = 44,
            CollectOmen = 45,
            CollectPrimaeval = 46,
            None = 99,
        }

        private class Achievement_t
        {
            public Achievement id;
            public string APIName;
            public bool unlocked;

            public Achievement_t(Achievement id, string APIName)
            {
                this.id = id;
                this.APIName = APIName;
                this.unlocked = false;
            }
        }

        private Achievement_t[] m_Achievements = new Achievement_t[]
        {
            new Achievement_t(Achievement.DiscoverBrightmaw, "P_DISC_BRIGHTMAW"),
            new Achievement_t(Achievement.DefeatBrightmaw, "P_DEF_BRIGHTMAW"),
            new Achievement_t(Achievement.DiscoverBonestag, "P_DISC_BONESTAG"),
            new Achievement_t(Achievement.DefeatBonestag, "P_DEF_BONESTAG"),
            new Achievement_t(Achievement.DiscoverWailingTree, "P_DISC_WAILINGTREE"),
            new Achievement_t(Achievement.DefeatWailingTree, "P_DEF_WAILINGTREE"),
            new Achievement_t(Achievement.DiscoverOmen, "P_DISC_OMEN"),
            new Achievement_t(Achievement.DefeatOmen, "P_DEF_OMEN"),
            new Achievement_t(Achievement.DiscoverPrimaeval, "P_DISC_PRIMAEVAL"),
            new Achievement_t(Achievement.DefeatPrimaeval, "P_DEF_PRIMAEVAL"),
            new Achievement_t(Achievement.FullyUpgradeBank, "COMPLETE_BANK"),
            new Achievement_t(Achievement.PurchaseAllWeapons, "COMPLETE_WEAPONS"),
            new Achievement_t(Achievement.DrinkGrumbo, "J_DRINK_GRUMBO"),
            new Achievement_t(Achievement.DefeatPossessedDummy, "J_ROOTS"),
            new Achievement_t(Achievement.HuntSquib, "J_SQUIB"),
            new Achievement_t(Achievement.JudgeJuryExecutioner, "J_COMBO"),
            new Achievement_t(Achievement.DieInTutorial, "J_DIE_IN_TUTORIAL"),
            new Achievement_t(Achievement.DieXFifty, "J_DIE_X50"),
            new Achievement_t(Achievement.Die, "J_DIE"),
            new Achievement_t(Achievement.AllLoreBrightmoles, "C_BRIGHTMOLES"),
            new Achievement_t(Achievement.AllLorePlagueBeetles, "C_PLAGUEBEETLES"),
            new Achievement_t(Achievement.AllLoreCorpsePuppets, "C_CORPSEPUPPETS"),
            new Achievement_t(Achievement.AllLoreAfflicted, "C_AFFLICTED"),
            new Achievement_t(Achievement.AllLoreCompleted, "C_100PERCENTLORE"),
            new Achievement_t(Achievement.CompleteEasyBounty, "B_EASY"),
            new Achievement_t(Achievement.CompleteMediumBounty, "B_MEDIUM"),
            new Achievement_t(Achievement.CompleteHardBounty, "B_HARD"),
            new Achievement_t(Achievement.BuyAllWeapons, "W_ALL"),
            new Achievement_t(Achievement.W_Executioner, "W_EXECUTIONER"),
            new Achievement_t(Achievement.W_DavyJones, "W_DAVYJONES"),
            new Achievement_t(Achievement.W_Obituary, "W_OBITUARY"),
            new Achievement_t(Achievement.W_Offering, "W_OFFERING"),
            new Achievement_t(Achievement.W_Rattlesnake, "W_RATTLESNAKE"),
            new Achievement_t(Achievement.W_WitchHunter, "W_WITCHHUNTER"),
            new Achievement_t(Achievement.W_GraveRobbing, "W_GRAVEROBBER"),
            new Achievement_t(Achievement.W_CuttingWords, "W_CUTTINGWORDS"),
            new Achievement_t(Achievement.W_Buzzard, "W_BUZZARD"),
            new Achievement_t(Achievement.W_Taboo, "W_TABOO"),
            new Achievement_t(Achievement.W_CaveIn, "W_CAVEIN"),
            new Achievement_t(Achievement.W_Dustman, "W_DUSTMAN"),
            new Achievement_t(Achievement.W_Judge, "W_JUDGE"),
            new Achievement_t(Achievement.W_Jury, "W_JURY"),
            new Achievement_t(Achievement.CollectBrightmaw, "C_BRIGHTMAW"),
            new Achievement_t(Achievement.CollectBonestag, "C_BONESTAG"),
            new Achievement_t(Achievement.CollectWailingTree, "C_WAILINGTREE"),
            new Achievement_t(Achievement.CollectOmen, "C_OMEN"),
            new Achievement_t(Achievement.CollectPrimaeval, "C_PRIMAEVAL"),
        };





        CGameID m_GameID;

        int m_weaponsOwned;
        int m_legendaryWeaponsOwned;

        protected Callback<UserStatsReceived_t> m_UserStatsRecieved;
        protected Callback<UserStatsStored_t> m_UserStatsStored;
        protected Callback<UserAchievementStored_t> m_UserAchievementStored;

        public static AchievementsManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Init()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            m_GameID = new CGameID(SteamUtils.GetAppID());

            m_UserStatsRecieved = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
            m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
        }

        public void UnlockAchievement(Achievement achievement)
        {
            if (achievement == Achievement.None)
            {
                return;
            }
            if (!SteamManager.Initialized)
            {
                return;
            }
            if (m_Achievements[(int)achievement].unlocked)
            {
                Debug.Log("Achievement already unlocked");
                return;
            }
            m_Achievements[(int)achievement].unlocked = true;

            SteamUserStats.SetAchievement(m_Achievements[(int)achievement].APIName);
            SteamUserStats.StoreStats();
        }
        private void OnUserStatsReceived(UserStatsReceived_t pCallback)
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if ((ulong)m_GameID == pCallback.m_nGameID)
            {
                if (EResult.k_EResultOK == pCallback.m_eResult)
                {
                    Debug.Log("Received stats and achievements from Steam");
                    foreach (Achievement_t achievement in m_Achievements)
                    {
                        bool ret = SteamUserStats.GetAchievement(achievement.APIName, out achievement.unlocked);
                        if (ret)
                        {
                            Debug.Log("Achievement " + achievement.APIName + " is " + achievement.unlocked);
                        }
                        else
                        {
                            Debug.Log("Achievement " + achievement.APIName + " could not be found. Please check SteamAPI Implementation.");
                        }
                    }
                }
                else
                {
                    Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
                }
            }
        }

        private void OnUserStatsStored(UserStatsStored_t pCallback)
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if ((ulong)m_GameID == pCallback.m_nGameID)
            {
                if (EResult.k_EResultOK == pCallback.m_eResult)
                {
                    Debug.Log("Stored stats and achievements to Steam");
                }
                else
                {
                    Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
                }
            }
        }

        private void OnAchievementStored(UserAchievementStored_t pCallback)
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if ((ulong)m_GameID == pCallback.m_nGameID)
            {
                Debug.Log("Stored Achievement " + pCallback.m_rgchAchievementName);
            }
        }
    }
}


