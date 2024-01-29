using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public class UICharacter : MonoBehaviour
    {
        public CharacterData CharacterData;
        [SerializeField] private Button _btnSelect;

        public bool IsTargetVew
        {
            set
            {
                _btnSelect.gameObject.SetActive(value);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void Setup(CharacterData characterData)
        {
            gameObject.SetActive(true);
            CharacterData = characterData;

        }
    }

}

