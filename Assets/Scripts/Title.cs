//タイトル用

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] private int selected = 0; //0:START 1:HowToPlay 2:Exit
    //public UnityEngine.UI.Text startText;
    //public UnityEngine.UI.Text exitText;
    //private Color selectedColor = new Color(1,1,1);
    //private Color unselectedColor = new Color(0.5f,0.5f,0.5f);

    public Camera mainCamera;
    public UnityEngine.UI.Image spaceImg;
    private Animator cameraAnimator;
    //[SerializeField] private bool wait = false;
    private bool[] isInputAcceptances = {false, false, false, false};

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip enterSE;

    // Start is called before the first frame update
    void Start()
    {
        cameraAnimator = mainCamera.GetComponent<Animator>();
        //startText.color = selectedColor;
        //exitText.color = unselectedColor;

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (selected < 2 && Input.GetAxis((i + 1) + "P_Vertical") < 0 && isInputAcceptances[i])
            {
                selected++;
                cameraAnimator.SetInteger("selected", selected);
                isInputAcceptances[i] = false;
                //startText.color = unselectedColor;
                //exitText.color = selectedColor;
                audioSource.PlayOneShot(selectSE);
            }

            if (selected > 0 && Input.GetAxis((i + 1) + "P_Vertical") > 0 && isInputAcceptances[i])
            {
                selected--;
                cameraAnimator.SetInteger("selected", selected);
                isInputAcceptances[i] = false;
                //startText.color = selectedColor;
                //exitText.color = unselectedColor;
                audioSource.PlayOneShot(selectSE);
            }

            if (Input.GetAxis((i + 1) + "P_Vertical") == 0)
            {
                isInputAcceptances[i] = true;
            }

            if (selected == 0 && Input.GetButtonDown((i + 1) + "P_normal"))
            {
                //audioSource.PlayOneShot(enterSE);
                //UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerSelect");
                StartCoroutine(WaitAndLoadScene("PlayerSelect"));
            }

            if (selected == 1 && Input.GetButtonDown((i + 1) + "P_normal"))
            {
                //audioSource.PlayOneShot(enterSE);
                //UnityEngine.SceneManagement.SceneManager.LoadScene("HowToPlay");
                StartCoroutine(WaitAndLoadScene("HowToPlay"));
            }

            if (selected == 2 && Input.GetButtonDown((i + 1) + "P_normal"))
            {
                UnityEngine.Application.Quit();
            }
        }

        //if(selected < 2 && Input.GetAxis("Vertical") < 0 && !wait)
        //{
        //    selected++;
        //    cameraAnimator.SetInteger("selected", selected);
        //    wait = true;
        //    //startText.color = unselectedColor;
        //    //exitText.color = selectedColor;
        //}

        //if(selected > 0 && Input.GetAxis("Vertical") > 0 && !wait)
        //{
        //    selected--;
        //    cameraAnimator.SetInteger("selected", selected);
        //    wait = true;
        //    //startText.color = selectedColor;
        //    //exitText.color = unselectedColor;
        //}

        //if (Input.GetAxis("Vertical") == 0)
        //{
        //    wait = false;
        //}

        //if (selected == 0 && Input.GetButtonDown("Jump")) {
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerSelect");
        //}

        //if (selected == 1 && Input.GetButtonDown("Jump"))
        //{
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("HowToPlay");
        //}

        //if (selected == 2 && Input.GetKeyDown(""))
        //{
        //    //ゲーム終了
        //
    }

    private IEnumerator WaitAndLoadScene(string sceneName)
    {
        audioSource.PlayOneShot(enterSE);
        yield return new WaitForSeconds(0.1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
