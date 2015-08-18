///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Vintage - Image Effects.
// Copyright (c) Ibuprogames. All rights reserved.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;

using UnityEngine;

namespace VintageImageEffects.Demo
{
  /// <summary>
  /// Slideshow demo.
  /// </summary>
  [RequireComponent(typeof(Camera))]
  public sealed class SlideShow : MonoBehaviour
  {
    /// 0 no change.
    public float changeTime = 5.0f;

    public Shader guiShader;

    public List<Texture2D> slideTextures = new List<Texture2D>();

    private float timeToChange = 0.0f;

    private int currentSlide = 0;

    private Material guiMaterial = null;

    public void NextPicture()
    {
      currentSlide = (currentSlide < (slideTextures.Count - 1) ? currentSlide + 1 : 0);

      timeToChange = changeTime = 0.0f;
    }

    public void PrevPicture()
    {
      currentSlide = (currentSlide > 0 ? currentSlide - 1 : slideTextures.Count - 1);

      timeToChange = changeTime = 0.0f;
    }

    private void Update()
    {
      if (changeTime > 0.0f)
      {
        timeToChange += Time.deltaTime;

        if (timeToChange >= changeTime)
        {
          currentSlide = (currentSlide < (slideTextures.Count - 1) ? currentSlide + 1 : 0);

          timeToChange = 0.0f;
        }
      }

      if (slideTextures.Count > 0)
      {
        if (Input.GetKeyDown(KeyCode.PageDown) == true)
          PrevPicture();

        if (Input.GetKeyDown(KeyCode.PageUp) == true)
          NextPicture();
      }
    }

    private void OnPostRender()
    {
      if (slideTextures.Count > 0)
      {
        if (guiMaterial == null)
        {
          if (guiShader != null)
            guiMaterial = new Material(guiShader);

          if (guiMaterial == null)
          {
            Debug.LogError("guiMaterial null.");

            this.enabled = false;

            return;
          }
        }

        GL.PushMatrix();

        guiMaterial.SetPass(0);
        guiMaterial.SetTexture("_MainTex", slideTextures[currentSlide]);

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        //   0       3
        //   +-------+
        //   |      /|
        //   |    /  |
        //   |  /    |
        //   |/      |
        //   +-------+
        //   1       2

        // 0
        GL.TexCoord(new Vector3(0.0f, 0.0f, 0.0f));
        GL.Vertex3(0.0f, 0.0f, 0);

        // 1
        GL.TexCoord(new Vector3(0.0f, 1.0f, 0.0f));
        GL.Vertex3(0.0f, 1.0f, 0);

        // 2
        GL.TexCoord(new Vector3(1.0f, 1.0f, 0.0f));
        GL.Vertex3(1.0f, 1.0f, 0);

        // 3
        GL.TexCoord(new Vector3(1.0f, 0.0f, 0.0f));
        GL.Vertex3(1.0f, 0.0f, 0);

        GL.End();
        GL.PopMatrix();
      }
    }
  }
}