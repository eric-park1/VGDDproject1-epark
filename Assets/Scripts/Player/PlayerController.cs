using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("How fast the player should move when running around")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("The transform of the camera following the rotation")]
    private Transform m_CameraTransform;

    [SerializeField]
    [Tooltip("The list of all attacks and info about them")]
    private PlayerAttackInfo[] m_Attacks;

    [SerializeField]
    [Tooltip("Amount of health the player starts with")]
    private int m_MaxHealth;

    [SerializeField]
    [Tooltip("The HUD Script")]
    private HUDController m_HUD;
    #endregion

    #region Cached References
    private Animator cr_Anim;
    private Renderer cr_Renderer;
    #endregion


    #region Cached Componenets
    private Rigidbody cc_Rb;
    #endregion

    #region Private Variable
    //current move direction of player, does NOT include magnitude
    private Vector2 p_Velocity;

    private float p_FrozenTimer;

    //the default color
    private Color p_DefaultColor;

    //current amount of health the player has
    private float p_CurHealth;
    #endregion

    #region Initialization
    private void Awake() 
    {
        p_Velocity = Vector2.zero;
        cc_Rb = GetComponent<Rigidbody>();
        cr_Anim = GetComponent<Animator>();
        cr_Renderer = GetComponentInChildren<Renderer>();
        p_DefaultColor = cr_Renderer.material.color;

        p_FrozenTimer = 0;
        p_CurHealth = m_MaxHealth;

        for (int i=0; i < m_Attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_Attacks[i];
            attack.Cooldown = 0;

            if (attack.WindUpTime > attack.FrozenTime) 
            {
                Debug.LogError(attack.AttackName + "has a wind up time that is larger than the amount of time the player is frozen for");
            }
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

    #region Main Updates
    private void Update() 
    {
        if (p_FrozenTimer > 0)
        {
            p_Velocity = Vector2.zero;
            p_FrozenTimer -= Time.deltaTime;
            return;
        } 
        else
        {
            p_FrozenTimer = 0;
        }

        //ability use
        for (int i=0; i < m_Attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_Attacks[i];

            if (attack.IsReady())
            {
                if (Input.GetButtonDown(attack.Button))
                {
                    p_FrozenTimer = attack.FrozenTime;
                    DecreaseHealth(attack.HealthCost);
                    StartCoroutine(UseAttack(attack));
                    break;
                }
            } else if (attack.Cooldown > 0) 
            {
                attack.Cooldown -= Time.deltaTime;
            }
        }

        //set how hard the player is pressing movement buttons
        float forward = Input.GetAxis("Vertical");
        float right = Input.GetAxis("Horizontal");

        //updating the animation
        cr_Anim.SetFloat("Speed", Mathf.Clamp01(Mathf.Abs(forward) + Mathf.Abs(right)));

        //updating velocity
        float moveThreshold = 0.3f;

        if (forward > 0 && forward < moveThreshold) 
        {
            forward = 0;
        } else if (forward < 0 && forward > -moveThreshold) 
        {
            forward = 0;
        } if (right > 0 && right < moveThreshold) {
            right = 0;
        } if (right < 0 && right > -moveThreshold) 
        {
            right = 0;
        }
        
        p_Velocity = new Vector2(right, forward);
            
    }

    private void FixedUpdate() 
    {
        cc_Rb.MovePosition(cc_Rb.position + m_Speed * Time.fixedDeltaTime * transform.forward * p_Velocity.magnitude);

        //update rotation of player
        cc_Rb.angularVelocity = Vector3.zero;

        if (p_Velocity.sqrMagnitude > 0)
        {
            float angleToRotCam = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.up, p_Velocity);
            Vector3 camForward = m_CameraTransform.forward;
            Vector3 newRot = new Vector3(Mathf.Cos(angleToRotCam) * camForward.x - Mathf.Sin(angleToRotCam) * camForward.z, 0,
                Mathf.Cos(angleToRotCam) * camForward.z + Mathf.Sin(angleToRotCam) * camForward.x);
            float theta = Vector3.SignedAngle(transform.forward, newRot, Vector3.up);
            cc_Rb.rotation = Quaternion.Slerp(cc_Rb.rotation, cc_Rb.rotation * Quaternion.Euler(0, theta, 0), 0.2f);

        }
    }

    #endregion
    
    #region Health/Dying methods
    public void DecreaseHealth(float amount) 
    {
        p_CurHealth -= amount;
        m_HUD.UpdateHealth(1.0f * p_CurHealth / m_MaxHealth);
        if (p_CurHealth <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void IncreaseHealth(float amount) 
    {
        p_CurHealth += amount;
        if (p_CurHealth > m_MaxHealth)
        {
            p_CurHealth = m_MaxHealth;
        }
        m_HUD.UpdateHealth(1.0f * p_CurHealth / m_MaxHealth);
    }
    #endregion

    #region Attack Methods
    private IEnumerator UseAttack(PlayerAttackInfo attack)
    {
        cc_Rb.rotation = Quaternion.Euler(0, m_CameraTransform.eulerAngles.y, 0);
        cr_Anim.SetTrigger(attack.TriggerName);
        IEnumerator toColor = ChangeColor(attack.AbilityColor, 10);
        StartCoroutine(toColor);
        yield return new WaitForSeconds(attack.WindUpTime);

        Vector3 offset = transform.forward * attack.Offset.z + transform.right * attack.Offset.x + transform.up * attack.Offset.y;
        GameObject go = Instantiate(attack.AbilityGO, transform.position + offset, cc_Rb.rotation);
        go.GetComponent<Ability>().Use(transform.position + offset);

        StopCoroutine(toColor);
        StartCoroutine(ChangeColor(p_DefaultColor, 50));
        yield return new WaitForSeconds(attack.Cooldown);

        attack.ResetCooldown();
    }

    private IEnumerator IncreaseAttack(float amount)
    {
        List<int> originalPowers = new List<int>();

        for (int i = 0; i < m_Attacks.Length; i++)
        {
            Ability ability = m_Attacks[i].AbilityGO.GetComponent<Ability>();

            if (ability != null && ability.m_Info != null)
            {
                originalPowers.Add(ability.m_Info.Power);
                ability.m_Info.IncreasePower(amount); 
            }
        }

        yield return new WaitForSeconds(10f); 

        for (int i = 0; i < m_Attacks.Length; i++)
        {
            Ability ability = m_Attacks[i].AbilityGO.GetComponent<Ability>();

            if (ability != null && ability.m_Info != null)
            {
                ability.m_Info.SetPower(originalPowers[i]);
            }
        }
    }

    #endregion

    #region Misc Methods
    private IEnumerator ChangeColor(Color newColor, float speed)
    {
        Color curColor = cr_Renderer.material.color;
        while (curColor != newColor)
        {
            curColor = Color.Lerp(curColor, newColor, speed/100);
            cr_Renderer.material.color = curColor;
            yield return null;
        }
    }
    #endregion

    #region Collision Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPill"))
        {
            IncreaseHealth(other.GetComponent<HealthPill>().HealthGain);
        }
        else if (other.CompareTag("AttackPill"))
        {
            float attackGain = other.GetComponent<AttackPill>().AttackGain;
            StartCoroutine(IncreaseAttack(attackGain));
        }
        Destroy(other.gameObject);
    }
    #endregion
}