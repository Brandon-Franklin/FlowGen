using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class PerlinNoise : MonoBehaviour {

    public int pixWidth;
    public int pixHeight;
    public float xOrg;
    public float yOrg;
    public float scale = 1.0F;
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;

        //CalcNoise();
        rend.materials[0].mainTexture = noiseTex;

        //for (int x = 0; x < 30; x++)
        //{
        //    for (int y = 0; y < 30; y++)
        //    {
        //        Debug.Log(noiseTex.GetPixel(x, y).r);
        //    }
        //}
    }
    void CalcNoise()
    {
        float y = 0.0F;
        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        byte[] bytes = noiseTex.EncodeToPNG();

        File.WriteAllBytes("Assets/perlinNoise.png", bytes);

    }

    private void Update()
    {

        
    }
}
