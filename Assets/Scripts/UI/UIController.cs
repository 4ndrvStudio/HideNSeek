using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4.UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        [SerializeField] private GameObject _controller;
        public Joystick MoveJoyStick;
        
        // Start is called before the first frame update
        void Start()
        {
            if(Instance == null) 
                Instance = this;
        }

        public void Active() => _controller.SetActive(true);
        public void Deactive(){
            
            MoveJoyStick.input.x = 0;
            MoveJoyStick.input.y = 0;
            _controller.SetActive(false);
        } 


     
    }

}
