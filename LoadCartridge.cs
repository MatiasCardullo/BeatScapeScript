
using UnityEngine;

public class LoadCartridge : MonoBehaviour
{
    public Cartridge input;
    public AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
        music.clip = input.audio;
        input.timeDuration = music.time;
    }
}
