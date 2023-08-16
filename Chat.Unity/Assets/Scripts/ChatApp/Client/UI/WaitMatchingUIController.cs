using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ChatApp.Client;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;


public class WaitMatchingUIController : MonoBehaviour
{
    [SerializeField] private ChatMatchingManager chatMatchingManager;

    //Matching... 텍스트 라벨
    private Label waitLabel;

    private readonly string[] waitTexts = new[]
    {
        "Matching",
        "Matching.",
        "Matching..",
        "Matching..."
    };

    //취소버튼
    private Button cancelButton;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        waitLabel = rootVisualElement.Q<Label>("waitLabel");
        cancelButton = rootVisualElement.Q<Button>("cancelButton");

        cancelButton.clicked += () => { chatMatchingManager.DisconnectAsync(); };

        StartCoroutine(UpdateWaitLabel());
    }

    IEnumerator UpdateWaitLabel()
    {
        int lastIndex = waitTexts.Length;
        int currentIndex = 0;

        while (true)
        {
            waitLabel.text = waitTexts[currentIndex++];

            if (currentIndex >= lastIndex)
            {
                currentIndex = 0;
            }

            yield return new WaitForSeconds(0.8f);
        }
    }
}