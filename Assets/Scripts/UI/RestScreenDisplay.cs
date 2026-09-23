using System;
using UnityEngine;

public class RestScreenDisplay : MonoBehaviour
{
    [SerializeField] private GameObject restScreen;

    private void Awake()
    {
        restScreen.SetActive(false);
    }

    public void DisplayRestScreen()
    {
        restScreen.SetActive(true);
    }

    public void HideRestScreen()
    {
        restScreen.SetActive(false);
    }
}
