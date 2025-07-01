using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum TurnType { Player, Enemy, NoCombat }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerController player;

    public UnityAction<TurnType, TurnType> TurnChanged;
    private static TurnType turnType = TurnType.NoCombat;

    public Accelerometer Accelerometer { get; private set; }
    public string MicDevice { get; private set; } = string.Empty;
    public WebCamTexture CamTexture { get; private set; }

    public static Action StartCombat;
    public static Action EndCombat;
    public static Action LoadNewScene;

    public static bool isBossFight = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    #region Getters and Setters

    /// <summary>
    /// Get the current turn type of the game (Player, Enemy, or NoCombat).
    /// </summary>
    /// <returns></returns>
    public static TurnType GetTurnType()
    {
        return turnType;
    }

    private static void SetTurnType(TurnType newType)
    {
        turnType = newType;
    }

    /// <summary>
    /// Change the current turn type and notify listeners if it has changed.
    /// </summary>
    /// <param name="newType"></param>
    public static void ChangeTurn(TurnType newType)
    {
        TurnType oldType = turnType;
        if (oldType != newType)
        {
            SetTurnType(newType);
            Instance.TurnChanged?.Invoke(oldType, newType);
        }
    }

    public static void ChangeTurnDelayed(TurnType newType)
    {
        TurnType oldType = turnType;
        if (oldType != newType)
        {
            SetTurnType(newType);
            Instance.StartCoroutine(TurnDelay(oldType, newType));
        }
    }

    public static int GetActionPoints()
    {
        return Instance.player.GetActionPoints();
    }
    public static void SetPlayer(PlayerController newPlayer)
    {
        Instance.player = newPlayer;
    }

    public static PlayerController GetPlayer()
    {
        if(Instance.player == null)
        {
            Instance.player = FindAnyObjectByType<PlayerController>();
        }
        return Instance.player;
    }

    public static void OnStartCombat()
    {
        ChangeTurn(TurnType.Player);
        StartCombat?.Invoke();
        Debug.Log("Start Combat");
    }

    public static void EndBossFight()
    {

    }

    public static void OnEndCombat()
    {
        ChangeTurn(TurnType.NoCombat);
        EndCombat?.Invoke();
        Debug.Log("End Combat");
        CardManager.Instance.StartCardPick(3, new Vector2(Screen.width / 2, Screen.height / 2));
        CardManager.Instance.PickEnded += Instance.ReturnToMap;
    }

    public static void LoadScene(string sceneName)
    {
        LoadNewScene?.Invoke();
        levelLoader.LoadNewScene(sceneName);
    }

    private void ReturnToMap()
    {
        CardManager.Instance.PickEnded -= Instance.ReturnToMap;
        LoadScene("MapScene");
    }

    private static IEnumerator TurnDelay(TurnType oldType, TurnType newType)
    {
        yield return new WaitForSeconds(1f);
        Instance.TurnChanged?.Invoke(oldType, newType);
    }

    public static bool HasAccelerometer(out Accelerometer accelerometer)
    {
        Instance.Accelerometer ??= Accelerometer.current;
        accelerometer = Instance.Accelerometer;
        return Instance.Accelerometer != null;
    }

    public static bool HasAccelerometer()
    {
        return HasAccelerometer(out _);
    }

    public static bool HasMicrophone(out string microphone)
    {
        if (Instance.MicDevice != string.Empty)
        {
            microphone = Instance.MicDevice;
            return true;
        }
        if (Microphone.devices.Length > 0)
        {
            Instance.MicDevice = Microphone.devices[0];
            microphone = Instance.MicDevice;
            return true;
        }
        else
        {
            microphone = null;
            return false;
        }
    }

    public static bool HasMicrophone()
    {
        return HasMicrophone(out _);
    }

    public static bool HasFrontCam(out WebCamTexture camTexture)
    {
        if (Instance.CamTexture != null)
        {
            camTexture = Instance.CamTexture;
            return true;
        }
        WebCamDevice? frontCam = null;
        foreach (var device in WebCamTexture.devices)
        {
            if (device.isFrontFacing)
            {
                frontCam = device;
                break;
            }
        }

        if (frontCam.HasValue)
        {
            Instance.CamTexture = new WebCamTexture(frontCam.Value.name, 160, 120);
            camTexture = Instance.CamTexture;
            return true;
        }
        camTexture = null;
        return false;
    }

    public static bool HasFrontCam()
    {
        return HasFrontCam(out _);
    }
    #endregion
}
