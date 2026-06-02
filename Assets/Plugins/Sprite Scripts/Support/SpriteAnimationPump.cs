using System.Collections;
using UnityEngine;

public class SpriteAnimationPump : MonoBehaviour
{
	private static SpriteAnimationPump instance;

	protected static ISpriteAnimatable head;

	protected static ISpriteAnimatable cur;

	private static float _timeScale = 1f;

	private static float startTime;

	private static float time;

	private static float elapsed;

	private static float timePaused;

	private static bool isPaused;

	private static ISpriteAnimatable next;

	protected static bool pumpIsRunning;

	protected static bool pumpIsDone = true;

	public static float animationPumpInterval = 0.03333f;

	public bool IsRunning
	{
		get
		{
			return pumpIsRunning;
		}
	}

	public static float timeScale
	{
		get
		{
			return _timeScale;
		}
		set
		{
			_timeScale = Mathf.Max(0f, value);
		}
	}

	public static SpriteAnimationPump Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject("SpriteAnimationPump");
				instance = (SpriteAnimationPump)gameObject.AddComponent(typeof(SpriteAnimationPump));
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused && !isPaused)
		{
			timePaused = Time.realtimeSinceStartup;
		}
		else if (!paused && isPaused)
		{
			float num = Time.realtimeSinceStartup - timePaused;
			startTime += num;
		}
		isPaused = paused;
	}

	public void StartAnimationPump()
	{
		if (!pumpIsRunning)
		{
			pumpIsRunning = true;
			StartCoroutine(PumpStarter());
		}
	}

	protected IEnumerator PumpStarter()
	{
		while (!pumpIsDone)
		{
			yield return null;
		}
		StartCoroutine(AnimationPump());
	}

	public static void StopAnimationPump()
	{
	}

	protected static IEnumerator AnimationPump()
	{
		startTime = Time.realtimeSinceStartup;
		pumpIsDone = false;
		while (pumpIsRunning)
		{
			if ((!isPaused && Time.timeScale == 0f) || (isPaused && Time.timeScale != 0f))
			{
				instance.OnApplicationPause(Time.timeScale == 0f);
			}
			yield return null;
			time = Time.realtimeSinceStartup;
			elapsed = (time - startTime) * _timeScale;
			startTime = time;
			for (cur = head; cur != null; cur = next)
			{
				next = cur.next;
				cur.StepAnim(elapsed);
			}
		}
		pumpIsDone = true;
	}

	public void OnDestroy()
	{
		instance = null;
	}

	public static void Add(ISpriteAnimatable s)
	{
		if (head != null)
		{
			s.next = head;
			head.prev = s;
			head = s;
		}
		else
		{
			head = s;
			Instance.StartAnimationPump();
		}
	}

	public static void Remove(ISpriteAnimatable s)
	{
		if (head == s)
		{
			head = s.next;
			if (head == null)
			{
				StopAnimationPump();
			}
		}
		else if (s.next != null)
		{
			s.prev.next = s.next;
			s.next.prev = s.prev;
		}
		else if (s.prev != null)
		{
			s.prev.next = null;
		}
		s.next = null;
		s.prev = null;
	}
}
