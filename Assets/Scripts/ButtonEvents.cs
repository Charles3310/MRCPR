using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvents : MonoBehaviour
{
    [SerializeField] private GameObject bookPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void btnLoadScene(){
        string sceneName = this.gameObject.name.Replace("LoadBTN", "");
        SceneManager.LoadScene(sceneName);
    }

    public void exitApplication()
    {
        Application.Quit();
    }

    public void openBook(){
        // 如果書本實例不存在，並且預製件已指定，則創建一個新的實例
        if (CprBook.instance == null && bookPrefab != null)
        {
            Instantiate(bookPrefab);
        }
    }
}

