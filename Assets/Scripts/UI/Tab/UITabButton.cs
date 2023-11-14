using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public enum UITabName {
        Join,
        Create
    }
    public abstract class UITabButton : MonoBehaviour
    {   
        public UITabName TabName;
        [HideInInspector] public Button Button;

        public void Awake() {
            Button = GetComponent<Button>();
        }

        public virtual void Active() {}
        public virtual void Deactive() {}
    }

}
