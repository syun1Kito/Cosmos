using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultConroller : MonoBehaviour
{
    int playerNumber;
    int[] ranks;
    [SerializeField] PlayerOfResult[] playerOfResults;
    [SerializeField] GameObject[] UIs;

    // Start is called before the first frame update
    void Start()
    {
        //playerNumber = PlayerSelectController.GetPlayerNumber();]
        playerNumber = PlayerSelectController.GetPlayerNumber();

        //順位を取得
        ranks = PlayerController.GetPlayerRanks();

        for (int i = 0; i < playerNumber; i++)
        {
            playerOfResults[i].gameObject.SetActive(true);
            UIs[i].SetActive(true);
            playerOfResults[i].SetRank(ranks[i]);
        }
        for (int i = playerNumber; i < 4; i++)
        {
            playerOfResults[i].gameObject.SetActive(false);
            UIs[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int cnt = 0;
        for (int i = 0; i < playerNumber; i++)
        {
            if (playerOfResults[i].GetIsOK())
            {
                cnt++;
            }
        }
        if (cnt == playerNumber)
        {
            LoadTitleScene();
        }
    }

    void LoadTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
}
