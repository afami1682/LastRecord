using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatinController : MonoBehaviour {

    public float fps = 16.0f;
    public Sprite[] frames;
    public bool roop = false;

    private int frameIndex;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    void NextFrame()
    {
        spriteRenderer.sprite = frames[frameIndex];
///            sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 0001) % frames.Length;
        if (!roop && frameIndex == frames.Length - 1) Destroy(gameObject);
    }
}
