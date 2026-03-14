using UnityEngine;

public class AxiomSphericalCamera : MonoBehaviour 
{
    public AxiomFactory factory;

    [Header("Adaptive Constraints")]
    public float minRadius = 2.0f;     
    public float maxRadius = 50.0f;    
    public Vector2 pitchLimits = new Vector2(-15f, 80f); 

    [Header("Sensitivity")]
    public float rotateSpeed = 5.0f;
    public float zoomSpeed = 10.0f;
    public float smoothing = 8.0f;

    [Header("Current State")]
    public float radius = 10f;
    public float yaw = 45f;   
    public float pitch = 30f; 

    private float _targetRadius;
    private float _targetYaw;
    private float _targetPitch;

    void Start() 
    {
        _targetRadius = radius;
        _targetYaw = yaw;
        _targetPitch = pitch;
    }

    /// <summary>
    /// Senior Logic: Calculates adaptive zoom limits based on the furthest point in the 3D scene.
    /// </summary>
    public void FocusAll() 
    {
        var objects = factory.GetActiveObjects();
        if (objects.Count == 0) return;

        float furthestPointMagnitude = 0f;

        foreach (var obj in objects) {
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null) {
                // Check all 8 corners of the bounding box to find the absolute furthest point from 0,0,0
                Vector3 min = r.bounds.min;
                Vector3 max = r.bounds.max;

                Vector3[] corners = new Vector3[] {
                    new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z),
                    new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z),
                    new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z)
                };

                foreach (Vector3 corner in corners) {
                    float mag = corner.magnitude;
                    if (mag > furthestPointMagnitude) furthestPointMagnitude = mag;
                }
            }
        }

        // 1. SET ADAPTIVE CONSTRAINTS
        // Min Radius: The "Skin" of the object plus 10% safety padding
        minRadius = furthestPointMagnitude * 1.1f; 
        
        // Max Radius: Exactly 3x the Min as per your requirement
        maxRadius = minRadius * 3.0f;

        // 2. SET INITIAL VIEWING DISTANCE
        // Start the student at a comfortable 1.8x distance
        _targetRadius = minRadius * 1.8f; 

        Debug.Log($"[Axiom Camera] Adaptive Sync: Min={minRadius:F2}, Max={maxRadius:F2}");
    }

    void Update() 
    {
        HandleInputs();
        ApplySphericalPhysics();
    }

    void HandleInputs() 
    {
        // 1. ORBIT (One Finger / Left Mouse)
        if (Input.GetMouseButton(0) && Input.touchCount <= 1) {
            _targetYaw += Input.GetAxis("Mouse X") * rotateSpeed;
            _targetPitch -= Input.GetAxis("Mouse Y") * rotateSpeed;
        }

        // 2. ZOOM (Scroll Wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _targetRadius -= scroll * zoomSpeed * 3f;

        // 3. PINCH TO ZOOM (Two Fingers)
        if (Input.touchCount == 2) {
            Touch t0 = Input.GetTouch(0); 
            Touch t1 = Input.GetTouch(1);
            
            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;
            
            float prevDist = (prevT0 - prevT1).magnitude;
            float curDist = (t0.position - t1.position).magnitude;
            
            _targetRadius -= (curDist - prevDist) * 0.02f * zoomSpeed;
        }

        // Apply Clamping BEFORE Lerp
        _targetPitch = Mathf.Clamp(_targetPitch, pitchLimits.x, pitchLimits.y);
        _targetRadius = Mathf.Clamp(_targetRadius, minRadius, maxRadius);
    }

    void ApplySphericalPhysics() 
    {
        // Smoothly interpolate values for that "Premium App" feel
        radius = Mathf.Lerp(radius, _targetRadius, Time.deltaTime * smoothing);
        yaw = Mathf.Lerp(yaw, _targetYaw, Time.deltaTime * smoothing);
        pitch = Mathf.Lerp(pitch, _targetPitch, Time.deltaTime * smoothing);

        // Standard Spherical-to-Cartesian conversion centered on (0,0,0)
        float x = radius * Mathf.Cos(pitch * Mathf.Deg2Rad) * Mathf.Sin(yaw * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(pitch * Mathf.Deg2Rad);
        float z = radius * Mathf.Cos(pitch * Mathf.Deg2Rad) * Mathf.Cos(yaw * Mathf.Deg2Rad);

        transform.position = new Vector3(x, y, z);
        transform.LookAt(Vector3.zero); // Locked on the Math Center
    }
}
