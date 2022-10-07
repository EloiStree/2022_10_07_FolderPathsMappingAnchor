using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIgnore_MustFinishByMono : PathIgnoreMono
{
    public string [] m_mustEndWith;
    public bool m_useLower;
    public bool m_useTrim;

    public override bool IsPathAllow(in string path)
    {
        for (int i = 0; i < m_mustEndWith.Length; i++)
        {
            if (m_useLower || m_useTrim)
            {
                string v = path;
                if (m_useLower)
                    v = v.ToLower();
                if (m_useTrim)
                    v = v.Trim();
                string t = m_mustEndWith[i];
                if (m_useLower)
                    t= t.ToLower();
                if (m_useTrim)
                    t = t.Trim();

                if (Eloi.E_StringUtility.EndWith(in v, in t))
                {
                    return true;
                }
            }
            else {
                if (Eloi.E_StringUtility.EndWith(in path, in m_mustEndWith[i])) {
                    return true;
                }
            }
        }
        return false;
    }


}
