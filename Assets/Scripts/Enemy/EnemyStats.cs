using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float attackDamage;
    [SerializeField] private float sanityDamage;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageHealth(float damage)
    {
        currentHealth -= damage;
    }

    public void DestroyEnemy()
    {
        Destroy(this.gameObject.GetComponent<Rigidbody>());
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        StartCoroutine(DeleteEnemy(this.gameObject));
    }

    public IEnumerator DeleteEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(5f);
        Destroy(enemy);
    }

    public float GetCurrentHeath() => currentHealth;
    public float GetAttackDamage() => attackDamage;
    public float GetSanityDamage() => sanityDamage;
}
