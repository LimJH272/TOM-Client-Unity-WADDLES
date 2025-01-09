﻿using TOM.Common.UI;

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
        /*
        [SerializeField] private GameObject TemplateUI;
        [SerializeField] private TextMesh panelText;
        [SerializeField] private GameObject imageDisplayRenderer;
        [SerializeField] private AudioSource audioSource;

        
        private string previousAudio;

        // Start is called before the first frame update
        void Start()
        {
            previousAudio = "";
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ResetUI()
        {
            Debug.Log("ResetUI");
            UpdateText("");
            imageDisplayRenderer.SetActive(false);
            audioSource.Stop();
        }

        public void UpdateText(string text)
        {
            panelText.text = text;
        }

        public void SetImage(byte[] image)
        {
            Debug.Log("SetImage");
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(image))
            {
                imageDisplayRenderer.SetActive(true);
                RawImage img = imageDisplayRenderer.GetComponentInChildren<RawImage>();
                img.texture = texture;
            }
            else
            {
                imageDisplayRenderer.SetActive(false);
                Debug.LogError("Failed to load image");
            }
        }

        public void PlayAudio(string audioName)
        {
            Debug.Log("PlayAudio");
            // Guard clause to prevent same audio from replaying prematurely
            if (previousAudio == audioName && audioSource.isPlaying)
            {
                return;
            }

            audioSource.Stop();
            AudioClip clip = LoadAudioClip(audioName);
            audioSource.clip = clip;
            audioSource.Play();
        }

        private AudioClip LoadAudioClip(string audioName)
        {
            // Instructions for new users: songs and soundtracks should all sit in the Assets/Resources/Audio folder by convention
            string path = "Audio/" + audioName;
            AudioClip audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio clip from path: " + path);
                return null;
            }

            Debug.Log("Successfully loaded clip from path: " + path);
            return audioClip;
        }
        
        */
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
