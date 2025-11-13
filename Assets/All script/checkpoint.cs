using System;
using System.Collections;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    audiomanager audiomanager;
    public BoxCollider2D trigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            respawncontroller.instance.respawnpoint = transform;
            trigger.enabled = false;

            audiomanager = GameObject.FindGameObjectWithTag("audio").GetComponent<audiomanager>();
            audiomanager.playSFX(audiomanager.checkpoint);

            // ✅ Reset all falling platforms
            fallingplatform.ResetAllPlatforms();
        }
    }


    //audiomanager audiomanager;
    //public BoxCollider2D trigger;
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        respawncontroller.instance.respawnpoint = transform;
    //        trigger.enabled = false;
    //        audiomanager = GameObject.FindGameObjectWithTag("audio").GetComponent<audiomanager>();
    //        audiomanager.playSFX(audiomanager.checkpoint);
    //    }
    //}
}
