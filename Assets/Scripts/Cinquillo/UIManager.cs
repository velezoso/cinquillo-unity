using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.Cinquillo
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text mainText;
        [SerializeField] GameObject menuGO;
        IWorldManager worldManager;
        int numberOfPlayers;

        void Awake()
        {
            Assert.IsNotNull(mainText, "ERROR olvidaste texto");
            Assert.IsNotNull(menuGO, "ERROR olvidaste menú");
            mainText.gameObject.SetActive(false);
        }

        internal void Setup(IWorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public void Play()
        {
            menuGO.SetActive(false);
            worldManager.Play(numberOfPlayers);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        internal void GameFinished(string text, float duration)
        {
            DisplayText(text, duration);
            menuGO.SetActive(true);
        }

        internal void DisplayText(string text, float duration)
        {
            // Debug.Log($"{text} - {duration}");
            StartCoroutine(ShowTextCoroutine(text, duration));
        }

        IEnumerator ShowTextCoroutine(string text, float duration)
        {
            mainText.gameObject.SetActive(true);
            mainText.text = text;
            yield return new WaitForSeconds(duration);
            mainText.gameObject.SetActive(false);
            worldManager.TextDisplayed();
        }

        public void OnValueChanged(int value)
        {
            numberOfPlayers = value;
        }
    }
}