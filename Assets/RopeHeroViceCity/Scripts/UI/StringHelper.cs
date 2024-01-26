using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringHelper
{
    #region util
    public const string PLUS_NUMBER = "+{0}";
    public const string HOUR_FORMAT = "{0}h";
    public const string ADS_NOT_AVAILABLE = "No Video at the moment!";
    public const string ADS_NOT_AVAILABLE_HEADER = "";
    public const string NOT_ENOUGHT_LEVEL = "Not Enought Level For Upgrade ";
    public const string NOT_ENOUGHT_SKILL_POINT = "Not Enought Skill Point For Upgrade";
    //Hien chi co cac goi bán giá trị bằng giờ hoặc phút
    public static string ConverMinuteToTimeShowAtUI(int minuteValue)
    {
        if (minuteValue >= 60)
        {
            if (minuteValue % 60 == 0)
            {
                return string.Format(HOUR_FORMAT, minuteValue / 60);
            }
            return string.Format(HOUR_N_MINUTE_FORMAT, minuteValue / 60, minuteValue % 60);
        }
        return string.Format(MINUTE_FORMAT, minuteValue);
    }
    //dùng để đổi số phút sang ngày-giờ,giờ-phút,phút
    public static string ConverMinuteToTimeShowAtCounter(int minuteValue, int second = 0)
    {
        if (minuteValue > 1440)
        {
            return string.Format(DAY_N_HOUR_FORMAT, minuteValue / 1440, (minuteValue % 1440) / 60);
        }

        if (minuteValue < 60) return string.Format(MINUTE_N_SECOND_FORMAT, minuteValue, second);
        return minuteValue % 60 == 0 ? string.Format(HOUR_FORMAT, minuteValue / 60) : string.Format(HOUR_N_MINUTE_FORMAT, minuteValue / 60, minuteValue % 60);
    }
    public const string DAY_N_HOUR_FORMAT = "{0}d {1}h";
    public const string HOUR_N_MINUTE_FORMAT = "{0:00}h {1:00}m";
    public const string MINUTE_N_SECOND_FORMAT = "{0:00}m {1:00}s";
    public const string MINUTE_FORMAT = "{0}m";
    public const string SECONDS_FORMAT = "{0}s";
    public const string DAY = "Day {0}";
    public const string INT_ROUND_TO_THOUSAND = "{0}k";
    public const string INT_ROUND_TO_MILLION = "{0}m";
    public const string INT_ROUND_TO_BILLION = "{0}b";
    public const string IN_APP_ORIGINAL_PRICE_SMALL_VALUE = "{0:n} {1}";
    public const string IN_APP_ORIGINAL_PRICE_BIG_VALUE = "{0:n0} {1}";
    public const string STRING_THOUSAND = "k";
    public const string STRING_MILLION = "m";
    public const string STRING_BILLION = "b";
    public const string PERCENT_NUMBER = "{0}/{1}";
    public const string WATCH = "Watch";
    public const string LEVEL_FORMAT = "Lvl {0}";
    public const string KEY_SHOP_BANK_TIME_COUNTER = "KEY_SHOP_BANK_TIME_COUNTER_{0}";
    public const string CONTENT_GET__CASH = "You have {0} cashes";
    public const string CONTENT_GET__GEM = "You have {0} gems";
    public const string CONTENT_GET_REWARD = "Claim";
    public const string FORMAT_ID_LEVEL = "H{0}M{1}L{2}";
    public const string FREE = "Free";
    public const string LOCKED = "Locked";
    public const string PERCENT_STRING = "{0}%";
    public const string PERCENT_STRING_SALE_OFF = "{0}% OFF";
    public const string PERCENT_STRING_SALE_OFF_OLD = "-{0}%";
    public const string MULTIPLE_STRING = "x{0}";
    public const string TITTLE_BUY_CASH = "Buy Coin";
    public const string CONTENT_DO_YOU_WANT_BUY = "Purchase Coin Pack?";
    public const string THANK_FOR_PURCHASE = "Thanks for purchasing!";
    public const string TITTLE_OOPS = "Oops!";
    public const string TITTLE_WARNING = "Warning";
    public const string CONTENT_OPEN_CRATE = "Open it now?";
    public const string DATA_LOST_WARNING = "If you delete the game, all of your data will be lost and unable to restore.";
    public const string CONTENT_COMING_SOON = "Congratulations on completing Hotel Tycoon!\nPlease stay tuned for more new updated Hotels in the future. Thank you!";
    public const string CONTENT_NOT_ENOUGH_DIAMOND = "Not enough diamond\nplease buy more";
    public const string CONTENT_NOT_ENOUGH_COIN = "Not enough coin\nplease buy more";
    public const string CONTENT_THERE_ARE_NO_VIDEO_TO_WATCH = "There are no video to watch now";
    public const string CONTENT_YOU_ONLY_CAN_WATCH_TIME_TODAY = "You can only watch {0} times today";
    public const string ARE_YOU_SURE_WANT_QUIT = "Are you sure to quit game?";
    public const string ARE_YOU_SURE = "Are you sure?";
    public const string CONTENT_YES = "YES";
    public const string CONTENT_OK = "OK";
    public const string CONTENT_NO = "NO";
    public const string CONTENT_SURE = "SURE";
    public const string CONTENT_OPEN_FULL_SKIN = "Unlocked All";
    public const string RESTORE_DONE = "Restore purchase successful!";
    public const string YOUR_LIVE_IS_REFILLED = "Your lives is refilled!";
    public const string MY_HOTEL_LEVEL_TO_NEXT_HOTEL = "{0} level(s) to the next Hotel";
    public const string MY_HOTEL_YOU_ARE_CURRENT_AT_LEVEL = "You are currently at level {0}";
    public const string YOU_CAN_GET_GEM_IN = "You can get gems in {0}";
    #endregion
    #region RemoveAds
    public const string TITTLE_REMOVE_ADS = "Remove Ads";
    public const string CONTENT_REMOVE_ADS = "Dont have to watch ads just {0} ?";
    public const string CONTENT_REMOVE_ADS_DONE = "You dont have to watch ads now";
    #endregion
    #region VideoReward
    public const string TITTLE_CANT_LOAD_VIDEO = "Can't load video";
    public const string CONTENT_CANT_LOAD_VIDEO = "Please check your internet connection";
    public const string TITTLE_SKIPPED = "You Skipped";
    public const string CONTENT_SKIPPED = "Please watch video again";
    public const string NO_INTERNET_TITTLE = "No Internet";
    public const string NO_INTERNET_CONTENT = "The shop is not available now!";
    public const string TITLE_CONGRATULATION = "Congratulation!";
    public const string OUT_OF_AMOUNT_WATCH_VIDEO_LOBBY = "You can watch more ads on the next day";

    #endregion
    #region SceneName
    public const string SCENE_NAME_LOBBY = "Lobby";
    public const string SCENE_NAME_GAMEPLAY = "GamePlay";
    #endregion
    #region Inapp

    public const string THIS_ITEM_NOT_AVAILABLE = "This item is not avaialbe now, please try another one";
    public const string PURCHASE_FAIL = "Purchase fail";
    public static string[] listInappIdProduct = new string[26] {
        "com.hoteltycoon.20gems",
        "com.hoteltycoon.110gems",
        "com.hoteltycoon.240gems",
        "com.hoteltycoon.500gems",
        "com.hoteltycoon.1100gems",
        "com.hoteltycoon.2400gems",
        "com.hoteltycoon.bellhop",
        "com.hoteltycoon.porter",
        "com.hoteltycoon.concierge",
        "com.hoteltycoon.manager",
        "com.hoteltycoon.grandhotelier",
        "com.hoteltycoon.newcommerset",
        "com.hoteltycoon.smallwinner1",
        "com.hoteltycoon.smallwinner2",
        "com.hoteltycoon.smallwinner3",
        "com.hoteltycoon.smallwinner4",
        "com.hoteltycoon.greatedeal1",
        "com.hoteltycoon.greatedeal2",
        "com.hoteltycoon.greatedeal3",
        "com.hoteltycoon.helpinghand",
        "com.hoteltycoon.102",
        "com.hoteltycoon.38",
        "com.hoteltycoon.160",
        "com.hoteltycoon.65",
        "com.hoteltycoon.128",
        "com.hoteltycoon.72",
    };
    #endregion
    #region LocalizeKey
    public const string tittlePause = "pause_popup_title";
    public const string loading = "loading..";
    #endregion

    #region Sound
    public const string SFX = "quite_mod";
    public const string MUSIC_MUTE = "music_mute";
    #endregion
    #region Player ID
    public const string PLAYER_ID_DEFAULT = "Player1";
    #endregion

    #region FirebaseRemoteConfig

    public const string upgrade_ad_count = "MAX_UPGRADE_AD_COUNT";
    public const string upgrade_ad_duration = "MIN_DURATION_UPGRADE_AD";
    public const string commercialBreakAdCount = "MAX_COMMERCIALBREAK_AD_COUNT";
    public const string commercialBreakGemValue = "MAX_COMMERCIALBREAK_GEM_VALUE";
    public const string maxPopupLifeAdCount = "MAX_POPUPLIFE_AD_COUNT";
    public const string maxPopupWinAdCount = "MAX_POPUPWIN_AD_COUNT";
    public const string second_chance_ad = "SECOND_CHANCE_AD";
    public const string second_chance_watch_ads_count = "MAX_SECONDCHANCE_AD_COUNT";
    public const string watch_video_daily = "ENABLE_DAILYLOGIN_ADS";
    public const string second_chance_enable_level = "SECOND_CHACNE_LEVEL";
    public const string second_chance_description = "SECOND_CHANCE_DESCRIPTION";
    public const string watchad_button_available_level = "WATCHAD_LEVEL_AVAILABLE";
    public const string remote_config_is_enable_tut = "IS_ENABLE_TUT";
    public const string skip_tutorial = "SKIP_TUTORIAL";
    #endregion


    public const string ONOFF_SOUND = "ONOFF_SOUND";
    public const string ONOFF_MUSIC = "ONOFF_MUSIC";
    public const string ONOFF_VIBRATION = "ONOFF_VIBRATION";
    public const string FIRST_TIME_INSTALL = "FIRST_TIME_INSTALL";

    public const string VERSION_FIRST_INSTALL = "VERSION_FIRST_INSTALL";
    public const string REMOVE_ADS = "REMOVE_ADS";
    public const string CURRENT_LEVEL = "CURRENT_LEVEL";
    public const string CURRENT_LEVEL_PLAY = "CURRENT_LEVEL_PLAY";
    public const string PATH_CONFIG_LEVEL = "Levels/Level_{0}";
    public const string PATH_CONFIG_LEVEL_TEST = "LevelsTest/Level_{0}";

    public const string SALE_IAP = "_sale";

    public const string LOGGED_REVENUE_CENTS = "logged_revenue_cents";

    public const string PREVIOUS_REVENUE_D0_D1 = "previous_revenue_d0";
    public const string REVENUE_D0_D1 = "revenue_d0";
    public const string NUMBER_OF_DISPLAYED_INTERSTITIAL_D0_D1 = "number_of_displayed_interstitial_d0";

    public const string PREVIOUS_REVENUE_D1 = "previous_revenue_d1";
    public const string REVENUE_D1 = "revenue_d1";
    public const string NUMBER_OF_DISPLAYED_INTERSTITIAL_D1 = "number_of_displayed_interstitial_d1";

    public const string RETENTION_D = "retent_type";
    public const string DAYS_PLAYED = "days_played";
    public const string PAYING_TYPE = "retent_type";
    public const string LEVEL = "level";

    public const string LAST_TIME_OPEN_GAME = "LAST_TIME_OPEN_GAME";
    public const string FIRST_TIME_OPEN_GAME = "FIRST_TIME_OPEN_GAME";

    public const string CAN_SHOW_RATE = "CAN_SHOW_RATE";

    public const string IS_TUTED_RETURN = "IS_TUTED_RETURN";
    public const string CURRENT_NUM_RETURN = "CURRENT_NUM_RETURN";
    public const string CURRENT_NUM_ADD_STAND = "CURRENT_NUM_ADD_STAND";
    public const string CURRENT_NUM_REMOVE_BOMB = "CURRENT_NUM_REMOVE_BOMB";
    public const string CURRENT_NUM_REMOVE_CAGE = "CURRENT_NUM_REMOVE_CAGE";
    public const string CURRENT_NUM_REMOVE_EGG = "CURRENT_NUM_REMOVE_EGG";
    public const string CURRENT_NUM_REMOVE_SLEEP = "CURRENT_NUM_REMOVE_SLEEP";
    public const string CURRENT_NUM_REMOVE_JAIL = "CURRENT_NUM_REMOVE_JAIL";


    public const string IS_TUTED_BUY_STAND = "IS_TUTED_BUY_STAND";
    public const string ACCUMULATION_REWARD = "ACCUMULATION_REWARD";
    public const string CURRENT_BIRD_SKIN = "CURRENT_BIRD_SKIN";
    public const string CURRENT_BRANCH_SKIN = "CURRENT_BRANCH_SKIN";
    public const string CURRENT_THEME = "CURRENT_THEME";
    public const string OWNED_BIRD_SKIN = "OWNED_BIRD_SKIN";
    public const string OWNED_BRANCH_SKIN = "OWNED_BRANCH_SKIN";
    public const string OWNED_THEME = "OWNED_THEME";
    public const string RANDOM_BIRD_SKIN_IN_SHOP = "RANDOM_BIRD_SKIN_IN_SHOP";
    public const string RANDOM_BRANCH_IN_SHOP = "RANDOM_BRANCH_IN_SHOP";
    public const string RANDOM_THEME_IN_SHOP = "RANDOM_THEME_IN_SHOP";

    public const string RANDOM_BIRD_SKIN_SALE_WEEKEND_1 = "RANDOM_BIRD_SKIN_SALE_WEEKEND_1";
    public const string RANDOM_BRANCH_SALE_WEEKEND_2 = "RANDOM_BRANCH_SALE_WEEKEND_2";
    public const string RANDOM_THEME_SALE_WEEKEND_2 = "RANDOM_THEME_SALE_WEEKEND_2";

    public const string CURRENT_RANDOM_BIRD_SKIN = "CURRENT_RANDOM_BIRD_SKIN";
    public const string CURRENT_RANDOM_BRANCH_SKIN = "CURRENT_RANDOM_BRANCH_SKIN";
    public const string CURRENT_RANDOM_THEME = "CURRENT_RANDOM_THEME";


    public const string NUM_SHOWED_ACCUMULATION_REWARD_RANDOM = "NUM_SHOWED_ACCUMULATION_REWARD_RANDOM";

    public const string NUMBER_OF_ADS_IN_DAY = "NUMBER_OF_ADS_IN_DAY";
    public const string NUMBER_OF_ADS_IN_PLAY = "NUMBER_OF_ADS_IN_PLAY";

    public const string IS_PACK_PURCHASED_ = "IS_PACK_PURCHASED_";
    public const string NUM_OF_PURCHASED_ = "NUM_OF_PURCHASED_";

    public const string IS_PACK_ACTIVATED_ = "IS_PACK_ACTIVATED_";
    public const string LAST_TIME_PACK_ACTIVE_ = "LAST_TIME_PACK_ACTIVE_";
    public const string LAST_TIME_PACK_SHOW_HOME_ = "LAST_TIME_PACK_SHOW_HOME_";
    public const string STARTER_PACK_IS_COMPLETED = "STARTER_PACK_IS_COMPLETED";

    public const string HAS_PACK_IN_WEEK_TODAY = "HAS_PACK_IN_WEEK_TODAY";
    public const string HAS_PACK_WEEKEND_TODAY = "HAS_PACK_WEEKEND_TODAY";

    public const string CURRENT_PACK_IN_WEEK = "CURRENT_PACK_IN_WEEK";
    public const string CURRENT_PACK_WEEKEND = "CURRENT_PACK_WEEKEND";
    public const string CURRENT_RANDOM_SET_SKIN = "CURRENT_RANDOM_SET_SKIN";


    public const string LAST_TIME_RESET_SALE_PACK_SHOP = "LAST_TIME_RESET_SALE_PACK_SHOP";

    public const string LAST_TIME_ONLINE = "LAST_TIME_ONLINE";
    public const string CURRENT_ID_MINI_GAME = "current_id_mini_game";

    public const string VERSION_MINI_GAME_MINIGAME = "version_mini_game_minigame";
    public const string POINT_MINIGAME_CONNECT_BIRD = "point_minigame_connect_bird";
    public const string LIST_REWARD_MINI_GAME_CONNECT_BIRD = "list_reward_mini_game_connect_bird";
    public const string WAS_COMMPLETE_MINI_GAME_CONNECT_BIRD = "was_commplete_mini_game_connect_bird";
    public const string CONNECT_BIRD_TURN_FREE = "connect_bird_turn_free";
    public const string CONNECT_BIRD_TURN_BUY = "connect_bird_turn_buy";
    public const string LIMIT_NUMB_WATCH_VIDEO_IN_DAY = "limit_numb_watch_video_in_day";
    public const string FIRST_OPEN_CONNECT_BIRD_MINI_GAME = "first_open_connect_bird_mini_game";
    public const string FIRST_SHOW_HAND = "first_show_hand";
    public const string CURRENT_NUM_WATCH_VIDEO_IN_DAY_MINI_GAME_CONNECT_BIRD = "current_num_watch_video_in_day_mini_game_connect_bird";
    public const string DATA_TO_RESET_MINI_GAME_CONNECT_BIRD = "data_to_reset_mini_game_connect_bird";
    public const string FIRST_WEEK = "first_week";
}

public class KeyPref
{
    public const string SERVER_INDEX = "SERVER_INDEX";
}

public class FirebaseConfig
{

    public const string DELAY_SHOW_INITSTIALL = "delay_show_initi_ads";//Thời gian giữa 2 lần show inital 30
    public const string LEVEL_START_SHOW_INITSTIALL = "level_start_show_initstiall";//Level bắt đầu show initial//3


    public const string LEVEL_START_SHOW_RATE = "level_start_show_rate";//Level bắt đầu show popuprate

    public const string DEFAULT_NUM_ADD_BRANCH = "default_num_add_branch";//2
    public const string DEFAULT_NUM_REMOVE_BOMB = "default_num_remove_bomb";//0
    public const string DEFAULT_NUM_REMOVE_EGG = "default_num_remove_egg";//0
    public const string DEFAULT_NUM_REMOVE_JAIL = "default_num_remove_jail";//0
    public const string DEFAULT_NUM_REMOVE_SLEEP = "default_num_remove_sleep";//0
    public const string DEFAULT_NUM_REMOVE_CAGE = "default_num_remove_cage";//0

    public const string DEFAULT_NUM_RETURN = "default_num_return";//2
    public const string NUM_RETURN_CLAIM_VIDEO_REWARD = "num_return_claim_video_reward";//3

    public const string LEVEL_START_TUT_RETURN = "level_start_tut_return";//4
    public const string LEVEL_START_TUT_BUY_STAND = "level_start_tut_buy_stand";//5

    public const string ON_OFF_REMOVE_ADS = "on_off_remove_ads_2";//5
    public const string MAX_LEVEL_SHOW_RATE = "max_level_show_rate";//30

    public const string TEST_LEVEL_CAGE_BOOM = "test_level_cage_boom_ver2";//30

    public const string ON_OFF_ACCUMULATION_REWARD_LEVEL_START = "on_off_accumulation_reward_level_start";//true
    public const string ACCUMULATION_REWARD_LEVEL_START = "accumulation_reward_level_start";//6
    public const string ACCUMULATION_REWARD_END_LEVEL = "accumulation_reward_end_level_{0}";//
    public const string ACCUMULATION_REWARD_TIME_SHOW_NEXT_BUTTON = "accumulation_reward_time_show_next_button";//1.5
    public const string ACCUMULATION_REWARD_END_LEVEL_RANDOM = "accumulation_reward_end_level_random";//10
    public const string MAX_TURN_ACCUMULATION_REWARD_END_LEVEL_RANDOM = "max_turn_accumulation_reward_end_level_random";//150

    public const string ON_OFF_SALE_INAPP = "on_off_sale_inapp";//true

    public const string LEVEL_UNLOCK_SALE_PACK = "level_unlock_sale_pack"; //11
    public const string LEVEL_UNLOCK_PREMIUM_PACK = "level_unlock_premium_pack"; //25
    public const string TIME_LIFE_STARTER_PACK = "time_life_starter_pack"; // 3DAY
    public const string TIME_LIFE_PREMIUM_PACK = "time_life_premium_pack"; // 2DAY
    public const string TIME_LIFE_SALE_PACK = "time_life_premium_pack"; // 1DAY
    public const string TIME_LIFE_BIG_REMOVE_ADS_PACK = "time_life_big_remove_ads_pack"; // 3h

    public const string NUMBER_OF_ADS_IN_DAY_TO_SHOW_PACK = "number_of_ads_in_day_to_show_pack"; //5ADS
    public const string NUMBER_OF_ADS_IN_PLAY_TO_SHOW_PACK = "number_of_ads_in_play_to_show_pack"; //3ADS
    public const string TIME_DELAY_SHOW_POPUP_SALE_PACK_ = "time_delay_show_popup_sale_pack_"; // 6H
    public const string TIME_DELAY_ACTIVE_SALE_PACK = "time_delay_active_sale_pack_"; // 6H

    public const string CONFIG_SALE_PACK_HALLOWEEN = "config_sale_pack_halloween";
    public const string CONFIG_SALE_PACK_BLACK_FRIDAY = "config_sale_pack_black_friday";

    public const string CONFIG_EVENT_GAME = "config_event_game";

    public const string GAME_THEME_CONFIG = "game_theme_config";
}
