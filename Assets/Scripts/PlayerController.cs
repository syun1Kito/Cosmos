using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{



    //int playerID;

    //Vector3 startpos;

    //int x_pos = 0;
    //int z_pos = 0;
    //int x_dir = 1;
    //int z_dir = 1;

    //float waittime = 0.1f;


    [SerializeField] Field field;
    [SerializeField] static int playerNum;
    int MaxPlayerNum = 4;
    [SerializeField] GameObject[] playersObj = new GameObject[4];
    Player[] players = new Player[4];
    [SerializeField] GameObject[] playersStatus = new GameObject[4];

    static int[] playerAttackTypes = new int[4] {1,2,3,4};//1:Cross,2:Twin,3:Wall,4:Front
    static int[] playerRanks;
    static bool isPlaying;

    [SerializeField] Animator animator;

    void Start()
    {
        isPlaying = false;
        playerRanks = new int[4] { 0, 0, 0, 0 };
        playerNum = PlayerSelectController.GetPlayerNumber();

        for (int i = 0; i < MaxPlayerNum; i++)
        {
            //playersObj[i] = GameObject.FindWithTag((i + 1) + "P");
            players[i] = playersObj[i].GetComponent<Player>();
            //playersStatus[i] = GameObject.FindWithTag((i + 1) + "Pflame");
        }

        for (int i = 0; i < MaxPlayerNum - playerNum; i++)
        {
            playersObj[MaxPlayerNum - 1 - i].SetActive(false);
            playersStatus[MaxPlayerNum - 1 - i].SetActive(false);

        }

        for (int i = 0; i < playerNum; i++)
        {
            playerRanks[i] = 1;
        }

    }

    void Update()
    {

        for (int i = 0; i < playerNum; i++)
        {

            players[i].Move();
            players[i].GetFieldPos();
            players[i].GaugeUpdate();
            players[i].Attack();
            players[i].Dead();


        }
        

        if (playerNum - Field.getDeathNum() == 1)
        {
            animator.SetBool("gameset",true);
           
        }
    }
    public static void Result() {
        SceneManager.LoadScene("Result");
    }

    public static void SetPlayerRank(int playerID) {
        playerRanks[playerID - 1] = playerNum - Field.getDeathNum() + 1;
        Debug.Log(playerRanks[0]+" "+ playerRanks[1] + " " + playerRanks[2] + " " + playerRanks[3]);
    }

    public static int[] GetPlayerRanks() {
        return playerRanks;
    }

    public static bool GetisPlaying() {
        return isPlaying;
     }

    public static void SetisPlayingTrue() {
        isPlaying = true;
    }
    /*public static int GetPlayerAttackType(int playerID)
    {
        return playerAttackTypes[playerID - 1];
    }*/
}
