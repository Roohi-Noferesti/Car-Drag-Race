using DataSerialization;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;

public sealed class TierXConfigurationSerializer : TypeModel
{
    private static readonly Dictionary<Type, int> knownTypes = new Dictionary<Type, int>(60);

    private static void Write(BubbleMessageData bubbleMessageData, ProtoWriter writer)
    {
        if (bubbleMessageData.GetType() != typeof(BubbleMessageData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(BubbleMessageData), bubbleMessageData.GetType());
        }
        string expr_2D = bubbleMessageData.Text;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        float arg_53_0 = bubbleMessageData.YOffset;
        ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_53_0, writer);
        float arg_67_0 = bubbleMessageData.NipplePos;
        ProtoWriter.WriteFieldHeader(3, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_67_0, writer);
    }

    private static BubbleMessageData Read(BubbleMessageData bubbleMessageData, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (bubbleMessageData == null)
                        {
                            BubbleMessageData expr_A3 = new BubbleMessageData();
                            ProtoReader.NoteObject(expr_A3, protoReader);
                            bubbleMessageData = expr_A3;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (bubbleMessageData == null)
                        {
                            BubbleMessageData expr_7C = new BubbleMessageData();
                            ProtoReader.NoteObject(expr_7C, protoReader);
                            bubbleMessageData = expr_7C;
                        }
                        float num2 = protoReader.ReadSingle();
                        bubbleMessageData.NipplePos = num2;
                    }
                }
                else
                {
                    if (bubbleMessageData == null)
                    {
                        BubbleMessageData expr_4C = new BubbleMessageData();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        bubbleMessageData = expr_4C;
                    }
                    float num2 = protoReader.ReadSingle();
                    bubbleMessageData.YOffset = num2;
                }
            }
            else
            {
                if (bubbleMessageData == null)
                {
                    BubbleMessageData expr_19 = new BubbleMessageData();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    bubbleMessageData = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    bubbleMessageData.Text = text;
                }
            }
        }
        if (bubbleMessageData == null)
        {
            BubbleMessageData expr_CB = new BubbleMessageData();
            ProtoReader.NoteObject(expr_CB, protoReader);
            bubbleMessageData = expr_CB;
        }
        return bubbleMessageData;
    }

    private static void Write(CarOverride carOverride, ProtoWriter writer)
    {
        if (carOverride.GetType() != typeof(CarOverride))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(CarOverride), carOverride.GetType());
        }
        string expr_2D = carOverride.SequenceID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        string expr_4A = carOverride.ScheduledPinID;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
        string expr_67 = carOverride.ChoiceSequenceID;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, writer);
            ProtoWriter.WriteString(expr_67, writer);
        }
        string expr_84 = carOverride.ThemeID;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, writer);
            ProtoWriter.WriteString(expr_84, writer);
        }
        bool arg_AA_0 = carOverride.ShouldSetHumanCarToAI;
        ProtoWriter.WriteFieldHeader(5, WireType.Variant, writer);
        ProtoWriter.WriteBoolean(arg_AA_0, writer);
    }

    private static CarOverride Read(CarOverride carOverride, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (carOverride == null)
                                {
                                    CarOverride expr_10C = new CarOverride();
                                    ProtoReader.NoteObject(expr_10C, protoReader);
                                    carOverride = expr_10C;
                                }
                                protoReader.SkipField();
                            }
                            else
                            {
                                if (carOverride == null)
                                {
                                    CarOverride expr_E5 = new CarOverride();
                                    ProtoReader.NoteObject(expr_E5, protoReader);
                                    carOverride = expr_E5;
                                }
                                bool shouldSetHumanCarToAI = protoReader.ReadBoolean();
                                carOverride.ShouldSetHumanCarToAI = shouldSetHumanCarToAI;
                            }
                        }
                        else
                        {
                            if (carOverride == null)
                            {
                                CarOverride expr_B2 = new CarOverride();
                                ProtoReader.NoteObject(expr_B2, protoReader);
                                carOverride = expr_B2;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                carOverride.ThemeID = text;
                            }
                        }
                    }
                    else
                    {
                        if (carOverride == null)
                        {
                            CarOverride expr_7F = new CarOverride();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            carOverride = expr_7F;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            carOverride.ChoiceSequenceID = text;
                        }
                    }
                }
                else
                {
                    if (carOverride == null)
                    {
                        CarOverride expr_4C = new CarOverride();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        carOverride = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        carOverride.ScheduledPinID = text;
                    }
                }
            }
            else
            {
                if (carOverride == null)
                {
                    CarOverride expr_19 = new CarOverride();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    carOverride = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    carOverride.SequenceID = text;
                }
            }
        }
        if (carOverride == null)
        {
            CarOverride expr_134 = new CarOverride();
            ProtoReader.NoteObject(expr_134, protoReader);
            carOverride = expr_134;
        }
        return carOverride;
    }

    private static void Write(ChoiceScreenInfo choiceScreenInfo, ProtoWriter protoWriter)
    {
        if (choiceScreenInfo.GetType() != typeof(ChoiceScreenInfo))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ChoiceScreenInfo), choiceScreenInfo.GetType());
        }
        string expr_2D = choiceScreenInfo.SequenceID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = choiceScreenInfo.Title;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        Color arg_78_0 = choiceScreenInfo.Colour;
        ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_78_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
    }

    private static ChoiceScreenInfo Read(ChoiceScreenInfo choiceScreenInfo, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (choiceScreenInfo == null)
                        {
                            ChoiceScreenInfo expr_BA = new ChoiceScreenInfo();
                            ProtoReader.NoteObject(expr_BA, protoReader);
                            choiceScreenInfo = expr_BA;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (choiceScreenInfo == null)
                        {
                            ChoiceScreenInfo expr_7F = new ChoiceScreenInfo();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            choiceScreenInfo = expr_7F;
                        }
                        Color arg_96_0 = choiceScreenInfo.Colour;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        Color arg_A2_0 = TierXConfigurationSerializer.Read(arg_96_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        Color colour = arg_A2_0;
                        choiceScreenInfo.Colour = colour;
                    }
                }
                else
                {
                    if (choiceScreenInfo == null)
                    {
                        ChoiceScreenInfo expr_4C = new ChoiceScreenInfo();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        choiceScreenInfo = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        choiceScreenInfo.Title = text;
                    }
                }
            }
            else
            {
                if (choiceScreenInfo == null)
                {
                    ChoiceScreenInfo expr_19 = new ChoiceScreenInfo();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    choiceScreenInfo = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    choiceScreenInfo.SequenceID = text;
                }
            }
        }
        if (choiceScreenInfo == null)
        {
            ChoiceScreenInfo expr_E2 = new ChoiceScreenInfo();
            ProtoReader.NoteObject(expr_E2, protoReader);
            choiceScreenInfo = expr_E2;
        }
        return choiceScreenInfo;
    }

    private static void Write(Color color, ProtoWriter writer)
    {
        float arg_10_0 = color.r;
        ProtoWriter.WriteFieldHeader(1, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_10_0, writer);
        float arg_25_0 = color.g;
        ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_25_0, writer);
        float arg_3A_0 = color.b;
        ProtoWriter.WriteFieldHeader(3, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_3A_0, writer);
        float arg_4F_0 = color.a;
        ProtoWriter.WriteFieldHeader(4, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_4F_0, writer);
    }

    private static Color Read(Color result, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            protoReader.SkipField();
                        }
                        else
                        {
                            float num2 = protoReader.ReadSingle();
                            result.a = num2;
                        }
                    }
                    else
                    {
                        float num2 = protoReader.ReadSingle();
                        result.b = num2;
                    }
                }
                else
                {
                    float num2 = protoReader.ReadSingle();
                    result.g = num2;
                }
            }
            else
            {
                float num2 = protoReader.ReadSingle();
                result.r = num2;
            }
        }
        return result;
    }

    private static void Write(ConditionallyModifiedString conditionallyModifiedString, ProtoWriter protoWriter)
    {
        if (conditionallyModifiedString.GetType() != typeof(ConditionallyModifiedString))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ConditionallyModifiedString), conditionallyModifiedString.GetType());
        }
        StringModification expr_2D = conditionallyModifiedString.StringModification;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_2D, protoWriter);
            TierXConfigurationSerializer.Write(expr_2D, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        EligibilityRequirements expr_59 = conditionallyModifiedString.Requirements;
        if (expr_59 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_59, protoWriter);
            TierXConfigurationSerializer.Write(expr_59, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static ConditionallyModifiedString Read(ConditionallyModifiedString conditionallyModifiedString, ProtoReader protoReader)
    {
        if (conditionallyModifiedString != null)
        {
            conditionallyModifiedString.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (conditionallyModifiedString == null)
                    {
                        ConditionallyModifiedString expr_B6 = new ConditionallyModifiedString();
                        ProtoReader.NoteObject(expr_B6, protoReader);
                        expr_B6.Init();
                        conditionallyModifiedString = expr_B6;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (conditionallyModifiedString == null)
                    {
                        ConditionallyModifiedString expr_72 = new ConditionallyModifiedString();
                        ProtoReader.NoteObject(expr_72, protoReader);
                        expr_72.Init();
                        conditionallyModifiedString = expr_72;
                    }
                    EligibilityRequirements arg_8F_0 = conditionallyModifiedString.Requirements;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityRequirements arg_9B_0 = TierXConfigurationSerializer.Read(arg_8F_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    EligibilityRequirements eligibilityRequirements = arg_9B_0;
                    if (eligibilityRequirements != null)
                    {
                        conditionallyModifiedString.Requirements = eligibilityRequirements;
                    }
                }
            }
            else
            {
                if (conditionallyModifiedString == null)
                {
                    ConditionallyModifiedString expr_25 = new ConditionallyModifiedString();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    conditionallyModifiedString = expr_25;
                }
                StringModification arg_42_0 = conditionallyModifiedString.StringModification;
                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                StringModification arg_4E_0 = TierXConfigurationSerializer.Read(arg_42_0, protoReader);
                ProtoReader.EndSubItem(token, protoReader);
                StringModification stringModification = arg_4E_0;
                if (stringModification != null)
                {
                    conditionallyModifiedString.StringModification = stringModification;
                }
            }
        }
        if (conditionallyModifiedString == null)
        {
            ConditionallyModifiedString expr_E4 = new ConditionallyModifiedString();
            ProtoReader.NoteObject(expr_E4, protoReader);
            expr_E4.Init();
            conditionallyModifiedString = expr_E4;
        }
        return conditionallyModifiedString;
    }

    private static void Write(ConditionallySelectedSnapshotList conditionallySelectedSnapshotList, ProtoWriter protoWriter)
    {
        if (conditionallySelectedSnapshotList.GetType() != typeof(ConditionallySelectedSnapshotList))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ConditionallySelectedSnapshotList), conditionallySelectedSnapshotList.GetType());
        }
        List<ConditionallySelectedSnapshots> expr_2D = conditionallySelectedSnapshotList.Lists;
        if (expr_2D != null)
        {
            List<ConditionallySelectedSnapshots> list = expr_2D;
            foreach (ConditionallySelectedSnapshots arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static ConditionallySelectedSnapshotList Read(ConditionallySelectedSnapshotList conditionallySelectedSnapshotList, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (conditionallySelectedSnapshotList == null)
                {
                    ConditionallySelectedSnapshotList expr_87 = new ConditionallySelectedSnapshotList();
                    ProtoReader.NoteObject(expr_87, protoReader);
                    conditionallySelectedSnapshotList = expr_87;
                }
                protoReader.SkipField();
            }
            else
            {
                if (conditionallySelectedSnapshotList == null)
                {
                    ConditionallySelectedSnapshotList expr_19 = new ConditionallySelectedSnapshotList();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    conditionallySelectedSnapshotList = expr_19;
                }
                List<ConditionallySelectedSnapshots> list = conditionallySelectedSnapshotList.Lists;
                List<ConditionallySelectedSnapshots> list2 = list;
                if (list == null)
                {
                    list = new List<ConditionallySelectedSnapshots>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<ConditionallySelectedSnapshots> arg_53_0 = list;
                    ConditionallySelectedSnapshots arg_46_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    ConditionallySelectedSnapshots arg_53_1 = TierXConfigurationSerializer.Read(arg_46_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_53_0.Add(arg_53_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    conditionallySelectedSnapshotList.Lists = list2;
                }
            }
        }
        if (conditionallySelectedSnapshotList == null)
        {
            ConditionallySelectedSnapshotList expr_AF = new ConditionallySelectedSnapshotList();
            ProtoReader.NoteObject(expr_AF, protoReader);
            conditionallySelectedSnapshotList = expr_AF;
        }
        return conditionallySelectedSnapshotList;
    }

    private static void Write(ConditionallySelectedSnapshots conditionallySelectedSnapshots, ProtoWriter protoWriter)
    {
        if (conditionallySelectedSnapshots.GetType() != typeof(ConditionallySelectedSnapshots))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ConditionallySelectedSnapshots), conditionallySelectedSnapshots.GetType());
        }
        List<CarOverride> expr_2D = conditionallySelectedSnapshots.Cars;
        if (expr_2D != null)
        {
            List<CarOverride> list = expr_2D;
            foreach (CarOverride arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        EligibilityRequirements expr_91 = conditionallySelectedSnapshots.Requirements;
        if (expr_91 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_91, protoWriter);
            TierXConfigurationSerializer.Write(expr_91, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static ConditionallySelectedSnapshots Read(ConditionallySelectedSnapshots conditionallySelectedSnapshots, ProtoReader protoReader)
    {
        if (conditionallySelectedSnapshots != null)
        {
            conditionallySelectedSnapshots.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (conditionallySelectedSnapshots == null)
                    {
                        ConditionallySelectedSnapshots expr_EB = new ConditionallySelectedSnapshots();
                        ProtoReader.NoteObject(expr_EB, protoReader);
                        expr_EB.Init();
                        conditionallySelectedSnapshots = expr_EB;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (conditionallySelectedSnapshots == null)
                    {
                        ConditionallySelectedSnapshots expr_A2 = new ConditionallySelectedSnapshots();
                        ProtoReader.NoteObject(expr_A2, protoReader);
                        expr_A2.Init();
                        conditionallySelectedSnapshots = expr_A2;
                    }
                    EligibilityRequirements arg_C0_0 = conditionallySelectedSnapshots.Requirements;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityRequirements arg_CD_0 = TierXConfigurationSerializer.Read(arg_C0_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    EligibilityRequirements eligibilityRequirements = arg_CD_0;
                    if (eligibilityRequirements != null)
                    {
                        conditionallySelectedSnapshots.Requirements = eligibilityRequirements;
                    }
                }
            }
            else
            {
                if (conditionallySelectedSnapshots == null)
                {
                    ConditionallySelectedSnapshots expr_25 = new ConditionallySelectedSnapshots();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    conditionallySelectedSnapshots = expr_25;
                }
                List<CarOverride> list = conditionallySelectedSnapshots.Cars;
                List<CarOverride> list2 = list;
                if (list == null)
                {
                    list = new List<CarOverride>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<CarOverride> arg_65_0 = list;
                    CarOverride arg_58_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    CarOverride arg_65_1 = TierXConfigurationSerializer.Read(arg_58_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_65_0.Add(arg_65_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    conditionallySelectedSnapshots.Cars = list2;
                }
            }
        }
        if (conditionallySelectedSnapshots == null)
        {
            ConditionallySelectedSnapshots expr_119 = new ConditionallySelectedSnapshots();
            ProtoReader.NoteObject(expr_119, protoReader);
            expr_119.Init();
            conditionallySelectedSnapshots = expr_119;
        }
        return conditionallySelectedSnapshots;
    }

    private static void Write(ConditionallySelectedString conditionallySelectedString, ProtoWriter protoWriter)
    {
        if (conditionallySelectedString.GetType() != typeof(ConditionallySelectedString))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ConditionallySelectedString), conditionallySelectedString.GetType());
        }
        List<ConditionallyModifiedString> expr_2D = conditionallySelectedString.Strings;
        if (expr_2D != null)
        {
            List<ConditionallyModifiedString> list = expr_2D;
            foreach (ConditionallyModifiedString arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static ConditionallySelectedString Read(ConditionallySelectedString conditionallySelectedString, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (conditionallySelectedString == null)
                {
                    ConditionallySelectedString expr_87 = new ConditionallySelectedString();
                    ProtoReader.NoteObject(expr_87, protoReader);
                    conditionallySelectedString = expr_87;
                }
                protoReader.SkipField();
            }
            else
            {
                if (conditionallySelectedString == null)
                {
                    ConditionallySelectedString expr_19 = new ConditionallySelectedString();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    conditionallySelectedString = expr_19;
                }
                List<ConditionallyModifiedString> list = conditionallySelectedString.Strings;
                List<ConditionallyModifiedString> list2 = list;
                if (list == null)
                {
                    list = new List<ConditionallyModifiedString>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<ConditionallyModifiedString> arg_53_0 = list;
                    ConditionallyModifiedString arg_46_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    ConditionallyModifiedString arg_53_1 = TierXConfigurationSerializer.Read(arg_46_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_53_0.Add(arg_53_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    conditionallySelectedString.Strings = list2;
                }
            }
        }
        if (conditionallySelectedString == null)
        {
            ConditionallySelectedString expr_AF = new ConditionallySelectedString();
            ProtoReader.NoteObject(expr_AF, protoReader);
            conditionallySelectedString = expr_AF;
        }
        return conditionallySelectedString;
    }

    private static void Write(CrewMemberLayout crewMemberLayout, ProtoWriter writer)
    {
        if (crewMemberLayout.GetType() != typeof(CrewMemberLayout))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(CrewMemberLayout), crewMemberLayout.GetType());
        }
        string expr_2D = crewMemberLayout.Texture;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        string expr_4A = crewMemberLayout.Name;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
        string expr_67 = crewMemberLayout.Event;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, writer);
            ProtoWriter.WriteString(expr_67, writer);
        }
        string expr_84 = crewMemberLayout.ScheduleID;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, writer);
            ProtoWriter.WriteString(expr_84, writer);
        }
    }

    private static CrewMemberLayout Read(CrewMemberLayout crewMemberLayout, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (crewMemberLayout == null)
                            {
                                CrewMemberLayout expr_DC = new CrewMemberLayout();
                                ProtoReader.NoteObject(expr_DC, protoReader);
                                crewMemberLayout = expr_DC;
                            }
                            protoReader.SkipField();
                        }
                        else
                        {
                            if (crewMemberLayout == null)
                            {
                                CrewMemberLayout expr_B2 = new CrewMemberLayout();
                                ProtoReader.NoteObject(expr_B2, protoReader);
                                crewMemberLayout = expr_B2;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                crewMemberLayout.ScheduleID = text;
                            }
                        }
                    }
                    else
                    {
                        if (crewMemberLayout == null)
                        {
                            CrewMemberLayout expr_7F = new CrewMemberLayout();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            crewMemberLayout = expr_7F;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            crewMemberLayout.Event = text;
                        }
                    }
                }
                else
                {
                    if (crewMemberLayout == null)
                    {
                        CrewMemberLayout expr_4C = new CrewMemberLayout();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        crewMemberLayout = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        crewMemberLayout.Name = text;
                    }
                }
            }
            else
            {
                if (crewMemberLayout == null)
                {
                    CrewMemberLayout expr_19 = new CrewMemberLayout();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    crewMemberLayout = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    crewMemberLayout.Texture = text;
                }
            }
        }
        if (crewMemberLayout == null)
        {
            CrewMemberLayout expr_104 = new CrewMemberLayout();
            ProtoReader.NoteObject(expr_104, protoReader);
            crewMemberLayout = expr_104;
        }
        return crewMemberLayout;
    }

    private static void Write(DifficultyDeltas difficultyDeltas, ProtoWriter writer)
    {
        if (difficultyDeltas.GetType() != typeof(DifficultyDeltas))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(DifficultyDeltas), difficultyDeltas.GetType());
        }
        int expr_2D = difficultyDeltas.Easy;
        if (expr_2D != 0)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
            ProtoWriter.WriteInt32(expr_2D, writer);
        }
        int expr_4A = difficultyDeltas.Challenging;
        if (expr_4A != 0)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.Variant, writer);
            ProtoWriter.WriteInt32(expr_4A, writer);
        }
        int expr_67 = difficultyDeltas.Difficult;
        if (expr_67 != 0)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.Variant, writer);
            ProtoWriter.WriteInt32(expr_67, writer);
        }
    }

    private static DifficultyDeltas Read(DifficultyDeltas difficultyDeltas, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (difficultyDeltas == null)
                        {
                            DifficultyDeltas expr_A0 = new DifficultyDeltas();
                            ProtoReader.NoteObject(expr_A0, protoReader);
                            difficultyDeltas = expr_A0;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (difficultyDeltas == null)
                        {
                            DifficultyDeltas expr_79 = new DifficultyDeltas();
                            ProtoReader.NoteObject(expr_79, protoReader);
                            difficultyDeltas = expr_79;
                        }
                        int num2 = protoReader.ReadInt32();
                        difficultyDeltas.Difficult = num2;
                    }
                }
                else
                {
                    if (difficultyDeltas == null)
                    {
                        DifficultyDeltas expr_49 = new DifficultyDeltas();
                        ProtoReader.NoteObject(expr_49, protoReader);
                        difficultyDeltas = expr_49;
                    }
                    int num2 = protoReader.ReadInt32();
                    difficultyDeltas.Challenging = num2;
                }
            }
            else
            {
                if (difficultyDeltas == null)
                {
                    DifficultyDeltas expr_19 = new DifficultyDeltas();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    difficultyDeltas = expr_19;
                }
                int num2 = protoReader.ReadInt32();
                difficultyDeltas.Easy = num2;
            }
        }
        if (difficultyDeltas == null)
        {
            DifficultyDeltas expr_C8 = new DifficultyDeltas();
            ProtoReader.NoteObject(expr_C8, protoReader);
            difficultyDeltas = expr_C8;
        }
        return difficultyDeltas;
    }

    private static void Write(EligibilityCondition eligibilityCondition, ProtoWriter protoWriter)
    {
        if (eligibilityCondition.GetType() != typeof(EligibilityCondition))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(EligibilityCondition), eligibilityCondition.GetType());
        }
        string expr_2D = eligibilityCondition.Type;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        EligibilityConditionDetails expr_4A = eligibilityCondition.Details;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_4A, protoWriter);
            TierXConfigurationSerializer.Write(expr_4A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static EligibilityCondition Read(EligibilityCondition eligibilityCondition, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (eligibilityCondition == null)
                    {
                        EligibilityCondition expr_8A = new EligibilityCondition();
                        ProtoReader.NoteObject(expr_8A, protoReader);
                        eligibilityCondition = expr_8A;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (eligibilityCondition == null)
                    {
                        EligibilityCondition expr_4C = new EligibilityCondition();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        eligibilityCondition = expr_4C;
                    }
                    EligibilityConditionDetails arg_63_0 = eligibilityCondition.Details;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityConditionDetails arg_6F_0 = TierXConfigurationSerializer.Read(arg_63_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    EligibilityConditionDetails eligibilityConditionDetails = arg_6F_0;
                    if (eligibilityConditionDetails != null)
                    {
                        eligibilityCondition.Details = eligibilityConditionDetails;
                    }
                }
            }
            else
            {
                if (eligibilityCondition == null)
                {
                    EligibilityCondition expr_19 = new EligibilityCondition();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    eligibilityCondition = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    eligibilityCondition.Type = text;
                }
            }
        }
        if (eligibilityCondition == null)
        {
            EligibilityCondition expr_B2 = new EligibilityCondition();
            ProtoReader.NoteObject(expr_B2, protoReader);
            eligibilityCondition = expr_B2;
        }
        return eligibilityCondition;
    }

    private static void Write(EligibilityConditionDetails eligibilityConditionDetails, ProtoWriter protoWriter)
    {
        if (eligibilityConditionDetails.GetType() != typeof(EligibilityConditionDetails))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(EligibilityConditionDetails), eligibilityConditionDetails.GetType());
        }
        bool arg_36_0 = eligibilityConditionDetails.RequiredResult;
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_36_0, protoWriter);
        int arg_4A_0 = eligibilityConditionDetails.MinValue;
        ProtoWriter.WriteFieldHeader(2, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_4A_0, protoWriter);
        int arg_5E_0 = eligibilityConditionDetails.MaxValue;
        ProtoWriter.WriteFieldHeader(3, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_5E_0, protoWriter);
        float arg_72_0 = eligibilityConditionDetails.MinFloatValue;
        ProtoWriter.WriteFieldHeader(4, WireType.Fixed32, protoWriter);
        ProtoWriter.WriteSingle(arg_72_0, protoWriter);
        float arg_86_0 = eligibilityConditionDetails.MaxFloatValue;
        ProtoWriter.WriteFieldHeader(5, WireType.Fixed32, protoWriter);
        ProtoWriter.WriteSingle(arg_86_0, protoWriter);
        int arg_9A_0 = eligibilityConditionDetails.IncrementValue;
        ProtoWriter.WriteFieldHeader(6, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_9A_0, protoWriter);
        int arg_AE_0 = eligibilityConditionDetails.Tier;
        ProtoWriter.WriteFieldHeader(7, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_AE_0, protoWriter);
        string expr_B9 = eligibilityConditionDetails.Objective;
        if (expr_B9 != null)
        {
            ProtoWriter.WriteFieldHeader(8, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_B9, protoWriter);
        }
        TimeSpan expr_D6 = eligibilityConditionDetails.TimeSpanDifference;
        if (!(expr_D6 == TimeSpan.Zero))
        {
            ProtoWriter.WriteFieldHeader(9, WireType.String, protoWriter);
            BclHelpers.WriteTimeSpan(expr_D6, protoWriter);
        }
        string expr_FE = eligibilityConditionDetails.TimeDifference;
        if (expr_FE != null)
        {
            ProtoWriter.WriteFieldHeader(10, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_FE, protoWriter);
        }
        DateTime arg_126_0 = eligibilityConditionDetails.MinDateTime;
        ProtoWriter.WriteFieldHeader(11, WireType.String, protoWriter);
        BclHelpers.WriteDateTime(arg_126_0, protoWriter);
        DateTime arg_13B_0 = eligibilityConditionDetails.MaxDateTime;
        ProtoWriter.WriteFieldHeader(12, WireType.String, protoWriter);
        BclHelpers.WriteDateTime(arg_13B_0, protoWriter);
        string expr_146 = eligibilityConditionDetails.MinTime;
        if (expr_146 != null)
        {
            ProtoWriter.WriteFieldHeader(13, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_146, protoWriter);
        }
        string expr_164 = eligibilityConditionDetails.MaxTime;
        if (expr_164 != null)
        {
            ProtoWriter.WriteFieldHeader(14, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_164, protoWriter);
        }
        string expr_182 = eligibilityConditionDetails.StringValue;
        if (expr_182 != null)
        {
            ProtoWriter.WriteFieldHeader(15, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_182, protoWriter);
        }
        string expr_1A0 = eligibilityConditionDetails.SequenceID;
        if (expr_1A0 != null)
        {
            ProtoWriter.WriteFieldHeader(16, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1A0, protoWriter);
        }
        string expr_1BE = eligibilityConditionDetails.EventType;
        if (expr_1BE != null)
        {
            ProtoWriter.WriteFieldHeader(17, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1BE, protoWriter);
        }
        string expr_1DC = eligibilityConditionDetails.ThemeID;
        if (expr_1DC != null)
        {
            ProtoWriter.WriteFieldHeader(18, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1DC, protoWriter);
        }
        string expr_1FA = eligibilityConditionDetails.GameMode;
        if (expr_1FA != null)
        {
            ProtoWriter.WriteFieldHeader(19, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1FA, protoWriter);
        }
        int arg_222_0 = eligibilityConditionDetails.IntValue;
        ProtoWriter.WriteFieldHeader(20, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_222_0, protoWriter);
        List<string> expr_22D = eligibilityConditionDetails.StringValues;
        if (expr_22D != null)
        {
            List<string> list = expr_22D;
            foreach (string arg_254_0 in list)
            {
                ProtoWriter.WriteFieldHeader(21, WireType.String, protoWriter);
                ProtoWriter.WriteString(arg_254_0, protoWriter);
            }
        }
        List<int> expr_283 = eligibilityConditionDetails.IntValues;
        if (expr_283 != null)
        {
            List<int> list2 = expr_283;
            foreach (int arg_2AA_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(22, WireType.Variant, protoWriter);
                ProtoWriter.WriteInt32(arg_2AA_0, protoWriter);
            }
        }
        bool arg_2E3_0 = eligibilityConditionDetails.Won;
        ProtoWriter.WriteFieldHeader(23, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_2E3_0, protoWriter);
        EligibilityConditionDetails.ConditionMatchRequirment expr_2EE = eligibilityConditionDetails.MatchRequirmentEnum;
        if (expr_2EE != EligibilityConditionDetails.ConditionMatchRequirment.Any)
        {
            ProtoWriter.WriteFieldHeader(24, WireType.Variant, protoWriter);
            EligibilityConditionDetails.ConditionMatchRequirment conditionMatchRequirment = expr_2EE;
            if (conditionMatchRequirment != EligibilityConditionDetails.ConditionMatchRequirment.Any)
            {
                if (conditionMatchRequirment != EligibilityConditionDetails.ConditionMatchRequirment.All)
                {
                    if (conditionMatchRequirment != EligibilityConditionDetails.ConditionMatchRequirment.Single)
                    {
                        ProtoWriter.ThrowEnumException(protoWriter, conditionMatchRequirment);
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(2, protoWriter);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(1, protoWriter);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(0, protoWriter);
            }
        }
        string expr_34E = eligibilityConditionDetails.MatchRequirment;
        if (expr_34E != null)
        {
            ProtoWriter.WriteFieldHeader(25, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_34E, protoWriter);
        }
    }

    private static EligibilityConditionDetails Read(EligibilityConditionDetails eligibilityConditionDetails, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (num != 10)
                                                {
                                                    if (num != 11)
                                                    {
                                                        if (num != 12)
                                                        {
                                                            if (num != 13)
                                                            {
                                                                if (num != 14)
                                                                {
                                                                    if (num != 15)
                                                                    {
                                                                        if (num != 16)
                                                                        {
                                                                            if (num != 17)
                                                                            {
                                                                                if (num != 18)
                                                                                {
                                                                                    if (num != 19)
                                                                                    {
                                                                                        if (num != 20)
                                                                                        {
                                                                                            if (num != 21)
                                                                                            {
                                                                                                if (num != 22)
                                                                                                {
                                                                                                    if (num != 23)
                                                                                                    {
                                                                                                        if (num != 24)
                                                                                                        {
                                                                                                            if (num != 25)
                                                                                                            {
                                                                                                                if (eligibilityConditionDetails == null)
                                                                                                                {
                                                                                                                    EligibilityConditionDetails expr_61B = new EligibilityConditionDetails();
                                                                                                                    ProtoReader.NoteObject(expr_61B, protoReader);
                                                                                                                    eligibilityConditionDetails = expr_61B;
                                                                                                                }
                                                                                                                protoReader.SkipField();
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (eligibilityConditionDetails == null)
                                                                                                                {
                                                                                                                    EligibilityConditionDetails expr_5EE = new EligibilityConditionDetails();
                                                                                                                    ProtoReader.NoteObject(expr_5EE, protoReader);
                                                                                                                    eligibilityConditionDetails = expr_5EE;
                                                                                                                }
                                                                                                                string text = protoReader.ReadString();
                                                                                                                if (text != null)
                                                                                                                {
                                                                                                                    eligibilityConditionDetails.MatchRequirment = text;
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (eligibilityConditionDetails == null)
                                                                                                            {
                                                                                                                EligibilityConditionDetails expr_574 = new EligibilityConditionDetails();
                                                                                                                ProtoReader.NoteObject(expr_574, protoReader);
                                                                                                                eligibilityConditionDetails = expr_574;
                                                                                                            }
                                                                                                            int num2 = protoReader.ReadInt32();
                                                                                                            EligibilityConditionDetails.ConditionMatchRequirment conditionMatchRequirment = EligibilityConditionDetails.ConditionMatchRequirment.Any;
                                                                                                            if (num2 != 0)
                                                                                                            {
                                                                                                                if (num2 != 1)
                                                                                                                {
                                                                                                                    if (num2 != 2)
                                                                                                                    {
                                                                                                                        protoReader.ThrowEnumException(typeof(EligibilityConditionDetails.ConditionMatchRequirment), num2);
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        conditionMatchRequirment = EligibilityConditionDetails.ConditionMatchRequirment.Single;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    conditionMatchRequirment = EligibilityConditionDetails.ConditionMatchRequirment.All;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                conditionMatchRequirment = EligibilityConditionDetails.ConditionMatchRequirment.Any;
                                                                                                            }
                                                                                                            //conditionMatchRequirment = conditionMatchRequirment;
                                                                                                            eligibilityConditionDetails.MatchRequirmentEnum = conditionMatchRequirment;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (eligibilityConditionDetails == null)
                                                                                                        {
                                                                                                            EligibilityConditionDetails expr_543 = new EligibilityConditionDetails();
                                                                                                            ProtoReader.NoteObject(expr_543, protoReader);
                                                                                                            eligibilityConditionDetails = expr_543;
                                                                                                        }
                                                                                                        bool flag = protoReader.ReadBoolean();
                                                                                                        eligibilityConditionDetails.Won = flag;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (eligibilityConditionDetails == null)
                                                                                                    {
                                                                                                        EligibilityConditionDetails expr_495 = new EligibilityConditionDetails();
                                                                                                        ProtoReader.NoteObject(expr_495, protoReader);
                                                                                                        eligibilityConditionDetails = expr_495;
                                                                                                    }
                                                                                                    List<int> list = eligibilityConditionDetails.IntValues;
                                                                                                    List<int> list2 = list;
                                                                                                    if (list == null)
                                                                                                    {
                                                                                                        list = new List<int>();
                                                                                                    }
                                                                                                    if (protoReader.WireType != WireType.String)
                                                                                                    {
                                                                                                        int num2 = protoReader.FieldNumber;
                                                                                                        do
                                                                                                        {
                                                                                                            list.Add(protoReader.ReadInt32());
                                                                                                        }
                                                                                                        while (protoReader.TryReadFieldHeader(num2));
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        SubItemToken arg_50B_0 = ProtoReader.StartSubItem(protoReader);
                                                                                                        while (ProtoReader.HasSubValue(WireType.Variant, protoReader))
                                                                                                        {
                                                                                                            list.Add(protoReader.ReadInt32());
                                                                                                        }
                                                                                                        ProtoReader.EndSubItem(arg_50B_0, protoReader);
                                                                                                    }
                                                                                                    list2 = ((list2 == list) ? null : list);
                                                                                                    if (list2 != null)
                                                                                                    {
                                                                                                        eligibilityConditionDetails.IntValues = list2;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (eligibilityConditionDetails == null)
                                                                                                {
                                                                                                    EligibilityConditionDetails expr_422 = new EligibilityConditionDetails();
                                                                                                    ProtoReader.NoteObject(expr_422, protoReader);
                                                                                                    eligibilityConditionDetails = expr_422;
                                                                                                }
                                                                                                List<string> list3 = eligibilityConditionDetails.StringValues;
                                                                                                List<string> list4 = list3;
                                                                                                if (list3 == null)
                                                                                                {
                                                                                                    list3 = new List<string>();
                                                                                                }
                                                                                                int num2 = protoReader.FieldNumber;
                                                                                                do
                                                                                                {
                                                                                                    list3.Add(protoReader.ReadString());
                                                                                                }
                                                                                                while (protoReader.TryReadFieldHeader(num2));
                                                                                                list4 = ((list4 == list3) ? null : list3);
                                                                                                if (list4 != null)
                                                                                                {
                                                                                                    eligibilityConditionDetails.StringValues = list4;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (eligibilityConditionDetails == null)
                                                                                            {
                                                                                                EligibilityConditionDetails expr_3F1 = new EligibilityConditionDetails();
                                                                                                ProtoReader.NoteObject(expr_3F1, protoReader);
                                                                                                eligibilityConditionDetails = expr_3F1;
                                                                                            }
                                                                                            int num2 = protoReader.ReadInt32();
                                                                                            eligibilityConditionDetails.IntValue = num2;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (eligibilityConditionDetails == null)
                                                                                        {
                                                                                            EligibilityConditionDetails expr_3BA = new EligibilityConditionDetails();
                                                                                            ProtoReader.NoteObject(expr_3BA, protoReader);
                                                                                            eligibilityConditionDetails = expr_3BA;
                                                                                        }
                                                                                        string text = protoReader.ReadString();
                                                                                        if (text != null)
                                                                                        {
                                                                                            eligibilityConditionDetails.GameMode = text;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (eligibilityConditionDetails == null)
                                                                                    {
                                                                                        EligibilityConditionDetails expr_383 = new EligibilityConditionDetails();
                                                                                        ProtoReader.NoteObject(expr_383, protoReader);
                                                                                        eligibilityConditionDetails = expr_383;
                                                                                    }
                                                                                    string text = protoReader.ReadString();
                                                                                    if (text != null)
                                                                                    {
                                                                                        eligibilityConditionDetails.ThemeID = text;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (eligibilityConditionDetails == null)
                                                                                {
                                                                                    EligibilityConditionDetails expr_34C = new EligibilityConditionDetails();
                                                                                    ProtoReader.NoteObject(expr_34C, protoReader);
                                                                                    eligibilityConditionDetails = expr_34C;
                                                                                }
                                                                                string text = protoReader.ReadString();
                                                                                if (text != null)
                                                                                {
                                                                                    eligibilityConditionDetails.EventType = text;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (eligibilityConditionDetails == null)
                                                                            {
                                                                                EligibilityConditionDetails expr_315 = new EligibilityConditionDetails();
                                                                                ProtoReader.NoteObject(expr_315, protoReader);
                                                                                eligibilityConditionDetails = expr_315;
                                                                            }
                                                                            string text = protoReader.ReadString();
                                                                            if (text != null)
                                                                            {
                                                                                eligibilityConditionDetails.SequenceID = text;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (eligibilityConditionDetails == null)
                                                                        {
                                                                            EligibilityConditionDetails expr_2DE = new EligibilityConditionDetails();
                                                                            ProtoReader.NoteObject(expr_2DE, protoReader);
                                                                            eligibilityConditionDetails = expr_2DE;
                                                                        }
                                                                        string text = protoReader.ReadString();
                                                                        if (text != null)
                                                                        {
                                                                            eligibilityConditionDetails.StringValue = text;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (eligibilityConditionDetails == null)
                                                                    {
                                                                        EligibilityConditionDetails expr_2A7 = new EligibilityConditionDetails();
                                                                        ProtoReader.NoteObject(expr_2A7, protoReader);
                                                                        eligibilityConditionDetails = expr_2A7;
                                                                    }
                                                                    string text = protoReader.ReadString();
                                                                    if (text != null)
                                                                    {
                                                                        eligibilityConditionDetails.MaxTime = text;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (eligibilityConditionDetails == null)
                                                                {
                                                                    EligibilityConditionDetails expr_270 = new EligibilityConditionDetails();
                                                                    ProtoReader.NoteObject(expr_270, protoReader);
                                                                    eligibilityConditionDetails = expr_270;
                                                                }
                                                                string text = protoReader.ReadString();
                                                                if (text != null)
                                                                {
                                                                    eligibilityConditionDetails.MinTime = text;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (eligibilityConditionDetails == null)
                                                            {
                                                                EligibilityConditionDetails expr_23D = new EligibilityConditionDetails();
                                                                ProtoReader.NoteObject(expr_23D, protoReader);
                                                                eligibilityConditionDetails = expr_23D;
                                                            }
                                                            DateTime dateTime = BclHelpers.ReadDateTime(protoReader);
                                                            eligibilityConditionDetails.MaxDateTime = dateTime;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (eligibilityConditionDetails == null)
                                                        {
                                                            EligibilityConditionDetails expr_20A = new EligibilityConditionDetails();
                                                            ProtoReader.NoteObject(expr_20A, protoReader);
                                                            eligibilityConditionDetails = expr_20A;
                                                        }
                                                        DateTime dateTime = BclHelpers.ReadDateTime(protoReader);
                                                        eligibilityConditionDetails.MinDateTime = dateTime;
                                                    }
                                                }
                                                else
                                                {
                                                    if (eligibilityConditionDetails == null)
                                                    {
                                                        EligibilityConditionDetails expr_1D3 = new EligibilityConditionDetails();
                                                        ProtoReader.NoteObject(expr_1D3, protoReader);
                                                        eligibilityConditionDetails = expr_1D3;
                                                    }
                                                    string text = protoReader.ReadString();
                                                    if (text != null)
                                                    {
                                                        eligibilityConditionDetails.TimeDifference = text;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (eligibilityConditionDetails == null)
                                                {
                                                    EligibilityConditionDetails expr_1A0 = new EligibilityConditionDetails();
                                                    ProtoReader.NoteObject(expr_1A0, protoReader);
                                                    eligibilityConditionDetails = expr_1A0;
                                                }
                                                TimeSpan timeSpanDifference = BclHelpers.ReadTimeSpan(protoReader);
                                                eligibilityConditionDetails.TimeSpanDifference = timeSpanDifference;
                                            }
                                        }
                                        else
                                        {
                                            if (eligibilityConditionDetails == null)
                                            {
                                                EligibilityConditionDetails expr_169 = new EligibilityConditionDetails();
                                                ProtoReader.NoteObject(expr_169, protoReader);
                                                eligibilityConditionDetails = expr_169;
                                            }
                                            string text = protoReader.ReadString();
                                            if (text != null)
                                            {
                                                eligibilityConditionDetails.Objective = text;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (eligibilityConditionDetails == null)
                                        {
                                            EligibilityConditionDetails expr_139 = new EligibilityConditionDetails();
                                            ProtoReader.NoteObject(expr_139, protoReader);
                                            eligibilityConditionDetails = expr_139;
                                        }
                                        int num2 = protoReader.ReadInt32();
                                        eligibilityConditionDetails.Tier = num2;
                                    }
                                }
                                else
                                {
                                    if (eligibilityConditionDetails == null)
                                    {
                                        EligibilityConditionDetails expr_109 = new EligibilityConditionDetails();
                                        ProtoReader.NoteObject(expr_109, protoReader);
                                        eligibilityConditionDetails = expr_109;
                                    }
                                    int num2 = protoReader.ReadInt32();
                                    eligibilityConditionDetails.IncrementValue = num2;
                                }
                            }
                            else
                            {
                                if (eligibilityConditionDetails == null)
                                {
                                    EligibilityConditionDetails expr_D9 = new EligibilityConditionDetails();
                                    ProtoReader.NoteObject(expr_D9, protoReader);
                                    eligibilityConditionDetails = expr_D9;
                                }
                                float num3 = protoReader.ReadSingle();
                                eligibilityConditionDetails.MaxFloatValue = num3;
                            }
                        }
                        else
                        {
                            if (eligibilityConditionDetails == null)
                            {
                                EligibilityConditionDetails expr_A9 = new EligibilityConditionDetails();
                                ProtoReader.NoteObject(expr_A9, protoReader);
                                eligibilityConditionDetails = expr_A9;
                            }
                            float num3 = protoReader.ReadSingle();
                            eligibilityConditionDetails.MinFloatValue = num3;
                        }
                    }
                    else
                    {
                        if (eligibilityConditionDetails == null)
                        {
                            EligibilityConditionDetails expr_79 = new EligibilityConditionDetails();
                            ProtoReader.NoteObject(expr_79, protoReader);
                            eligibilityConditionDetails = expr_79;
                        }
                        int num2 = protoReader.ReadInt32();
                        eligibilityConditionDetails.MaxValue = num2;
                    }
                }
                else
                {
                    if (eligibilityConditionDetails == null)
                    {
                        EligibilityConditionDetails expr_49 = new EligibilityConditionDetails();
                        ProtoReader.NoteObject(expr_49, protoReader);
                        eligibilityConditionDetails = expr_49;
                    }
                    int num2 = protoReader.ReadInt32();
                    eligibilityConditionDetails.MinValue = num2;
                }
            }
            else
            {
                if (eligibilityConditionDetails == null)
                {
                    EligibilityConditionDetails expr_19 = new EligibilityConditionDetails();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    eligibilityConditionDetails = expr_19;
                }
                bool flag = protoReader.ReadBoolean();
                eligibilityConditionDetails.RequiredResult = flag;
            }
        }
        if (eligibilityConditionDetails == null)
        {
            EligibilityConditionDetails expr_643 = new EligibilityConditionDetails();
            ProtoReader.NoteObject(expr_643, protoReader);
            eligibilityConditionDetails = expr_643;
        }
        return eligibilityConditionDetails;
    }

    private static void Write(EligibilityConditionDetails.ConditionMatchRequirment conditionMatchRequirment, ProtoWriter writer)
    {
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        if (conditionMatchRequirment != EligibilityConditionDetails.ConditionMatchRequirment.Any)
        {
            if (conditionMatchRequirment != EligibilityConditionDetails.ConditionMatchRequirment.All)
            {
                if (conditionMatchRequirment != EligibilityConditionDetails.ConditionMatchRequirment.Single)
                {
                    ProtoWriter.ThrowEnumException(writer, conditionMatchRequirment);
                }
                else
                {
                    ProtoWriter.WriteInt32(2, writer);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(1, writer);
            }
        }
        else
        {
            ProtoWriter.WriteInt32(0, writer);
        }
    }

    private static EligibilityConditionDetails.ConditionMatchRequirment Read(EligibilityConditionDetails.ConditionMatchRequirment conditionMatchRequirment, ProtoReader protoReader)
    {
        int num = protoReader.ReadInt32();
        EligibilityConditionDetails.ConditionMatchRequirment result = EligibilityConditionDetails.ConditionMatchRequirment.Any;
        if (num != 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    protoReader.ThrowEnumException(typeof(EligibilityConditionDetails.ConditionMatchRequirment), num);
                }
                else
                {
                    result = EligibilityConditionDetails.ConditionMatchRequirment.Single;
                }
            }
            else
            {
                result = EligibilityConditionDetails.ConditionMatchRequirment.All;
            }
        }
        else
        {
            result = EligibilityConditionDetails.ConditionMatchRequirment.Any;
        }
        return result;
    }

    private static void Write(EligibilityRequirements eligibilityRequirements, ProtoWriter protoWriter)
    {
        if (eligibilityRequirements.GetType() != typeof(EligibilityRequirements))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(EligibilityRequirements), eligibilityRequirements.GetType());
        }
        List<EligibilityRequirements.PossibleGameState> expr_2D = eligibilityRequirements.PossibleGameStates;
        if (expr_2D != null)
        {
            List<EligibilityRequirements.PossibleGameState> list = expr_2D;
            foreach (EligibilityRequirements.PossibleGameState arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static EligibilityRequirements Read(EligibilityRequirements eligibilityRequirements, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (eligibilityRequirements == null)
                {
                    EligibilityRequirements expr_87 = new EligibilityRequirements();
                    ProtoReader.NoteObject(expr_87, protoReader);
                    eligibilityRequirements = expr_87;
                }
                protoReader.SkipField();
            }
            else
            {
                if (eligibilityRequirements == null)
                {
                    EligibilityRequirements expr_19 = new EligibilityRequirements();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    eligibilityRequirements = expr_19;
                }
                List<EligibilityRequirements.PossibleGameState> list = eligibilityRequirements.PossibleGameStates;
                List<EligibilityRequirements.PossibleGameState> list2 = list;
                if (list == null)
                {
                    list = new List<EligibilityRequirements.PossibleGameState>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<EligibilityRequirements.PossibleGameState> arg_53_0 = list;
                    EligibilityRequirements.PossibleGameState arg_46_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityRequirements.PossibleGameState arg_53_1 = TierXConfigurationSerializer.Read(arg_46_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_53_0.Add(arg_53_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    eligibilityRequirements.PossibleGameStates = list2;
                }
            }
        }
        if (eligibilityRequirements == null)
        {
            EligibilityRequirements expr_AF = new EligibilityRequirements();
            ProtoReader.NoteObject(expr_AF, protoReader);
            eligibilityRequirements = expr_AF;
        }
        return eligibilityRequirements;
    }

    private static void Write(EligibilityRequirements.PossibleGameState possibleGameState, ProtoWriter protoWriter)
    {
        if (possibleGameState.GetType() != typeof(EligibilityRequirements.PossibleGameState))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(EligibilityRequirements.PossibleGameState), possibleGameState.GetType());
        }
        List<EligibilityCondition> expr_2D = possibleGameState.Conditions;
        if (expr_2D != null)
        {
            List<EligibilityCondition> list = expr_2D;
            foreach (EligibilityCondition arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static EligibilityRequirements.PossibleGameState Read(EligibilityRequirements.PossibleGameState possibleGameState, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (possibleGameState == null)
                {
                    EligibilityRequirements.PossibleGameState expr_87 = new EligibilityRequirements.PossibleGameState();
                    ProtoReader.NoteObject(expr_87, protoReader);
                    possibleGameState = expr_87;
                }
                protoReader.SkipField();
            }
            else
            {
                if (possibleGameState == null)
                {
                    EligibilityRequirements.PossibleGameState expr_19 = new EligibilityRequirements.PossibleGameState();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    possibleGameState = expr_19;
                }
                List<EligibilityCondition> list = possibleGameState.Conditions;
                List<EligibilityCondition> list2 = list;
                if (list == null)
                {
                    list = new List<EligibilityCondition>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<EligibilityCondition> arg_53_0 = list;
                    EligibilityCondition arg_46_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityCondition arg_53_1 = TierXConfigurationSerializer.Read(arg_46_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_53_0.Add(arg_53_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    possibleGameState.Conditions = list2;
                }
            }
        }
        if (possibleGameState == null)
        {
            EligibilityRequirements.PossibleGameState expr_AF = new EligibilityRequirements.PossibleGameState();
            ProtoReader.NoteObject(expr_AF, protoReader);
            possibleGameState = expr_AF;
        }
        return possibleGameState;
    }

    private static void Write(EventSelectEvent eventSelectEvent, ProtoWriter protoWriter)
    {
        if (eventSelectEvent.GetType() != typeof(EventSelectEvent))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(EventSelectEvent), eventSelectEvent.GetType());
        }
        string expr_2D = eventSelectEvent.FunctionName;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        float expr_4A = eventSelectEvent.EventTime;
        if (expr_4A != 0f)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, protoWriter);
            ProtoWriter.WriteSingle(expr_4A, protoWriter);
        }
        EligibilityRequirements expr_6C = eventSelectEvent.EventRequirements;
        if (expr_6C != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_6C, protoWriter);
            TierXConfigurationSerializer.Write(expr_6C, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static EventSelectEvent Read(EventSelectEvent eventSelectEvent, ProtoReader protoReader)
    {
        if (eventSelectEvent != null)
        {
            eventSelectEvent.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (eventSelectEvent == null)
                        {
                            EventSelectEvent expr_DB = new EventSelectEvent();
                            ProtoReader.NoteObject(expr_DB, protoReader);
                            expr_DB.Init();
                            eventSelectEvent = expr_DB;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (eventSelectEvent == null)
                        {
                            EventSelectEvent expr_94 = new EventSelectEvent();
                            ProtoReader.NoteObject(expr_94, protoReader);
                            expr_94.Init();
                            eventSelectEvent = expr_94;
                        }
                        EligibilityRequirements arg_B1_0 = eventSelectEvent.EventRequirements;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        EligibilityRequirements arg_BD_0 = TierXConfigurationSerializer.Read(arg_B1_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        EligibilityRequirements eligibilityRequirements = arg_BD_0;
                        if (eligibilityRequirements != null)
                        {
                            eventSelectEvent.EventRequirements = eligibilityRequirements;
                        }
                    }
                }
                else
                {
                    if (eventSelectEvent == null)
                    {
                        EventSelectEvent expr_5E = new EventSelectEvent();
                        ProtoReader.NoteObject(expr_5E, protoReader);
                        expr_5E.Init();
                        eventSelectEvent = expr_5E;
                    }
                    float eventTime = protoReader.ReadSingle();
                    eventSelectEvent.EventTime = eventTime;
                }
            }
            else
            {
                if (eventSelectEvent == null)
                {
                    EventSelectEvent expr_25 = new EventSelectEvent();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    eventSelectEvent = expr_25;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    eventSelectEvent.FunctionName = text;
                }
            }
        }
        if (eventSelectEvent == null)
        {
            EventSelectEvent expr_109 = new EventSelectEvent();
            ProtoReader.NoteObject(expr_109, protoReader);
            expr_109.Init();
            eventSelectEvent = expr_109;
        }
        return eventSelectEvent;
    }

    private static void Write(FormatStringData formatStringData, ProtoWriter protoWriter)
    {
        if (formatStringData.GetType() != typeof(FormatStringData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(FormatStringData), formatStringData.GetType());
        }
        string expr_2D = formatStringData.StringFormatBase;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        bool expr_4A = formatStringData.AlreadyTranslated;
        if (expr_4A)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_4A, protoWriter);
        }
        List<FormatStringParameterData> expr_67 = formatStringData.StringFormatParameters;
        if (expr_67 != null)
        {
            List<FormatStringParameterData> list = expr_67;
            foreach (FormatStringParameterData arg_8C_0 in list)
            {
                ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_8C_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_8C_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static FormatStringData Read(FormatStringData formatStringData, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (formatStringData == null)
                        {
                            FormatStringData expr_F1 = new FormatStringData();
                            ProtoReader.NoteObject(expr_F1, protoReader);
                            formatStringData = expr_F1;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (formatStringData == null)
                        {
                            FormatStringData expr_7C = new FormatStringData();
                            ProtoReader.NoteObject(expr_7C, protoReader);
                            formatStringData = expr_7C;
                        }
                        List<FormatStringParameterData> list = formatStringData.StringFormatParameters;
                        List<FormatStringParameterData> list2 = list;
                        if (list == null)
                        {
                            list = new List<FormatStringParameterData>();
                        }
                        int fieldNumber = protoReader.FieldNumber;
                        do
                        {
                            List<FormatStringParameterData> arg_B8_0 = list;
                            FormatStringParameterData arg_AB_0 = null;
                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                            FormatStringParameterData arg_B8_1 = TierXConfigurationSerializer.Read(arg_AB_0, protoReader);
                            ProtoReader.EndSubItem(token, protoReader);
                            arg_B8_0.Add(arg_B8_1);
                        }
                        while (protoReader.TryReadFieldHeader(fieldNumber));
                        list2 = ((list2 == list) ? null : list);
                        if (list2 != null)
                        {
                            formatStringData.StringFormatParameters = list2;
                        }
                    }
                }
                else
                {
                    if (formatStringData == null)
                    {
                        FormatStringData expr_4C = new FormatStringData();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        formatStringData = expr_4C;
                    }
                    bool alreadyTranslated = protoReader.ReadBoolean();
                    formatStringData.AlreadyTranslated = alreadyTranslated;
                }
            }
            else
            {
                if (formatStringData == null)
                {
                    FormatStringData expr_19 = new FormatStringData();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    formatStringData = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    formatStringData.StringFormatBase = text;
                }
            }
        }
        if (formatStringData == null)
        {
            FormatStringData expr_119 = new FormatStringData();
            ProtoReader.NoteObject(expr_119, protoReader);
            formatStringData = expr_119;
        }
        return formatStringData;
    }

    private static void Write(FormatStringParameterData formatStringParameterData, ProtoWriter writer)
    {
        if (formatStringParameterData.GetType() != typeof(FormatStringParameterData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(FormatStringParameterData), formatStringParameterData.GetType());
        }
        string expr_2D = formatStringParameterData.Value;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        string expr_4A = formatStringParameterData.DecorationString;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
    }

    private static FormatStringParameterData Read(FormatStringParameterData formatStringParameterData, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (formatStringParameterData == null)
                    {
                        FormatStringParameterData expr_76 = new FormatStringParameterData();
                        ProtoReader.NoteObject(expr_76, protoReader);
                        formatStringParameterData = expr_76;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (formatStringParameterData == null)
                    {
                        FormatStringParameterData expr_4C = new FormatStringParameterData();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        formatStringParameterData = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        formatStringParameterData.DecorationString = text;
                    }
                }
            }
            else
            {
                if (formatStringParameterData == null)
                {
                    FormatStringParameterData expr_19 = new FormatStringParameterData();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    formatStringParameterData = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    formatStringParameterData.Value = text;
                }
            }
        }
        if (formatStringParameterData == null)
        {
            FormatStringParameterData expr_9E = new FormatStringParameterData();
            ProtoReader.NoteObject(expr_9E, protoReader);
            formatStringParameterData = expr_9E;
        }
        return formatStringParameterData;
    }

    private static void Write(LoadTierAction loadTierAction, ProtoWriter protoWriter)
    {
        if (loadTierAction.GetType() != typeof(LoadTierAction))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(LoadTierAction), loadTierAction.GetType());
        }
        string expr_2D = loadTierAction.eventPaneTitle;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = loadTierAction.eventPaneDesc;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        string expr_67 = loadTierAction.eventPaneSprite;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_67, protoWriter);
        }
        string expr_84 = loadTierAction.themeToLoad;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_84, protoWriter);
        }
        string expr_A1 = loadTierAction.themeOption;
        if (expr_A1 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_A1, protoWriter);
        }
        List<PopupData> expr_BE = loadTierAction.entryPopups;
        if (expr_BE != null)
        {
            List<PopupData> list = expr_BE;
            foreach (PopupData arg_E3_0 in list)
            {
                ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_E3_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_E3_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static LoadTierAction Read(LoadTierAction loadTierAction, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (loadTierAction == null)
                                    {
                                        LoadTierAction expr_188 = new LoadTierAction();
                                        ProtoReader.NoteObject(expr_188, protoReader);
                                        loadTierAction = expr_188;
                                    }
                                    protoReader.SkipField();
                                }
                                else
                                {
                                    if (loadTierAction == null)
                                    {
                                        LoadTierAction expr_118 = new LoadTierAction();
                                        ProtoReader.NoteObject(expr_118, protoReader);
                                        loadTierAction = expr_118;
                                    }
                                    List<PopupData> list = loadTierAction.entryPopups;
                                    List<PopupData> list2 = list;
                                    if (list == null)
                                    {
                                        list = new List<PopupData>();
                                    }
                                    int fieldNumber = protoReader.FieldNumber;
                                    do
                                    {
                                        List<PopupData> arg_153_0 = list;
                                        PopupData arg_146_0 = null;
                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                        PopupData arg_153_1 = TierXConfigurationSerializer.Read(arg_146_0, protoReader);
                                        ProtoReader.EndSubItem(token, protoReader);
                                        arg_153_0.Add(arg_153_1);
                                    }
                                    while (protoReader.TryReadFieldHeader(fieldNumber));
                                    list2 = ((list2 == list) ? null : list);
                                    if (list2 != null)
                                    {
                                        loadTierAction.entryPopups = list2;
                                    }
                                }
                            }
                            else
                            {
                                if (loadTierAction == null)
                                {
                                    LoadTierAction expr_E5 = new LoadTierAction();
                                    ProtoReader.NoteObject(expr_E5, protoReader);
                                    loadTierAction = expr_E5;
                                }
                                string text = protoReader.ReadString();
                                if (text != null)
                                {
                                    loadTierAction.themeOption = text;
                                }
                            }
                        }
                        else
                        {
                            if (loadTierAction == null)
                            {
                                LoadTierAction expr_B2 = new LoadTierAction();
                                ProtoReader.NoteObject(expr_B2, protoReader);
                                loadTierAction = expr_B2;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                loadTierAction.themeToLoad = text;
                            }
                        }
                    }
                    else
                    {
                        if (loadTierAction == null)
                        {
                            LoadTierAction expr_7F = new LoadTierAction();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            loadTierAction = expr_7F;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            loadTierAction.eventPaneSprite = text;
                        }
                    }
                }
                else
                {
                    if (loadTierAction == null)
                    {
                        LoadTierAction expr_4C = new LoadTierAction();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        loadTierAction = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        loadTierAction.eventPaneDesc = text;
                    }
                }
            }
            else
            {
                if (loadTierAction == null)
                {
                    LoadTierAction expr_19 = new LoadTierAction();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    loadTierAction = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    loadTierAction.eventPaneTitle = text;
                }
            }
        }
        if (loadTierAction == null)
        {
            LoadTierAction expr_1B0 = new LoadTierAction();
            ProtoReader.NoteObject(expr_1B0, protoReader);
            loadTierAction = expr_1B0;
        }
        return loadTierAction;
    }

    private static void Write(NarrativeSceneForEventData narrativeSceneForEventData, ProtoWriter protoWriter)
    {
        if (narrativeSceneForEventData.GetType() != typeof(NarrativeSceneForEventData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(NarrativeSceneForEventData), narrativeSceneForEventData.GetType());
        }
        string expr_2D = narrativeSceneForEventData.PreRaceSceneID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = narrativeSceneForEventData.PostRaceWinSceneID;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        string expr_67 = narrativeSceneForEventData.PostRaceLoseSceneID;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_67, protoWriter);
        }
        string expr_84 = narrativeSceneForEventData.IntroSceneID;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_84, protoWriter);
        }
        ConditionallySelectedString expr_A1 = narrativeSceneForEventData.ConditionalPreRaceSceneID;
        if (expr_A1 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_A1, protoWriter);
            TierXConfigurationSerializer.Write(expr_A1, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        ConditionallySelectedString expr_CD = narrativeSceneForEventData.ConditionalPostRaceWinSceneID;
        if (expr_CD != null)
        {
            ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_CD, protoWriter);
            TierXConfigurationSerializer.Write(expr_CD, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        ConditionallySelectedString expr_F9 = narrativeSceneForEventData.ConditionalPostRaceLoseSceneID;
        if (expr_F9 != null)
        {
            ProtoWriter.WriteFieldHeader(7, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_F9, protoWriter);
            TierXConfigurationSerializer.Write(expr_F9, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        ConditionallySelectedString expr_125 = narrativeSceneForEventData.ConditionalIntroSceneID;
        if (expr_125 != null)
        {
            ProtoWriter.WriteFieldHeader(8, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_125, protoWriter);
            TierXConfigurationSerializer.Write(expr_125, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        EligibilityRequirements expr_151 = narrativeSceneForEventData.PostRaceRequirements;
        if (expr_151 != null)
        {
            ProtoWriter.WriteFieldHeader(9, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_151, protoWriter);
            TierXConfigurationSerializer.Write(expr_151, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        EligibilityRequirements expr_17E = narrativeSceneForEventData.PreRaceRequirements;
        if (expr_17E != null)
        {
            ProtoWriter.WriteFieldHeader(10, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_17E, protoWriter);
            TierXConfigurationSerializer.Write(expr_17E, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        EligibilityRequirements expr_1AB = narrativeSceneForEventData.IntroRequirements;
        if (expr_1AB != null)
        {
            ProtoWriter.WriteFieldHeader(11, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_1AB, protoWriter);
            TierXConfigurationSerializer.Write(expr_1AB, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static NarrativeSceneForEventData Read(NarrativeSceneForEventData narrativeSceneForEventData, ProtoReader protoReader)
    {
        if (narrativeSceneForEventData != null)
        {
            narrativeSceneForEventData.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (num != 10)
                                                {
                                                    if (num != 11)
                                                    {
                                                        if (narrativeSceneForEventData == null)
                                                        {
                                                            NarrativeSceneForEventData expr_327 = new NarrativeSceneForEventData();
                                                            ProtoReader.NoteObject(expr_327, protoReader);
                                                            expr_327.Init();
                                                            narrativeSceneForEventData = expr_327;
                                                        }
                                                        protoReader.SkipField();
                                                    }
                                                    else
                                                    {
                                                        if (narrativeSceneForEventData == null)
                                                        {
                                                            NarrativeSceneForEventData expr_2E0 = new NarrativeSceneForEventData();
                                                            ProtoReader.NoteObject(expr_2E0, protoReader);
                                                            expr_2E0.Init();
                                                            narrativeSceneForEventData = expr_2E0;
                                                        }
                                                        EligibilityRequirements arg_2FD_0 = narrativeSceneForEventData.IntroRequirements;
                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                        EligibilityRequirements arg_309_0 = TierXConfigurationSerializer.Read(arg_2FD_0, protoReader);
                                                        ProtoReader.EndSubItem(token, protoReader);
                                                        EligibilityRequirements eligibilityRequirements = arg_309_0;
                                                        if (eligibilityRequirements != null)
                                                        {
                                                            narrativeSceneForEventData.IntroRequirements = eligibilityRequirements;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (narrativeSceneForEventData == null)
                                                    {
                                                        NarrativeSceneForEventData expr_28F = new NarrativeSceneForEventData();
                                                        ProtoReader.NoteObject(expr_28F, protoReader);
                                                        expr_28F.Init();
                                                        narrativeSceneForEventData = expr_28F;
                                                    }
                                                    EligibilityRequirements arg_2AC_0 = narrativeSceneForEventData.PreRaceRequirements;
                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                    EligibilityRequirements arg_2B8_0 = TierXConfigurationSerializer.Read(arg_2AC_0, protoReader);
                                                    ProtoReader.EndSubItem(token, protoReader);
                                                    EligibilityRequirements eligibilityRequirements = arg_2B8_0;
                                                    if (eligibilityRequirements != null)
                                                    {
                                                        narrativeSceneForEventData.PreRaceRequirements = eligibilityRequirements;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (narrativeSceneForEventData == null)
                                                {
                                                    NarrativeSceneForEventData expr_23E = new NarrativeSceneForEventData();
                                                    ProtoReader.NoteObject(expr_23E, protoReader);
                                                    expr_23E.Init();
                                                    narrativeSceneForEventData = expr_23E;
                                                }
                                                EligibilityRequirements arg_25B_0 = narrativeSceneForEventData.PostRaceRequirements;
                                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                EligibilityRequirements arg_267_0 = TierXConfigurationSerializer.Read(arg_25B_0, protoReader);
                                                ProtoReader.EndSubItem(token, protoReader);
                                                EligibilityRequirements eligibilityRequirements = arg_267_0;
                                                if (eligibilityRequirements != null)
                                                {
                                                    narrativeSceneForEventData.PostRaceRequirements = eligibilityRequirements;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (narrativeSceneForEventData == null)
                                            {
                                                NarrativeSceneForEventData expr_1F0 = new NarrativeSceneForEventData();
                                                ProtoReader.NoteObject(expr_1F0, protoReader);
                                                expr_1F0.Init();
                                                narrativeSceneForEventData = expr_1F0;
                                            }
                                            ConditionallySelectedString arg_20D_0 = narrativeSceneForEventData.ConditionalIntroSceneID;
                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                            ConditionallySelectedString arg_219_0 = TierXConfigurationSerializer.Read(arg_20D_0, protoReader);
                                            ProtoReader.EndSubItem(token, protoReader);
                                            ConditionallySelectedString conditionallySelectedString = arg_219_0;
                                            if (conditionallySelectedString != null)
                                            {
                                                narrativeSceneForEventData.ConditionalIntroSceneID = conditionallySelectedString;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (narrativeSceneForEventData == null)
                                        {
                                            NarrativeSceneForEventData expr_1A3 = new NarrativeSceneForEventData();
                                            ProtoReader.NoteObject(expr_1A3, protoReader);
                                            expr_1A3.Init();
                                            narrativeSceneForEventData = expr_1A3;
                                        }
                                        ConditionallySelectedString arg_1C0_0 = narrativeSceneForEventData.ConditionalPostRaceLoseSceneID;
                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                        ConditionallySelectedString arg_1CC_0 = TierXConfigurationSerializer.Read(arg_1C0_0, protoReader);
                                        ProtoReader.EndSubItem(token, protoReader);
                                        ConditionallySelectedString conditionallySelectedString = arg_1CC_0;
                                        if (conditionallySelectedString != null)
                                        {
                                            narrativeSceneForEventData.ConditionalPostRaceLoseSceneID = conditionallySelectedString;
                                        }
                                    }
                                }
                                else
                                {
                                    if (narrativeSceneForEventData == null)
                                    {
                                        NarrativeSceneForEventData expr_156 = new NarrativeSceneForEventData();
                                        ProtoReader.NoteObject(expr_156, protoReader);
                                        expr_156.Init();
                                        narrativeSceneForEventData = expr_156;
                                    }
                                    ConditionallySelectedString arg_173_0 = narrativeSceneForEventData.ConditionalPostRaceWinSceneID;
                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                    ConditionallySelectedString arg_17F_0 = TierXConfigurationSerializer.Read(arg_173_0, protoReader);
                                    ProtoReader.EndSubItem(token, protoReader);
                                    ConditionallySelectedString conditionallySelectedString = arg_17F_0;
                                    if (conditionallySelectedString != null)
                                    {
                                        narrativeSceneForEventData.ConditionalPostRaceWinSceneID = conditionallySelectedString;
                                    }
                                }
                            }
                            else
                            {
                                if (narrativeSceneForEventData == null)
                                {
                                    NarrativeSceneForEventData expr_109 = new NarrativeSceneForEventData();
                                    ProtoReader.NoteObject(expr_109, protoReader);
                                    expr_109.Init();
                                    narrativeSceneForEventData = expr_109;
                                }
                                ConditionallySelectedString arg_126_0 = narrativeSceneForEventData.ConditionalPreRaceSceneID;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                ConditionallySelectedString arg_132_0 = TierXConfigurationSerializer.Read(arg_126_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                ConditionallySelectedString conditionallySelectedString = arg_132_0;
                                if (conditionallySelectedString != null)
                                {
                                    narrativeSceneForEventData.ConditionalPreRaceSceneID = conditionallySelectedString;
                                }
                            }
                        }
                        else
                        {
                            if (narrativeSceneForEventData == null)
                            {
                                NarrativeSceneForEventData expr_D0 = new NarrativeSceneForEventData();
                                ProtoReader.NoteObject(expr_D0, protoReader);
                                expr_D0.Init();
                                narrativeSceneForEventData = expr_D0;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                narrativeSceneForEventData.IntroSceneID = text;
                            }
                        }
                    }
                    else
                    {
                        if (narrativeSceneForEventData == null)
                        {
                            NarrativeSceneForEventData expr_97 = new NarrativeSceneForEventData();
                            ProtoReader.NoteObject(expr_97, protoReader);
                            expr_97.Init();
                            narrativeSceneForEventData = expr_97;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            narrativeSceneForEventData.PostRaceLoseSceneID = text;
                        }
                    }
                }
                else
                {
                    if (narrativeSceneForEventData == null)
                    {
                        NarrativeSceneForEventData expr_5E = new NarrativeSceneForEventData();
                        ProtoReader.NoteObject(expr_5E, protoReader);
                        expr_5E.Init();
                        narrativeSceneForEventData = expr_5E;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        narrativeSceneForEventData.PostRaceWinSceneID = text;
                    }
                }
            }
            else
            {
                if (narrativeSceneForEventData == null)
                {
                    NarrativeSceneForEventData expr_25 = new NarrativeSceneForEventData();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    narrativeSceneForEventData = expr_25;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    narrativeSceneForEventData.PreRaceSceneID = text;
                }
            }
        }
        if (narrativeSceneForEventData == null)
        {
            NarrativeSceneForEventData expr_355 = new NarrativeSceneForEventData();
            ProtoReader.NoteObject(expr_355, protoReader);
            expr_355.Init();
            narrativeSceneForEventData = expr_355;
        }
        return narrativeSceneForEventData;
    }

    private static void Write(PinAnimationDetail pinAnimationDetail, ProtoWriter protoWriter)
    {
        if (pinAnimationDetail.GetType() != typeof(PinAnimationDetail))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinAnimationDetail), pinAnimationDetail.GetType());
        }
        string expr_2D = pinAnimationDetail.Name;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = pinAnimationDetail.PinLabel;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        float expr_67 = pinAnimationDetail.EventTime;
        if (expr_67 != 0f)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.Fixed32, protoWriter);
            ProtoWriter.WriteSingle(expr_67, protoWriter);
        }
        bool expr_89 = pinAnimationDetail.Required;
        if (expr_89)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_89, protoWriter);
        }
        EligibilityRequirements expr_A6 = pinAnimationDetail.AnimationRequirements;
        if (expr_A6 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_A6, protoWriter);
            TierXConfigurationSerializer.Write(expr_A6, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static PinAnimationDetail Read(PinAnimationDetail pinAnimationDetail, ProtoReader protoReader)
    {
        if (pinAnimationDetail != null)
        {
            pinAnimationDetail.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (pinAnimationDetail == null)
                                {
                                    PinAnimationDetail expr_14C = new PinAnimationDetail();
                                    ProtoReader.NoteObject(expr_14C, protoReader);
                                    expr_14C.Init();
                                    pinAnimationDetail = expr_14C;
                                }
                                protoReader.SkipField();
                            }
                            else
                            {
                                if (pinAnimationDetail == null)
                                {
                                    PinAnimationDetail expr_103 = new PinAnimationDetail();
                                    ProtoReader.NoteObject(expr_103, protoReader);
                                    expr_103.Init();
                                    pinAnimationDetail = expr_103;
                                }
                                EligibilityRequirements arg_121_0 = pinAnimationDetail.AnimationRequirements;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                EligibilityRequirements arg_12E_0 = TierXConfigurationSerializer.Read(arg_121_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                EligibilityRequirements eligibilityRequirements = arg_12E_0;
                                if (eligibilityRequirements != null)
                                {
                                    pinAnimationDetail.AnimationRequirements = eligibilityRequirements;
                                }
                            }
                        }
                        else
                        {
                            if (pinAnimationDetail == null)
                            {
                                PinAnimationDetail expr_CD = new PinAnimationDetail();
                                ProtoReader.NoteObject(expr_CD, protoReader);
                                expr_CD.Init();
                                pinAnimationDetail = expr_CD;
                            }
                            bool required = protoReader.ReadBoolean();
                            pinAnimationDetail.Required = required;
                        }
                    }
                    else
                    {
                        if (pinAnimationDetail == null)
                        {
                            PinAnimationDetail expr_97 = new PinAnimationDetail();
                            ProtoReader.NoteObject(expr_97, protoReader);
                            expr_97.Init();
                            pinAnimationDetail = expr_97;
                        }
                        float eventTime = protoReader.ReadSingle();
                        pinAnimationDetail.EventTime = eventTime;
                    }
                }
                else
                {
                    if (pinAnimationDetail == null)
                    {
                        PinAnimationDetail expr_5E = new PinAnimationDetail();
                        ProtoReader.NoteObject(expr_5E, protoReader);
                        expr_5E.Init();
                        pinAnimationDetail = expr_5E;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        pinAnimationDetail.PinLabel = text;
                    }
                }
            }
            else
            {
                if (pinAnimationDetail == null)
                {
                    PinAnimationDetail expr_25 = new PinAnimationDetail();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    pinAnimationDetail = expr_25;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pinAnimationDetail.Name = text;
                }
            }
        }
        if (pinAnimationDetail == null)
        {
            PinAnimationDetail expr_17A = new PinAnimationDetail();
            ProtoReader.NoteObject(expr_17A, protoReader);
            expr_17A.Init();
            pinAnimationDetail = expr_17A;
        }
        return pinAnimationDetail;
    }

    private static void Write(PinDetail pinDetail, ProtoWriter protoWriter)
    {
        if (pinDetail.GetType() != typeof(PinDetail))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinDetail), pinDetail.GetType());
        }
        string expr_2D = pinDetail.PinID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = pinDetail.TemplateName;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        Vector2 arg_78_0 = pinDetail.Position;
        ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_78_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        Vector2 arg_9B_0 = pinDetail.PositionOffset;
        ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_9B_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        string expr_AD = pinDetail.LoadingScreen;
        if (expr_AD != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_AD, protoWriter);
        }
        string expr_CA = pinDetail.Title;
        if (expr_CA != null)
        {
            ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_CA, protoWriter);
        }
        string expr_E7 = pinDetail.EventDescription;
        if (expr_E7 != null)
        {
            ProtoWriter.WriteFieldHeader(7, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_E7, protoWriter);
        }
        bool arg_10D_0 = pinDetail.ProgressIndicator;
        ProtoWriter.WriteFieldHeader(8, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_10D_0, protoWriter);
        bool arg_122_0 = pinDetail.ShowSelectionArrow;
        ProtoWriter.WriteFieldHeader(9, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_122_0, protoWriter);
        bool arg_137_0 = pinDetail.CanDisplayAsComplete;
        ProtoWriter.WriteFieldHeader(10, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_137_0, protoWriter);
        bool arg_14C_0 = pinDetail.IsSelectable;
        ProtoWriter.WriteFieldHeader(11, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_14C_0, protoWriter);
        bool arg_161_0 = pinDetail.HideTextBox;
        ProtoWriter.WriteFieldHeader(12, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_161_0, protoWriter);
        int arg_176_0 = pinDetail.EventID;
        ProtoWriter.WriteFieldHeader(13, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_176_0, protoWriter);
        int arg_18B_0 = pinDetail.GroupID;
        ProtoWriter.WriteFieldHeader(14, WireType.Variant, protoWriter);
        ProtoWriter.WriteInt32(arg_18B_0, protoWriter);
        bool arg_1A0_0 = pinDetail.IsSuperNitrous;
        ProtoWriter.WriteFieldHeader(15, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_1A0_0, protoWriter);
        bool arg_1B5_0 = pinDetail.IsProgressPin;
        ProtoWriter.WriteFieldHeader(16, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_1B5_0, protoWriter);
        PinLock expr_1C0 = pinDetail.Lock;
        if (expr_1C0 != null)
        {
            ProtoWriter.WriteFieldHeader(17, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_1C0, protoWriter);
            TierXConfigurationSerializer.Write(expr_1C0, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        LoadTierAction expr_1ED = pinDetail.ClickAction;
        if (expr_1ED != null)
        {
            ProtoWriter.WriteFieldHeader(18, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_1ED, protoWriter);
            TierXConfigurationSerializer.Write(expr_1ED, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        PushScreenAction expr_21A = pinDetail.PushScreenAction;
        if (expr_21A != null)
        {
            ProtoWriter.WriteFieldHeader(19, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_21A, protoWriter);
            TierXConfigurationSerializer.Write(expr_21A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        Color arg_259_0 = pinDetail.Colour;
        ProtoWriter.WriteFieldHeader(20, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_259_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        Dictionary<string, TextureDetail> expr_26B = pinDetail.Textures;
        if (expr_26B != null)
        {
            Dictionary<string, TextureDetail> dictionary = expr_26B;
            foreach (KeyValuePair<string, TextureDetail> arg_29A_0 in dictionary)
            {
                ProtoWriter.WriteFieldHeader(21, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(null, protoWriter);
                TierXConfigurationSerializer.Write(arg_29A_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        Dictionary<string, ConditionallySelectedString> expr_2D0 = pinDetail.ConditionallySelectedStrings;
        if (expr_2D0 != null)
        {
            Dictionary<string, ConditionallySelectedString> dictionary2 = expr_2D0;
            foreach (KeyValuePair<string, ConditionallySelectedString> arg_300_0 in dictionary2)
            {
                ProtoWriter.WriteFieldHeader(22, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(null, protoWriter);
                TierXConfigurationSerializer.Write(arg_300_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        float expr_336 = pinDetail.Greyness;
        if (expr_336 != 0f)
        {
            ProtoWriter.WriteFieldHeader(23, WireType.Fixed32, protoWriter);
            ProtoWriter.WriteSingle(expr_336, protoWriter);
        }
        string expr_359 = pinDetail.CompletedTitle;
        if (expr_359 != null)
        {
            ProtoWriter.WriteFieldHeader(24, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_359, protoWriter);
        }
    }

    private static PinDetail Read(PinDetail pinDetail, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (num != 10)
                                                {
                                                    if (num != 11)
                                                    {
                                                        if (num != 12)
                                                        {
                                                            if (num != 13)
                                                            {
                                                                if (num != 14)
                                                                {
                                                                    if (num != 15)
                                                                    {
                                                                        if (num != 16)
                                                                        {
                                                                            if (num != 17)
                                                                            {
                                                                                if (num != 18)
                                                                                {
                                                                                    if (num != 19)
                                                                                    {
                                                                                        if (num != 20)
                                                                                        {
                                                                                            if (num != 21)
                                                                                            {
                                                                                                if (num != 22)
                                                                                                {
                                                                                                    if (num != 23)
                                                                                                    {
                                                                                                        if (num != 24)
                                                                                                        {
                                                                                                            if (pinDetail == null)
                                                                                                            {
                                                                                                                PinDetail expr_60A = new PinDetail();
                                                                                                                ProtoReader.NoteObject(expr_60A, protoReader);
                                                                                                                pinDetail = expr_60A;
                                                                                                            }
                                                                                                            protoReader.SkipField();
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (pinDetail == null)
                                                                                                            {
                                                                                                                PinDetail expr_5E0 = new PinDetail();
                                                                                                                ProtoReader.NoteObject(expr_5E0, protoReader);
                                                                                                                pinDetail = expr_5E0;
                                                                                                            }
                                                                                                            string text = protoReader.ReadString();
                                                                                                            if (text != null)
                                                                                                            {
                                                                                                                pinDetail.CompletedTitle = text;
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (pinDetail == null)
                                                                                                        {
                                                                                                            PinDetail expr_5AD = new PinDetail();
                                                                                                            ProtoReader.NoteObject(expr_5AD, protoReader);
                                                                                                            pinDetail = expr_5AD;
                                                                                                        }
                                                                                                        float greyness = protoReader.ReadSingle();
                                                                                                        pinDetail.Greyness = greyness;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (pinDetail == null)
                                                                                                    {
                                                                                                        PinDetail expr_520 = new PinDetail();
                                                                                                        ProtoReader.NoteObject(expr_520, protoReader);
                                                                                                        pinDetail = expr_520;
                                                                                                    }
                                                                                                    Dictionary<string, ConditionallySelectedString> dictionary = pinDetail.ConditionallySelectedStrings;
                                                                                                    Dictionary<string, ConditionallySelectedString> dictionary2 = dictionary;
                                                                                                    if (dictionary == null)
                                                                                                    {
                                                                                                        dictionary = new Dictionary<string, ConditionallySelectedString>();
                                                                                                    }
                                                                                                    int num2 = protoReader.FieldNumber;
                                                                                                    do
                                                                                                    {
                                                                                                        ICollection<KeyValuePair<string, ConditionallySelectedString>> arg_568_0 = dictionary;
                                                                                                        KeyValuePair<string, ConditionallySelectedString> arg_55C_0 = default(KeyValuePair<string, ConditionallySelectedString>);
                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                        KeyValuePair<string, ConditionallySelectedString> arg_568_1 = TierXConfigurationSerializer.Read(arg_55C_0, protoReader);
                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                        arg_568_0.Add(arg_568_1);
                                                                                                    }
                                                                                                    while (protoReader.TryReadFieldHeader(num2));
                                                                                                    dictionary2 = ((dictionary2 == dictionary) ? null : dictionary);
                                                                                                    if (dictionary2 != null)
                                                                                                    {
                                                                                                        pinDetail.ConditionallySelectedStrings = dictionary2;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (pinDetail == null)
                                                                                                {
                                                                                                    PinDetail expr_493 = new PinDetail();
                                                                                                    ProtoReader.NoteObject(expr_493, protoReader);
                                                                                                    pinDetail = expr_493;
                                                                                                }
                                                                                                Dictionary<string, TextureDetail> dictionary3 = pinDetail.Textures;
                                                                                                Dictionary<string, TextureDetail> dictionary4 = dictionary3;
                                                                                                if (dictionary3 == null)
                                                                                                {
                                                                                                    dictionary3 = new Dictionary<string, TextureDetail>();
                                                                                                }
                                                                                                int num2 = protoReader.FieldNumber;
                                                                                                do
                                                                                                {
                                                                                                    ICollection<KeyValuePair<string, TextureDetail>> arg_4DB_0 = dictionary3;
                                                                                                    KeyValuePair<string, TextureDetail> arg_4CF_0 = default(KeyValuePair<string, TextureDetail>);
                                                                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                    KeyValuePair<string, TextureDetail> arg_4DB_1 = TierXConfigurationSerializer.Read(arg_4CF_0, protoReader);
                                                                                                    ProtoReader.EndSubItem(token, protoReader);
                                                                                                    arg_4DB_0.Add(arg_4DB_1);
                                                                                                }
                                                                                                while (protoReader.TryReadFieldHeader(num2));
                                                                                                dictionary4 = ((dictionary4 == dictionary3) ? null : dictionary3);
                                                                                                if (dictionary4 != null)
                                                                                                {
                                                                                                    pinDetail.Textures = dictionary4;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (pinDetail == null)
                                                                                            {
                                                                                                PinDetail expr_44C = new PinDetail();
                                                                                                ProtoReader.NoteObject(expr_44C, protoReader);
                                                                                                pinDetail = expr_44C;
                                                                                            }
                                                                                            Color arg_463_0 = pinDetail.Colour;
                                                                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                            Color arg_46F_0 = TierXConfigurationSerializer.Read(arg_463_0, protoReader);
                                                                                            ProtoReader.EndSubItem(token, protoReader);
                                                                                            Color colour = arg_46F_0;
                                                                                            pinDetail.Colour = colour;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (pinDetail == null)
                                                                                        {
                                                                                            PinDetail expr_401 = new PinDetail();
                                                                                            ProtoReader.NoteObject(expr_401, protoReader);
                                                                                            pinDetail = expr_401;
                                                                                        }
                                                                                        PushScreenAction arg_418_0 = pinDetail.PushScreenAction;
                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                        PushScreenAction arg_424_0 = TierXConfigurationSerializer.Read(arg_418_0, protoReader);
                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                        PushScreenAction pushScreenAction = arg_424_0;
                                                                                        if (pushScreenAction != null)
                                                                                        {
                                                                                            pinDetail.PushScreenAction = pushScreenAction;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (pinDetail == null)
                                                                                    {
                                                                                        PinDetail expr_3B6 = new PinDetail();
                                                                                        ProtoReader.NoteObject(expr_3B6, protoReader);
                                                                                        pinDetail = expr_3B6;
                                                                                    }
                                                                                    LoadTierAction arg_3CD_0 = pinDetail.ClickAction;
                                                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                    LoadTierAction arg_3D9_0 = TierXConfigurationSerializer.Read(arg_3CD_0, protoReader);
                                                                                    ProtoReader.EndSubItem(token, protoReader);
                                                                                    LoadTierAction loadTierAction = arg_3D9_0;
                                                                                    if (loadTierAction != null)
                                                                                    {
                                                                                        pinDetail.ClickAction = loadTierAction;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (pinDetail == null)
                                                                                {
                                                                                    PinDetail expr_36B = new PinDetail();
                                                                                    ProtoReader.NoteObject(expr_36B, protoReader);
                                                                                    pinDetail = expr_36B;
                                                                                }
                                                                                PinLock arg_382_0 = pinDetail.Lock;
                                                                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                PinLock arg_38E_0 = TierXConfigurationSerializer.Read(arg_382_0, protoReader);
                                                                                ProtoReader.EndSubItem(token, protoReader);
                                                                                PinLock pinLock = arg_38E_0;
                                                                                if (pinLock != null)
                                                                                {
                                                                                    pinDetail.Lock = pinLock;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (pinDetail == null)
                                                                            {
                                                                                PinDetail expr_338 = new PinDetail();
                                                                                ProtoReader.NoteObject(expr_338, protoReader);
                                                                                pinDetail = expr_338;
                                                                            }
                                                                            bool flag = protoReader.ReadBoolean();
                                                                            pinDetail.IsProgressPin = flag;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (pinDetail == null)
                                                                        {
                                                                            PinDetail expr_305 = new PinDetail();
                                                                            ProtoReader.NoteObject(expr_305, protoReader);
                                                                            pinDetail = expr_305;
                                                                        }
                                                                        bool flag = protoReader.ReadBoolean();
                                                                        pinDetail.IsSuperNitrous = flag;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (pinDetail == null)
                                                                    {
                                                                        PinDetail expr_2D2 = new PinDetail();
                                                                        ProtoReader.NoteObject(expr_2D2, protoReader);
                                                                        pinDetail = expr_2D2;
                                                                    }
                                                                    int num2 = protoReader.ReadInt32();
                                                                    pinDetail.GroupID = num2;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (pinDetail == null)
                                                                {
                                                                    PinDetail expr_29F = new PinDetail();
                                                                    ProtoReader.NoteObject(expr_29F, protoReader);
                                                                    pinDetail = expr_29F;
                                                                }
                                                                int num2 = protoReader.ReadInt32();
                                                                pinDetail.EventID = num2;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (pinDetail == null)
                                                            {
                                                                PinDetail expr_26C = new PinDetail();
                                                                ProtoReader.NoteObject(expr_26C, protoReader);
                                                                pinDetail = expr_26C;
                                                            }
                                                            bool flag = protoReader.ReadBoolean();
                                                            pinDetail.HideTextBox = flag;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (pinDetail == null)
                                                        {
                                                            PinDetail expr_239 = new PinDetail();
                                                            ProtoReader.NoteObject(expr_239, protoReader);
                                                            pinDetail = expr_239;
                                                        }
                                                        bool flag = protoReader.ReadBoolean();
                                                        pinDetail.IsSelectable = flag;
                                                    }
                                                }
                                                else
                                                {
                                                    if (pinDetail == null)
                                                    {
                                                        PinDetail expr_206 = new PinDetail();
                                                        ProtoReader.NoteObject(expr_206, protoReader);
                                                        pinDetail = expr_206;
                                                    }
                                                    bool flag = protoReader.ReadBoolean();
                                                    pinDetail.CanDisplayAsComplete = flag;
                                                }
                                            }
                                            else
                                            {
                                                if (pinDetail == null)
                                                {
                                                    PinDetail expr_1D3 = new PinDetail();
                                                    ProtoReader.NoteObject(expr_1D3, protoReader);
                                                    pinDetail = expr_1D3;
                                                }
                                                bool flag = protoReader.ReadBoolean();
                                                pinDetail.ShowSelectionArrow = flag;
                                            }
                                        }
                                        else
                                        {
                                            if (pinDetail == null)
                                            {
                                                PinDetail expr_1A0 = new PinDetail();
                                                ProtoReader.NoteObject(expr_1A0, protoReader);
                                                pinDetail = expr_1A0;
                                            }
                                            bool flag = protoReader.ReadBoolean();
                                            pinDetail.ProgressIndicator = flag;
                                        }
                                    }
                                    else
                                    {
                                        if (pinDetail == null)
                                        {
                                            PinDetail expr_16D = new PinDetail();
                                            ProtoReader.NoteObject(expr_16D, protoReader);
                                            pinDetail = expr_16D;
                                        }
                                        string text = protoReader.ReadString();
                                        if (text != null)
                                        {
                                            pinDetail.EventDescription = text;
                                        }
                                    }
                                }
                                else
                                {
                                    if (pinDetail == null)
                                    {
                                        PinDetail expr_13A = new PinDetail();
                                        ProtoReader.NoteObject(expr_13A, protoReader);
                                        pinDetail = expr_13A;
                                    }
                                    string text = protoReader.ReadString();
                                    if (text != null)
                                    {
                                        pinDetail.Title = text;
                                    }
                                }
                            }
                            else
                            {
                                if (pinDetail == null)
                                {
                                    PinDetail expr_107 = new PinDetail();
                                    ProtoReader.NoteObject(expr_107, protoReader);
                                    pinDetail = expr_107;
                                }
                                string text = protoReader.ReadString();
                                if (text != null)
                                {
                                    pinDetail.LoadingScreen = text;
                                }
                            }
                        }
                        else
                        {
                            if (pinDetail == null)
                            {
                                PinDetail expr_C3 = new PinDetail();
                                ProtoReader.NoteObject(expr_C3, protoReader);
                                pinDetail = expr_C3;
                            }
                            Vector2 arg_DA_0 = pinDetail.PositionOffset;
                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                            Vector2 arg_E6_0 = TierXConfigurationSerializer.Read(arg_DA_0, protoReader);
                            ProtoReader.EndSubItem(token, protoReader);
                            Vector2 vector = arg_E6_0;
                            pinDetail.PositionOffset = vector;
                        }
                    }
                    else
                    {
                        if (pinDetail == null)
                        {
                            PinDetail expr_7F = new PinDetail();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            pinDetail = expr_7F;
                        }
                        Vector2 arg_96_0 = pinDetail.Position;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        Vector2 arg_A2_0 = TierXConfigurationSerializer.Read(arg_96_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        Vector2 vector = arg_A2_0;
                        pinDetail.Position = vector;
                    }
                }
                else
                {
                    if (pinDetail == null)
                    {
                        PinDetail expr_4C = new PinDetail();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        pinDetail = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        pinDetail.TemplateName = text;
                    }
                }
            }
            else
            {
                if (pinDetail == null)
                {
                    PinDetail expr_19 = new PinDetail();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinDetail = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pinDetail.PinID = text;
                }
            }
        }
        if (pinDetail == null)
        {
            PinDetail expr_632 = new PinDetail();
            ProtoReader.NoteObject(expr_632, protoReader);
            pinDetail = expr_632;
        }
        return pinDetail;
    }

    private static void Write(PinDetail.PinType pinType, ProtoWriter writer)
    {
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        if (pinType != PinDetail.PinType.NORMAL)
        {
            if (pinType != PinDetail.PinType.DAILYBATTLE)
            {
                if (pinType != PinDetail.PinType.MECHANIC)
                {
                    if (pinType != PinDetail.PinType.PIZZAPIN)
                    {
                        if (pinType != PinDetail.PinType.MULTIPLAYERPIN)
                        {
                            if (pinType != PinDetail.PinType.WORKSHOPPIN)
                            {
                                ProtoWriter.ThrowEnumException(writer, pinType);
                            }
                            else
                            {
                                ProtoWriter.WriteInt32(5, writer);
                            }
                        }
                        else
                        {
                            ProtoWriter.WriteInt32(4, writer);
                        }
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(3, writer);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(2, writer);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(1, writer);
            }
        }
        else
        {
            ProtoWriter.WriteInt32(0, writer);
        }
    }

    private static PinDetail.PinType Read(PinDetail.PinType pinType, ProtoReader protoReader)
    {
        int num = protoReader.ReadInt32();
        PinDetail.PinType result = PinDetail.PinType.NORMAL;
        if (num != 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                protoReader.ThrowEnumException(typeof(PinDetail.PinType), num);
                            }
                            else
                            {
                                result = PinDetail.PinType.WORKSHOPPIN;
                            }
                        }
                        else
                        {
                            result = PinDetail.PinType.MULTIPLAYERPIN;
                        }
                    }
                    else
                    {
                        result = PinDetail.PinType.PIZZAPIN;
                    }
                }
                else
                {
                    result = PinDetail.PinType.MECHANIC;
                }
            }
            else
            {
                result = PinDetail.PinType.DAILYBATTLE;
            }
        }
        else
        {
            result = PinDetail.PinType.NORMAL;
        }
        return result;
    }

    private static void Write(PinDetail.TextureKeys textureKeys, ProtoWriter writer)
    {
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        if (textureKeys != PinDetail.TextureKeys.CarRender)
        {
            if (textureKeys != PinDetail.TextureKeys.CarRenderElite)
            {
                if (textureKeys != PinDetail.TextureKeys.EventPaneOverlay)
                {
                    if (textureKeys != PinDetail.TextureKeys.EventPaneBackground)
                    {
                        if (textureKeys != PinDetail.TextureKeys.EventPaneBoss)
                        {
                            if (textureKeys != PinDetail.TextureKeys.PinOverlay)
                            {
                                if (textureKeys != PinDetail.TextureKeys.PinBoss)
                                {
                                    if (textureKeys != PinDetail.TextureKeys.PinBackground)
                                    {
                                        if (textureKeys != PinDetail.TextureKeys.PinBranding)
                                        {
                                            if (textureKeys != PinDetail.TextureKeys.Cross)
                                            {
                                                ProtoWriter.ThrowEnumException(writer, textureKeys);
                                            }
                                            else
                                            {
                                                ProtoWriter.WriteInt32(9, writer);
                                            }
                                        }
                                        else
                                        {
                                            ProtoWriter.WriteInt32(8, writer);
                                        }
                                    }
                                    else
                                    {
                                        ProtoWriter.WriteInt32(7, writer);
                                    }
                                }
                                else
                                {
                                    ProtoWriter.WriteInt32(6, writer);
                                }
                            }
                            else
                            {
                                ProtoWriter.WriteInt32(5, writer);
                            }
                        }
                        else
                        {
                            ProtoWriter.WriteInt32(4, writer);
                        }
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(3, writer);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(2, writer);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(1, writer);
            }
        }
        else
        {
            ProtoWriter.WriteInt32(0, writer);
        }
    }

    private static PinDetail.TextureKeys Read(PinDetail.TextureKeys textureKeys, ProtoReader protoReader)
    {
        int num = protoReader.ReadInt32();
        PinDetail.TextureKeys result = PinDetail.TextureKeys.CarRender;
        if (num != 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                protoReader.ThrowEnumException(typeof(PinDetail.TextureKeys), num);
                                            }
                                            else
                                            {
                                                result = PinDetail.TextureKeys.Cross;
                                            }
                                        }
                                        else
                                        {
                                            result = PinDetail.TextureKeys.PinBranding;
                                        }
                                    }
                                    else
                                    {
                                        result = PinDetail.TextureKeys.PinBackground;
                                    }
                                }
                                else
                                {
                                    result = PinDetail.TextureKeys.PinBoss;
                                }
                            }
                            else
                            {
                                result = PinDetail.TextureKeys.PinOverlay;
                            }
                        }
                        else
                        {
                            result = PinDetail.TextureKeys.EventPaneBoss;
                        }
                    }
                    else
                    {
                        result = PinDetail.TextureKeys.EventPaneBackground;
                    }
                }
                else
                {
                    result = PinDetail.TextureKeys.EventPaneOverlay;
                }
            }
            else
            {
                result = PinDetail.TextureKeys.CarRenderElite;
            }
        }
        else
        {
            result = PinDetail.TextureKeys.CarRender;
        }
        return result;
    }

    private static void Write(PinDetail.TimelineDirection timelineDirection, ProtoWriter writer)
    {
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        if (timelineDirection != PinDetail.TimelineDirection.None)
        {
            if (timelineDirection != PinDetail.TimelineDirection.Previous)
            {
                if (timelineDirection != PinDetail.TimelineDirection.Next)
                {
                    ProtoWriter.ThrowEnumException(writer, timelineDirection);
                }
                else
                {
                    ProtoWriter.WriteInt32(2, writer);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(1, writer);
            }
        }
        else
        {
            ProtoWriter.WriteInt32(0, writer);
        }
    }

    private static PinDetail.TimelineDirection Read(PinDetail.TimelineDirection timelineDirection, ProtoReader protoReader)
    {
        int num = protoReader.ReadInt32();
        PinDetail.TimelineDirection result = PinDetail.TimelineDirection.None;
        if (num != 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    protoReader.ThrowEnumException(typeof(PinDetail.TimelineDirection), num);
                }
                else
                {
                    result = PinDetail.TimelineDirection.Next;
                }
            }
            else
            {
                result = PinDetail.TimelineDirection.Previous;
            }
        }
        else
        {
            result = PinDetail.TimelineDirection.None;
        }
        return result;
    }

    private static void Write(PinLock pinLock, ProtoWriter protoWriter)
    {
        if (pinLock.GetType() != typeof(PinLock))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinLock), pinLock.GetType());
        }
        string expr_2D = pinLock.Type;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        PinLockDetails expr_4A = pinLock.Details;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_4A, protoWriter);
            TierXConfigurationSerializer.Write(expr_4A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static PinLock Read(PinLock pinLock, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (pinLock == null)
                    {
                        PinLock expr_8A = new PinLock();
                        ProtoReader.NoteObject(expr_8A, protoReader);
                        pinLock = expr_8A;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (pinLock == null)
                    {
                        PinLock expr_4C = new PinLock();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        pinLock = expr_4C;
                    }
                    PinLockDetails arg_63_0 = pinLock.Details;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    PinLockDetails arg_6F_0 = TierXConfigurationSerializer.Read(arg_63_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    PinLockDetails pinLockDetails = arg_6F_0;
                    if (pinLockDetails != null)
                    {
                        pinLock.Details = pinLockDetails;
                    }
                }
            }
            else
            {
                if (pinLock == null)
                {
                    PinLock expr_19 = new PinLock();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinLock = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pinLock.Type = text;
                }
            }
        }
        if (pinLock == null)
        {
            PinLock expr_B2 = new PinLock();
            ProtoReader.NoteObject(expr_B2, protoReader);
            pinLock = expr_B2;
        }
        return pinLock;
    }

    private static void Write(PinLockDetails pinLockDetails, ProtoWriter writer)
    {
        if (pinLockDetails.GetType() != typeof(PinLockDetails))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinLockDetails), pinLockDetails.GetType());
        }
        int arg_36_0 = pinLockDetails.IntValue;
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        ProtoWriter.WriteInt32(arg_36_0, writer);
    }

    private static PinLockDetails Read(PinLockDetails pinLockDetails, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (pinLockDetails == null)
                {
                    PinLockDetails expr_40 = new PinLockDetails();
                    ProtoReader.NoteObject(expr_40, protoReader);
                    pinLockDetails = expr_40;
                }
                protoReader.SkipField();
            }
            else
            {
                if (pinLockDetails == null)
                {
                    PinLockDetails expr_19 = new PinLockDetails();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinLockDetails = expr_19;
                }
                int intValue = protoReader.ReadInt32();
                pinLockDetails.IntValue = intValue;
            }
        }
        if (pinLockDetails == null)
        {
            PinLockDetails expr_68 = new PinLockDetails();
            ProtoReader.NoteObject(expr_68, protoReader);
            pinLockDetails = expr_68;
        }
        return pinLockDetails;
    }

    private static void Write(PinScheduleConfiguration pinScheduleConfiguration, ProtoWriter protoWriter)
    {
        if (pinScheduleConfiguration.GetType() != typeof(PinScheduleConfiguration))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinScheduleConfiguration), pinScheduleConfiguration.GetType());
        }
        List<PinSequence> expr_2D = pinScheduleConfiguration.Sequences;
        if (expr_2D != null)
        {
            List<PinSequence> list = expr_2D;
            foreach (PinSequence arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static PinScheduleConfiguration Read(PinScheduleConfiguration pinScheduleConfiguration, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (pinScheduleConfiguration == null)
                {
                    PinScheduleConfiguration expr_87 = new PinScheduleConfiguration();
                    ProtoReader.NoteObject(expr_87, protoReader);
                    pinScheduleConfiguration = expr_87;
                }
                protoReader.SkipField();
            }
            else
            {
                if (pinScheduleConfiguration == null)
                {
                    PinScheduleConfiguration expr_19 = new PinScheduleConfiguration();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinScheduleConfiguration = expr_19;
                }
                List<PinSequence> list = pinScheduleConfiguration.Sequences;
                List<PinSequence> list2 = list;
                if (list == null)
                {
                    list = new List<PinSequence>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<PinSequence> arg_53_0 = list;
                    PinSequence arg_46_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    PinSequence arg_53_1 = TierXConfigurationSerializer.Read(arg_46_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_53_0.Add(arg_53_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    pinScheduleConfiguration.Sequences = list2;
                }
            }
        }
        if (pinScheduleConfiguration == null)
        {
            PinScheduleConfiguration expr_AF = new PinScheduleConfiguration();
            ProtoReader.NoteObject(expr_AF, protoReader);
            pinScheduleConfiguration = expr_AF;
        }
        return pinScheduleConfiguration;
    }

    private static void Write(PinSchedulerAIDriverOverrides pinSchedulerAIDriverOverrides, ProtoWriter writer)
    {
        if (pinSchedulerAIDriverOverrides.GetType() != typeof(PinSchedulerAIDriverOverrides))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinSchedulerAIDriverOverrides), pinSchedulerAIDriverOverrides.GetType());
        }
        string expr_2D = pinSchedulerAIDriverOverrides.Name;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        string expr_4A = pinSchedulerAIDriverOverrides.NumberPlateString;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
    }

    private static PinSchedulerAIDriverOverrides Read(PinSchedulerAIDriverOverrides pinSchedulerAIDriverOverrides, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (pinSchedulerAIDriverOverrides == null)
                    {
                        PinSchedulerAIDriverOverrides expr_76 = new PinSchedulerAIDriverOverrides();
                        ProtoReader.NoteObject(expr_76, protoReader);
                        pinSchedulerAIDriverOverrides = expr_76;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (pinSchedulerAIDriverOverrides == null)
                    {
                        PinSchedulerAIDriverOverrides expr_4C = new PinSchedulerAIDriverOverrides();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        pinSchedulerAIDriverOverrides = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        pinSchedulerAIDriverOverrides.NumberPlateString = text;
                    }
                }
            }
            else
            {
                if (pinSchedulerAIDriverOverrides == null)
                {
                    PinSchedulerAIDriverOverrides expr_19 = new PinSchedulerAIDriverOverrides();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinSchedulerAIDriverOverrides = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pinSchedulerAIDriverOverrides.Name = text;
                }
            }
        }
        if (pinSchedulerAIDriverOverrides == null)
        {
            PinSchedulerAIDriverOverrides expr_9E = new PinSchedulerAIDriverOverrides();
            ProtoReader.NoteObject(expr_9E, protoReader);
            pinSchedulerAIDriverOverrides = expr_9E;
        }
        return pinSchedulerAIDriverOverrides;
    }

    private static void Write(PinSequence pinSequence, ProtoWriter protoWriter)
    {
        if (pinSequence.GetType() != typeof(PinSequence))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinSequence), pinSequence.GetType());
        }
        string expr_2D = pinSequence.ID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = pinSequence.Type;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        EligibilityRequirements expr_67 = pinSequence.Requirements;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_67, protoWriter);
            TierXConfigurationSerializer.Write(expr_67, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        string expr_93 = pinSequence.RewardsMultipliersID;
        if (expr_93 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_93, protoWriter);
        }
        List<ScheduledPin> expr_B0 = pinSequence.Pins;
        if (expr_B0 != null)
        {
            List<ScheduledPin> list = expr_B0;
            foreach (ScheduledPin arg_D5_0 in list)
            {
                ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_D5_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_D5_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        bool arg_11D_0 = pinSequence.Repeatable;
        ProtoWriter.WriteFieldHeader(6, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_11D_0, protoWriter);
        bool arg_131_0 = pinSequence.AllowRestarts;
        ProtoWriter.WriteFieldHeader(7, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_131_0, protoWriter);
        PinSequenceTimelineData expr_13C = pinSequence.TimelineData;
        if (expr_13C != null)
        {
            ProtoWriter.WriteFieldHeader(8, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_13C, protoWriter);
            TierXConfigurationSerializer.Write(expr_13C, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        PinSequence.eSequenceType expr_168 = pinSequence.TypeEnum;
        if (expr_168 != PinSequence.eSequenceType.Ladder)
        {
            ProtoWriter.WriteFieldHeader(9, WireType.Variant, protoWriter);
            PinSequence.eSequenceType eSequenceType = expr_168;
            if (eSequenceType != PinSequence.eSequenceType.Ladder)
            {
                if (eSequenceType != PinSequence.eSequenceType.Cycle)
                {
                    if (eSequenceType != PinSequence.eSequenceType.CycleNextValidAfterWin)
                    {
                        if (eSequenceType != PinSequence.eSequenceType.CycleNextValidAfterWinOrLose)
                        {
                            if (eSequenceType != PinSequence.eSequenceType.CycleNextValidNotRaced)
                            {
                                if (eSequenceType != PinSequence.eSequenceType.NextRandomValidAfterWin)
                                {
                                    if (eSequenceType != PinSequence.eSequenceType.NextRandomValidAfterWinOrLose)
                                    {
                                        if (eSequenceType != PinSequence.eSequenceType.Choice)
                                        {
                                            ProtoWriter.ThrowEnumException(protoWriter, eSequenceType);
                                        }
                                        else
                                        {
                                            ProtoWriter.WriteInt32(7, protoWriter);
                                        }
                                    }
                                    else
                                    {
                                        ProtoWriter.WriteInt32(6, protoWriter);
                                    }
                                }
                                else
                                {
                                    ProtoWriter.WriteInt32(5, protoWriter);
                                }
                            }
                            else
                            {
                                ProtoWriter.WriteInt32(4, protoWriter);
                            }
                        }
                        else
                        {
                            ProtoWriter.WriteInt32(3, protoWriter);
                        }
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(2, protoWriter);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(1, protoWriter);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(0, protoWriter);
            }
        }
    }

    private static PinSequence Read(PinSequence pinSequence, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (pinSequence == null)
                                                {
                                                    PinSequence expr_2FA = new PinSequence();
                                                    ProtoReader.NoteObject(expr_2FA, protoReader);
                                                    pinSequence = expr_2FA;
                                                }
                                                protoReader.SkipField();
                                            }
                                            else
                                            {
                                                if (pinSequence == null)
                                                {
                                                    PinSequence expr_22B = new PinSequence();
                                                    ProtoReader.NoteObject(expr_22B, protoReader);
                                                    pinSequence = expr_22B;
                                                }
                                                int num2 = protoReader.ReadInt32();
                                                PinSequence.eSequenceType eSequenceType = PinSequence.eSequenceType.Ladder;
                                                if (num2 != 0)
                                                {
                                                    if (num2 != 1)
                                                    {
                                                        if (num2 != 2)
                                                        {
                                                            if (num2 != 3)
                                                            {
                                                                if (num2 != 4)
                                                                {
                                                                    if (num2 != 5)
                                                                    {
                                                                        if (num2 != 6)
                                                                        {
                                                                            if (num2 != 7)
                                                                            {
                                                                                protoReader.ThrowEnumException(typeof(PinSequence.eSequenceType), num2);
                                                                            }
                                                                            else
                                                                            {
                                                                                eSequenceType = PinSequence.eSequenceType.Choice;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            eSequenceType = PinSequence.eSequenceType.NextRandomValidAfterWinOrLose;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        eSequenceType = PinSequence.eSequenceType.NextRandomValidAfterWin;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    eSequenceType = PinSequence.eSequenceType.CycleNextValidNotRaced;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                eSequenceType = PinSequence.eSequenceType.CycleNextValidAfterWinOrLose;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            eSequenceType = PinSequence.eSequenceType.CycleNextValidAfterWin;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        eSequenceType = PinSequence.eSequenceType.Cycle;
                                                    }
                                                }
                                                else
                                                {
                                                    eSequenceType = PinSequence.eSequenceType.Ladder;
                                                }
                                                eSequenceType = eSequenceType;
                                                pinSequence.TypeEnum = eSequenceType;
                                            }
                                        }
                                        else
                                        {
                                            if (pinSequence == null)
                                            {
                                                PinSequence expr_1E0 = new PinSequence();
                                                ProtoReader.NoteObject(expr_1E0, protoReader);
                                                pinSequence = expr_1E0;
                                            }
                                            PinSequenceTimelineData arg_1F7_0 = pinSequence.TimelineData;
                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                            PinSequenceTimelineData arg_203_0 = TierXConfigurationSerializer.Read(arg_1F7_0, protoReader);
                                            ProtoReader.EndSubItem(token, protoReader);
                                            PinSequenceTimelineData pinSequenceTimelineData = arg_203_0;
                                            if (pinSequenceTimelineData != null)
                                            {
                                                pinSequence.TimelineData = pinSequenceTimelineData;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (pinSequence == null)
                                        {
                                            PinSequence expr_1AE = new PinSequence();
                                            ProtoReader.NoteObject(expr_1AE, protoReader);
                                            pinSequence = expr_1AE;
                                        }
                                        bool flag = protoReader.ReadBoolean();
                                        pinSequence.AllowRestarts = flag;
                                    }
                                }
                                else
                                {
                                    if (pinSequence == null)
                                    {
                                        PinSequence expr_17C = new PinSequence();
                                        ProtoReader.NoteObject(expr_17C, protoReader);
                                        pinSequence = expr_17C;
                                    }
                                    bool flag = protoReader.ReadBoolean();
                                    pinSequence.Repeatable = flag;
                                }
                            }
                            else
                            {
                                if (pinSequence == null)
                                {
                                    PinSequence expr_F9 = new PinSequence();
                                    ProtoReader.NoteObject(expr_F9, protoReader);
                                    pinSequence = expr_F9;
                                }
                                List<ScheduledPin> list = pinSequence.Pins;
                                List<ScheduledPin> list2 = list;
                                if (list == null)
                                {
                                    list = new List<ScheduledPin>();
                                }
                                int num2 = protoReader.FieldNumber;
                                do
                                {
                                    List<ScheduledPin> arg_138_0 = list;
                                    ScheduledPin arg_12C_0 = null;
                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                    ScheduledPin arg_138_1 = TierXConfigurationSerializer.Read(arg_12C_0, protoReader);
                                    ProtoReader.EndSubItem(token, protoReader);
                                    arg_138_0.Add(arg_138_1);
                                }
                                while (protoReader.TryReadFieldHeader(num2));
                                list2 = ((list2 == list) ? null : list);
                                if (list2 != null)
                                {
                                    pinSequence.Pins = list2;
                                }
                            }
                        }
                        else
                        {
                            if (pinSequence == null)
                            {
                                PinSequence expr_C6 = new PinSequence();
                                ProtoReader.NoteObject(expr_C6, protoReader);
                                pinSequence = expr_C6;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                pinSequence.RewardsMultipliersID = text;
                            }
                        }
                    }
                    else
                    {
                        if (pinSequence == null)
                        {
                            PinSequence expr_7F = new PinSequence();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            pinSequence = expr_7F;
                        }
                        EligibilityRequirements arg_96_0 = pinSequence.Requirements;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        EligibilityRequirements arg_A2_0 = TierXConfigurationSerializer.Read(arg_96_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        EligibilityRequirements eligibilityRequirements = arg_A2_0;
                        if (eligibilityRequirements != null)
                        {
                            pinSequence.Requirements = eligibilityRequirements;
                        }
                    }
                }
                else
                {
                    if (pinSequence == null)
                    {
                        PinSequence expr_4C = new PinSequence();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        pinSequence = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        pinSequence.Type = text;
                    }
                }
            }
            else
            {
                if (pinSequence == null)
                {
                    PinSequence expr_19 = new PinSequence();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinSequence = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pinSequence.ID = text;
                }
            }
        }
        if (pinSequence == null)
        {
            PinSequence expr_322 = new PinSequence();
            ProtoReader.NoteObject(expr_322, protoReader);
            pinSequence = expr_322;
        }
        return pinSequence;
    }

    private static void Write(PinSequence.eSequenceType eSequenceType, ProtoWriter writer)
    {
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        if (eSequenceType != PinSequence.eSequenceType.Ladder)
        {
            if (eSequenceType != PinSequence.eSequenceType.Cycle)
            {
                if (eSequenceType != PinSequence.eSequenceType.CycleNextValidAfterWin)
                {
                    if (eSequenceType != PinSequence.eSequenceType.CycleNextValidAfterWinOrLose)
                    {
                        if (eSequenceType != PinSequence.eSequenceType.CycleNextValidNotRaced)
                        {
                            if (eSequenceType != PinSequence.eSequenceType.NextRandomValidAfterWin)
                            {
                                if (eSequenceType != PinSequence.eSequenceType.NextRandomValidAfterWinOrLose)
                                {
                                    if (eSequenceType != PinSequence.eSequenceType.Choice)
                                    {
                                        ProtoWriter.ThrowEnumException(writer, eSequenceType);
                                    }
                                    else
                                    {
                                        ProtoWriter.WriteInt32(7, writer);
                                    }
                                }
                                else
                                {
                                    ProtoWriter.WriteInt32(6, writer);
                                }
                            }
                            else
                            {
                                ProtoWriter.WriteInt32(5, writer);
                            }
                        }
                        else
                        {
                            ProtoWriter.WriteInt32(4, writer);
                        }
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(3, writer);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(2, writer);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(1, writer);
            }
        }
        else
        {
            ProtoWriter.WriteInt32(0, writer);
        }
    }

    private static PinSequence.eSequenceType Read(PinSequence.eSequenceType eSequenceType, ProtoReader protoReader)
    {
        int num = protoReader.ReadInt32();
        PinSequence.eSequenceType result = PinSequence.eSequenceType.Ladder;
        if (num != 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        protoReader.ThrowEnumException(typeof(PinSequence.eSequenceType), num);
                                    }
                                    else
                                    {
                                        result = PinSequence.eSequenceType.Choice;
                                    }
                                }
                                else
                                {
                                    result = PinSequence.eSequenceType.NextRandomValidAfterWinOrLose;
                                }
                            }
                            else
                            {
                                result = PinSequence.eSequenceType.NextRandomValidAfterWin;
                            }
                        }
                        else
                        {
                            result = PinSequence.eSequenceType.CycleNextValidNotRaced;
                        }
                    }
                    else
                    {
                        result = PinSequence.eSequenceType.CycleNextValidAfterWinOrLose;
                    }
                }
                else
                {
                    result = PinSequence.eSequenceType.CycleNextValidAfterWin;
                }
            }
            else
            {
                result = PinSequence.eSequenceType.Cycle;
            }
        }
        else
        {
            result = PinSequence.eSequenceType.Ladder;
        }
        return result;
    }

    private static void Write(PinSequenceTimelineData pinSequenceTimelineData, ProtoWriter writer)
    {
        if (pinSequenceTimelineData.GetType() != typeof(PinSequenceTimelineData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinSequenceTimelineData), pinSequenceTimelineData.GetType());
        }
        bool arg_36_0 = pinSequenceTimelineData.ShowTimeline;
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        ProtoWriter.WriteBoolean(arg_36_0, writer);
        string expr_41 = pinSequenceTimelineData.PredecessorSequence;
        if (expr_41 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_41, writer);
        }
        string expr_5E = pinSequenceTimelineData.SuccessorSequence;
        if (expr_5E != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, writer);
            ProtoWriter.WriteString(expr_5E, writer);
        }
    }

    private static PinSequenceTimelineData Read(PinSequenceTimelineData pinSequenceTimelineData, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (pinSequenceTimelineData == null)
                        {
                            PinSequenceTimelineData expr_A6 = new PinSequenceTimelineData();
                            ProtoReader.NoteObject(expr_A6, protoReader);
                            pinSequenceTimelineData = expr_A6;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (pinSequenceTimelineData == null)
                        {
                            PinSequenceTimelineData expr_7C = new PinSequenceTimelineData();
                            ProtoReader.NoteObject(expr_7C, protoReader);
                            pinSequenceTimelineData = expr_7C;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            pinSequenceTimelineData.SuccessorSequence = text;
                        }
                    }
                }
                else
                {
                    if (pinSequenceTimelineData == null)
                    {
                        PinSequenceTimelineData expr_49 = new PinSequenceTimelineData();
                        ProtoReader.NoteObject(expr_49, protoReader);
                        pinSequenceTimelineData = expr_49;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        pinSequenceTimelineData.PredecessorSequence = text;
                    }
                }
            }
            else
            {
                if (pinSequenceTimelineData == null)
                {
                    PinSequenceTimelineData expr_19 = new PinSequenceTimelineData();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinSequenceTimelineData = expr_19;
                }
                bool showTimeline = protoReader.ReadBoolean();
                pinSequenceTimelineData.ShowTimeline = showTimeline;
            }
        }
        if (pinSequenceTimelineData == null)
        {
            PinSequenceTimelineData expr_CE = new PinSequenceTimelineData();
            ProtoReader.NoteObject(expr_CE, protoReader);
            pinSequenceTimelineData = expr_CE;
        }
        return pinSequenceTimelineData;
    }

    private static void Write(PinTemplate pinTemplate, ProtoWriter protoWriter)
    {
        if (pinTemplate.GetType() != typeof(PinTemplate))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PinTemplate), pinTemplate.GetType());
        }
        string expr_2D = pinTemplate.TemplateName;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        Vector2 arg_5B_0 = pinTemplate.Position;
        ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_5B_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        Vector2 arg_7E_0 = pinTemplate.PositionOffset;
        ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_7E_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        string expr_90 = pinTemplate.LoadingScreen;
        if (expr_90 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_90, protoWriter);
        }
        bool arg_B6_0 = pinTemplate.ProgressIndicator;
        ProtoWriter.WriteFieldHeader(5, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_B6_0, protoWriter);
        bool arg_CA_0 = pinTemplate.ShowSelectionArrow;
        ProtoWriter.WriteFieldHeader(6, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_CA_0, protoWriter);
        Dictionary<string, TextureDetail> expr_D5 = pinTemplate.Textures;
        if (expr_D5 != null)
        {
            Dictionary<string, TextureDetail> dictionary = expr_D5;
            foreach (KeyValuePair<string, TextureDetail> arg_103_0 in dictionary)
            {
                ProtoWriter.WriteFieldHeader(7, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(null, protoWriter);
                TierXConfigurationSerializer.Write(arg_103_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static PinTemplate Read(PinTemplate pinTemplate, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (pinTemplate == null)
                                        {
                                            PinTemplate expr_1EE = new PinTemplate();
                                            ProtoReader.NoteObject(expr_1EE, protoReader);
                                            pinTemplate = expr_1EE;
                                        }
                                        protoReader.SkipField();
                                    }
                                    else
                                    {
                                        if (pinTemplate == null)
                                        {
                                            PinTemplate expr_16B = new PinTemplate();
                                            ProtoReader.NoteObject(expr_16B, protoReader);
                                            pinTemplate = expr_16B;
                                        }
                                        Dictionary<string, TextureDetail> dictionary = pinTemplate.Textures;
                                        Dictionary<string, TextureDetail> dictionary2 = dictionary;
                                        if (dictionary == null)
                                        {
                                            dictionary = new Dictionary<string, TextureDetail>();
                                        }
                                        int fieldNumber = protoReader.FieldNumber;
                                        do
                                        {
                                            ICollection<KeyValuePair<string, TextureDetail>> arg_1B3_0 = dictionary;
                                            KeyValuePair<string, TextureDetail> arg_1A7_0 = default(KeyValuePair<string, TextureDetail>);
                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                            KeyValuePair<string, TextureDetail> arg_1B3_1 = TierXConfigurationSerializer.Read(arg_1A7_0, protoReader);
                                            ProtoReader.EndSubItem(token, protoReader);
                                            arg_1B3_0.Add(arg_1B3_1);
                                        }
                                        while (protoReader.TryReadFieldHeader(fieldNumber));
                                        dictionary2 = ((dictionary2 == dictionary) ? null : dictionary);
                                        if (dictionary2 != null)
                                        {
                                            pinTemplate.Textures = dictionary2;
                                        }
                                    }
                                }
                                else
                                {
                                    if (pinTemplate == null)
                                    {
                                        PinTemplate expr_139 = new PinTemplate();
                                        ProtoReader.NoteObject(expr_139, protoReader);
                                        pinTemplate = expr_139;
                                    }
                                    bool flag = protoReader.ReadBoolean();
                                    pinTemplate.ShowSelectionArrow = flag;
                                }
                            }
                            else
                            {
                                if (pinTemplate == null)
                                {
                                    PinTemplate expr_107 = new PinTemplate();
                                    ProtoReader.NoteObject(expr_107, protoReader);
                                    pinTemplate = expr_107;
                                }
                                bool flag = protoReader.ReadBoolean();
                                pinTemplate.ProgressIndicator = flag;
                            }
                        }
                        else
                        {
                            if (pinTemplate == null)
                            {
                                PinTemplate expr_D4 = new PinTemplate();
                                ProtoReader.NoteObject(expr_D4, protoReader);
                                pinTemplate = expr_D4;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                pinTemplate.LoadingScreen = text;
                            }
                        }
                    }
                    else
                    {
                        if (pinTemplate == null)
                        {
                            PinTemplate expr_90 = new PinTemplate();
                            ProtoReader.NoteObject(expr_90, protoReader);
                            pinTemplate = expr_90;
                        }
                        Vector2 arg_A7_0 = pinTemplate.PositionOffset;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        Vector2 arg_B3_0 = TierXConfigurationSerializer.Read(arg_A7_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        Vector2 vector = arg_B3_0;
                        pinTemplate.PositionOffset = vector;
                    }
                }
                else
                {
                    if (pinTemplate == null)
                    {
                        PinTemplate expr_4C = new PinTemplate();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        pinTemplate = expr_4C;
                    }
                    Vector2 arg_63_0 = pinTemplate.Position;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    Vector2 arg_6F_0 = TierXConfigurationSerializer.Read(arg_63_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    Vector2 vector = arg_6F_0;
                    pinTemplate.Position = vector;
                }
            }
            else
            {
                if (pinTemplate == null)
                {
                    PinTemplate expr_19 = new PinTemplate();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pinTemplate = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pinTemplate.TemplateName = text;
                }
            }
        }
        if (pinTemplate == null)
        {
            PinTemplate expr_216 = new PinTemplate();
            ProtoReader.NoteObject(expr_216, protoReader);
            pinTemplate = expr_216;
        }
        return pinTemplate;
    }

    private static void Write(PopUpConfiguration popUpConfiguration, ProtoWriter protoWriter)
    {
        if (popUpConfiguration.GetType() != typeof(PopUpConfiguration))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PopUpConfiguration), popUpConfiguration.GetType());
        }
        Dictionary<string, PopupData> expr_2D = popUpConfiguration.PopUpLookup;
        if (expr_2D != null)
        {
            Dictionary<string, PopupData> dictionary = expr_2D;
            foreach (KeyValuePair<string, PopupData> arg_5B_0 in dictionary)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
                TierXConfigurationSerializer.Write(arg_5B_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static PopUpConfiguration Read(PopUpConfiguration popUpConfiguration, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (popUpConfiguration == null)
                {
                    PopUpConfiguration expr_90 = new PopUpConfiguration();
                    ProtoReader.NoteObject(expr_90, protoReader);
                    popUpConfiguration = expr_90;
                }
                protoReader.SkipField();
            }
            else
            {
                if (popUpConfiguration == null)
                {
                    PopUpConfiguration expr_19 = new PopUpConfiguration();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    popUpConfiguration = expr_19;
                }
                Dictionary<string, PopupData> dictionary = popUpConfiguration.PopUpLookup;
                Dictionary<string, PopupData> dictionary2 = dictionary;
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, PopupData>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    ICollection<KeyValuePair<string, PopupData>> arg_5C_0 = dictionary;
                    KeyValuePair<string, PopupData> arg_4F_0 = default(KeyValuePair<string, PopupData>);
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    KeyValuePair<string, PopupData> arg_5C_1 = TierXConfigurationSerializer.Read(arg_4F_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_5C_0.Add(arg_5C_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                dictionary2 = ((dictionary2 == dictionary) ? null : dictionary);
                if (dictionary2 != null)
                {
                    popUpConfiguration.PopUpLookup = dictionary2;
                }
            }
        }
        if (popUpConfiguration == null)
        {
            PopUpConfiguration expr_B8 = new PopUpConfiguration();
            ProtoReader.NoteObject(expr_B8, protoReader);
            popUpConfiguration = expr_B8;
        }
        return popUpConfiguration;
    }

    private static void Write(PopupData popupData, ProtoWriter protoWriter)
    {
        if (popupData.GetType() != typeof(PopupData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PopupData), popupData.GetType());
        }
        string expr_2D = popupData.CharacterName;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = popupData.CharacterTexture;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        string expr_67 = popupData.TitleText;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_67, protoWriter);
        }
        List<FormatStringData> expr_84 = popupData.BodyText;
        if (expr_84 != null)
        {
            List<FormatStringData> list = expr_84;
            foreach (FormatStringData arg_A9_0 in list)
            {
                ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_A9_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_A9_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        string expr_E8 = popupData.StarterPackItem1;
        if (expr_E8 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_E8, protoWriter);
        }
        string expr_105 = popupData.StarterPackItem2;
        if (expr_105 != null)
        {
            ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_105, protoWriter);
        }
        string expr_122 = popupData.StarterPackOfferItem;
        if (expr_122 != null)
        {
            ProtoWriter.WriteFieldHeader(7, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_122, protoWriter);
        }
        TimeSpan expr_13F = popupData.StarterPackValidityDuration;
        if (!(expr_13F == TimeSpan.Zero))
        {
            ProtoWriter.WriteFieldHeader(8, WireType.String, protoWriter);
            BclHelpers.WriteTimeSpan(expr_13F, protoWriter);
        }
        string expr_166 = popupData.ConfirmButtonText;
        if (expr_166 != null)
        {
            ProtoWriter.WriteFieldHeader(9, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_166, protoWriter);
        }
        EligibilityRequirements expr_184 = popupData.OkButtonRequirement;
        if (expr_184 != null)
        {
            ProtoWriter.WriteFieldHeader(10, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_184, protoWriter);
            TierXConfigurationSerializer.Write(expr_184, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        string expr_1B1 = popupData.CancelButtonText;
        if (expr_1B1 != null)
        {
            ProtoWriter.WriteFieldHeader(11, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1B1, protoWriter);
        }
        EligibilityRequirements expr_1CF = popupData.CancelButtonRequirement;
        if (expr_1CF != null)
        {
            ProtoWriter.WriteFieldHeader(12, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_1CF, protoWriter);
            TierXConfigurationSerializer.Write(expr_1CF, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        bool arg_206_0 = popupData.SetupForCrewLeader;
        ProtoWriter.WriteFieldHeader(13, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_206_0, protoWriter);
        bool arg_21B_0 = popupData.CheckForShowOnlyOnce;
        ProtoWriter.WriteFieldHeader(14, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_21B_0, protoWriter);
        bool arg_230_0 = popupData.IsBodyTranslated;
        ProtoWriter.WriteFieldHeader(15, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_230_0, protoWriter);
        EligibilityRequirements expr_23B = popupData.PopupRequirements;
        if (expr_23B != null)
        {
            ProtoWriter.WriteFieldHeader(16, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_23B, protoWriter);
            TierXConfigurationSerializer.Write(expr_23B, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        List<PopupDataButtonAction> expr_268 = popupData.ConfirmActions;
        if (expr_268 != null)
        {
            List<PopupDataButtonAction> list2 = expr_268;
            foreach (PopupDataButtonAction arg_28F_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(17, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_28F_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_28F_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        List<PopupDataButtonAction> expr_2CE = popupData.CancelActions;
        if (expr_2CE != null)
        {
            List<PopupDataButtonAction> list2 = expr_2CE;
            foreach (PopupDataButtonAction arg_2F5_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(18, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_2F5_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_2F5_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        bool arg_33E_0 = popupData.HasBeenShown;
        ProtoWriter.WriteFieldHeader(20, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_33E_0, protoWriter);

        bool isBig = popupData.IsBig;
        ProtoWriter.WriteFieldHeader(21, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(isBig, protoWriter);
    }

    private static PopupData Read(PopupData popupData, ProtoReader protoReader)
    {
        if (popupData != null)
        {
            popupData.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (num != 10)
                                                {
                                                    if (num != 11)
                                                    {
                                                        if (num != 12)
                                                        {
                                                            if (num != 13)
                                                            {
                                                                if (num != 14)
                                                                {
                                                                    if (num != 15)
                                                                    {
                                                                        if (num != 16)
                                                                        {
                                                                            if (num != 17)
                                                                            {
                                                                                if (num != 18)
                                                                                {
                                                                                    if (num != 20)
                                                                                    {
                                                                                        if (num != 21)
                                                                                        {
                                                                                            if (popupData == null)
                                                                                            {
                                                                                                PopupData expr_592 = new PopupData();
                                                                                                ProtoReader.NoteObject(expr_592, protoReader);
                                                                                                expr_592.Init();
                                                                                                popupData = expr_592;
                                                                                            }
                                                                                            protoReader.SkipField();
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (popupData == null)
                                                                                            {
                                                                                                PopupData expr_563 = new PopupData();
                                                                                                ProtoReader.NoteObject(expr_563, protoReader);
                                                                                                expr_563.Init();
                                                                                                popupData = expr_563;
                                                                                            }
                                                                                            bool flag = protoReader.ReadBoolean();
                                                                                            popupData.IsBig = flag;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (popupData == null)
                                                                                        {
                                                                                            PopupData expr_563 = new PopupData();
                                                                                            ProtoReader.NoteObject(expr_563, protoReader);
                                                                                            expr_563.Init();
                                                                                            popupData = expr_563;
                                                                                        }
                                                                                        bool flag = protoReader.ReadBoolean();
                                                                                        popupData.HasBeenShown = flag;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (popupData == null)
                                                                                    {
                                                                                        PopupData expr_4D7 = new PopupData();
                                                                                        ProtoReader.NoteObject(expr_4D7, protoReader);
                                                                                        expr_4D7.Init();
                                                                                        popupData = expr_4D7;
                                                                                    }
                                                                                    List<PopupDataButtonAction> list = popupData.CancelActions;
                                                                                    List<PopupDataButtonAction> list2 = list;
                                                                                    if (list == null)
                                                                                    {
                                                                                        list = new List<PopupDataButtonAction>();
                                                                                    }
                                                                                    int fieldNumber = protoReader.FieldNumber;
                                                                                    do
                                                                                    {
                                                                                        List<PopupDataButtonAction> arg_51E_0 = list;
                                                                                        PopupDataButtonAction arg_511_0 = null;
                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                        PopupDataButtonAction arg_51E_1 = TierXConfigurationSerializer.Read(arg_511_0, protoReader);
                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                        arg_51E_0.Add(arg_51E_1);
                                                                                    }
                                                                                    while (protoReader.TryReadFieldHeader(fieldNumber));
                                                                                    list2 = ((list2 == list) ? null : list);
                                                                                    if (list2 != null)
                                                                                    {
                                                                                        popupData.CancelActions = list2;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (popupData == null)
                                                                                {
                                                                                    PopupData expr_44B = new PopupData();
                                                                                    ProtoReader.NoteObject(expr_44B, protoReader);
                                                                                    expr_44B.Init();
                                                                                    popupData = expr_44B;
                                                                                }
                                                                                List<PopupDataButtonAction> list2 = popupData.ConfirmActions;
                                                                                List<PopupDataButtonAction> list = list2;
                                                                                if (list2 == null)
                                                                                {
                                                                                    list2 = new List<PopupDataButtonAction>();
                                                                                }
                                                                                int fieldNumber = protoReader.FieldNumber;
                                                                                do
                                                                                {
                                                                                    List<PopupDataButtonAction> arg_492_0 = list2;
                                                                                    PopupDataButtonAction arg_485_0 = null;
                                                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                    PopupDataButtonAction arg_492_1 = TierXConfigurationSerializer.Read(arg_485_0, protoReader);
                                                                                    ProtoReader.EndSubItem(token, protoReader);
                                                                                    arg_492_0.Add(arg_492_1);
                                                                                }
                                                                                while (protoReader.TryReadFieldHeader(fieldNumber));
                                                                                list = ((list == list2) ? null : list2);
                                                                                if (list != null)
                                                                                {
                                                                                    popupData.ConfirmActions = list;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (popupData == null)
                                                                            {
                                                                                PopupData expr_3F8 = new PopupData();
                                                                                ProtoReader.NoteObject(expr_3F8, protoReader);
                                                                                expr_3F8.Init();
                                                                                popupData = expr_3F8;
                                                                            }
                                                                            EligibilityRequirements arg_416_0 = popupData.PopupRequirements;
                                                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                            EligibilityRequirements arg_423_0 = TierXConfigurationSerializer.Read(arg_416_0, protoReader);
                                                                            ProtoReader.EndSubItem(token, protoReader);
                                                                            EligibilityRequirements eligibilityRequirements = arg_423_0;
                                                                            if (eligibilityRequirements != null)
                                                                            {
                                                                                popupData.PopupRequirements = eligibilityRequirements;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (popupData == null)
                                                                        {
                                                                            PopupData expr_3BF = new PopupData();
                                                                            ProtoReader.NoteObject(expr_3BF, protoReader);
                                                                            expr_3BF.Init();
                                                                            popupData = expr_3BF;
                                                                        }
                                                                        bool flag = protoReader.ReadBoolean();
                                                                        popupData.IsBodyTranslated = flag;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (popupData == null)
                                                                    {
                                                                        PopupData expr_386 = new PopupData();
                                                                        ProtoReader.NoteObject(expr_386, protoReader);
                                                                        expr_386.Init();
                                                                        popupData = expr_386;
                                                                    }
                                                                    bool flag = protoReader.ReadBoolean();
                                                                    popupData.CheckForShowOnlyOnce = flag;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (popupData == null)
                                                                {
                                                                    PopupData expr_34D = new PopupData();
                                                                    ProtoReader.NoteObject(expr_34D, protoReader);
                                                                    expr_34D.Init();
                                                                    popupData = expr_34D;
                                                                }
                                                                bool flag = protoReader.ReadBoolean();
                                                                popupData.SetupForCrewLeader = flag;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (popupData == null)
                                                            {
                                                                PopupData expr_2FA = new PopupData();
                                                                ProtoReader.NoteObject(expr_2FA, protoReader);
                                                                expr_2FA.Init();
                                                                popupData = expr_2FA;
                                                            }
                                                            EligibilityRequirements arg_318_0 = popupData.CancelButtonRequirement;
                                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                            EligibilityRequirements arg_325_0 = TierXConfigurationSerializer.Read(arg_318_0, protoReader);
                                                            ProtoReader.EndSubItem(token, protoReader);
                                                            EligibilityRequirements eligibilityRequirements = arg_325_0;
                                                            if (eligibilityRequirements != null)
                                                            {
                                                                popupData.CancelButtonRequirement = eligibilityRequirements;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (popupData == null)
                                                        {
                                                            PopupData expr_2C0 = new PopupData();
                                                            ProtoReader.NoteObject(expr_2C0, protoReader);
                                                            expr_2C0.Init();
                                                            popupData = expr_2C0;
                                                        }
                                                        string text = protoReader.ReadString();
                                                        if (text != null)
                                                        {
                                                            popupData.CancelButtonText = text;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (popupData == null)
                                                    {
                                                        PopupData expr_26D = new PopupData();
                                                        ProtoReader.NoteObject(expr_26D, protoReader);
                                                        expr_26D.Init();
                                                        popupData = expr_26D;
                                                    }
                                                    EligibilityRequirements arg_28B_0 = popupData.OkButtonRequirement;
                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                    EligibilityRequirements arg_298_0 = TierXConfigurationSerializer.Read(arg_28B_0, protoReader);
                                                    ProtoReader.EndSubItem(token, protoReader);
                                                    EligibilityRequirements eligibilityRequirements = arg_298_0;
                                                    if (eligibilityRequirements != null)
                                                    {
                                                        popupData.OkButtonRequirement = eligibilityRequirements;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (popupData == null)
                                                {
                                                    PopupData expr_233 = new PopupData();
                                                    ProtoReader.NoteObject(expr_233, protoReader);
                                                    expr_233.Init();
                                                    popupData = expr_233;
                                                }
                                                string text = protoReader.ReadString();
                                                if (text != null)
                                                {
                                                    popupData.ConfirmButtonText = text;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (popupData == null)
                                            {
                                                PopupData expr_1FA = new PopupData();
                                                ProtoReader.NoteObject(expr_1FA, protoReader);
                                                expr_1FA.Init();
                                                popupData = expr_1FA;
                                            }
                                            TimeSpan starterPackValidityDuration = BclHelpers.ReadTimeSpan(protoReader);
                                            popupData.StarterPackValidityDuration = starterPackValidityDuration;
                                        }
                                    }
                                    else
                                    {
                                        if (popupData == null)
                                        {
                                            PopupData expr_1C1 = new PopupData();
                                            ProtoReader.NoteObject(expr_1C1, protoReader);
                                            expr_1C1.Init();
                                            popupData = expr_1C1;
                                        }
                                        string text = protoReader.ReadString();
                                        if (text != null)
                                        {
                                            popupData.StarterPackOfferItem = text;
                                        }
                                    }
                                }
                                else
                                {
                                    if (popupData == null)
                                    {
                                        PopupData expr_188 = new PopupData();
                                        ProtoReader.NoteObject(expr_188, protoReader);
                                        expr_188.Init();
                                        popupData = expr_188;
                                    }
                                    string text = protoReader.ReadString();
                                    if (text != null)
                                    {
                                        popupData.StarterPackItem2 = text;
                                    }
                                }
                            }
                            else
                            {
                                if (popupData == null)
                                {
                                    PopupData expr_14F = new PopupData();
                                    ProtoReader.NoteObject(expr_14F, protoReader);
                                    expr_14F.Init();
                                    popupData = expr_14F;
                                }
                                string text = protoReader.ReadString();
                                if (text != null)
                                {
                                    popupData.StarterPackItem1 = text;
                                }
                            }
                        }
                        else
                        {
                            if (popupData == null)
                            {
                                PopupData expr_D0 = new PopupData();
                                ProtoReader.NoteObject(expr_D0, protoReader);
                                expr_D0.Init();
                                popupData = expr_D0;
                            }
                            List<FormatStringData> list3 = popupData.BodyText;
                            List<FormatStringData> list4 = list3;
                            if (list3 == null)
                            {
                                list3 = new List<FormatStringData>();
                            }
                            int fieldNumber = protoReader.FieldNumber;
                            do
                            {
                                List<FormatStringData> arg_111_0 = list3;
                                FormatStringData arg_104_0 = null;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                FormatStringData arg_111_1 = TierXConfigurationSerializer.Read(arg_104_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                arg_111_0.Add(arg_111_1);
                            }
                            while (protoReader.TryReadFieldHeader(fieldNumber));
                            list4 = ((list4 == list3) ? null : list3);
                            if (list4 != null)
                            {
                                popupData.BodyText = list4;
                            }
                        }
                    }
                    else
                    {
                        if (popupData == null)
                        {
                            PopupData expr_97 = new PopupData();
                            ProtoReader.NoteObject(expr_97, protoReader);
                            expr_97.Init();
                            popupData = expr_97;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            popupData.TitleText = text;
                        }
                    }
                }
                else
                {
                    if (popupData == null)
                    {
                        PopupData expr_5E = new PopupData();
                        ProtoReader.NoteObject(expr_5E, protoReader);
                        expr_5E.Init();
                        popupData = expr_5E;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        popupData.CharacterTexture = text;
                    }
                }
            }
            else
            {
                if (popupData == null)
                {
                    PopupData expr_25 = new PopupData();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    popupData = expr_25;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    popupData.CharacterName = text;
                }
            }
        }
        if (popupData == null)
        {
            PopupData expr_5C0 = new PopupData();
            ProtoReader.NoteObject(expr_5C0, protoReader);
            expr_5C0.Init();
            popupData = expr_5C0;
        }
        return popupData;
    }

    private static void Write(PopupDataButtonAction popupDataButtonAction, ProtoWriter protoWriter)
    {
        if (popupDataButtonAction.GetType() != typeof(PopupDataButtonAction))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PopupDataButtonAction), popupDataButtonAction.GetType());
        }
        string expr_2D = popupDataButtonAction.Type;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        EligibilityConditionDetails expr_4A = popupDataButtonAction.Details;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_4A, protoWriter);
            TierXConfigurationSerializer.Write(expr_4A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static PopupDataButtonAction Read(PopupDataButtonAction popupDataButtonAction, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (popupDataButtonAction == null)
                    {
                        PopupDataButtonAction expr_8A = new PopupDataButtonAction();
                        ProtoReader.NoteObject(expr_8A, protoReader);
                        popupDataButtonAction = expr_8A;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (popupDataButtonAction == null)
                    {
                        PopupDataButtonAction expr_4C = new PopupDataButtonAction();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        popupDataButtonAction = expr_4C;
                    }
                    EligibilityConditionDetails arg_63_0 = popupDataButtonAction.Details;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityConditionDetails arg_6F_0 = TierXConfigurationSerializer.Read(arg_63_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    EligibilityConditionDetails eligibilityConditionDetails = arg_6F_0;
                    if (eligibilityConditionDetails != null)
                    {
                        popupDataButtonAction.Details = eligibilityConditionDetails;
                    }
                }
            }
            else
            {
                if (popupDataButtonAction == null)
                {
                    PopupDataButtonAction expr_19 = new PopupDataButtonAction();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    popupDataButtonAction = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    popupDataButtonAction.Type = text;
                }
            }
        }
        if (popupDataButtonAction == null)
        {
            PopupDataButtonAction expr_B2 = new PopupDataButtonAction();
            ProtoReader.NoteObject(expr_B2, protoReader);
            popupDataButtonAction = expr_B2;
        }
        return popupDataButtonAction;
    }

    private static void Write(ProgressionVisualisation progressionVisualisation, ProtoWriter writer)
    {
        if (progressionVisualisation.GetType() != typeof(ProgressionVisualisation))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ProgressionVisualisation), progressionVisualisation.GetType());
        }
        string expr_2D = progressionVisualisation.ViewStyleString;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        string expr_4A = progressionVisualisation.Accumulator;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
        List<string> expr_67 = progressionVisualisation.SequenceIDs;
        if (expr_67 != null)
        {
            List<string> list = expr_67;
            foreach (string arg_8D_0 in list)
            {
                ProtoWriter.WriteFieldHeader(3, WireType.String, writer);
                ProtoWriter.WriteString(arg_8D_0, writer);
            }
        }
        string expr_BC = progressionVisualisation.ThemeID;
        if (expr_BC != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, writer);
            ProtoWriter.WriteString(expr_BC, writer);
        }
    }

    private static ProgressionVisualisation Read(ProgressionVisualisation progressionVisualisation, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (progressionVisualisation == null)
                            {
                                ProgressionVisualisation expr_111 = new ProgressionVisualisation();
                                ProtoReader.NoteObject(expr_111, protoReader);
                                progressionVisualisation = expr_111;
                            }
                            protoReader.SkipField();
                        }
                        else
                        {
                            if (progressionVisualisation == null)
                            {
                                ProgressionVisualisation expr_E7 = new ProgressionVisualisation();
                                ProtoReader.NoteObject(expr_E7, protoReader);
                                progressionVisualisation = expr_E7;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                progressionVisualisation.ThemeID = text;
                            }
                        }
                    }
                    else
                    {
                        if (progressionVisualisation == null)
                        {
                            ProgressionVisualisation expr_7F = new ProgressionVisualisation();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            progressionVisualisation = expr_7F;
                        }
                        List<string> list = progressionVisualisation.SequenceIDs;
                        List<string> list2 = list;
                        if (list == null)
                        {
                            list = new List<string>();
                        }
                        int fieldNumber = protoReader.FieldNumber;
                        do
                        {
                            list.Add(protoReader.ReadString());
                        }
                        while (protoReader.TryReadFieldHeader(fieldNumber));
                        list2 = ((list2 == list) ? null : list);
                        if (list2 != null)
                        {
                            progressionVisualisation.SequenceIDs = list2;
                        }
                    }
                }
                else
                {
                    if (progressionVisualisation == null)
                    {
                        ProgressionVisualisation expr_4C = new ProgressionVisualisation();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        progressionVisualisation = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        progressionVisualisation.Accumulator = text;
                    }
                }
            }
            else
            {
                if (progressionVisualisation == null)
                {
                    ProgressionVisualisation expr_19 = new ProgressionVisualisation();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    progressionVisualisation = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    progressionVisualisation.ViewStyleString = text;
                }
            }
        }
        if (progressionVisualisation == null)
        {
            ProgressionVisualisation expr_139 = new ProgressionVisualisation();
            ProtoReader.NoteObject(expr_139, protoReader);
            progressionVisualisation = expr_139;
        }
        return progressionVisualisation;
    }

    private static void Write(PushScreenAction pushScreenAction, ProtoWriter writer)
    {
        if (pushScreenAction.GetType() != typeof(PushScreenAction))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(PushScreenAction), pushScreenAction.GetType());
        }
        string expr_2D = pushScreenAction.ScreenID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
    }

    private static PushScreenAction Read(PushScreenAction pushScreenAction, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (pushScreenAction == null)
                {
                    PushScreenAction expr_43 = new PushScreenAction();
                    ProtoReader.NoteObject(expr_43, protoReader);
                    pushScreenAction = expr_43;
                }
                protoReader.SkipField();
            }
            else
            {
                if (pushScreenAction == null)
                {
                    PushScreenAction expr_19 = new PushScreenAction();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    pushScreenAction = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    pushScreenAction.ScreenID = text;
                }
            }
        }
        if (pushScreenAction == null)
        {
            PushScreenAction expr_6B = new PushScreenAction();
            ProtoReader.NoteObject(expr_6B, protoReader);
            pushScreenAction = expr_6B;
        }
        return pushScreenAction;
    }

    private static void Write(RestrictionRaceHelperOverride restrictionRaceHelperOverride, ProtoWriter writer)
    {
        if (restrictionRaceHelperOverride.GetType() != typeof(RestrictionRaceHelperOverride))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(RestrictionRaceHelperOverride), restrictionRaceHelperOverride.GetType());
        }
        int expr_2D = restrictionRaceHelperOverride.EventID;
        if (expr_2D != 0)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
            ProtoWriter.WriteInt32(expr_2D, writer);
        }
        string expr_4A = restrictionRaceHelperOverride.BundledGraphicPath;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
        bool expr_67 = restrictionRaceHelperOverride.IsCrewLeader;
        if (expr_67)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.Variant, writer);
            ProtoWriter.WriteBoolean(expr_67, writer);
        }
        string expr_84 = restrictionRaceHelperOverride.BodyText;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, writer);
            ProtoWriter.WriteString(expr_84, writer);
        }
        string expr_A1 = restrictionRaceHelperOverride.ImageCaption;
        if (expr_A1 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, writer);
            ProtoWriter.WriteString(expr_A1, writer);
        }
    }

    private static RestrictionRaceHelperOverride Read(RestrictionRaceHelperOverride restrictionRaceHelperOverride, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (restrictionRaceHelperOverride == null)
                                {
                                    RestrictionRaceHelperOverride expr_109 = new RestrictionRaceHelperOverride();
                                    ProtoReader.NoteObject(expr_109, protoReader);
                                    restrictionRaceHelperOverride = expr_109;
                                }
                                protoReader.SkipField();
                            }
                            else
                            {
                                if (restrictionRaceHelperOverride == null)
                                {
                                    RestrictionRaceHelperOverride expr_DF = new RestrictionRaceHelperOverride();
                                    ProtoReader.NoteObject(expr_DF, protoReader);
                                    restrictionRaceHelperOverride = expr_DF;
                                }
                                string text = protoReader.ReadString();
                                if (text != null)
                                {
                                    restrictionRaceHelperOverride.ImageCaption = text;
                                }
                            }
                        }
                        else
                        {
                            if (restrictionRaceHelperOverride == null)
                            {
                                RestrictionRaceHelperOverride expr_AC = new RestrictionRaceHelperOverride();
                                ProtoReader.NoteObject(expr_AC, protoReader);
                                restrictionRaceHelperOverride = expr_AC;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                restrictionRaceHelperOverride.BodyText = text;
                            }
                        }
                    }
                    else
                    {
                        if (restrictionRaceHelperOverride == null)
                        {
                            RestrictionRaceHelperOverride expr_7C = new RestrictionRaceHelperOverride();
                            ProtoReader.NoteObject(expr_7C, protoReader);
                            restrictionRaceHelperOverride = expr_7C;
                        }
                        bool isCrewLeader = protoReader.ReadBoolean();
                        restrictionRaceHelperOverride.IsCrewLeader = isCrewLeader;
                    }
                }
                else
                {
                    if (restrictionRaceHelperOverride == null)
                    {
                        RestrictionRaceHelperOverride expr_49 = new RestrictionRaceHelperOverride();
                        ProtoReader.NoteObject(expr_49, protoReader);
                        restrictionRaceHelperOverride = expr_49;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        restrictionRaceHelperOverride.BundledGraphicPath = text;
                    }
                }
            }
            else
            {
                if (restrictionRaceHelperOverride == null)
                {
                    RestrictionRaceHelperOverride expr_19 = new RestrictionRaceHelperOverride();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    restrictionRaceHelperOverride = expr_19;
                }
                int eventID = protoReader.ReadInt32();
                restrictionRaceHelperOverride.EventID = eventID;
            }
        }
        if (restrictionRaceHelperOverride == null)
        {
            RestrictionRaceHelperOverride expr_131 = new RestrictionRaceHelperOverride();
            ProtoReader.NoteObject(expr_131, protoReader);
            restrictionRaceHelperOverride = expr_131;
        }
        return restrictionRaceHelperOverride;
    }

    private static void Write(ScheduledPin scheduledPin, ProtoWriter protoWriter)
    {
        if (scheduledPin.GetType() != typeof(ScheduledPin))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ScheduledPin), scheduledPin.GetType());
        }
        string expr_2D = scheduledPin.ID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = scheduledPin.EventType;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        string expr_67 = scheduledPin.LifetimeGroup;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_67, protoWriter);
        }
        string expr_84 = scheduledPin.PinID;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_84, protoWriter);
        }
        PinSchedulerAIDriverOverrides expr_A1 = scheduledPin.AIDriverOverrides;
        SubItemToken token;
        if (expr_A1 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_A1, protoWriter);
            TierXConfigurationSerializer.Write(expr_A1, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        NarrativeSceneForEventData expr_CD = scheduledPin.Narrative;
        if (expr_CD != null)
        {
            ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_CD, protoWriter);
            TierXConfigurationSerializer.Write(expr_CD, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        EligibilityRequirements expr_F9 = scheduledPin.Requirements;
        if (expr_F9 != null)
        {
            ProtoWriter.WriteFieldHeader(7, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_F9, protoWriter);
            TierXConfigurationSerializer.Write(expr_F9, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        string expr_125 = scheduledPin.OnPostRaceLostPopupID;
        if (expr_125 != null)
        {
            ProtoWriter.WriteFieldHeader(8, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_125, protoWriter);
        }
        string expr_142 = scheduledPin.OnWorkshopPopupID;
        if (expr_142 != null)
        {
            ProtoWriter.WriteFieldHeader(9, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_142, protoWriter);
        }
        string expr_160 = scheduledPin.OnMapPinTapPopupID;
        if (expr_160 != null)
        {
            ProtoWriter.WriteFieldHeader(10, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_160, protoWriter);
        }
        BubbleMessageData expr_17E = scheduledPin.PinBubbleMessage;
        if (expr_17E != null)
        {
            ProtoWriter.WriteFieldHeader(11, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_17E, protoWriter);
            TierXConfigurationSerializer.Write(expr_17E, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        bool expr_1AB = scheduledPin.IsNextButtonLocked;
        if (expr_1AB)
        {
            ProtoWriter.WriteFieldHeader(12, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_1AB, protoWriter);
        }
        bool expr_1C9 = scheduledPin.AutoStart;
        if (expr_1C9)
        {
            ProtoWriter.WriteFieldHeader(13, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_1C9, protoWriter);
        }
        string expr_1E7 = scheduledPin.NextScreen;
        if (expr_1E7 != null)
        {
            ProtoWriter.WriteFieldHeader(14, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1E7, protoWriter);
        }
        bool expr_205 = scheduledPin.ShowAnimationIn;
        if (expr_205)
        {
            ProtoWriter.WriteFieldHeader(15, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_205, protoWriter);
        }
        string expr_223 = scheduledPin.AppearAnimationSelectionTypeString;
        if (expr_223 != null)
        {
            ProtoWriter.WriteFieldHeader(16, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_223, protoWriter);
        }
        Vector3 arg_253_0 = scheduledPin.AppearAnimationInitialScale;
        ProtoWriter.WriteFieldHeader(17, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_253_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        List<string> expr_265 = scheduledPin.AppearAnimations;
        if (expr_265 != null)
        {
            List<string> list = expr_265;
            foreach (string arg_28C_0 in list)
            {
                ProtoWriter.WriteFieldHeader(18, WireType.String, protoWriter);
                ProtoWriter.WriteString(arg_28C_0, protoWriter);
            }
        }
        List<CarOverride> expr_2BB = scheduledPin.CarOverrides;
        if (expr_2BB != null)
        {
            List<CarOverride> list2 = expr_2BB;
            foreach (CarOverride arg_2E2_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(19, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_2E2_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_2E2_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        ProgressionVisualisation expr_321 = scheduledPin.ProgressionVisualisation;
        if (expr_321 != null)
        {
            ProtoWriter.WriteFieldHeader(20, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_321, protoWriter);
            TierXConfigurationSerializer.Write(expr_321, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        string expr_34E = scheduledPin.CarToAwardID;
        if (expr_34E != null)
        {
            ProtoWriter.WriteFieldHeader(21, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_34E, protoWriter);
        }
        byte expr_36C = scheduledPin.CarToAwardUpgradeLevel;
        if (expr_36C != 0)
        {
            ProtoWriter.WriteFieldHeader(22, WireType.Variant, protoWriter);
            ProtoWriter.WriteByte(expr_36C, protoWriter);
        }
        ChoiceScreenInfo expr_38A = scheduledPin.ChoiceScreen;
        if (expr_38A != null)
        {
            ProtoWriter.WriteFieldHeader(23, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_38A, protoWriter);
            TierXConfigurationSerializer.Write(expr_38A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        int expr_3B7 = scheduledPin.DefaultAutoSelectPriority;
        if (expr_3B7 != 0)
        {
            ProtoWriter.WriteFieldHeader(24, WireType.Variant, protoWriter);
            ProtoWriter.WriteInt32(expr_3B7, protoWriter);
        }
        int expr_3D5 = scheduledPin.LastSequenceAutoSelectPriority;
        if (expr_3D5 != 0)
        {
            ProtoWriter.WriteFieldHeader(25, WireType.Variant, protoWriter);
            ProtoWriter.WriteInt32(expr_3D5, protoWriter);
        }
        string expr_3F3 = scheduledPin.SequenceReference;
        if (expr_3F3 != null)
        {
            ProtoWriter.WriteFieldHeader(26, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_3F3, protoWriter);
        }
    }

    private static ScheduledPin Read(ScheduledPin scheduledPin, ProtoReader protoReader)
    {
        if (scheduledPin != null)
        {
            scheduledPin.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (num != 10)
                                                {
                                                    if (num != 11)
                                                    {
                                                        if (num != 12)
                                                        {
                                                            if (num != 13)
                                                            {
                                                                if (num != 14)
                                                                {
                                                                    if (num != 15)
                                                                    {
                                                                        if (num != 16)
                                                                        {
                                                                            if (num != 17)
                                                                            {
                                                                                if (num != 18)
                                                                                {
                                                                                    if (num != 19)
                                                                                    {
                                                                                        if (num != 20)
                                                                                        {
                                                                                            if (num != 21)
                                                                                            {
                                                                                                if (num != 22)
                                                                                                {
                                                                                                    if (num != 23)
                                                                                                    {
                                                                                                        if (num != 24)
                                                                                                        {
                                                                                                            if (num != 25)
                                                                                                            {
                                                                                                                if (num != 26)
                                                                                                                {
                                                                                                                    if (scheduledPin == null)
                                                                                                                    {
                                                                                                                        ScheduledPin expr_71D = new ScheduledPin();
                                                                                                                        ProtoReader.NoteObject(expr_71D, protoReader);
                                                                                                                        expr_71D.Init();
                                                                                                                        scheduledPin = expr_71D;
                                                                                                                    }
                                                                                                                    protoReader.SkipField();
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (scheduledPin == null)
                                                                                                                    {
                                                                                                                        ScheduledPin expr_6ED = new ScheduledPin();
                                                                                                                        ProtoReader.NoteObject(expr_6ED, protoReader);
                                                                                                                        expr_6ED.Init();
                                                                                                                        scheduledPin = expr_6ED;
                                                                                                                    }
                                                                                                                    string text = protoReader.ReadString();
                                                                                                                    if (text != null)
                                                                                                                    {
                                                                                                                        scheduledPin.SequenceReference = text;
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (scheduledPin == null)
                                                                                                                {
                                                                                                                    ScheduledPin expr_6B4 = new ScheduledPin();
                                                                                                                    ProtoReader.NoteObject(expr_6B4, protoReader);
                                                                                                                    expr_6B4.Init();
                                                                                                                    scheduledPin = expr_6B4;
                                                                                                                }
                                                                                                                int num2 = protoReader.ReadInt32();
                                                                                                                scheduledPin.LastSequenceAutoSelectPriority = num2;
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (scheduledPin == null)
                                                                                                            {
                                                                                                                ScheduledPin expr_67B = new ScheduledPin();
                                                                                                                ProtoReader.NoteObject(expr_67B, protoReader);
                                                                                                                expr_67B.Init();
                                                                                                                scheduledPin = expr_67B;
                                                                                                            }
                                                                                                            int num2 = protoReader.ReadInt32();
                                                                                                            scheduledPin.DefaultAutoSelectPriority = num2;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (scheduledPin == null)
                                                                                                        {
                                                                                                            ScheduledPin expr_62A = new ScheduledPin();
                                                                                                            ProtoReader.NoteObject(expr_62A, protoReader);
                                                                                                            expr_62A.Init();
                                                                                                            scheduledPin = expr_62A;
                                                                                                        }
                                                                                                        ChoiceScreenInfo arg_647_0 = scheduledPin.ChoiceScreen;
                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                        ChoiceScreenInfo arg_653_0 = TierXConfigurationSerializer.Read(arg_647_0, protoReader);
                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                        ChoiceScreenInfo choiceScreenInfo = arg_653_0;
                                                                                                        if (choiceScreenInfo != null)
                                                                                                        {
                                                                                                            scheduledPin.ChoiceScreen = choiceScreenInfo;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (scheduledPin == null)
                                                                                                    {
                                                                                                        ScheduledPin expr_5F1 = new ScheduledPin();
                                                                                                        ProtoReader.NoteObject(expr_5F1, protoReader);
                                                                                                        expr_5F1.Init();
                                                                                                        scheduledPin = expr_5F1;
                                                                                                    }
                                                                                                    byte carToAwardUpgradeLevel = protoReader.ReadByte();
                                                                                                    scheduledPin.CarToAwardUpgradeLevel = carToAwardUpgradeLevel;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (scheduledPin == null)
                                                                                                {
                                                                                                    ScheduledPin expr_5B7 = new ScheduledPin();
                                                                                                    ProtoReader.NoteObject(expr_5B7, protoReader);
                                                                                                    expr_5B7.Init();
                                                                                                    scheduledPin = expr_5B7;
                                                                                                }
                                                                                                string text = protoReader.ReadString();
                                                                                                if (text != null)
                                                                                                {
                                                                                                    scheduledPin.CarToAwardID = text;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (scheduledPin == null)
                                                                                            {
                                                                                                ScheduledPin expr_566 = new ScheduledPin();
                                                                                                ProtoReader.NoteObject(expr_566, protoReader);
                                                                                                expr_566.Init();
                                                                                                scheduledPin = expr_566;
                                                                                            }
                                                                                            ProgressionVisualisation arg_583_0 = scheduledPin.ProgressionVisualisation;
                                                                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                            ProgressionVisualisation arg_58F_0 = TierXConfigurationSerializer.Read(arg_583_0, protoReader);
                                                                                            ProtoReader.EndSubItem(token, protoReader);
                                                                                            ProgressionVisualisation progressionVisualisation = arg_58F_0;
                                                                                            if (progressionVisualisation != null)
                                                                                            {
                                                                                                scheduledPin.ProgressionVisualisation = progressionVisualisation;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (scheduledPin == null)
                                                                                        {
                                                                                            ScheduledPin expr_4DC = new ScheduledPin();
                                                                                            ProtoReader.NoteObject(expr_4DC, protoReader);
                                                                                            expr_4DC.Init();
                                                                                            scheduledPin = expr_4DC;
                                                                                        }
                                                                                        List<CarOverride> list = scheduledPin.CarOverrides;
                                                                                        List<CarOverride> list2 = list;
                                                                                        if (list == null)
                                                                                        {
                                                                                            list = new List<CarOverride>();
                                                                                        }
                                                                                        int num2 = protoReader.FieldNumber;
                                                                                        do
                                                                                        {
                                                                                            List<CarOverride> arg_521_0 = list;
                                                                                            CarOverride arg_515_0 = null;
                                                                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                            CarOverride arg_521_1 = TierXConfigurationSerializer.Read(arg_515_0, protoReader);
                                                                                            ProtoReader.EndSubItem(token, protoReader);
                                                                                            arg_521_0.Add(arg_521_1);
                                                                                        }
                                                                                        while (protoReader.TryReadFieldHeader(num2));
                                                                                        list2 = ((list2 == list) ? null : list);
                                                                                        if (list2 != null)
                                                                                        {
                                                                                            scheduledPin.CarOverrides = list2;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (scheduledPin == null)
                                                                                    {
                                                                                        ScheduledPin expr_461 = new ScheduledPin();
                                                                                        ProtoReader.NoteObject(expr_461, protoReader);
                                                                                        expr_461.Init();
                                                                                        scheduledPin = expr_461;
                                                                                    }
                                                                                    List<string> list3 = scheduledPin.AppearAnimations;
                                                                                    List<string> list4 = list3;
                                                                                    if (list3 == null)
                                                                                    {
                                                                                        list3 = new List<string>();
                                                                                    }
                                                                                    int num2 = protoReader.FieldNumber;
                                                                                    do
                                                                                    {
                                                                                        list3.Add(protoReader.ReadString());
                                                                                    }
                                                                                    while (protoReader.TryReadFieldHeader(num2));
                                                                                    list4 = ((list4 == list3) ? null : list3);
                                                                                    if (list4 != null)
                                                                                    {
                                                                                        scheduledPin.AppearAnimations = list4;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (scheduledPin == null)
                                                                                {
                                                                                    ScheduledPin expr_414 = new ScheduledPin();
                                                                                    ProtoReader.NoteObject(expr_414, protoReader);
                                                                                    expr_414.Init();
                                                                                    scheduledPin = expr_414;
                                                                                }
                                                                                Vector3 arg_431_0 = scheduledPin.AppearAnimationInitialScale;
                                                                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                Vector3 arg_43D_0 = TierXConfigurationSerializer.Read(arg_431_0, protoReader);
                                                                                ProtoReader.EndSubItem(token, protoReader);
                                                                                Vector3 appearAnimationInitialScale = arg_43D_0;
                                                                                scheduledPin.AppearAnimationInitialScale = appearAnimationInitialScale;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (scheduledPin == null)
                                                                            {
                                                                                ScheduledPin expr_3DA = new ScheduledPin();
                                                                                ProtoReader.NoteObject(expr_3DA, protoReader);
                                                                                expr_3DA.Init();
                                                                                scheduledPin = expr_3DA;
                                                                            }
                                                                            string text = protoReader.ReadString();
                                                                            if (text != null)
                                                                            {
                                                                                scheduledPin.AppearAnimationSelectionTypeString = text;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (scheduledPin == null)
                                                                        {
                                                                            ScheduledPin expr_3A1 = new ScheduledPin();
                                                                            ProtoReader.NoteObject(expr_3A1, protoReader);
                                                                            expr_3A1.Init();
                                                                            scheduledPin = expr_3A1;
                                                                        }
                                                                        bool flag = protoReader.ReadBoolean();
                                                                        scheduledPin.ShowAnimationIn = flag;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (scheduledPin == null)
                                                                    {
                                                                        ScheduledPin expr_367 = new ScheduledPin();
                                                                        ProtoReader.NoteObject(expr_367, protoReader);
                                                                        expr_367.Init();
                                                                        scheduledPin = expr_367;
                                                                    }
                                                                    string text = protoReader.ReadString();
                                                                    if (text != null)
                                                                    {
                                                                        scheduledPin.NextScreen = text;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (scheduledPin == null)
                                                                {
                                                                    ScheduledPin expr_32E = new ScheduledPin();
                                                                    ProtoReader.NoteObject(expr_32E, protoReader);
                                                                    expr_32E.Init();
                                                                    scheduledPin = expr_32E;
                                                                }
                                                                bool flag = protoReader.ReadBoolean();
                                                                scheduledPin.AutoStart = flag;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (scheduledPin == null)
                                                            {
                                                                ScheduledPin expr_2F5 = new ScheduledPin();
                                                                ProtoReader.NoteObject(expr_2F5, protoReader);
                                                                expr_2F5.Init();
                                                                scheduledPin = expr_2F5;
                                                            }
                                                            bool flag = protoReader.ReadBoolean();
                                                            scheduledPin.IsNextButtonLocked = flag;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (scheduledPin == null)
                                                        {
                                                            ScheduledPin expr_2A4 = new ScheduledPin();
                                                            ProtoReader.NoteObject(expr_2A4, protoReader);
                                                            expr_2A4.Init();
                                                            scheduledPin = expr_2A4;
                                                        }
                                                        BubbleMessageData arg_2C1_0 = scheduledPin.PinBubbleMessage;
                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                        BubbleMessageData arg_2CD_0 = TierXConfigurationSerializer.Read(arg_2C1_0, protoReader);
                                                        ProtoReader.EndSubItem(token, protoReader);
                                                        BubbleMessageData bubbleMessageData = arg_2CD_0;
                                                        if (bubbleMessageData != null)
                                                        {
                                                            scheduledPin.PinBubbleMessage = bubbleMessageData;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (scheduledPin == null)
                                                    {
                                                        ScheduledPin expr_26A = new ScheduledPin();
                                                        ProtoReader.NoteObject(expr_26A, protoReader);
                                                        expr_26A.Init();
                                                        scheduledPin = expr_26A;
                                                    }
                                                    string text = protoReader.ReadString();
                                                    if (text != null)
                                                    {
                                                        scheduledPin.OnMapPinTapPopupID = text;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (scheduledPin == null)
                                                {
                                                    ScheduledPin expr_230 = new ScheduledPin();
                                                    ProtoReader.NoteObject(expr_230, protoReader);
                                                    expr_230.Init();
                                                    scheduledPin = expr_230;
                                                }
                                                string text = protoReader.ReadString();
                                                if (text != null)
                                                {
                                                    scheduledPin.OnWorkshopPopupID = text;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (scheduledPin == null)
                                            {
                                                ScheduledPin expr_1F6 = new ScheduledPin();
                                                ProtoReader.NoteObject(expr_1F6, protoReader);
                                                expr_1F6.Init();
                                                scheduledPin = expr_1F6;
                                            }
                                            string text = protoReader.ReadString();
                                            if (text != null)
                                            {
                                                scheduledPin.OnPostRaceLostPopupID = text;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (scheduledPin == null)
                                        {
                                            ScheduledPin expr_1A6 = new ScheduledPin();
                                            ProtoReader.NoteObject(expr_1A6, protoReader);
                                            expr_1A6.Init();
                                            scheduledPin = expr_1A6;
                                        }
                                        EligibilityRequirements arg_1C3_0 = scheduledPin.Requirements;
                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                        EligibilityRequirements arg_1CF_0 = TierXConfigurationSerializer.Read(arg_1C3_0, protoReader);
                                        ProtoReader.EndSubItem(token, protoReader);
                                        EligibilityRequirements eligibilityRequirements = arg_1CF_0;
                                        if (eligibilityRequirements != null)
                                        {
                                            scheduledPin.Requirements = eligibilityRequirements;
                                        }
                                    }
                                }
                                else
                                {
                                    if (scheduledPin == null)
                                    {
                                        ScheduledPin expr_156 = new ScheduledPin();
                                        ProtoReader.NoteObject(expr_156, protoReader);
                                        expr_156.Init();
                                        scheduledPin = expr_156;
                                    }
                                    NarrativeSceneForEventData arg_173_0 = scheduledPin.Narrative;
                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                    NarrativeSceneForEventData arg_17F_0 = TierXConfigurationSerializer.Read(arg_173_0, protoReader);
                                    ProtoReader.EndSubItem(token, protoReader);
                                    NarrativeSceneForEventData narrativeSceneForEventData = arg_17F_0;
                                    if (narrativeSceneForEventData != null)
                                    {
                                        scheduledPin.Narrative = narrativeSceneForEventData;
                                    }
                                }
                            }
                            else
                            {
                                if (scheduledPin == null)
                                {
                                    ScheduledPin expr_109 = new ScheduledPin();
                                    ProtoReader.NoteObject(expr_109, protoReader);
                                    expr_109.Init();
                                    scheduledPin = expr_109;
                                }
                                PinSchedulerAIDriverOverrides arg_126_0 = scheduledPin.AIDriverOverrides;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                PinSchedulerAIDriverOverrides arg_132_0 = TierXConfigurationSerializer.Read(arg_126_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                PinSchedulerAIDriverOverrides pinSchedulerAIDriverOverrides = arg_132_0;
                                if (pinSchedulerAIDriverOverrides != null)
                                {
                                    scheduledPin.AIDriverOverrides = pinSchedulerAIDriverOverrides;
                                }
                            }
                        }
                        else
                        {
                            if (scheduledPin == null)
                            {
                                ScheduledPin expr_D0 = new ScheduledPin();
                                ProtoReader.NoteObject(expr_D0, protoReader);
                                expr_D0.Init();
                                scheduledPin = expr_D0;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                scheduledPin.PinID = text;
                            }
                        }
                    }
                    else
                    {
                        if (scheduledPin == null)
                        {
                            ScheduledPin expr_97 = new ScheduledPin();
                            ProtoReader.NoteObject(expr_97, protoReader);
                            expr_97.Init();
                            scheduledPin = expr_97;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            scheduledPin.LifetimeGroup = text;
                        }
                    }
                }
                else
                {
                    if (scheduledPin == null)
                    {
                        ScheduledPin expr_5E = new ScheduledPin();
                        ProtoReader.NoteObject(expr_5E, protoReader);
                        expr_5E.Init();
                        scheduledPin = expr_5E;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        scheduledPin.EventType = text;
                    }
                }
            }
            else
            {
                if (scheduledPin == null)
                {
                    ScheduledPin expr_25 = new ScheduledPin();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    scheduledPin = expr_25;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    scheduledPin.ID = text;
                }
            }
        }
        if (scheduledPin == null)
        {
            ScheduledPin expr_74B = new ScheduledPin();
            ProtoReader.NoteObject(expr_74B, protoReader);
            expr_74B.Init();
            scheduledPin = expr_74B;
        }
        return scheduledPin;
    }

    private static void Write(SoundEventDetail soundEventDetail, ProtoWriter protoWriter)
    {
        if (soundEventDetail.GetType() != typeof(SoundEventDetail))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(SoundEventDetail), soundEventDetail.GetType());
        }
        string expr_2D = soundEventDetail.Name;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        float expr_4A = soundEventDetail.EventTime;
        if (expr_4A != 0f)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, protoWriter);
            ProtoWriter.WriteSingle(expr_4A, protoWriter);
        }
        float expr_6C = soundEventDetail.SoundStart;
        if (expr_6C != 0f)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.Fixed32, protoWriter);
            ProtoWriter.WriteSingle(expr_6C, protoWriter);
        }
        EligibilityRequirements expr_8E = soundEventDetail.SoundRequirements;
        if (expr_8E != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_8E, protoWriter);
            TierXConfigurationSerializer.Write(expr_8E, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static SoundEventDetail Read(SoundEventDetail soundEventDetail, ProtoReader protoReader)
    {
        if (soundEventDetail != null)
        {
            soundEventDetail.Init();
        }
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (soundEventDetail == null)
                            {
                                SoundEventDetail expr_111 = new SoundEventDetail();
                                ProtoReader.NoteObject(expr_111, protoReader);
                                expr_111.Init();
                                soundEventDetail = expr_111;
                            }
                            protoReader.SkipField();
                        }
                        else
                        {
                            if (soundEventDetail == null)
                            {
                                SoundEventDetail expr_CA = new SoundEventDetail();
                                ProtoReader.NoteObject(expr_CA, protoReader);
                                expr_CA.Init();
                                soundEventDetail = expr_CA;
                            }
                            EligibilityRequirements arg_E7_0 = soundEventDetail.SoundRequirements;
                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                            EligibilityRequirements arg_F3_0 = TierXConfigurationSerializer.Read(arg_E7_0, protoReader);
                            ProtoReader.EndSubItem(token, protoReader);
                            EligibilityRequirements eligibilityRequirements = arg_F3_0;
                            if (eligibilityRequirements != null)
                            {
                                soundEventDetail.SoundRequirements = eligibilityRequirements;
                            }
                        }
                    }
                    else
                    {
                        if (soundEventDetail == null)
                        {
                            SoundEventDetail expr_94 = new SoundEventDetail();
                            ProtoReader.NoteObject(expr_94, protoReader);
                            expr_94.Init();
                            soundEventDetail = expr_94;
                        }
                        float num2 = protoReader.ReadSingle();
                        soundEventDetail.SoundStart = num2;
                    }
                }
                else
                {
                    if (soundEventDetail == null)
                    {
                        SoundEventDetail expr_5E = new SoundEventDetail();
                        ProtoReader.NoteObject(expr_5E, protoReader);
                        expr_5E.Init();
                        soundEventDetail = expr_5E;
                    }
                    float num2 = protoReader.ReadSingle();
                    soundEventDetail.EventTime = num2;
                }
            }
            else
            {
                if (soundEventDetail == null)
                {
                    SoundEventDetail expr_25 = new SoundEventDetail();
                    ProtoReader.NoteObject(expr_25, protoReader);
                    expr_25.Init();
                    soundEventDetail = expr_25;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    soundEventDetail.Name = text;
                }
            }
        }
        if (soundEventDetail == null)
        {
            SoundEventDetail expr_13F = new SoundEventDetail();
            ProtoReader.NoteObject(expr_13F, protoReader);
            expr_13F.Init();
            soundEventDetail = expr_13F;
        }
        return soundEventDetail;
    }

    private static void Write(StringModification stringModification, ProtoWriter protoWriter)
    {
        if (stringModification.GetType() != typeof(StringModification))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(StringModification), stringModification.GetType());
        }
        string expr_2D = stringModification.Type;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        StringModification.Details expr_4A = stringModification.ModificationDetails;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_4A, protoWriter);
            TierXConfigurationSerializer.Write(expr_4A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static StringModification Read(StringModification stringModification, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (stringModification == null)
                    {
                        StringModification expr_8A = new StringModification();
                        ProtoReader.NoteObject(expr_8A, protoReader);
                        stringModification = expr_8A;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (stringModification == null)
                    {
                        StringModification expr_4C = new StringModification();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        stringModification = expr_4C;
                    }
                    StringModification.Details arg_63_0 = stringModification.ModificationDetails;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    StringModification.Details arg_6F_0 = TierXConfigurationSerializer.Read(arg_63_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    StringModification.Details details = arg_6F_0;
                    if (details != null)
                    {
                        stringModification.ModificationDetails = details;
                    }
                }
            }
            else
            {
                if (stringModification == null)
                {
                    StringModification expr_19 = new StringModification();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    stringModification = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    stringModification.Type = text;
                }
            }
        }
        if (stringModification == null)
        {
            StringModification expr_B2 = new StringModification();
            ProtoReader.NoteObject(expr_B2, protoReader);
            stringModification = expr_B2;
        }
        return stringModification;
    }

    private static void Write(StringModification.Details details, ProtoWriter writer)
    {
        if (details.GetType() != typeof(StringModification.Details))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(StringModification.Details), details.GetType());
        }
        string expr_2D = details.Default;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
            ProtoWriter.WriteString(expr_2D, writer);
        }
        string expr_4A = details.StringValue;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(expr_4A, writer);
        }
        string[] expr_67 = details.StringValues;
        if (expr_67 != null)
        {
            string[] array = expr_67;
            for (int i = 0; i < array.Length; i++)
            {
                string expr_7B = array[i];
                if (expr_7B != null)
                {
                    ProtoWriter.WriteFieldHeader(3, WireType.String, writer);
                    ProtoWriter.WriteString(expr_7B, writer);
                }
            }
        }
        string expr_A5 = details.ThemeID;
        if (expr_A5 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, writer);
            ProtoWriter.WriteString(expr_A5, writer);
        }
        bool arg_CB_0 = details.Translate;
        ProtoWriter.WriteFieldHeader(5, WireType.Variant, writer);
        ProtoWriter.WriteBoolean(arg_CB_0, writer);
        int arg_DF_0 = details.Offset;
        ProtoWriter.WriteFieldHeader(6, WireType.Variant, writer);
        ProtoWriter.WriteInt32(arg_DF_0, writer);
        string[] expr_EA = details.DefaultModifications;
        if (expr_EA != null)
        {
            string[] array = expr_EA;
            for (int i = 0; i < array.Length; i++)
            {
                string expr_FE = array[i];
                if (expr_FE != null)
                {
                    ProtoWriter.WriteFieldHeader(7, WireType.String, writer);
                    ProtoWriter.WriteString(expr_FE, writer);
                }
            }
        }
    }

    private static StringModification.Details Read(StringModification.Details details, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (details == null)
                                        {
                                            StringModification.Details expr_22B = new StringModification.Details();
                                            ProtoReader.NoteObject(expr_22B, protoReader);
                                            details = expr_22B;
                                        }
                                        protoReader.SkipField();
                                    }
                                    else
                                    {
                                        if (details == null)
                                        {
                                            StringModification.Details expr_1A5 = new StringModification.Details();
                                            ProtoReader.NoteObject(expr_1A5, protoReader);
                                            details = expr_1A5;
                                        }
                                        string[] array = details.DefaultModifications;
                                        List<string> list = new List<string>();
                                        int num2 = protoReader.FieldNumber;
                                        do
                                        {
                                            list.Add(protoReader.ReadString());
                                        }
                                        while (protoReader.TryReadFieldHeader(num2));
                                        string[] expr_1DF = array;
                                        string[] array2 = new string[(num2 = ((expr_1DF != null) ? expr_1DF.Length : 0)) + list.Count];
                                        if (num2 != 0)
                                        {
                                            array.CopyTo(array2, 0);
                                        }
                                        list.CopyTo(array2, num2);
                                        array2 = array2;
                                        if (array2 != null)
                                        {
                                            details.DefaultModifications = array2;
                                        }
                                    }
                                }
                                else
                                {
                                    if (details == null)
                                    {
                                        StringModification.Details expr_173 = new StringModification.Details();
                                        ProtoReader.NoteObject(expr_173, protoReader);
                                        details = expr_173;
                                    }
                                    int num2 = protoReader.ReadInt32();
                                    details.Offset = num2;
                                }
                            }
                            else
                            {
                                if (details == null)
                                {
                                    StringModification.Details expr_141 = new StringModification.Details();
                                    ProtoReader.NoteObject(expr_141, protoReader);
                                    details = expr_141;
                                }
                                bool translate = protoReader.ReadBoolean();
                                details.Translate = translate;
                            }
                        }
                        else
                        {
                            if (details == null)
                            {
                                StringModification.Details expr_10E = new StringModification.Details();
                                ProtoReader.NoteObject(expr_10E, protoReader);
                                details = expr_10E;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                details.ThemeID = text;
                            }
                        }
                    }
                    else
                    {
                        if (details == null)
                        {
                            StringModification.Details expr_7F = new StringModification.Details();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            details = expr_7F;
                        }
                        string[] array2 = details.StringValues;
                        List<string> list = new List<string>();
                        int num2 = protoReader.FieldNumber;
                        do
                        {
                            list.Add(protoReader.ReadString());
                        }
                        while (protoReader.TryReadFieldHeader(num2));
                        string[] expr_B9 = array2;
                        string[] array = new string[(num2 = ((expr_B9 != null) ? expr_B9.Length : 0)) + list.Count];
                        if (num2 != 0)
                        {
                            array2.CopyTo(array, 0);
                        }
                        list.CopyTo(array, num2);
                        array = array;
                        if (array != null)
                        {
                            details.StringValues = array;
                        }
                    }
                }
                else
                {
                    if (details == null)
                    {
                        StringModification.Details expr_4C = new StringModification.Details();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        details = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        details.StringValue = text;
                    }
                }
            }
            else
            {
                if (details == null)
                {
                    StringModification.Details expr_19 = new StringModification.Details();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    details = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    details.Default = text;
                }
            }
        }
        if (details == null)
        {
            StringModification.Details expr_253 = new StringModification.Details();
            ProtoReader.NoteObject(expr_253, protoReader);
            details = expr_253;
        }
        return details;
    }

    private static void Write(TextureDetail textureDetail, ProtoWriter protoWriter)
    {
        if (textureDetail.GetType() != typeof(TextureDetail))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(TextureDetail), textureDetail.GetType());
        }
        string expr_2D = textureDetail.Name;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        Vector2 arg_5B_0 = textureDetail.Offset;
        ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_5B_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        Vector2 arg_7E_0 = textureDetail.Scale;
        ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_7E_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        bool expr_90 = textureDetail.AppendThemeChoiceToName;
        if (expr_90)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_90, protoWriter);
        }
    }

    private static TextureDetail Read(TextureDetail textureDetail, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (textureDetail == null)
                            {
                                TextureDetail expr_FD = new TextureDetail();
                                ProtoReader.NoteObject(expr_FD, protoReader);
                                textureDetail = expr_FD;
                            }
                            protoReader.SkipField();
                        }
                        else
                        {
                            if (textureDetail == null)
                            {
                                TextureDetail expr_D4 = new TextureDetail();
                                ProtoReader.NoteObject(expr_D4, protoReader);
                                textureDetail = expr_D4;
                            }
                            bool appendThemeChoiceToName = protoReader.ReadBoolean();
                            textureDetail.AppendThemeChoiceToName = appendThemeChoiceToName;
                        }
                    }
                    else
                    {
                        if (textureDetail == null)
                        {
                            TextureDetail expr_90 = new TextureDetail();
                            ProtoReader.NoteObject(expr_90, protoReader);
                            textureDetail = expr_90;
                        }
                        Vector2 arg_A7_0 = textureDetail.Scale;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        Vector2 arg_B3_0 = TierXConfigurationSerializer.Read(arg_A7_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        Vector2 vector = arg_B3_0;
                        textureDetail.Scale = vector;
                    }
                }
                else
                {
                    if (textureDetail == null)
                    {
                        TextureDetail expr_4C = new TextureDetail();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        textureDetail = expr_4C;
                    }
                    Vector2 arg_63_0 = textureDetail.Offset;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    Vector2 arg_6F_0 = TierXConfigurationSerializer.Read(arg_63_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    Vector2 vector = arg_6F_0;
                    textureDetail.Offset = vector;
                }
            }
            else
            {
                if (textureDetail == null)
                {
                    TextureDetail expr_19 = new TextureDetail();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    textureDetail = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    textureDetail.Name = text;
                }
            }
        }
        if (textureDetail == null)
        {
            TextureDetail expr_125 = new TextureDetail();
            ProtoReader.NoteObject(expr_125, protoReader);
            textureDetail = expr_125;
        }
        return textureDetail;
    }

    private static void Write(ThemeAnimationDetail themeAnimationDetail, ProtoWriter protoWriter)
    {
        if (themeAnimationDetail.GetType() != typeof(ThemeAnimationDetail))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ThemeAnimationDetail), themeAnimationDetail.GetType());
        }
        string expr_2D = themeAnimationDetail.Name;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        List<PinAnimationDetail> expr_4A = themeAnimationDetail.PinAnimations;
        if (expr_4A != null)
        {
            List<PinAnimationDetail> list = expr_4A;
            foreach (PinAnimationDetail arg_6F_0 in list)
            {
                ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_6F_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_6F_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        List<EventSelectEvent> expr_AE = themeAnimationDetail.EventSelectEvents;
        if (expr_AE != null)
        {
            List<EventSelectEvent> list2 = expr_AE;
            foreach (EventSelectEvent arg_D4_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_D4_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_D4_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        List<SoundEventDetail> expr_113 = themeAnimationDetail.SoundEvents;
        if (expr_113 != null)
        {
            List<SoundEventDetail> list3 = expr_113;
            foreach (SoundEventDetail arg_13B_0 in list3)
            {
                ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_13B_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_13B_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        EligibilityRequirements expr_17A = themeAnimationDetail.AnimationRequirements;
        if (expr_17A != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_17A, protoWriter);
            TierXConfigurationSerializer.Write(expr_17A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        List<EventSelectEvent> expr_1A6 = themeAnimationDetail.InitEventSelectEvents;
        if (expr_1A6 != null)
        {
            List<EventSelectEvent> list2 = expr_1A6;
            foreach (EventSelectEvent arg_1CC_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_1CC_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_1CC_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static ThemeAnimationDetail Read(ThemeAnimationDetail themeAnimationDetail, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (themeAnimationDetail == null)
                                    {
                                        ThemeAnimationDetail expr_297 = new ThemeAnimationDetail();
                                        ProtoReader.NoteObject(expr_297, protoReader);
                                        themeAnimationDetail = expr_297;
                                    }
                                    protoReader.SkipField();
                                }
                                else
                                {
                                    if (themeAnimationDetail == null)
                                    {
                                        ThemeAnimationDetail expr_21B = new ThemeAnimationDetail();
                                        ProtoReader.NoteObject(expr_21B, protoReader);
                                        themeAnimationDetail = expr_21B;
                                    }
                                    List<EventSelectEvent> list = themeAnimationDetail.InitEventSelectEvents;
                                    List<EventSelectEvent> list2 = list;
                                    if (list == null)
                                    {
                                        list = new List<EventSelectEvent>();
                                    }
                                    int fieldNumber = protoReader.FieldNumber;
                                    do
                                    {
                                        List<EventSelectEvent> arg_25C_0 = list;
                                        EventSelectEvent arg_24F_0 = null;
                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                        EventSelectEvent arg_25C_1 = TierXConfigurationSerializer.Read(arg_24F_0, protoReader);
                                        ProtoReader.EndSubItem(token, protoReader);
                                        arg_25C_0.Add(arg_25C_1);
                                    }
                                    while (protoReader.TryReadFieldHeader(fieldNumber));
                                    list2 = ((list2 == list) ? null : list);
                                    if (list2 != null)
                                    {
                                        themeAnimationDetail.InitEventSelectEvents = list2;
                                    }
                                }
                            }
                            else
                            {
                                if (themeAnimationDetail == null)
                                {
                                    ThemeAnimationDetail expr_1CF = new ThemeAnimationDetail();
                                    ProtoReader.NoteObject(expr_1CF, protoReader);
                                    themeAnimationDetail = expr_1CF;
                                }
                                EligibilityRequirements arg_1E7_0 = themeAnimationDetail.AnimationRequirements;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                EligibilityRequirements arg_1F4_0 = TierXConfigurationSerializer.Read(arg_1E7_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                EligibilityRequirements eligibilityRequirements = arg_1F4_0;
                                if (eligibilityRequirements != null)
                                {
                                    themeAnimationDetail.AnimationRequirements = eligibilityRequirements;
                                }
                            }
                        }
                        else
                        {
                            if (themeAnimationDetail == null)
                            {
                                ThemeAnimationDetail expr_14A = new ThemeAnimationDetail();
                                ProtoReader.NoteObject(expr_14A, protoReader);
                                themeAnimationDetail = expr_14A;
                            }
                            List<SoundEventDetail> list3 = themeAnimationDetail.SoundEvents;
                            List<SoundEventDetail> list4 = list3;
                            if (list3 == null)
                            {
                                list3 = new List<SoundEventDetail>();
                            }
                            int fieldNumber = protoReader.FieldNumber;
                            do
                            {
                                List<SoundEventDetail> arg_18B_0 = list3;
                                SoundEventDetail arg_17E_0 = null;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                SoundEventDetail arg_18B_1 = TierXConfigurationSerializer.Read(arg_17E_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                arg_18B_0.Add(arg_18B_1);
                            }
                            while (protoReader.TryReadFieldHeader(fieldNumber));
                            list4 = ((list4 == list3) ? null : list3);
                            if (list4 != null)
                            {
                                themeAnimationDetail.SoundEvents = list4;
                            }
                        }
                    }
                    else
                    {
                        if (themeAnimationDetail == null)
                        {
                            ThemeAnimationDetail expr_C5 = new ThemeAnimationDetail();
                            ProtoReader.NoteObject(expr_C5, protoReader);
                            themeAnimationDetail = expr_C5;
                        }
                        List<EventSelectEvent> list2 = themeAnimationDetail.EventSelectEvents;
                        List<EventSelectEvent> list = list2;
                        if (list2 == null)
                        {
                            list2 = new List<EventSelectEvent>();
                        }
                        int fieldNumber = protoReader.FieldNumber;
                        do
                        {
                            List<EventSelectEvent> arg_106_0 = list2;
                            EventSelectEvent arg_F9_0 = null;
                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                            EventSelectEvent arg_106_1 = TierXConfigurationSerializer.Read(arg_F9_0, protoReader);
                            ProtoReader.EndSubItem(token, protoReader);
                            arg_106_0.Add(arg_106_1);
                        }
                        while (protoReader.TryReadFieldHeader(fieldNumber));
                        list = ((list == list2) ? null : list2);
                        if (list != null)
                        {
                            themeAnimationDetail.EventSelectEvents = list;
                        }
                    }
                }
                else
                {
                    if (themeAnimationDetail == null)
                    {
                        ThemeAnimationDetail expr_4C = new ThemeAnimationDetail();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        themeAnimationDetail = expr_4C;
                    }
                    List<PinAnimationDetail> list5 = themeAnimationDetail.PinAnimations;
                    List<PinAnimationDetail> list6 = list5;
                    if (list5 == null)
                    {
                        list5 = new List<PinAnimationDetail>();
                    }
                    int fieldNumber = protoReader.FieldNumber;
                    do
                    {
                        List<PinAnimationDetail> arg_87_0 = list5;
                        PinAnimationDetail arg_7A_0 = null;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        PinAnimationDetail arg_87_1 = TierXConfigurationSerializer.Read(arg_7A_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        arg_87_0.Add(arg_87_1);
                    }
                    while (protoReader.TryReadFieldHeader(fieldNumber));
                    list6 = ((list6 == list5) ? null : list5);
                    if (list6 != null)
                    {
                        themeAnimationDetail.PinAnimations = list6;
                    }
                }
            }
            else
            {
                if (themeAnimationDetail == null)
                {
                    ThemeAnimationDetail expr_19 = new ThemeAnimationDetail();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    themeAnimationDetail = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    themeAnimationDetail.Name = text;
                }
            }
        }
        if (themeAnimationDetail == null)
        {
            ThemeAnimationDetail expr_2BF = new ThemeAnimationDetail();
            ProtoReader.NoteObject(expr_2BF, protoReader);
            themeAnimationDetail = expr_2BF;
        }
        return themeAnimationDetail;
    }

    private static void Write(ThemeAnimationsConfiguration themeAnimationsConfiguration, ProtoWriter protoWriter)
    {
        if (themeAnimationsConfiguration.GetType() != typeof(ThemeAnimationsConfiguration))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ThemeAnimationsConfiguration), themeAnimationsConfiguration.GetType());
        }
        List<ThemeAnimationDetail> expr_2D = themeAnimationsConfiguration.Animations;
        if (expr_2D != null)
        {
            List<ThemeAnimationDetail> list = expr_2D;
            foreach (ThemeAnimationDetail arg_52_0 in list)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_52_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_52_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static ThemeAnimationsConfiguration Read(ThemeAnimationsConfiguration themeAnimationsConfiguration, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (themeAnimationsConfiguration == null)
                {
                    ThemeAnimationsConfiguration expr_87 = new ThemeAnimationsConfiguration();
                    ProtoReader.NoteObject(expr_87, protoReader);
                    themeAnimationsConfiguration = expr_87;
                }
                protoReader.SkipField();
            }
            else
            {
                if (themeAnimationsConfiguration == null)
                {
                    ThemeAnimationsConfiguration expr_19 = new ThemeAnimationsConfiguration();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    themeAnimationsConfiguration = expr_19;
                }
                List<ThemeAnimationDetail> list = themeAnimationsConfiguration.Animations;
                List<ThemeAnimationDetail> list2 = list;
                if (list == null)
                {
                    list = new List<ThemeAnimationDetail>();
                }
                int fieldNumber = protoReader.FieldNumber;
                do
                {
                    List<ThemeAnimationDetail> arg_53_0 = list;
                    ThemeAnimationDetail arg_46_0 = null;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    ThemeAnimationDetail arg_53_1 = TierXConfigurationSerializer.Read(arg_46_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    arg_53_0.Add(arg_53_1);
                }
                while (protoReader.TryReadFieldHeader(fieldNumber));
                list2 = ((list2 == list) ? null : list);
                if (list2 != null)
                {
                    themeAnimationsConfiguration.Animations = list2;
                }
            }
        }
        if (themeAnimationsConfiguration == null)
        {
            ThemeAnimationsConfiguration expr_AF = new ThemeAnimationsConfiguration();
            ProtoReader.NoteObject(expr_AF, protoReader);
            themeAnimationsConfiguration = expr_AF;
        }
        return themeAnimationsConfiguration;
    }

    private static void Write(ThemeLayout themeLayout, ProtoWriter protoWriter)
    {
        if (themeLayout.GetType() != typeof(ThemeLayout))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ThemeLayout), themeLayout.GetType());
        }
        string expr_2D = themeLayout.Name;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = themeLayout.Description;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        string expr_67 = themeLayout.ID;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_67, protoWriter);
        }
        string expr_84 = themeLayout.Localisation;
        if (expr_84 != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_84, protoWriter);
        }
        Color arg_B2_0 = themeLayout.Colour;
        ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_B2_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        Color arg_D5_0 = themeLayout.GlowColour;
        ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_D5_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        DifficultyDeltas expr_E7 = themeLayout.DifficultyDeltas;
        if (expr_E7 != null)
        {
            ProtoWriter.WriteFieldHeader(7, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_E7, protoWriter);
            TierXConfigurationSerializer.Write(expr_E7, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        bool arg_11C_0 = themeLayout.IsOverviewTheme;
        ProtoWriter.WriteFieldHeader(8, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_11C_0, protoWriter);
        bool arg_131_0 = themeLayout.ShowTitle;
        ProtoWriter.WriteFieldHeader(9, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_131_0, protoWriter);
        bool arg_146_0 = themeLayout.ShowDescription;
        ProtoWriter.WriteFieldHeader(10, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_146_0, protoWriter);
        bool arg_15B_0 = themeLayout.CheckForProgression;
        ProtoWriter.WriteFieldHeader(11, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_15B_0, protoWriter);
        bool arg_170_0 = themeLayout.AllowForRightJustifiedPins;
        ProtoWriter.WriteFieldHeader(12, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_170_0, protoWriter);
        bool arg_185_0 = themeLayout.UseButtonsForPins;
        ProtoWriter.WriteFieldHeader(13, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_185_0, protoWriter);
        bool arg_19A_0 = themeLayout.ShowEventPane;
        ProtoWriter.WriteFieldHeader(14, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_19A_0, protoWriter);
        bool arg_1AF_0 = themeLayout.ShowObjective;
        ProtoWriter.WriteFieldHeader(15, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_1AF_0, protoWriter);
        bool arg_1C4_0 = themeLayout.ShowTierText;
        ProtoWriter.WriteFieldHeader(16, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_1C4_0, protoWriter);
        bool arg_1D9_0 = themeLayout.ShowSelectedThemeDescription;
        ProtoWriter.WriteFieldHeader(17, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_1D9_0, protoWriter);
        string expr_1E4 = themeLayout.ThemeToLoadOnBackOut;
        if (expr_1E4 != null)
        {
            ProtoWriter.WriteFieldHeader(18, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_1E4, protoWriter);
        }
        bool arg_20C_0 = themeLayout.CanSwipe;
        ProtoWriter.WriteFieldHeader(19, WireType.Variant, protoWriter);
        ProtoWriter.WriteBoolean(arg_20C_0, protoWriter);
        List<TimelinePinDetails> expr_217 = themeLayout.PreviousPinsLayout;
        if (expr_217 != null)
        {
            List<TimelinePinDetails> list = expr_217;
            foreach (TimelinePinDetails arg_23D_0 in list)
            {
                ProtoWriter.WriteFieldHeader(20, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_23D_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_23D_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        List<TimelinePinDetails> expr_27C = themeLayout.NextPinsLayout;
        if (expr_27C != null)
        {
            List<TimelinePinDetails> list = expr_27C;
            foreach (TimelinePinDetails arg_2A2_0 in list)
            {
                ProtoWriter.WriteFieldHeader(21, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_2A2_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_2A2_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        List<PinTemplate> expr_2E1 = themeLayout.PinTemplates;
        if (expr_2E1 != null)
        {
            List<PinTemplate> list2 = expr_2E1;
            foreach (PinTemplate arg_308_0 in list2)
            {
                ProtoWriter.WriteFieldHeader(22, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_308_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_308_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        ThemeProgressionData expr_347 = themeLayout.Progression;
        if (expr_347 != null)
        {
            ProtoWriter.WriteFieldHeader(23, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_347, protoWriter);
            TierXConfigurationSerializer.Write(expr_347, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        List<PinDetail> expr_374 = themeLayout.PinDetails;
        if (expr_374 != null)
        {
            List<PinDetail> list3 = expr_374;
            foreach (PinDetail arg_39D_0 in list3)
            {
                ProtoWriter.WriteFieldHeader(24, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_39D_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_39D_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        List<RestrictionRaceHelperOverride> expr_3DC = themeLayout.RestrictionRaceHelperOverrides;
        if (expr_3DC != null)
        {
            List<RestrictionRaceHelperOverride> list4 = expr_3DC;
            foreach (RestrictionRaceHelperOverride arg_405_0 in list4)
            {
                ProtoWriter.WriteFieldHeader(25, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_405_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_405_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        string[] expr_444 = themeLayout.EventIDsForAnimation;
        if (expr_444 != null)
        {
            string[] array = expr_444;
            for (int i = 0; i < array.Length; i++)
            {
                string expr_45C = array[i];
                if (expr_45C != null)
                {
                    ProtoWriter.WriteFieldHeader(26, WireType.String, protoWriter);
                    ProtoWriter.WriteString(expr_45C, protoWriter);
                }
            }
        }
        string expr_48B = themeLayout.OutroAnimFlag;
        if (expr_48B != null)
        {
            ProtoWriter.WriteFieldHeader(27, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_48B, protoWriter);
        }
        WorldTourProgressLayout expr_4A9 = themeLayout.ProgressLayout;
        if (expr_4A9 != null)
        {
            ProtoWriter.WriteFieldHeader(28, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_4A9, protoWriter);
            TierXConfigurationSerializer.Write(expr_4A9, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        WorldTourBossPinDetais expr_4D6 = themeLayout.BossPinDetails;
        if (expr_4D6 != null)
        {
            ProtoWriter.WriteFieldHeader(29, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_4D6, protoWriter);
            TierXConfigurationSerializer.Write(expr_4D6, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        Dictionary<string, ThemeOptionLayoutDetails> expr_503 = themeLayout.ThemeOptionLayoutDetails;
        if (expr_503 != null)
        {
            Dictionary<string, ThemeOptionLayoutDetails> dictionary = expr_503;
            foreach (KeyValuePair<string, ThemeOptionLayoutDetails> arg_535_0 in dictionary)
            {
                ProtoWriter.WriteFieldHeader(30, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(null, protoWriter);
                TierXConfigurationSerializer.Write(arg_535_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
        PopupData expr_56B = themeLayout.CarAwardPopupData;
        if (expr_56B != null)
        {
            ProtoWriter.WriteFieldHeader(31, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_56B, protoWriter);
            TierXConfigurationSerializer.Write(expr_56B, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        string expr_598 = themeLayout.ThemePrizeCar;
        if (expr_598 != null)
        {
            ProtoWriter.WriteFieldHeader(32, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_598, protoWriter);
        }
        bool expr_5B6 = themeLayout.ThemePrizeCarIsElite;
        if (expr_5B6)
        {
            ProtoWriter.WriteFieldHeader(33, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_5B6, protoWriter);
        }
        List<ThemeTransition> expr_5D4 = themeLayout.Transitions;
        if (expr_5D4 != null)
        {
            List<ThemeTransition> list5 = expr_5D4;
            foreach (ThemeTransition arg_5FD_0 in list5)
            {
                ProtoWriter.WriteFieldHeader(34, WireType.String, protoWriter);
                token = ProtoWriter.StartSubItem(arg_5FD_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_5FD_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static ThemeLayout Read(ThemeLayout themeLayout, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (num != 8)
                                        {
                                            if (num != 9)
                                            {
                                                if (num != 10)
                                                {
                                                    if (num != 11)
                                                    {
                                                        if (num != 12)
                                                        {
                                                            if (num != 13)
                                                            {
                                                                if (num != 14)
                                                                {
                                                                    if (num != 15)
                                                                    {
                                                                        if (num != 16)
                                                                        {
                                                                            if (num != 17)
                                                                            {
                                                                                if (num != 18)
                                                                                {
                                                                                    if (num != 19)
                                                                                    {
                                                                                        if (num != 20)
                                                                                        {
                                                                                            if (num != 21)
                                                                                            {
                                                                                                if (num != 22)
                                                                                                {
                                                                                                    if (num != 23)
                                                                                                    {
                                                                                                        if (num != 24)
                                                                                                        {
                                                                                                            if (num != 25)
                                                                                                            {
                                                                                                                if (num != 26)
                                                                                                                {
                                                                                                                    if (num != 27)
                                                                                                                    {
                                                                                                                        if (num != 28)
                                                                                                                        {
                                                                                                                            if (num != 29)
                                                                                                                            {
                                                                                                                                if (num != 30)
                                                                                                                                {
                                                                                                                                    if (num != 31)
                                                                                                                                    {
                                                                                                                                        if (num != 32)
                                                                                                                                        {
                                                                                                                                            if (num != 33)
                                                                                                                                            {
                                                                                                                                                if (num != 34)
                                                                                                                                                {
                                                                                                                                                    if (themeLayout == null)
                                                                                                                                                    {
                                                                                                                                                        ThemeLayout expr_A18 = new ThemeLayout();
                                                                                                                                                        ProtoReader.NoteObject(expr_A18, protoReader);
                                                                                                                                                        themeLayout = expr_A18;
                                                                                                                                                    }
                                                                                                                                                    protoReader.SkipField();
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (themeLayout == null)
                                                                                                                                                    {
                                                                                                                                                        ThemeLayout expr_99E = new ThemeLayout();
                                                                                                                                                        ProtoReader.NoteObject(expr_99E, protoReader);
                                                                                                                                                        themeLayout = expr_99E;
                                                                                                                                                    }
                                                                                                                                                    List<ThemeTransition> list = themeLayout.Transitions;
                                                                                                                                                    List<ThemeTransition> list2 = list;
                                                                                                                                                    if (list == null)
                                                                                                                                                    {
                                                                                                                                                        list = new List<ThemeTransition>();
                                                                                                                                                    }
                                                                                                                                                    int num2 = protoReader.FieldNumber;
                                                                                                                                                    do
                                                                                                                                                    {
                                                                                                                                                        List<ThemeTransition> arg_9DD_0 = list;
                                                                                                                                                        ThemeTransition arg_9D1_0 = null;
                                                                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                                                        ThemeTransition arg_9DD_1 = TierXConfigurationSerializer.Read(arg_9D1_0, protoReader);
                                                                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                                                                        arg_9DD_0.Add(arg_9DD_1);
                                                                                                                                                    }
                                                                                                                                                    while (protoReader.TryReadFieldHeader(num2));
                                                                                                                                                    list2 = ((list2 == list) ? null : list);
                                                                                                                                                    if (list2 != null)
                                                                                                                                                    {
                                                                                                                                                        themeLayout.Transitions = list2;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (themeLayout == null)
                                                                                                                                                {
                                                                                                                                                    ThemeLayout expr_96B = new ThemeLayout();
                                                                                                                                                    ProtoReader.NoteObject(expr_96B, protoReader);
                                                                                                                                                    themeLayout = expr_96B;
                                                                                                                                                }
                                                                                                                                                bool flag = protoReader.ReadBoolean();
                                                                                                                                                themeLayout.ThemePrizeCarIsElite = flag;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (themeLayout == null)
                                                                                                                                            {
                                                                                                                                                ThemeLayout expr_937 = new ThemeLayout();
                                                                                                                                                ProtoReader.NoteObject(expr_937, protoReader);
                                                                                                                                                themeLayout = expr_937;
                                                                                                                                            }
                                                                                                                                            string text = protoReader.ReadString();
                                                                                                                                            if (text != null)
                                                                                                                                            {
                                                                                                                                                themeLayout.ThemePrizeCar = text;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (themeLayout == null)
                                                                                                                                        {
                                                                                                                                            ThemeLayout expr_8EC = new ThemeLayout();
                                                                                                                                            ProtoReader.NoteObject(expr_8EC, protoReader);
                                                                                                                                            themeLayout = expr_8EC;
                                                                                                                                        }
                                                                                                                                        PopupData arg_903_0 = themeLayout.CarAwardPopupData;
                                                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                                        PopupData arg_90F_0 = TierXConfigurationSerializer.Read(arg_903_0, protoReader);
                                                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                                                        PopupData popupData = arg_90F_0;
                                                                                                                                        if (popupData != null)
                                                                                                                                        {
                                                                                                                                            themeLayout.CarAwardPopupData = popupData;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (themeLayout == null)
                                                                                                                                    {
                                                                                                                                        ThemeLayout expr_85F = new ThemeLayout();
                                                                                                                                        ProtoReader.NoteObject(expr_85F, protoReader);
                                                                                                                                        themeLayout = expr_85F;
                                                                                                                                    }
                                                                                                                                    Dictionary<string, ThemeOptionLayoutDetails> dictionary = themeLayout.ThemeOptionLayoutDetails;
                                                                                                                                    Dictionary<string, ThemeOptionLayoutDetails> dictionary2 = dictionary;
                                                                                                                                    if (dictionary == null)
                                                                                                                                    {
                                                                                                                                        dictionary = new Dictionary<string, ThemeOptionLayoutDetails>();
                                                                                                                                    }
                                                                                                                                    int num2 = protoReader.FieldNumber;
                                                                                                                                    do
                                                                                                                                    {
                                                                                                                                        ICollection<KeyValuePair<string, ThemeOptionLayoutDetails>> arg_8A7_0 = dictionary;
                                                                                                                                        KeyValuePair<string, ThemeOptionLayoutDetails> arg_89B_0 = default(KeyValuePair<string, ThemeOptionLayoutDetails>);
                                                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                                        KeyValuePair<string, ThemeOptionLayoutDetails> arg_8A7_1 = TierXConfigurationSerializer.Read(arg_89B_0, protoReader);
                                                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                                                        arg_8A7_0.Add(arg_8A7_1);
                                                                                                                                    }
                                                                                                                                    while (protoReader.TryReadFieldHeader(num2));
                                                                                                                                    dictionary2 = ((dictionary2 == dictionary) ? null : dictionary);
                                                                                                                                    if (dictionary2 != null)
                                                                                                                                    {
                                                                                                                                        themeLayout.ThemeOptionLayoutDetails = dictionary2;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (themeLayout == null)
                                                                                                                                {
                                                                                                                                    ThemeLayout expr_814 = new ThemeLayout();
                                                                                                                                    ProtoReader.NoteObject(expr_814, protoReader);
                                                                                                                                    themeLayout = expr_814;
                                                                                                                                }
                                                                                                                                WorldTourBossPinDetais arg_82B_0 = themeLayout.BossPinDetails;
                                                                                                                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                                WorldTourBossPinDetais arg_837_0 = TierXConfigurationSerializer.Read(arg_82B_0, protoReader);
                                                                                                                                ProtoReader.EndSubItem(token, protoReader);
                                                                                                                                WorldTourBossPinDetais worldTourBossPinDetais = arg_837_0;
                                                                                                                                if (worldTourBossPinDetais != null)
                                                                                                                                {
                                                                                                                                    themeLayout.BossPinDetails = worldTourBossPinDetais;
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (themeLayout == null)
                                                                                                                            {
                                                                                                                                ThemeLayout expr_7C9 = new ThemeLayout();
                                                                                                                                ProtoReader.NoteObject(expr_7C9, protoReader);
                                                                                                                                themeLayout = expr_7C9;
                                                                                                                            }
                                                                                                                            WorldTourProgressLayout arg_7E0_0 = themeLayout.ProgressLayout;
                                                                                                                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                            WorldTourProgressLayout arg_7EC_0 = TierXConfigurationSerializer.Read(arg_7E0_0, protoReader);
                                                                                                                            ProtoReader.EndSubItem(token, protoReader);
                                                                                                                            WorldTourProgressLayout worldTourProgressLayout = arg_7EC_0;
                                                                                                                            if (worldTourProgressLayout != null)
                                                                                                                            {
                                                                                                                                themeLayout.ProgressLayout = worldTourProgressLayout;
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (themeLayout == null)
                                                                                                                        {
                                                                                                                            ThemeLayout expr_795 = new ThemeLayout();
                                                                                                                            ProtoReader.NoteObject(expr_795, protoReader);
                                                                                                                            themeLayout = expr_795;
                                                                                                                        }
                                                                                                                        string text = protoReader.ReadString();
                                                                                                                        if (text != null)
                                                                                                                        {
                                                                                                                            themeLayout.OutroAnimFlag = text;
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (themeLayout == null)
                                                                                                                    {
                                                                                                                        ThemeLayout expr_6FB = new ThemeLayout();
                                                                                                                        ProtoReader.NoteObject(expr_6FB, protoReader);
                                                                                                                        themeLayout = expr_6FB;
                                                                                                                    }
                                                                                                                    string[] eventIDsForAnimation = themeLayout.EventIDsForAnimation;
                                                                                                                    List<string> list3 = new List<string>();
                                                                                                                    int num2 = protoReader.FieldNumber;
                                                                                                                    do
                                                                                                                    {
                                                                                                                        list3.Add(protoReader.ReadString());
                                                                                                                    }
                                                                                                                    while (protoReader.TryReadFieldHeader(num2));
                                                                                                                    string[] expr_737 = eventIDsForAnimation;
                                                                                                                    string[] array = new string[(num2 = ((expr_737 != null) ? expr_737.Length : 0)) + list3.Count];
                                                                                                                    if (num2 != 0)
                                                                                                                    {
                                                                                                                        eventIDsForAnimation.CopyTo(array, 0);
                                                                                                                    }
                                                                                                                    list3.CopyTo(array, num2);
                                                                                                                    array = array;
                                                                                                                    if (array != null)
                                                                                                                    {
                                                                                                                        themeLayout.EventIDsForAnimation = array;
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (themeLayout == null)
                                                                                                                {
                                                                                                                    ThemeLayout expr_677 = new ThemeLayout();
                                                                                                                    ProtoReader.NoteObject(expr_677, protoReader);
                                                                                                                    themeLayout = expr_677;
                                                                                                                }
                                                                                                                List<RestrictionRaceHelperOverride> list4 = themeLayout.RestrictionRaceHelperOverrides;
                                                                                                                List<RestrictionRaceHelperOverride> list5 = list4;
                                                                                                                if (list4 == null)
                                                                                                                {
                                                                                                                    list4 = new List<RestrictionRaceHelperOverride>();
                                                                                                                }
                                                                                                                int num2 = protoReader.FieldNumber;
                                                                                                                do
                                                                                                                {
                                                                                                                    List<RestrictionRaceHelperOverride> arg_6B6_0 = list4;
                                                                                                                    RestrictionRaceHelperOverride arg_6AA_0 = null;
                                                                                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                    RestrictionRaceHelperOverride arg_6B6_1 = TierXConfigurationSerializer.Read(arg_6AA_0, protoReader);
                                                                                                                    ProtoReader.EndSubItem(token, protoReader);
                                                                                                                    arg_6B6_0.Add(arg_6B6_1);
                                                                                                                }
                                                                                                                while (protoReader.TryReadFieldHeader(num2));
                                                                                                                list5 = ((list5 == list4) ? null : list4);
                                                                                                                if (list5 != null)
                                                                                                                {
                                                                                                                    themeLayout.RestrictionRaceHelperOverrides = list5;
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (themeLayout == null)
                                                                                                            {
                                                                                                                ThemeLayout expr_5F3 = new ThemeLayout();
                                                                                                                ProtoReader.NoteObject(expr_5F3, protoReader);
                                                                                                                themeLayout = expr_5F3;
                                                                                                            }
                                                                                                            List<PinDetail> list6 = themeLayout.PinDetails;
                                                                                                            List<PinDetail> list7 = list6;
                                                                                                            if (list6 == null)
                                                                                                            {
                                                                                                                list6 = new List<PinDetail>();
                                                                                                            }
                                                                                                            int num2 = protoReader.FieldNumber;
                                                                                                            do
                                                                                                            {
                                                                                                                List<PinDetail> arg_632_0 = list6;
                                                                                                                PinDetail arg_626_0 = null;
                                                                                                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                                PinDetail arg_632_1 = TierXConfigurationSerializer.Read(arg_626_0, protoReader);
                                                                                                                ProtoReader.EndSubItem(token, protoReader);
                                                                                                                arg_632_0.Add(arg_632_1);
                                                                                                            }
                                                                                                            while (protoReader.TryReadFieldHeader(num2));
                                                                                                            list7 = ((list7 == list6) ? null : list6);
                                                                                                            if (list7 != null)
                                                                                                            {
                                                                                                                themeLayout.PinDetails = list7;
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (themeLayout == null)
                                                                                                        {
                                                                                                            ThemeLayout expr_5A8 = new ThemeLayout();
                                                                                                            ProtoReader.NoteObject(expr_5A8, protoReader);
                                                                                                            themeLayout = expr_5A8;
                                                                                                        }
                                                                                                        ThemeProgressionData arg_5BF_0 = themeLayout.Progression;
                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                        ThemeProgressionData arg_5CB_0 = TierXConfigurationSerializer.Read(arg_5BF_0, protoReader);
                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                        ThemeProgressionData themeProgressionData = arg_5CB_0;
                                                                                                        if (themeProgressionData != null)
                                                                                                        {
                                                                                                            themeLayout.Progression = themeProgressionData;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (themeLayout == null)
                                                                                                    {
                                                                                                        ThemeLayout expr_524 = new ThemeLayout();
                                                                                                        ProtoReader.NoteObject(expr_524, protoReader);
                                                                                                        themeLayout = expr_524;
                                                                                                    }
                                                                                                    List<PinTemplate> list8 = themeLayout.PinTemplates;
                                                                                                    List<PinTemplate> list9 = list8;
                                                                                                    if (list8 == null)
                                                                                                    {
                                                                                                        list8 = new List<PinTemplate>();
                                                                                                    }
                                                                                                    int num2 = protoReader.FieldNumber;
                                                                                                    do
                                                                                                    {
                                                                                                        List<PinTemplate> arg_563_0 = list8;
                                                                                                        PinTemplate arg_557_0 = null;
                                                                                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                        PinTemplate arg_563_1 = TierXConfigurationSerializer.Read(arg_557_0, protoReader);
                                                                                                        ProtoReader.EndSubItem(token, protoReader);
                                                                                                        arg_563_0.Add(arg_563_1);
                                                                                                    }
                                                                                                    while (protoReader.TryReadFieldHeader(num2));
                                                                                                    list9 = ((list9 == list8) ? null : list8);
                                                                                                    if (list9 != null)
                                                                                                    {
                                                                                                        themeLayout.PinTemplates = list9;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (themeLayout == null)
                                                                                                {
                                                                                                    ThemeLayout expr_4A0 = new ThemeLayout();
                                                                                                    ProtoReader.NoteObject(expr_4A0, protoReader);
                                                                                                    themeLayout = expr_4A0;
                                                                                                }
                                                                                                List<TimelinePinDetails> list10 = themeLayout.NextPinsLayout;
                                                                                                List<TimelinePinDetails> list11 = list10;
                                                                                                if (list10 == null)
                                                                                                {
                                                                                                    list10 = new List<TimelinePinDetails>();
                                                                                                }
                                                                                                int num2 = protoReader.FieldNumber;
                                                                                                do
                                                                                                {
                                                                                                    List<TimelinePinDetails> arg_4DF_0 = list10;
                                                                                                    TimelinePinDetails arg_4D3_0 = null;
                                                                                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                    TimelinePinDetails arg_4DF_1 = TierXConfigurationSerializer.Read(arg_4D3_0, protoReader);
                                                                                                    ProtoReader.EndSubItem(token, protoReader);
                                                                                                    arg_4DF_0.Add(arg_4DF_1);
                                                                                                }
                                                                                                while (protoReader.TryReadFieldHeader(num2));
                                                                                                list11 = ((list11 == list10) ? null : list10);
                                                                                                if (list11 != null)
                                                                                                {
                                                                                                    themeLayout.NextPinsLayout = list11;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (themeLayout == null)
                                                                                            {
                                                                                                ThemeLayout expr_41C = new ThemeLayout();
                                                                                                ProtoReader.NoteObject(expr_41C, protoReader);
                                                                                                themeLayout = expr_41C;
                                                                                            }
                                                                                            List<TimelinePinDetails> list11 = themeLayout.PreviousPinsLayout;
                                                                                            List<TimelinePinDetails> list10 = list11;
                                                                                            if (list11 == null)
                                                                                            {
                                                                                                list11 = new List<TimelinePinDetails>();
                                                                                            }
                                                                                            int num2 = protoReader.FieldNumber;
                                                                                            do
                                                                                            {
                                                                                                List<TimelinePinDetails> arg_45B_0 = list11;
                                                                                                TimelinePinDetails arg_44F_0 = null;
                                                                                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                                                                                TimelinePinDetails arg_45B_1 = TierXConfigurationSerializer.Read(arg_44F_0, protoReader);
                                                                                                ProtoReader.EndSubItem(token, protoReader);
                                                                                                arg_45B_0.Add(arg_45B_1);
                                                                                            }
                                                                                            while (protoReader.TryReadFieldHeader(num2));
                                                                                            list10 = ((list10 == list11) ? null : list11);
                                                                                            if (list10 != null)
                                                                                            {
                                                                                                themeLayout.PreviousPinsLayout = list10;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (themeLayout == null)
                                                                                        {
                                                                                            ThemeLayout expr_3E9 = new ThemeLayout();
                                                                                            ProtoReader.NoteObject(expr_3E9, protoReader);
                                                                                            themeLayout = expr_3E9;
                                                                                        }
                                                                                        bool flag = protoReader.ReadBoolean();
                                                                                        themeLayout.CanSwipe = flag;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (themeLayout == null)
                                                                                    {
                                                                                        ThemeLayout expr_3B5 = new ThemeLayout();
                                                                                        ProtoReader.NoteObject(expr_3B5, protoReader);
                                                                                        themeLayout = expr_3B5;
                                                                                    }
                                                                                    string text = protoReader.ReadString();
                                                                                    if (text != null)
                                                                                    {
                                                                                        themeLayout.ThemeToLoadOnBackOut = text;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (themeLayout == null)
                                                                                {
                                                                                    ThemeLayout expr_382 = new ThemeLayout();
                                                                                    ProtoReader.NoteObject(expr_382, protoReader);
                                                                                    themeLayout = expr_382;
                                                                                }
                                                                                bool flag = protoReader.ReadBoolean();
                                                                                themeLayout.ShowSelectedThemeDescription = flag;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (themeLayout == null)
                                                                            {
                                                                                ThemeLayout expr_34F = new ThemeLayout();
                                                                                ProtoReader.NoteObject(expr_34F, protoReader);
                                                                                themeLayout = expr_34F;
                                                                            }
                                                                            bool flag = protoReader.ReadBoolean();
                                                                            themeLayout.ShowTierText = flag;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (themeLayout == null)
                                                                        {
                                                                            ThemeLayout expr_31C = new ThemeLayout();
                                                                            ProtoReader.NoteObject(expr_31C, protoReader);
                                                                            themeLayout = expr_31C;
                                                                        }
                                                                        bool flag = protoReader.ReadBoolean();
                                                                        themeLayout.ShowObjective = flag;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (themeLayout == null)
                                                                    {
                                                                        ThemeLayout expr_2E9 = new ThemeLayout();
                                                                        ProtoReader.NoteObject(expr_2E9, protoReader);
                                                                        themeLayout = expr_2E9;
                                                                    }
                                                                    bool flag = protoReader.ReadBoolean();
                                                                    themeLayout.ShowEventPane = flag;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (themeLayout == null)
                                                                {
                                                                    ThemeLayout expr_2B6 = new ThemeLayout();
                                                                    ProtoReader.NoteObject(expr_2B6, protoReader);
                                                                    themeLayout = expr_2B6;
                                                                }
                                                                bool flag = protoReader.ReadBoolean();
                                                                themeLayout.UseButtonsForPins = flag;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (themeLayout == null)
                                                            {
                                                                ThemeLayout expr_283 = new ThemeLayout();
                                                                ProtoReader.NoteObject(expr_283, protoReader);
                                                                themeLayout = expr_283;
                                                            }
                                                            bool flag = protoReader.ReadBoolean();
                                                            themeLayout.AllowForRightJustifiedPins = flag;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (themeLayout == null)
                                                        {
                                                            ThemeLayout expr_250 = new ThemeLayout();
                                                            ProtoReader.NoteObject(expr_250, protoReader);
                                                            themeLayout = expr_250;
                                                        }
                                                        bool flag = protoReader.ReadBoolean();
                                                        themeLayout.CheckForProgression = flag;
                                                    }
                                                }
                                                else
                                                {
                                                    if (themeLayout == null)
                                                    {
                                                        ThemeLayout expr_21D = new ThemeLayout();
                                                        ProtoReader.NoteObject(expr_21D, protoReader);
                                                        themeLayout = expr_21D;
                                                    }
                                                    bool flag = protoReader.ReadBoolean();
                                                    themeLayout.ShowDescription = flag;
                                                }
                                            }
                                            else
                                            {
                                                if (themeLayout == null)
                                                {
                                                    ThemeLayout expr_1EA = new ThemeLayout();
                                                    ProtoReader.NoteObject(expr_1EA, protoReader);
                                                    themeLayout = expr_1EA;
                                                }
                                                bool flag = protoReader.ReadBoolean();
                                                themeLayout.ShowTitle = flag;
                                            }
                                        }
                                        else
                                        {
                                            if (themeLayout == null)
                                            {
                                                ThemeLayout expr_1B7 = new ThemeLayout();
                                                ProtoReader.NoteObject(expr_1B7, protoReader);
                                                themeLayout = expr_1B7;
                                            }
                                            bool flag = protoReader.ReadBoolean();
                                            themeLayout.IsOverviewTheme = flag;
                                        }
                                    }
                                    else
                                    {
                                        if (themeLayout == null)
                                        {
                                            ThemeLayout expr_16D = new ThemeLayout();
                                            ProtoReader.NoteObject(expr_16D, protoReader);
                                            themeLayout = expr_16D;
                                        }
                                        DifficultyDeltas arg_184_0 = themeLayout.DifficultyDeltas;
                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                        DifficultyDeltas arg_190_0 = TierXConfigurationSerializer.Read(arg_184_0, protoReader);
                                        ProtoReader.EndSubItem(token, protoReader);
                                        DifficultyDeltas difficultyDeltas = arg_190_0;
                                        if (difficultyDeltas != null)
                                        {
                                            themeLayout.DifficultyDeltas = difficultyDeltas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (themeLayout == null)
                                    {
                                        ThemeLayout expr_129 = new ThemeLayout();
                                        ProtoReader.NoteObject(expr_129, protoReader);
                                        themeLayout = expr_129;
                                    }
                                    Color arg_140_0 = themeLayout.GlowColour;
                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                    Color arg_14C_0 = TierXConfigurationSerializer.Read(arg_140_0, protoReader);
                                    ProtoReader.EndSubItem(token, protoReader);
                                    Color color = arg_14C_0;
                                    themeLayout.GlowColour = color;
                                }
                            }
                            else
                            {
                                if (themeLayout == null)
                                {
                                    ThemeLayout expr_E5 = new ThemeLayout();
                                    ProtoReader.NoteObject(expr_E5, protoReader);
                                    themeLayout = expr_E5;
                                }
                                Color arg_FC_0 = themeLayout.Colour;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                Color arg_108_0 = TierXConfigurationSerializer.Read(arg_FC_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                Color color = arg_108_0;
                                themeLayout.Colour = color;
                            }
                        }
                        else
                        {
                            if (themeLayout == null)
                            {
                                ThemeLayout expr_B2 = new ThemeLayout();
                                ProtoReader.NoteObject(expr_B2, protoReader);
                                themeLayout = expr_B2;
                            }
                            string text = protoReader.ReadString();
                            if (text != null)
                            {
                                themeLayout.Localisation = text;
                            }
                        }
                    }
                    else
                    {
                        if (themeLayout == null)
                        {
                            ThemeLayout expr_7F = new ThemeLayout();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            themeLayout = expr_7F;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            themeLayout.ID = text;
                        }
                    }
                }
                else
                {
                    if (themeLayout == null)
                    {
                        ThemeLayout expr_4C = new ThemeLayout();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        themeLayout = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        themeLayout.Description = text;
                    }
                }
            }
            else
            {
                if (themeLayout == null)
                {
                    ThemeLayout expr_19 = new ThemeLayout();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    themeLayout = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    themeLayout.Name = text;
                }
            }
        }
        if (themeLayout == null)
        {
            ThemeLayout expr_A40 = new ThemeLayout();
            ProtoReader.NoteObject(expr_A40, protoReader);
            themeLayout = expr_A40;
        }
        return themeLayout;
    }

    private static void Write(ThemeOptionLayoutDetails themeOptionLayoutDetails, ProtoWriter protoWriter)
    {
        if (themeOptionLayoutDetails.GetType() != typeof(ThemeOptionLayoutDetails))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ThemeOptionLayoutDetails), themeOptionLayoutDetails.GetType());
        }
        Color arg_3E_0 = themeOptionLayoutDetails.Colour;
        ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_3E_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        string expr_50 = themeOptionLayoutDetails.Background;
        if (expr_50 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_50, protoWriter);
        }
        string expr_6D = themeOptionLayoutDetails.Title;
        if (expr_6D != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_6D, protoWriter);
        }
        ConditionallySelectedString expr_8A = themeOptionLayoutDetails.ProgressionText;
        if (expr_8A != null)
        {
            ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_8A, protoWriter);
            TierXConfigurationSerializer.Write(expr_8A, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        ConditionallySelectedString expr_B6 = themeOptionLayoutDetails.EventNameText;
        if (expr_B6 != null)
        {
            ProtoWriter.WriteFieldHeader(5, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_B6, protoWriter);
            TierXConfigurationSerializer.Write(expr_B6, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        ConditionallySelectedSnapshotList expr_E2 = themeOptionLayoutDetails.ProgressionSnapshots;
        if (expr_E2 != null)
        {
            ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
            token = ProtoWriter.StartSubItem(expr_E2, protoWriter);
            TierXConfigurationSerializer.Write(expr_E2, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        bool expr_10E = themeOptionLayoutDetails.UseBackgroundGlow;
        if (expr_10E)
        {
            ProtoWriter.WriteFieldHeader(7, WireType.Variant, protoWriter);
            ProtoWriter.WriteBoolean(expr_10E, protoWriter);
        }
    }

    private static ThemeOptionLayoutDetails Read(ThemeOptionLayoutDetails themeOptionLayoutDetails, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (num != 5)
                            {
                                if (num != 6)
                                {
                                    if (num != 7)
                                    {
                                        if (themeOptionLayoutDetails == null)
                                        {
                                            ThemeOptionLayoutDetails expr_1CA = new ThemeOptionLayoutDetails();
                                            ProtoReader.NoteObject(expr_1CA, protoReader);
                                            themeOptionLayoutDetails = expr_1CA;
                                        }
                                        protoReader.SkipField();
                                    }
                                    else
                                    {
                                        if (themeOptionLayoutDetails == null)
                                        {
                                            ThemeOptionLayoutDetails expr_1A1 = new ThemeOptionLayoutDetails();
                                            ProtoReader.NoteObject(expr_1A1, protoReader);
                                            themeOptionLayoutDetails = expr_1A1;
                                        }
                                        bool useBackgroundGlow = protoReader.ReadBoolean();
                                        themeOptionLayoutDetails.UseBackgroundGlow = useBackgroundGlow;
                                    }
                                }
                                else
                                {
                                    if (themeOptionLayoutDetails == null)
                                    {
                                        ThemeOptionLayoutDetails expr_157 = new ThemeOptionLayoutDetails();
                                        ProtoReader.NoteObject(expr_157, protoReader);
                                        themeOptionLayoutDetails = expr_157;
                                    }
                                    ConditionallySelectedSnapshotList arg_16E_0 = themeOptionLayoutDetails.ProgressionSnapshots;
                                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                    ConditionallySelectedSnapshotList arg_17A_0 = TierXConfigurationSerializer.Read(arg_16E_0, protoReader);
                                    ProtoReader.EndSubItem(token, protoReader);
                                    ConditionallySelectedSnapshotList conditionallySelectedSnapshotList = arg_17A_0;
                                    if (conditionallySelectedSnapshotList != null)
                                    {
                                        themeOptionLayoutDetails.ProgressionSnapshots = conditionallySelectedSnapshotList;
                                    }
                                }
                            }
                            else
                            {
                                if (themeOptionLayoutDetails == null)
                                {
                                    ThemeOptionLayoutDetails expr_10D = new ThemeOptionLayoutDetails();
                                    ProtoReader.NoteObject(expr_10D, protoReader);
                                    themeOptionLayoutDetails = expr_10D;
                                }
                                ConditionallySelectedString arg_124_0 = themeOptionLayoutDetails.EventNameText;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                ConditionallySelectedString arg_130_0 = TierXConfigurationSerializer.Read(arg_124_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                ConditionallySelectedString conditionallySelectedString = arg_130_0;
                                if (conditionallySelectedString != null)
                                {
                                    themeOptionLayoutDetails.EventNameText = conditionallySelectedString;
                                }
                            }
                        }
                        else
                        {
                            if (themeOptionLayoutDetails == null)
                            {
                                ThemeOptionLayoutDetails expr_C3 = new ThemeOptionLayoutDetails();
                                ProtoReader.NoteObject(expr_C3, protoReader);
                                themeOptionLayoutDetails = expr_C3;
                            }
                            ConditionallySelectedString arg_DA_0 = themeOptionLayoutDetails.ProgressionText;
                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                            ConditionallySelectedString arg_E6_0 = TierXConfigurationSerializer.Read(arg_DA_0, protoReader);
                            ProtoReader.EndSubItem(token, protoReader);
                            ConditionallySelectedString conditionallySelectedString = arg_E6_0;
                            if (conditionallySelectedString != null)
                            {
                                themeOptionLayoutDetails.ProgressionText = conditionallySelectedString;
                            }
                        }
                    }
                    else
                    {
                        if (themeOptionLayoutDetails == null)
                        {
                            ThemeOptionLayoutDetails expr_90 = new ThemeOptionLayoutDetails();
                            ProtoReader.NoteObject(expr_90, protoReader);
                            themeOptionLayoutDetails = expr_90;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            themeOptionLayoutDetails.Title = text;
                        }
                    }
                }
                else
                {
                    if (themeOptionLayoutDetails == null)
                    {
                        ThemeOptionLayoutDetails expr_5D = new ThemeOptionLayoutDetails();
                        ProtoReader.NoteObject(expr_5D, protoReader);
                        themeOptionLayoutDetails = expr_5D;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        themeOptionLayoutDetails.Background = text;
                    }
                }
            }
            else
            {
                if (themeOptionLayoutDetails == null)
                {
                    ThemeOptionLayoutDetails expr_19 = new ThemeOptionLayoutDetails();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    themeOptionLayoutDetails = expr_19;
                }
                Color arg_30_0 = themeOptionLayoutDetails.Colour;
                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                Color arg_3C_0 = TierXConfigurationSerializer.Read(arg_30_0, protoReader);
                ProtoReader.EndSubItem(token, protoReader);
                Color colour = arg_3C_0;
                themeOptionLayoutDetails.Colour = colour;
            }
        }
        if (themeOptionLayoutDetails == null)
        {
            ThemeOptionLayoutDetails expr_1F2 = new ThemeOptionLayoutDetails();
            ProtoReader.NoteObject(expr_1F2, protoReader);
            themeOptionLayoutDetails = expr_1F2;
        }
        return themeOptionLayoutDetails;
    }

    private static void Write(ThemeProgressionData themeProgressionData, ProtoWriter protoWriter)
    {
        if (themeProgressionData.GetType() != typeof(ThemeProgressionData))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ThemeProgressionData), themeProgressionData.GetType());
        }
        NarrativeSceneForEventData expr_2D = themeProgressionData.IntroNarrative;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_2D, protoWriter);
            TierXConfigurationSerializer.Write(expr_2D, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
        EligibilityRequirements expr_59 = themeProgressionData.IncreaseThemeCompletionLevelRequirements;
        if (expr_59 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_59, protoWriter);
            TierXConfigurationSerializer.Write(expr_59, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static ThemeProgressionData Read(ThemeProgressionData themeProgressionData, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (themeProgressionData == null)
                    {
                        ThemeProgressionData expr_9E = new ThemeProgressionData();
                        ProtoReader.NoteObject(expr_9E, protoReader);
                        themeProgressionData = expr_9E;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (themeProgressionData == null)
                    {
                        ThemeProgressionData expr_60 = new ThemeProgressionData();
                        ProtoReader.NoteObject(expr_60, protoReader);
                        themeProgressionData = expr_60;
                    }
                    EligibilityRequirements arg_77_0 = themeProgressionData.IncreaseThemeCompletionLevelRequirements;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    EligibilityRequirements arg_83_0 = TierXConfigurationSerializer.Read(arg_77_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    EligibilityRequirements eligibilityRequirements = arg_83_0;
                    if (eligibilityRequirements != null)
                    {
                        themeProgressionData.IncreaseThemeCompletionLevelRequirements = eligibilityRequirements;
                    }
                }
            }
            else
            {
                if (themeProgressionData == null)
                {
                    ThemeProgressionData expr_19 = new ThemeProgressionData();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    themeProgressionData = expr_19;
                }
                NarrativeSceneForEventData arg_30_0 = themeProgressionData.IntroNarrative;
                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                NarrativeSceneForEventData arg_3C_0 = TierXConfigurationSerializer.Read(arg_30_0, protoReader);
                ProtoReader.EndSubItem(token, protoReader);
                NarrativeSceneForEventData narrativeSceneForEventData = arg_3C_0;
                if (narrativeSceneForEventData != null)
                {
                    themeProgressionData.IntroNarrative = narrativeSceneForEventData;
                }
            }
        }
        if (themeProgressionData == null)
        {
            ThemeProgressionData expr_C6 = new ThemeProgressionData();
            ProtoReader.NoteObject(expr_C6, protoReader);
            themeProgressionData = expr_C6;
        }
        return themeProgressionData;
    }

    private static void Write(ThemeTransition themeTransition, ProtoWriter protoWriter)
    {
        if (themeTransition.GetType() != typeof(ThemeTransition))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(ThemeTransition), themeTransition.GetType());
        }
        string expr_2D = themeTransition.ThemeID;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = themeTransition.Option;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        EligibilityRequirements expr_67 = themeTransition.TransitionRequirements;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_67, protoWriter);
            TierXConfigurationSerializer.Write(expr_67, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static ThemeTransition Read(ThemeTransition themeTransition, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (themeTransition == null)
                        {
                            ThemeTransition expr_BD = new ThemeTransition();
                            ProtoReader.NoteObject(expr_BD, protoReader);
                            themeTransition = expr_BD;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (themeTransition == null)
                        {
                            ThemeTransition expr_7F = new ThemeTransition();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            themeTransition = expr_7F;
                        }
                        EligibilityRequirements arg_96_0 = themeTransition.TransitionRequirements;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        EligibilityRequirements arg_A2_0 = TierXConfigurationSerializer.Read(arg_96_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        EligibilityRequirements eligibilityRequirements = arg_A2_0;
                        if (eligibilityRequirements != null)
                        {
                            themeTransition.TransitionRequirements = eligibilityRequirements;
                        }
                    }
                }
                else
                {
                    if (themeTransition == null)
                    {
                        ThemeTransition expr_4C = new ThemeTransition();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        themeTransition = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        themeTransition.Option = text;
                    }
                }
            }
            else
            {
                if (themeTransition == null)
                {
                    ThemeTransition expr_19 = new ThemeTransition();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    themeTransition = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    themeTransition.ThemeID = text;
                }
            }
        }
        if (themeTransition == null)
        {
            ThemeTransition expr_E5 = new ThemeTransition();
            ProtoReader.NoteObject(expr_E5, protoReader);
            themeTransition = expr_E5;
        }
        return themeTransition;
    }

    private static void Write(TimelinePinDetails timelinePinDetails, ProtoWriter protoWriter)
    {
        if (timelinePinDetails.GetType() != typeof(TimelinePinDetails))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(TimelinePinDetails), timelinePinDetails.GetType());
        }
        Vector2 arg_3E_0 = timelinePinDetails.Position;
        ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_3E_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        float arg_59_0 = timelinePinDetails.Alpha;
        ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, protoWriter);
        ProtoWriter.WriteSingle(arg_59_0, protoWriter);
        float expr_64 = timelinePinDetails.Greyness;
        if (expr_64 != 0f)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.Fixed32, protoWriter);
            ProtoWriter.WriteSingle(expr_64, protoWriter);
        }
        Vector2 arg_97_0 = timelinePinDetails.Scale;
        ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_97_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
    }

    private static TimelinePinDetails Read(TimelinePinDetails timelinePinDetails, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (timelinePinDetails == null)
                            {
                                TimelinePinDetails expr_F8 = new TimelinePinDetails();
                                ProtoReader.NoteObject(expr_F8, protoReader);
                                timelinePinDetails = expr_F8;
                            }
                            protoReader.SkipField();
                        }
                        else
                        {
                            if (timelinePinDetails == null)
                            {
                                TimelinePinDetails expr_BD = new TimelinePinDetails();
                                ProtoReader.NoteObject(expr_BD, protoReader);
                                timelinePinDetails = expr_BD;
                            }
                            Vector2 arg_D4_0 = timelinePinDetails.Scale;
                            SubItemToken token = ProtoReader.StartSubItem(protoReader);
                            Vector2 arg_E0_0 = TierXConfigurationSerializer.Read(arg_D4_0, protoReader);
                            ProtoReader.EndSubItem(token, protoReader);
                            Vector2 vector = arg_E0_0;
                            timelinePinDetails.Scale = vector;
                        }
                    }
                    else
                    {
                        if (timelinePinDetails == null)
                        {
                            TimelinePinDetails expr_8D = new TimelinePinDetails();
                            ProtoReader.NoteObject(expr_8D, protoReader);
                            timelinePinDetails = expr_8D;
                        }
                        float num2 = protoReader.ReadSingle();
                        timelinePinDetails.Greyness = num2;
                    }
                }
                else
                {
                    if (timelinePinDetails == null)
                    {
                        TimelinePinDetails expr_5D = new TimelinePinDetails();
                        ProtoReader.NoteObject(expr_5D, protoReader);
                        timelinePinDetails = expr_5D;
                    }
                    float num2 = protoReader.ReadSingle();
                    timelinePinDetails.Alpha = num2;
                }
            }
            else
            {
                if (timelinePinDetails == null)
                {
                    TimelinePinDetails expr_19 = new TimelinePinDetails();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    timelinePinDetails = expr_19;
                }
                Vector2 arg_30_0 = timelinePinDetails.Position;
                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                Vector2 arg_3C_0 = TierXConfigurationSerializer.Read(arg_30_0, protoReader);
                ProtoReader.EndSubItem(token, protoReader);
                Vector2 vector = arg_3C_0;
                timelinePinDetails.Position = vector;
            }
        }
        if (timelinePinDetails == null)
        {
            TimelinePinDetails expr_120 = new TimelinePinDetails();
            ProtoReader.NoteObject(expr_120, protoReader);
            timelinePinDetails = expr_120;
        }
        return timelinePinDetails;
    }

    private static void Write(Vector2 vector, ProtoWriter writer)
    {
        float arg_10_0 = vector.x;
        ProtoWriter.WriteFieldHeader(1, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_10_0, writer);
        float arg_25_0 = vector.y;
        ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_25_0, writer);
    }

    private static Vector2 Read(Vector2 result, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    protoReader.SkipField();
                }
                else
                {
                    float num2 = protoReader.ReadSingle();
                    result.y = num2;
                }
            }
            else
            {
                float num2 = protoReader.ReadSingle();
                result.x = num2;
            }
        }
        return result;
    }

    private static void Write(Vector3 vector, ProtoWriter writer)
    {
        float arg_10_0 = vector.x;
        ProtoWriter.WriteFieldHeader(1, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_10_0, writer);
        float arg_25_0 = vector.y;
        ProtoWriter.WriteFieldHeader(2, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_25_0, writer);
        float arg_3A_0 = vector.z;
        ProtoWriter.WriteFieldHeader(3, WireType.Fixed32, writer);
        ProtoWriter.WriteSingle(arg_3A_0, writer);
    }

    private static Vector3 Read(Vector3 result, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        protoReader.SkipField();
                    }
                    else
                    {
                        float num2 = protoReader.ReadSingle();
                        result.z = num2;
                    }
                }
                else
                {
                    float num2 = protoReader.ReadSingle();
                    result.y = num2;
                }
            }
            else
            {
                float num2 = protoReader.ReadSingle();
                result.x = num2;
            }
        }
        return result;
    }

    private static void Write(WorldTourBossPinDetais worldTourBossPinDetais, ProtoWriter protoWriter)
    {
        if (worldTourBossPinDetais.GetType() != typeof(WorldTourBossPinDetais))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(WorldTourBossPinDetais), worldTourBossPinDetais.GetType());
        }
        string expr_2D = worldTourBossPinDetais.PieceTexture;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        WorldTourBossPinPieceDetails[] expr_4A = worldTourBossPinDetais.PinDetails;
        if (expr_4A != null)
        {
            WorldTourBossPinPieceDetails[] array = expr_4A;
            for (int i = 0; i < array.Length; i++)
            {
                WorldTourBossPinPieceDetails expr_5E = array[i];
                if (expr_5E != null)
                {
                    ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
                    SubItemToken token = ProtoWriter.StartSubItem(expr_5E, protoWriter);
                    TierXConfigurationSerializer.Write(expr_5E, protoWriter);
                    ProtoWriter.EndSubItem(token, protoWriter);
                }
            }
        }
    }

    private static WorldTourBossPinDetais Read(WorldTourBossPinDetais worldTourBossPinDetais, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (worldTourBossPinDetais == null)
                    {
                        WorldTourBossPinDetais expr_E3 = new WorldTourBossPinDetais();
                        ProtoReader.NoteObject(expr_E3, protoReader);
                        worldTourBossPinDetais = expr_E3;
                    }
                    protoReader.SkipField();
                }
                else
                {
                    if (worldTourBossPinDetais == null)
                    {
                        WorldTourBossPinDetais expr_4C = new WorldTourBossPinDetais();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        worldTourBossPinDetais = expr_4C;
                    }
                    WorldTourBossPinPieceDetails[] pinDetails = worldTourBossPinDetais.PinDetails;
                    List<WorldTourBossPinPieceDetails> list = new List<WorldTourBossPinPieceDetails>();
                    int num2 = protoReader.FieldNumber;
                    do
                    {
                        List<WorldTourBossPinPieceDetails> arg_84_0 = list;
                        WorldTourBossPinPieceDetails arg_77_0 = null;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        WorldTourBossPinPieceDetails arg_84_1 = TierXConfigurationSerializer.Read(arg_77_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        arg_84_0.Add(arg_84_1);
                    }
                    while (protoReader.TryReadFieldHeader(num2));
                    WorldTourBossPinPieceDetails[] expr_97 = pinDetails;
                    WorldTourBossPinPieceDetails[] array = new WorldTourBossPinPieceDetails[(num2 = ((expr_97 != null) ? expr_97.Length : 0)) + list.Count];
                    if (num2 != 0)
                    {
                        pinDetails.CopyTo(array, 0);
                    }
                    list.CopyTo(array, num2);
                    array = array;
                    if (array != null)
                    {
                        worldTourBossPinDetais.PinDetails = array;
                    }
                }
            }
            else
            {
                if (worldTourBossPinDetais == null)
                {
                    WorldTourBossPinDetais expr_19 = new WorldTourBossPinDetais();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    worldTourBossPinDetais = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    worldTourBossPinDetais.PieceTexture = text;
                }
            }
        }
        if (worldTourBossPinDetais == null)
        {
            WorldTourBossPinDetais expr_10B = new WorldTourBossPinDetais();
            ProtoReader.NoteObject(expr_10B, protoReader);
            worldTourBossPinDetais = expr_10B;
        }
        return worldTourBossPinDetais;
    }

    private static void Write(WorldTourBossPinPieceDetails worldTourBossPinPieceDetails, ProtoWriter protoWriter)
    {
        if (worldTourBossPinPieceDetails.GetType() != typeof(WorldTourBossPinPieceDetails))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(WorldTourBossPinPieceDetails), worldTourBossPinPieceDetails.GetType());
        }
        Vector2 arg_3E_0 = worldTourBossPinPieceDetails.StartPosition;
        ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
        SubItemToken token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_3E_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        Vector2 arg_61_0 = worldTourBossPinPieceDetails.EndPosition;
        ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
        token = ProtoWriter.StartSubItem(null, protoWriter);
        TierXConfigurationSerializer.Write(arg_61_0, protoWriter);
        ProtoWriter.EndSubItem(token, protoWriter);
        string expr_73 = worldTourBossPinPieceDetails.SequenceID;
        if (expr_73 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_73, protoWriter);
        }
    }

    private static WorldTourBossPinPieceDetails Read(WorldTourBossPinPieceDetails worldTourBossPinPieceDetails, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (worldTourBossPinPieceDetails == null)
                        {
                            WorldTourBossPinPieceDetails expr_CB = new WorldTourBossPinPieceDetails();
                            ProtoReader.NoteObject(expr_CB, protoReader);
                            worldTourBossPinPieceDetails = expr_CB;
                        }
                        protoReader.SkipField();
                    }
                    else
                    {
                        if (worldTourBossPinPieceDetails == null)
                        {
                            WorldTourBossPinPieceDetails expr_A1 = new WorldTourBossPinPieceDetails();
                            ProtoReader.NoteObject(expr_A1, protoReader);
                            worldTourBossPinPieceDetails = expr_A1;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            worldTourBossPinPieceDetails.SequenceID = text;
                        }
                    }
                }
                else
                {
                    if (worldTourBossPinPieceDetails == null)
                    {
                        WorldTourBossPinPieceDetails expr_5D = new WorldTourBossPinPieceDetails();
                        ProtoReader.NoteObject(expr_5D, protoReader);
                        worldTourBossPinPieceDetails = expr_5D;
                    }
                    Vector2 arg_74_0 = worldTourBossPinPieceDetails.EndPosition;
                    SubItemToken token = ProtoReader.StartSubItem(protoReader);
                    Vector2 arg_80_0 = TierXConfigurationSerializer.Read(arg_74_0, protoReader);
                    ProtoReader.EndSubItem(token, protoReader);
                    Vector2 vector = arg_80_0;
                    worldTourBossPinPieceDetails.EndPosition = vector;
                }
            }
            else
            {
                if (worldTourBossPinPieceDetails == null)
                {
                    WorldTourBossPinPieceDetails expr_19 = new WorldTourBossPinPieceDetails();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    worldTourBossPinPieceDetails = expr_19;
                }
                Vector2 arg_30_0 = worldTourBossPinPieceDetails.StartPosition;
                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                Vector2 arg_3C_0 = TierXConfigurationSerializer.Read(arg_30_0, protoReader);
                ProtoReader.EndSubItem(token, protoReader);
                Vector2 vector = arg_3C_0;
                worldTourBossPinPieceDetails.StartPosition = vector;
            }
        }
        if (worldTourBossPinPieceDetails == null)
        {
            WorldTourBossPinPieceDetails expr_F3 = new WorldTourBossPinPieceDetails();
            ProtoReader.NoteObject(expr_F3, protoReader);
            worldTourBossPinPieceDetails = expr_F3;
        }
        return worldTourBossPinPieceDetails;
    }

    private static void Write(WorldTourProgressLayout worldTourProgressLayout, ProtoWriter protoWriter)
    {
        if (worldTourProgressLayout.GetType() != typeof(WorldTourProgressLayout))
        {
            TypeModel.ThrowUnexpectedSubtype(typeof(WorldTourProgressLayout), worldTourProgressLayout.GetType());
        }
        string expr_2D = worldTourProgressLayout.ProgressText;
        if (expr_2D != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_2D, protoWriter);
        }
        string expr_4A = worldTourProgressLayout.CrewLeaderName;
        if (expr_4A != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_4A, protoWriter);
        }
        string expr_67 = worldTourProgressLayout.CrewLeaderEvent;
        if (expr_67 != null)
        {
            ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_67, protoWriter);
        }
        List<CrewMemberLayout> expr_84 = worldTourProgressLayout.CrewMembers;
        if (expr_84 != null)
        {
            List<CrewMemberLayout> list = expr_84;
            foreach (CrewMemberLayout arg_A9_0 in list)
            {
                ProtoWriter.WriteFieldHeader(4, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(arg_A9_0, protoWriter);
                TierXConfigurationSerializer.Write(arg_A9_0, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }
    }

    private static WorldTourProgressLayout Read(WorldTourProgressLayout worldTourProgressLayout, ProtoReader protoReader)
    {
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            if (num != 1)
            {
                if (num != 2)
                {
                    if (num != 3)
                    {
                        if (num != 4)
                        {
                            if (worldTourProgressLayout == null)
                            {
                                WorldTourProgressLayout expr_122 = new WorldTourProgressLayout();
                                ProtoReader.NoteObject(expr_122, protoReader);
                                worldTourProgressLayout = expr_122;
                            }
                            protoReader.SkipField();
                        }
                        else
                        {
                            if (worldTourProgressLayout == null)
                            {
                                WorldTourProgressLayout expr_B2 = new WorldTourProgressLayout();
                                ProtoReader.NoteObject(expr_B2, protoReader);
                                worldTourProgressLayout = expr_B2;
                            }
                            List<CrewMemberLayout> list = worldTourProgressLayout.CrewMembers;
                            List<CrewMemberLayout> list2 = list;
                            if (list == null)
                            {
                                list = new List<CrewMemberLayout>();
                            }
                            int fieldNumber = protoReader.FieldNumber;
                            do
                            {
                                List<CrewMemberLayout> arg_ED_0 = list;
                                CrewMemberLayout arg_E0_0 = null;
                                SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                CrewMemberLayout arg_ED_1 = TierXConfigurationSerializer.Read(arg_E0_0, protoReader);
                                ProtoReader.EndSubItem(token, protoReader);
                                arg_ED_0.Add(arg_ED_1);
                            }
                            while (protoReader.TryReadFieldHeader(fieldNumber));
                            list2 = ((list2 == list) ? null : list);
                            if (list2 != null)
                            {
                                worldTourProgressLayout.CrewMembers = list2;
                            }
                        }
                    }
                    else
                    {
                        if (worldTourProgressLayout == null)
                        {
                            WorldTourProgressLayout expr_7F = new WorldTourProgressLayout();
                            ProtoReader.NoteObject(expr_7F, protoReader);
                            worldTourProgressLayout = expr_7F;
                        }
                        string text = protoReader.ReadString();
                        if (text != null)
                        {
                            worldTourProgressLayout.CrewLeaderEvent = text;
                        }
                    }
                }
                else
                {
                    if (worldTourProgressLayout == null)
                    {
                        WorldTourProgressLayout expr_4C = new WorldTourProgressLayout();
                        ProtoReader.NoteObject(expr_4C, protoReader);
                        worldTourProgressLayout = expr_4C;
                    }
                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        worldTourProgressLayout.CrewLeaderName = text;
                    }
                }
            }
            else
            {
                if (worldTourProgressLayout == null)
                {
                    WorldTourProgressLayout expr_19 = new WorldTourProgressLayout();
                    ProtoReader.NoteObject(expr_19, protoReader);
                    worldTourProgressLayout = expr_19;
                }
                string text = protoReader.ReadString();
                if (text != null)
                {
                    worldTourProgressLayout.ProgressText = text;
                }
            }
        }
        if (worldTourProgressLayout == null)
        {
            WorldTourProgressLayout expr_14A = new WorldTourProgressLayout();
            ProtoReader.NoteObject(expr_14A, protoReader);
            worldTourProgressLayout = expr_14A;
        }
        return worldTourProgressLayout;
    }

    private static void Write(KeyValuePair<string, TextureDetail> keyValuePair, ProtoWriter protoWriter)
    {
        string expr_07 = keyValuePair.Key;
        if (expr_07 != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_07, protoWriter);
        }
        TextureDetail expr_25 = keyValuePair.Value;
        if (expr_25 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_25, protoWriter);
            TierXConfigurationSerializer.Write(expr_25, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static KeyValuePair<string, TextureDetail> Read(KeyValuePair<string, TextureDetail> result, ProtoReader protoReader)
    {
        string key = result.Key;
        TextureDetail textureDetail = result.Value;
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            switch (num)
            {
                case 1:
                    {
                        string expr_30 = protoReader.ReadString();
                        if (expr_30 != null)
                        {
                            key = expr_30;
                        }
                        break;
                    }
                case 2:
                    {
                        TextureDetail arg_45_0 = textureDetail;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        TextureDetail arg_51_0 = TierXConfigurationSerializer.Read(arg_45_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        if (arg_51_0 != null)
                        {
                            textureDetail = arg_51_0;
                        }
                        break;
                    }
                default:
                    protoReader.SkipField();
                    break;
            }
        }
        result = new KeyValuePair<string, TextureDetail>(key, textureDetail);
        return result;
    }

    private static void Write(KeyValuePair<string, ConditionallySelectedString> keyValuePair, ProtoWriter protoWriter)
    {
        string expr_07 = keyValuePair.Key;
        if (expr_07 != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_07, protoWriter);
        }
        ConditionallySelectedString expr_25 = keyValuePair.Value;
        if (expr_25 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_25, protoWriter);
            TierXConfigurationSerializer.Write(expr_25, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static KeyValuePair<string, ConditionallySelectedString> Read(KeyValuePair<string, ConditionallySelectedString> result, ProtoReader protoReader)
    {
        string key = result.Key;
        ConditionallySelectedString conditionallySelectedString = result.Value;
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            switch (num)
            {
                case 1:
                    {
                        string expr_30 = protoReader.ReadString();
                        if (expr_30 != null)
                        {
                            key = expr_30;
                        }
                        break;
                    }
                case 2:
                    {
                        ConditionallySelectedString arg_45_0 = conditionallySelectedString;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        ConditionallySelectedString arg_51_0 = TierXConfigurationSerializer.Read(arg_45_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        if (arg_51_0 != null)
                        {
                            conditionallySelectedString = arg_51_0;
                        }
                        break;
                    }
                default:
                    protoReader.SkipField();
                    break;
            }
        }
        result = new KeyValuePair<string, ConditionallySelectedString>(key, conditionallySelectedString);
        return result;
    }

    private static void Write(KeyValuePair<string, PopupData> keyValuePair, ProtoWriter protoWriter)
    {
        string expr_07 = keyValuePair.Key;
        if (expr_07 != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_07, protoWriter);
        }
        PopupData expr_25 = keyValuePair.Value;
        if (expr_25 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_25, protoWriter);
            TierXConfigurationSerializer.Write(expr_25, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static KeyValuePair<string, PopupData> Read(KeyValuePair<string, PopupData> result, ProtoReader protoReader)
    {
        string key = result.Key;
        PopupData popupData = result.Value;
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            switch (num)
            {
                case 1:
                    {
                        string expr_30 = protoReader.ReadString();
                        if (expr_30 != null)
                        {
                            key = expr_30;
                        }
                        break;
                    }
                case 2:
                    {
                        PopupData arg_45_0 = popupData;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        PopupData arg_51_0 = TierXConfigurationSerializer.Read(arg_45_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        if (arg_51_0 != null)
                        {
                            popupData = arg_51_0;
                        }
                        break;
                    }
                default:
                    protoReader.SkipField();
                    break;
            }
        }
        result = new KeyValuePair<string, PopupData>(key, popupData);
        return result;
    }

    private static void Write(KeyValuePair<string, ThemeOptionLayoutDetails> keyValuePair, ProtoWriter protoWriter)
    {
        string expr_07 = keyValuePair.Key;
        if (expr_07 != null)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
            ProtoWriter.WriteString(expr_07, protoWriter);
        }
        ThemeOptionLayoutDetails expr_25 = keyValuePair.Value;
        if (expr_25 != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, protoWriter);
            SubItemToken token = ProtoWriter.StartSubItem(expr_25, protoWriter);
            TierXConfigurationSerializer.Write(expr_25, protoWriter);
            ProtoWriter.EndSubItem(token, protoWriter);
        }
    }

    private static KeyValuePair<string, ThemeOptionLayoutDetails> Read(KeyValuePair<string, ThemeOptionLayoutDetails> result, ProtoReader protoReader)
    {
        string key = result.Key;
        ThemeOptionLayoutDetails themeOptionLayoutDetails = result.Value;
        int num;
        while ((num = protoReader.ReadFieldHeader()) > 0)
        {
            switch (num)
            {
                case 1:
                    {
                        string expr_30 = protoReader.ReadString();
                        if (expr_30 != null)
                        {
                            key = expr_30;
                        }
                        break;
                    }
                case 2:
                    {
                        ThemeOptionLayoutDetails arg_45_0 = themeOptionLayoutDetails;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        ThemeOptionLayoutDetails arg_51_0 = TierXConfigurationSerializer.Read(arg_45_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        if (arg_51_0 != null)
                        {
                            themeOptionLayoutDetails = arg_51_0;
                        }
                        break;
                    }
                default:
                    protoReader.SkipField();
                    break;
            }
        }
        result = new KeyValuePair<string, ThemeOptionLayoutDetails>(key, themeOptionLayoutDetails);
        return result;
    }

    protected override int GetKeyImpl(Type key)
    {
        int result;
        if (TierXConfigurationSerializer.knownTypes.TryGetValue(key, out result))
        {
            return result;
        }
        return -1;
    }

    protected  override void Serialize(int num, object obj, ProtoWriter protoWriter)
    {
        switch (num)
        {
            case 0:
                TierXConfigurationSerializer.Write((BubbleMessageData)obj, protoWriter);
                return;
            case 1:
                TierXConfigurationSerializer.Write((CarOverride)obj, protoWriter);
                return;
            case 2:
                TierXConfigurationSerializer.Write((ChoiceScreenInfo)obj, protoWriter);
                return;
            case 3:
                TierXConfigurationSerializer.Write((Color)obj, protoWriter);
                return;
            case 4:
                TierXConfigurationSerializer.Write((ConditionallyModifiedString)obj, protoWriter);
                return;
            case 5:
                TierXConfigurationSerializer.Write((ConditionallySelectedSnapshotList)obj, protoWriter);
                return;
            case 6:
                TierXConfigurationSerializer.Write((ConditionallySelectedSnapshots)obj, protoWriter);
                return;
            case 7:
                TierXConfigurationSerializer.Write((ConditionallySelectedString)obj, protoWriter);
                return;
            case 8:
                TierXConfigurationSerializer.Write((CrewMemberLayout)obj, protoWriter);
                return;
            case 9:
                TierXConfigurationSerializer.Write((DifficultyDeltas)obj, protoWriter);
                return;
            case 10:
                TierXConfigurationSerializer.Write((EligibilityCondition)obj, protoWriter);
                return;
            case 11:
                TierXConfigurationSerializer.Write((EligibilityConditionDetails)obj, protoWriter);
                return;
            case 12:
                TierXConfigurationSerializer.Write((EligibilityConditionDetails.ConditionMatchRequirment)obj, protoWriter);
                return;
            case 13:
                TierXConfigurationSerializer.Write((EligibilityRequirements)obj, protoWriter);
                return;
            case 14:
                TierXConfigurationSerializer.Write((EligibilityRequirements.PossibleGameState)obj, protoWriter);
                return;
            case 15:
                TierXConfigurationSerializer.Write((EventSelectEvent)obj, protoWriter);
                return;
            case 16:
                TierXConfigurationSerializer.Write((FormatStringData)obj, protoWriter);
                return;
            case 17:
                TierXConfigurationSerializer.Write((FormatStringParameterData)obj, protoWriter);
                return;
            case 18:
                TierXConfigurationSerializer.Write((LoadTierAction)obj, protoWriter);
                return;
            case 19:
                TierXConfigurationSerializer.Write((NarrativeSceneForEventData)obj, protoWriter);
                return;
            case 20:
                TierXConfigurationSerializer.Write((PinAnimationDetail)obj, protoWriter);
                return;
            case 21:
                TierXConfigurationSerializer.Write((PinDetail)obj, protoWriter);
                return;
            case 22:
                TierXConfigurationSerializer.Write((PinDetail.PinType)obj, protoWriter);
                return;
            case 23:
                TierXConfigurationSerializer.Write((PinDetail.TextureKeys)obj, protoWriter);
                return;
            case 24:
                TierXConfigurationSerializer.Write((PinDetail.TimelineDirection)obj, protoWriter);
                return;
            case 25:
                TierXConfigurationSerializer.Write((PinLock)obj, protoWriter);
                return;
            case 26:
                TierXConfigurationSerializer.Write((PinLockDetails)obj, protoWriter);
                return;
            case 27:
                TierXConfigurationSerializer.Write((PinScheduleConfiguration)obj, protoWriter);
                return;
            case 28:
                TierXConfigurationSerializer.Write((PinSchedulerAIDriverOverrides)obj, protoWriter);
                return;
            case 29:
                TierXConfigurationSerializer.Write((PinSequence)obj, protoWriter);
                return;
            case 30:
                TierXConfigurationSerializer.Write((PinSequence.eSequenceType)obj, protoWriter);
                return;
            case 31:
                TierXConfigurationSerializer.Write((PinSequenceTimelineData)obj, protoWriter);
                return;
            case 32:
                TierXConfigurationSerializer.Write((PinTemplate)obj, protoWriter);
                return;
            case 33:
                TierXConfigurationSerializer.Write((PopUpConfiguration)obj, protoWriter);
                return;
            case 34:
                TierXConfigurationSerializer.Write((PopupData)obj, protoWriter);
                return;
            case 35:
                TierXConfigurationSerializer.Write((PopupDataButtonAction)obj, protoWriter);
                return;
            case 36:
                TierXConfigurationSerializer.Write((ProgressionVisualisation)obj, protoWriter);
                return;
            case 37:
                TierXConfigurationSerializer.Write((PushScreenAction)obj, protoWriter);
                return;
            case 38:
                TierXConfigurationSerializer.Write((RestrictionRaceHelperOverride)obj, protoWriter);
                return;
            case 39:
                TierXConfigurationSerializer.Write((ScheduledPin)obj, protoWriter);
                return;
            case 40:
                TierXConfigurationSerializer.Write((SoundEventDetail)obj, protoWriter);
                return;
            case 41:
                TierXConfigurationSerializer.Write((StringModification)obj, protoWriter);
                return;
            case 42:
                TierXConfigurationSerializer.Write((StringModification.Details)obj, protoWriter);
                return;
            case 43:
                TierXConfigurationSerializer.Write((TextureDetail)obj, protoWriter);
                return;
            case 44:
                TierXConfigurationSerializer.Write((ThemeAnimationDetail)obj, protoWriter);
                return;
            case 45:
                TierXConfigurationSerializer.Write((ThemeAnimationsConfiguration)obj, protoWriter);
                return;
            case 46:
                TierXConfigurationSerializer.Write((ThemeLayout)obj, protoWriter);
                return;
            case 47:
                TierXConfigurationSerializer.Write((ThemeOptionLayoutDetails)obj, protoWriter);
                return;
            case 48:
                TierXConfigurationSerializer.Write((ThemeProgressionData)obj, protoWriter);
                return;
            case 49:
                TierXConfigurationSerializer.Write((ThemeTransition)obj, protoWriter);
                return;
            case 50:
                TierXConfigurationSerializer.Write((TimelinePinDetails)obj, protoWriter);
                return;
            case 51:
                TierXConfigurationSerializer.Write((Vector2)obj, protoWriter);
                return;
            case 52:
                TierXConfigurationSerializer.Write((Vector3)obj, protoWriter);
                return;
            case 53:
                TierXConfigurationSerializer.Write((WorldTourBossPinDetais)obj, protoWriter);
                return;
            case 54:
                TierXConfigurationSerializer.Write((WorldTourBossPinPieceDetails)obj, protoWriter);
                return;
            case 55:
                TierXConfigurationSerializer.Write((WorldTourProgressLayout)obj, protoWriter);
                return;
            case 56:
                TierXConfigurationSerializer.Write((KeyValuePair<string, TextureDetail>)obj, protoWriter);
                return;
            case 57:
                TierXConfigurationSerializer.Write((KeyValuePair<string, ConditionallySelectedString>)obj, protoWriter);
                return;
            case 58:
                TierXConfigurationSerializer.Write((KeyValuePair<string, PopupData>)obj, protoWriter);
                return;
            case 59:
                TierXConfigurationSerializer.Write((KeyValuePair<string, ThemeOptionLayoutDetails>)obj, protoWriter);
                return;
            default:
                return;
        }
    }

    protected override object Deserialize(int num, object obj, ProtoReader protoReader)
    {
        switch (num)
        {
            case 0:
                return TierXConfigurationSerializer.Read((BubbleMessageData)obj, protoReader);
            case 1:
                return TierXConfigurationSerializer.Read((CarOverride)obj, protoReader);
            case 2:
                return TierXConfigurationSerializer.Read((ChoiceScreenInfo)obj, protoReader);
            case 3:
                return TierXConfigurationSerializer._3(obj, protoReader);
            case 4:
                return TierXConfigurationSerializer.Read((ConditionallyModifiedString)obj, protoReader);
            case 5:
                return TierXConfigurationSerializer.Read((ConditionallySelectedSnapshotList)obj, protoReader);
            case 6:
                return TierXConfigurationSerializer.Read((ConditionallySelectedSnapshots)obj, protoReader);
            case 7:
                return TierXConfigurationSerializer.Read((ConditionallySelectedString)obj, protoReader);
            case 8:
                return TierXConfigurationSerializer.Read((CrewMemberLayout)obj, protoReader);
            case 9:
                return TierXConfigurationSerializer.Read((DifficultyDeltas)obj, protoReader);
            case 10:
                return TierXConfigurationSerializer.Read((EligibilityCondition)obj, protoReader);
            case 11:
                return TierXConfigurationSerializer.Read((EligibilityConditionDetails)obj, protoReader);
            case 12:
                return TierXConfigurationSerializer._12(obj, protoReader);
            case 13:
                return TierXConfigurationSerializer.Read((EligibilityRequirements)obj, protoReader);
            case 14:
                return TierXConfigurationSerializer.Read((EligibilityRequirements.PossibleGameState)obj, protoReader);
            case 15:
                return TierXConfigurationSerializer.Read((EventSelectEvent)obj, protoReader);
            case 16:
                return TierXConfigurationSerializer.Read((FormatStringData)obj, protoReader);
            case 17:
                return TierXConfigurationSerializer.Read((FormatStringParameterData)obj, protoReader);
            case 18:
                return TierXConfigurationSerializer.Read((LoadTierAction)obj, protoReader);
            case 19:
                return TierXConfigurationSerializer.Read((NarrativeSceneForEventData)obj, protoReader);
            case 20:
                return TierXConfigurationSerializer.Read((PinAnimationDetail)obj, protoReader);
            case 21:
                return TierXConfigurationSerializer.Read((PinDetail)obj, protoReader);
            case 22:
                return TierXConfigurationSerializer._22(obj, protoReader);
            case 23:
                return TierXConfigurationSerializer._23(obj, protoReader);
            case 24:
                return TierXConfigurationSerializer._24(obj, protoReader);
            case 25:
                return TierXConfigurationSerializer.Read((PinLock)obj, protoReader);
            case 26:
                return TierXConfigurationSerializer.Read((PinLockDetails)obj, protoReader);
            case 27:
                return TierXConfigurationSerializer.Read((PinScheduleConfiguration)obj, protoReader);
            case 28:
                return TierXConfigurationSerializer.Read((PinSchedulerAIDriverOverrides)obj, protoReader);
            case 29:
                return TierXConfigurationSerializer.Read((PinSequence)obj, protoReader);
            case 30:
                return TierXConfigurationSerializer._30(obj, protoReader);
            case 31:
                return TierXConfigurationSerializer.Read((PinSequenceTimelineData)obj, protoReader);
            case 32:
                return TierXConfigurationSerializer.Read((PinTemplate)obj, protoReader);
            case 33:
                return TierXConfigurationSerializer.Read((PopUpConfiguration)obj, protoReader);
            case 34:
                return TierXConfigurationSerializer.Read((PopupData)obj, protoReader);
            case 35:
                return TierXConfigurationSerializer.Read((PopupDataButtonAction)obj, protoReader);
            case 36:
                return TierXConfigurationSerializer.Read((ProgressionVisualisation)obj, protoReader);
            case 37:
                return TierXConfigurationSerializer.Read((PushScreenAction)obj, protoReader);
            case 38:
                return TierXConfigurationSerializer.Read((RestrictionRaceHelperOverride)obj, protoReader);
            case 39:
                return TierXConfigurationSerializer.Read((ScheduledPin)obj, protoReader);
            case 40:
                return TierXConfigurationSerializer.Read((SoundEventDetail)obj, protoReader);
            case 41:
                return TierXConfigurationSerializer.Read((StringModification)obj, protoReader);
            case 42:
                return TierXConfigurationSerializer.Read((StringModification.Details)obj, protoReader);
            case 43:
                return TierXConfigurationSerializer.Read((TextureDetail)obj, protoReader);
            case 44:
                return TierXConfigurationSerializer.Read((ThemeAnimationDetail)obj, protoReader);
            case 45:
                return TierXConfigurationSerializer.Read((ThemeAnimationsConfiguration)obj, protoReader);
            case 46:
                return TierXConfigurationSerializer.Read((ThemeLayout)obj, protoReader);
            case 47:
                return TierXConfigurationSerializer.Read((ThemeOptionLayoutDetails)obj, protoReader);
            case 48:
                return TierXConfigurationSerializer.Read((ThemeProgressionData)obj, protoReader);
            case 49:
                return TierXConfigurationSerializer.Read((ThemeTransition)obj, protoReader);
            case 50:
                return TierXConfigurationSerializer.Read((TimelinePinDetails)obj, protoReader);
            case 51:
                return TierXConfigurationSerializer._51(obj, protoReader);
            case 52:
                return TierXConfigurationSerializer._52(obj, protoReader);
            case 53:
                return TierXConfigurationSerializer.Read((WorldTourBossPinDetais)obj, protoReader);
            case 54:
                return TierXConfigurationSerializer.Read((WorldTourBossPinPieceDetails)obj, protoReader);
            case 55:
                return TierXConfigurationSerializer.Read((WorldTourProgressLayout)obj, protoReader);
            case 56:
                return TierXConfigurationSerializer._56(obj, protoReader);
            case 57:
                return TierXConfigurationSerializer._57(obj, protoReader);
            case 58:
                return TierXConfigurationSerializer._58(obj, protoReader);
            case 59:
                return TierXConfigurationSerializer._59(obj, protoReader);
            default:
                return null;
        }
    }

    static object _3(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((Color)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(Color), protoReader);
    }

    static object _12(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((EligibilityConditionDetails.ConditionMatchRequirment)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(EligibilityConditionDetails.ConditionMatchRequirment.Any, protoReader);
    }

    static object _22(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((PinDetail.PinType)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(PinDetail.PinType.NORMAL, protoReader);
    }

    static object _23(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((PinDetail.TextureKeys)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(PinDetail.TextureKeys.CarRender, protoReader);
    }

    static object _24(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((PinDetail.TimelineDirection)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(PinDetail.TimelineDirection.None, protoReader);
    }

    static object _30(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((PinSequence.eSequenceType)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(PinSequence.eSequenceType.Ladder, protoReader);
    }

    static object _51(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((Vector2)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(Vector2), protoReader);
    }

    static object _52(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((Vector3)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(Vector3), protoReader);
    }

    static object _56(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((KeyValuePair<string, TextureDetail>)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(KeyValuePair<string, TextureDetail>), protoReader);
    }

    static object _57(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((KeyValuePair<string, ConditionallySelectedString>)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(KeyValuePair<string, ConditionallySelectedString>), protoReader);
    }

    static object _58(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((KeyValuePair<string, PopupData>)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(KeyValuePair<string, PopupData>), protoReader);
    }

    static object _59(object obj, ProtoReader protoReader)
    {
        if (obj != null)
        {
            return TierXConfigurationSerializer.Read((KeyValuePair<string, ThemeOptionLayoutDetails>)obj, protoReader);
        }
        return TierXConfigurationSerializer.Read(default(KeyValuePair<string, ThemeOptionLayoutDetails>), protoReader);
    }

    static TierXConfigurationSerializer()
    {
        // Note: this type is marked as 'beforefieldinit'.
        TierXConfigurationSerializer.knownTypes.Add(typeof(BubbleMessageData), 0);
        TierXConfigurationSerializer.knownTypes.Add(typeof(CarOverride), 1);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ChoiceScreenInfo), 2);
        TierXConfigurationSerializer.knownTypes.Add(typeof(Color), 3);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ConditionallyModifiedString), 4);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ConditionallySelectedSnapshotList), 5);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ConditionallySelectedSnapshots), 6);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ConditionallySelectedString), 7);
        TierXConfigurationSerializer.knownTypes.Add(typeof(CrewMemberLayout), 8);
        TierXConfigurationSerializer.knownTypes.Add(typeof(DifficultyDeltas), 9);
        TierXConfigurationSerializer.knownTypes.Add(typeof(EligibilityCondition), 10);
        TierXConfigurationSerializer.knownTypes.Add(typeof(EligibilityConditionDetails), 11);
        TierXConfigurationSerializer.knownTypes.Add(typeof(EligibilityConditionDetails.ConditionMatchRequirment), 12);
        TierXConfigurationSerializer.knownTypes.Add(typeof(EligibilityRequirements), 13);
        TierXConfigurationSerializer.knownTypes.Add(typeof(EligibilityRequirements.PossibleGameState), 14);
        TierXConfigurationSerializer.knownTypes.Add(typeof(EventSelectEvent), 15);
        TierXConfigurationSerializer.knownTypes.Add(typeof(FormatStringData), 16);
        TierXConfigurationSerializer.knownTypes.Add(typeof(FormatStringParameterData), 17);
        TierXConfigurationSerializer.knownTypes.Add(typeof(LoadTierAction), 18);
        TierXConfigurationSerializer.knownTypes.Add(typeof(NarrativeSceneForEventData), 19);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinAnimationDetail), 20);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinDetail), 21);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinDetail.PinType), 22);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinDetail.TextureKeys), 23);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinDetail.TimelineDirection), 24);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinLock), 25);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinLockDetails), 26);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinScheduleConfiguration), 27);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinSchedulerAIDriverOverrides), 28);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinSequence), 29);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinSequence.eSequenceType), 30);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinSequenceTimelineData), 31);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PinTemplate), 32);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PopUpConfiguration), 33);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PopupData), 34);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PopupDataButtonAction), 35);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ProgressionVisualisation), 36);
        TierXConfigurationSerializer.knownTypes.Add(typeof(PushScreenAction), 37);
        TierXConfigurationSerializer.knownTypes.Add(typeof(RestrictionRaceHelperOverride), 38);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ScheduledPin), 39);
        TierXConfigurationSerializer.knownTypes.Add(typeof(SoundEventDetail), 40);
        TierXConfigurationSerializer.knownTypes.Add(typeof(StringModification), 41);
        TierXConfigurationSerializer.knownTypes.Add(typeof(StringModification.Details), 42);
        TierXConfigurationSerializer.knownTypes.Add(typeof(TextureDetail), 43);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ThemeAnimationDetail), 44);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ThemeAnimationsConfiguration), 45);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ThemeLayout), 46);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ThemeOptionLayoutDetails), 47);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ThemeProgressionData), 48);
        TierXConfigurationSerializer.knownTypes.Add(typeof(ThemeTransition), 49);
        TierXConfigurationSerializer.knownTypes.Add(typeof(TimelinePinDetails), 50);
        TierXConfigurationSerializer.knownTypes.Add(typeof(Vector2), 51);
        TierXConfigurationSerializer.knownTypes.Add(typeof(Vector3), 52);
        TierXConfigurationSerializer.knownTypes.Add(typeof(WorldTourBossPinDetais), 53);
        TierXConfigurationSerializer.knownTypes.Add(typeof(WorldTourBossPinPieceDetails), 54);
        TierXConfigurationSerializer.knownTypes.Add(typeof(WorldTourProgressLayout), 55);
        TierXConfigurationSerializer.knownTypes.Add(typeof(KeyValuePair<string, TextureDetail>), 56);
        TierXConfigurationSerializer.knownTypes.Add(typeof(KeyValuePair<string, ConditionallySelectedString>), 57);
        TierXConfigurationSerializer.knownTypes.Add(typeof(KeyValuePair<string, PopupData>), 58);
        TierXConfigurationSerializer.knownTypes.Add(typeof(KeyValuePair<string, ThemeOptionLayoutDetails>), 59);
    }
}
