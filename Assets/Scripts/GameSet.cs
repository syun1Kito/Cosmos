using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSet : MonoBehaviour
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

    public void PlayWhistle()
    {
        soundCountroller.PlaySE(SoundCountroller.Sound.whistle);
    }

    public void End() {
        PlayerController.Result();
    }
}
