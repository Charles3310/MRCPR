using UnityEngine;
using UnityEngine.UI;

public class CprBook : MonoBehaviour
{
    public static CprBook instance; // 單例實例



    void Awake()
    {
        // 如果還沒有實例，將自己設為實例
        if (instance == null)
        {
            instance = this;
        }
        // 如果實例已存在，銷毀這個重複的物件
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
    }

    public void closeBookBTN()
    {
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        // 當物件被銷毀時，清空靜態實例，以便下次可以創建新的
        if (instance == this)
        {
            instance = null;
        }
    }

   
}


