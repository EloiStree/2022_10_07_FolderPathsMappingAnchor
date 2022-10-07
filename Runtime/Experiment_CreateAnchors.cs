using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FolderPathsMappingAnchorEvent : UnityEvent<FolderPathsMappingAnchor> {}

public class Experiment_CreateAnchors : MonoBehaviour
{

    public string m_targetAbsolutePath;
    public string m_whereToStoreAnchor;
    public FolderPathsMappingAnchor m_anchor;
    public FolderPathsMappingAnchorEvent m_onChanged;

    public FolderPathsMappingAnchorBuilder m_builder;
    [TextArea(0,10)]
    public string m_debugText;
    [ContextMenu("Generate Anchor")]
    void GenerateAnchor()
    {
        m_builder.GenerateAnchor(in m_targetAbsolutePath, out m_anchor);
        m_onChanged.Invoke(m_anchor);
    }
    [ContextMenu("Load Anchor")]
    void LoadAnchor()
    {
        FolderPathsMappingAnchorUtility.I.Convert(in m_debugText, out m_anchor);
        m_onChanged.Invoke(m_anchor);
    }
    [ContextMenu("Save Anchor")]
    void SaveAnchor()
    {
        FolderPathsMappingAnchorUtility.I.Convert(in m_anchor, out  m_debugText);
        m_onChanged.Invoke(m_anchor);
    }

}

[System.Serializable]
public class GroupPathIgnoreMono {
    public PathIgnoreMono[] m_pathIgnore;

    public bool m_nullPathAllowValue;

    public bool IsPathAllow(in string target)
    {
        //if ( target == null)
        //    return m_nullPathAllowValue;
        for (int i = 0; i < m_pathIgnore.Length; i++)
        {
            if (!m_pathIgnore[i].IsPathAllow(in target))
                return false;
        }
        return true;
    }
}

[System.Serializable]
public abstract class PathIgnoreMono : MonoBehaviour
{
    public abstract bool IsPathAllow(in string path);

   
}
//[System.Serializable]
//public abstract class FileIgnoreMono {
//    public abstract bool IsFileAllow(in string path);
//}
//[System.Serializable]
//public abstract class FolderExplorationIgnoreMono {
//   public abstract bool IsFolderAllow(in string path);
//}



[System.Serializable]
public class FolderPathsMappingAnchorBuilder
{

    public GroupPathIgnoreMono m_pathIgnoreFilter;

    public void GenerateAnchor(in string directoryPath, out FolderPathsMappingAnchor anchor)
    {
        anchor = new FolderPathsMappingAnchor();
        List<string> toExplore = new List<string>();
        List<string> files = new List<string>();
        List<string> directory = new List<string>();
        toExplore.Add(directoryPath);
        while (toExplore.Count > 0)
        {
            string target = toExplore[0];
            if (File.Exists(target))
            {
                if (m_pathIgnoreFilter.IsPathAllow(in target))
                    files.Add(target);
            }
            else if (Directory.Exists(target))
            {
                string[] fil = Directory.GetFiles(target, "*", SearchOption.TopDirectoryOnly);
                string[] dir = Directory.GetDirectories(target, "*", SearchOption.TopDirectoryOnly);
                foreach (var item in fil)
                {
                    if (m_pathIgnoreFilter.IsPathAllow(in item))
                        files.Add(item);
                }
                foreach (var item in dir)
                {
                    if (m_pathIgnoreFilter.IsPathAllow(in item)) {

                        directory.Add(item);
                        toExplore.Add(item);
                    }
                }
            }
            toExplore.RemoveAt(0);
        }
        anchor.m_trackedFilePathInRoot = files.Select(
            k => new PathFileWithInfo()
            {
                m_absolutePath = k,
                m_fileByteSize = new System.IO.FileInfo(k).Length
            }
            ).ToList();
        anchor.m_trackedFolderPathInRoot = directory.Select(
            k => new PathFolderWithInfo()
            {
                m_absolutePath = k,
            }
            ).ToList();
    }

    //public IEnumerator GenerateAnchorCoroutine(in string directoryPath, out FolderPathsMappingAnchor anchor)
    //{
    //    List<string> toExplore = new List<string>();
    //    List<string> files = new List<string>();
    //    List<string> directory = new List<string>();
    //    toExplore.Add(directoryPath);
    //    while (toExplore.Count > 0) {
    //        string target = toExplore[0];
    //        if (File.Exists(target))
    //        {
    //            files.Add(target);
                
    //        }
    //        else if (Directory.Exists(target)) {

    //            Directory.GetFiles(directoryPath, "*",SearchOption.TopDirectoryOnly);
    //        }

    //        toExplore.RemoveAt(0);
    //    }
    //}
}


public class FolderPathsMappingAnchorUtility {

    public static FolderPathsMappingAnchorUtility I = new FolderPathsMappingAnchorUtility();

    


    public void Convert(in FolderPathsMappingAnchor source, out string sourceAsText) {
        StringBuilder sb = new StringBuilder();
        sb.Append("#|" + source.m_rootPath+"\n");
        for (int i = 0; i < source.m_trackedFilePathInRoot.Count; i++)
        {
            sb.AppendLine(string.Format("{0}❖{1}",
                source.m_trackedFilePathInRoot[i].m_absolutePath,
                source.m_trackedFilePathInRoot[i].m_fileByteSize));
        }
        for (int i = 0; i < source.m_trackedFolderPathInRoot.Count; i++)
        {
            sb.AppendLine(                source.m_trackedFolderPathInRoot[i].m_absolutePath);
        }
        sourceAsText = sb.ToString();

    }
    public void Convert(in string source, out FolderPathsMappingAnchor sourceAsClass) {
        sourceAsClass = new FolderPathsMappingAnchor();
        string[] lines = source.Split(new char[] { '\n', '\r' });
        List<PathFileWithInfo> infoFile = new List<PathFileWithInfo>();
        List<PathFolderWithInfo> infoDirectory = new List<PathFolderWithInfo>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length > 0) {
                if (i == 0 && lines[i].Length >= 2 && lines[i][0] == '#' && lines[i][1] == '|')
                {
                    sourceAsClass.m_rootPath = lines[i].Substring(2);
                }
                else { 
                    string[] tokens = lines[i].Split('❖');

                    if (tokens.Length == 1)
                    {
                        PathFolderWithInfo pathInfo = new PathFolderWithInfo();
                        pathInfo.m_absolutePath = tokens[0];
                        infoDirectory.Add(pathInfo);
                    }
                    if (tokens.Length > 1)
                    {

                        PathFileWithInfo pathInfo = new PathFileWithInfo();
                        pathInfo.m_absolutePath = tokens[0];
                        if(long.TryParse(tokens[1], out long result))
                            pathInfo.m_fileByteSize = result;
                        infoFile .Add(pathInfo);
                    }
                }
            }
        }
        sourceAsClass.m_trackedFilePathInRoot = infoFile;
        sourceAsClass.m_trackedFolderPathInRoot = infoDirectory;
    }

}


[System.Serializable]
public class FolderPathsMappingAnchor {

    public string m_rootPath;
    public List<PathFileWithInfo> m_trackedFilePathInRoot = new List<PathFileWithInfo>();
    public List<PathFolderWithInfo> m_trackedFolderPathInRoot= new List<PathFolderWithInfo>();

    public void GetAllFilePath(out string[] path)
    {
        path = m_trackedFilePathInRoot.Select(k => k.m_absolutePath).ToArray();
    }
    public void GetAllDirectoryPath(out string[] path)
    {
        path = m_trackedFolderPathInRoot.Select(k => k.m_absolutePath).ToArray();
    }
    public void GetAllPath(out string[] path)
    {
        GetAllFilePath(out string[] pf);
        GetAllDirectoryPath(out string [] pd);
        path = pf.Concat(pd).ToArray();
    }
}

public  class FolderPathsMappingAnchorFetch {
    public static FolderPathsMappingAnchorFetch I = new FolderPathsMappingAnchorFetch();


}


[System.Serializable]
public class PathFileWithInfo
{
    public string m_absolutePath;
    public long m_fileByteSize;
}
[System.Serializable]
public class PathFolderWithInfo
{
    public string m_absolutePath;
}