using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResistStatusEffect : MonoBehaviour
{
    private BaseCharacter playerController;


    private void Awake()
    {
        playerController = GetComponent<BaseCharacter>();
    }

    public void TryResistStatus(StatusType effect, float resistTime, int stacks)
    {
        switch (effect)
        {
            case StatusType.Fire:
                StartCoroutine(BlowToResist(resistTime, stacks));
                break;
            case StatusType.Blind:
                StartCoroutine(CoverToResist(resistTime, stacks));
                break;
            case StatusType.Water:
                StartCoroutine(ShakeToResist(resistTime, stacks));
                break;
            default:
                playerController.AddStatusStack(effect, stacks);
                break;
        }
    }

    IEnumerator ShakeToResist(float resistTime, int stacks)
    {
        float timer = 0f;

        int shakesRequired = 3;
        int shakeCount = 0;
        float minShakeInterval = 0.2f;
        float lastShakeTime = -999f;

        // Optional: visual prompt
        StatusUIHandler.StartWaterUI();

        while (timer < resistTime && shakeCount < shakesRequired)
        {

            if (Time.time - lastShakeTime >= minShakeInterval && DetectShake())
            {
                shakeCount++;
                lastShakeTime = Time.time;
                Debug.Log("Shake detected! " + shakeCount);

                // Add a small delay to prevent multiple counts per shake
                yield return new WaitForSeconds(0.1f);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        StatusUIHandler.StopWaterUI();

        if (shakeCount >= shakesRequired)
        {
            Debug.Log("Water resisted!");
        }
        else
        {
            Debug.Log("Water not resisted.");
            playerController.AddStatusStack(StatusType.Water, stacks);
        }
    }

    IEnumerator CoverToResist(float resistTime, int stacks)
    {
        float timer = 0f;
        bool isCovered = false;
        float coverDuration = 0.1f;
        float coverTimer = 0f;


        WebCamTexture camTexture = null;

        StatusUIHandler.StartBlindUI();
        // Try to get the front camera
        if (GameManager.HasFrontCam(out camTexture)) 
        { 
            camTexture.Play();
            yield return new WaitForSeconds(0.5f); // Let it initialize
        }

        while (timer < resistTime && !isCovered)
        {
            timer += Time.deltaTime;

            if (GameManager.HasFrontCam(out camTexture) && camTexture != null && camTexture.didUpdateThisFrame)
            {
                Color32[] pixels = camTexture.GetPixels32();
                float avgBrightness = 0f;

                for (int i = 0; i < pixels.Length; i += 20) // Sample every 20th pixel for performance
                {
                    Color32 pixel = pixels[i];
                    avgBrightness += (pixel.r + pixel.g + pixel.b) / 3f / 255f;
                }

                avgBrightness /= (pixels.Length / 20f);

                if (avgBrightness < 0.5f) // Covered = low brightness
                {
                    coverTimer += Time.deltaTime;
                    Debug.Log(coverTimer);
                    if (coverTimer >= coverDuration)
                    {
                        isCovered = true;
                        break;
                    }
                }
                else
                {
                    coverTimer = 0f;
                }
            }
            else if (!GameManager.HasFrontCam() && Keyboard.current.cKey.isPressed)
            {
                coverTimer += Time.deltaTime;
                if (coverTimer >= coverDuration)
                {
                    isCovered = true;
                    break;
                }
            }

            yield return null;
        }

        StatusUIHandler.StopBlindUI();

        if (camTexture != null && camTexture.isPlaying)
            camTexture.Stop();

        if (isCovered)
        {
            Debug.Log("Blind resisted!");
        }
        else
        {
            Debug.Log("Blind not resisted.");
            playerController.AddStatusStack(StatusType.Blind, stacks);
        }
    }

    IEnumerator BlowToResist(float resistTime, int stacks)
    {
        float timer = 0f;

        bool blowDetected = false;

        StatusUIHandler.StartFireUI();
        // Try to start microphone recording
        AudioClip micInput = null;
        string MicDevice = string.Empty;
        if (GameManager.HasMicrophone(out MicDevice))
        {
            micInput = Microphone.Start(MicDevice, true, 1, 44100);
            yield return new WaitForSeconds(0.1f); // Let mic warm up
        }

        while (timer < resistTime && !blowDetected)
        {
            timer += Time.deltaTime;
            blowDetected = DetectBlow(micInput, MicDevice);
            yield return null;
        }

        // Stop microphone if we were using it
        if (micInput != null && Microphone.IsRecording(MicDevice))
            Microphone.End(MicDevice);

        StatusUIHandler.StopFireUI();

        // Result
        if (blowDetected)
        {
            Debug.Log("Fire resisted!");
        }
        else
        {
            Debug.Log("Fire not resisted.");
            playerController.AddStatusStack(StatusType.Fire, stacks);
        }
    }


    private bool DetectShake()
    {
        float shakeThreshold = 2.5f; // Tune this based on testing

        // If acceleration exceeds threshold, count as a shake
        bool shakeDetected = false;
        Accelerometer accelerometer = null;
        if (GameManager.HasAccelerometer(out accelerometer))
        {
            Vector3 acceleration = accelerometer.acceleration.ReadValue();
            if (acceleration.sqrMagnitude > shakeThreshold * shakeThreshold)
            {
                shakeDetected = true;
            }
        }
        else
        {
            // Fallback: press spacebar or left click rapidly
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                shakeDetected = true; 
            }
        }

        return shakeDetected;
    }

    private bool DetectBlow(AudioClip micInput, string MicDevice)
    {
        float micThreshold = 0.1f; // Adjust based on testing

       
        if (MicDevice != string.Empty && Microphone.IsRecording(MicDevice))
        {
            float[] samples = new float[128];
            int micPos = Microphone.GetPosition(MicDevice);
            int startPos = Mathf.Max(0, micPos - samples.Length);
            micInput.GetData(samples, startPos);

            float volume = 0f;
            foreach (float sample in samples)
            {
                volume += Mathf.Abs(sample);
            }

            volume /= samples.Length;
            Debug.Log(volume);

            if (volume > micThreshold)
            {
                return true;
            }
        }
        else
        {
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                Debug.Log("Blow fallback triggered (key press)");
                return true;
            }
        }
        return false;
    }

}
