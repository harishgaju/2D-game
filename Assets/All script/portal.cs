using System.Collections;
using UnityEngine;

public class portal : MonoBehaviour
{
    public Transform destination;
    private GameObject Player;
    private Animation Animation;
    private Rigidbody2D Playerrb;


    audiomanager audiomanager;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Animation = Player.GetComponent<Animation>();
        Playerrb = Player.GetComponent<Rigidbody2D>();
        audiomanager = GameObject.FindGameObjectWithTag("audio").GetComponent<audiomanager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Vector2.Distance(Player.transform.position, transform.position) > 0.3f)
            {
                StartCoroutine(PortalIn());
            }
        }
    }
    private IEnumerator PortalIn()
    {
        audiomanager.playSFX(audiomanager.portalA);
        Playerrb.simulated = false;
        //if (Animation != null) Animation.Play("portalA");

        yield return new WaitForSeconds(0.5f);

        Player.transform.position = destination.position;
        Playerrb.linearVelocity = Vector2.zero;

        //if (Animation != null) Animation.Play("portalB");
        audiomanager.playSFX(audiomanager.portaloB);

        yield return new WaitForSeconds(0.5f);
        Playerrb.simulated = true;
    }
    private IEnumerator MoveIntoPortal()
    {
        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            Player.transform.position = Vector2.MoveTowards(Player.transform.position, transform.position, 4 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}   
