using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Taunt : MonoBehaviour, IAction
    {
        private bool m_IsTaunting;
        private float m_TauntTime = 5f;
        private float m_TimeTaunt = 0f;
        
        private void Update()
        {
            if (m_IsTaunting)
            {
                m_TimeTaunt += Time.deltaTime;
                if(m_TimeTaunt > m_TauntTime) Cancel();
            }
        }

        public void StartTaunting()
        {
            if(m_IsTaunting) return;
            m_IsTaunting = true;
            m_TimeTaunt = 0;
            GetComponent<ActionScheduler>().StartAction(this);
            GetComponent<Animator>().SetTrigger("Taunt");
        }

        public void Cancel()
        {
            m_IsTaunting = false;
            GetComponent<Animator>().ResetTrigger("Taunt");
            GetComponent<Animator>().SetTrigger("stopTaunt");
            //if(GetComponent<CombatTarget>().m_CurrentState == UnitStates.Fighting) GetComponent<Fighter>().TryAttackLastTarget();
            //if(GetComponent<CombatTarget>().m_CurrentState == UnitStates.Moving) GetComponent<Mover>().RestoreMovement();
        }
    }
}
