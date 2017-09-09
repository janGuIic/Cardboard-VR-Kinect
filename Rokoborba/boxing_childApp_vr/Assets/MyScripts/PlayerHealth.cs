using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int startingHealth = 100;                            // The amount of health the player starts the game with.
    public int currentHealth;                                   // The current health the player has.
    public Slider healthSlider;
    public int currentEnemyHealth;                                   // The current health the player has.
    public Slider enemyHealthSlider;                                 // Reference to the UI's health bar.
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


   // Animator anim;                                              // Reference to the Animator component.
    AudioSource playerAudio;                                    // Reference to the AudioSource component.
    MyAvatarControllerRokoborba playerMovement;                              // Reference to the player's movement.
    public bool isDead;
    public bool isEnemyDead;    // Whether the player is dead.
    bool damaged;                                              // True when the player gets damaged.
      
    void Start()
    {
       
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<MyAvatarControllerRokoborba>();        

        // Set the initial health of the player.
        currentHealth = startingHealth;
        currentEnemyHealth = startingHealth;
    }


    void Update()
    {     
        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        damaged = false;
    }


    public void TakeDamage(int amount)
    {

        damaged = true;
        currentHealth -= amount;
        healthSlider.value = currentHealth;
         playerAudio.Play();

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    public void GiveDamage(int amount)
    {
        damaged = true;
        currentEnemyHealth -= amount;
        enemyHealthSlider.value = currentEnemyHealth;
        playerAudio.Play();
        if (currentEnemyHealth <= 0 && !isEnemyDead)
        {
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        isEnemyDead = true;        
        playerAudio.clip = deathClip;
        playerAudio.Play();

        // Turn off the movement and shooting scripts.
        //playerMovement.enabled = false;
    }

    void Death()
    {
        isDead = true;        
        playerAudio.clip = deathClip;
        playerAudio.Play();   

        // Turn off the movement and shooting scripts.
        //playerMovement.enabled = false;
    }


}
