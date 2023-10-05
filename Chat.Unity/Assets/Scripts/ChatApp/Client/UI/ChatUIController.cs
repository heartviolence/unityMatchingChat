using System;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ChatApp.Client.UI
{
    public class ChatUIController : MonoBehaviour
    {
        [SerializeField] private ChatManager chatManager;

        private ListView listview;
        private TextField sendMessageField;
        private Button sendButton;
        private Button exitButton;

        private void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            listview = rootVisualElement.Q<ListView>("chatList");
            sendMessageField = rootVisualElement.Q<TextField>("sendMessage");
            sendButton = rootVisualElement.Q<Button>("sendButton");
            exitButton = rootVisualElement.Q<Button>("exitButton");

            listview.itemsSource = chatManager.ChatLog;
            chatManager.ChatLog.CollectionChanged += RefreshListView;

            sendButton.clicked += async () =>
            {
                await chatManager.SendMessageAsync(sendMessageField.value);
                sendMessageField.value = "";
            };

            exitButton.clicked += chatManager.ExitChatRoom;
        }

        private void OnDisable()
        {
            chatManager.ChatLog.CollectionChanged -= RefreshListView;
        }

        private void RefreshListView(object sender, NotifyCollectionChangedEventArgs args)
        {
            listview.RefreshItems();
        }
    }
}