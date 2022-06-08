using SC.XR.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.SDK;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Skyworth.Interaction.SafetyArea
{
    public class SafetyAreaManager : SingletonMono<SafetyAreaManager>
    {
        private class XRNodeLongPress
        {
            private const float PRESSTIME = 1.0f;
            private float starttime;
            private bool saved;
            private XRNode mXRNode;
            public XRNodeLongPress(XRNode node)
            {
                mXRNode = node;
            }
            public void OnLongPress(Action press)
            {
                InputDevice inputDevice = InputDevices.GetDeviceAtXRNode(mXRNode);
                if (inputDevice.isValid)
                {
                    if (inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonvalue) && menuButtonvalue)
                    {
                        if (starttime == 0) starttime = Time.time;
                        if (Time.time - starttime > PRESSTIME && !saved)
                        {
                            starttime = 0;
                            saved = true;

                            press();
                        }
                    }
                    else
                    {
                        starttime = 0;
                        saved = false;
                    }
                }
                else
                {
                    starttime = 0;
                    saved = false;
                }
            }
            public void TryRecenter() 
            {
                var inputsystem = new List<XRInputSubsystem>();
                SubsystemManager.GetInstances(inputsystem);
                bool statue = false;
                for (int i = 0; i < inputsystem.Count; i++)
                {
                    statue = inputsystem[i].TryRecenter();
                }
                if (statue)
                {
                    Camera.main.transform.rotation = Quaternion.identity;
                    Camera.main.transform.position = Vector3.zero;
                }
            }
        }

        private XRNodeLongPress LeftHandLongpress = new XRNodeLongPress(XRNode.LeftHand);
        private XRNodeLongPress RgihtHandLongpress = new XRNodeLongPress(XRNode.RightHand);
        //是否已从配置文件中读取安全区域
        private static bool hasReadFromFile = false;

        public Action OnBeginSetSafeArea;
        public Action OnFinishSetSafeArea;

        public Action OnEnterSafetyArea;
        public Action OnExitSafetyArea;

        private Dictionary<SafetyAreaStepEnum, ISafetyAreaStep> areaStepDic;

        private GameObject safetyAreaGameObject;
        private SafetyAreaMono safetyAreaMono;
        private ISafetyAreaStep currentStep;

        [HideInInspector]
        public StationaryAreaMono stationaryAreaMono;
        private GameObject stationaryAreaObject;
        [HideInInspector]
        public PlayAreaMono playAreaMono;
        private GameObject playAreaObject;

        [HideInInspector]
        public bool isDisableSafetyArea = false;
        [HideInInspector]
        public bool isSettingSafetyArea = false;
        [HideInInspector]
        public bool onlySettingGroundHeight = false;

        private bool showAreaWhenBowHead = true;
        private float originAlphaParam = 0.4f;
        private int originSafetyAreaColorIndex = 0;

        private SafetyAreaInfo currentSafetyAreaInfo = null;
        private bool alreayExitSafetyArea = false;

        public bool IsDisableSafetyArea
        {
            get
            {
                return isDisableSafetyArea;
            }
            set
            {
                isDisableSafetyArea = value;
            }
        }

        public bool ShowAreaWhenBowHead
        {
            get
            {
                if (!hasReadFromFile)
                {
                    LoadSafetyArea();
                }
                return showAreaWhenBowHead;
            }
            set
            {
                showAreaWhenBowHead = value;
                SaveSafetyArea();
            }
        }

        public int OriginSafetyAreaColorIndex
        {
            get
            {
                if (!hasReadFromFile)
                {
                    LoadSafetyArea();
                }
                return originSafetyAreaColorIndex;
            }
            set
            {
                originSafetyAreaColorIndex = value;
                SaveSafetyArea();
            }
        }

        public float OriginAlphaParam
        {
            get
            {
                if (!hasReadFromFile)
                {
                    LoadSafetyArea();
                }
                return originAlphaParam;
            }
            set
            {
                originAlphaParam = value;
                SaveSafetyArea();
            }
        }

        //public Color SafetyAreaColor
        //{
        //    get
        //    {
        //        return safetyAreaColor[originSafetyAreaColorIndex];
        //    }
        //}

        public float AlphaParam
        {
            get
            {
                return Mathf.Lerp(5f, 120f, originAlphaParam);
            }
        }

        //private GameObject savedSafetyArea;

        private void Start()
        {
            SafetyAreaLanguageManager.Instance.Init();
            if (!hasReadFromFile)
            {
                LoadSafetyArea();
            }
        }

        private void Init()
        {
            InitSafetyAreaMono();
            InitStep(safetyAreaMono);
            safetyAreaMono.Init();
        }

        private void Release()
        {
            OnFinishSetSafeArea?.Invoke();
            if (safetyAreaMono != null)
            {
                safetyAreaMono.Release();
                GameObject.Destroy(safetyAreaGameObject);
                safetyAreaMono = null;
                areaStepDic = null;
            }
        }

        private void InitStep(SafetyAreaMono safetyAreaMono)
        {
            if (areaStepDic == null)
            {
                areaStepDic = new Dictionary<SafetyAreaStepEnum, ISafetyAreaStep>();
                areaStepDic.Add(SafetyAreaStepEnum.GroundHeight, new GroundHeightStep(safetyAreaMono));
                areaStepDic.Add(SafetyAreaStepEnum.PlayArea, new PlayAreaStep(safetyAreaMono));
                areaStepDic.Add(SafetyAreaStepEnum.StationaryArea, new StationaryAreaStep(safetyAreaMono));
                //areaStepDic.Add(SafetyAreaStepEnum.ConfirmPlayArea, new ConfirmPlayAreaStep());
            }
        }

        private void InitSafetyAreaMono()
        {
            if (safetyAreaMono == null)
            {
                GameObject safetyAreaMonoResource = Resources.Load<GameObject>("SafetyAreaMono");
                safetyAreaGameObject = GameObject.Instantiate(safetyAreaMonoResource);
                safetyAreaMono = safetyAreaGameObject.GetComponent<SafetyAreaMono>();
            }
            //safetyAreaMono.Init();
        }

        //创建原地区域
        public void CreateStationarySafetyArea()
        {
            if (stationaryAreaMono == null)
            {
                stationaryAreaObject = new GameObject(PlayAreaConstant.STATIONARY_NAME);
                stationaryAreaObject.layer = 9;//1 << LayerMask.NameToLayer("SafetyArea");
                float groundHeight = GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight).GetPlaneHeight();
                Mesh cylinderMesh = SafetyAreaVertexHelper.GenerateCylinderMesh(new Vector3(0, groundHeight, 0), groundHeight + PlayAreaConstant.SAFETY_AREA_HEIGHT, groundHeight, PlayAreaConstant.STATIONARY_AREA_RADIUS);
                stationaryAreaMono = stationaryAreaObject.AddComponent<StationaryAreaMono>();
                stationaryAreaMono.Init();
                stationaryAreaMono.SetMesh(cylinderMesh, PlayAreaConstant.STATIONARY_AREA_RADIUS);
            }
        }

        //销毁原地区域
        public void DestroyStationaryArea()
        {
            if (stationaryAreaObject != null)
            {
                GameObject.Destroy(stationaryAreaObject);
            }
            stationaryAreaObject = null;
            stationaryAreaMono = null;
        }

        //创建安全网格
        public void CreatePlayArea(Mesh mesh, Color[] colors, float perimeter, Vector3 worldPosition)
        {
            if (playAreaObject != null)
            {
                GameObject.Destroy(playAreaObject);
            }
            playAreaObject = new GameObject(PlayAreaConstant.PLAY_AREA_NAME);
            playAreaObject.layer = LayerMask.NameToLayer("SafetyArea");
            playAreaObject.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);
            playAreaMono = playAreaObject.AddComponent<PlayAreaMono>();
            playAreaMono.Init();
            playAreaMono.SetMesh(mesh, colors, perimeter);
        }

        public void DestroyPlayArea()
        {
            if (playAreaObject != null)
            {
                Debug.LogError("Destroy playAreaObject");
                GameObject.Destroy(playAreaObject);
            }
            playAreaObject = null;
            playAreaMono = null;
        }

        public void StartSetStationaryArea()
        {
            if (!Unity.XR.Skyworth.SkyworthSettings.Instance.m_SafetyAreaEditor)
            {
                SkyworthVRPlatform.ToStationaryArea();
            }
            else
            {
                StartSetSafetyAreaInternal();
                ChangeStep(SafetyAreaStepEnum.StationaryArea);
            }
        }

        public void StartSetSafetyArea()
        {
            if (!Unity.XR.Skyworth.SkyworthSettings.Instance.m_SafetyAreaEditor)
            {
                SkyworthVRPlatform.ToSetGameArea();
            }
            else
            {
                StartSetSafetyAreaInternal();
            }
        }

        private void StartSetSafetyAreaInternal() 
        {
            if (safetyAreaMono != null)
            {
                Debug.LogError("last set safety area process not complete");
                return;
            }

            //if (savedSafetyArea != null)
            //{
            //    savedSafetyArea.SetActive(false);
            //}

            Init();
            OnBeginSetSafeArea?.Invoke();
            DestroyExistSafetyArea();
            SkyworthPerformance.GSXR_ResaveMap("forTest");
            isSettingSafetyArea = true;
            SkyworthPerformance.GSXR_StartSeeThrough();
            //API_GSXR_Slam.GSXR_Add_SlamPauseCallback(OnSlamPause);
        }
        public void StartSetSafetyAreaHeight()
        {
            if (!CheckSafetyAreaExist())
            {
                StartSetSafetyArea();
                return;
            }

            onlySettingGroundHeight = true;
            Init();
            OnBeginSetSafeArea?.Invoke();
            SkyworthPerformance.GSXR_StartSeeThrough();
            //API_GSXR_Slam.GSXR_Add_SlamPauseCallback(OnSlamPause);
        }

        //判断安全区域是否存在
        public bool CheckSafetyAreaExist()
        {
            if (!hasReadFromFile)
            {
                LoadSafetyArea();
            }

            GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
            if (playArea != null)
            {
                return true;
            }

            GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
            if (stationaryArea != null)
            {
                return true;
            }

            return false;
        }

        //重新设置安全区域高度
        public void ResetSafetyAreaHeight()
        {
            GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
            if (playArea != null)
            {
                playArea.GetComponent<PlayAreaMono>().ResetPlaneHeight(GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight).GetPlaneHeight());
            }

            GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
            if (stationaryArea != null)
            {
                stationaryArea.GetComponent<StationaryAreaMono>().ResetPlaneHeight(GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight).GetPlaneHeight());
            }
        }

        public void DestroyExistSafetyArea()
        {
            GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
            if (playArea != null)
            {
                GameObject.Destroy(playArea);
            }

            GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
            if (stationaryArea != null)
            {
                GameObject.Destroy(stationaryArea);
            }
        }

        public void ExitSafeAreaStep()
        {
            if (currentStep != null)
            {
                currentStep.OnExitStep();
            }
            currentStep = null;
            Release();
            //API_GSXR_Slam.GSXR_Remove_SlamPauseCallback(OnSlamPause);
            SkyworthPerformance.GSXR_StopSeeThrough();
            SkyworthPerformance.GSXR_SaveMap();
            isSettingSafetyArea = false;
            onlySettingGroundHeight = false;
            stationaryAreaMono = null;
            stationaryAreaObject = null;
            playAreaMono = null;
            playAreaObject = null;
            SaveSafetyArea(true);
        }

        private void LoadSafetyArea()
        {
            hasReadFromFile = true;
            string filepath;
#if UNITY_EDITOR
            Directory.SetCurrentDirectory(Directory.GetParent(Application.dataPath).FullName);
            filepath = Path.Combine(Directory.GetCurrentDirectory(), "SafetyArea.txt");
#else
        filepath = PlayAreaConstant.SAVE_FILE_NAME;
#endif
            if (!File.Exists(filepath))
            {
                Debug.LogWarningFormat("Can not found config in path :{0}", filepath);
                currentSafetyAreaInfo = new SafetyAreaInfo();
                return;
            }

            string fileStr = File.ReadAllText(filepath);

            currentSafetyAreaInfo = JsonUtility.FromJson<SafetyAreaInfo>(fileStr);
            CreateSafetyArea();
        }
        private void CreateSafetyArea()
        {
            showAreaWhenBowHead = true;// currentSafetyAreaInfo.showAreaWhenBowHead;
            originAlphaParam = currentSafetyAreaInfo.originAlphaParam;
            originSafetyAreaColorIndex = currentSafetyAreaInfo.originSafetyAreaColorIndex;

            if (string.IsNullOrEmpty(currentSafetyAreaInfo.safetyAreaName))
            {
                return;
            }

            Mesh edgeMesh = new Mesh();
            edgeMesh.vertices = currentSafetyAreaInfo.vertices.ToArray();
            edgeMesh.uv = currentSafetyAreaInfo.uv.ToArray();
            edgeMesh.triangles = currentSafetyAreaInfo.triangles.ToArray();

            GameObject safetyArea = new GameObject(currentSafetyAreaInfo.safetyAreaName);
            safetyArea.transform.position = currentSafetyAreaInfo.transform.position;
            safetyArea.transform.rotation = currentSafetyAreaInfo.transform.rotation;
            
            MeshRenderer meshRenderer = safetyArea.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = safetyArea.AddComponent<MeshFilter>();
            meshFilter.mesh = edgeMesh;
            meshRenderer.sharedMaterial = Resources.Load<Material>("Material/SafetyEdgeMat_Normal");

            SafetyAreaBase safetyAreaBase = null;
            if (currentSafetyAreaInfo.safetyAreaName == PlayAreaConstant.PLAY_AREA_NAME)
            {
                safetyAreaBase = safetyArea.AddComponent<PlayAreaMono>();
                safetyArea.GetComponent<PlayAreaMono>().Init();
                safetyArea.GetComponent<PlayAreaMono>().SetColor(currentSafetyAreaInfo.colors.ToArray());
            }
            else
            {
                safetyAreaBase = safetyArea.AddComponent<StationaryAreaMono>();
                safetyArea.GetComponent<StationaryAreaMono>().Init();
                safetyArea.GetComponent<StationaryAreaMono>().SetRadius(currentSafetyAreaInfo.radius);
            }
            safetyAreaBase.SetOriginHeight(currentSafetyAreaInfo.originHeight);
            safetyAreaBase.SetPerimeter(currentSafetyAreaInfo.perimeter);
            UpdateAreaPosition(safetyArea);
        }
        private void UpdateAreaPosition(GameObject safteyAreaObject) 
        {
            safteyAreaObject.transform.position = safteyAreaObject.transform.position - new Vector3(currentSafetyAreaInfo.observer.position.x,0, currentSafetyAreaInfo.observer.position.z);

            safteyAreaObject.transform.rotation = safteyAreaObject.transform.rotation
                * Quaternion.Inverse(Quaternion.Euler(0, currentSafetyAreaInfo.observer.rotation.eulerAngles.y, 0));
            safteyAreaObject.transform.position = Quaternion.Inverse(Quaternion.Euler(0, currentSafetyAreaInfo.observer.rotation.eulerAngles.y, 0)) * safteyAreaObject.transform.position;

        }
        private void SaveSafetyArea(bool overrideVertexData = false)
        {
            GameObject selfAreaObject = null;
            string areaName = "";
            if (CheckSafetyAreaExist())
            {
                GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                if (playArea != null)
                {
                    areaName = PlayAreaConstant.PLAY_AREA_NAME;
                    selfAreaObject = playArea;
                }

                GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                if (stationaryArea != null)
                {
                    areaName = PlayAreaConstant.STATIONARY_NAME;
                    selfAreaObject = stationaryArea;
                }

                if(areaName != currentSafetyAreaInfo.safetyAreaName)
                    currentSafetyAreaInfo.lastchangetime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            }

            if (overrideVertexData)
            {

                if (selfAreaObject!= null)
                {
                    currentSafetyAreaInfo.safetyAreaName = areaName;
                    currentSafetyAreaInfo.vertices = new List<Vector3>(selfAreaObject.GetComponent<MeshFilter>().mesh.vertices);
                    currentSafetyAreaInfo.triangles = new List<int>(selfAreaObject.GetComponent<MeshFilter>().mesh.triangles);
                    currentSafetyAreaInfo.uv = new List<Vector2>(selfAreaObject.GetComponent<MeshFilter>().mesh.uv);
                    currentSafetyAreaInfo.originHeight = selfAreaObject.GetComponent<SafetyAreaBase>().GetOriginHeight();
                    currentSafetyAreaInfo.perimeter = selfAreaObject.GetComponent<SafetyAreaBase>().GetPerimter();
                    currentSafetyAreaInfo.radius = selfAreaObject.GetComponent<StationaryAreaMono>().GetRadius();

                }
                else
                {
                    currentSafetyAreaInfo.safetyAreaName = string.Empty;
                    currentSafetyAreaInfo.vertices = null;
                    currentSafetyAreaInfo.triangles = null;
                    currentSafetyAreaInfo.uv = null;
                    currentSafetyAreaInfo.originHeight = 0f;
                }

                currentSafetyAreaInfo.lastchangetime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            }

            if (selfAreaObject != null)
            {
                currentSafetyAreaInfo.transform.position = selfAreaObject.transform.position;
                currentSafetyAreaInfo.transform.rotation = selfAreaObject.transform.rotation;
                currentSafetyAreaInfo.transform.lastchangetime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            }
            else
            {
                currentSafetyAreaInfo.transform.position = Vector3.zero;
                currentSafetyAreaInfo.transform.rotation = Quaternion.identity;
                currentSafetyAreaInfo.transform.lastchangetime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            }

            currentSafetyAreaInfo.showAreaWhenBowHead = ShowAreaWhenBowHead;
            currentSafetyAreaInfo.originAlphaParam = OriginAlphaParam;
            currentSafetyAreaInfo.originSafetyAreaColorIndex = OriginSafetyAreaColorIndex;

            currentSafetyAreaInfo.observer = new SafetyAreaInfo.SafetyTransform
            {
                position = Camera.main.transform.position,
                rotation = Camera.main.transform.rotation,
                lastchangetime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            };
            string filepath;
#if UNITY_EDITOR
            Directory.SetCurrentDirectory(Directory.GetParent(Application.dataPath).FullName);
            filepath = Path.Combine(Directory.GetCurrentDirectory(), "SafetyArea.txt");
#else
        filepath = PlayAreaConstant.SAVE_FILE_NAME;
#endif
            try
            {
                if (!File.Exists(filepath))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    File.Create(filepath).Dispose();
                }
                File.WriteAllText(filepath, JsonUtility.ToJson(currentSafetyAreaInfo));
            }
            catch (Exception e)
            {
                Debug.LogError("[SkyworthSDKException]:" + e.ToString());
            }

        }

        public void DestroySafetyArea()
        {
            if (CheckSafetyAreaExist())
            {
                DestroyExistSafetyArea();
            }
            SaveSafetyArea();
        }

        public T GetStep<T>(SafetyAreaStepEnum safetyAreaStepEnum) where T : class, ISafetyAreaStep
        {
            if (!areaStepDic.ContainsKey(safetyAreaStepEnum))
            {
                return default(T);
            }
            return areaStepDic[safetyAreaStepEnum] as T;
        }

        public void ChangeStep(SafetyAreaStepEnum safetyAreaStep)
        {
            if (currentStep != null)
            {
                currentStep.OnExitStep();
            }
            ISafetyAreaStep nextStep = areaStepDic[safetyAreaStep];
            nextStep.OnEnterStep();
            currentStep = nextStep;
        }

        public SafetyAreaStepEnum GetCurrentStepEnum()
        {
            if (currentStep == null) return SafetyAreaStepEnum.Null;
            return currentStep.GetStepEnum();
        }

        public void DelayExitSafeAreaStep(float delayTime, Action onExitComplete)
        {
            StartCoroutine(ExitSafeAreaStepCoroutine(delayTime, onExitComplete));
        }

        public void DelayChangeStep(SafetyAreaStepEnum safetyAreaStep, float delayTime)
        {
            StartCoroutine(ChangeStepCoroutine(safetyAreaStep, delayTime));
        }

        public void ExitSafetyAreaInvoke()
        {
            if (!alreayExitSafetyArea)
            {
                alreayExitSafetyArea = true;
                OnExitSafetyArea?.Invoke();
            }
        }

        public void EnterSafetyAreaInvoke()
        {
            if (alreayExitSafetyArea)
            {
                alreayExitSafetyArea = false;
                OnEnterSafetyArea?.Invoke();
            }
        }

        private IEnumerator ChangeStepCoroutine(SafetyAreaStepEnum safetyAreaStep, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            ChangeStep(safetyAreaStep);
        }

        private IEnumerator ExitSafeAreaStepCoroutine(float delayTime, Action onExitComplete)
        {
            yield return new WaitForSeconds(delayTime);
            onExitComplete?.Invoke();
            ExitSafeAreaStep();
        }

        public void DelayShow(float delayTime, Action delayMethod)
        {
            StartCoroutine(DelayShowCoroutine(delayTime, delayMethod));
        }

        private IEnumerator DelayShowCoroutine(float delayTime, Action delayMethod)
        {
            yield return new WaitForSeconds(delayTime);
            delayMethod?.Invoke();
        }

        private void OnSlamPause(bool isPause)
        {
            if (!isPause)
            {
                SkyworthPerformance.GSXR_StartSeeThrough();
            }
        }
#if UNITY_EDITOR
        [ContextMenu("Test-Recenter")]
        public void EditorSave() 
        {
            SaveSafetyArea();
            Camera.main.transform.rotation = Quaternion.identity;
            Camera.main.transform.position = Vector3.zero;
            if (CheckSafetyAreaExist())
            {
                GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                if (playArea != null)
                {
                    UpdateAreaPosition(playArea);
                }

                GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                if (stationaryArea != null)
                {
                    UpdateAreaPosition(stationaryArea);
                }
            }
            
        }

        [ContextMenu("Test-ReLoad")]
        public void ReLoad()
        {
            //SaveSafetyArea();
            Camera.main.transform.rotation = Quaternion.identity;
            Camera.main.transform.position = Vector3.zero;
            DestroyExistSafetyArea();
            LoadSafetyArea();
        }

        [ContextMenu("Test-Pause")]
        public void TestPause()
        {
            SaveSafetyArea();
        }

        [ContextMenu("Test-Resume")]
        public void TestResume()
        {
            Camera.main.transform.rotation = Quaternion.identity;
            Camera.main.transform.position = Vector3.zero;

            Directory.SetCurrentDirectory(Directory.GetParent(Application.dataPath).FullName);
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "SafetyArea.txt");
            if (!File.Exists(filepath))
            {
                Debug.LogWarningFormat("Can not found config in path :{0}", filepath);
                return;
            }

            string fileStr = File.ReadAllText(filepath);

            SafetyAreaInfo lastSafetyAreaInfo = JsonUtility.FromJson<SafetyAreaInfo>(fileStr);
            if (currentSafetyAreaInfo != null)
            {

                if (lastSafetyAreaInfo.lastchangetime != currentSafetyAreaInfo.lastchangetime)
                {
                    currentSafetyAreaInfo = lastSafetyAreaInfo;
                    DestroyExistSafetyArea();
                    CreateSafetyArea();
                }
                else if (lastSafetyAreaInfo.observer.lastchangetime != currentSafetyAreaInfo.observer.lastchangetime)
                {
                    currentSafetyAreaInfo = lastSafetyAreaInfo;
                    if (CheckSafetyAreaExist())
                    {
                        GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                        if (playArea != null)
                        {
                            playArea.transform.position = currentSafetyAreaInfo.transform.position;
                            playArea.transform.rotation = currentSafetyAreaInfo.transform.rotation;
                            UpdateAreaPosition(playArea);
                        }

                        GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                        if (stationaryArea != null)
                        {
                            stationaryArea.transform.position = currentSafetyAreaInfo.transform.position;
                            stationaryArea.transform.rotation = currentSafetyAreaInfo.transform.rotation;
                            UpdateAreaPosition(stationaryArea);
                        }
                    }
                }

                
            }
        }
#endif
        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                string filepath;
#if UNITY_EDITOR
                Directory.SetCurrentDirectory(Directory.GetParent(Application.dataPath).FullName);
                filepath = Path.Combine(Directory.GetCurrentDirectory(), "SafetyArea.txt");
#else
            filepath = PlayAreaConstant.SAVE_FILE_NAME;
#endif
                if (!File.Exists(filepath))
                {
                    Debug.LogWarningFormat("Can not found config in path :{0}", filepath);
                    return;
                }

                string fileStr = File.ReadAllText(filepath);

                SafetyAreaInfo lastSafetyAreaInfo = JsonUtility.FromJson<SafetyAreaInfo>(fileStr);
                if (currentSafetyAreaInfo != null)
                {
                    
                    if (lastSafetyAreaInfo.lastchangetime != currentSafetyAreaInfo.lastchangetime)
                    {
                        currentSafetyAreaInfo = lastSafetyAreaInfo;
                        DestroyExistSafetyArea();
                        CreateSafetyArea();
                    }
                    else if (lastSafetyAreaInfo.transform.lastchangetime != currentSafetyAreaInfo.transform.lastchangetime) 
                    {
                        currentSafetyAreaInfo = lastSafetyAreaInfo;
                        if (CheckSafetyAreaExist())
                        {
                            GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                            if (playArea != null)
                            {
                                playArea.transform.position = currentSafetyAreaInfo.transform.position;
                                playArea.transform.rotation = currentSafetyAreaInfo.transform.rotation;
                                UpdateAreaPosition(playArea);
                            }

                            GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                            if (stationaryArea != null)
                            {
                                stationaryArea.transform.position = currentSafetyAreaInfo.transform.position;
                                stationaryArea.transform.rotation = currentSafetyAreaInfo.transform.rotation;
                                UpdateAreaPosition(stationaryArea);
                            }
                        }
                    }

                    
                }
            }
            else 
            {
                SaveSafetyArea();
            }
        }

        
        
        private void Update()
        {
            LeftHandLongpress.OnLongPress(() => {
                SaveSafetyArea();
                LeftHandLongpress.TryRecenter();
                if (CheckSafetyAreaExist())
                {
                    GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                    if (playArea != null)
                    {
                        UpdateAreaPosition(playArea);
                    }

                    GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                    if (stationaryArea != null)
                    {
                        UpdateAreaPosition(stationaryArea);
                    }
                }
            });
            RgihtHandLongpress.OnLongPress(() => {
                SaveSafetyArea();
                RgihtHandLongpress.TryRecenter();
                if (CheckSafetyAreaExist())
                {
                    GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                    if (playArea != null)
                    {
                        UpdateAreaPosition(playArea);
                    }

                    GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                    if (stationaryArea != null)
                    {
                        UpdateAreaPosition(stationaryArea);
                    }
                }
            });
        }

    }
}
