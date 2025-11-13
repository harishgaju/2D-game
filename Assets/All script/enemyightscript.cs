using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyightscript : MonoBehaviour
{
    Vector2 startpos;
    private Rigidbody2D Playerrb;
    private void Awake()
    {
        Playerrb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        startpos = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("obstacle"))
        {
            die();
        }
    }
    private void die()
    {
        StartCoroutine(Respawn(0.5f));
    }
    private IEnumerator Respawn(float duration)
    {
        Playerrb.simulated = false;
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(duration);
        transform.position = startpos;
        transform.localScale = new Vector3(1, 1, 1);
        Playerrb.simulated = true;
    }

}


