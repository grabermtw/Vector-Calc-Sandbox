using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWithTrigger : MonoBehaviour
{
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(index);
    }
}
