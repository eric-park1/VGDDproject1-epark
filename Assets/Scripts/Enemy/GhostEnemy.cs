using UnityEngine;

public class GhostEnemy : EnemyController
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void DropItem()
    {
        if (Random.value < m_HealthPillDropRate)
        {
            Instantiate(m_HealthPill, transform.position, Quaternion.identity);
        }
    }
}
    