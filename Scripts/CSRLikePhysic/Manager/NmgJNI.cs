using System;
using UnityEngine;

public class NmgJNI
{
	public static jvalue[] CreateNullJNIParams()
	{
		return new jvalue[1];
	}

	public static IntPtr FindClass(string classId)
	{
		NmgUnityPlugin.ASSERT(!string.IsNullOrEmpty(classId));
		CheckExceptions();
		IntPtr result = AndroidJNI.FindClass(classId);
		CheckExceptions();
		return result;
	}

	public static IntPtr FindClass(IntPtr classLoader, string classId)
	{
		NmgUnityPlugin.ASSERT(classLoader != IntPtr.Zero);
		NmgUnityPlugin.ASSERT(!string.IsNullOrEmpty(classId));
		IntPtr intPtr = AndroidJNI.FindClass("java/lang/Class");
		IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(intPtr, "forName", "(Ljava/lang/String;ZLjava/lang/ClassLoader;)Ljava/lang/Class;");
		CheckExceptions();
		string bytes = classId.Replace("/", ".");
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(bytes);
		jvalue[] array = new jvalue[3];
		array[0].l = intPtr2;
		array[1].z = true;
		array[2].l = classLoader;
		NmgUnityPlugin.ASSERT(staticMethodID != IntPtr.Zero);
		IntPtr intPtr3 = AndroidJNI.CallStaticObjectMethod(intPtr, staticMethodID, array);
		NmgUnityPlugin.ASSERT(intPtr3 != IntPtr.Zero);
		AndroidJNI.DeleteLocalRef(intPtr2);
		AndroidJNI.DeleteLocalRef(intPtr);
		CheckExceptions();
		return intPtr3;
	}

	public static IntPtr GetObjectClassLoader(IntPtr obj)
	{
		CheckExceptions();
		IntPtr objectClass = AndroidJNI.GetObjectClass(obj);
		IntPtr methodID = GetMethodID(objectClass, "getClassLoader", "()Ljava/lang/ClassLoader;");
		NmgUnityPlugin.ASSERT(methodID != IntPtr.Zero);
		IntPtr intPtr = AndroidJNI.CallObjectMethod(obj, methodID, CreateNullJNIParams());
		NmgUnityPlugin.ASSERT(intPtr != IntPtr.Zero);
		AndroidJNI.DeleteLocalRef(objectClass);
		CheckExceptions();
		return intPtr;
	}

	public static IntPtr GetMethodID(IntPtr clazz, string methodId, string methodSignature)
	{
		NmgUnityPlugin.ASSERT(clazz != IntPtr.Zero);
		NmgUnityPlugin.ASSERT(!string.IsNullOrEmpty(methodId));
		NmgUnityPlugin.ASSERT(!string.IsNullOrEmpty(methodSignature));
		CheckExceptions();
		IntPtr methodID = AndroidJNI.GetMethodID(clazz, methodId, methodSignature);
		NmgUnityPlugin.ASSERT(methodID != IntPtr.Zero);
		CheckExceptions();
		return methodID;
	}

	public static IntPtr GetStaticMethodID(IntPtr clazz, string methodId, string methodSignature)
	{
		NmgUnityPlugin.ASSERT(clazz != IntPtr.Zero);
		NmgUnityPlugin.ASSERT(!string.IsNullOrEmpty(methodId));
		NmgUnityPlugin.ASSERT(!string.IsNullOrEmpty(methodSignature));
		IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, methodId, methodSignature);
		NmgUnityPlugin.ASSERT(staticMethodID != IntPtr.Zero);
		CheckExceptions();
		return staticMethodID;
	}

	public static void CheckExceptions()
	{
		IntPtr value = AndroidJNI.ExceptionOccurred();
		if (value != IntPtr.Zero)
		{
			AndroidJNI.ExceptionDescribe();
			AndroidJNI.ExceptionClear();
		}
	}
}
