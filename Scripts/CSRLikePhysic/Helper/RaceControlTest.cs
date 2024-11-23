using System.Collections;
using UnityEngine;

public class RaceControlTest : MonoBehaviour
{
    //[SerializeField] private CarPhysics m_carPhysics;
    [SerializeField] private RaceCar m_car;
    [SerializeField] private CarInfo m_carInfo;
    [SerializeField] private CarVisuals m_carVisual;
    [SerializeField] private CarUpgradeSetup m_carUpgradeSetup;
    public static bool gameStarted;
    public static RaceCar Car { get; private set; }
    void Awake()
    {
        m_carUpgradeSetup = new CarUpgradeSetup();
        m_car.Initialise(m_carInfo, m_carVisual, true, m_carUpgradeSetup);
        Car = m_car;
        //m_carPhysics.Initialise();
        //CompetitorManager.Create();
    }

    IEnumerator Start()
    {
        gameStarted = false;
        int i = 3;
        while (i>0)
        {
            yield return new WaitForSeconds(1);
            i--;
            Debug.Log(i);
        }
        gameStarted = true;
        yield return 0;
        m_car.physics.GearBox.GearShiftUp();
    }

    void FixedUpdate()
    {
        m_car.physics.RunCarPhysics();
        //m_carPhysics.RunCarPhysics();
    }
}
