using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4.UI
{
    public abstract class UITab : MonoBehaviour
    {
        public UITabName TabName;

        public virtual void Active() 
        {
            gameObject.SetActive(true);
        }
        public virtual void Deactive() 
        {
            gameObject.SetActive(false);
        }

    }

}
