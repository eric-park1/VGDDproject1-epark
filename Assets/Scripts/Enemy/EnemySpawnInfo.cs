using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The name of this enemy")]
    private string m_Name;
    public string name
    {
        get
        {
            return m_Name;
        }
    }

    [SerializeField]
    [Tooltip("The prefab of the enemy that will be spawned")]
    private GameObject m_EnemyGO;
    public GameObject EnemyGO
    {
        get
        {
            return m_EnemyGO;
        }
    }

    [SerializeField]
    [Tooltip("The number of enemies to spawn. If set to 0, it will spawn endlessly")]
    private float m_TimeToNextSpawn;
    public float TimeToNextSpawn
    {
        get
        {
            return m_TimeToNextSpawn;
        }
    }

    [SerializeField]
    [Tooltip("The number of seconds before the next enemy is spawned")]
    private int m_NumberToSpawn;
    public int NumberToSpawn
    {
        get
        {
            return m_NumberToSpawn;
        }
    }
    #endregion
}
