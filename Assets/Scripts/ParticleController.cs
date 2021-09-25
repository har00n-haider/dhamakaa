using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleController : MonoBehaviour {

	[SerializeField]
	private ParticleSystem particles;
	private ParticleSystem.MainModule particlesMainModule;

	[SerializeField]
	private RawImage particleImage;
	private RectTransform particleImageRect;

	void Awake(){
		particlesMainModule = particles.main;
		particleImageRect   = particleImage.rectTransform;
		particles.Stop();
	}

	public void ShowParticles(int emitCount, Vector2 position)
	{
		particleImageRect.position = position;
		particles.Emit(emitCount);
		//particleImageRect
	}

	public void StartContinuousEmission()
	{
		particlesMainModule.loop = true;
		particles.Play();
	}

	public void StopContinuousEmission()
	{
		particlesMainModule.loop = false;
		particles.Stop();
	}
}