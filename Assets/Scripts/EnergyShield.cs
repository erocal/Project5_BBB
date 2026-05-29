using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyShield : MonoBehaviour
{
    [SerializeField] private float shakeStrength = 0.03f;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float rippleCooldown = 0.4f;
    [SerializeField] private GameObject sparkVFX;

    [Header("Collision")]
    [SerializeField] private float raycastOffset = 0.05f;
    [SerializeField] private float raycastDistance = 0.2f;

    private Material material;
    private float rippleTime = 100.0f;
    private Coroutine shakeRoutine;
    private Vector3 originalPosition;
    private Collider shieldCollider;
    private AudioSource audioSource;

    private void Awake()
    {
        shieldCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    private void OnCollisionEnter(Collision collision)
    {

        audioSource.Stop();
        audioSource.Play();

        if (rippleTime < rippleCooldown)
        {
            return;
        }

        if (collision.contactCount <= 0)
        {
            return;
        }

        ContactPoint contact = collision.GetContact(0);

        RaycastHit hit;
        if (TryGetHitFromContact(contact, out hit))
        {
            PlayHitEffect(hit.point, hit.normal, hit.textureCoord);
        }
        else
        {
            // 如果 Raycast 沒拿到 UV，至少仍然播放震動與特效
            PlayHitEffect(contact.point, contact.normal, new Vector2(0.5f, 0.5f));
        }
    }

    private bool TryGetHitFromContact(ContactPoint contact, out RaycastHit hit)
    {
        Vector3 origin = contact.point + contact.normal * raycastOffset;
        Vector3 direction = -contact.normal;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance))
        {
            if (hit.collider == shieldCollider)
            {
                return true;
            }
        }

        // 有些碰撞法線方向可能相反，所以反方向再試一次
        origin = contact.point - contact.normal * raycastOffset;
        direction = contact.normal;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance))
        {
            if (hit.collider == shieldCollider)
            {
                return true;
            }
        }

        return false;
    }

    private void PlayHitEffect(Vector3 hitPoint, Vector3 hitNormal, Vector2 textureCoord)
    {
        material.SetVector("_RippleOrigin", textureCoord);
        rippleTime = material.GetFloat("_RippleThickness") * -2.0f;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            transform.position = originalPosition;
        }

        originalPosition = transform.position;
        shakeRoutine = StartCoroutine(Shake(hitPoint, hitNormal));
    }

    private void Update()
    {
        rippleTime += Time.deltaTime;
        material.SetFloat("_RippleTime", rippleTime);
    }

    private IEnumerator Shake(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (sparkVFX != null && !sparkVFX.activeInHierarchy)
        {
            sparkVFX.transform.position = hitPoint;
            sparkVFX.transform.rotation = Quaternion.LookRotation(Vector3.up, hitNormal);
            sparkVFX.SetActive(true);
        }

        for (float t = 0.0f; t < shakeDuration; t += Time.deltaTime)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeStrength;
            yield return null;
        }

        transform.position = originalPosition;
    }
}