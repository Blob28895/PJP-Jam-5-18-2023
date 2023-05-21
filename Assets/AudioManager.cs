using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[System.Serializable]
	public class audioDict {
        public string clipName;
        public AudioClip audioClip;
    }

    [SerializeField]
    public audioDict[] clipList;

    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>(); 
	// Start is called before the first frame update
	void Start()
    {
        foreach(audioDict clip in clipList)
		{
            clips[clip.clipName] = clip.audioClip;
		}
    }

    public void PlaySound(string sound)
    {
        GameObject soundObj = new GameObject("Sound");
        AudioSource audioSrc = soundObj.AddComponent<AudioSource>();
        audioSrc.PlayOneShot(clips[sound]);
        Destroy(soundObj, 5f);
    }
}
