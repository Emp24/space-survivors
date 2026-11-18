using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Pooled floating damage numbers that pop up where an enemy got hit, drift upward and fade.
public class DamageNumbers : MonoBehaviour
{
    private const float Lifetime = 0.7f;
    private const float RiseSpeed = 1.6f;
    private const int PrewarmCount = 32;

    private class Number
    {
        public TextMeshPro text;
        public float age;
        public Vector3 velocity;
    }

    private readonly List<Number> active = new List<Number>();
    private readonly Stack<Number> pool = new Stack<Number>();

    void Awake()
    {
        for (int i = 0; i < PrewarmCount; i++) pool.Push(CreateNumber());
        GameEvents.OnEnemyDamaged += Spawn;
    }

    void OnDestroy()
    {
        GameEvents.OnEnemyDamaged -= Spawn;
    }

    private Number CreateNumber()
    {
        GameObject go = new GameObject("DamageNumber");
        go.transform.SetParent(transform, false);
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.sortingOrder = 100;
        tmp.rectTransform.sizeDelta = new Vector2(3f, 1f);
        go.SetActive(false);
        return new Number { text = tmp };
    }

    private void Spawn(Vector3 position, float amount)
    {
        Number number = pool.Count > 0 ? pool.Pop() : CreateNumber();
        number.age = 0f;
        number.velocity = new Vector3(Random.Range(-0.6f, 0.6f), RiseSpeed, 0f);

        // Bigger hits get bigger, warmer numbers.
        float weight = Mathf.Clamp01(amount / 50f);
        TextMeshPro tmp = number.text;
        tmp.text = Mathf.RoundToInt(amount).ToString();
        tmp.fontSize = Mathf.Lerp(4f, 7f, weight);
        tmp.color = Color.Lerp(new Color(1f, 0.95f, 0.6f), new Color(1f, 0.4f, 0.2f), weight);
        tmp.transform.position = position + new Vector3(Random.Range(-0.25f, 0.25f), 0.4f, -1f);
        tmp.transform.localScale = Vector3.one * 1.4f;
        tmp.gameObject.SetActive(true);
        active.Add(number);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = active.Count - 1; i >= 0; i--)
        {
            Number number = active[i];
            number.age += dt;
            if (number.age >= Lifetime)
            {
                number.text.gameObject.SetActive(false);
                active.RemoveAt(i);
                pool.Push(number);
                continue;
            }

            float t = number.age / Lifetime;
            Transform tr = number.text.transform;
            tr.position += number.velocity * dt;
            number.velocity *= 1f - 3f * dt;
            // Quick "pop" then settle, fade out over the second half.
            tr.localScale = Vector3.one * Mathf.Lerp(1.4f, 1f, Mathf.Clamp01(t * 5f));
            number.text.alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;
        }
    }
}
