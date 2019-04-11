using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{

    public float fps = 16.0f;
    public Sprite[] frames;
    public bool roop = false;


    private int frameIndex;
    private SpriteRenderer spriteRenderer;
    private Image image;

    void Start()
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            NextFrameRenderer();
            InvokeRepeating("NextFrameRenderer", 1 / fps, 1 / fps);
        }
        else
        {
            image = GetComponent<Image>();
            NextFrameImage();
            InvokeRepeating("NextFrameImage", 1 / fps, 1 / fps);
        }
    }

    /// <summary>
    /// 次のフレーム処理(Renderer)
    /// </summary>
    void NextFrameRenderer()
    {
        spriteRenderer.sprite = frames[frameIndex];
        frameIndex = (frameIndex + 0001) % frames.Length;
        if (!roop && frameIndex == frames.Length - 1) Destroy(gameObject);
    }

    /// <summary>
    /// 次のフレーム処理(Image)
    /// </summary>
    void NextFrameImage()
    {
        image.sprite = frames[frameIndex];
        frameIndex = (frameIndex + 0001) % frames.Length;
        if (!roop && frameIndex == frames.Length - 1) Destroy(gameObject);
    }
}
