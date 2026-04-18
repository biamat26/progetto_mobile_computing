using UnityEngine;

public class EnemyEngagementManager : MonoBehaviour
{
    public static EnemyEngagementManager Instance { get; private set; }

    [Header("Global Engagement")]
    [SerializeField] private bool useGlobalEngageDistance = true;
    [SerializeField] private float globalEngageDistance = 6f;

    private float globalEngageDistanceSqr;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        globalEngageDistanceSqr = globalEngageDistance * globalEngageDistance;
    }

    private void OnValidate()
    {
        if (globalEngageDistance < 0f)
            globalEngageDistance = 0f;

        globalEngageDistanceSqr = globalEngageDistance * globalEngageDistance;
    }

    public bool IsInsideEngageDistance(Vector2 enemyPosition, Vector2 playerPosition, float localEngageDistance)
    {
        if (localEngageDistance < 0f)
            localEngageDistance = 0f;

        float threshold = useGlobalEngageDistance ? globalEngageDistance : localEngageDistance;
        float thresholdSqr = useGlobalEngageDistance ? globalEngageDistanceSqr : threshold * threshold;

        return (playerPosition - enemyPosition).sqrMagnitude <= thresholdSqr;
    }
}
