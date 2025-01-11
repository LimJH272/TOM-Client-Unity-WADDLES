using TOM.Common.Communication;
using TOM.Common.Utils;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.Audio;
using Google.Protobuf;

using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TOM.Common.Config;



#if BUILD_XREAL
using NRKernal;
using NRKernal.Record;
#endif

namespace TOM.Apps.Template
{

    public class PhotoTemplateController : HomeController
    {
        public SocketCommunication socketCommunication;

        public PhotoTemplateUIController templateUIController;

        private const float REQUEST_GAP_SECONDS = 5f;

        /// <summary> The photo capture object. </summary>
        private NRPhotoCapture m_PhotoCaptureObject;

        // /// <summary> The camera resolution. </summary>
        // private Resolution m_CameraResolution;

        private int m_CameraWidth = 1280;
        private int m_CameraHeight = 720;

        private bool isOnPhotoProcess = false;


        // Start is called before the first frame update
        void Start()
        {
            templateUIController.ResetUI();
        }



        // Update is called once per frame
        void Update()
        {
            handleCommunication();
        }


        private void SendImageFrameData(byte[] cameradata, int width, int height, int cmd, string param)
        {
            // Debug.Log("SendImageFrameData: " + cameraFrame.width + ", " + cameraFrame.height + ", " + cameraFrame.data.Length);
            ImageFrameData imageFrameData = new ImageFrameData
            {
                Width = width,
                Height = height,
                Cmd = cmd,
                Params = param,
            };
            imageFrameData.Image.Add(ByteString.CopyFrom(cameradata));

            SocketData socketData = new SocketData();
            socketData.DataType = DataTypes.PHOTO_TEMPLATE_IMAGE_FRAME_DATA;
            socketData.Data = ByteString.CopyFrom(imageFrameData.ToByteArray());

            socketCommunication.SendMessages(socketData.ToByteArray());
        }

        private void handleCommunication()
        {
            if (socketCommunication.DataReceived())
            {
                List<byte[]> messages = socketCommunication.GetMessages();
                foreach (byte[] message in messages)
                {
                    ProcessDataBytes(message);
                }
            }
        }

        private bool ProcessDataBytes(byte[] byteData)
        {
            try
            {
                Debug.Log("ProcessDataBytes");
                SocketData socketData = SocketData.Parser.ParseFrom(byteData);

                int dataType = socketData.DataType;
                ByteString data = socketData.Data;

                if (dataType == DataTypes.TEMPLATE_DATA)
                {
                    try
                    {
                        TemplateData templateData = TemplateData.Parser.ParseFrom(data);
                        UpdateTemplateUI(dataType, templateData);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Bytes received cannot be decoded as TemplateData: " + e.Message);
                        return false;
                    }

                }
                else
                {
                    Debug.LogError("Datatype is not supported: " + dataType);
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Bytes received are not SocketData: " + e.Message + ", " +
                               System.Text.Encoding.UTF8.GetString(byteData));
                return false;
            }
        }

        private void UpdateTemplateUI(int dataType, TemplateData templateData)
        {
            Debug.Log("TemplateData:\n" +
                      "dataType: " + dataType + "\n" +
                      "text_message: " + templateData.TextMessage + "\n" +
                      "image: " + templateData.Image.ToByteArray() + "\n" +
                      "audio_path: " + templateData.AudioPath + "\n");

            templateUIController.UpdateText(templateData.TextMessage);
            // templateUIController.SetImage(templateData.Image.ToByteArray());
            // templateUIController.PlayAudio(templateData.AudioPath);
        }


        /// <summary> Use this for initialization. </summary>
        void Create(Action<NRPhotoCapture> onCreated)
        {
            if (m_PhotoCaptureObject != null)
            {
                VisualLog.Log("The NRPhotoCapture has already been created.");
                return;
            }

            // Create a PhotoCapture object
            NRPhotoCapture.CreateAsync(false, delegate (NRPhotoCapture captureObject)
            {
                // m_CameraResolution = NRPhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

                if (captureObject == null)
                {
                    VisualLog.Log("Can not get a captureObject.");
                    return;
                }

                m_PhotoCaptureObject = captureObject;

                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.cameraResolutionWidth = m_CameraWidth; // m_CameraResolution.width;
                cameraParameters.cameraResolutionHeight = m_CameraHeight; // m_CameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.JPEG; // BGRA32;
                cameraParameters.frameRate = NativeConstants.RECORD_FPS_DEFAULT;
                cameraParameters.blendMode = BlendMode.RGBOnly;

                // Activate the camera
                Debug.Log("Start PhotoMode Async"); 
                m_PhotoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (NRPhotoCapture.PhotoCaptureResult result)
                {
                    // NRDebugger.Info("Start PhotoMode Async");
                    if (result.success)
                    {
                        onCreated?.Invoke(m_PhotoCaptureObject);
                    }
                    else
                    {
                        isOnPhotoProcess = false;
                        this.CloseCamera();
                        VisualLog.Log("Start PhotoMode faild." + result.resultType);
                    }
                }, true);
            });
        }

        /// <summary> Take a photo. </summary>
        public void TakeAPhoto()
        {
            Debug.Log("TakeAPhoto");    
            if (isOnPhotoProcess)
            {
                NRDebugger.Warning("Currently in the process of taking pictures, Can not take photo .");
                return;
            }

            isOnPhotoProcess = true;
            if (m_PhotoCaptureObject == null)
            {
                this.Create((capture) =>
                {
                    capture.TakePhotoAsync(OnCapturedPhotoToMemory);
                });
            }
            else
            {
                m_PhotoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
        }

        /// <summary> Executes the 'captured photo memory' action. </summary>
        /// <param name="result">            The result.</param>
        /// <param name="photoCaptureFrame"> The photo capture frame.</param>
        void OnCapturedPhotoToMemory(NRPhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            // List<byte> imageBufferList = new List<byte>();
            // photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            
            // var targetTexture = new Texture2D(m_CameraWidth, m_CameraHeight);
            // // Copy the raw image data into our target texture
            // photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            // Create a gameobject that we can apply our texture to
            // GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            // Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
            // quadRenderer.material = new Material(Resources.Load<Shader>("Record/Shaders/CaptureScreen"));

            // var headTran = NRSessionManager.Instance.NRHMDPoseTracker.centerAnchor;
            // quad.name = "picture";
            // quad.transform.localPosition = headTran.position + headTran.forward * 3f;
            // quad.transform.forward = headTran.forward;
            // quad.transform.localScale = new Vector3(1.6f, 0.9f, 0);
            // quadRenderer.material.SetTexture("_MainTex", targetTexture);

            // Debug.Log("Pic Saved, " + imageBufferList.Count);

            // PhotoCameraFrame cameraFrame = new PhotoCameraFrame(m_CameraWidth, m_CameraHeight);
            // cameraFrame.data = targetTexture.EncodeToJPG();
            SendImageFrameData(photoCaptureFrame.data, m_CameraWidth, m_CameraHeight, 1, "latitude:37.788132839912215;longitude:-122.40753565325528");
            // SaveTextureAsPNG(targetTexture);

            // SaveTextureToGallery(targetTexture);
            // Release camera resource after capture the photo.
            this.CloseCamera();
        }

        // void SaveTextureAsPNG(Texture2D _texture)
        // {
        //     try
        //     {
        //         string filename = string.Format("Nreal_Shot_{0}.png", NRTools.GetTimeStamp().ToString());
        //         string path = string.Format("{0}/NrealShots", Application.persistentDataPath);
        //         string filePath = string.Format("{0}/{1}", path, filename);

        //         byte[] _bytes = _texture.EncodeToPNG();
        //         NRDebugger.Info("Photo capture: {0}Kb was saved to [{1}]",  _bytes.Length / 1024, filePath);
        //         if (!Directory.Exists(path))
        //         {
        //             Directory.CreateDirectory(path);
        //         }
        //         File.WriteAllBytes(string.Format("{0}/{1}", path, filename), _bytes);

        //     }
        //     catch (Exception e)
        //     {
        //         NRDebugger.Error("Save picture faild!");
        //         throw e;
        //     }
        // }

        /// <summary> Closes this object. </summary>
        void CloseCamera()
        {
            if (m_PhotoCaptureObject == null)
            {
                NRDebugger.Error("The NRPhotoCapture has not been created.");
                return;
            }
            // Deactivate our camera
            m_PhotoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        /// <summary> Executes the 'stopped photo mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedPhotoMode(NRPhotoCapture.PhotoCaptureResult result)
        {
            // Shutdown our photo capture resource
            m_PhotoCaptureObject?.Dispose();
            m_PhotoCaptureObject = null;
            isOnPhotoProcess = false;
        }

        /// <summary> Executes the 'destroy' action. </summary>
        void OnDestroy()
        {
            // Shutdown our photo capture resource
            m_PhotoCaptureObject?.Dispose();
            m_PhotoCaptureObject = null;
        }

        // public void SaveTextureToGallery(Texture2D _texture)
        // {
        //     try
        //     {
        //         string filename = string.Format("Nreal_Shot_{0}.png", NRTools.GetTimeStamp().ToString());
        //         byte[] _bytes = _texture.EncodeToPNG();
        //         NRDebugger.Info(_bytes.Length / 1024 + "Kb was saved as: " + filename);
        //         if (galleryDataTool == null)
        //         {
        //             galleryDataTool = new GalleryDataProvider();
        //         }

        //         galleryDataTool.InsertImage(_bytes, filename, "Screenshots");
        //     }
        //     catch (Exception e)
        //     {
        //         NRDebugger.Error("[TakePicture] Save picture faild!");
        //         throw e;
        //     }
        // }
        

    }

}
