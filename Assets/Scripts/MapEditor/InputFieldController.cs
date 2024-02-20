using UnityEngine;
using TMPro; // 引用TextMeshPro命名空间
using UnityEngine.UI;
using Unity.Mathematics; // 引用UI命名空间
public class InputFieldController : MonoBehaviour
{
    private TMP_InputField inputField; // TMP_InputField组件的引用
    private Button increaseButton; // 增加按钮的引用
    private Button decreaseButton; // 减少按钮的引用
    const int maxVal = 25;

    void Awake()
    {
        // 获取当前GameObject上的TMP_InputField组件
        inputField = GetComponentInChildren<TMP_InputField>();

        // 自动查找增加和减少按钮的引用
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.name == "IncreaseButton") // 按钮的名字需要和实际匹配
            {
                increaseButton = btn;
                increaseButton.onClick.AddListener(IncreaseValue);
            }
            else if (btn.gameObject.name == "DecreaseButton") // 按钮的名字需要和实际匹配
            {
                decreaseButton = btn;
                decreaseButton.onClick.AddListener(DecreaseValue);
            }
        }
    }
    void IncreaseValue()
    {
        UpdateInputFieldValue(1);
    }
    void DecreaseValue()
    {
        UpdateInputFieldValue(-1);
    }
    void UpdateInputFieldValue(int change)
    {
        if (int.TryParse(inputField.text, out int currentValue))
        {
            currentValue += change;
            currentValue = math.max(currentValue,0);
            currentValue = math.min(currentValue,maxVal);
            inputField.text = currentValue.ToString();
        }
        else
        {
            Debug.Log($"Invalid input: {inputField.text}");
            inputField.text = "0"; 
        }
    }
}
