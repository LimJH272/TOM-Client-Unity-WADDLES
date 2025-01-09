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

        [SerializeField] private GameObject safe;

        [SerializeField] private GameObject not_safe;

        private enum State {
            Idle,
            Loading,
            ResultSafe,
            ResultNotSafe
        }

        private State currentState;

        void Start() {
            SetState(State.Idle);
        }

        public void SetStateToIdle()
        {
            SetState(State.Idle);
        }

        public void SetStateToLoading()
        {
            SetState(State.Loading);
        }

        public void SetStateToResult(bool isSafe)
        {
            SetState(isSafe ? State.ResultSafe : State.ResultNotSafe);
        }

        private void SetState(State newState)
        {
            currentState = newState;

            waddles.SetActive(true);
            loading.SetActive(false);
            safe.SetActive(false);
            not_safe.SetActive(false);

            // Activate GameObjects based on the current state
            switch (currentState)
            {
                case State.Idle:
                    break;

                case State.Loading:
                    loading.SetActive(true);
                    break;

                case State.ResultSafe:
                    safe.SetActive(true);
                    break;

                case State.ResultNotSafe:
                    not_safe.SetActive(true);
                    break;
            }
        }
    }

}
