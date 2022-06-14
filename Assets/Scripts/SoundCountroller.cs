using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCountroller : MonoBehaviour
{


    public enum Sound
    {
        // mainBGM,
        move_from,
        move_to,
        select,
        flame,
        water,
        spark,
        moth,
        damage,
        enter,
        _3,
        _2,
        _1,
        go,
        whistle,
    }

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] BGMClips, SEClips;



    // Start is called before the first frame update
    void Start()
    {



        audioSource.PlayOneShot(BGMClips[0]);        
        audioSource.loop = true;
        audioSource.PlayScheduled(AudioSettings.dspTime + BGMClips[0].length);

    }
    // Update is called once per frame
    void Update()
    {

    }


    public void PlaySE(Sound num)
    {
        audioSource.PlayOneShot(SEClips[(int)num]);
    }

    public void PlayBGM()
    {
        // audioSource.loop = true;
        audioSource.Play();
    }

}
