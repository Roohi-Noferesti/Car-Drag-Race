
    public struct CarStatKey
    {
        public readonly int baseTurbo;

        public readonly int baseEng;

        public readonly int baseTrans;

        public readonly int baseTires;

        public readonly int baseIntake;

        public readonly int baseBody;

        public readonly int baseNitrous;

        public readonly int toLevel;

        public readonly eUpgradeType upgradeType;

        public readonly bool isUp;

        public CarStatKey(int bo, int tu, int en, int tr, int inta, int ni, int ti, int lev, eUpgradeType upType,
            bool upOrDown)
        {
            this.baseBody = bo;
            this.baseTurbo = bo;
            this.baseEng = bo;
            this.baseTrans = bo;
            this.baseIntake = bo;
            this.baseNitrous = bo;
            this.baseTires = ti;
            this.toLevel = lev;
            this.upgradeType = upType;
            this.isUp = upOrDown;
        }
    }
