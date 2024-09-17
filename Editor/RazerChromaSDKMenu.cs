#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RazerChromaSDKMenu : MonoBehaviour
{
    public static int CompareDirectoriesByCreationTime(DirectoryInfo dir1, DirectoryInfo dir2)
    {
        return dir2.CreationTimeUtc.CompareTo(dir1.CreationTimeUtc);
    }

    static DirectoryInfo GetMostRecentPackageFolder()
    {
        DirectoryInfo packageCache = new DirectoryInfo("Library\\PackageCache");
        if (packageCache.Exists)
        {
            List<DirectoryInfo> packageFolders = new List<DirectoryInfo>();
            foreach (DirectoryInfo di in packageCache.GetDirectories("com.razer.chromasdk@*"))
            {
                packageFolders.Add(di);
            }

            if (packageFolders.Count > 0)
            {
                packageFolders.Sort(CompareDirectoriesByCreationTime);
                /*
                foreach (DirectoryInfo di in packageFolders)
                {
                    Debug.LogFormat("{0} {1}", di.CreationTimeUtc, di.Name);
                }
                */

                return packageFolders[0];

            }


        }
        return null;
    }


    // Add a menu item to setup sample scenes
    [MenuItem("Assets/Razer Chroma SDK - Setup Sample Scenes")]
    static void MenuSetupSampleData()
    {
        DirectoryInfo packageFolder = GetMostRecentPackageFolder();
        if (packageFolder != null)
        {
            // get assets fp;der
            DirectoryInfo assets = new DirectoryInfo("Assets");
            if (!assets.Exists)
            {
                Debug.LogError("Assets does not exist!");
            }
            else
            {
                // create scenes folder if missing
                DirectoryInfo scenes = new DirectoryInfo("Assets\\Scenes");
                if (!scenes.Exists)
                {
                    Debug.Log("Creating Assets\\Scenes");
                    scenes.Create();
                }

                // copy scenes to destination
                if (!scenes.Exists)
                {
                    Debug.LogError("Assets\\Scenes does not exist!");
                }
                else
                {
                    //Debug.LogFormat("Assets\\Scenes exists");
                    string pathSampleScenes = packageFolder.FullName + "\\Tests\\Scenes";
                    //Debug.Log(pathSampleScenes);
                    DirectoryInfo sampleScenes = new DirectoryInfo(pathSampleScenes);
                    if (sampleScenes.Exists)
                    {
                        foreach (FileInfo fi in sampleScenes.GetFiles("*.unity"))
                        {
                            try
                            {
                                string pathDestination = scenes.FullName + "\\" + fi.Name;
                                fi.CopyTo(pathDestination, true);
                            }
                            catch
                            {
                                Debug.LogErrorFormat("Failed to copy: {0}", fi.Name);
                            }
                        }
                    }
                }

                // Copy Streaming Assets - Animations

                DirectoryInfo streamingAssets = new DirectoryInfo("Assets\\StreamingAssets");
                if (!streamingAssets.Exists)
                {
                    Debug.Log("Creating Assets\\StreamingAssets");
                    streamingAssets.Create();
                }

                // copy animations to destination
                if (!streamingAssets.Exists)
                {
                    Debug.LogError("Assets\\StreamingAssets does not exist!");
                }
                else
                {

                    DirectoryInfo animations = new DirectoryInfo("Assets\\StreamingAssets\\Animations");
                    if (!animations.Exists)
                    {
                        Debug.Log("Creating Assets\\StreamingAssets\\Animations");
                        animations.Create();
                    }

                    if (!animations.Exists)
                    {
                        Debug.LogError("Assets\\StreamingAssets\\Animations does not exist!");
                    }
                    else
                    {
                        //Debug.LogFormat("Assets\\StreamingAssets\\Animations exists");
                        string pathSampleAnimations = packageFolder.FullName + "\\StreamingAssets\\Animations";
                        //Debug.Log(pathSampleAnimations);
                        DirectoryInfo sampleAnimations = new DirectoryInfo(pathSampleAnimations);
                        if (sampleAnimations.Exists)
                        {
                            foreach (FileInfo fi in sampleAnimations.GetFiles("*.chroma"))
                            {
                                try
                                {
                                    string pathDestination = animations.FullName + "\\" + fi.Name;
                                    fi.CopyTo(pathDestination, true);
                                }
                                catch
                                {
                                    Debug.LogErrorFormat("Failed to copy: {0}", fi.Name);
                                }
                            }
                        }
                    }
                }

                // Refresh asset database
                AssetDatabase.Refresh();

                Debug.Log("Razer ChromaSDK sample content copied!");
            }

        }
    }
}

#endif
