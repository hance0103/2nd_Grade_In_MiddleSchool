using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    private Dictionary<string, int> animCache = new Dictionary<string, int>();

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // 상태 이름 해시 미리 캐싱
        animCache["Idle"] = Animator.StringToHash("Idle");
        animCache["Move"] = Animator.StringToHash("Move");
        animCache["Jump"] = Animator.StringToHash("Jump");
        animCache["Attack"] = Animator.StringToHash("Attack");
    }

    public void PlayAnimation(string name)
    {
        if (animCache.TryGetValue(name, out int hash))
        {
            animator.Play(hash);
        }
        else
        {
            Debug.LogWarning($"Animation '{name}' not found in cache!");
        }
    }

    public void PlayAnimationCrossFade(string name, float fadeTime = 0.1f)
    {
        if (animCache.TryGetValue(name, out int hash))
        {
            animator.CrossFade(hash, fadeTime);
        }
    }
}
