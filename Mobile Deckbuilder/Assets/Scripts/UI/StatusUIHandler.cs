using System;
using System.Collections.Generic;
using UnityEngine;

public enum SensorType { Accelerometer, Microphone, Camera }
public class StatusUIHandler : MonoBehaviour
{
    public static StatusUIHandler Instance { get; private set; }
    public Dictionary<SensorType, Func<bool>> SensorStatus { get; private set; }


    [Header("BlindUI")]
    [SerializeField] private StatusUI blindUI;
    private Animator blindUIAnimator;

    [Header("FireUI")]
    [SerializeField] private StatusUI fireUI;

    [Header("WaterUI")]
    [SerializeField] private StatusUI waterUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        blindUIAnimator = blindUI.GetComponent<Animator>();

        Instance.SensorStatus = new Dictionary<SensorType, Func<bool>>
        {
            { SensorType.Accelerometer, GameManager.HasAccelerometer},
            { SensorType.Microphone, GameManager.HasMicrophone},
            { SensorType.Camera, GameManager.HasFrontCam}
        };
    }

    public static bool HasSensor(SensorType type)
    {
        return Instance.SensorStatus[type]();
    }

    public static void StartBlindUI()
    {
        if (!Instance.blindUI.gameObject.activeInHierarchy)
        {
            Instance.blindUI.gameObject.SetActive(true);
        }
        Instance.blindUIAnimator.SetTrigger("FadeIn");
    }

    public static void StopBlindUI()
    {
        Instance.blindUIAnimator.SetTrigger("FadeOut");
    }

    public static void StartFireUI()
    {
        Instance.fireUI.gameObject.SetActive(true);
    }

    public static void StopFireUI()
    {
        Instance.fireUI.gameObject.SetActive(false);
    }

    public static void StartWaterUI()
    {
        Instance.waterUI.gameObject.SetActive(true);
    }

    public static void StopWaterUI()
    {
        Instance.waterUI.gameObject.SetActive(false);
    }
}
