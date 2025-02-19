using UnityEngine;
using System.Collections;

public class NinjaEnemy : EnemyController
{
    [SerializeField]
    [Tooltip("Maximum teleport distance from the current position")]
    private float m_TeleportRange = 5f;

    private void Start()
    {
        base.Start();
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(7f); 

            Teleport();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void Teleport()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-m_TeleportRange, m_TeleportRange),
            0,
            Random.Range(-m_TeleportRange, m_TeleportRange)
        );

        transform.position += randomOffset; 
    }

    protected override void DropItem()
    {
        if (Random.value < m_HealthPillDropRate)
        {
            Instantiate(m_AttackPill, transform.position, Quaternion.identity);
        }
    }

}
