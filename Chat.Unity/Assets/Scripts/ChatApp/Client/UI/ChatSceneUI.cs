using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChatApp.Client.UI
{
    public class ChatSceneUI : MonoBehaviour
    {
        public static ChatSceneUI instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            GoToMainMenu();
        }

        [SerializeField] private GameObject chatUI;
        [SerializeField] private GameObject mainMenuUI;

        [SerializeField] private GameObject matchingWaitUI;


        public void GoToMainMenu()
        {
            DeactiveateAll();
            mainMenuUI.SetActive(true);
        }

        public void GoToMatchingWaiting()
        {
            DeactiveateAll();
            mainMenuUI.SetActive(true);
            matchingWaitUI.SetActive(true);
        }

        public void GoToChatRoom()
        {
            DeactiveateAll();
            chatUI.SetActive(true);
        }

        private void DeactiveateAll()
        {
            chatUI.SetActive(false);
            mainMenuUI.SetActive(false);
            matchingWaitUI.SetActive(false);
        }
    }
}