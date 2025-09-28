using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudioClipManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic(Sound.MainTheme, true);
    }

    public void AudioOnButtonClick()
    {
        AudioManager.Instance.PlayUI(Sound.UIButtonClick);
    }
}
