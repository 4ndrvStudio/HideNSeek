using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using Unity.Services.RemoteConfig;
using Newtonsoft.Json;

namespace HS4.Config
{

    public static class SceneName
    {
        public static string Gameplay = "scene_gameplay";
        public static string Character = "scene_character";
    }

    public class ConfigManager : Singleton<ConfigManager>
    {
        public struct userAttributes
        {
            public bool expansionFlag;
        }
        public struct appAttributes
        {
            public string appVersion;
        }

        [Header("Backend")]
        public BundlePackConfig BundlePackConfig;

        [Header("Client")]
        public List<CharacterConfig> CharacterConfigs;

        public async void Init()
        {
            //load backend config
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            string jsonConfig = RemoteConfigService.Instance.appConfig.GetJson("bundlePackConfig");
            BundlePackConfig = JsonConvert.DeserializeObject<BundlePackConfig>(jsonConfig);

            //load client config
            CharacterConfigs = Resources.LoadAll<CharacterConfig>("Configs/CharacterConfig").ToList();
        }

        public GameObject GetCharacterPrefab(string id)
        {
            int index = CharacterConfigs.FindIndex(item => item.Id == id);
            if (index != -1)
                return CharacterConfigs[index].Prefab;
            else
                return null;
        }

        public int GetCharacterPrice(string id)
        {
            int index = BundlePackConfig.Characters.FindIndex(item => item.Id == id);
            if (index != -1)
                return BundlePackConfig.Characters[index].Price;
            else
                return 99999;
        }
    }

    [System.Serializable]
    public class BundlePackConfig
    {
        [JsonProperty("gold")]
        public List<GoldPack> GoldPacks;
        [JsonProperty("gem")]
        public List<GemPack> GemPacks;
        [JsonProperty("character")]
        public List<Character> Characters;
    }
    [System.Serializable]
    public class GoldPack
    {
        public string PackId;
        public int Amount;
        public int Price;
    }

    [System.Serializable]
    public class GemPack
    {
        public string PackId;
        public int Amount;
        public GemPrices Price;
    }

    [System.Serializable]
    public class GemPrices
    {
        public double EN;
        public double VI;
    }
    [System.Serializable]
    public class Character
    {
        public string Id;
        public int Price;
    }
}

