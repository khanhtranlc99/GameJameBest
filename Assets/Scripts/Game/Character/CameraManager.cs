using Game.Character.CameraEffects;
using Game.Character.Input;
using Game.Character.Modes;
using Game.Character.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
    public class CameraManager : MonoBehaviour
    {
        private struct CameraTransform
        {
            private Vector3 pos;

            private Quaternion rot;

            private float fov;

            private Vector3 posVel;

            private Vector3 rotVel;

            private float fovVel;

            private float timeout;

            private float speedRatio;

            public CameraTransform(Camera cam)
            {
                pos = cam.transform.position;
                rot = cam.transform.rotation;
                fov = cam.fieldOfView;
                posVel = Vector3.zero;
                rotVel = Vector3.zero;
                fovVel = 0f;
                timeout = 0f;
                speedRatio = 1f;
            }

            public bool Interpolate(Camera cam, float speed, float timeMax)
            {
                float smoothTime = speed * speedRatio;
                pos = Vector3.SmoothDamp(pos, cam.transform.position, ref posVel, smoothTime);
                Vector3 eulerAngles = rot.eulerAngles;
                Vector3 eulerAngles2 = rot.eulerAngles;
                float x = eulerAngles2.x;
                Vector3 eulerAngles3 = cam.transform.eulerAngles;
                eulerAngles.x = Mathf.SmoothDampAngle(x, eulerAngles3.x, ref rotVel.x, smoothTime);
                Vector3 eulerAngles4 = rot.eulerAngles;
                float y = eulerAngles4.y;
                Vector3 eulerAngles5 = cam.transform.eulerAngles;
                eulerAngles.y = Mathf.SmoothDampAngle(y, eulerAngles5.y, ref rotVel.y, smoothTime);
                Vector3 eulerAngles6 = rot.eulerAngles;
                float z = eulerAngles6.z;
                Vector3 eulerAngles7 = cam.transform.eulerAngles;
                eulerAngles.z = Mathf.SmoothDampAngle(z, eulerAngles7.z, ref rotVel.z, smoothTime);
                if (!eulerAngles.z.Equals(float.NaN) && !eulerAngles.y.Equals(float.NaN) && !eulerAngles.x.Equals(float.NaN))
                {
                    rot = Quaternion.Euler(eulerAngles);
                }
                Game.Character.Utils.Math.CorrectRotationUp(ref rot);
                fov = Mathf.SmoothDamp(fov, cam.fieldOfView, ref fovVel, 0.05f);
                bool flag = (cam.transform.position - pos).sqrMagnitude < 0.001f && Quaternion.Angle(cam.transform.rotation, rot) < 0.001f && Mathf.Abs(fov - cam.fieldOfView) < 0.001f;
                timeout += Time.deltaTime;
                speedRatio = 1f - Mathf.Clamp01(timeout / timeMax);
                cam.transform.position = pos;
                cam.transform.rotation = rot;
                cam.fieldOfView = fov;
                return !flag;
            }
        }

        public delegate void OnTransitionFinished();

        public Camera UnityCamera;

        public float NormalTransitionSpeed = 0.5f;

        public float NormalTransitionTimeMax = 1f;

        public float FastTransitionSpeed = 0.2f;

        public float FastTransitionTimeMax = 0.5f;

        private bool FastTranslation;

        public GUISkin GuiSkin;

        public CameraMode ActivateModeOnStart;

        public Transform CameraTarget;

        private static CameraManager instance;

        private static readonly Vector3 PointBounds = Vector3.one * 0.1f;

        private Dictionary<Game.Character.Modes.Type, CameraMode> cameraModes;

        private bool transition;

        private CameraMode currMode;

        private CameraTransform oldModeTransform;

        private OnTransitionFinished finishedCallbak;

        public static CameraManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = UnityEngine.Object.FindObjectOfType<CameraManager>();
                }
                return instance;
            }
        }

        private float TransitionSpeed => (!FastTranslation) ? NormalTransitionSpeed : FastTransitionSpeed;

        private float TransitionTimeMax => (!FastTranslation) ? NormalTransitionTimeMax : FastTransitionTimeMax;

        public void RegisterMode(CameraMode cameraMode)
        {
            cameraModes.Add(cameraMode.Type, cameraMode);
            cameraMode.gameObject.SetActive(value: false);
        }

        public CameraMode SetMode(CameraMode cameraMode, bool FastTranslation = false)
        {
            if (currMode != cameraMode)
            {
                this.FastTranslation = FastTranslation;
                if (currMode != null)
                {
                    currMode.OnDeactivate();
                    Game.Character.Utils.Debug.SetActive(currMode.gameObject, status: false);
                    oldModeTransform = new CameraTransform(UnityCamera);
                    transition = true;
                }
                currMode = cameraMode;
                if (currMode != null)
                {
                    currMode.SetCameraTarget(CameraTarget);
                    currMode.Init();
                    Game.Character.Utils.Debug.SetActive(currMode.gameObject, status: true);
                    currMode.OnActivate();
                    currMode.Reset();
                }
            }
            return currMode;
        }

        public CameraMode SetMode(Game.Character.Modes.Type modeType, bool fastTranslation = false)
        {
            return SetMode(GetCameraModeByType(modeType), fastTranslation);
        }

        public void ResetCameraMode()
        {
            SetMode(ActivateModeOnStart);
        }

        public void SetCameraStatus(bool status)
        {
            UnityCamera.gameObject.SetActive(status);
        }

        public void SetDefaultConfiguration(Game.Character.Modes.Type cameraMode, string configuration)
        {
            cameraModes[cameraMode].DefaultConfiguration = configuration;
        }

        public void SetCameraTarget(Transform target)
        {
            CameraTarget = target;
            if ((bool)currMode)
            {
                currMode.SetCameraTarget(target);
            }
        }

        public CameraMode GetCurrentCameraMode()
        {
            return currMode;
        }

        public CameraMode GetCameraModeByType(Game.Character.Modes.Type modeType)
        {
            CameraMode value;
            if (cameraModes.TryGetValue(modeType, out value))
            {
                return value;
            }
            throw new Exception("Camera Manager not contains moode for '" + modeType + "' mode type.");
        }

        public void RegisterTransitionCallback(OnTransitionFinished callback)
        {
            finishedCallbak = (OnTransitionFinished)Delegate.Combine(finishedCallbak, callback);
        }

        public void UnregisterTransitionCallback(OnTransitionFinished callback)
        {
            finishedCallbak = (OnTransitionFinished)Delegate.Remove(finishedCallbak, callback);
        }

        private void Awake()
        {
            if (CameraTarget == null)
            {
                UnityEngine.Debug.LogWarning("Empty CameraTarget! Creating dummy one...");
                GameObject gameObject = new GameObject("DummyTarget");
                CameraTarget = gameObject.transform;
            }
            instance = this;
            cameraModes = new Dictionary<Game.Character.Modes.Type, CameraMode>();
            currMode = null;
            if (!UnityCamera)
            {
                UnityCamera = GetComponent<Camera>();
                if (!UnityCamera)
                {
                    UnityCamera = Camera.main;
                }
            }
            Transform parent = base.gameObject.transform.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if ((bool)child)
                {
                    CameraMode component = child.GetComponent<CameraMode>();
                    if ((bool)component)
                    {
                        RegisterMode(component);
                    }
                }
            }
            Initialize();
            SetMode(ActivateModeOnStart);
        }

        private void Initialize()
        {
        }

        private void Update()
        {
            if (!(currMode == null))
            {
                InputManager.Instance.GameUpdate();
                currMode.GameUpdate();
            }
        }

        private void LateUpdate()
        {
            if (currMode == null)
            {
                return;
            }
            if (Time.deltaTime > 0f)
            {
                currMode.PostUpdate();
            }
            if (transition)
            {
                transition = oldModeTransform.Interpolate(UnityCamera, TransitionSpeed, TransitionTimeMax);
                if (!transition && finishedCallbak != null)
                {
                    finishedCallbak();
                }
            }
            if ((bool)EffectManager.Instance)
            {
                EffectManager.Instance.PostUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (!(currMode == null))
            {
                currMode.FixedStepUpdate();
            }
        }

        public bool IsInCameraFrustrum(Vector3 point)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(UnityCamera);
            Bounds bounds = new Bounds(point, PointBounds);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
    }
}
