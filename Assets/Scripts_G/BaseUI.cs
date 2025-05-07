using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

// ��� UI�� �⺻ ������ �����ϴ� �߻� Ŭ����
public abstract class BaseUI : MonoBehaviour
{
    protected UIManager uiManager;

    public virtual void Init(UIManager uiManager)
    {
        this.uiManager = uiManager;
    }

    // ���� UI ���� ���� (�ڽ� Ŭ�������� �����ؾ���)
    protected abstract UIState GetUIState();
    // ���޵� ���¿� ���� UI�� ���°� ��ġ�ϸ� Ȱ��ȭ, �ƴϸ� ��Ȱ��ȭ
    public void SetActive(UIState state)
    {
        gameObject.SetActive(GetUIState() == state);
    }
}
