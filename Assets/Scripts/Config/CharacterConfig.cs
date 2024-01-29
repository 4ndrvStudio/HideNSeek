using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4.Config
{
    [CreateAssetMenu(fileName = "Character Config", menuName = "Data/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        public string Id;
        public GameObject Prefab;
    }

}
