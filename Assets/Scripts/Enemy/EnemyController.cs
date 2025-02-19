using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much health this enemy has")]
    protected int m_MaxHealth;

    [SerializeField]
    [Tooltip("How fast enemy can move")]
    protected float m_Speed;

    [SerializeField]
    [Tooltip("Approx amt of dmg dealt per frame")]
    protected float m_Damage;

    [SerializeField]
    [Tooltip("Explosion that occurs when enemy dies")]
    protected ParticleSystem m_DeathExplosion;

    [SerializeField]
    [Tooltip("Probability that enemy drops a health pill")]
    protected float m_HealthPillDropRate;

    [SerializeField]
    [Tooltip("The type of health pill the enemy drops")]
    protected GameObject m_HealthPill;

    [SerializeField]
    [Tooltip("The type of attack buff pill the enemy drops")]
    protected GameObject m_AttackPill;


    [SerializeField]
    [Tooltip("How many points killing this enemy provides")]
    protected int m_Score;
    #endregion

    #region Cached Components
    protected Rigidbody cc_Rb;
    #endregion

    #region Private Variables
    
    protected float p_curHealth;
    #endregion

    #region Cached References
    
    protected Transform cr_Player;
    #endregion

    #region Initialization
    private void Awake() 
    {
        p_curHealth = m_MaxHealth;

        cc_Rb = GetComponent<Rigidbody>();
    }

    protected void Start()
    {
        cr_Player = FindFirstObjectByType<PlayerController>().transform;
    }
    #endregion

    #region Main Updates
    public virtual void FixedUpdate() 
    {
        Vector3 dir = cr_Player.position - transform.position;
        dir.Normalize();
        cc_Rb.MovePosition(cc_Rb.position + dir * m_Speed * Time.fixedDeltaTime);
    }
    #endregion

    #region Collision Methods
    private void OnCollisionStay(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().DecreaseHealth(m_Damage);
        }
    }
    #endregion

    #region Health Methods
    public void DecreaseHealth(float amount)
    {   
        p_curHealth -= amount;
        if (p_curHealth <= 0)
        {
            ScoreManager.singleton.IncreaseScore(m_Score);
            DropItem();
            Instantiate(m_DeathExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    protected virtual void DropItem()
    {
        if (Random.value < m_HealthPillDropRate)
        {
            Instantiate(m_HealthPill, transform.position, Quaternion.identity);
        }
    }
    #endregion
}
