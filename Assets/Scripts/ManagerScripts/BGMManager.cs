using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    public AudioSource audioSource; // AudioSourceをInspectorで設定
    public AudioClip[] bgmClips; // 複数のBGMを格納する配列
    public AudioMixer audioMixer; // AudioMixerをInspectorで設定
    private const string BGM_VOLUME_PARAM = "BGMVolume"; // Exposeしたパラメータ名

    // 指定したBGMを再生する
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogWarning("指定されたBGMのインデックスが範囲外です: " + index);
            return;
        }

        audioSource.Stop();
        audioSource.clip = bgmClips[index];
        audioSource.Play();
        FadeVolume(-10, 2.0f);
    }

    // BGMの音量を変更する（dB単位）
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(BGM_VOLUME_PARAM, volume);
    }

    // 音量をフェードイン・フェードアウトで変更する
    public void FadeVolume(float targetVolume, float duration)
    {
        StartCoroutine(FadeVolumeCoroutine(targetVolume, duration));
    }

    private System.Collections.IEnumerator FadeVolumeCoroutine(float targetVolume, float duration)
    {
        if (audioMixer.GetFloat(BGM_VOLUME_PARAM, out float currentVolume))
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newVolume = Mathf.Lerp(currentVolume, targetVolume, elapsed / duration);
                audioMixer.SetFloat(BGM_VOLUME_PARAM, newVolume);
                yield return null;
            }
            audioMixer.SetFloat(BGM_VOLUME_PARAM, targetVolume);
        }
        else
        {
            Debug.LogWarning("AudioMixerのパラメータを取得できませんでした: " + BGM_VOLUME_PARAM);
        }
    }
}
