using UnityEngine;

public static class Utils
{
    public static void CopyComponentValues<T>(T original, T destination) where T : Component
    {
        System.Type type = original.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(destination, field.GetValue(original));
        }
    }

    public static void CopyComponentValues<T>(GameObject original, T destination) where T : Component
    {
        CopyComponentValues<T>(original.GetComponent<T>(), destination);
    }

    public static void CopyComponentValues<T>(T original, GameObject destination) where T : Component
    {
        CopyComponentValues<T>(original, destination.GetComponent<T>());
    }

    public static void CopyComponentValues<T>(GameObject original, GameObject destination) where T : Component
    {
        CopyComponentValues<T>(original.GetComponent<T>(), destination.GetComponent<T>());
    }
}