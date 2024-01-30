using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using JetBrains.Annotations;

namespace HS4.UI
{
    public class UISelectCharacter : Singleton<UISelectCharacter>, IDragHandler
    {
        public Camera SelectCharacterCamera;
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private GameObject _test;

        [SerializeField] private GameObject _uiCharacterContainer;
        [SerializeField] private GameObject _uiCharacterOb;
        public List<UICharacter> UiCharacterList = new();

        [SerializeField] private Button _nextBtn;
        [SerializeField] private Button _prevBtn;

        private bool _isCompleteSetup;
        private float _dragSpeed = 2.0f;
        private float _maxPos;

        private Vector3 _startDragPosition;
        [SerializeField] private float _currentViewPosX;


        private void Start()
        {
            _nextBtn.onClick.AddListener(() => MoveCamera(3f));
            _prevBtn.onClick.AddListener(() => MoveCamera(-3f));
        }

        private void MoveCamera(float offset)
        {
            if (!_isCompleteSetup)
                return;

            _currentViewPosX = Mathf.Clamp(_currentViewPosX - offset, _maxPos, 0);
            SelectCharacterCamera.transform.DOMoveX(_currentViewPosX, 1);
        }


        public void Active(Camera camera)
        {
            SelectCharacterCamera = camera;
            //instantiate
            InstantiateUI();
            _maxPos = -(CharacterManager.Instance.CharacterSpawnedList.Count - 1) * 3f;
            _isCompleteSetup = true;

            var characterInUse = UiCharacterList.Find(item => item.CharacterData.IsInUse);
            _currentViewPosX = characterInUse.CharacterData.PostionX;
            MoveCamera(0);
        }

        public void Deactive()
        {
            UiCharacterList.ForEach(item => Destroy(item.gameObject));
            UiCharacterList.Clear();
            _isCompleteSetup = false;
        }

        private void InstantiateUI()
        {
            UiCharacterList.ForEach(item => Destroy(item.gameObject));
            UiCharacterList.Clear();

            foreach(var character in CharacterManager.Instance.CharacterSpawnedList)
            {
                var uiCharacterOb = Instantiate(_uiCharacterOb, _uiCharacterContainer.transform);
                var script = uiCharacterOb.GetComponent<UICharacter>();
                script.Setup(character);
                UiCharacterList.Add(script);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isCompleteSetup)
            {
                if (eventData.delta != Vector2.zero)
                {
                    float screenWidth = Screen.width;

                    float normalizedX = eventData.delta.x / screenWidth;

                    float dragSpeedX = _dragSpeed * screenWidth / 1000.0f;

                    Vector3 newPosition = SelectCharacterCamera.transform.position +
                        new Vector3(normalizedX * dragSpeedX, 0, 0);


                    newPosition.x = Mathf.Clamp(newPosition.x, _maxPos, 0);
                    _currentViewPosX = newPosition.x;
                    SelectCharacterCamera.transform.position = newPosition;
                }  
        }

        }


        void Update()
        {
            if (Screen.width != _renderTexture.width || Screen.height != _renderTexture.height)
            {
                AdjustSize();
            }

            if(_isCompleteSetup)
            {
               foreach (var character in UiCharacterList)
               {
                    var characterPos = ConvertWorldToCanvas(CharacterManager.Instance.GetCharacter(character.CharacterData.Id).GetCharacterPosition());
                    character.GetComponent<RectTransform>().localPosition = characterPos;
                    character.IsTargetVew = Mathf.Abs(character.CharacterData.PostionX - _currentViewPosX) < 1.5f;
                }
            }
        }

        public void RefreshSelect() =>
            UiCharacterList.ForEach(item => {
                item.CharacterData.IsInUse = false;
                item.CharacterData.Animator.Play("Idle");
            });
        

        void AdjustSize()
        {
            _renderTexture.Release();
            _renderTexture.width = Screen.width;
            _renderTexture.height = Screen.height;
            _renderTexture.Create();
        }

        public Vector2 ConvertWorldToCanvas(Vector3 worldPosition)
        {
            var canvasRectTransform = gameObject.GetComponent<RectTransform>();
            if (canvasRectTransform != null)
            {
                Vector2 screenPosition = SelectCharacterCamera.WorldToScreenPoint(worldPosition);

                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, null, out canvasPosition);
                return canvasPosition;
            }
            else
            {
                return Vector2.zero;
            }
        }


    }
}
