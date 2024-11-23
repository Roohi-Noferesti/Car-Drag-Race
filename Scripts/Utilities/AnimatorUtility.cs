using System.Collections;
using UnityEngine;

public static class AnimUtility
{
    public static IEnumerator CheckAnimationReachEnd(Animator animator,string animationName)
    {
        var endStateReached = false;
        while (!endStateReached)
        {
            if (!animator.IsInTransition(0))
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                endStateReached =
                    stateInfo.IsName(animationName)
                    && stateInfo.normalizedTime >= 1;
            }

            yield return new WaitForEndOfFrame();
        }
    }


    public static bool IsFinished(this Animator animator, string animationName,float timeToFinish = 1)
    {
        if (!animator.IsInTransition(0))
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName(animationName)
                && stateInfo.normalizedTime >= timeToFinish;
        }
        return false;
    }
}
