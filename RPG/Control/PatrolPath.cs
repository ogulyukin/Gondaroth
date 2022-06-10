using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(GetWaypoint(i), 0.3f);
                Gizmos.DrawLine(GetWaypoint(i), i + 1 < transform.childCount ? GetWaypoint(i + 1) : GetWaypoint(0));
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
