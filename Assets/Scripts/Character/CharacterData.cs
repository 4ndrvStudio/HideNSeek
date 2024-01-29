using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS4.Config;

namespace HS4
{
    public class CharacterData : MonoBehaviour
    {
        public string Id;
        public bool IsInUse;
        public bool IsUnlocked;
        public int Price;

        public void Setup(string id, bool isInUse, bool isUnlocked, int price)
        {
            Id = id;
            IsInUse = isInUse;
            IsUnlocked = isUnlocked;
            Price = price;
        }

        public Vector3 GetPosition() => transform.position;
    }

}

