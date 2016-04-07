using UnityEngine;
using System.Collections;

public class AudioGenerator : MonoBehaviour {

	public PlayerBehaviour player;
	private float x;
	private float sampleRate;
	private float frequency;

	void Awake()
	{
		sampleRate = AudioSettings.outputSampleRate;
	}

	void Update()
	{
		frequency = 100 * (player.transform.position.y % 8);
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		for (int i=0;i<data.Length;i++)
		{
			data[i] = Mathf.Sin(x);
			x += (frequency/sampleRate)*Mathf.PI*2;
		}

	}
}
