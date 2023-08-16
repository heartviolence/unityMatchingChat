using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ChatApp.Client.UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] private ChatMatchingManager chatMatchingManager;

        private Button matchingButton;
        private TextField usernameField;

        private void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

            matchingButton = rootVisualElement.Q<Button>("matchingButton");
            usernameField = rootVisualElement.Q<TextField>("usernameField");

            matchingButton.clicked += () => { chatMatchingManager.MatchingAsync(usernameField.value); };
        }
    }
}