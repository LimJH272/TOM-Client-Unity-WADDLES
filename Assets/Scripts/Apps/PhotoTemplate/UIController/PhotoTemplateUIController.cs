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

    public class PhotoTemplateUIController : MonoBehaviour
    {
        [SerializeField] private GameObject TemplateUI;
        [SerializeField] private TextMesh panelText;
        // [SerializeField] private GameObject imageDisplayRenderer;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ResetUI()
        {
            Debug.Log("ResetUI");
            UpdateText("");
            // imageDisplayRenderer.SetActive(false);
        }

        public void UpdateText(string text)
        {
            if (panelText != null) 
            {   
                panelText.text = text;
            }
        }

        // public void SetImage(byte[] image)
        // {
        //     Debug.Log("SetImage");
        //     Texture2D texture = new Texture2D(2, 2);

        //     if (texture.LoadImage(image))
        //     {
        //         imageDisplayRenderer.SetActive(true);
        //         RawImage img = imageDisplayRenderer.GetComponentInChildren<RawImage>();
        //         img.texture = texture;
        //     }
        //     else
        //     {
        //         imageDisplayRenderer.SetActive(false);
        //         Debug.LogError("Failed to load image");
        //     }
        // }

    }

}
