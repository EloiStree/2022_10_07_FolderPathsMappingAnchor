using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIgnore_FinishByMono : PathIgnoreMono
{
    public string [] m_cantEndWith;
    public bool m_useLower;
    public bool m_useTrim;

    public override bool IsPathAllow(in string path)
    {
        for (int i = 0; i < m_cantEndWith.Length; i++)
        {
            if (m_useLower || m_useTrim)
            {
                string v = path;
                if (m_useLower)
                    v = v.ToLower();
                if (m_useTrim)
                    v = v.Trim();
                string t = m_cantEndWith[i];
                if (m_useLower)
                    t= t.ToLower();
                if (m_useTrim)
                    t = t.Trim();

                if (Eloi.E_StringUtility.EndWith(in v, in t))
                {
                    return false;
                }
            }
            else {
                if (Eloi.E_StringUtility.EndWith(in path, in m_cantEndWith[i])) {
                    return false;
                }
            }
        }
        return true;
    }


}
