using UnityEngine;

public class AttackPill : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The amount of attack the pill helps player gain")]
    private int m_AttackGain;
    public int AttackGain
    {
        get
        {
            return m_AttackGain;
        }
    }
    #endregion
    
}
