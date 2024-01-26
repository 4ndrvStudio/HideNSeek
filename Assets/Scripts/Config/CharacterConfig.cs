using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4
{
    [CreateAssetMenu(fileName = "Character Settings", menuName = "Data/Character Settings")]
    public class CharacterConfig : ScriptableObject
    {
        public List<Characters> Characters;

        public Prices GetCharacterPrices(string id)  {
            var character = Characters.Find(item=> item.Id == id);
            return character.Prices;
        } 
    }

}
