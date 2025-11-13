using UnityEngine;
using UnityEngine.SceneManagement;


public class NextLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadSceneAsync("NextLevel"); 
        }
    }
}
