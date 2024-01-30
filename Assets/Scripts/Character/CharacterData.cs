using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS4.Config;

namespace HS4
{
    [System.Serializable]
    public class CharacterData : MonoBehaviour
    {
        public string Id;
        public bool IsInUse;
        public bool IsUnlocked;
        public int Price;
        public float PostionX;
        public Animator Animator;

        public void Setup(string id, bool isInUse, bool isUnlocked, int price)
        {
            Id = id;
            IsInUse = isInUse;
            IsUnlocked = isUnlocked;
            Price = price;
            PostionX = transform.position.x;

        }

        public Vector3 GetCharacterPosition() => transform.position;

       
    }

}

