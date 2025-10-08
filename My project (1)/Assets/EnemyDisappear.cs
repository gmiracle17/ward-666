using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class EnemyDisappear : MonoBehaviour
{
    [Header("Settings")]
    public float lookTimeToDisappear = 3f;   // How long player must look

    [Header("Audio")]
    public AudioClip disappearClip;

    [Header("Fade Settings")]
    public bool fadeOnDisappear = false;
    public float fadeTime = 0.3f;

    private Transform player;
    private Camera playerCamera;
    private Renderer[] renderers;
    private Collider[] colliders;
    private AudioSource audioSource;

    private bool hasDisappeared = false;
    private float lookTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerCamera = Camera.main;
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        audioSource = GetComponent<AudioSource>();

        if (player == null || playerCamera == null)
            Debug.LogWarning("EnemyDisappear: Player or Camera not found.");
    }

    void Update()
    {
        if (hasDisappeared) return;

        if (IsPlayerLookingAtEnemy())
        {
            lookTimer += Time.deltaTime;
            if (lookTimer >= lookTimeToDisappear)
            {
                hasDisappeared = true;
                StartCoroutine(Disappear());
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }

    bool IsPlayerLookingAtEnemy()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider.transform.IsChildOf(transform);
        }
        return false;
    }

    IEnumerator Disappear()
    {
        PlayClip(disappearClip);

        if (fadeOnDisappear)
            yield return StartCoroutine(FadeOutCoroutine());

        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers) r.enabled = visible;
        foreach (var c in colliders) c.enabled = visible;
    }

    void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    IEnumerator FadeOutCoroutine()
    {
        float t = 0f;
        Material[] mats = GetMaterials();
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / fadeTime);
            SetMaterialAlpha(mats, a);
            yield return null;
        }
        SetMaterialAlpha(mats, 0f);
    }

    Material[] GetMaterials()
    {
        var mats = new System.Collections.Generic.List<Material>();
        foreach (var r in renderers)
            mats.AddRange(r.materials);
        return mats.ToArray();
    }

    void SetMaterialAlpha(Material[] materials, float alpha)
    {
        foreach (var m in materials)
        {
            if (m.HasProperty("_Color"))
            {
                Color c = m.color;
                c.a = alpha;
                m.color = c;
            }
        }
    }
}
