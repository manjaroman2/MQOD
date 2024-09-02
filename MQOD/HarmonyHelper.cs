using System;
using HarmonyLib;

namespace MQOD
{
    public static class HarmonyHelper
    {
        public static void Patch(Type clazz, string method, Type[] types = null, Type prefixClazz = null,
            string prefixMethod = null, Type postfixClazz = null, string postfixMethod = null)
        {
            types ??= new Type[] { };
            HarmonyMethod prefix = null;
            HarmonyMethod postfix = null;
            if (prefixClazz != null && prefixMethod != null)
            {
                prefix = new HarmonyMethod(prefixClazz.GetMethod(prefixMethod, AccessTools.all));
            }

            if (postfixClazz != null && postfixMethod != null)
            {
                postfix = new HarmonyMethod(postfixClazz.GetMethod(postfixMethod, AccessTools.all));
            }

            if (prefix != null)
            {
                if (postfix != null)
                {
                    MQOD.Instance.HarmonyInstance.Patch(clazz.GetMethod(method, AccessTools.all, null, types, null),
                        postfix: postfix, prefix: prefix);
                }
                else
                {
                    MQOD.Instance.HarmonyInstance.Patch(clazz.GetMethod(method, AccessTools.all, null, types, null),
                        prefix: prefix);
                }
            }
            else if (postfix != null)
            {
                MQOD.Instance.HarmonyInstance.Patch(clazz.GetMethod(method, AccessTools.all, null, types, null),
                    postfix: postfix);
            }
        }
    }
}