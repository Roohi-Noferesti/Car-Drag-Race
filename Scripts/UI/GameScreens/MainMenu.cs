using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private StartScreen m_startScreen;

    void Start()
    {
        //ObjectLocator.resolve(out m_dataManager);
        //m_startScreen.name = m_dataManager.localPlayerInfo.Name;

        m_startScreen.nameChanged += m_startScreen_nameChanged;
    }

    void m_startScreen_nameChanged(string obj)
    {
        //PhotonNetwork.playerName = m_dataManager.localPlayerInfo.Name = obj;
    }
    public void playMultiplayer()
    {
        //GameManager.loadMultiPlayer();
    }

    public void playSingleplayer()
    {
        //GameManager.loadSinglePlayer();
    }

    public void quit()
    {
        //GameManager.quit();
    }
}
