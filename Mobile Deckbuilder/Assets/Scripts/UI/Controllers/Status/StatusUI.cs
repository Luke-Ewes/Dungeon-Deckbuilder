using System;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField, TextArea] private string PhoneMessage;
    [SerializeField, TextArea] private string PCMessage;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private SensorType sensorType;

    protected virtual void OnEnable()
    {
        text.text = StatusUIHandler.HasSensor(sensorType) ? PhoneMessage : PCMessage;
    }
}