using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Newtonsoft.Json;

namespace HS4.UI
{
    public struct ApiName {
        public static string User_Get_Info = "user_get_info";
        public static string User_Set_Name = "user_set_name";
    }

    public class UserInfo
    {
        public string? UserName;
        public int Level;
        public int Exp;
    }
 

    public static class User
    {
        public static UserInfo Info;

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

        public static async Task<CloudCodeResult> SetName(string name) 
        {
            var handleParams = new Dictionary<string, object> {{"name", name}};
 
            var result = await CallApi(ApiName.User_Set_Name,handleParams);
            Debug.Log(result.Message);
            return  result;
        }




    }

    public class CloudCodeResult
    {
        public bool IsSuccess;
        public string Message;
        public object Data;
    }

}

