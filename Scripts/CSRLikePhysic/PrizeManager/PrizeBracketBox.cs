using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PrizeBracketBox
{
    public GameObject BracketRoot;

    public GameObject PercentileSticker;

    public TextMeshProUGUI BeatLabel;

    public TextMeshProUGUI BeatRPValue;

    public TextMeshProUGUI GoldOnlyPrize;

    public Image PrizeImage;

    public Transform ProCarOverlayParent;

    public GameObject LoadingSpinner;
}
