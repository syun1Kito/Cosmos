using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{

    [SerializeField] SoundCountroller soundCountroller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Gamestart()
    {
        Player.ChangeisPlaying();
    }

    public void Call3()
    {
        soundCountroller.PlaySE(SoundCountroller.Sound._3);
    }

    public void Call2()
    {
        soundCountroller.PlaySE(SoundCountroller.Sound._2);
    }
    public void Call1()
    {
        soundCountroller.PlaySE(SoundCountroller.Sound._1);
    }
    public void CallGO()
    {
        soundCountroller.PlaySE(SoundCountroller.Sound.go);
    }
}
