using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxValueText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI timeText;
    

    private Slider speedBar;
    private PageCtrl guidePageCtrl;
    private Image goodImage;
    [SerializeField] private GoodPool goodPool;

    [SerializeField] private GameObject speedBarObject;
    [SerializeField] private GameObject guideObject;
    [SerializeField] private GameObject goodObject;
    [SerializeField] private Sprite[] goodImages;
    
    [SerializeField] private GameObject ratingBoxObject;    
    [SerializeField] private TextMeshProUGUI AVGSpeedTMP;
    [SerializeField] private TextMeshProUGUI AVGDepthTMP;

    [Header("Speed Bar Settings")]
    [SerializeField] private float speedFastThreshold = 0.1f;
    [SerializeField] private float speedMediumThreshold1 = 0.5f;
    [SerializeField] private float speedMediumThreshold2 = 0.6f;
    [SerializeField] private float speedSlowThreshold = 1.2f;

    [Header("Depth Effect Settings")]
    [SerializeField] private float depthShallowThreshold = 5f;
    [SerializeField] private float depthGoodThreshold = 6f;

    public void SetStatus(string msg) => statusText.text = msg;
    public void SetMaxValue(float val) => maxValueText.text = $"maxvalue: {val:F2} cm";
    public void SetValue(float val) => valueText.text = $"distance: {val:F2} cm";
    public void SetTime(float t) => timeText.text = t.ToString("F2");

    private void Start() {
        speedBar = speedBarObject.GetComponent<Slider>();
        guidePageCtrl = guideObject.GetComponent<PageCtrl>();
        goodImage = goodObject.GetComponent<Image>();
        goodPool = speedBarObject.GetComponent<GoodPool>();
    }
    public int getGuidePage(){
        return guidePageCtrl.getCurrentPage();
    }
    public void setGuideObjectActive(bool active){
        guideObject.SetActive(active);
    }
    public void setSpeedBarActive(bool active){
        speedBarObject.SetActive(active);
    }
    public void setRatingBoxObjectActive(bool active){
        ratingBoxObject.SetActive(active);
    }

    public void setAVGSpeedValue(float value, float std)
    {
        if (std < 20)
        {
            var result = $"{value:F0} per/min";
            if (value < 100)
            {
                result += ",\ntoo slow!";
            }
            else if (value < 120)
            {
                result += ",\nmeets the standard!";
            }
            else
            {
                result += ",\ntoo fast!";
            }
            AVGSpeedTMP.text = result;
        }
        else
        {
            AVGSpeedTMP.text = $"Too unstable!";
        }
    }

    public void setAVGDepthValue(float value, float std)
    {
        if (std < 1f)
        {
            var result = $"{value:F1} cm";
            if (value < 5)
            {
                result += ",\ntoo shallow!";
            }
            else if (value < 6)
            {
                result += ",\nmeets the standard!";
            }
            else
            {
                result += ",\ntoo deep!";
            }
            AVGDepthTMP.text = result;
        }
        else
        {
            AVGDepthTMP.text = $"Too unstable!";
        }
    }

    public void setSpeedBarValue(float interval){
        SetStatus(interval.ToString("F2"));
        float speedValue;
        if (interval <= speedFastThreshold)
            speedValue = 1f; // 太快
        else if (interval <= speedMediumThreshold1)
            speedValue = Mathf.Lerp(1f, 0.65f, Mathf.InverseLerp(speedFastThreshold, speedMediumThreshold1, interval));
        else if (interval <= speedMediumThreshold2)
            speedValue = Mathf.Lerp(0.65f, 0.35f, Mathf.InverseLerp(speedMediumThreshold1, speedMediumThreshold2, interval));
        else if (interval < speedSlowThreshold)
            speedValue = Mathf.Lerp(0.35f, 0f, Mathf.InverseLerp(speedMediumThreshold2, speedSlowThreshold, interval));
        else
            speedValue = 0f; // 太慢

        speedBar.value = speedValue;
    }
    
    public void setDepthEffect(float value){  
        var goodImageIndex = 0;      
        if(value < depthShallowThreshold){
            goodImageIndex = 0; // 太淺
        }else if(value <= depthGoodThreshold){
            goodImageIndex = 1;//剛好
        }else if(value > depthGoodThreshold){
            goodImageIndex = 2;//太深
        }
        // ✅ 從物件池取得物件（不再 Instantiate）
        GameObject newGood = goodPool.Get();
        newGood.GetComponent<Image>().sprite = goodImages[goodImageIndex];
    }
}
