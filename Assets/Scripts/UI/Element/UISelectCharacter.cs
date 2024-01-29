using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HS4
{
    public class UISelectCharacter : MonoBehaviour, IDragHandler
    {
        public Camera SelectCharacterCamera;
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private GameObject _test;

        private float _dragSpeed = 2.0f;

        private Vector3 _startDragPosition;

        public void OnDrag(PointerEventData eventData)
        {
            if (SelectCharacterCamera != null)
            {
                if (eventData.delta != Vector2.zero)
                {
                    float maxPos = -(4 - 1) * 3f;

                    float screenWidth = Screen.width;

                    float normalizedX = eventData.delta.x / screenWidth;

                    float dragSpeedX = _dragSpeed * screenWidth / 1000.0f;

                    Vector3 newPosition = SelectCharacterCamera.transform.position +
                        new Vector3(normalizedX * dragSpeedX, 0, 0);


                    newPosition.x = Mathf.Clamp(newPosition.x, maxPos, 0);
                    SelectCharacterCamera.transform.position = newPosition;
                }

                if (CharacterManager.Instance != null)
                {
                    _test.GetComponent<RectTransform>().localPosition = ConvertWorldToCanvas(CharacterManager.Instance.CharacterSpawnedList[0].GetPosition());
                }
        }

        }

        void Update()
        {
            if (Screen.width != _renderTexture.width || Screen.height != _renderTexture.height)
            {
                AdjustSize();
            }
        }

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
                Debug.LogError("Canvas RectTransform is not assigned!");
            }
        }


    }
}
