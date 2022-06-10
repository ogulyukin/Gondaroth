using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main != null) transform.forward = Camera.main.transform.forward;
        }
    }
}
