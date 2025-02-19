using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityInfo 
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Too much power this ability has")]
    private int m_Power;
    public int Power
    {
        get
        {
            return m_Power;
        }
    }

    public void IncreasePower(float amount)
    {
        m_Power += Mathf.RoundToInt(amount);
    }

    public void SetPower(int newPower)
    {
        m_Power = newPower;
    }

    [SerializeField]
    [Tooltip("If this is an attack that shoots something out, this value describes how far the attack can shoot")]
    private float m_Range;
    public float Range
    {
        get
        {
            return m_Range;
        }
    }
    #endregion
}
