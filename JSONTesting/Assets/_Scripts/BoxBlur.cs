using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// http://danjohnmoran.com/Shaders/102/
/// </summary>
[ExecuteInEditMode]
public class BoxBlur : MonoBehaviour {
    public static BoxBlur currentBlur;
    public Material BlurMaterial;
    [Range(0, 10)]
    public int Iterations;
    [Range(0, 4)]
    public int DownRes;

    public bool doTheBlur;

    private void Awake()
    {
        currentBlur = this;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (!doTheBlur)
        {
            return;
        }

        // Scale down the immage
        int width = src.width >> DownRes;
        int height = src.height >> DownRes;

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(src, rt);

        for (int i = 0; i < Iterations; i++)
        {
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, BlurMaterial);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, dst);
        RenderTexture.ReleaseTemporary(rt);
    }
}
