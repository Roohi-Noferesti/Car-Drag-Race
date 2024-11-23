
    public class OptimalGearChangeSpeedCalculator
    {
        private bool haveCalculatedOptimalGearChanges;

        private CarPhysics carPhysics;

        public float[] optimalGearChangeSpeeds { get; private set; }

        public float[] averageGearChangeSpeeds { get; private set; }

        public OptimalGearChangeSpeedCalculator(CarPhysics zCarPhysics)
        {
            this.carPhysics = zCarPhysics;
            this.optimalGearChangeSpeeds = new float[this.carPhysics.GearBox.NumGears];
            this.haveCalculatedOptimalGearChanges = false;
        }

        public void CalculateGearChangeSpeeds()
        {
            this.CalculateApproximateOptimalGearChangeSpeeds();
        }

        public void CalculateApproximateOptimalGearChangeSpeeds()
        {
            InGameTorqueCurve torqueCurve = this.carPhysics.Engine.TorqueCurve;
            float extraTorqueFromNitrousInFootPounds = 0f;
            float num = this.carPhysics.Engine.TrueRedLineRPM - this.carPhysics.Engine.IdleRPM;
            this.carPhysics.GearBox.CalculatedOptimalGearChangeMPHArray().CopyTo(this.optimalGearChangeSpeeds, 0);
            for (int i = 1; i < this.carPhysics.GearBox.NumGears; i++)
            {
                float num2 = this.carPhysics.GearBox.GearRatio(i)*this.carPhysics.GearBox.GearRatioFinal;
                for (float num3 = 0f; num3 < 1.02f; num3 += 0.01f)
                {
                    float num4 = num*num3 + this.carPhysics.Engine.IdleRPM;
                    float num5 = CarPhysicsCalculations.EvaluateTorqueAtWheelAtThisRPM(torqueCurve, num3,
                        extraTorqueFromNitrousInFootPounds, num2);
                    float num6 = PhysicsConstants.WheelLinearSpeedFromRPM(this.carPhysics.TireData.WheelRadius,
                        num4/num2);
                    if (num4 > this.carPhysics.Engine.RedLineRPM - 15f)
                    {
                        num6 = PhysicsConstants.WheelLinearSpeedFromRPM(this.carPhysics.TireData.WheelRadius,
                            (this.carPhysics.Engine.RedLineRPM - 15f)/num2);
                        this.optimalGearChangeSpeeds[i - 1] = num6*2.236f;
                        break;
                    }
                    float num7 = this.carPhysics.GearBox.GearRatio(i + 1)*this.carPhysics.GearBox.GearRatioFinal;
                    float num8 = PhysicsConstants.WheelRPMFromLinearSpeed(this.carPhysics.TireData.WheelRadius, num6)*
                                 num7;
                    float zRPMNormalised = (num8 - this.carPhysics.Engine.IdleRPM)/num;
                    float num9 = CarPhysicsCalculations.EvaluateTorqueAtWheelAtThisRPM(torqueCurve, zRPMNormalised,
                        extraTorqueFromNitrousInFootPounds, num7);
                    if (num9 > num5)
                    {
                        this.optimalGearChangeSpeeds[i - 1] = num6*2.236f;
                        break;
                    }
                }
            }
            this.CopyOptimalGearChangeSpeedsToCarPhysics();
            this.haveCalculatedOptimalGearChanges = true;
        }

        public void CopyOptimalGearChangeSpeedsToCarPhysics()
        {
            this.optimalGearChangeSpeeds.CopyTo(this.carPhysics.GearBox.CalculatedOptimalGearChangeMPHArray(), 0);
        }

        public void CalculateBadGearChangeSpeeds()
        {
            if (!this.haveCalculatedOptimalGearChanges)
            {
                return;
            }
            this.averageGearChangeSpeeds = new float[this.carPhysics.GearBox.NumGears];
            this.carPhysics.GearBox.CalculatedOptimalGearChangeMPHArray().CopyTo(this.averageGearChangeSpeeds, 0);
            float[] array = this.carPhysics.GearBox.CalculatedOptimalGearChangeMPHArray();
            float launchWindowLeadupBand;
            float goodLaunchBand;
            float launchPerfectWindowFraction;
            float leadUpToGearChangeBand;
            float goodGearChangeBand;
            float perfectGearChangeBandFraction;
            CarGearChangeLightRangesData.GetTotalGearLightRPMRange(out launchWindowLeadupBand, out goodLaunchBand, out launchPerfectWindowFraction, out leadUpToGearChangeBand, out goodGearChangeBand,
                out perfectGearChangeBandFraction);
            goodGearChangeBand *= 4f;
            for (int i = 0; i < this.carPhysics.GearBox.NumGears - 1; i++)
            {
                float zLinearSpeed = array[i]*0.44722718f;
                float num7 = this.carPhysics.GearBox.GearRatio(i + 1)*this.carPhysics.GearBox.GearRatioFinal;
                float num8 =
                    PhysicsConstants.WheelRPMFromLinearSpeed(this.carPhysics.TireData.WheelRadius, zLinearSpeed)*num7;
                this.averageGearChangeSpeeds[i] = num8 - goodGearChangeBand;
                this.averageGearChangeSpeeds[i] =
                    PhysicsConstants.WheelLinearSpeedFromRPM(this.carPhysics.TireData.WheelRadius,
                        this.averageGearChangeSpeeds[i]/num7)*2.236f;
            }
            this.CopyAverageGearChangeSpeedsToCarPhysics();
        }

        public void CopyAverageGearChangeSpeedsToCarPhysics()
        {
            this.averageGearChangeSpeeds.CopyTo(this.carPhysics.GearBox.CalculatedOptimalGearChangeMPHArray(), 0);
        }
    }
