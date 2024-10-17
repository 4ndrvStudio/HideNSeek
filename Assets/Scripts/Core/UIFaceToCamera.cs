using UnityEngine;

namespace masterland.UI
{
     public class FaceToCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            Transform mainCamara = Camera.main.transform;
            transform.LookAt(transform.position + mainCamara.rotation * Vector3.forward, mainCamara.rotation * Vector3.up);
        }
    }
}
