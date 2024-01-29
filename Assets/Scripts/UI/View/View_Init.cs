using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using HS4.Config;

namespace HS4.UI
{
    public class View_Init : UIView
    {
        // Start is called before the first frame update
        async void Start()
        {
            while(true) {

                bool isSignedIn =  await User.SignInAnonymouslyAsync();

                if(isSignedIn) {
                    await User.Setup();
                    ConfigManager.Instance.Init();
                    break;
                }
                 
            }
            await Task.Delay(1000);
            UIManager.Instance.ToggleView(ViewName.Home);
           
                
        }

       
    }

}
