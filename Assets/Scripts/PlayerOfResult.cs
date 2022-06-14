using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOfResult : MonoBehaviour
{
    [SerializeField] int playerID;
    [SerializeField] UnityEngine.UI.Text rankText;
    [SerializeField] UnityEngine.UI.Text OKText;
    [SerializeField] int rank;
    [SerializeField] float rotateSpeed = 0.1f;
    bool isOK = false;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip enterSE;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(playerID + "P_Horizontal") > 0)
        {
            transform.Rotate(new Vector3(0, -rotateSpeed, 0));
        }
        if (Input.GetAxis(playerID + "P_Horizontal") < 0)
        {
            transform.Rotate(new Vector3(0, rotateSpeed, 0));
        }
        if (Input.GetButtonDown(playerID + "P_normal"))
        {
            if (!isOK)
            {
                audioSource.PlayOneShot(selectSE);
                OKText.gameObject.SetActive(true);
                isOK = true;
            }
        }
    }

    public void SetRank(int rank)
    {
        this.rank = rank;
    }

    public void SetUp()
    {
        OKText.gameObject.SetActive(false);
        float s = 0.3f;
        switch (rank)
        {
            case 1:
                s = 0.6f;
                rankText.text = "1st";
                break;

            case 2:
                s = 0.5f;
                rankText.text = "2nd";
                break;

            case 3:
                s = 0.4f;
                rankText.text = "3rd";
                break;

            case 4:
                s = 0.3f;
                rankText.text = "4th";
                break;
            //default:
            //    s = 1.0f;
            //    rankText.text = "???";
            //    break;
                
        }
        gameObject.transform.localScale = new Vector3(s, s, s);
    }

    public bool GetIsOK()
    {
        return isOK;
    }
}
