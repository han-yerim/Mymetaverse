using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "GameScene";

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetSceneName);
        }

        IEnumerator LoadSceneWithDelay()
        {
            yield return new WaitForSeconds(1.0f);
        }
    }
}
