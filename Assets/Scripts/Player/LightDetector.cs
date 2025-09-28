using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetector : MonoBehaviour
{
    public static LightDetector Instance;

    [SerializeField] private int lightsInside = 0; // Number of light sources currently inside
    [SerializeField] private LayerMask ignoreLayers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (lightsInside > 0)
            PlayerStats.Instance.HealSanityOverTime();
        else
            PlayerStats.Instance.DamageSanityOverTime();
    }

    public void ResetLights()
    {
        lightsInside = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            Light light = other.GetComponent<Light>();
            Vector3 direction = (other.transform.position - transform.position).normalized;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, light.range, ~ignoreLayers))
            {
                //Debug.DrawRay(transform.position, direction, Color.cyan, 5f);
                //Debug.Log("Light detector detected: " + hit.collider.name);
                // Only count it if the ray directly hits the light source
                if (hit.collider == other)
                {
                    lightsInside++;
                    //Debug.Log($"Entered Light, count = {lightsInside}");
                }
                else
                {
                    return;
                    //Debug.Log("Light is blocked by something! " + hit.collider.name);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            lightsInside = Mathf.Max(0, lightsInside - 1); // prevent going negative
            //Debug.Log($"Exited Light, count = {lightsInside}");
        }
    }
}
