using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxSanity;
    [SerializeField] private float _sanityHealRate;
    [SerializeField] private float _sanityDamageRate;
    [SerializeField] private float _sanityDamageDelay;   // seconds before damage starts
    [SerializeField] private float _currentSanity;

    [SerializeField] private float healSanityTimer = 0f;
    [SerializeField] private float damageSanityTimer = 0f;
    [SerializeField] private float darkTimer = 0f; // how long player has been in darkness
    private const float tickInterval = 1f; // seconds per tick

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
        _currentSanity = _maxSanity;
    }

    /*private void Update()
    {
        SanityChecker();
    }*/

    public void DamageHealth(float damage)
    {
        _currentHealth = Mathf.Max(_currentHealth -= damage, 0);
    }

    public void DamageSanity(float damage)
    {
        _currentSanity = Mathf.Max(_currentSanity -= damage, 0);
    }

    public void DamageSanityOverTime()
    {
        // Count how long we've been in darkness
        darkTimer += Time.deltaTime;

        // Only start damaging if we passed the grace period
        if (darkTimer >= _sanityDamageDelay)
        {
            damageSanityTimer += Time.deltaTime;
            if (damageSanityTimer >= tickInterval)
            {
                if (_currentSanity > 0)
                {
                    _currentSanity -= _maxSanity * (_sanityDamageRate / 100f);
                    _currentSanity = Mathf.Max(_currentSanity, 0);
                }
                damageSanityTimer = 0f;
            }
        }

        healSanityTimer = 0f; // Reset heal so it doesn't trigger after switching
    }

    public void HealHealth(float healPercent)
    {
        float healAmount = _maxHealth * (healPercent / 100f);
        _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
        //Debug.Log($"Healed {healAmount} HP. Current Health: {currentHealth}/{maxHealth}");
    }

    public void HealSanity(float healPercent)
    {
        float healAmount = _maxSanity * (healPercent / 100f);
        _currentSanity = Mathf.Min(_currentSanity + healAmount, _maxSanity);
        //Debug.Log($"Healed {healAmount} Sanity. Current Sanity: {currentSanity}/{maxSanity}");
    }

    public void HealSanityOverTime()
    {
        // Reset darkness tracking
        darkTimer = 0f;
        damageSanityTimer = 0f;

        healSanityTimer += Time.deltaTime;
        if (healSanityTimer >= tickInterval)
        {
            if (_currentSanity < _maxSanity)
            {
                _currentSanity += _maxSanity * (_sanityHealRate / 100f);
                _currentSanity = Mathf.Min(_currentSanity, _maxSanity);
            }
            healSanityTimer = 0f;
        }
    }

    public void ResetTimers()
    {
        healSanityTimer = 0f;
        damageSanityTimer = 0f;
        darkTimer = 0f;
    }

    public void ResetPlayerStats()
    {
        _currentHealth = maxHealth;
        _currentSanity = maxSanity;
        ResetTimers();
    }

    /*public void SanityChecker()
    {
        if (_currentSanity > _maxSanity * 60 / 100)
        {
            return;
        }

        if (_currentSanity < _maxSanity * 10 / 100)
        {
            // play sfx paranoid_3
            return;
        }
        else if (_currentSanity <= _maxSanity * 30 / 100)
        {
            // play sfx paranoid_2
        }
        else if (_currentSanity <= _maxSanity * 60 / 100)
        {
            // play sfx paranoid_1
        }
        else
        {

        }
    }*/

    public float currentHealth => _currentHealth;

    public float currentSanity => _currentSanity;

    public float maxHealth => _maxHealth;

    public float maxSanity => _maxSanity;

    public void SetHealth(float value)
    {
        _currentHealth = value;
    }

    public void SetSanity(float value)
    {
        _currentSanity = value;
    }
}
