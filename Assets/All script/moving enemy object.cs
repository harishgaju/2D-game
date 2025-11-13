using UnityEngine;

public class movingenemyobject : MonoBehaviour
{
    public Transform posA, posB; 
    public int speed = 5; 
    private bool movingToB = true; 
    private Vector2 targetPosition; 

    private void Start()
    {
      
        targetPosition = posB.position;
    }
    private void Update()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (movingToB)
            {
                targetPosition = posA.position;
            }
            else
            {
                targetPosition = posB.position;
            }

            movingToB = !movingToB; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
            collision.transform.localScale = Vector3.one; 
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            collision.transform.SetParent(null);
        }
    }
    private void OnDrawGizmos()
    {
        if (posA != null && posB != null)
        {
           
            Gizmos.color = Color.green;
            Gizmos.DrawLine(posA.position, posB.position);

            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(posA.position, 0.2f); 
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(posB.position, 0.2f); 
        }
    }
    //    public Vector2 startDirection = new Vector2(1, 0); 
    //    public int speed = 5; 
    //    public float distance = 5f; 
    //    private Vector2 startingPosition; 
    //    private bool movingForward = true; 
    //    private Vector2 targetPosition; 

    //    private void Start()
    //    {
    //        startingPosition = transform.position;
    //        targetPosition = startingPosition + (startDirection.normalized * distance); 
    //    }
    //    private void Update()
    //    {
    //        if (movingForward)
    //        {
    //            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    //            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
    //            {
    //                movingForward = false;
    //                targetPosition = startingPosition; 
    //            }
    //        }
    //        else
    //        {
    //            transform.position = Vector2.MoveTowards(transform.position, startingPosition, speed * Time.deltaTime);
    //            if (Vector2.Distance(transform.position, startingPosition) < 0.1f)
    //            {
    //                movingForward = true;
    //                targetPosition = startingPosition + (startDirection.normalized * distance); 
    //            }
    //        }
    //    }

    //    private void OnTriggerEnter2D(Collider2D collision)
    //    {
    //        if (collision.CompareTag("Player"))
    //        {
    //            collision.transform.SetParent(transform);
    //            collision.transform.localScale = Vector3.one;
    //        }
    //    }

    //    private void OnTriggerExit2D(Collider2D collision)
    //    {
    //        if (collision.CompareTag("Player"))
    //        {
    //            collision.transform.SetParent(null);
    //        }
    //    }
    //    private void OnDrawGizmos()
    //    {
    //        if (startDirection != null)
    //        {
    //            Gizmos.color = Color.green;
    //            Gizmos.DrawLine(transform.position, transform.position + (Vector3)startDirection.normalized * distance); 
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(transform.position + (Vector3)startDirection.normalized * distance, 0.2f); 
    //            Gizmos.color = Color.blue;
    //            Gizmos.DrawSphere(transform.position, 0.2f); 
    //        }
    //    }
}
