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
            waddles.SetActive(true);
            loading.SetActive(false);
            safe.SetActive(false);
            not_safe.SetActive(false);

            SetState(State.Idle);
        }

        public void OnButtonClicked() // Renamed from SetStateToLoading to be more descriptive
        {
            StartCoroutine(LoadingSequence());
        }

        private IEnumerator LoadingSequence()
        {
            SetState(State.Loading);

            yield return new WaitForSeconds(3f); // Wait for 2 seconds

            // Randomly select between safe and not safe
            //bool isSafe = Random.value > 0.5f; // Random.value returns float between 0 and 1
            SetState(State.ResultNotSafe);
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

            // Activate GameObjects based on the current state
            switch (currentState)
            {
                case State.Idle:
                    loading.SetActive(false);
                    safe.SetActive(false);
                    not_safe.SetActive(false);
                    break;

                case State.Loading:
                    loading.SetActive(true);
                    safe.SetActive(false);
                    not_safe.SetActive(false);
                    break;

                case State.ResultSafe:
                    loading.SetActive(false);
                    safe.SetActive(true);
                    not_safe.SetActive(false);
                    break;

                case State.ResultNotSafe:
                    loading.SetActive(false);
                    safe.SetActive(false);
                    not_safe.SetActive(true);
                    break;
            }
        }
    }

}
