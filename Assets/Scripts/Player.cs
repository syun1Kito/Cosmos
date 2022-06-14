using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Player : MonoBehaviour
{

    [SerializeField] float speed = 3;
    [SerializeField] int HP = 3;

    [SerializeField] Animator animator;
    [SerializeField] int playerID;
    [SerializeField] GameObject model;

    [SerializeField] GameObject[] HPgauge;
    [SerializeField] Field field;
    [SerializeField] Vector3 startpos;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Slider slider;

    [SerializeField] SoundCountroller soundCountroller;
    // Animation animation;

    int x_pos = 0;
    int z_pos = 0;
    int x_dir;
    int z_dir;

    int attackType;
    float waittime = 0.05f;
    float spAttackTime = 8.0f;
    float passedtime = 0.0f;
    bool isAttacking = false;
    bool damaged = false;
    bool invincible = false;//無敵
    bool spAttackable = false;
    bool deadflag = false;
    bool innerAttack = false;
   //static bool isPlaying;





    private Vector3 Player_pos;

    /*public Player()
    {
        //GameObject  = Instantiate(player, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        //player = GameObject.Find(playerID + "P");
        // this.playerID = playerID;
        // this.startpos = startpos;
        // this.field = field;
    }*/

    void Start()
    {
        switch (playerID)
        {
            case 1:
                x_dir = 1;
                z_dir = -1;                  
                break;

            case 2:
                x_dir = -1;
                z_dir = -1;
                break;

            case 3:
                x_dir = 1;
                z_dir = 1;
                break;

            case 4:
                x_dir = -1;
                z_dir = 1;
                break;
        }

        attackType = PlayerSelectController.GetAttackType(playerID);
        //attackType = PlayerController.GetPlayerAttackType(playerID);
        transform.position = startpos;
        Player_pos = transform.position;

    }

    void Update()
    {

        Attack();

        Dead();

        //Debug.Log(playerID + "P" + innerAttack);
       
    }

    public int GetHP() { return HP; }

    public static void ChangeisPlaying()
    {
        PlayerController.SetisPlayingTrue();

    }

    public void GetFieldPos()
    {

        x_pos = Mathf.CeilToInt(transform.position.x) + 5;
        z_pos = Mathf.CeilToInt(transform.position.z) + 5;
        //return new Vector3(Mathf.CeilToInt(pos.x) + 5, 0, Mathf.CeilToInt(pos.z) + 5);
    }

    public void Move()
    {
        //  transform.position += new Vector3(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, 0);
        //  transform.position += new Vector3(0, 0,Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);
        if (!damaged && !isAttacking && PlayerController.GetisPlaying())
        {



            Vector3 direction = new Vector3(Input.GetAxis(playerID + "P_Horizontal"), 0, Input.GetAxis(playerID + "P_Vertical")).normalized;

            if (direction != Vector3.zero)
            {
                animator.SetBool("walk", true);

                agent.Move(direction * (Time.deltaTime * speed));
            }
            else
            {
                animator.SetBool("walk", false);
            }

            Vector3 diff = transform.position - Player_pos;

            if (diff.magnitude > 0.05f)
            {
                transform.rotation = Quaternion.LookRotation(diff);

                if (transform.rotation.eulerAngles.y > 10 && transform.rotation.eulerAngles.y < 170) { x_dir = 1; }
                else if (transform.rotation.eulerAngles.y > 190 && transform.rotation.eulerAngles.y < 350) { x_dir = -1; }
                else { x_dir = 0; }

                if (transform.rotation.eulerAngles.y < 80 || transform.rotation.eulerAngles.y > 280) { z_dir = 1; }
                else if (transform.rotation.eulerAngles.y > 100 && transform.rotation.eulerAngles.y < 260) { z_dir = -1; }
                else { z_dir = 0; }

            }

            Player_pos = transform.position;


        }
    }


    public void Attack()
    {
        if (!damaged && !isAttacking && Time.timeScale == 1 && PlayerController.GetisPlaying())
        {


            if (Input.GetButtonDown(playerID + "P_normal"))
            {
                NormalShot();
            }
            else if (Input.GetButtonDown(playerID + "P_special1") && spAttackable)
            {
                SpecialShot();
                passedtime = 0.0f;
                spAttackable = false;
            }
        }
    }

    public void NormalShot()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        StartCoroutine(Straight(Field.FieldState.none + playerID, Field.FieldState.none + playerID + 4));
    }

    public void SpecialShot()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        switch (attackType)
        {
            case 1:
                StartCoroutine(Cross(Field.FieldState.none + playerID, Field.FieldState.none + playerID + 4));
                break;
            case 2:
                StartCoroutine(Twin(Field.FieldState.none + playerID, Field.FieldState.none + playerID + 4));
                break;
            case 3:
                StartCoroutine(Wall(Field.FieldState.none + playerID, Field.FieldState.none + playerID + 4));
                break;
            case 4:
                StartCoroutine(Front(Field.FieldState.none + playerID, Field.FieldState.none + playerID + 4));
                break;
        }

    }

    public void GaugeUpdate()
    {
        if (PlayerController.GetisPlaying())
        {


            if (passedtime < spAttackTime)
            {
                passedtime += Time.deltaTime;
                slider.value = passedtime / spAttackTime * 100;
            }
            else
            {
                if (!spAttackable) { spAttackable = true; }
            }
        }
    }

    public void Damage()
    {
        if (HP > 0)
        {

            invincible = true;
            damaged = true;
           
            animator.SetTrigger("damage");
            soundCountroller.PlaySE(SoundCountroller.Sound.damage);
            HP--;
            HPgauge[HP].SetActive(false);
            if (HP != 0)
            {
                StartCoroutine(DamagedWait());
            }

        }
    }

    public void Dead()
    {
        if (HP <= 0 && !deadflag)
        {
            deadflag = true;
            field.SetDeathZone(playerID);
            PlayerController.SetPlayerRank(playerID);
            StartCoroutine(DeadWait());
        }
    }

    /* private IEnumerator Straight(Field.FieldState fieldState1,Field.FieldState fieldState2)
     {
         int count = 0;
         int x_pos_temp = x_pos;
         int z_pos_temp = z_pos;
         int x_dir_temp = x_dir;
         int z_dir_temp = z_dir;



         while (true)
         {
             count++;

             if (x_pos_temp + count * x_dir_temp > 10 || x_pos_temp + count * x_dir_temp < 1 || z_pos_temp + count * z_dir_temp > 10 || z_pos_temp + count * z_dir_temp < 1)
             {
                count = 0;
                break;
             }

             field.FieldChange(x_pos_temp + count * x_dir_temp, z_pos_temp + count * z_dir_temp, fieldState1);

             yield return new WaitForSeconds(waittime);
         }

         while (true)
         {
             count++;

             if (x_pos_temp + count * x_dir_temp > 10 || x_pos_temp + count * x_dir_temp < 1 || z_pos_temp + count * z_dir_temp > 10 || z_pos_temp + count * z_dir_temp < 1)
             {
                 count = 0;
                 break;
             }

             field.FieldChange(x_pos_temp + count * x_dir_temp, z_pos_temp + count * z_dir_temp, fieldState2);

             yield return new WaitForSeconds(waittime);
         }

         while (true)
         {
             count++;

             if (x_pos_temp + count * x_dir_temp > 10 || x_pos_temp + count * x_dir_temp < 1 || z_pos_temp + count * z_dir_temp > 10 || z_pos_temp + count * z_dir_temp < 1)
             {
                 yield break;
             }

             field.FieldChange(x_pos_temp + count * x_dir_temp, z_pos_temp + count * z_dir_temp, Field.FieldState.none);

             yield return new WaitForSeconds(waittime);
         }
     }
     */

    public IEnumerator Straight(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {
        int count = 0;
        int x_pos_temp = x_pos;
        int z_pos_temp = z_pos;
        int x_dir_temp = x_dir;
        int z_dir_temp = z_dir;

        bool check1 = false;

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count * x_dir_temp;
            int z1 = z_pos_temp + count * z_dir_temp;


            if (x1 >= 1 && x1 <= 10 && z1 >= 1 && z1 <= 10)
            {
                field.FieldChange(x1, z1, fieldState1);
            }
            else { check1 = true; }

            if (check1)
            {
                check1 = false;
                count = 0;
                break;
            }

            yield return new WaitForSeconds(waittime);
        }

        soundCountroller.PlaySE(SoundCountroller.Sound.select + playerID);

        while (true)
        {
            count++;
     
            int x1 = x_pos_temp + count * x_dir_temp;
            int z1 = z_pos_temp + count * z_dir_temp;

            if (x1 >= 1 && x1 <= 10 && z1 >= 1 && z1 <= 10)
            {
                field.FieldChange(x1, z1, fieldState2);
            }
            else { check1 = true; }

            if (check1)
            {
                check1 = false;
                count = 0;
                break;
            }

            yield return new WaitForSeconds(waittime);
        }

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count * x_dir_temp;
            int z1 = z_pos_temp + count * z_dir_temp;



            if (x1 >= 1 && x1 <= 10 && z1 >= 1 && z1 <= 10)
            {
                field.FieldChange(x1, z1, Field.FieldState.none);
            }
            else { check1 = true; }

            if (check1)
            {
                isAttacking = false;
                yield break;
            }

            yield return new WaitForSeconds(waittime);
        }

    }

    public IEnumerator Cross(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {
        int count = 0;
        int x_pos_temp = x_pos;
        int z_pos_temp = z_pos;
        int x_dir_temp = x_dir;
        int z_dir_temp = z_dir;

        bool check1, check2, check3, check4;
        check1 = check2 = check3 = check4 = false;

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count;
            int x2 = x_pos_temp - count;

            int z1 = z_pos_temp + count;
            int z2 = z_pos_temp - count;


            if (x_dir_temp * z_dir_temp == 0)
            {
                if (z1 <= 10)
                {
                    field.FieldChange(x_pos_temp, z1, fieldState1);
                }
                else { check1 = true; }

                if (x1 <= 10)
                {
                    field.FieldChange(x1, z_pos_temp, fieldState1);
                }
                else { check2 = true; }

                if (z2 >= 1)
                {
                    field.FieldChange(x_pos_temp, z2, fieldState1);
                }
                else { check3 = true; }

                if (x2 >= 1)
                {
                    field.FieldChange(x2, z_pos_temp, fieldState1);
                }
                else { check4 = true; }
            }
            else
            {
                if (x1 <= 10 && z1 <= 10)
                {
                    field.FieldChange(x1, z1, fieldState1);
                }
                else { check1 = true; }

                if (x1 <= 10 && z2 >= 1)
                {
                    field.FieldChange(x1, z2, fieldState1);
                }
                else { check2 = true; }

                if (x2 >= 1 && z2 >= 1)
                {
                    field.FieldChange(x2, z2, fieldState1);
                }
                else { check3 = true; }

                if (x2 >= 1 && z1 <= 10)
                {
                    field.FieldChange(x2, z1, fieldState1);
                }
                else { check4 = true; }
            }


            if (check1 && check2 && check3 && check4)
            {
                check1 = check2 = check3 = check4 = false;
                count = 0;
                break;
            }

            yield return new WaitForSeconds(waittime);
        }

        soundCountroller.PlaySE(SoundCountroller.Sound.select + playerID);

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count;
            int x2 = x_pos_temp - count;

            int z1 = z_pos_temp + count;
            int z2 = z_pos_temp - count;


            if (x_dir_temp * z_dir_temp == 0)
            {
                if (z1 <= 10)
                {
                    field.FieldChange(x_pos_temp, z1, fieldState2);
                }
                else { check1 = true; }

                if (x1 <= 10)
                {
                    field.FieldChange(x1, z_pos_temp, fieldState2);
                }
                else { check2 = true; }

                if (z2 >= 1)
                {
                    field.FieldChange(x_pos_temp, z2, fieldState2);
                }
                else { check3 = true; }

                if (x2 >= 1)
                {
                    field.FieldChange(x2, z_pos_temp, fieldState2);
                }
                else { check4 = true; }
            }
            else
            {
                if (x1 <= 10 && z1 <= 10)
                {
                    field.FieldChange(x1, z1, fieldState2);
                }
                else { check1 = true; }

                if (x1 <= 10 && z2 >= 1)
                {
                    field.FieldChange(x1, z2, fieldState2);
                }
                else { check2 = true; }

                if (x2 >= 1 && z2 >= 1)
                {
                    field.FieldChange(x2, z2, fieldState2);
                }
                else { check3 = true; }

                if (x2 >= 1 && z1 <= 10)
                {
                    field.FieldChange(x2, z1, fieldState2);
                }
                else { check4 = true; }
            }


            if (check1 && check2 && check3 && check4)
            {
                check1 = check2 = check3 = check4 = false;
                count = 0;
                break;
            }

            yield return new WaitForSeconds(waittime);
        }

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count;
            int x2 = x_pos_temp - count;

            int z1 = z_pos_temp + count;
            int z2 = z_pos_temp - count;


            if (x_dir_temp * z_dir_temp == 0)
            {
                if (z1 <= 10)
                {
                    field.FieldChange(x_pos_temp, z1, Field.FieldState.none);
                }
                else { check1 = true; }

                if (x1 <= 10)
                {
                    field.FieldChange(x1, z_pos_temp, Field.FieldState.none);
                }
                else { check2 = true; }

                if (z2 >= 1)
                {
                    field.FieldChange(x_pos_temp, z2, Field.FieldState.none);
                }
                else { check3 = true; }

                if (x2 >= 1)
                {
                    field.FieldChange(x2, z_pos_temp, Field.FieldState.none);
                }
                else { check4 = true; }
            }
            else
            {
                if (x1 <= 10 && z1 <= 10)
                {
                    field.FieldChange(x1, z1, Field.FieldState.none);
                }
                else { check1 = true; }

                if (x1 <= 10 && z2 >= 1)
                {
                    field.FieldChange(x1, z2, Field.FieldState.none);
                }
                else { check2 = true; }

                if (x2 >= 1 && z2 >= 1)
                {
                    field.FieldChange(x2, z2, Field.FieldState.none);
                }
                else { check3 = true; }

                if (x2 >= 1 && z1 <= 10)
                {
                    field.FieldChange(x2, z1, Field.FieldState.none);
                }
                else { check4 = true; }
            }


            if (check1 && check2 && check3 && check4)
            {
                isAttacking = false;
                yield break;
            }

            yield return new WaitForSeconds(waittime);
        }

    }

    public IEnumerator Twin(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {
        int count = 0;
        int x_pos_temp = x_pos;
        int z_pos_temp = z_pos;
        int x_dir_temp = x_dir;
        int z_dir_temp = z_dir;

        bool check1 = false;

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count * x_dir_temp;
            int z1 = z_pos_temp + count * z_dir_temp;


            if (x_dir_temp * z_dir_temp == 0)
            {
                if (x1 >= 1 && x1 <= 10 && z1 >= 1 && z1 <= 10)
                {
                    field.FieldChange(x1 + z_dir_temp * 1, z1 + x_dir_temp * 1, fieldState1);
                    field.FieldChange(x1 + z_dir_temp * -1, z1 + x_dir_temp * -1, fieldState1);
                }
                else { check1 = true; }

            }
            else
            {
                if (x1 >= 0 && x1 <= 11 && z1 >= 0 && z1 <= 11)
                {
                    field.FieldChange(x1 + z_dir_temp * 1, z1 + x_dir_temp * -1, fieldState1);
                    field.FieldChange(x1 + z_dir_temp * -1, z1 + x_dir_temp * 1, fieldState1);
                }
                else { check1 = true; }


            }

            if (check1)
            {
                check1 = false;
                count = 0;
                break;
            }

            yield return new WaitForSeconds(waittime);
        }

        soundCountroller.PlaySE(SoundCountroller.Sound.select + playerID);

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count * x_dir_temp;
            int z1 = z_pos_temp + count * z_dir_temp;


            if (x_dir_temp * z_dir_temp == 0)
            {
                if (x1 >= 1 && x1 <= 10 && z1 >= 1 && z1 <= 10)
                {
                    field.FieldChange(x1 + z_dir_temp * 1, z1 + x_dir_temp * 1, fieldState2);
                    field.FieldChange(x1 + z_dir_temp * -1, z1 + x_dir_temp * -1, fieldState2);
                }
                else { check1 = true; }

            }
            else
            {
                if (x1 >= 0 && x1 <= 11 && z1 >= 0 && z1 <= 11)
                {
                    field.FieldChange(x1 + z_dir_temp * 1, z1 + x_dir_temp * -1, fieldState2);
                    field.FieldChange(x1 + z_dir_temp * -1, z1 + x_dir_temp * 1, fieldState2);
                }
                else { check1 = true; }


            }

            if (check1)
            {
                check1 = false;
                count = 0;
                break;
            }

            yield return new WaitForSeconds(waittime);
        }

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            count++;

            int x1 = x_pos_temp + count * x_dir_temp;
            int z1 = z_pos_temp + count * z_dir_temp;


            if (x_dir_temp * z_dir_temp == 0)
            {
                if (x1 >= 1 && x1 <= 10 && z1 >= 1 && z1 <= 10)
                {
                    field.FieldChange(x1 + z_dir_temp * 1, z1 + x_dir_temp * 1, Field.FieldState.none);
                    field.FieldChange(x1 + z_dir_temp * -1, z1 + x_dir_temp * -1, Field.FieldState.none);
                }
                else { check1 = true; }

            }
            else
            {
                if (x1 >= 0 && x1 <= 11 && z1 >= 0 && z1 <= 11)
                {
                    field.FieldChange(x1 + z_dir_temp * 1, z1 + x_dir_temp * -1, Field.FieldState.none);
                    field.FieldChange(x1 + z_dir_temp * -1, z1 + x_dir_temp * 1, Field.FieldState.none);
                }
                else { check1 = true; }


            }

            if (check1)
            {
                isAttacking = false;
                yield break;
            }

            yield return new WaitForSeconds(waittime);
        }

    }

    public IEnumerator Wall(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {

        int x_pos_temp = x_pos;
        int z_pos_temp = z_pos;
        int x_dir_temp = x_dir;
        int z_dir_temp = z_dir;


        if (x_dir_temp * z_dir_temp == 0)
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 0, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 1 + x_dir_temp * -2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 0 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 0 + x_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 0 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 0 + x_dir_temp * -2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -1 + z_dir_temp * 2, z_pos_temp + z_dir_temp * -1 + x_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -1 + z_dir_temp * -2, z_pos_temp + z_dir_temp * -1 + x_dir_temp * -2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 2, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * -2, z_pos_temp + z_dir_temp * -2 + x_dir_temp * -2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * -2 + x_dir_temp * -1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 0, fieldState1);
            yield return new WaitForSeconds(waittime);
        }
        else
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3, z_pos_temp + z_dir_temp * 0, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 3, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * -1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -1, z_pos_temp + z_dir_temp * 2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * -2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -2, z_pos_temp + z_dir_temp * 1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * -3, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -3, z_pos_temp + z_dir_temp * 0, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -1, z_pos_temp + z_dir_temp * -2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * -2, z_pos_temp + z_dir_temp * -1, fieldState1);
            yield return new WaitForSeconds(waittime);
        }

        soundCountroller.PlaySE(SoundCountroller.Sound.select + playerID);

        if (x_dir_temp * z_dir_temp == 0)
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 0, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 1 + x_dir_temp * -2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 0 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 0 + x_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 0 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 0 + x_dir_temp * -2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -1 + z_dir_temp * 2, z_pos_temp + z_dir_temp * -1 + x_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -1 + z_dir_temp * -2, z_pos_temp + z_dir_temp * -1 + x_dir_temp * -2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 2, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * -2, z_pos_temp + z_dir_temp * -2 + x_dir_temp * -2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * -2 + x_dir_temp * -1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 0, fieldState2);
            yield return new WaitForSeconds(waittime);
        }
        else
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3, z_pos_temp + z_dir_temp * 0, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 3, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * -1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -1, z_pos_temp + z_dir_temp * 2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * -2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -2, z_pos_temp + z_dir_temp * 1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * -3, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -3, z_pos_temp + z_dir_temp * 0, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -1, z_pos_temp + z_dir_temp * -2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * -2, z_pos_temp + z_dir_temp * -1, fieldState2);
            yield return new WaitForSeconds(waittime);
        }

        yield return new WaitForSeconds(0.5f);

        if (x_dir_temp * z_dir_temp == 0)
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 0, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 1 + x_dir_temp * -2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 0 + z_dir_temp * 2, z_pos_temp + z_dir_temp * 0 + x_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 0 + z_dir_temp * -2, z_pos_temp + z_dir_temp * 0 + x_dir_temp * -2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -1 + z_dir_temp * 2, z_pos_temp + z_dir_temp * -1 + x_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -1 + z_dir_temp * -2, z_pos_temp + z_dir_temp * -1 + x_dir_temp * -2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 2, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * -2, z_pos_temp + z_dir_temp * -2 + x_dir_temp * -2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * -2 + x_dir_temp * -1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * -2 + x_dir_temp * 0, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
        }
        else
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3, z_pos_temp + z_dir_temp * 0, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 3, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * -1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -1, z_pos_temp + z_dir_temp * 2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * -2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -2, z_pos_temp + z_dir_temp * 1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * -3, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -3, z_pos_temp + z_dir_temp * 0, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * -1, z_pos_temp + z_dir_temp * -2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * -2, z_pos_temp + z_dir_temp * -1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
        }

        isAttacking = false;
    }

    public IEnumerator Front(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {

        int x_pos_temp = x_pos;
        int z_pos_temp = z_pos;
        int x_dir_temp = x_dir;
        int z_dir_temp = z_dir;

        if (x_dir_temp * z_dir_temp == 0)
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 0, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 1 + x_dir_temp * -1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 0, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 3 + x_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 3 + x_dir_temp * 0, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 3 + x_dir_temp * -1, fieldState1);
            yield return new WaitForSeconds(waittime);
        }
        else
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 0, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 0, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 2, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 1, fieldState1);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3, z_pos_temp + z_dir_temp * 1, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 2, fieldState1);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 3, fieldState1);
            yield return new WaitForSeconds(waittime);
        }

        soundCountroller.PlaySE(SoundCountroller.Sound.select + playerID);

        if (x_dir_temp * z_dir_temp == 0)
        {

            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 0, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 1 + x_dir_temp * -1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 0, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 3 + x_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 3 + x_dir_temp * 0, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 3 + x_dir_temp * -1, fieldState2);
            yield return new WaitForSeconds(waittime);
        }
        else
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 0, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 0, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 2, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 1, fieldState2);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3, z_pos_temp + z_dir_temp * 1, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 2, fieldState2);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 3, fieldState2);
            yield return new WaitForSeconds(waittime);
        }

        yield return new WaitForSeconds(0.5f);

        if (x_dir_temp * z_dir_temp == 0)
        {

            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 1 + x_dir_temp * 0, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 1 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 1 + x_dir_temp * -1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 2 + x_dir_temp * 0, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 2 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 2 + x_dir_temp * -1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * 1, z_pos_temp + z_dir_temp * 3 + x_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * 0, z_pos_temp + z_dir_temp * 3 + x_dir_temp * 0, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 3 + z_dir_temp * -1, z_pos_temp + z_dir_temp * 3 + x_dir_temp * -1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
        }
        else
        {
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 0, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 0, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 0, z_pos_temp + z_dir_temp * 2, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 1, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
            field.FieldChange(x_pos_temp + x_dir_temp * 3, z_pos_temp + z_dir_temp * 1, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 2, z_pos_temp + z_dir_temp * 2, Field.FieldState.none);
            field.FieldChange(x_pos_temp + x_dir_temp * 1, z_pos_temp + z_dir_temp * 3, Field.FieldState.none);
            yield return new WaitForSeconds(waittime);
        }

        isAttacking = false;
    }


    public IEnumerator DamagedWait()
    {
        yield return new WaitForSeconds(2.0f);
        damaged = false;
        transform.position = new Vector3(Random.Range(-5.0f + Field.getDeathNum(), 5.0f - Field.getDeathNum()), 0, Random.Range(-5.0f + Field.getDeathNum(), 5.0f - Field.getDeathNum()));
        innerAttack = false;
        float invincibleTime = 3.0f;
        SkinnedMeshRenderer alpha = model.GetComponent<SkinnedMeshRenderer>();
        float r = alpha.material.color.r;
        float g = alpha.material.color.g;
        float b = alpha.material.color.b;
        do
        {

            //alpha.material.SetColor("_color", new Color(1, 1, 1, Mathf.Sin(Time.time * 3) / 2 + 0.5f));
            //alpha.material.color = new Color(r, g, b, Mathf.Pow(Mathf.Cos(Mathf.PI * invincibleTime), 2)/2+0.5f);

            alpha.material.color = new Color(r, g, b, Mathf.PingPong(Time.time * 10, 1));

            yield return new WaitForEndOfFrame();
            invincibleTime -= Time.deltaTime;
        } while (invincibleTime > 0);

        alpha.material.color = new Color(r, g, b, 1);

        invincible = false;

        if (innerAttack)
        {
            Damage();
        }
        yield break;
    }

    public IEnumerator DeadWait()
    {


        yield return new WaitForSeconds(2.0f);
        agent.Warp(new Vector3(0, 50, 0));
        //transform.position = new Vector3(0 , 50, 0);
        yield return new WaitForSeconds(3.0f);

        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!damaged && !invincible && other.tag != playerID + "PAttack")
        {
            Damage();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (invincible && other.tag != playerID + "PAttack")
        {
            innerAttack = true;
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (invincible && other.tag != playerID + "PAttack")
        {
            innerAttack = false;
        }

    }
}
