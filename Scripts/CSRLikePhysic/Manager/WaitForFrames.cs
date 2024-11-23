using UnityEngine;

public class WaitForFrames : CustomYieldInstruction
{
    private readonly int m_frameCount;
    private readonly int m_startFrame;
    public WaitForFrames(int frameCount)
    {
        m_frameCount = frameCount;
        m_startFrame = Time.frameCount;
    }
    public override bool keepWaiting
    {
        get { return Time.frameCount - m_startFrame <= m_frameCount; }
    }
}