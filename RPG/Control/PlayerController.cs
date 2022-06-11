using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using RPG.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public enum CursorType
    {
        None, Movement, Combat, UI, PickUp, FullPickup,
        Dialogue
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 1f;
        [SerializeField] private GameObject pauseCanvas;
        private bool _isDragging;
        private bool _warMode;
        private MagicControl _magicControl;
        private NavMeshAgent _navMeshAgent;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMappings;
        private Mover _mover;
        private Health _health;
        private static readonly int CombatStyle = Animator.StringToHash("CombatStyle");

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
            _magicControl = GetComponent<MagicControl>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.Escape)) pauseCanvas.GetComponent<PauseCanvas>().PauseCanvasHandler();
            if(Input.GetKeyUp(KeyCode.R)) _mover.ToggleNavMeshSpeed();
            if(_health.IsAlive()) Interaction();
        }

        
        private void Interaction()
        {
            if (InteractWithUI()) return;
            if (!_health.IsAlive())
            {
                SetCursor(CursorType.None);
                return;
            }
            if(Input.GetKeyUp(KeyCode.Tab)) ToggleWarMode();
            if(InteractWithComponent()) return;
            //if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (var hit in hits)
            {
                var raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }    
                }
            }
            return false;
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0)) _isDragging = false;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0)) _isDragging = true;
                SetCursor(CursorType.UI);
                return true;
            }

            if (_isDragging) return true;
            return false;
        }
        private bool InteractWithMovement()
        {
            if (Input.GetKey(KeyCode.A))
            {
                _mover.StartRotation(false);
                return true;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                _mover.StartRotation(true);
                return true;
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                var dest = transform.position + transform.forward;
                return DirectMove(dest);
            }

            if (Input.GetKey(KeyCode.S))
            {
                //not work properly
                //var dest = transform.position - transform.forward;
                //return DirectMove(dest);
            }
            if (RayCastOnNavMesh(out Vector3 target))
            {
                if (!_mover.CanMoveTo(target)) return false;
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoving(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool DirectMove(Vector3 dest)
        {
            if (!_mover.CanMoveTo(dest)) return false;
            _mover.StartMoving(dest);
            return true;
        }

        private Vector3 NormalToCameraView(float x)
        {
            return Vector3.Cross((transform.position - Camera.main.transform.position).normalized, Vector3.down) * x;
        }
        
        private bool RayCastOnNavMesh(out Vector3 target)
        {
            target = new Vector3();
            if (!Physics.Raycast(GetMouseRay(), out var hit)) return false;
            if (!NavMesh.SamplePosition(hit.point, out var navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas)) return false;
            target = navMeshHit.position;
            return true;
        }
        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType type)
        {
            var mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            for (var i = 0; i < cursorMappings.Length; i++)
            {
                if (cursorMappings[i].type == type)
                {
                    return cursorMappings[i];
                }
            }
            return cursorMappings[0];
        }

        private void ToggleWarMode()
        {
            GetComponent<Animator>().SetFloat(CombatStyle, _warMode ? 1f : 0f);
            _warMode = !_warMode;
        }
    }
}
