using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;

namespace HS4
{
    public class Global : MonoBehaviour
    {
        public static Global Instance;
       
         void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
         
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        void Start() => DontDestroyOnLoad(gameObject);


    }

}
