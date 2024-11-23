using UnityEngine;

public class TestSequence : MonoBehaviour
{
    [SerializeField] private string m_sequenceName;

    private void Start()
    {
        SequenceManager.Instance.PlaySequence(m_sequenceName);
    }
}
