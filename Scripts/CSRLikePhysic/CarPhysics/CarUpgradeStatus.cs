
[System.Serializable]
    public class CarUpgradeStatus
    {
        public byte levelOwned;

        public byte levelFitted;

        public byte evoOwned;

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Level Owned : ",
                this.levelOwned,
                ", Level Fitted : ",
                this.levelFitted
            });
        }

        public static byte Convert(int l)
        {
            if (l > (int) CarUpgradeData.NUM_UPGRADE_LEVELS)
            {
                l = (int) CarUpgradeData.NUM_UPGRADE_LEVELS;
            }
            else if (l < 0)
            {
                l = 0;
            }
            return (byte) l;
        }
    }
