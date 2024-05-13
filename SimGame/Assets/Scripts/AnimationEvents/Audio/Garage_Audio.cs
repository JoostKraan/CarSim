using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garage_Audio : MonoBehaviour
{
    public AudioSource openSound, closeSound;

    public void PlaySoundOpen() => openSound.Play();

    public void PlaySoundClose() => closeSound.Play();
}
