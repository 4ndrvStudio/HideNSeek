using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Newtonsoft.Json;

namespace HS4
{
    #region Model
    public struct ApiName
    {
        //get
        public static string User_Get_Info = "user_get_info";
        public static string User_Get_Balance = "user_get_currency";
        public static string User_Get_Characters = "user_get_characters";
        public static string User_Get_Character_In_Use = "user_get_character_in_use";

        //set
        public static string User_Setup = "user_setup";
        public static string User_Set_Name = "user_set_name";
        public static string User_Select_Character = "user_select_character";

        //buy
        public static string Buy_Gold = "buy_gold";
        public static string Buy_Gem = "add_gem";
        public static string Buy_Character = "buy_character";

    }

    public class UserInfo
    {
        public string? UserName;
        public int Level;
        public int Exp;
    }

    public class Balance
    {
        public int Gold;
        public int Gem;
        public int Energy;
    }
    
    public class Prices {
        public int Gold;
        public int Gem;
    }

    namespace Backend
    {
        public class Character
        {
            [JsonProperty("playersInventoryItemId")]
            public string Id;
            [JsonProperty("inventoryItemId")]
            public string InventoryItemId;
        }
    }

    #endregion

    public static class User
    {
        public static UserInfo Info;
        public static Balance Balance;

        public static async Task<bool> SignInAnonymouslyAsync()
        {
            InitializationOptions options = new InitializationOptions()
                .SetEnvironmentName("production");

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log("Sign Success: " + AuthenticationService.Instance.PlayerId);
            return AuthenticationService.Instance.IsSignedIn;
        }
        public static async Task Setup()
        {
            await SetupDatabase();
            await GetUserInfo();
            await GetBalance();
        }

        public static async Task<CloudCodeResult> CallApi(string apiName, Dictionary<string, object> paramsHandle = null)
        {
            try
            {
                return await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>(apiName,paramsHandle);
            }
            catch (CloudCodeException err)
            {
                var trimmedMessage = err.Message;
                trimmedMessage = trimmedMessage.Substring(trimmedMessage.IndexOf('{'));
                trimmedMessage = trimmedMessage.Substring(0, trimmedMessage.LastIndexOf('}') + 1);
                return JsonConvert.DeserializeObject<CloudCodeResult>(trimmedMessage);;
            }
        }

        public static async Task<CloudCodeResult> SetupDatabase() 
        {
            var result = await CallApi(ApiName.User_Setup);

            return result;
        }

        public static async Task<CloudCodeResult> GetUserInfo() 
        {
            var result = await CallApi(ApiName.User_Get_Info);
            if(result.IsSuccess == true)
            {
                Info = JsonConvert.DeserializeObject<UserInfo>(result.Data.ToString());
            } else 
            {
                Debug.Log(result.Message);
            }
            return result;
        }
        public static async Task<CloudCodeResult> GetBalance()
        {
            var result = await CallApi(ApiName.User_Get_Balance);
            if(result.IsSuccess)
            {
                Balance = JsonConvert.DeserializeObject<Balance>(result.Data.ToString());
                Debug.Log(result.Data);
            } else
            {
                Debug.Log(result.Message);
            }
            return result;
        }

        public static async Task<CloudCodeResult> SetName(string name) 
        {
            var handleParams = new Dictionary<string, object> {{"name", name}};
 
            var result = await CallApi(ApiName.User_Set_Name,handleParams);
            Debug.Log(result.Message);
            return  result;
        }

        public static async Task<CloudCodeResult> GetCharacters()
        {
            var result = await CallApi(ApiName.User_Get_Characters);

            return result;
        }

        public static async Task<CloudCodeResult> Buy_Gem()
        {
            var result = await CallApi(ApiName.Buy_Gem);
           
            return result;
        }

        public static async Task<CloudCodeResult> Buy_Gold(string bundlePackId)
        {
            var result = await CallApi(ApiName.Buy_Gold, new Dictionary<string, object>
            {
                {"bundlePackId",bundlePackId}
            });

            return result;
        }

        public static async Task<CloudCodeResult> BuyCharacter(string characterId)
        {
            var result = await CallApi(ApiName.Buy_Character, new Dictionary<string, object>
            {
                {"characterId", characterId}
            });

            return result;
        }

        public static async Task<CloudCodeResult> SelectCharacter(string characterId)
        {
            var result = await CallApi(ApiName.User_Select_Character, new Dictionary<string, object>
            {
                {"characterId", characterId}
            });

            return result;
        }

        public static async Task<CloudCodeResult> GetCharacterInUse()
        {
            var result = await CallApi(ApiName.User_Get_Character_In_Use);
            return result;
        }


    }

    public class CloudCodeResult
    {
        public bool IsSuccess;
        public string Message;
        public object Data;
    }

}

