using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafetyGreyCameraUI : MonoBehaviour
{
    public Material greyCameraMaterial;
    public Transform planeTransform;
    //public RawImage rawImage;

    int imageWidth;
    int imageHeight;
    bool outBUdate = true;
    uint outCurrFrameIndex = 0;
    ulong outFrameExposureNano = 0;
    byte[] outLeftFrameData;
    byte[] outRightFrameData;
    float[] trArray;
    TextureFormat textureFormat = TextureFormat.Alpha8;
    Texture2D textureTemp;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        //imageWidth = (int)API_Module_Device.Current.FishEyeResolution.x;
        //imageHeight = (int)API_Module_Device.Current.FishEyeResolution.y;
        outBUdate = true;
        outCurrFrameIndex = 0;
        outFrameExposureNano = 0;
        outLeftFrameData = new byte[imageWidth * imageHeight];
        outRightFrameData = new byte[imageWidth * imageHeight];
        trArray = new float[7];
        textureFormat = TextureFormat.Alpha8;

    }
    
    // Update is called once per frame
    private void LateUpdate()
    {
        //if (!GSXRManager.Instance.Initialized)
        //{
        //    return;
        //}

        this.transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized * 0.4f;
        this.transform.rotation = Camera.main.transform.rotation;
        planeTransform.gameObject.transform.localScale = new Vector3(imageWidth / 8f, 1f, imageHeight / 8f);

        if (Application.platform == RuntimePlatform.Android)
        {

            //API_GSXR_Slam.GSXR_Get_LatestFishEyeBinocularData(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outLeftFrameData, outRightFrameData);
            //GSXRPluginAndroid.GSXRGetLatestCameraFrameData(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outLeftFrameData, trArray);
            if (outBUdate)
            {
                Debug.Log("outBUdate == true");
                GetTexture(outLeftFrameData);
                greyCameraMaterial.mainTexture = textureTemp;
                //rawImage.texture = textureTemp;
            }
            else
            {
                Debug.Log("Error: Please Check Slamconfig prop: gUseXXXCamera = true");
            }
        }
    }

    public Texture2D GetTexture(byte[] outFrameData)
    {
        if (textureTemp == null)
        {
            textureTemp = new Texture2D(imageWidth, imageHeight, textureFormat, false);
        }
        textureTemp.LoadRawTextureData(outFrameData);
        textureTemp.Apply();
        return textureTemp;
    }
}
