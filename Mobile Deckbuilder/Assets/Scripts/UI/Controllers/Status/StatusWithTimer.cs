using UnityEngine;
using UnityEngine.UI;

public class StatusWithTimer : StatusUI
{
    [SerializeField] private float timerDuration = 2f;
    private float timer;
    [SerializeField] private Image timerImage;

    protected override void OnEnable()
    {
        base.OnEnable();
        timer = timerDuration;
        timerImage.fillAmount = 1f;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerImage.fillAmount = timer / timerDuration;
        }
    }
}
