using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FolderPathsMappingAnchorFilterMono : MonoBehaviour
{
    public GroupPathIgnoreMono m_fileFilter;
    public GroupPathIgnoreMono m_folderFilter;
    public FolderPathsMappingAnchorEvent m_filtered;
    public PushType m_pushType;
    public enum PushType {PushCopy, PushRef}
    public void Push(FolderPathsMappingAnchor pathMap) {

        if (m_pushType == PushType.PushRef)
        {
            for (int i = pathMap.m_trackedFilePathInRoot.Count - 1; i >= 0; i--)
            {
                if (!m_fileFilter.IsPathAllow(in pathMap.m_trackedFilePathInRoot[i].m_absolutePath))
                    pathMap.m_trackedFilePathInRoot.RemoveAt(i);
            }
            for (int i = pathMap.m_trackedFolderPathInRoot.Count - 1; i >= 0; i--)
            {
                if (!m_folderFilter.IsPathAllow(in pathMap.m_trackedFolderPathInRoot[i].m_absolutePath))
                    pathMap.m_trackedFolderPathInRoot.RemoveAt(i);
            }
            m_filtered.Invoke(pathMap);
        }
        if (m_pushType == PushType.PushCopy)
        {
            FolderPathsMappingAnchor p = new FolderPathsMappingAnchor();
            p.m_rootPath = pathMap.m_rootPath;
            p.m_trackedFilePathInRoot = pathMap.m_trackedFilePathInRoot.ToList();
            p.m_trackedFolderPathInRoot = pathMap.m_trackedFolderPathInRoot.ToList();

            for (int i = p.m_trackedFilePathInRoot.Count - 1; i >= 0; i--)
            {
                if (!m_fileFilter.IsPathAllow(in p.m_trackedFilePathInRoot[i].m_absolutePath))
                    p.m_trackedFilePathInRoot.RemoveAt(i);
            }
            for (int i = p.m_trackedFolderPathInRoot.Count - 1; i >= 0; i--)
            {
                if (!m_folderFilter.IsPathAllow(in p.m_trackedFolderPathInRoot[i].m_absolutePath))
                    p.m_trackedFolderPathInRoot.RemoveAt(i);
            }
            m_filtered.Invoke(p);
        }
    }
}
