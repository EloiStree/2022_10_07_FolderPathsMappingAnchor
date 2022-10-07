using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIgnore_ContainsStringMono : PathIgnoreMono
{
    public string [] m_cantContain;
    public bool m_useLower;
    public bool m_useTrim;
    public ContainsType m_containType;
    public enum ContainsType { CantContain, MustContain}

    public override bool IsPathAllow(in string path)
    {
        for (int i = 0; i < m_cantContain.Length; i++)
        {
            if (m_useLower || m_useTrim)
            {
                string v = path;
                if (m_useLower)
                    v = v.ToLower();
                if (m_useTrim)
                    v = v.Trim();
                string t = m_cantContain[i];
                if (m_useLower)
                    t= t.ToLower();
                if (m_useTrim)
                    t = t.Trim();

                if (v.IndexOf(t) > 0)
                {
                    if (m_containType == ContainsType.CantContain)
                        return false;
                    if (m_containType == ContainsType.MustContain)
                        return true;
                }
            }
            else {
                if (path.IndexOf(m_cantContain[i])>0) {
                    if (m_containType == ContainsType.CantContain)
                        return false;
                    if (m_containType == ContainsType.MustContain)
                        return true;
                }
            }
        }
        if (m_containType == ContainsType.CantContain)
            return true;
        else // (m_containType == ContainsType.MustContain)
            return false;
    }


}
