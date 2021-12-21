/*
 * @Author: xieminghui
 * @Date: 2020-12-30 14:39:04
 * @Description: Description
 * @LastEditors: xieminghui
 * @LastEditTime: 2020-12-31 15:45:38
 * @Copyright: Copyright 2020 Skyworth VR. All rights reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Svr.Keyboard
{
    /// <summary>
    /// 键盘底层交互
    /// </summary>
    public class InputMethodApi
    {

        private static AndroidJavaObject mJavaObject;
        private static string PACKAGE = "com.ssnwt.vr.inputmethod";
        private static string InputMethodApiCLASSNAME = "InputMethodApi";
        private int mKeyboardWidth;
        public int KeyboardWidth { get { return mKeyboardWidth; } }
        private int mKeyboardHight;
        public int KeyboardHight { get { return mKeyboardHight; } }
        private GLTexture mKeyboardTexture;
        private Queue<Action> mMainThreadQueue = new Queue<Action>();
        public Action<GLTexture> OnTextureCreated;
        public Action<string, int, int> OnTextChange;
        public Action<string[]> OnGetLanguagesAction;
        public Action OnHidEvent;
        public Action OnShowKeyboard;
        public Action OnMoveStart;
        public Action OnMoveEnd;
        private OpenGLRenderApi mOpenGLRenderApi;
        private AndroidJavaObject mApplication;
        public static bool debug;
        public InputMethodApi(bool systeminternal = false)
        {
            mKeyboardTexture = new GLTexture();
            mOpenGLRenderApi = new OpenGLRenderApi();
            OpenGLRenderApi.CreatTextureAction += OpenGLRenderApi_CreatTextureAction;
            OpenGLRenderApi.UpdateTextureAction += OpenGLRenderApi_UpdateTextureAction;
            OpenGLRenderApi.ReleaseTextureAction += OpenGLRenderApi_ReleaseTextureAction;
            mJavaObject = new AndroidJavaObject(PACKAGE + "." + InputMethodApiCLASSNAME);
            AndroidJavaClass unitypalyer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unitypalyer.GetStatic<AndroidJavaObject>("currentActivity");
            mApplication = activity.Call<AndroidJavaObject>("getApplication");
            mJavaObject.Call("setShowKeyboardListener", new KeyboardListener(this));
            Log("systeminternal:"+ systeminternal);
            mJavaObject.Call("connect", mApplication, systeminternal, new BindResult(this));
        }
        internal static void Log(string message)
        {
            if (debug)
                Debug.Log("[InputMethodApi]" + message);
        }
        internal static void LogFormat(string format, params object[] args)
        {
            if (debug)
                Debug.LogFormat("[InputMethodApi]" + format, args);
        }
        internal void Add(Action action)
        {
            lock (mMainThreadQueue)
            {
                mMainThreadQueue.Enqueue(action);
            }
        }
        private void OpenGLRenderApi_ReleaseTextureAction(int id, int textureid)
        {
            if (id != mKeyboardTexture.ID) return;
            Log("OpenGLRenderApi_ReleaseTextureAction");
            mKeyboardTexture.glRelease();
        }
        private void OpenGLRenderApi_UpdateTextureAction(int id, int textureid)
        {
            if (id != mKeyboardTexture.ID) return;
            mKeyboardTexture.glUpdateTexImage();
        }

        private void OpenGLRenderApi_CreatTextureAction(int id,int textureid)
        {
            if (id != mKeyboardTexture.ID) return;
            Log("OpenGLRenderApi_CreatTextureAction");
            mKeyboardTexture.glCreateSurface(mKeyboardWidth, mKeyboardHight);
            mJavaObject.Call("setSurface", mKeyboardTexture.getSurface());
            Add(() =>
            {
                if (OnTextureCreated != null) OnTextureCreated(mKeyboardTexture);
            });
        }
        private void UpdateMainThreadMessage()
        {
            while (mMainThreadQueue.Count > 0)
            {
                mMainThreadQueue.Dequeue()();
            }

        }
        #region PUBLIC METHODS
        public void Connect(bool systeminternal)
        {
            if (!IsConnected())
                mJavaObject.Call("connect", mApplication, systeminternal, new BindResult(this));
        }
        public void Update()
        {
            UpdateMainThreadMessage();
            if (mKeyboardTexture != null)
                OpenGLRenderApi.IssuePluginEvent(mKeyboardTexture.ID, (int)OpenGLRenderApi.OpenGLOption.UpdateTexture, 0);
        }
        public void Disconnect()
        {
            mJavaObject.Call("disconnect", mApplication);
        }
        public bool IsConnected()
        {
            return mJavaObject.Call<bool>("isConnected");
        }
        public void performClickDown(int x, int y)
        {
            LogFormat("performClickDown:{0},{1}",x,y);
            mJavaObject.Call("performClickDown", x, y);
        }
        public void performClickUp(int x, int y)
        {
            LogFormat("performClickUp:{0},{1}",x,y);
            mJavaObject.Call("performClickUp", x, y);
        }
        public void updateFocusPosition(int x, int y)
        {
            //LogFormat("updateFocusPosition:{0},{1}", x, y);
            mJavaObject.Call("updateFocusPosition", x, y);
        }
        public void setTextSelection(int selStart, int selEnd)
        {
            LogFormat("setTextSelection {0},{1}", selStart, selEnd);
            mJavaObject.Call("setTextSelection", selStart, selEnd);
        }
        public void show()
        {
            Log("show");
            mJavaObject.Call("show");
        }
        public void show(string text)
        {
            Log("show " + text);
            mJavaObject.Call("show", text);
        }
        public void hide()
        {
            Log("hide");
            mJavaObject.Call("hide");
        }
        //public void enableSystemKeyboard(bool system)
        //{
        //    Log("enableSystemKeyboard:" + system);
        //    mJavaObject.Call("enableSystemKeyboard", system);
        //}
        public void Release()
        {
            Log("Release");
            Disconnect();
            if (mKeyboardTexture.TextureID != 0)
                OpenGLRenderApi.IssuePluginEvent(mKeyboardTexture.ID, (int)OpenGLRenderApi.OpenGLOption.ReleaseTexture, 0);

            OpenGLRenderApi.CreatTextureAction -= OpenGLRenderApi_CreatTextureAction;
            OpenGLRenderApi.UpdateTextureAction -= OpenGLRenderApi_UpdateTextureAction;
            OpenGLRenderApi.ReleaseTextureAction -= OpenGLRenderApi_ReleaseTextureAction;
        }

        /// <summary>
        /// 英 en,法 fr,德 de,日 ja,韩 ko,中国 zh
        /// </summary>
        /// <param name="language">language</param>
        public void SetLanguage(string language)
        {
            mJavaObject.Call("setLanguage",language);
        }
        #endregion
        
        #region LISTENER
        private sealed class KeyboardListener : AndroidJavaProxy
        {
            private InputMethodApi InputMethodApi;
            public KeyboardListener(InputMethodApi input) : base(PACKAGE + "." + InputMethodApiCLASSNAME + "$KeyboardListener")
            {
                InputMethodApi = input;
            }
            public void onShowKeyboard()
            {
                Log("onShowKeyboard");
                InputMethodApi.Add(() =>
                {
                    if (InputMethodApi.OnShowKeyboard != null) InputMethodApi.OnShowKeyboard();
                });
            }
            public void onUpdateKeyboardSize(int var1, int var2)
            {
                LogFormat("onUpdateKeyboardSize:{0},{1}", var1, var2);
                InputMethodApi.mKeyboardWidth = var1;
                InputMethodApi.mKeyboardHight = var2;
                InputMethodApi.Add(() =>
                {
                    OpenGLRenderApi.IssuePluginEvent(InputMethodApi.mKeyboardTexture.ID, (int)OpenGLRenderApi.OpenGLOption.CreatTexture, 0);
                });
            }
            public void onTextChanged(string text, int selectionStart, int selectionEnd)
            {
                LogFormat("onTextChanged:{0},({1},{2})", text, selectionStart, selectionEnd);
                InputMethodApi.Add(() =>
                {
                    if (InputMethodApi.OnTextChange != null) InputMethodApi.OnTextChange(text, selectionStart, selectionEnd);
                });
            }
            public void onGetLanguages(string[] languages)
            {
                InputMethodApi.Add(() =>
                {
                    if (InputMethodApi.OnGetLanguagesAction != null) InputMethodApi.OnGetLanguagesAction(languages);
                });

            }
            public void onHideKeyboard()
            {
                Log("onHideKeyboard");
                InputMethodApi.Add(()=> 
                {
                    if (InputMethodApi.OnHidEvent != null) InputMethodApi.OnHidEvent();
                });
            }
            public void onMoveStart()
            {
                Log("onMoveStart");
                InputMethodApi.Add(() =>
                {
                    if (InputMethodApi.OnMoveStart != null) InputMethodApi.OnMoveStart();
                });
            }
            public void onMoveEnd()
            {
                Log("onMoveEnd");
                InputMethodApi.Add(() =>
                {
                    if (InputMethodApi.OnMoveEnd != null) InputMethodApi.OnMoveEnd();
                });
            }
        }

        private sealed class BindResult : AndroidJavaProxy
        {
            private InputMethodApi InputMethodApi;
            public BindResult(InputMethodApi input) : base(PACKAGE + "." + InputMethodApiCLASSNAME + "$BindResult")
            {
                InputMethodApi = input;
            }
            public void onConnected()
            {
                Log("onConnected");
                InputMethodApi.Add(() =>
                {

                });
            }
            public void onDisconnected()
            {
                Log("onDisconnected");
                InputMethodApi.Add(() =>
                {

                });
            }
            public void onError()
            {
                Log("onError");
                InputMethodApi.Add(() =>
                {

                });
            }
        }
        #endregion
    }
    /// <summary>
    /// OpenGL环境
    /// </summary>
    public class OpenGLRenderApi
    {
        public class MonoPInvokeCallbackAttribute : Attribute
        {
            public MonoPInvokeCallbackAttribute() { }
        }
        public enum OpenGLOption
        {
            CreatTexture,
            UpdateTexture,
            ReleaseTexture
        }
        private const string DLL = "svr_plugin_opengl_render";
        [DllImport(DLL)]
        private static extern IntPtr GetRenderEventFunc();
        [DllImport(DLL)]
        private static extern void SetOnRenderEventCallback(Action<int> action);
        [DllImport(DLL)]
        private static extern int getRenderId(int _event);
        [DllImport(DLL)]
        private static extern int getRenderOperate(int _event);
        [DllImport(DLL)]
        private static extern int getRenderTextureId(int _event);
        [DllImport(DLL)]
        private static extern int getRenderEvent(int id, int operate, int textureId);

        public static event Action<int,int> CreatTextureAction;
        public static event Action<int,int> UpdateTextureAction;
        public static event Action<int,int> ReleaseTextureAction;
        static OpenGLRenderApi()
        {
            SetOnRenderEventCallback(OnRenderEvent);
        }

        public static void IssuePluginEvent(int id, int operate, int textureId)
        {
            int msg = getRenderEvent(id, operate, textureId);
            GL.IssuePluginEvent(GetRenderEventFunc(), msg);
        }

        [MonoPInvokeCallback]
        public static void OnRenderEvent(int _event)
        {
            AndroidJNI.AttachCurrentThread();
            OpenGLOption option = (OpenGLOption)getRenderOperate(_event);
            int renderid = getRenderId(_event);
            int textureid = getRenderTextureId(_event);
            switch (option)
            {
                case OpenGLOption.CreatTexture:

                    if (CreatTextureAction != null) CreatTextureAction(renderid, textureid);
                    break;
                case OpenGLOption.UpdateTexture:
                    if (UpdateTextureAction != null) UpdateTextureAction(renderid, textureid);
                    break;
                case OpenGLOption.ReleaseTexture:
                    if (ReleaseTextureAction != null) ReleaseTextureAction(renderid, textureid);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// OpenGL 纹理
    /// </summary>
    public class GLTexture
    {
        public int Width { private set; get; }
        public int Hight { private set; get; }
        public int TextureID { private set; get; }
        public int ID { private set; get; }
        private AndroidJavaObject mJavaObject;
        public GLTexture()
        {
            mJavaObject = new AndroidJavaObject("com.ssnwt.vr.openglrender.GLTexture");
            ID = getID();
        }
        public void glCreateSurface(int w, int h)
        {
            mJavaObject.Call("glCreateSurface", w, h);
            Width = w;
            Hight = h;
            TextureID = getTextureID();
            InputMethodApi.LogFormat("GLTexture {0}x{1},{2}", Width, Hight, TextureID);
        }

        public AndroidJavaObject getSurface()
        {
            return mJavaObject.Call<AndroidJavaObject>("getSurface");
        }

        private int getTextureID()
        {
            return mJavaObject.Call<int>("getTextureID");
        }
        public Matrix4x4 getTransformMatrix()
        {
            float[] matrix_java = mJavaObject.Call<float[]>("getTransformMatrix");
            Matrix4x4 matrix4X4 = Matrix4x4.identity;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix4X4[j, i] = matrix_java[i * 4 + j];
                }
            }

            return matrix4X4;
        }

        public void glRelease()
        {
            mJavaObject.Call("glRelease");
            TextureID = 0;
        }
        public void glUpdateTexImage()
        {
            mJavaObject.Call("glUpdateTexImage");
        }
        public void resize(int w, int h)
        {
            mJavaObject.Call("resize", w, h);
        }
        private int getID()
        {
            return mJavaObject.Call<int>("getID");
        }
    }
}