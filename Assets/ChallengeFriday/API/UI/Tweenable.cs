using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ChallengeFriday.API.UI
{
    public class Tweenable : MonoBehaviour
    {
        public float Duration = 1;
        public iTween.EaseType EaseType = iTween.EaseType.spring;

        private Vector3 _startLocation;

        void Start()
        {
            _startLocation = transform.position;
        }

        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        public void MoveTo(Vector3 fromLocation, Vector3 toLocation)
        {
            transform.position = fromLocation;
            MoveToPosition(toLocation);
        }

        public void MoveTo(GameObject fromLocation, GameObject toLocation)
        {
            transform.position = fromLocation.transform.position;
            MoveTo(toLocation);
        }

        public void MoveTo(GameObject toLocation)
        {
            MoveToPosition(toLocation.transform.position);
        }

        private void MoveToPosition(Vector3 location)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", location, "easetype", EaseType, "time", Duration));
        }

        public void MoveToStartLocation()
        {
            MoveToPosition(_startLocation);
        }

        public void SpringOnX(float x)
        {
            iTween.MoveTo(gameObject, iTween.Hash("x", gameObject.transform.position.x + x, "easetype", EaseType, "time", Duration));
        }

        public void PunchScale()
        {
            iTween.PunchScale(gameObject, new Vector3(0.5f, 0.5f, 0.5f), Duration);
        }

        public void PunchRotation()
        {
            iTween.PunchRotation(gameObject, new Vector3(0, 0, 180), Duration);
        }

        public void RotateFrom(GameObject fromLocation)
        {
            iTween.RotateFrom(gameObject, fromLocation.transform.eulerAngles, Duration);
        }

        public void FilImageTo(float amount)
        {
            //_newImageFillAmount = amount;

            iTween.ValueTo(gameObject,
                iTween.Hash(
                    "from", GetComponent<Image>().fillAmount,
                    "to", amount,
                    "time", 1f,
                    "onupdatetarget", gameObject,
                    "onupdate", "TweenOnUpdateCallBack",
                    "easetype", EaseType
                    )
                );
        }

        void TweenOnUpdateCallBack(float newValue)
        {
            GetComponent<Image>().fillAmount = newValue;
        }
    }
}
