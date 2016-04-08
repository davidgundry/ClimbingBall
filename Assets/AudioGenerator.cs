using UnityEngine;
using System.Collections;
using System;

public class AudioGenerator : MonoBehaviour {

	public PlayerBehaviour player;
	private float x,x1,x2,x3,vol1,vol2,vol3;
	private float sampleRate;
	private float frequency1;
	private float frequency2;
	private float frequency3;
	private const float gain = 0.1f;
	//private float noi;
	private const double a = 1.059463094359;
	private float referenceNote;

	void Awake()
	{
		sampleRate = AudioSettings.outputSampleRate;
	}

	void Update()
	{
        referenceNote = 440;// (float)(440 * Math.Pow(a, (Mathf.Round(player.transform.position.y) % 16f) + 8));

        float pos = 2;// (float)(Mathf.Round(player.transform.position.x / 1.5f)) % 8f;

		frequency1 = (float) (referenceNote * Math.Pow(a,(pos-16)*2));
		vol1 = ((pos*pos) / 64f);
		frequency2 = (float) (referenceNote * Math.Pow(a,(pos-8)*2));
		vol2 =  pos < 3.5f ? (pos/ 8f): 1-(pos/8f);
		frequency3 = (float) (referenceNote * Math.Pow(a,(pos)*2));
        vol3 = 1 - ((pos * pos) / 64f);
		//noi = UnityEngine.Random.value*0.1f;
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		for (int i=0;i<data.Length;i++)
		{
            data[i] = gain * ((Mathf.Sin(x1) * vol1 + Mathf.Sin(x2) * vol2 + Mathf.Sin(x3) * vol3) / 3);
            data[i] = gain * Mathf.Sin(x);
            x+=0.1f;
            x1 += ((frequency1 / sampleRate) * Mathf.PI * 2);// % Mathf.PI  *2;
            x2 += ((frequency2 / sampleRate) * Mathf.PI * 2);// % Mathf.PI * 2;
            x3 += ((frequency3 / sampleRate) * Mathf.PI * 2);// % Mathf.PI * 2;
		}

	}
}
