using System;
using System.Collections;
using UnityEngine;

public class CPRAppManager : MonoBehaviour
{
    private enum RecordState { Idle, Ready, Recording,rating }
    private RecordState state = RecordState.Idle;

    private float tempTime;
    private float startTime;

    [SerializeField] private float recordingLimitTime = 10f;
    [SerializeField] private float maxDepthValue = 18f;
    [SerializeField] private float idleValueAbsThreshold = 1f;
    [SerializeField] private float idleMaxDepthThreshold = 18f;
    [SerializeField] private float readyValueThreshold = 2f;
    [SerializeField] private string serverIpAddress = "192.168.4.1";
    [SerializeField] private int serverPort = 1234;

    private NetworkManager network;
    private CPRDataProcessor processor;
    private InferenceManager inference;

    private GameObject roger;
    private RogerAnime rogerAnime;
    [SerializeField] private UICtrl ui;
    
    private AudioSource audioSource;
    [SerializeField] private QRCodeScanner qrCodeScanner;

    private Coroutine networkRoutine;

    void Start()
    {
        inference = this.gameObject.GetComponent<InferenceManager>();
        processor = new CPRDataProcessor();
        network = new NetworkManager(serverIpAddress,serverPort);
        network.OnMessageReceived += HandleMessage;
        audioSource = this.gameObject.GetComponent<AudioSource>();
        qrCodeScanner.OnObjectSpawned += setRoger;
        


        // 啟動 TCP 連線
        networkRoutine = StartCoroutine(network.Connect());
        network.Send("start\n");
        ui.setGuideObjectActive(true);

        ui.SetStatus("Idle");
    }

    void Update()
    {
        if (state == RecordState.Recording)
        {
            
            ui.SetTime(tempTime);

            if (tempTime > recordingLimitTime)
            {
                rating();
            }else if(roger == null){
                state = RecordState.Idle;
                ui.SetStatus("Idle");
            }
        }
    }

    private void rating()
    {        
        inference.turnOff();
        state = RecordState.rating;
        rogerAnime.switchIdle(true);
        ui.setSpeedBarActive(false);
        ui.setRatingBoxObjectActive(true);

        // 從 processor 取得平均速度與深度
        var (AVGSpeed, STDSpeed, AVGDepth,STDDepth) = processor.getAVGSpeedAndAVGDepth();

        // 更新到 UI
        ui.setAVGSpeedValue(AVGSpeed,STDSpeed);
        ui.setAVGDepthValue(AVGDepth,STDDepth);
    }
    private void setRoger(GameObject box){        
        if (rogerAnime != null){
            processor.onDepthUp -= rogerAnime.PlayUp;
            processor.onDepthDown -= rogerAnime.PlayDown;
        }
        roger = box;
        rogerAnime = roger.GetComponentInChildren<RogerAnime>();
        processor.onDepthUp += rogerAnime.PlayUp;
        processor.onDepthDown += rogerAnime.PlayDown;
    }
    public void ResetBTN()
    {
        if(roger != null){
            state = RecordState.Ready;
            tempTime = 0f;
            processor.Reset();
            ui.SetStatus("Ready");
            rogerAnime.switchIdle(true);;
        }else{            
            ui.SetStatus("no roger");
        }
    }
    public void RetryBTN(){
        state = RecordState.Idle;
        ui.setRatingBoxObjectActive(false);
        ui.setGuideObjectActive(true);
    }

    private void HandleMessage(string line)
    {
        tempTime += Time.time - startTime;
        if (float.TryParse(line, out float value))
        {
            value = maxDepthValue - value;
            if (state != RecordState.Idle && value < 0) value = 0;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                ui.SetValue(value);
            });
            switch (state)
            {
                case RecordState.Idle:
                    if(Math.Abs(value) > idleValueAbsThreshold && maxDepthValue - value > idleMaxDepthThreshold){
                        maxDepthValue -= value;
                    }
                    ui.SetMaxValue(maxDepthValue);
                    if(ui.getGuidePage() == 2){
                        ResetBTN();
                        inference.OnRunButtonPressed();
                    }
                    break;

                case RecordState.Ready:
                    if (value > readyValueThreshold)
                    {
                        state = RecordState.Recording;
                        startTime = Time.time;
                        ui.setGuideObjectActive(false);
                        ui.setSpeedBarActive(true);
                        rogerAnime.switchIdle(false);
                    }
                    break;

                case RecordState.Recording:
                    if (processor.Process(tempTime, value))
                    {
                        if (processor.IsFalling())
                        {
                            audioSource.Play();        
                            ui.setSpeedBarValue(processor.getInterval());
                        }
                        else
                        {
                            var minima = processor.GetMinima();
                            ui.setDepthEffect(minima[minima.Count - 1].value);
                        }
                    }
                    break;
            }
        }
    }
    
    private void OnDestroy()
    {
        if (networkRoutine != null) StopCoroutine(networkRoutine);
        network.Close();
    }
}
