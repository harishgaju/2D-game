using UnityEngine;

public class respawncontroller : MonoBehaviour
{
    public static respawncontroller instance;
    public Transform respawnpoint;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dissolve dissolveScript = collision.GetComponent<dissolve>();

            if (dissolveScript != null)
            {
                dissolveScript.TriggerRespawn(respawnpoint.position);
            }
            else
            {
                collision.transform.position = respawnpoint.position;
            }
        }
    }
}
