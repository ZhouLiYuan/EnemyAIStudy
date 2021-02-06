using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public static void PlaySound(string name)
    {
        new GameObject().AddComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("audio"));
        // 播放完成后销毁
    }
}
