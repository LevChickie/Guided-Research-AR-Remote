using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ReceiveAndUseDepthValues : MonoBehaviour
{
    [SerializeField]
    private Text depthText;
    private AROcclusionManager occlusionManager;

    public ARCameraManager CameraManager
    {
        get => _cameraManager;
        set => _cameraManager = value;
    }

    [SerializeField]
    [Tooltip("The ARCameraManager which will produce camera frame events.")]
    private ARCameraManager _cameraManager;


    public AROcclusionManager OcclusionManager
    {
        get => _occlusionManager;
        set => _occlusionManager = value;
    }

    [SerializeField]
    [Tooltip("The AROcclusionManager which will produce depth textures.")]
    private AROcclusionManager _occlusionManager;

    public RawImage RawImage
    {
        get => _rawImage;
        set => _rawImage = value;
    }

    [SerializeField]
    [Tooltip("The UI RawImage used to display the image on screen.")]
    private RawImage _rawImage;


    void Start()
    {
         occlusionManager = this.gameObject.transform.GetChild(0).gameObject.GetComponent<AROcclusionManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            _rawImage.gameObject.SetActive(true);
            using (image)
            {
                        //Check for the number of planes
                        //Assert.IsTrue(image.planeCount == 1);
                        var plane = image.GetPlane(0);
                        //get plane information - depth, and pixel locations
                        var dataLength = plane.data.Length;
                        var pixelStride = plane.pixelStride;
                        var rowStride = plane.rowStride;
                        var centerRowIndex = dataLength / rowStride / 2;
                        var centerPixelIndex = rowStride / pixelStride / 2;
                        var centerPixelData = plane.data.GetSubArray(centerRowIndex * rowStride + centerPixelIndex * pixelStride, pixelStride);
                        var depthInMeters = convertPixelDataToDistanceInMeters(centerPixelData.ToArray(), image.format);
                        if(depthInMeters != null)
                        {
                            depthText.text = "depth is " + depthInMeters + " m";
                            print($"depth texture size: ({image.width},{image.height}), pixelStride: {pixelStride}, rowStride: {rowStride}, pixel pos: ({centerPixelIndex}, {centerRowIndex}), depthInMeters of the center pixel: {depthInMeters}");
                        }
                        else
                        {
                            depthText.text = "depth api not supported here";
                        }
                UpdateRawImage(_rawImage, image);
            }
        }
        else
        {
            depthText.text = "depth api not supported";
            _rawImage.gameObject.SetActive(false);
        }
    }

    float convertPixelDataToDistanceInMeters(byte[] data, XRCpuImage.Format format)
    {
        switch (format)
        {
            case XRCpuImage.Format.DepthUint16:
                return BitConverter.ToUInt16(data, 0) / 1000f;
            case XRCpuImage.Format.DepthFloat32:
                return BitConverter.ToSingle(data, 0);
            default:
                throw new Exception($"Format not supported: {format}");
        }
    }
    private static void UpdateRawImage(RawImage rawImage, XRCpuImage cpuImage)
    {
        // Get the texture associated with the UI.RawImage that we wish to display on screen.
        var texture = rawImage.texture as Texture2D;

        // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    and human stencil buffers.
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
            rawImage.texture = texture;
        }

        // For display, we need to mirror about the vertical access.
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), XRCpuImage.Transformation.MirrorY);

        //Debug.Log("Texture format: " + cpuImage.format.AsTextureFormat()); -> RFloat

        // Get the Texture2D's underlying pixel buffer.
        var rawTextureData = texture.GetRawTextureData<byte>();

        // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
        Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
            "The Texture2D is not the same size as the converted data.");

        // Perform the conversion.
        cpuImage.Convert(conversionParams, rawTextureData);

        // "Apply" the new pixel data to the Texture2D.
        texture.Apply();

        // Get the aspect ratio for the current texture.
        var textureAspectRatio = (float)texture.width / texture.height;

        // Determine the raw image rectSize preserving the texture aspect ratio, matching the screen orientation,
        // and keeping a minimum dimension size.
        const float minDimension = 480.0f;
        var maxDimension = Mathf.Round(minDimension * textureAspectRatio);
        var rectSize = new Vector2(maxDimension, minDimension);
        //var rectSize = new Vector2(minDimension, maxDimension);   //Portrait
        rawImage.rectTransform.sizeDelta = rectSize;
    }
}
