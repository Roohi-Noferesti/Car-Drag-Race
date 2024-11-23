using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RevenueTrackingConfiguration))]
public class RevenueTrackingConfigurationInspector : Editor {
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Load From InAppPurchases"))
        {

            if (!EditorUtility.DisplayDialog("Load From InappPurchases", "Are you sure? All prices will be reset to 0.",
                "OK", "Cancel"))
            {
                return;
            }
            var inAppPurchases =
                    AssetDatabase.LoadAssetAtPath<InAppPurchases>("Assets/configuration/InAppPurchases.asset");

            var revenu = target as RevenueTrackingConfiguration;

            revenu.Prices = new ProductPrice[inAppPurchases.GetProductsListLength()];

            for (int i = 0; i < inAppPurchases.GetProductsListLength(); i++)
            {
                revenu.Prices[i] = new ProductPrice()
                {
                    ProductID = inAppPurchases.ABTestReadyGtProducts[i].Code,
                    CADPrice = 0
                };
            }

            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<RevenueTrackingConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<RevenueTrackingConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((RevenueTrackingConfiguration)target);
        }
        base.OnInspectorGUI();
    }
}
