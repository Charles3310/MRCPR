using UnityEngine;
using PassthroughCameraSamples;
public class PassthroughCameraDisplay : MonoBehaviour
{
    [SerializeField] private WebCamTextureManager WebCamManager;
    [SerializeField] private Renderer quadRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // if(WebCamManager.WebCamTexture != null){
        //     quadRenderer.material.mainTexture = WebCamManager.WebCamTexture;
        // }
    }
}
