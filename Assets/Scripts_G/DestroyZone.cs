using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // ���� �� ������Ʈ�� �浹���� �� �ڵ� ȣ��
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Rubble")) // �浹�� ������Ʈ�� �̸��� "Rubble"�� ���
        {
            Destroy(collision.gameObject); // �ش� ������Ʈ�� ������ ����
        }
    }
}
