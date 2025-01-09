using TOM.Common.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;

using Microsoft.MixedReality.Toolkit.Audio;


namespace TOM.Apps.Template
{

    public class TemplateUIController : MonoBehaviour
    {
        [SerializeField] private GameObject waddles;
        [SerializeField] private GameObject loading;
        [SerializeField] private GameObject textbox;
        
        private enum UIState {
            Idle,
            Loading,
            Result
        }

        private UIState currentState;

        // Start is called before the first frame update
        void Start()
        {
            setUIState(UiState.Idle);
        }

        private void SetUIState(UIState state) {
            currentState = state;

            waddles.SetActive(true);
            loading.SetActive(state = UIState.Loading);
            textbox.SetActive(state = UIState.Result);
        }

        public void ShowIdle() {
            SetUIState(UIState.Idle);
        }

        public void ShowLoading(string text) {
            SetUIState(UIState.Loading);
        }

        public void ShowResult(string text) {
            SetUIState(UIState.Result);
            UpdateText(text);
        }

        private void UpdateTest(string text) {
            TesxtMesh panelText = textbox.GetComponentInChildren<TextMesh.();
            if (panelText!= null) {
                panelText.text = text;
            }
        }

}
