using EntangledShift.Runtime.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EntangledShift.Runtime.UI.Menu
{
    public class UIMenuButtonAudio : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private AudioClip _hoverClip;
        [SerializeField] private AudioClip _clickClip;

        private void Awake()
        {
            Button[] allButtons = GetComponentsInChildren<Button>(true);

            foreach (var btn in allButtons)
            {
                btn.onClick.AddListener(PlayClick);

                EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = btn.gameObject.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry entry = new();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { PlayHover(); });

                trigger.triggers.Add(entry);
            }
        }

        public void PlayHover()
        {
            if (_hoverClip == null)
            {
                Debug.Log($"No hover audio for [{this.name}]");
                return;
            }

            AudioController.Instance.PlayOneShotSFXSound(_hoverClip);
        }

        public void PlayClick()
        {
            if (_clickClip == null)
            {
                Debug.Log($"No click audio for [{this.name}]");
                return;
            }

            AudioController.Instance.PlayOneShotSFXSound(_clickClip);
        }
    }
}