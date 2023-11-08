using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;

namespace HS4
{
    public class AuthenticationManager : MonoBehaviour
    {
        async void Awake()
        {
            InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName("production");

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log("Sign Success");
        }

    
    }

}
