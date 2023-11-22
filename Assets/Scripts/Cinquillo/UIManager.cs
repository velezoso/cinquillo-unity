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
        [SerializeField] WorldManager worldManager;
        int numberOfPlayers;

        void Awake()
        {
            Assert.IsNotNull(mainText, "ERROR olvidaste texto");
            Assert.IsNotNull(worldManager, "ERROR olvidaste worldManager");
            Assert.IsNotNull(menuGO, "ERROR olvidaste menú");
            mainText.gameObject.SetActive(false);
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
            ShowText(text, duration);
            menuGO.SetActive(true);
        }

        internal void ShowText(string text, float duration)
        {
            StartCoroutine(ShowTextCoroutine(text, duration));
        }

        IEnumerator ShowTextCoroutine(string text, float duration)
        {
            mainText.gameObject.SetActive(true);
            mainText.text = text;
            yield return new WaitForSeconds(duration);
            mainText.gameObject.SetActive(false);
        }

        public void OnValueChanged(int value)
        {
            numberOfPlayers = value;
        }
    }
}