
    public struct DriverInputs
    {
        public float Throttle;

        public bool GearChangeUp;

        public bool GearChangeDown;

        public bool Nitrous;

        public void Reset()
        {
            this.Throttle = 0f;
            this.GearChangeUp = false;
            this.GearChangeDown = false;
            this.Nitrous = false;
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Throttle : ",
                this.Throttle,
                " , GearUp : ",
                this.GearChangeUp,
                " , GearChangeDown : ",
                this.GearChangeDown,
                " , Nitrous : ",
                this.Nitrous,
                "\n"
            });
        }
    }
