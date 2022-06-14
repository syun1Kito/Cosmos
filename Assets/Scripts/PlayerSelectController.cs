using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectController : MonoBehaviour
{
    //[SerializeField] private PlayerSelect ps1;
    //[SerializeField] private PlayerSelect ps2;
    //[SerializeField] private PlayerSelect ps3;
    //[SerializeField] private PlayerSelect ps4;
    [SerializeField] private PlayerSelect[] playerSelects = new PlayerSelect[4];
    [SerializeField] GameObject[] players;
    [SerializeField] private UnityEngine.UI.Text descriptionText;
    [SerializeField] private UnityEngine.UI.Text text2P;
    [SerializeField] private UnityEngine.UI.Text text3P;
    [SerializeField] private UnityEngine.UI.Text text4P;
    private int mode;
    [SerializeField] public static int playerNumber = 4;
    private Color selectedColor = new Color(1, 1, 1);
    private Color unselectedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private GameObject[] attackTypeSelectUIs;
    //private bool wait = false; //スティックの連続入力を防ぐ
    private bool[] isInputAcceptances = { true, true, true, true};
    //[SerializeField] private Camera[] cameras = new Camera[4];
    public static int[] attackTypes = {0, 0, 0, 0};
    [SerializeField] UnityEngine.UI.Image timerGauge;
    float cancelTimer = 0;
    [SerializeField] UnityEngine.UI.Text cancellationText;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip enterSE;
    bool isSEPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        //cameras[0].gameObject.SetActive(true);
        //cameras[1].gameObject.SetActive(false);
        //cameras[2].gameObject.SetActive(false);
        //cameras[3].gameObject.SetActive(false);
        //cameras[0].rect = new Rect(0, 0, 1, 1);
        mode = 0;
        ResetStaticsFields();
        isSEPlayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == 0)
        {
            SelectPlayerNumber();
        }
        if (mode == 1)
        {
            SelectPlayersAttackType();
        }

        //キャンセルボタン長押し判定
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetButton((i + 1) + "P_special1"))
            {
                cancelTimer += 0.02f;
            }
            if (Input.GetButtonUp((i + 1) + "P_special1"))
            {
                cancelTimer = 0;
            }
        }

        timerGauge.fillAmount = cancelTimer;
        if (cancelTimer >= 1 && !isSEPlayed)
        {
            isSEPlayed = true;
            if (mode == 0)
            {
                StartCoroutine(WaitAndLoadScene("Title"));
                //UnityEngine.SceneManagement.SceneManager.LoadScene("Title");

            }
            else if(mode == 1)
            {
                StartCoroutine(WaitAndLoadScene("PlayerSelect"));
                //UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerSelect");
            }
        }
    }

    public static int GetPlayerNumber()
    {
        return playerNumber;
    }

    public static int GetAttackType(int playerID)
    {
        return attackTypes[playerID - 1] + 1;
    }

    private void SelectPlayerNumber()
    {
        descriptionText.text = "プレイ人数を選択:";
        cancellationText.text = "キャンセルボタン長押しで\nタイトルに戻る";

        //if (playerNumber < 4 && Input.GetAxis("Horizontal") > 0 && !wait)
        //{
        //    playerNumber++;
        //    wait = true;
        //}

        //if (playerNumber > 2 && Input.GetAxis("Horizontal") < 0 && !wait)
        //{
        //    playerNumber--;
        //    wait = true;
        //}

        //if (Input.GetAxis("Horizontal") == 0)
        //{
        //    wait = false;
        //}

        //if (Input.GetButtonDown("Jump"))
        //{
        //    mode = 1;
        //    text2P.gameObject.SetActive(false);
        //    text3P.gameObject.SetActive(false);
        //    text4P.gameObject.SetActive(false);

        //    //SetUpCameras(playerNumber);
        //    for (int i = 0; i < playerNumber; i++)
        //    {
        //        attackTypeSelectUIs[i].SetActive(true);
        //    }
        //}

        for (int i = 0; i < 4; i++)
        {
            if(Input.GetAxis((i + 1) + "P_Horizontal") > 0 && isInputAcceptances[i])
            {
                if (playerNumber < 4)
                {
                    playerNumber++;
                    audioSource.PlayOneShot(selectSE);
                }
                isInputAcceptances[i] = false;
                //audioSource.PlayOneShot(selectSE);
            }
            if (Input.GetAxis((i + 1) + "P_Horizontal") < 0 && isInputAcceptances[i])
            {
                if (playerNumber > 2)
                {
                    playerNumber--;
                    audioSource.PlayOneShot(selectSE);
                }
                isInputAcceptances[i] = false;
                //audioSource.PlayOneShot(selectSE);
            }
            if (Input.GetAxis((i + 1) + "P_Horizontal") == 0)
            {
                isInputAcceptances[i] = true;
            }
            if(Input.GetButtonDown((i + 1) + "P_normal"))
            {
                mode = 1;
                text2P.gameObject.SetActive(false);
                text3P.gameObject.SetActive(false);
                text4P.gameObject.SetActive(false);
                for (int j = 0; j < playerNumber; j++)
                {
                    attackTypeSelectUIs[j].SetActive(true);
                }
                SetUIPos();
                audioSource.PlayOneShot(enterSE);
            }


            //debug
            //if (Input.GetKeyDown("2"))
            //{
            //    playerNumber = 2;
            //    mode = 1;
            //    text2P.gameObject.SetActive(false);
            //    text3P.gameObject.SetActive(false);
            //    text4P.gameObject.SetActive(false);
            //    for (int j = 0; j < playerNumber; j++)
            //    {
            //        attackTypeSelectUIs[j].SetActive(true);
            //    }
            //}
            //if (Input.GetKeyDown("3"))
            //{
            //    playerNumber = 3;
            //    mode = 1;
            //    text2P.gameObject.SetActive(false);
            //    text3P.gameObject.SetActive(false);
            //    text4P.gameObject.SetActive(false);
            //    for (int j = 0; j < playerNumber; j++)
            //    {
            //        attackTypeSelectUIs[j].SetActive(true);
            //    }
            //}
            //if (Input.GetKeyDown("4"))
            //{
            //    playerNumber = 4;
            //    mode = 1;
            //    text2P.gameObject.SetActive(false);
            //    text3P.gameObject.SetActive(false);
            //    text4P.gameObject.SetActive(false);
            //    for (int j = 0; j < playerNumber; j++)
            //    {
            //        attackTypeSelectUIs[j].SetActive(true);
            //    }
            //}
        }


        switch (playerNumber)
        {
            case 2:
                text2P.color = selectedColor;
                text3P.color = unselectedColor;
                text4P.color = unselectedColor;
                break;

            case 3:
                text2P.color = unselectedColor;
                text3P.color = selectedColor;
                text4P.color = unselectedColor;
                break;

            case 4:
                text2P.color = unselectedColor;
                text3P.color = unselectedColor;
                text4P.color = selectedColor;
                break;

            default:
                break;
        }
    }

    //private void AttackTypeSelect()
    //{
    //    descriptionText.text = "攻撃タイプを選択:";
    //    for (int i = 0; i < playerNumber; i++)
    //    {
    //        if (Input.GetAxis((i + 1) + "P_Horizontal") > 0 && !waitOfEachP[i])
    //        {
    //            playerSelects[i].ChangeAttackType(1);
    //            waitOfEachP[i] = true;
    //        }
    //        if (Input.GetAxis((i + 1) + "P_Horizontal") < 0 && !waitOfEachP[i])
    //        {
    //            playerSelects[i].ChangeAttackType(-1);
    //            waitOfEachP[i] = true;
    //        }
    //        if (Input.GetAxis((i + 1) + "P_Horizontal") == 0)
    //        {
    //            waitOfEachP[i] = false;
    //        }

    //        if(Input.GetButtonDown((i + 1) + "P_normal"))
    //        {
    //            playerSelects[i].DecideAttackType();
    //        }
    //        if (Input.GetButtonDown((i + 1) + "P_special1"))
    //        {
    //            playerSelects[i].CanselDecide();
    //        }
    //    }
    //}

    //private void SetUpCameras(int playerNum)
    //{
    //    switch (playerNum)
    //    {
    //        case 2:
    //            cameras[0].rect = new Rect(0, 0, 0.5f, 1);
    //            cameras[1].rect = new Rect(0.5f, 0, 0.5f, 1);
    //            break;

    //        case 3:
    //            cameras[0].rect = new Rect(0, 0, 0.33f, 1);
    //            cameras[1].rect = new Rect(0.33f, 0, 0.33f, 1);
    //            cameras[2].rect = new Rect(0.66f, 0, 0.34f, 1);
    //            break;

    //        case 4:
    //            cameras[0].rect = new Rect(0, 0, 0.25f, 1);
    //            cameras[1].rect = new Rect(0.25f, 0, 0.25f, 1);
    //            cameras[2].rect = new Rect(0.5f, 0, 0.25f, 1);
    //            cameras[3].rect = new Rect(0.75f, 0, 0.25f, 1);
    //            break;

    //    }

    //}
    void SelectPlayersAttackType()
    {
        //SetUIPos();
        descriptionText.text = "攻撃タイプを選択:";
        cancellationText.text = "キャンセルボタン長押しで\n人数選択に戻る";

        int cnt = 0;
        for (int i = 0; i < playerNumber; i++)
        {
            if (playerSelects[i].GetIsDecided())
            {
                cnt++;
            }
        }
        if (cnt == playerNumber)
        {
            for (int i = 0; i < playerNumber; i++)
            {
                attackTypes[i] = playerSelects[i].GetAttackType();
            }
            //LoadMainScene();
            StartCoroutine(WaitAndLoadScene("Main"));
        }
    }

    //private void LoadMainScene()
    //{
    //    UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    //}

    private IEnumerator WaitAndLoadScene(string sceneName)
    {
        audioSource.PlayOneShot(enterSE);
        yield return new WaitForSeconds(0.1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void ResetStaticsFields()
    {
        playerNumber = 2;
        for (int i = 0; i < 4; i++)
        {
            attackTypes[i] = 0;
        }
    }

    void SetUIPos()
    {
        /*
        RectTransform[] rects = new RectTransform[4];
        switch (GetPlayerNumber())
        {
            case 2:
                players[0].transform.position = new Vector3(-3, 1, 0);
                players[1].transform.position = new Vector3(3, 1, 0);
                rects[0] = attackTypeSelectUIs[0].GetComponent<RectTransform>();
                rects[1] = attackTypeSelectUIs[1].GetComponent<RectTransform>();
                rects[0].position = new Vector3(-130, -50, 0);
                rects[1].position = new Vector3(130, -50, 0);
                break;

            case 3:
                players[0].transform.position = new Vector3(-5, 1, 0);
                players[1].transform.position = new Vector3(0, 1, 0);
                players[2].transform.position = new Vector3(5, 1, 0);
                rects[0] = attackTypeSelectUIs[0].GetComponent<RectTransform>();
                rects[1] = attackTypeSelectUIs[1].GetComponent<RectTransform>();
                rects[2] = attackTypeSelectUIs[2].GetComponent<RectTransform>();
                rects[0].position = new Vector2(-200, -50);
                rects[1].position = new Vector2(0, -50);
                rects[2].position = new Vector2(200, -50);
                break;

            case 4:
                players[0].transform.position = new Vector3(-6, 1, 0);
                players[1].transform.position = new Vector3(-2, 1, 0);
                players[2].transform.position = new Vector3(2, 1, 0);
                players[3].transform.position = new Vector3(6, 1, 0);
                rects[0] = attackTypeSelectUIs[0].GetComponent<RectTransform>();
                rects[1] = attackTypeSelectUIs[1].GetComponent<RectTransform>();
                rects[2] = attackTypeSelectUIs[2].GetComponent<RectTransform>();
                rects[3] = attackTypeSelectUIs[3].GetComponent<RectTransform>();
                rects[0].position = new Vector2(-240, -50);
                rects[1].position = new Vector2(-80, -50);
                rects[2].position = new Vector2(80, -50);
                rects[3].position = new Vector2(240, -50);

                break;

            default:
                break;
        }
        */
    }
}
