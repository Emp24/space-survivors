using UnityEngine;

// Trauma-based screen shake. The player re-centres the camera every Update, so the offset
// is re-applied in LateUpdate each frame rather than accumulated into the camera position.
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float maxOffset = 0.45f;
    [SerializeField] private float maxRoll = 2.5f;
    [SerializeField] private float traumaDecay = 1.6f;
    [SerializeField] private float frequency = 22f;

    private float trauma;
    private Camera cam;
    private Quaternion baseRotation = Quaternion.identity;

    void Awake()
    {
        cam = Camera.main;
        if (cam != null) baseRotation = cam.transform.rotation;
        GameEvents.OnPlayerDamaged += HandlePlayerDamaged;
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    void OnDestroy()
    {
        GameEvents.OnPlayerDamaged -= HandlePlayerDamaged;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
    }

    public void AddTrauma(float amount) => trauma = Mathf.Clamp01(trauma + amount);

    private void HandlePlayerDamaged(float damage) => AddTrauma(Mathf.Lerp(0.25f, 0.6f, damage / 30f));
    private void HandlePlayerDied() => trauma = 0f;

    void LateUpdate()
    {
        if (cam == null) return;
        if (trauma <= 0f)
        {
            cam.transform.rotation = baseRotation;
            return;
        }

        // Squared trauma feels better: small hits are subtle, big hits are violent.
        float shake = trauma * trauma;
        float t = Time.time * frequency;
        float x = (Mathf.PerlinNoise(t, 0f) * 2f - 1f) * maxOffset * shake;
        float y = (Mathf.PerlinNoise(0f, t) * 2f - 1f) * maxOffset * shake;
        float roll = (Mathf.PerlinNoise(t, t) * 2f - 1f) * maxRoll * shake;

        cam.transform.position += new Vector3(x, y, 0f);
        cam.transform.rotation = baseRotation * Quaternion.Euler(0f, 0f, roll);
        trauma = Mathf.Max(0f, trauma - traumaDecay * Time.deltaTime);
    }
}
