using UnityEngine;
using UnityEngine.Events;

namespace Assets.ChallengeFriday.API.UI.Navigation
{
    public class ScreenBase : Tweenable 
    {
        public string ScreenName;
        public bool StartDisabled = true;

        public UnityEvent OnFocusGained;
        public UnityEvent OnFocusLost;

        internal bool IsFocused;
        internal bool DisableOnUnfocus = true;
        private bool _scheduleDisable;
        private float _disableTime;

        public void Update()
        {
            if (!DisableOnUnfocus) return;
            if (!_scheduleDisable) return;
            if (_disableTime > Time.time) return;
            _scheduleDisable = false;
            gameObject.SetActive(false);
        }

        public void LateUpdate()
        {
            if (StartDisabled)
            {
                gameObject.SetActive(false);
                StartDisabled = false;
            }
        }

        public void SetFocus()
        {
            if (StartDisabled) StartDisabled = false;
            IsFocused = true;
            if (OnFocusGained == null) return;
            OnFocusGained.Invoke();
            gameObject.SetActive(true);
            _scheduleDisable = false;
        }

        public void UnsetFocus()
        {
            IsFocused = false;
            if (OnFocusLost == null) return;
            OnFocusLost.Invoke();
            _scheduleDisable = true;
            _disableTime = Time.time + base.Duration;
        }
    }
}
