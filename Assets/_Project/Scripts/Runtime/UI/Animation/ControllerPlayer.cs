using EnhancedShift.Player.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedShift.Player.Core
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ControllerPlayer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private CapsuleCollider cc;
        [SerializeField] private Animator an;
        [SerializeField] private Camera cam;

        [Header("Input")]
        [SerializeField] private List<ActionBinding> binds = new();

        public Rigidbody Rb => rb;
        public CapsuleCollider Cc => cc;
        public Animator An => an;
        public Camera Cam => cam;
        public PlayerState St { get; private set; }

        private readonly Dictionary<string, ActionBinding> map = new();

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();

            if (!cc)
                cc = GetComponent<CapsuleCollider>();

            if (!an)
                an = GetComponent<Animator>();

            if (!cam)
                cam = Camera.main;

            St = new PlayerState();

            map.Clear();

            foreach (var b in binds)
            {
                if (b == null)
                    continue;

                if (string.IsNullOrWhiteSpace(b.Action))
                    continue;

                if (!map.ContainsKey(b.Action))
                    map.Add(b.Action, b);
            }
        }

        public bool Down(string id)
        {
            return map.TryGetValue(id, out var b) && b.Down();
        }

        public bool Hold(string id)
        {
            return map.TryGetValue(id, out var b) && b.Hold();
        }

        public bool Up(string id)
        {
            return map.TryGetValue(id, out var b) && b.Up();
        }

        // --- Управление аниматором ---

        public void Anim(string id, bool value = true)
        {
            if (an == null || string.IsNullOrWhiteSpace(id))
                return;

            an.SetBool(id, value);
        }

        public void Anim(string id, float value)
        {
            if (an == null || string.IsNullOrWhiteSpace(id))
                return;

            an.SetFloat(id, value);
        }

        public void AnimTrigger(string id)
        {
            if (an == null || string.IsNullOrWhiteSpace(id))
                return;

            an.SetTrigger(id);
        }

        public ActionBinding Get(string id)
        {
            map.TryGetValue(id, out var b);
            return b;
        }
    }
}