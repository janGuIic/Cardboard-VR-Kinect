using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reset : MonoBehaviour {

    Animator anim;
    float restartTimer = 0;
    float restartDelay = 6f;
    bool juhu = false;


	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (GameObject.Find("U_Character_REF").transform.position.y < 2)
        {
            if (!juhu)
            {
                anim.SetTrigger("GameOver");
                juhu = true;              
            }
            restartTimer += Time.deltaTime;
            
            if (restartTimer >= restartDelay)
            {
                juhu = false;
                restartTimer = 0;             
                anim.ResetTrigger("GameOver");
                SceneManager.LoadScene("prepad");
            }
        }
        else
            restartTimer = 0;

    }
}
