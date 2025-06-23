using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public  class levelLoader : MonoBehaviour
{
    public static levelLoader Instance;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static void LoadNewScene(string sceneName)
    {
        Instance.StartCoroutine(CrossfadeCoroutine(sceneName));
    }

    private static  IEnumerator CrossfadeCoroutine(string sceneName)
    {
        Animator animator = GetAnimator();
        animator.SetTrigger("StartFade");
        var info = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);
        SceneManager.LoadScene("MainScene");
    }

    private static Animator GetAnimator()
    {
        if(Instance.animator == null)
        {
            Instance.animator = Instance.GetComponent<Animator>();
        }
        return Instance.animator;
    }
}
