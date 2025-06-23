using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum TurnType { Player, Enemy, NoCombat }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerController player;

    public UnityAction<TurnType> TurnChanged;
    private TurnType turnType;

    public Accelerometer Accelerometer { get; private set; }
    public string micDevice { get; private set; } = string.Empty;
    public WebCamTexture CamTexture { get; private set; }

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
    }

    private void Start()
    {
        // Initialize the game state
        SetTurnType(TurnType.Player);
    }

    #region Getters and Setters

    /// <summary>
    /// Get the current turn type of the game (Player, Enemy, or NoCombat).
    /// </summary>
    /// <returns></returns>
    public static TurnType GetTurnType()
    {
        return Instance.turnType;
    }

    private static void SetTurnType(TurnType newType)
    {

        Instance.turnType = newType;

    }

    /// <summary>
    /// Change the current turn type and notify listeners if it has changed.
    /// </summary>
    /// <param name="newType"></param>
    public static void ChangeTurn(TurnType newType)
    {
        if (Instance.turnType != newType)
        {
            SetTurnType(newType);
            Instance.StartCoroutine(TurnDelay(newType));
        }
    }

    public static int GetActionPoints()
    {
        return Instance.player.GetActionPoints();
    }

    public static PlayerController GetPlayer()
    {
        return Instance.player;
    }

    private static IEnumerator TurnDelay(TurnType newType)
    {
        yield return new WaitForSeconds(1f);
        Instance.TurnChanged?.Invoke(newType);
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
        if (Instance.micDevice != string.Empty)
        {
            microphone = Instance.micDevice;
            return true;
        }
        if (Microphone.devices.Length > 0)
        {
            Instance.micDevice = Microphone.devices[0];
            microphone = Instance.micDevice;
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
        if(Instance.CamTexture != null)
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
