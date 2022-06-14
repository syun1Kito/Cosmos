using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    //[SerializeField] GameObject pauseMenu;
    [SerializeField] Animator animator;
    [SerializeField] GameObject panel;
    [SerializeField] SoundCountroller soundCountroller;

    float axis;
    bool[] inputcheck = new bool[] {false,false,false,false};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("pause"))
        {
            soundCountroller.PlaySE(SoundCountroller.Sound.enter);
            //Debug.Log(!animator.GetBool("pause"));
            if (!animator.GetBool("pause"))
            {
               animator.SetInteger("state", 0);
            }
            animator.SetBool("pause", !animator.GetBool("pause"));

            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            
        }

        if (animator.GetBool("pause"))
        {


            //animator.SetInteger("state", (animator.GetInteger("state") + 1) % 2);
            /* if (Input.GetAxis("AllHorizontal")!=0 && Input.GetAxis("AllHorizontal")!=axis)
             {
                soundCountroller.PlaySE(SoundCountroller.Sound.select);
                axis = Input.GetAxis("AllHorizontal");
                animator.SetInteger("state", (animator.GetInteger("state") + 1) % 2);
             }
            axis = Input.GetAxis("AllHorizontal");
            */

            for (int i = 0; i < PlayerSelectController.GetPlayerNumber(); i++)
            {
                if (!inputcheck[i] && Input.GetAxis((i+1)+"P_Horizontal")!=0)
                {
                    soundCountroller.PlaySE(SoundCountroller.Sound.select);
                    animator.SetInteger("state", (animator.GetInteger("state") + 1) % 2);
                    inputcheck[i] = true;
                }
                else if(Input.GetAxis((i+1) + "P_Horizontal") == 0)
                {
                    inputcheck[i] = false;
                }


            }

            
        }

        if (Input.GetButtonDown("select") && animator.GetBool("pause"))
        {
            soundCountroller.PlaySE(SoundCountroller.Sound.enter);
            switch (animator.GetInteger("state"))
            {
                case 0:
                    animator.SetBool("pause",false);
                    //animator.SetInteger("state", 0);
                    Time.timeScale = 1;
                    break;
                case 1:
                    SceneManager.LoadScene("Title");
                    Time.timeScale = 1;
                    break;
            }
        }
    }
}
