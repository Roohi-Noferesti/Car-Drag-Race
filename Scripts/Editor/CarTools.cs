using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CarTools
{
    [MenuItem("Tools/Cars/TransferCosmeticData")]
    static void TransferCosmeticData()
    {
        var oldGtCars = CarListWindow.GetOldGTCars();
        var newGTCars = CarListWindow.GetGTCars();
        int count = 0;
        Undo.RegisterCompleteObjectUndo(newGTCars,"Transfer Cosmetic");
        foreach (var gtCar in newGTCars)
        {
            var oldCar = oldGtCars.FirstOrDefault(c => c.Key == gtCar.Key);

            if (oldCar != null)
            {
                gtCar.Stickers = new int[oldCar.Stickers.Length];
                for (int i = 0; i < oldCar.Stickers.Length; i++)
                {
                    gtCar.Stickers[i] = oldCar.Stickers[i];
                }

                count++;
            }
        }

        foreach (var newGtCar in newGTCars)
        {
            EditorUtility.SetDirty(newGtCar);
        }


        EditorUtility.DisplayDialog("Done", "Transfer sticker done . total cars affected : " + count, "OK");
    }
}