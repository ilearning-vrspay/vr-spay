using System;
using System.IO;
using UnityEngine;

public static class FileSystem
{
    public static string GetAbsolutePathFromUnityAssetFilePath(string unityAssetFilePath)
    {
        string p1 = Application.dataPath;
        p1 = p1.Replace("/Assets", "/");
        p1 = p1 + unityAssetFilePath;
        return p1;
    }

    public static bool DoesFileExistAtRelativePath(string unityAssetFilePath)
    {
        string absPath = GetAbsolutePathFromUnityAssetFilePath(unityAssetFilePath);
        return File.Exists(absPath);
    }

    public static bool DoesFileExistAtAbsolutePath(string absPath)
    {
        return File.Exists(absPath);
    }

    public static string CombinePathFilename(string path, string filename)
    {
        if (!path.EndsWith('/'))
        {
            path = $"{path}/";
        }

        if (filename.StartsWith('/'))
        {
            filename = filename.Substring(1);
        }
        return $"{path}{filename}";
    }

    public static string CombineFilenameExt(string filename, string extension)
    {
        if (filename.EndsWith('.'))
        {
            filename = filename.Substring(0, filename.Length - 1); ;
        }

        if (!extension.StartsWith('.'))
        {
            extension = $".{extension}";
        }

        return $"{filename}{extension}";
    }

    public static string CombinePathFilenameExt(string path, string filename, string extension)
    {
        return CombinePathFilename(path, CombineFilenameExt(filename,extension));
    }
}
