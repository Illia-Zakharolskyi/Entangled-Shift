using System.Collections.Generic;
using UnityEngine;

namespace EnhancedShift.Player.Actions
{
    public enum AnimType
    {
        Trigger,
        Bool
    }

    [System.Serializable]
    public class ActionBinding
    {
        [SerializeField] private string action;
        [SerializeField] private AnimType type = AnimType.Trigger;
        [SerializeField] private string param;
        [SerializeField] private List<KeyCode> keys = new();
        [SerializeField] private bool enable = true;

        public string Action => action;
        public AnimType Type => type;
        public string Param => param;
        public bool Enable => enable;
        public IReadOnlyList<KeyCode> Keys => keys;

 
        public bool Down()
        {
            if (!Check()) return false;

            
            if (keys.Count == 1)
                return Input.GetKeyDown(keys[0]);

           
            for (int i = 0; i < keys.Count - 1; i++)
            {
                if (!Input.GetKey(keys[i]))
                    return false;
            }

            return Input.GetKeyDown(keys[keys.Count - 1]);
        }

    
        public bool Hold()
        {
            if (!Check()) return false;

            for (int i = 0; i < keys.Count; i++)
            {
                if (!Input.GetKey(keys[i]))
                    return false;
            }

            return true;
        }

    
        public bool Up()
        {
            if (!Check()) return false;

            if (keys.Count == 1)
                return Input.GetKeyUp(keys[0]);

            for (int i = 0; i < keys.Count - 1; i++)
            {
                if (!Input.GetKey(keys[i]))
                    return false;
            }

            return Input.GetKeyUp(keys[keys.Count - 1]);
        }

        private bool Check()
        {
            return enable && keys != null && keys.Count > 0;
        }
    }
}