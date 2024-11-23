using System;
using System.Collections.Generic;
using Z2HSharedLibrary.DatabaseEntity;

[Serializable]
public class CarCollection : ItemCollectionAbstract<CarInfo>
{
    public CarCollection()
    {

    }
    public CarCollection(Dictionary<byte, object> parameters)
    {
        var ids = parameters[(byte)ParameterCode.ID] as string[];
        //var names = parameters[(byte)ParameterCode.Name] as string[];
        //var masses = (float[])parameters[(byte)ParameterCode.Mass];
        //var differentialRatios = (float[])parameters[(byte)ParameterCode.DifferentialRatio];
        //var maxPowerRpms = (short[])parameters[(byte)ParameterCode.MaxPowerRpm];
        //var maxPowers = (short[])parameters[(byte)ParameterCode.MaxPower];
        //var dragCoefficients = (float[])parameters[(byte)ParameterCode.DragCoefficient];
        //var frontalAreas = (float[])parameters[(byte)ParameterCode.FrontalArea];
        //var rollingResistances = (float[])parameters[(byte)ParameterCode.RollingResistance];
        //var wheelRadiuses = (float[])parameters[(byte)ParameterCode.WheelRadius];
        //var wheelMasses = (byte[])parameters[(byte)ParameterCode.WheelMass];
        //var gearsRatios = (float[][])parameters[(byte)ParameterCode.GearsRatio];
        //var nitrousForces = (short[])parameters[(byte)ParameterCode.NitrousHorsePowerIncrease];
        //var transmissions = (float[])parameters[(byte)ParameterCode.Transmission];
        //var tyreFrictions = (float[])parameters[(byte)ParameterCode.TyreFriction];
        //var manufacturers = (short[])parameters[(byte)ParameterCode.ManufacturerID];

        //var upgradeItemIDs = (Dictionary<string, object>)parameters[(byte)ParameterCode.UpgradeItemIDs];
        //var upgradableItemIDs = (Dictionary<string, object>)parameters[(byte)ParameterCode.UpgradableItemIDs];
        //var upgradeValues = (Dictionary<string, object>)parameters[(byte)ParameterCode.UpgradeValues];
        //var upgradeLevels = (Dictionary<string, object>)parameters[(byte)ParameterCode.UpgradeLevels];
        //var paintItems = (Dictionary<string, object>)parameters[(byte)ParameterCode.PaintItems];

        for (int i = 0; i < ids.Length; i++)
        {
            var item = new CarInfo();
            //{
            //    ID = ids[i],
            //    Name = names[i],
            //    Mass = masses[i],
            //    DifferentialRatio = differentialRatios[i],
            //    MaxPowerRpm = maxPowerRpms[i],
            //    MaxPower = maxPowers[i],
            //    DragCoefficient = dragCoefficients[i],
            //    FrontalArea = frontalAreas[i],
            //    RollingResistance = rollingResistances[i],
            //    WheelRadius = wheelRadiuses[i],
            //    WheelMass = wheelMasses[i],
            //    GearsRatio = gearsRatios[i],
            //    NitrousForce = nitrousForces[i],
            //    TransmissionValue = transmissions[i],
            //    TyreFrictionValue = tyreFrictions[i],
            //    ManufaturerID = (Manufacturer.ManufaturerID)manufacturers[i]
            //};

            //var upgradeCount = ((string[])upgradeItemIDs[item.ID]).Length;
            //item.CarUpgradeItems = new VirtualItemUpgradeCollection();
            //for (int j = 0; j < upgradeCount; j++)
            //{
            //    item.CarUpgradeItems.Add(new VirtualItemUpgrade
            //    {
            //        UpgradeItemID = ((string[])upgradeItemIDs[item.ID])[j],
            //        UpgradableItemID = ((string[])upgradableItemIDs[item.ID])[j],
            //        UpgradeLevel = ((byte[])upgradeLevels[item.ID])[j],
            //        UpgradeValue = ((float[])upgradeValues[item.ID])[j]
            //    });
            //}

            //item.PaintItems = ((string[])paintItems[item.ID]).ToList();

            Add(item);
        }
    }
}
