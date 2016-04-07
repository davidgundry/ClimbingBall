using UnityEngine;
using System.Collections;
using System;

public class AudioGenerator : MonoBehaviour {

	public PlayerBehaviour player;
	private float x1,x2,x3,vol1,vol2,vol3;
	private float sampleRate;
	private float frequency1;
	private float frequency2;
	private float frequency3;
	private const float gain = 0.5f;
	private float noi;
	private double a = 1.059463094359;
	private float referenceNote;

	void Awake()
	{
		sampleRate = AudioSettings.outputSampleRate;
	}

	void Update()
	{
		referenceNote = (float) (440 * Math.Pow(a,(player.transform.position.y % 15f)));
		float pos = (float) (Math.Sin(player.transform.position.x/10) * 7f);
		frequency1 = (float) (referenceNote * Math.Pow(a,(pos-16)*2));
		vol1 = (pos*pos) / 49;
		frequency2 = (float) (referenceNote * Math.Pow(a,(pos-8)*2));
		vol2 =  pos < 3.5 ? (pos/ 7): 1-(pos/7);
		frequency3 = (float) (referenceNote * Math.Pow(a,(pos)*2));
		vol3 = 1-(pos / 7);
		noi = UnityEngine.Random.value*0.1f;
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		for (int i=0;i<data.Length;i++)
		{
			data[i] = gain * ((Mathf.Tan(x1)*vol1+Mathf.Tan(x2)*vol2+Mathf.Tan(x3)*vol3)/3);
			x1 += (frequency1/sampleRate)*Mathf.PI*2;
			x2 += (frequency2/sampleRate)*Mathf.PI*2;
			x3 += (frequency3/sampleRate)*Mathf.PI*2;
		}

	}
}
