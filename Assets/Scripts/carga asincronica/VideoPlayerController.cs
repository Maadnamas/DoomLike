using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;
    [SerializeField] private RawImage rawImage;

    private void Start()
    {
        rawImage.enabled = false;
        player.prepareCompleted += OnPrepared;
        player.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        rawImage.enabled = true;
        vp.Play();
    }

}