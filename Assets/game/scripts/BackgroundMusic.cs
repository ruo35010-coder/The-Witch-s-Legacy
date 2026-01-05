using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [Header("背景音乐")]
    public AudioClip backgroundMusic;  // 背景音乐文件
    
    [Header("音量设置")]
    [Range(0f, 1f)]
    public float volume = 0.5f;        // 音量大小
    public bool loop = true;           // 是否循环播放
    
    private AudioSource audioSource;
    
    void Start()
    {
        // 添加AudioSource组件
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // 设置AudioSource属性
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = true;
        
        // 播放音乐
        audioSource.Play();
        
        // 确保游戏对象不被销毁
        DontDestroyOnLoad(gameObject);
    }
    
    // 控制方法
    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
    public void StopMusic()
    {
        audioSource.Stop();
    }
    
    public void PauseMusic()
    {
        audioSource.Pause();
    }
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
    
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}
