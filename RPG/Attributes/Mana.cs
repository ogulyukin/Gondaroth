using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    private float _manaPoints;
    [SerializeField] private Slider manaSlider;

    private void Start()
    {
        _manaPoints = GetComponent<BaseStats>().GetStat(MainStats.Intellect);
    }
    private void Update()
    {
        RestoreMana(0.1f);
    }

    public bool CheckManaAvaible(float mana)
    {
        return mana < _manaPoints;
    }

    public void SpendMana(float manaAmount)
    {
        _manaPoints = Mathf.Max(_manaPoints - manaAmount, 0);
        manaSlider.value = _manaPoints / GetComponent<BaseStats>().GetStat(MainStats.Intellect);
    }

    public void RestoreMana(float manaAmount)
    {
        var totalMana = GetComponent<BaseStats>().GetStat(MainStats.Intellect);
        _manaPoints += manaAmount;
        if (_manaPoints > totalMana) _manaPoints = totalMana;
        manaSlider.value = _manaPoints / totalMana;
    }
}
