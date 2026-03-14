using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCone : MonoBehaviour
{
    [Header("Forma")]
    public float distance = 3f;
    public float angle    = 60f;
    public int   rays     = 16;

    [Header("Layer muri")]
    public LayerMask wallMask;

    [Header("Aspetto")]
    public Color coneColor = new Color(1f, 0.2f, 0.1f, 0.35f);

    [Header("UI")]
    public GameObject gameOverPanel;  // trascina il Panel qui dall'Inspector

    private Mesh         _mesh;
    private MeshFilter   _mf;
    private MeshRenderer _mr;
    private bool         _triggered = false;

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();

        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = coneColor;
        _mr.material     = mat;
        _mr.sortingLayerName = "Enemy";
        _mr.sortingOrder = 0;

        _mesh = new Mesh { name = "VisionConeMesh" };
        _mf.mesh = _mesh;
    }

    void LateUpdate()
    {
        BuildCone();
        if (!_triggered) CheckPlayerInCone();
    }

    void CheckPlayerInCone()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, distance);
        foreach (var col in hits)
        {
            if (!col.CompareTag("Player")) continue;

            Vector2 toPlayer = col.transform.position - transform.position;
            float a = Vector2.Angle(AngleToDir(transform.eulerAngles.z), toPlayer);
            if (a > angle * 0.5f) continue;

            RaycastHit2D wall = Physics2D.Raycast(
                transform.position,
                toPlayer.normalized,
                toPlayer.magnitude,
                wallMask
            );
            if (wall) continue;

            // Player visto!
            _triggered = true;
            ShowGameOver();
        }
    }

    void ShowGameOver()
    {
        // Ferma il tempo di gioco
        Time.timeScale = 0f;

        // Mostra il pannello
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // Chiama questo dal bottone "Riprova"
    public void RestartLevel()
    {
        Time.timeScale = 1f;  // ripristina il tempo!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void BuildCone()
    {
        float halfAngle = angle * 0.5f;
        float angleStep = angle / rays;

        var verts = new Vector3[rays + 2];
        var tris  = new int[rays * 3];
        verts[0]  = Vector3.zero;

        for (int i = 0; i <= rays; i++)
        {
            float worldAngle = transform.eulerAngles.z + (-halfAngle + angleStep * i);
            float rad        = worldAngle * Mathf.Deg2Rad;
            Vector2 dir      = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, wallMask);
            Vector2 worldPoint = hit
                ? hit.point
                : (Vector2)transform.position + dir * distance;

            verts[i + 1] = transform.InverseTransformPoint(worldPoint);
        }

        for (int i = 0; i < rays; i++)
        {
            tris[i * 3]     = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices  = verts;
        _mesh.triangles = tris;
        _mesh.RecalculateNormals();
    }

    Vector2 AngleToDir(float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}