using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class CarSpecification : ScriptableObject
{
    [SerializeField] private float m_mass = 1500;
    private float? m_minRpm;
    private float? m_maxRpm;
    [SerializeField] private float[] m_gearsRatios = {2.66F, 1.87F, 1.30F, 1, .74F, 0.5F};
    [SerializeField] private float m_differentialRatio = 3.42F;
    [SerializeField] private AnimationCurve m_rpmTorqueCurve;
    //[SerializeField] private MinMax m_maxPowerRpm = new MinMax(4000, 4500);
    [SerializeField] private float m_maxPowerRpm = 5000;
    [SerializeField] private float m_airResistanceConstant;
    [SerializeField] private float m_frontalArea;
    [SerializeField] private float m_length = 3;
    [SerializeField] private float m_rollingResistanceConstant;
    [SerializeField] private float m_wheelRadius = 0.29F;
    //[SerializeField] private float m_wheelMass = 75;
    [SerializeField] private float m_transmission = 1;
    [SerializeField] private float m_tyreFriction = 1;
    [SerializeField] private DriveWheelType m_wheelsType;
    [SerializeField]
    private string m_manufacturer;


    public int gearlength
    {
        get { return m_gearsRatios.Length; }
    }

    public AnimationCurve EngineCurve
    {
        get { return m_rpmTorqueCurve; }
    }

    public float maxPowerRpm
    {
        get { return m_maxPowerRpm; }
    }

    public float[] GearRatio
    {
        get
        {
            return m_gearsRatios;
        }
    }

    public float getEngineTorque(float rpm)
    {
        rpm /= 1000;
        return m_rpmTorqueCurve.Evaluate(rpm)*100;
    }

    public float getTorque(float rpm,int gear)
    {
        return getEngineTorque(rpm) * m_gearsRatios[gear] * m_differentialRatio;
    }

    public float[] gearTopSpeed
    {
        get
        {
            var speeds = new float[m_gearsRatios.Length];
            for (int i = 0; i < speeds.Length; i++)
            {
                speeds[i] = (maxPowerRpm/(m_gearsRatios[i]*m_differentialRatio))*2*Mathf.PI*m_wheelRadius*3.6F/60 - 9;
            }

            return speeds;
        }
    }

    public Range[] getGearTopSpeedInRpmRange(int minRpmRange)
    {
        var speeds = new Range[m_gearsRatios.Length];
        for (int i = 0; i < speeds.Length; i++)
        {
            speeds[i].min = (minRpmRange/ (m_gearsRatios[i] * m_differentialRatio)) * 2 * Mathf.PI * m_wheelRadius * 3.6F / 60 - 9;
            speeds[i].max = (maxPowerRpm/(m_gearsRatios[i]*m_differentialRatio))*2*Mathf.PI*m_wheelRadius*3.6F/60 - 9;
        }

        return speeds;
    }

    public float bestRpmForTopTorque
    {
        get
        {
            return (from key in m_rpmTorqueCurve.keys
                where key.value >= m_rpmTorqueCurve.keys.Max(k => k.value)
                select key.time)
                .ToArray()[0]*100;
        }
    }

    public float getTopTorque(int engineLevel)
    {
        return m_rpmTorqueCurve.keys.Max(k => k.value)*100;
    }

    public float averageTorque
    {
        get
        {
            return m_rpmTorqueCurve.keys.Average(k => k.value)*100;
        }
    }

    public float averageGearRatio
    {
        get { return m_gearsRatios.Average(); }
    }

    public float airResistanceConstant
    {
        get { return m_airResistanceConstant*0.5F*1.25F*m_frontalArea; }
    }

    public float rollingResistanceConstant
    {
        get { return m_rollingResistanceConstant * mass * -Physics.gravity.y; }
    }

    public DriveWheelType wheelsType
    {
        get { return m_wheelsType; }
    }

    public float mass
    {
        get { return m_mass; }
    }

    public float wheelRadius
    {
        get { return m_wheelRadius; }
    }

    public float differentialRatio
    {
        get
        {
            return m_differentialRatio;
        }
    }

    public float maxPower
    {
        get
        {
            return getPowerByRpm(maxPowerRpm);
        }
    }

    public float length
    {
        get
        {
            return m_length;
        }
    }

    public float Tranmission
    {
        get { return m_transmission; }
    }

    public float TyreFriction
    {
        get { return m_tyreFriction; }
    }

    public string manufacturer
    {
        get { return m_manufacturer; }
    }

    public int getRpm(float wheelsRpm, int gear)
    {
        var rpm = (int) Mathf.Max(wheelsRpm*m_gearsRatios[gear]*m_differentialRatio, 1000);
        //if (debug)
        //{
        //    //Debug.Log(gear+"   "+rpm + "   " + "   " + wheelsRpm + "   " + m_gearsRatios[gear] + "   " + m_differentialRatio);
        //}
        return rpm;
    }

    public float getPowerByRpm(float rpm)
    {
        var torque = m_rpmTorqueCurve.Evaluate(rpm / 1000) * 100;
        var power = torque * rpm / 7120.9056f;
        return power;
    }
}