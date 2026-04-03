using UnityEngine;

public class RAMBackgroundSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public GameObject backgroundPrefab;
    public int larghezza = 3;
    public int altezza   = 3;
    public float offsetX = 40f; // Cols * TileSize
    public float offsetY = 25f; // Rows * TileSize

    void Start()
    {
        for (int x = 0; x < larghezza; x++)
        for (int y = 0; y < altezza;   y++)
        {
            Vector3 pos = new Vector3(
                transform.position.x + x * offsetX,
                transform.position.y + y * offsetY,
                0
            );
            Instantiate(backgroundPrefab, pos, Quaternion.identity, transform);
        }
    }
}
