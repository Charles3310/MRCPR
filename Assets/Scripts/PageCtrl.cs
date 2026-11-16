using UnityEngine;
using UnityEngine.UI;

public class PageCtrl : MonoBehaviour
{
    
    [SerializeField] private Sprite[] pages;
    [SerializeField] private Image background;
    [SerializeField] private GameObject previousButton,nextButton;
    private int currentPageIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateButtons();
    }

    public void nextPageBTN()
    {
        GoToPage(currentPageIndex + 1);
    }

    public void previousPageBTN()
    {
        GoToPage(currentPageIndex - 1);
    }

    private void GoToPage(int pageIndex)
    {
        // 邊界檢查，確保 pageIndex 不會超出陣列範圍
        if (pages == null || pages.Length == 0 || pageIndex < 0 || pageIndex >= pages.Length)
        {
            return; // 如果目標頁面無效，則不執行任何操作
        }
        this.currentPageIndex = pageIndex;
        background.sprite = pages[currentPageIndex];
        UpdateButtons();
    }
    private void UpdateButtons()
    {
        if(this.currentPageIndex == 0){
            this.previousButton.SetActive(false);
        }
        else if(this.currentPageIndex == 2){            
            this.nextButton.SetActive(false);
        }
        else{            
            this.nextButton.SetActive(true);            
            this.previousButton.SetActive(true);
        }
    }
    public int getCurrentPage(){
        return this.currentPageIndex;
    }
}
