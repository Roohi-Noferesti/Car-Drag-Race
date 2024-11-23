using System.Linq;
using UnityEngine;

public class CarUpgradeSpecifiction : ScriptableObject
{
    [SerializeField] private float[] m_shiftDelays = {0.4F, 0.3F, 0.2F, 0.1F};
    [SerializeField] private float[] m_engineUpgrades = {1, 1.1f, 1.2f, 1.3f};
    [SerializeField] private float[] m_bodyUpgrades = {1, 0.9F, 0.8F, 0.7F};
    [SerializeField] private float[] m_nitrousUpgrade = {5, 7, 9, 11, 13};
    [SerializeField] private float[] m_tyreUpgrades = {1, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f};
    [SerializeField] private float[] m_turboUpgrade = {1, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f};


    public int shiftLevelCount
    {
        get { return m_shiftDelays.Length; }
    }

    public int engineLevelCount
    {
        get { return m_engineUpgrades.Length; }
    }

    public int bodyLevelCount
    {
        get { return m_bodyUpgrades.Length; }
    }

    public int nitrousLevelCount
    {
        get { return m_nitrousUpgrade.Length; }
    }

    public int tyreLevelCount
    {
        get { return m_tyreUpgrades.Length; }
    }

    public int turboLevelCount
    {
        get { return m_turboUpgrade.Length; }
    }


    public float getShiftDelayModifer(int level)
    {
        if (level >= m_shiftDelays.Length)
            return m_shiftDelays.Last();
        return m_shiftDelays[level];
    }

    public float getEngineModifier(int level)
    {
        if (level >= m_engineUpgrades.Length)
            return m_engineUpgrades.Last();
        return m_engineUpgrades[level];
    }

    public float getMassModifier(int level)
    {
        if (level >= m_bodyUpgrades.Length)
            return m_bodyUpgrades.Last();
        return m_bodyUpgrades[level];
    }

    public float getNitrousDuration(int level)
    {
        if (level >= m_nitrousUpgrade.Length)
            return m_nitrousUpgrade.Last();
        return m_nitrousUpgrade[level];
    }

    public float getTyreModifer(int level)
    {
        if (level >= m_tyreUpgrades.Length)
            return m_tyreUpgrades.Last();
        return m_tyreUpgrades[level];
    }

    public float getTurboModifer(int level)
    {
        if (level >= m_turboUpgrade.Length)
            return m_turboUpgrade.Last();
        return m_turboUpgrade[level];
    }
}
