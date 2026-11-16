using UnityEngine;
using System.Collections;
using TMPro;
using PassthroughCameraSamples;
using UnityEngine.UI;
using Unity.InferenceEngine; 

public class InferenceManager : MonoBehaviour
{
    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private WebCamTextureManager webCamManager;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Renderer quadRenderer;

    private Worker worker;
    private TextureTransform textureTransform;
    private Tensor<float> inputTensor;
    private enum InferenceResult { Error, Bent, Straight }

    private Coroutine inferenceRoutine;
    private bool isEnabled;

    public void turnOff(){
        isEnabled = false;
    }

    void Start()
    {
        // 初始化模型
        worker = new Worker(
            ModelLoader.Load(modelAsset),
            BackendType.GPUCompute);

        // 設定影像轉換格式
        textureTransform = new TextureTransform();

        // 預先建立輸入 Tensor（可重複使用）
        inputTensor = new Tensor<float>(new TensorShape(1, 3, 640, 640));
    }

    public void OnRunButtonPressed()
    {
        if (inferenceRoutine != null)
            StopCoroutine(inferenceRoutine);
        isEnabled = true;   
        inferenceRoutine = StartCoroutine(RunInferenceLoop());
    }

    private IEnumerator RunInferenceLoop()
    {
        statusText.text = "Running...";
        var turn = 0;        
        var bent = 0;
        var straight = 0;   
        while (isEnabled)
        {
            var isStraight = runInferenceOnce();         

            switch(isStraight){
                case InferenceResult.Straight:
                    straight++;
                    break;
                case InferenceResult.Bent:
                    bent++;
                    break;
                case InferenceResult.Error:
                    enabled = false;
                    break;
            }

            turn++;

            if (turn == 5)
            {
                string result = bent > straight ? "Please straighten your arm." : "";
                statusText.text = $"{result}";
                bent = straight = turn = 0;
            }
            yield return new WaitForSeconds(0.2f);

        }
    }

    private InferenceResult runInferenceOnce()
    {
        var tex = webCamManager.WebCamTexture;      

        if (tex == null || !tex.isPlaying)
        {
            statusText.text = "Camera not ready";
            return InferenceResult.Error;
        }

        try
        {
            TextureConverter.ToTensor(tex, inputTensor, textureTransform);
            worker.Schedule(inputTensor);

            var output = worker.PeekOutput() as Tensor<float>;
            using var readableOutput = output.ReadbackAndClone();
            if (readableOutput == null)
            {
                statusText.text = "Output is null!";
                return InferenceResult.Error;
            }
            
            return readableOutput[0] < readableOutput[1]
                ? InferenceResult.Straight
                : InferenceResult.Bent;
        }
        catch (System.Exception e)
        {
            statusText.text = "Inference error: " + e.Message;
            return InferenceResult.Error;
        }
    }

    private void OnDestroy()
    {
        inferenceRoutine = null;
        inputTensor?.Dispose();
        worker?.Dispose();
    }
}
