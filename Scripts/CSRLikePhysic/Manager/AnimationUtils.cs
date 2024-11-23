using System;
using System.Reflection;
using UnityEngine;

public static class AnimationUtils
{
	public static void PlayFirstFrame(Animation anim)
	{
		AnimationUtils.PlayFirstFrame(anim, anim.clip.name);
	}

	public static void PlayFirstFrame(Animation anim, string name)
	{
		AnimationState animationState = anim[name];
		animationState.time = 0f;
		animationState.enabled = true;
		animationState.speed = 0f;
		anim.Play(name);
	}

	public static void PlayLastFrame(Animation anim)
	{
		AnimationUtils.PlayLastFrame(anim, anim.clip.name);
	}

	public static void PlayLastFrame(Animation anim, string name)
	{
		anim.Stop();
		AnimationState animationState = anim[name];
		animationState.time = animationState.length;
		animationState.enabled = true;
		animationState.speed = 0f;
		anim.Play(name);
	}

	public static void PlayAnim(Animation anim)
	{
		AnimationUtils.PlayAnim(anim, anim.clip.name);
	}

	public static void PlayAnim(Animation anim, string name)
	{
		AnimationState animationState = anim[name];
		animationState.time = 0f;
		animationState.enabled = true;
		animationState.speed = 1f;
		anim.Play(name);
	}

	public static bool IsAnimationPlaying(Animation anim, string name)
	{
		return anim.IsPlaying(name);
	}

	public static void TriggerEvent(object target, AnimationEvent animEvent)
	{
		MethodInfo method = target.GetType().GetMethod(animEvent.functionName);
		if (method == null)
		{
			return;
		}
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length > 1)
		{
			return;
		}
		if (parameters.Length == 0)
		{
			method.Invoke(target, new object[0]);
			return;
		}
		ParameterInfo parameterInfo = parameters[0];
		if (parameterInfo.ParameterType == typeof(float))
		{
			method.Invoke(target, new object[]
			{
				animEvent.floatParameter
			});
		}
		else if (parameterInfo.ParameterType == typeof(int))
		{
			method.Invoke(target, new object[]
			{
				animEvent.intParameter
			});
		}
		else if (parameterInfo.ParameterType == typeof(string))
		{
			method.Invoke(target, new object[]
			{
				animEvent.stringParameter
			});
		}
		else if (parameterInfo.ParameterType == typeof(UnityEngine.Object))
		{
			method.Invoke(target, new object[]
			{
				animEvent.objectReferenceParameter
			});
		}
	}
}
