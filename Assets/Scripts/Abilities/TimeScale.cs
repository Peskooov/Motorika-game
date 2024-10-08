using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour
{
    [SerializeField] private float slowDownFactor;
    public float SlowDownFactor => slowDownFactor;
    [SerializeField] private float slowDownDuration; 
    [SerializeField] private float radius; 
    private KeyCode slowDownKey = KeyCode.T; 

    private bool isSlowed = false;
    public bool IsSlowed => isSlowed;
    private float originalTimeScale;

    
    private Dictionary<GameObject, float> originalSpeeds = new Dictionary<GameObject, float>();

    private void Update()
    {
        if (Input.GetKeyDown(slowDownKey))
        {
            if (!isSlowed)
            {
                StartCoroutine(SlowDown());
            }
        }
    }

    private IEnumerator SlowDown()
    {
         isSlowed = true;
         originalTimeScale = Time.timeScale;
         Time.timeScale = slowDownFactor;
         Time.fixedDeltaTime = 0.02f * Time.timeScale; 

    
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
     

        yield return new WaitForSeconds(slowDownDuration);

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; 

        // reset original velocities
        foreach (var kvp in originalSpeeds)
        {
            var obj = kvp.Key;
            var rb = obj.GetComponent<Rigidbody>(); 
            if (rb != null)
            {
                rb.velocity = rb.velocity.normalized * kvp.Value; 
            }
        }

        isSlowed = false;
    }

    private void OnDrawGizmosSelected()
    {
      
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
