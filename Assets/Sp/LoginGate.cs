using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginGate : MonoBehaviour
{
    [Header("Refs")]
    public TMP_InputField userInput;
    public TMP_InputField passInput;
    public Button loginButton;
    public PopupUI warningPopup;       // 你的 PopupUI（警告页）

    [Header("Rules")]
    public int userMin = 1;
    public int passMin = 1;

    void Start()
    {
        if (loginButton) loginButton.onClick.AddListener(TryLogin);
        RefreshButton();
        userInput.onValueChanged.AddListener(_ => RefreshButton());
        passInput.onValueChanged.AddListener(_ => RefreshButton());
    }

    void RefreshButton()
    {
        bool ok = userInput && passInput &&
                  !string.IsNullOrEmpty(userInput.text) && userInput.text.Length >= userMin &&
                  !string.IsNullOrEmpty(passInput.text) && passInput.text.Length >= passMin;
        if (loginButton) loginButton.interactable = ok;
    }

    void TryLogin()
    {
        bool ok = loginButton == null || loginButton.interactable;
        if (!ok)
        {
            if (warningPopup) warningPopup.PopOpen();
            else Debug.LogWarning("WarningPopup 未绑定。");
            return;
        }

        // TODO: 在这里执行你的登录逻辑（校验/请求/切场景）
        Debug.Log($"Login OK: user={userInput.text}, pass=******");
    }
}

