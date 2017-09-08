using UnityEngine;
using System.Collections;


public class EnemyAttacked : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
    public int attackDamage = 10;               // The amount of health taken away per attack.
        
    GameObject player;
    PlayerHealth playerHealth;
    bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
    float timer;                                // Timer for counting up to the next attack.
    bool leftFistInRange, rightFistInRange, defensiveState;

    void Awake()
    {
        // Setting up the references.
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "left_fist" || other.tag == "right_fist")
        {
            playerInRange = true;
        }
        else if (other.tag == "enemy_left")
        {
            leftFistInRange = true;
        }

        else if (other.tag == "enemy_right")
        {
            rightFistInRange = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "left_fist" || other.tag == "right_fist")
        {
            playerInRange = false;
        }
        else if (other.tag == "enemy_left")
        {
            leftFistInRange = false;
        }

        else if (other.tag == "enemy_right")
        {
            rightFistInRange = false;
        }
    }


    void Update()
    {
        if (leftFistInRange && rightFistInRange)
        {
            defensiveState = true;
        }
        else
            defensiveState = false;
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
        //if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
        if (timer >= timeBetweenAttacks && playerInRange && !defensiveState)
        {
            // ... attack.
            Attack();
        }       
    }


    void Attack()
    {
        // Reset the timer.
        timer = 0f;

        // If the player has health to lose...
        if (playerHealth.currentHealth > 0)
        {
            // ... damage the player.
            playerHealth.GiveDamage(attackDamage);
        }
    }
}