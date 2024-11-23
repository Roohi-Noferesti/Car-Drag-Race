using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour
{
    private IEnumerator m_cor;

    void Awake()
    {
        m_cor = testCoro();
    }
    IEnumerator testCoro()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("coroutine finished here");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("coroutine started here");
            StartCoroutine(m_cor);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("coroutine stopped here");
            StopCoroutine(m_cor);
        }
    }
}
