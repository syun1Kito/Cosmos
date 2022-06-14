using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image image;
    //[SerializeField] Sprite[] sprites;
    [SerializeField] int num;
    [SerializeField] UnityEngine.UI.Text text;
    //bool wait = false;
    private bool[] isInputAcceptances = { false, false, false, false };

    [SerializeField] UnityEngine.UI.Image timerGauge;
    float cancelTimer = 0;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip enterSE;
    bool isSEPlayed = false;

    [SerializeField] GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        num = 0;
        //image.sprite = sprites[0];
        gameObjects[num].SetActive(true);
        text.text = (num + 1) + "/" + gameObjects.Length;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < 4; i++)
        {
            if (num < gameObjects.Length - 1 && Input.GetAxis((i + 1) + "P_Horizontal") > 0 && isInputAcceptances[i])
            {
                //Debug.Log("a");
                gameObjects[num].SetActive(false);
                num++;
                //image.sprite = sprites[num];
                gameObjects[num].SetActive(true);
                isInputAcceptances[i] = false;
                text.text = (num + 1) + "/" + gameObjects.Length;
                //startText.color = unselectedColor;
                //exitText.color = selectedColor;
                audioSource.PlayOneShot(selectSE);
            }

            if (num > 0 && Input.GetAxis((i + 1) + "P_Horizontal") < 0 && isInputAcceptances[i])
            {
                gameObjects[num].SetActive(false);
                num--;
                gameObjects[num].SetActive(true);
                //image.sprite = sprites[num];
                isInputAcceptances[i] = false;
                text.text = (num + 1) + "/" + gameObjects.Length;
                //startText.color = selectedColor;
                //exitText.color = unselectedColor;
                audioSource.PlayOneShot(selectSE);
            }

            if (Input.GetAxis((i + 1) + "P_Horizontal") == 0)
            {
                isInputAcceptances[i] = true;
            }

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
            //audioSource.PlayOneShot(enterSE);
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
            StartCoroutine(WaitAndLoadScene("Title"));
        }
    }

    private IEnumerator WaitAndLoadScene(string sceneName)
    {
        audioSource.PlayOneShot(enterSE);
        yield return new WaitForSeconds(0.1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
