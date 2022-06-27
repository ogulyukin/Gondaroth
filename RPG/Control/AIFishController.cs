using UnityEngine;

namespace RPG.Control
{
    public class AIFishController : MonoBehaviour
    {
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] private float swimSpeed = 1f;
        private const float WaypointTolerance = 1f;
        private int _currentWaypointIndex;
        private float _timeSinceArriveAtWaypoint = Mathf.Infinity;

        private void Start()
        {
            _currentWaypointIndex = 0;
        }
        
        private void Update()
        {

            if (AtWaypoint())
            {
                CycleWayPoint();
                GetCurrentWaypoint();
            }
            
            if (_timeSinceArriveAtWaypoint > waypointDwellTime)
            {
                transform.LookAt(GetCurrentWaypoint());
                transform.Translate(Vector3.forward * Time.deltaTime * swimSpeed);
            }
            else
            {
                _timeSinceArriveAtWaypoint += Time.deltaTime;
            }
        }
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }
        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < WaypointTolerance;
        }
        
        private void CycleWayPoint()
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex == patrolPath.transform.childCount)
            {
                _currentWaypointIndex = 0;
            }
            _timeSinceArriveAtWaypoint = 0f;
            transform.LookAt(GetCurrentWaypoint());
        }
    }
}
