using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class DataDrivenObject : MonoBehaviour, IBundleOwner
{
    private static DataDrivenObjectCreatedDelegate cache;
    public List<object> StringFormatArgs;

    public void CreateDataDrivenObjectFromAssetID(string assetID, DataDrivenObjectCreatedDelegate completed = null)
    {
        tempClass storeyb = new tempClass() {
            completed = completed,
            dataDrivenObject = this
        };
        if (storeyb.completed == null && cache == null)
        {
            cache = delegate {
            };
        }
        storeyb.completed = cache;
        AssetProviderClient.Instance.RequestAsset(assetID, storeyb.LoadDelegate, this);
    }

    public GameObject CreateDataDrivenScreen(GameObject prefab)
    {
        return this.CreateObject(prefab);
    }

    public static GameObject CreateDataDrivenScreenPrefab(AssetBundle dataDrivenScreenBundle)
    {
        return (dataDrivenScreenBundle.LoadAsset<GameObject>("DataDrivenScreen"));
    }

    private GameObject CreateObject(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }
        GameObject obj2 = Instantiate(prefab) as GameObject;
        obj2.transform.SetParent(base.transform,false);
        obj2.transform.localPosition = Vector3.zero;
        this.InitialiseChildren(base.transform);
        return obj2;
    }

    public void DestroyChildren()
    {
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                Destroy(current.gameObject);
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }

    private void InitialiseChildren(Transform trans)
    {
        this.TranslateText(trans);
        this.ResetAnimations();
    }

    private void PrimeAnimation(Transform trans)
    {
        if (trans.GetComponent<Animation>() != null)
        {
            Animation anim = trans.GetComponent<Animation>();
            AnimationUtils.PlayFirstFrame(anim);
            if (anim.playAutomatically)
            {
                AnimationUtils.PlayAnim(anim);
            }
        }
        IEnumerator enumerator = trans.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform)enumerator.Current;
                this.PrimeAnimation(current);
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }

    public void ResetAnimations()
    {
        this.PrimeAnimation(base.transform);
    }

    private void TranslateText(Transform trans)
    {
        TextMeshProUGUI component = trans.GetComponent<TextMeshProUGUI>();
        if ((component != null) && !string.IsNullOrEmpty(component.text))
        {
            string format = string.Empty;
            if (component.text.StartsWith("TEXT"))
            {
                format = LocalizationManager.GetTranslation(component.text);
            }
            else
            {
                format = component.text;
            }
            if ((this.StringFormatArgs != null) && (this.StringFormatArgs.Count != 0))
            {
                format = string.Format(format, this.StringFormatArgs.ToArray());
            }
            component.text = format;
        }
        IEnumerator enumerator = trans.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform)enumerator.Current;
                this.TranslateText(current);
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }

    private void UIBundleReadyDelegate(string assetID, AssetBundle assetBundle, IBundleOwner owner, DataDrivenObjectCreatedDelegate completed)
    {
        GameObject go = this.CreateObject(CreateDataDrivenScreenPrefab(assetBundle));
        completed(go);
        AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
    }

    private sealed class tempClass
    {
        public DataDrivenObject dataDrivenObject;
        public DataDrivenObjectCreatedDelegate completed;

        public void LoadDelegate(string asset, AssetBundle assetBundle, IBundleOwner owner)
        {
            dataDrivenObject.UIBundleReadyDelegate(asset, assetBundle, owner, completed);
        }
    }

    public delegate void DataDrivenObjectCreatedDelegate(GameObject go);
}
