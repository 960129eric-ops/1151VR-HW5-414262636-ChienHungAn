using System.Collections;
using UnityEngine;

public class PokeballCapture : MonoBehaviour
{
    public GameObject captureEffectPrefab;
    public float shrinkDuration = 0.4f;
    public float shakeDuration = 1.2f;

    private bool hasHit = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 6f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit && collision.gameObject.CompareTag("Pokemon"))
        {
            StartCoroutine(CaptureRoutine(collision.gameObject));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasHit && other.CompareTag("Pokemon"))
        {
            StartCoroutine(CaptureRoutine(other.gameObject));
        }
    }

    IEnumerator CaptureRoutine(GameObject pokemon)
    {
        hasHit = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (captureEffectPrefab != null)
        {
            GameObject fx = Instantiate(captureEffectPrefab, pokemon.transform.position, Quaternion.identity);
            Destroy(fx, 3f);
        }

        Vector3 startScale = pokemon.transform.localScale;
        Vector3 startPos = pokemon.transform.position;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            pokemon.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            pokemon.transform.position = Vector3.Lerp(startPos, transform.position, t);
            yield return null;
        }

        pokemon.SetActive(false);

        float shakeElapsed = 0f;
        Quaternion baseRot = transform.rotation;
        while (shakeElapsed < shakeDuration)
        {
            shakeElapsed += Time.deltaTime;
            float angle = Mathf.Sin(shakeElapsed * 25f) * 20f;
            transform.rotation = baseRot * Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        if (captureEffectPrefab != null)
        {
            GameObject successFx = Instantiate(captureEffectPrefab, transform.position, Quaternion.identity);
            Destroy(successFx, 3f);
        }

        Destroy(gameObject, 0.5f);
    }
}