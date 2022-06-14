using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    [SerializeField] int playerID;
    [SerializeField] Animator animator;
    [SerializeField] UnityEngine.UI.Text text;
    [SerializeField] int attackType = 0;
    [SerializeField] const int ATTACK_TYPE_NUM = 4;
    private bool isDecided = false;
    private bool isInputAcceptance = true;
    [SerializeField] GameObject playerModel;
    [SerializeField] UnityEngine.UI.Text decidedText;
    [SerializeField] UnityEngine.UI.Image frameImage;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip enterSE;
    //[SerializeField] Animator playerModelAnim;

    // Start is called before the first frame update
    void Start()
    {
        frameImage.gameObject.SetActive(true);
        //animator = GetComponent<Animator>();
        //text = GetComponent<UnityEngine.UI.Text>();
        text.text = ("クロス");
        text.gameObject.SetActive(true);
        playerModel.SetActive(true);
        animator.gameObject.SetActive(true);
        decidedText.gameObject.SetActive(false);
        //playerModelAnim = playerModel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SelectAttackType();
        //if (Input.GetKeyDown("right")) {
        //    attackType = (attackType + 1) % 2;
        //}
        //if (!isDecided)
        //{
        //    switch (attackType)
        //    {
        //        case 0:
        //            animator.SetInteger("type", 0);
        //            text.text = ("攻撃タイプ1");
        //            break;

        //        case 1:
        //            animator.SetInteger("type", 1);
        //            text.text = ("攻撃タイプ2");
        //            break;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Return)) {
        //    isDecided = true;
        //    switch (attackType)
        //    {
        //        case 0:
        //            animator.SetInteger("type", 0);
        //            text.text = ("【攻撃タイプ1】");
        //            break;

        //        case 1:
        //            animator.SetInteger("type", 1);
        //            text.text = ("【攻撃タイプ2】");
        //            break;
        //    }
        //}
    }

    public bool GetIsDecided()
    {
        return isDecided;
    }

    public int GetAttackType()
    {
        return attackType;
    }

    public void ChangeAttackType(int way)
    {
        if (!isDecided)
        {
            audioSource.PlayOneShot(selectSE);
            Debug.Log("z");
            if (way == 1)
            {
                attackType = (attackType + 1) % ATTACK_TYPE_NUM;
            }
            else if (way == -1)
            {
                attackType = (attackType - 1) % ATTACK_TYPE_NUM;
                if (attackType == -1)
                {
                    attackType = ATTACK_TYPE_NUM - 1;
                }
            }

            switch (attackType)
            {
                case 0:
                    animator.SetInteger("type", attackType);
                    text.text = ("クロス");
                    break;

                case 1:
                    animator.SetInteger("type", attackType);
                    text.text = ("ツイン");
                    break;

                case 2:
                    animator.SetInteger("type", attackType);
                    text.text = ("ウォール");
                    break;

                case 3:
                    animator.SetInteger("type", attackType);
                    text.text = ("フロント");
                    break;

                //case 4:
                //    animator.SetInteger("type", attackType);
                //    text.text = ("ツイン");
                //    break;

                default:
                    attackType = 0;
                    break;
            }
        }
    }

    private void SelectAttackType()
    {
        if (Input.GetAxis(playerID + "P_Horizontal") > 0 && isInputAcceptance)
        {
            ChangeAttackType(1);
            isInputAcceptance = false;
        }
        if (Input.GetAxis(playerID + "P_Horizontal") < 0 && isInputAcceptance)
        {
            ChangeAttackType(-1);
            isInputAcceptance = false;
        }
        if (Input.GetAxis(playerID + "P_Horizontal") == 0)
        {
            isInputAcceptance = true;
        }

        if (Input.GetButtonDown(playerID + "P_normal"))
        {
            DecideAttackType();
        }
        if (Input.GetButtonDown(playerID + "P_special1"))
        {
            audioSource.PlayOneShot(selectSE);
            CancelDecide();
        }
    }

    private void DecideAttackType()
    {
        if (!isDecided)
        {
            isDecided = true;
            audioSource.PlayOneShot(enterSE);
        }
        decidedText.gameObject.SetActive(true);
        //switch (attackType)
        //{
        //    case 0:
        //        //animator.SetInteger("type", 0);
        //        text.text = ("【クロス】");
        //        break;

        //    case 1:
        //        //animator.SetInteger("type", 1);
        //        text.text = ("【クロス2】");
        //        break;

        //    case 2:
        //        //animator.SetInteger("type", 1);
        //        text.text = ("【a】");
        //        break;

        //    case 3:
        //        //animator.SetInteger("type", 1);
        //        text.text = ("【b】");
        //        break;
        //}
        //playerModelAnim.SetBool("New Bool", true);
    }

    private void CancelDecide()
    {
        isDecided = false;
        decidedText.gameObject.SetActive(false);
    }
}
