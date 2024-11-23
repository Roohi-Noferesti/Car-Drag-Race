using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseInitiator : MonoBehaviour
{

    public static FirebaseInitiator _instance;
    public FirebaseApp firebaseApp;

    void Awake()
    {
        _instance = this;}

    void Start () {
		InitCrashlytics();
        StartCoroutine(doitLate());

    }


    void InitCrashlytics()
    {
        // Initialize Firebase
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                firebaseApp = app;

                // Set a flag here for indicating that your project is ready to use Firebase.
                UnityEngine.Debug.Log("Firebase is Ready to Use");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }




    IEnumerator doitLate()
    {
        yield return new WaitForSeconds(10);
        Crashlytics.IsCrashlyticsCollectionEnabled = true;
        Crashlytics.Log("moeen: please dont get fucked up");
        Crashlytics.LogException(new Exception("moeen: just exception"));

        Firebase.Analytics.FirebaseAnalytics.LogEvent(
            Firebase.Analytics.FirebaseAnalytics.EventSelectContent,
            new Firebase.Analytics.Parameter[] {
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.ParameterItemId, "ddsads"),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.ParameterItemName, "name"),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.UserPropertySignUpMethod, "Google"),
                new Firebase.Analytics.Parameter(
                    "favorite_food", "dsadsad"),
                new Firebase.Analytics.Parameter(
                    "user_id", "dsadsa"),
            }
        );

    }
}
