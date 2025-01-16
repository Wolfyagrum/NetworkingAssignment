using System;
using TMPro;
using UnityEngine;

public class PointUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI points;

    private void Start()
    {
        GameManager.instance.Points.OnValueChanged += UpdatePoints;
    }

    private void OnDestroy()
    {
        GameManager.instance.Points.OnValueChanged -= UpdatePoints;
    }

    private void UpdatePoints(int previousValue, int newValue)
    {
        points.text = "Points = " + newValue;
    }
}
