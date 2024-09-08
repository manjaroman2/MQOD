using System;
using UniverseLib.UI;

namespace MQOD
{
    public class UIBaseMQOD : UIBase
    {
        public readonly PreferencesManager prefManager;

        public UIBaseMQOD(string id, Action updateMethod, PreferencesManager preferencesManager) : base(id,
            updateMethod)
        {
            prefManager = preferencesManager;
        }
    }
}