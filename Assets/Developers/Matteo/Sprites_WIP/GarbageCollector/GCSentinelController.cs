using UnityEngine;

public class GCSentinelController : MonoBehaviour
{
    public Transform[] waypoints;   // assegna dal Inspector
    public float speed = 2f;
    public float waypointRadius = 0.1f;

    private int _currentWP = 0;
    private Animator _anim;
    private Transform _visionCone;

    // Direzioni: 0=giù 1=su 2=sinistra 3=destra
    private static readonly Vector2[] Dirs = {
        Vector2.down, Vector2.up, Vector2.left, Vector2.right
    };

    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _visionCone = transform.Find("VisionCone");
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[_currentWP];
        Vector2 dir = (target.position - transform.position).normalized;

        transform.Translate(dir * speed * Time.deltaTime);
        UpdateAnimation(dir);
        RotateVisionCone(dir);

        if (Vector2.Distance(transform.position, target.position) < waypointRadius)
            _currentWP = (_currentWP + 1) % waypoints.Length;
    }

    void UpdateAnimation(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        int d;
        if      (angle > 45  && angle <= 135)  d = 1; // su
        else if (angle < -45 && angle >= -135) d = 0; // giù
        else if (dir.x < 0)                    d = 2; // sinistra
        else                                   d = 3; // destra

        _anim.SetInteger("dir", d);
    }

    void RotateVisionCone(Vector2 dir)
    {
        if (_visionCone != null)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _visionCone.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}