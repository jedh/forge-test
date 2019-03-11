using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DasMusic : MonoBehaviour
{
	public AudioSource MusicSource;

	// Start is called before the first frame update
	void Start()
	{
		//if (MusicSource != null)
		//{
		//MusicSource.Play();
		//}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			if (MusicSource != null)
			{
				MusicSource.mute = !MusicSource.mute;
			}
		}
	}
}
