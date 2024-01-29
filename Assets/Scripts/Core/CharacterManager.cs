using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HS4
{
    using System.Linq;
    using Config;

    public class CharacterManager : Singleton<CharacterManager>
    {
        public Camera SelectCharacterCamera;

        [SerializeField] private GameObject _characterContainer;
        [SerializeField] private List<Backend.Character> _characterOwnedList = new();

        public List<CharacterData> CharacterSpawnedList = new();

        // Start is called before the first frame update
        void Start()
        {
            GetAllCharacter();
        }

        public async void GetAllCharacter()
        {
            ClearData();
            var characterOwnedResult = await User.GetCharacters();
            _characterOwnedList = JsonConvert.DeserializeObject<List<Backend.Character>>(
                                    characterOwnedResult.Data.ToString());

            var _characterConfigs = ConfigManager.Instance.CharacterConfigs;

            for (int i= 0; i< _characterConfigs.Count; i++)
            {
                bool hasOwn = _characterOwnedList.Any(itemCharacter => itemCharacter.InventoryItemId == _characterConfigs[i].Id.ToUpper());
                var characterOb = Instantiate(_characterConfigs[i].Prefab, _characterContainer.transform);
                var characterData = characterOb.GetComponent<CharacterData>();
                characterOb.transform.position = new Vector3(-i * 3f, 0, 0);
                characterData.Setup(
                    id: _characterConfigs[i].Id,
                    isInUse: false,
                    isUnlocked: hasOwn,
                    price: ConfigManager.Instance.GetCharacterPrice(_characterConfigs[i].Id)
                    );
                CharacterSpawnedList.Add(characterData);
            }
        }

        public void ClearData()
        {
            CharacterSpawnedList.ForEach(item => Destroy(item));
            CharacterSpawnedList.Clear();
        }
    }
}

