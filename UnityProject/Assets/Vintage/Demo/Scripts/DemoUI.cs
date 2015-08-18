///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Vintage - Image Effects.
// Copyright (c) Ibuprogames. All rights reserved.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#define USE_SLIDESHOW

//#define SHOW_SYSTEM_INFO

#define SHOW_ALL_COMMON_CONTROLS

using System.Collections.Generic;

using UnityEngine;

namespace VintageImageEffects.Demo
{
  /// <summary>
  /// UI for the demo.
  /// </summary>
  public class DemoUI : MonoBehaviour
  {
    public bool guiShow = true;

    public bool showEffectName = false;

    public float slideEffectTime = 0.0f;

    public AudioClip musicClip = null;

    private float effectTime = 0.0f;

    private List<ImageEffectBase> vintageImageEffects = new List<ImageEffectBase>();

    private List<ImageEffectBase> vintageImageEffectsExtra = new List<ImageEffectBase>();

#if USE_SLIDESHOW
    private SlideShow slideShow;
#endif
    private int guiSelection = 0;

    private bool menuOpen = false;

    private bool consoleShow = false;

    private const float guiMargen = 10.0f;
    private const float guiWidth = 200.0f;
    private const string guiTab = "   ";

    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 scrollLog = Vector2.zero;

    private float updateInterval = 0.5f;
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float fps = 0.0f;

    private GUIStyle effectNameStyle;
    private GUIStyle menuStyle;

    private List<string> logs = new List<string>();

    private void OnEnable()
    {
      Application.logMessageReceived += LogMessageReceived;
#if SHOW_SYSTEM_INFO
    Debug.Log(string.Format("{0}x{1}x{2}", Screen.width, Screen.height, Screen.currentResolution.refreshRate));
    Debug.Log(SystemInfo.operatingSystem);
    Debug.Log(string.Format("{0} x {1}", SystemInfo.processorCount, SystemInfo.processorType));
    Debug.Log(string.Format("Mem: {0:f1}gb", SystemInfo.systemMemorySize / 1000.0f));
    Debug.Log(SystemInfo.graphicsDeviceName);
    Debug.Log(SystemInfo.graphicsDeviceVersion);
    Debug.Log(string.Format("VMem: {0}mb", SystemInfo.graphicsMemorySize));
    Debug.Log(string.Format("Shader Level: {0:f1}", SystemInfo.graphicsShaderLevel / 10.0f));
    Debug.Log(string.Format("deviceName: {0}", SystemInfo.deviceName));
    Debug.Log(string.Format("deviceModel: {0}", SystemInfo.deviceModel));
#endif
      timeleft = updateInterval;

      Camera selectedCamera = null;
      Camera[] cameras = GameObject.FindObjectsOfType<Camera>();

      for (int i = 0; i < cameras.Length; ++i)
      {
        if (cameras[i].enabled == true)
        {
          selectedCamera = cameras[i];

          break;
        }
      }

      if (selectedCamera != null)
      {
        if (selectedCamera.GetComponents<ImageEffectBase>().Length == 0)
        {
          // Vintage Effects.
          selectedCamera.gameObject.AddComponent<VintageAmaro>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageBrannan>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageEarlybird>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageHefe>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageHudson>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageInkwell>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageLomofi>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageLordKelvin>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageNashville>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageRise>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageSierra>().enabled = false;
#if !UNITY_UNITY_WEBGL // No 'sampler3D' in gles2.
          selectedCamera.gameObject.AddComponent<VintageSlumber>().enabled = false;
#endif
          selectedCamera.gameObject.AddComponent<VintageSutro>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageToaster>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageValencia>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageWalden>().enabled = false;
          selectedCamera.gameObject.AddComponent<VintageXProII>().enabled = false;
          selectedCamera.gameObject.AddComponent<Vintage1977>().enabled = false;

          vintageImageEffects.AddRange(selectedCamera.GetComponents<ImageEffectBase>());

          // Vintage Extra Effects.
          vintageImageEffectsExtra.Add(selectedCamera.gameObject.AddComponent<VintageExtrasBrightnessContrastGamma>());
          vintageImageEffectsExtra.Add(selectedCamera.gameObject.AddComponent<VintageExtrasHueSaturation>());
          vintageImageEffectsExtra.Add(selectedCamera.gameObject.AddComponent<VintageExtrasFilm>());

          for (int i = 0; i < vintageImageEffectsExtra.Count; ++i)
            vintageImageEffectsExtra[i].enabled = false;
        }
        else
        {
          ImageEffectBase[] imageEffects = selectedCamera.GetComponents<ImageEffectBase>();
          for (int i = 0; i < imageEffects.Length; ++i)
          {
            if (imageEffects[i].IsExtraEffect == false)
              vintageImageEffects.Add(imageEffects[i]);
            else
              vintageImageEffectsExtra.Add(imageEffects[i]);
          }
        }

        Debug.Log(string.Format("{0} Vintage Effects. {1} Vintage Extra Effects.", vintageImageEffects.Count, vintageImageEffectsExtra.Count));

        for (int i = 0; i < vintageImageEffects.Count; ++i)
          vintageImageEffects[i].enabled = (i == guiSelection);

        if (musicClip != null)
        {
          AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
          audioSource.clip = musicClip;
          audioSource.loop = (slideEffectTime > 0.0f);
          audioSource.PlayDelayed(2.0f);

          if (slideEffectTime == 0.0f)
            slideEffectTime = (musicClip.length + 2.0f) / vintageImageEffects.Count;
        }
      }
      else
        Debug.LogWarning("No camera found.");

#if USE_SLIDESHOW
      slideShow = GameObject.FindObjectOfType<SlideShow>();
#endif
    }

    private void OnDisable()
    {
      Application.logMessageReceived -= LogMessageReceived;

      vintageImageEffects.Clear();
      vintageImageEffectsExtra.Clear();
    }

    private void Update()
    {
      timeleft -= Time.deltaTime;
      accum += Time.timeScale / Time.deltaTime;
      frames++;

      if (timeleft <= 0.0f)
      {
        fps = accum / frames;
        timeleft = updateInterval;
        accum = 0.0f;
        frames = 0;
      }

      if (slideEffectTime > 0.0f && vintageImageEffects.Count > 0)
      {
        effectTime += Time.deltaTime;
        if (effectTime >= slideEffectTime)
        {
          vintageImageEffects[guiSelection].enabled = false;

          guiSelection = (guiSelection < (vintageImageEffects.Count - 1) ? guiSelection + 1 : 0);

          vintageImageEffects[guiSelection].enabled = true;
          
          effectTime = 0.0f;
        }
      }

      if (Input.GetKeyUp(KeyCode.F1) == true)
        guiShow = !guiShow;

      if (Input.GetKeyUp(KeyCode.KeypadPlus) == true ||
          Input.GetKeyUp(KeyCode.KeypadMinus) == true ||
          Input.GetKeyUp(KeyCode.PageUp) == true ||
          Input.GetKeyUp(KeyCode.PageDown) == true)
      {
        int effectSelected = 0;

        slideEffectTime = 0.0f;

        for (int i = 0; i < vintageImageEffects.Count; ++i)
        {
          if (vintageImageEffects[i].enabled == true)
          {
            vintageImageEffects[i].enabled = false;

            effectSelected = i;

            break;
          }
        }

        if (Input.GetKeyUp(KeyCode.KeypadPlus) == true || Input.GetKeyUp(KeyCode.PageUp) == true)
        {
          guiSelection = (effectSelected < vintageImageEffects.Count - 1 ? effectSelected + 1 : 0);

          vintageImageEffects[guiSelection].enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.KeypadMinus) == true || Input.GetKeyUp(KeyCode.PageDown) == true)
        {
          guiSelection = (effectSelected > 0 ? effectSelected - 1 : vintageImageEffects.Count - 1);

          vintageImageEffects[guiSelection].enabled = true;
        }
      }

#if !UNITY_WEBPLAYER
    if (Input.GetKeyDown(KeyCode.Escape))
      Application.Quit();
#endif
    }

    private void OnGUI()
    {
#if UNITY_ANDROID || UNITY_IPHONE
      GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / 1280.0f, (float)Screen.height / 720.0f, 1.0f));
#endif
      if (vintageImageEffects.Count == 0)
        return;

      if (effectNameStyle == null)
      {
        effectNameStyle = new GUIStyle(GUI.skin.textArea);
        effectNameStyle.alignment = TextAnchor.MiddleCenter;
        effectNameStyle.fontSize = 22;
      }

      if (menuStyle == null)
      {
        menuStyle = new GUIStyle(GUI.skin.textArea);
        menuStyle.alignment = TextAnchor.MiddleCenter;
        menuStyle.fontSize = 14;
      }

      if (showEffectName == true && guiShow == false)
      {
        GUILayout.BeginArea(new Rect(20.0f, 20.0f, 160.0f, 30.0f),
          vintageImageEffects[guiSelection].GetType().ToString().Replace("VintageImageEffects.Vintage", string.Empty).ToUpper(),
          effectNameStyle);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width - 180.0f, 20.0f, 160.0f, 30.0f), "NORMAL", effectNameStyle);
        GUILayout.EndArea();
      }

      if (guiShow == false)
        return;

      GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
      {
        GUILayout.Space(guiMargen);

        if (GUILayout.Button("MENU", menuStyle, GUILayout.Width(80.0f)) == true)
          menuOpen = !menuOpen;

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("<<<", menuStyle) == true)
        {
          slideEffectTime = 0.0f;

          if (guiSelection > 0)
            guiSelection--;
          else
            guiSelection = vintageImageEffects.Count - 1;

          Event.current.Use();
        }

        GUI.contentColor = Color.white;
 
        GUILayout.Label(vintageImageEffects[guiSelection].GetType().ToString().Replace("VintageImageEffects.Vintage", string.Empty).ToUpper(),
          menuStyle,
          GUILayout.Width(150.0f));

        if (GUILayout.Button(">>>", menuStyle) == true)
        {
          slideEffectTime = 0.0f;

          if (guiSelection < vintageImageEffects.Count - 1)
            guiSelection++;
          else
            guiSelection = 0;
        }

        GUILayout.FlexibleSpace();

        if (fps < 30.0f)
          GUI.contentColor = Color.yellow;
        else if (fps < 15.0f)
          GUI.contentColor = Color.red;
        else
          GUI.contentColor = Color.green;

        GUILayout.Label(fps.ToString("000"), menuStyle, GUILayout.Width(40.0f));

        GUI.contentColor = Color.white;

        GUILayout.Space(guiMargen);
      }
      GUILayout.EndHorizontal();

      // Update
      for (int i = 0; i < vintageImageEffects.Count; ++i)
      {
        ImageEffectBase imageEffect = vintageImageEffects[i];

        if (guiSelection == i && imageEffect.enabled == false)
          imageEffect.enabled = true;

        if (imageEffect.enabled == true && guiSelection != i)
          imageEffect.enabled = false;
      }

      if (menuOpen == true)
      {
        GUILayout.BeginVertical("box", GUILayout.Width(guiWidth));
        {
          GUILayout.Space(guiMargen);

          // Vintage Effects.
          if (vintageImageEffects.Count > 0)
          {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, "box");
            {
              int effectChanged = -1;

              // Draw
              for (int i = 0; i < vintageImageEffects.Count; ++i)
              {
                ImageEffectBase imageEffect = vintageImageEffects[i];

                GUILayout.BeginHorizontal();
                {
                  if (imageEffect.enabled == true)
                    GUILayout.BeginVertical("box");

                  bool enableChanged = GUILayout.Toggle(imageEffect.enabled, guiTab + imageEffect.GetType().ToString().Replace("VintageImageEffects.Vintage", string.Empty));
                  if (enableChanged != imageEffect.enabled)
                    effectChanged = i;

                  if (imageEffect.enabled == true)
                  {
                    DrawCommonControls(imageEffect);

                    DrawCustomControls(imageEffect);

                    GUILayout.EndVertical();
                  }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(guiMargen * 0.5f);
              }

              // Update
              for (int i = 0; i < vintageImageEffects.Count; ++i)
              {
                ImageEffectBase imageEffect = vintageImageEffects[i];

                if (effectChanged == i)
                {
                  imageEffect.enabled = !imageEffect.enabled;

                  if (imageEffect.enabled == true)
                    guiSelection = i;
                }

                if (imageEffect.enabled == true && guiSelection != i)
                  imageEffect.enabled = false;
              }
            }
            GUILayout.EndScrollView();
          }
          else
            GUILayout.Label("No 'Vintage - Image Effects' found.");

          GUILayout.Space(guiMargen);

          // Vintage Extras.
          if (vintageImageEffectsExtra.Count > 0)
          {
            GUILayout.BeginVertical("box");
            {
              for (int i = 0; i < vintageImageEffectsExtra.Count; ++i)
              {
                ImageEffectBase imageEffect = vintageImageEffectsExtra[i];

                GUILayout.BeginHorizontal();
                {
                  if (imageEffect.enabled == true)
                    GUILayout.BeginVertical("box");

                  bool enableChanged = GUILayout.Toggle(imageEffect.enabled, guiTab + imageEffect.GetType().ToString().Replace("VintageImageEffects.VintageExtras", string.Empty));
                  if (enableChanged != imageEffect.enabled)
                    imageEffect.enabled = enableChanged;

                  if (imageEffect.enabled == true)
                  {
                    DrawExtraControls(imageEffect);

                    GUILayout.EndVertical();
                  }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(guiMargen * 0.5f);
              }
            }
            GUILayout.EndVertical();

            GUILayout.Space(guiMargen);
          }

          GUILayout.FlexibleSpace();

          GUILayout.BeginVertical("box");
          {
#if USE_SLIDESHOW
            if (slideShow != null)
            {
              slideShow.changeTime = GUILayout.Toggle(slideShow.changeTime > 0.0f, "Auto slideshow") ? 5.0f : 0.0f;

              GUILayout.BeginHorizontal();
              {
                if (GUILayout.Button("Prev") == true)
                  slideShow.PrevPicture();

                if (GUILayout.Button("Next") == true)
                  slideShow.NextPicture();
              }
              GUILayout.EndHorizontal();
            }
#endif
            GUILayout.Label("F1 - Hide/Show gui.");
            GUILayout.Label("PageUp\nPageDown - Change effects.");
          }
          GUILayout.EndVertical();

          GUILayout.Space(guiMargen);

          if (Debug.isDebugBuild == true && GUILayout.Button(consoleShow ? "Close console" : "Open console") == true)
          {
            consoleShow = !consoleShow;

            scrollLog.y = Mathf.Infinity;
          }

          if (GUILayout.Button(@"Open Web") == true)
            Application.OpenURL(@"http://www.ibuprogames.com/2015/05/04/vintage-image-efffects/");

#if !UNITY_WEBPLAYER
        if (GUILayout.Button(@"Quit") == true)
          Application.Quit();
#endif
        }
        GUILayout.EndVertical();

        // Log console
        if (consoleShow == true)
        {
          GUILayout.BeginVertical("box", GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 4));
          {
            scrollLog = GUILayout.BeginScrollView(scrollLog, false, true);
            {
              for (int i = 0; i < logs.Count; ++i)
                GUILayout.Label(logs[i]);
            }
            GUILayout.EndScrollView();
          }
          GUILayout.EndVertical();
        }
      }
    }

    private void DrawCommonControls(ImageEffectBase imageEffect)
    {
      // Amount.
      GUILayout.BeginHorizontal("box");
      {
        GUILayout.Label("Amount", GUILayout.Width(70));
        imageEffect.amount = GUILayout.HorizontalSlider(imageEffect.amount, 0.0f, 1.0f);
      }
      GUILayout.EndHorizontal();
    }

    private void DrawCustomControls(ImageEffectBase imageEffect)
    {
      System.Type type = imageEffect.GetType();

      if (type == typeof(VintageAmaro))
      {
        VintageAmaro effect = imageEffect as VintageAmaro;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Overlay", GUILayout.Width(60));
            effect.overlayStrength = GUILayout.HorizontalSlider(effect.overlayStrength, 0.0f, 1.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageEarlybird))
      {
        VintageEarlybird effect = imageEffect as VintageEarlybird;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Obturation", GUILayout.Width(60));
            effect.obturation = GUILayout.HorizontalSlider(effect.obturation, 0.0f, 2.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageWalden))
      {
        VintageWalden effect = imageEffect as VintageWalden;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Obturation", GUILayout.Width(60));
            effect.obturation = GUILayout.HorizontalSlider(effect.obturation, 0.0f, 2.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageXProII))
      {
        VintageXProII effect = imageEffect as VintageXProII;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Obturation", GUILayout.Width(60));
            effect.obturation = GUILayout.HorizontalSlider(effect.obturation, 0.0f, 2.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageHudson))
      {
        VintageHudson effect = imageEffect as VintageHudson;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Overlay", GUILayout.Width(60));
            effect.overlayStrength = GUILayout.HorizontalSlider(effect.overlayStrength, 0.0f, 1.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageRise))
      {
        VintageRise effect = imageEffect as VintageRise;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Overlay", GUILayout.Width(60));
            effect.overlayStrength = GUILayout.HorizontalSlider(effect.overlayStrength, 0.0f, 1.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageSutro))
      {
        VintageSutro effect = imageEffect as VintageSutro;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Obturation", GUILayout.Width(60));
            effect.obturation = GUILayout.HorizontalSlider(effect.obturation, 0.0f, 2.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (type == typeof(VintageSierra))
      {
        VintageSierra effect = imageEffect as VintageSierra;
        if (effect != null)
        {
          GUILayout.BeginHorizontal("box");
          {
            GUILayout.Label("Overlay", GUILayout.Width(60));
            effect.overlayStrength = GUILayout.HorizontalSlider(effect.overlayStrength, 0.0f, 1.0f);
          }
          GUILayout.EndHorizontal();
        }
      }
    }

    private void DrawExtraControls(ImageEffectBase imageEffect)
    {
      if (imageEffect.GetType() == typeof(VintageExtrasBrightnessContrastGamma))
      {
        VintageExtrasBrightnessContrastGamma extraEffect = imageEffect as VintageExtrasBrightnessContrastGamma;

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Brightness", GUILayout.Width(70));
          extraEffect.brightness = GUILayout.HorizontalSlider(extraEffect.brightness * 100.0f, -100.0f, 100.0f) * 0.01f;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Contrast", GUILayout.Width(70));
          extraEffect.contrast = GUILayout.HorizontalSlider(extraEffect.contrast * 100.0f, -100, 100) * 0.01f;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Gamma", GUILayout.Width(70));
          extraEffect.gamma = GUILayout.HorizontalSlider(extraEffect.gamma, 0.01f, 10.0f);
        }
        GUILayout.EndHorizontal();
      }
      else if (imageEffect.GetType() == typeof(VintageExtrasHueSaturation))
      {
        VintageExtrasHueSaturation extraEffect = imageEffect as VintageExtrasHueSaturation;

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Hue", GUILayout.Width(70));
          extraEffect.hue = GUILayout.HorizontalSlider(extraEffect.hue, 0.0f, 1.0f);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Saturation", GUILayout.Width(70));
          extraEffect.saturation = GUILayout.HorizontalSlider(extraEffect.saturation, 0.0f, 1.0f);
        }
        GUILayout.EndHorizontal();
      }
      else if (imageEffect.GetType() == typeof(VintageExtrasFilm))
      {
        VintageExtrasFilm extraEffect = imageEffect as VintageExtrasFilm;

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Grain", GUILayout.Width(70));
          extraEffect.grainStrength = GUILayout.HorizontalSlider(extraEffect.grainStrength, 0.0f, 100.0f);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Grain size", GUILayout.Width(70));
          extraEffect.grainSize = GUILayout.HorizontalSlider(extraEffect.grainSize, 0.0f, 0.1f);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label("Blink", GUILayout.Width(70));
          extraEffect.blinkStrenght = GUILayout.HorizontalSlider(extraEffect.blinkStrenght, 0.0f, 0.1f);
        }
        GUILayout.EndHorizontal();
      }
    }

    private void LogMessageReceived(string logString, string stackTrace, LogType type)
    {
      if (type == LogType.Error)
      {
        logs.Add("[<color=red>ERROR</color>] " + logString);
        logs.Add("[STACKTRACE] " + stackTrace);
      }
      else if (type == LogType.Warning)
        logs.Add("[<color=yellow>WARNING</color>] " + logString);
      else
        logs.Add("[LOG] " + logString);
    }
  }
}