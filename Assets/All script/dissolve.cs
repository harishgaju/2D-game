using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class dissolve : MonoBehaviour
{
    [SerializeField] private float dissolveTime = 0.75f;
    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;

    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        materials = new Material[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }
    public void TriggerRespawn(Vector3 respawnPosition)
    {
        StartCoroutine(RespawnSequence(respawnPosition));
    }

    private IEnumerator RespawnSequence(Vector3 respawnPosition)
    {
        yield return StartCoroutine(Vanish(true)); 
        transform.position = respawnPosition;       
        yield return StartCoroutine(Appear(true));
    }

    private IEnumerator Vanish(bool useDissolve)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(0, 1f, elapsedTime / dissolveTime);
            for (int i = 0; i < materials.Length; i++)
            {
                if (useDissolve)
                    materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
            }
            yield return null;
        }
    }

    private IEnumerator Appear(bool useDissolve)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(1f, 0f, elapsedTime / dissolveTime);
            for (int i = 0; i < materials.Length; i++)
            {
                if (useDissolve)
                    materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
            }
            yield return null;
        }
    }
}
