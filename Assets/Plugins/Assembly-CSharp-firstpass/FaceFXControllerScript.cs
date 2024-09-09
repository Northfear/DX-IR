using System.Collections;
using UnityEngine;

public class FaceFXControllerScript : MonoBehaviour
{
	private const int FACEFX_BONEPOSE_LAYER = 10;

	private const int FACEFX_ANIMATION_LAYER = 9;

	private const int FACEFX_EVENTTRACK_LAYER = 8;

	private const int FACEFX_REFERENCEPOSE_LAYER = 7;

	public bool UseReferencePoseFromFBX = true;

	public float ScaleFactor = 1f;

	public string EVENT_TRACK_NAME = "_eventtrack";

	private GameObject facefx_controller;

	private AudioClip audio_clip;

	private string animation_name;

	private int play_state;

	private float audio_start_time;

	private float anim_eval_time;

	private static AnimationCurve inverse_hermite;

	private static bool switch_anim;

	public void InitializeFaceFXController(GameObject ffxController)
	{
		if (ffxController == null)
		{
			Debug.Log("Can not initialize null FaceFX controller.");
			return;
		}
		facefx_controller = ffxController;
		foreach (Transform item in facefx_controller.transform)
		{
			AnimationState animationState = base.animation[item.name];
			if (animationState != null)
			{
				animationState.layer = 10;
				animationState.blendMode = AnimationBlendMode.Additive;
				animationState.wrapMode = WrapMode.ClampForever;
				animationState.enabled = true;
				animationState.weight = 1f;
			}
		}
		if (base.animation == null)
		{
			Debug.Log("Warning.  Animation component must be attached to " + base.name + " character for animations to play!");
		}
		else
		{
			AnimationState animationState2 = base.animation["facefx_loop_anim"];
			if (animationState2 != null)
			{
				animationState2.layer = 7;
				animationState2.wrapMode = WrapMode.ClampForever;
				base.animation.Play("facefx_loop_anim");
			}
			else
			{
				Debug.Log("No facefx_loop_anim animation found for " + base.name + ".  The facefx_controller is likely corrupt and should be reimported.");
			}
		}
		foreach (Transform item2 in facefx_controller.transform)
		{
			AnimationState animationState3 = base.animation[item2.name];
			if (animationState3 != null)
			{
				animationState3.normalizedSpeed = 0f;
			}
		}
	}

	private void Start()
	{
		Debug.Log("Starting FaceFX Controller.");
		switch_anim = true;
		play_state = 0;
		Transform transform = base.transform.Find("facefx_controller");
		if (transform != null)
		{
			facefx_controller = transform.gameObject;
		}
		if (facefx_controller == null)
		{
			Debug.Log("Warning.  Could not find FaceFX Controller for " + base.name + "!  You need to use the ImportXML function and pass a FaceFX XML file.");
		}
		else
		{
			InitializeFaceFXController(facefx_controller);
		}
		if (inverse_hermite == null)
		{
			inverse_hermite = new AnimationCurve();
			AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
			for (float num = 0f; num <= 1f; num += 0.01f)
			{
				inverse_hermite.AddKey(animationCurve.Evaluate(num), num);
			}
		}
	}

	public void StopAnim()
	{
		base.audio.Stop();
		play_state = 0;
		switch_anim = true;
	}

	public void PlayAudioFunction()
	{
		if (base.audio.isPlaying)
		{
			Debug.Log("Audio is already playing!");
			return;
		}
		base.audio.clip = audio_clip;
		base.audio.Play();
	}

	public IEnumerator PlayAnimCoroutine(string animName, AudioClip animAudio)
	{
		switch_anim = false;
		play_state = 1;
		anim_eval_time = 0f;
		audio_start_time = 0f;
		if (null == animAudio)
		{
			Debug.Log("Audio is null!");
		}
		animation_name = animName;
		audio_clip = animAudio;
		if (animName != null)
		{
			AnimationState animState = base.animation[animation_name];
			if (animState != null)
			{
				Debug.Log("playing anim " + animName);
				animState.speed = 0f;
				animState.time = 0f;
				animState.layer = 9;
				base.animation.Play(animName);
				if (facefx_controller != null)
				{
					audio_start_time = 1000f;
					yield return null;
					audio_start_time = facefx_controller.transform.localPosition.x;
				}
			}
			else
			{
				Debug.Log("No AnimationState for animation:" + animation_name + " on player " + base.name);
			}
			if (base.animation[animation_name + EVENT_TRACK_NAME] != null)
			{
				base.animation.Play(animation_name + EVENT_TRACK_NAME);
			}
		}
		else
		{
			Debug.Log("No animation passed into PlayAnim.  Playing audio.");
			PlayAudioFunction();
		}
	}

	public void PlayAnim(string animName, AudioClip animAudio)
	{
		StartCoroutine(PlayAnimCoroutine(animName, animAudio));
	}

	public void handleFaceFXPayLoadEvent(string payload)
	{
		if (payload.StartsWith("game: playanim "))
		{
			string text = payload.Substring(15);
			if (null != base.animation[text])
			{
				Debug.Log("playing body animation from payload: " + text);
				base.animation.Play(text);
			}
			else
			{
				Debug.Log("Payload animation doesn't exist: " + text);
			}
		}
		else
		{
			Debug.Log("Unknown event payload: " + payload);
		}
	}

	public void Update()
	{
		if (play_state <= 0)
		{
			return;
		}
		AnimationState animationState = base.animation[animation_name];
		if (animationState != null)
		{
			anim_eval_time += Time.deltaTime;
			if (play_state == 1 && animationState.time >= audio_start_time)
			{
				PlayAudioFunction();
				play_state = 2;
			}
			if (play_state == 2)
			{
				if (base.audio.isPlaying && base.audio.time < audio_clip.length)
				{
					anim_eval_time = base.audio.time + audio_start_time;
				}
				else if (!base.audio.isPlaying)
				{
					play_state = 3;
				}
			}
			if (play_state == 3 && anim_eval_time >= animationState.length)
			{
				switch_anim = true;
				play_state = 0;
			}
			if (anim_eval_time <= animationState.length)
			{
				animationState.time = anim_eval_time;
			}
			if (!(facefx_controller != null))
			{
				return;
			}
			{
				foreach (Transform item in facefx_controller.transform)
				{
					AnimationState animationState2 = base.animation[item.name];
					if (animationState2 != null)
					{
						animationState2.normalizedTime = inverse_hermite.Evaluate(item.transform.localPosition.x);
						animationState2.normalizedSpeed = 0f;
					}
				}
				return;
			}
		}
		if (!base.audio.isPlaying)
		{
			Debug.Log("audio with no animation case");
			switch_anim = true;
			play_state = 0;
		}
	}

	public int GetPlayState()
	{
		return play_state;
	}

	public static bool GetSwitchAnim()
	{
		return switch_anim;
	}

	public Transform GetFaceFXControllerGameObject()
	{
		return base.transform.Find("facefx_controller");
	}
}
