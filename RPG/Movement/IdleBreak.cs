using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

public class IdleBreak : MonoBehaviour, IAction
{
    [SerializeField] private float idleTime = 3f;
    private float timeIdle = 0;
    private bool m_IsIdle;
    void Update()
    {
        if (m_IsIdle)
        {
            timeIdle += Time.deltaTime;
            if(timeIdle > idleTime) Cancel();    
        }
    }

    public void StartIdleBreak()
    {
        if(m_IsIdle) return;
        print("Idle Break");
        m_IsIdle = true;
        timeIdle = 0;
        GetComponent<ActionScheduler>().StartAction(this);
        var state = GetComponent<Animator>().GetFloat("CombatStyle");
        GetComponent<Animator>().SetTrigger( state == 0 ? "IdleBreak" : "CombatIdleBreak");
    }
    public void Cancel()
    {
        m_IsIdle = false;
        GetComponent<Animator>().ResetTrigger("IdleBreak");
        GetComponent<Animator>().ResetTrigger("CombatIdleBreak");
        GetComponent<Animator>().SetTrigger( "stopBreakIddle");
    }
}
