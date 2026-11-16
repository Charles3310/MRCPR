using UnityEngine;
using PassthroughCameraSamples;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
using TMPro;
using System.Collections;
using Meta.XR.Samples; // 用於 PassthroughCameraUtils
using System;

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField] private WebCamTextureManager webCamManager;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Image scanningBox;
    [SerializeField] private GameObject boxPrefab;
    private BarcodeReader reader;
    public event Action<GameObject> OnObjectSpawned;
    private GameObject box=null;
    private Coroutine scanRoutine = null;

    void Start()
    {
        reader = new BarcodeReader
        {
            AutoRotate = true,
            Options = new DecodingOptions
            {
                PossibleFormats = new[] { BarcodeFormat.QR_CODE },
                TryHarder = true,
                #pragma warning disable CS0618
                TryInverted = true
                #pragma warning restore CS0618
            }
        };
        startScanQRcode();        
    }

    public void startScanQRcode(){
        if(this.box != null){
            Destroy(this.box);
            OnObjectSpawned?.Invoke(this.box);
        }
        this.scanningBox.gameObject.SetActive(true);
        if(scanRoutine == null)scanRoutine = StartCoroutine(ScanQRCodeLoop());
    }

    IEnumerator ScanQRCodeLoop()
    {
        while (this.box == null)
        {
            TryDetectAndSpawn();
            yield return new WaitForSeconds(1f); // 每秒執行一次
        }
        
        this.scanningBox.gameObject.SetActive(false);
        scanRoutine = null;
    }

    void TryDetectAndSpawn()
    {
        var tex = webCamManager.WebCamTexture;

        if (tex != null && tex.isPlaying)
        {
            try
            {
                Color32[] pixels = tex.GetPixels32();
                int width = tex.width;
                int height = tex.height;

                var result = reader.Decode(pixels, width, height);

                if (result != null && result.ResultPoints != null && result.ResultPoints.Length >= 3)
                {
                    // 計算 QR code 中心點（像素座標）
                    Vector2 center2D = Vector2.zero;
                    foreach (var point in result.ResultPoints)
                    {
                        center2D += new Vector2(point.X, point.Y);
                    }
                    center2D /= result.ResultPoints.Length;

                    // 轉成整數像素座標並反轉 y 軸（因為 WebCamTexture y 軸是反的）
                    Vector2Int pixel = new Vector2Int((int)center2D.x, height - (int)center2D.y);

                    // 用 PassthroughCameraUtils 轉成世界空間的 Ray                
                    Ray ray = PassthroughCameraUtils.ScreenPointToRayInWorld(PassthroughCameraEye.Right, pixel);
                    

                    // 在偵測到的位置生成一個新的預製件實例
                    if (Physics.Raycast(ray, out RaycastHit hit)){
                        // 生成物件
                        this.box = Instantiate(boxPrefab, hit.point+ new Vector3(0,0.0f,0), Quaternion.identity);

                        this.box.transform.LookAt(ray.origin);

                        // 只保留 Y 軸旋轉，X/Z 重置
                        this.box.transform.eulerAngles = new Vector3(0, this.box.transform.eulerAngles.y, 0);
                        OnObjectSpawned?.Invoke(this.box);
                    }

                }
            }
            catch (System.Exception e)
            {
                debugText.text = "QRCode decode error: " + e.Message;
            }
        }
    }
}
