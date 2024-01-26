using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectCharacter : MonoBehaviour, IDragHandler
{
    [SerializeField] private Camera _selectCharacterCamera;
    [SerializeField] private RenderTexture _renderTexture;
    

    private float _dragSpeed = 2.0f;

    private Vector3 _startDragPosition;

    public void OnDrag(PointerEventData eventData)
    {
        if (_selectCharacterCamera != null)
        {
            if (eventData.delta != Vector2.zero)
            {
                float currentCameraPosX = _selectCharacterCamera.transform.position.x;
                float maxPos = -(4 - 1) * 3f;

                
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;

                float normalizedX = eventData.delta.x / screenWidth;

                    float dragSpeedX = _dragSpeed * screenWidth / 1000.0f;

                Vector3 newPosition = _selectCharacterCamera.transform.position +
                    new Vector3(normalizedX * dragSpeedX, 0, 0);

                    
             newPosition.x = Mathf.Clamp(newPosition.x, maxPos, 0);
            _selectCharacterCamera.transform.position = newPosition;
            }
        }
        Debug.Log(System.Globalization.RegionInfo.CurrentRegion);
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
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
        // Điều chỉnh kích thước Render Texture để phản ánh kích thước cửa sổ chơi
        _renderTexture.Release();
        _renderTexture.width = Screen.width;
        _renderTexture.height = Screen.height;
        // Cập nhật kích thước Render Texture
        _renderTexture.Create();
    }

    
}
