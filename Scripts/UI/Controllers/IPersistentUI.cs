using KingKodeStudio;

internal interface IPersistentUI
{
	void OnScreenChanged(ScreenID screen);

	void Show(bool show);
}
