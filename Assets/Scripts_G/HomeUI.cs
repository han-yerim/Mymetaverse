using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HomeUI : BaseUI
{
    Button startButton;
    Button exitButton;
    Button manualButton;
    GameObject manualPanel;

    bool isManualOpen = false;

    protected override UIState GetUIState()
    {
        return UIState.Home;
    }

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);

        // ���� ������Ʈ���� ��ư���� ã�Ƽ� ����
        startButton = transform.Find("StartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        manualButton = transform.Find("ManualButton").GetComponent<Button>();
        manualPanel = transform.Find("ManualPanel").gameObject;

        manualPanel.SetActive(false);

        // ��ư Ŭ���� �̺�Ʈ ����
        startButton.onClick.AddListener(OnClickStartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
        manualButton.onClick.AddListener(OnClickManualButton);

    }

    void OnClickStartButton()
    {
        uiManager.OnClickStart();
    }

    void OnClickManualButton()
    {
        manualPanel.SetActive(true);
        isManualOpen = true;
    }

    void OnClickExitButton()
    {
        uiManager.OnclickExit();
    }
    void Update()
    {
        if (isManualOpen && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                manualPanel.SetActive(false);
                isManualOpen = false;
            }
        }
    }
}