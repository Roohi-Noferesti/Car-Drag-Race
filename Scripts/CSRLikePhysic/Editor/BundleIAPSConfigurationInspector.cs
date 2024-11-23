using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BundleOffersPopUpConfiguration))]
public class BundleIAPSConfigurationInspector : Editor
{
    private string productIDsGroupPrefix;

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<BundleOffersPopUpConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<BundleOffersPopUpConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((BundleOffersPopUpConfiguration)target);
        }

        productIDsGroupPrefix = EditorGUILayout.TextField("Group Prefix", productIDsGroupPrefix);

        if (GUILayout.Button("Add Group Prefix To Product IDs"))
        {
            if (string.IsNullOrEmpty(productIDsGroupPrefix))
                return;

            Undo.RegisterCompleteObjectUndo(target, "Add Prefix to Product IDs");
            AddGroupPrefix();
            EditorUtility.SetDirty(target);
        }

        base.OnInspectorGUI();
    }


    private void AddGroupPrefix()
    {
        var bundleOfferConfig = target as BundleOffersPopUpConfiguration;

        foreach (var bundleOfferData in bundleOfferConfig.OneTimeOffers)
        {
            AddGroupPrefixToOfferItem(bundleOfferData);
        }

        foreach (var bundleOfferData in bundleOfferConfig.RepeatableOffers)
        {
            AddGroupPrefixToOfferItem(bundleOfferData);
        }
    }

    private void AddGroupPrefixToOfferItem(BundleOfferData bundleOfferData)
    {
        bundleOfferData.PopupData.BundleOfferItem = bundleOfferData.PopupData.BundleOfferItem.Insert(0, productIDsGroupPrefix);
        if (!string.IsNullOrEmpty(bundleOfferData.PopupData.StarterPackItem1))
        {
            bundleOfferData.PopupData.StarterPackItem1 =
                bundleOfferData.PopupData.StarterPackItem1.Insert(0, productIDsGroupPrefix);
        }
        if (!string.IsNullOrEmpty(bundleOfferData.PopupData.StarterPackItem2))
        {
            bundleOfferData.PopupData.StarterPackItem2 =
                bundleOfferData.PopupData.StarterPackItem2.Insert(0, productIDsGroupPrefix);
        }


        foreach (var bundleOfferWidgetInfo in bundleOfferData.PopupData.WidgetInfo)
        {
            if (bundleOfferWidgetInfo.OfferType != "CAR" && !string.IsNullOrEmpty(bundleOfferWidgetInfo.ShopItem))
            {
                bundleOfferWidgetInfo.ShopItem =
                    bundleOfferWidgetInfo.ShopItem.Insert(0, productIDsGroupPrefix);
            }
        }
    }
}
