using System;
using System.Collections.Generic;
using LitJson;

internal class JsonTester
{
    private static Me CreateMe()
    {
        Me me = new Me
        {
            name = "Holmes",
            time = DateTime.Now,
            isGay = false,
            address = new List<string>()
        };
        me.address.Add("221 Baker Street");
        me.address.Add("London");
        me.address.Add("United Kingdom");
        me.pet = new Pet();
        me.pet.name = "Mimi";
        me.pet.type = PetType.Cat;
        me.pet.height = 0f;
        me.familyMembers = new List<FamilyMember>();
        FamilyMember item = new FamilyMember
        {
            name = "Clare",
            relationship = "sister",
            hairColor = HairColor.Red,
            age = 0x2a
        };
        me.familyMembers.Add(item);
        item = new FamilyMember
        {
            name = "William",
            relationship = "brother",
            hairColor = HairColor.Blue,
            age = 0x19
        };
        me.familyMembers.Add(item);
        item = new FamilyMember
        {
            name = "Chris",
            relationship = "dad",
            hairColor = HairColor.Green,
            age = 0x3f
        };
        me.familyMembers.Add(item);
        item = new FamilyMember
        {
            name = "Heather",
            relationship = "mum",
            hairColor = HairColor.Red,
            age = 0x3e
        };
        me.familyMembers.Add(item);
        return me;
    }

    private static void GetFamilyMember(JsonDict jsonDict, ref FamilyMember familyMember)
    {
        jsonDict.TryGetValue("name", out familyMember.name);
        jsonDict.TryGetValue("relationship", out familyMember.relationship);
        jsonDict.TryGetEnum<HairColor>("hairColor", out familyMember.hairColor);
        jsonDict.TryGetValue("age", out familyMember.age);
    }

    private static Me GetMe(JsonDict jsonDict)
    {
        Me me = new Me();
        jsonDict.TryGetValue("name", out me.name);
        jsonDict.TryGetValue("address", out me.address);
        jsonDict.TryGetObject<Pet>("pet", out me.pet, new GetObjectDelegate<Pet>(JsonTester.GetPet));
        jsonDict.TryGetObjectList<FamilyMember>("familyMembers", out me.familyMembers, new GetObjectDelegate<FamilyMember>(JsonTester.GetFamilyMember));
        jsonDict.TryGetValue("time", out me.time);
        jsonDict.TryGetValue("isGay", out me.isGay);
        return me;
    }

    private static void GetPet(JsonDict jsonDict, ref Pet pet)
    {
        jsonDict.TryGetValue("name", out pet.name);
        jsonDict.TryGetEnum<PetType>("type", out pet.type);
        jsonDict.TryGetValue("height", out pet.height);
    }

    public static void Run()
    {
        Me me = CreateMe();
        JsonDict jsonDict = new JsonDict();
        SetMe(me, ref jsonDict);
        string json = jsonDict.ToString();
        JsonDict dict2 = new JsonDict();
        dict2.Read(json);
        //string str2 = dict2.ToString();
        Me me2 = GetMe(dict2);
        JsonDict dict3 = new JsonDict();
        SetMe(me2, ref dict3);
        //string str3 = dict3.ToString();
    }

    private static void SetFamilyMember(FamilyMember familyMember, ref JsonDict jsonDict)
    {
        jsonDict.Set("name", familyMember.name);
        jsonDict.Set("relationship", familyMember.relationship);
        jsonDict.SetEnum<HairColor>("hairColor", familyMember.hairColor);
        jsonDict.Set("age", familyMember.age);
    }

    private static void SetMe(Me me, ref JsonDict jsonDict)
    {
        jsonDict.Set("name", me.name);
        jsonDict.Set("address", me.address);
        jsonDict.SetObject<Pet>("pet", me.pet, new SetObjectDelegate<Pet>(JsonTester.SetPet));
        jsonDict.SetObjectList<FamilyMember>("familyMembers", me.familyMembers, new SetObjectDelegate<FamilyMember>(JsonTester.SetFamilyMember));
        jsonDict.Set("time", me.time);
        jsonDict.Set("isGay", me.isGay);
    }

    private static void SetPet(Pet pet, ref JsonDict jsonDict)
    {
        jsonDict.Set("name", pet.name);
        jsonDict.SetEnum<PetType>("type", pet.type);
        jsonDict.Set("height", pet.height);
    }
}
