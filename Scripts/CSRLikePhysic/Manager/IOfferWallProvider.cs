using System;

public interface IOfferWallProvider
{
	void FetchContent();

	void CheckForRewards();

	bool ShowContent();
}
