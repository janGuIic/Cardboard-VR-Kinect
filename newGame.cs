using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class newGame : MonoBehaviour
{

    Animator anim;
    float restartTimer1, restartTimer2 = 0;
    float restartDelay = 6f;
    bool alreadyTriggered1, alreadyTriggered2 = false;


    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player Dimp").GetComponent<PlayerHealth>().isDead)
        {
            if (!alreadyTriggered1)
            {
                anim.SetTrigger("GameOver");
                alreadyTriggered1 = true;
            }
            restartTimer1 += Time.deltaTime;

            if (restartTimer1 >= restartDelay)
            {
                alreadyTriggered1 = false;
                restartTimer1 = 0;
                anim.ResetTrigger("GameOver");
                SceneManager.LoadScene("boxing");
            }
        }
        else
            restartTimer1 = 0;

        if (GameObject.Find("Player Dimp").GetComponent<PlayerHealth>().isEnemyDead)
        {
            if (!alreadyTriggered2)
            {
                anim.SetTrigger("Victory");
                alreadyTriggered2 = true;
            }
            restartTimer2 += Time.deltaTime;

            if (restartTimer2 >= restartDelay)
            {
                alreadyTriggered2 = false;
                restartTimer2 = 0;
                anim.ResetTrigger("Victory");
                SceneManager.LoadScene("boxing");                
            }
        }
        else
            restartTimer2 = 0;

    }
}
