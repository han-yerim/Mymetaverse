using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // 뭔가 이 오브젝트와 충돌했을 때 자동 호출
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Rubble")) // 충돌한 오브젝트의 이름이 "Rubble"인 경우
        {
            Destroy(collision.gameObject); // 해당 오브젝트를 씬에서 제거
        }
    }
}
