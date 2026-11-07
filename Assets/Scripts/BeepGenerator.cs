using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeepGenerator : MonoBehaviour
{
    public static AudioClip GenerateBeep(float frequency = 800f,
        float duration = 0.08f, float volume = 0.3f)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("Beep", samples, 1, sampleRate, false);

        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            // Sine wave dengan envelope supaya tidak terlalu harsh
            float envelope = Mathf.Sin(Mathf.PI * i / samples);
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate)
                      * volume * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }
}