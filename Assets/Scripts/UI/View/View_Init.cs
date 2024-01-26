using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
                    User.Setup();
                    break;
                }
                 
            }
            await Task.Delay(2000);
            UIManager.Instance.ToggleView(ViewName.Home);
           
                
        }

       
    }

}
