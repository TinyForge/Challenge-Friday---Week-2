using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ChallengeFriday.API.UI.Navigation
{
    public class NavigationBase : MonoBehaviour 
    {

        public ScreenBase[] Screens;
        public string StartScreen;
        public bool AutoStart = true;
        public bool LoopNavigation = true;
        public float DistantanceBetweenScreens = 1.2f;
        public UnityEvent StartEvent;
        public UnityEvent NavigationEndEvent;

        public ScreenBase CurrentScreen { get; private set; }
        public ScreenBase PreviousScreen { get; private set; }

        protected Vector3 GridCenter;
        private Vector3 _transitionR;
        private Vector3 _transitionL;
        private Vector3 _transitionT;
        private Vector3 _transitionB;
        public int CurrentScreenId { get; private set; }

        protected void ResetNavigation()
        {
            CurrentScreen = null;
            PreviousScreen = null;
            CurrentScreenId = 0;
        }

        protected void Setup()
        {
            GridCenter = transform.localPosition;
            _transitionR = GridCenter + (Vector3.right * Screen.width * DistantanceBetweenScreens);
            _transitionL = GridCenter - (Vector3.right * Screen.width * DistantanceBetweenScreens);
            _transitionT = GridCenter + (Vector3.up * Screen.height * DistantanceBetweenScreens);
            _transitionB = GridCenter - (Vector3.up * Screen.height * DistantanceBetweenScreens);
        }

        public void Start()
        {
            Setup();
            if (AutoStart)
                NavigateToScreenLeft(StartScreen);
        }

        public void Startup()
        {
            if (StartEvent == null) return;
            StartEvent.Invoke();
        }

        public void LoadStartScreen()
        {
            NavigateToScreenLeft(StartScreen);
        }

        public void NavigateToScreenRight(string screenName)
        {
            NavigateToScreen(screenName, Direction.Right);
        }

        public void NavigateToScreenLeft(string screenName)
        {
            NavigateToScreen(screenName, Direction.Left);
        }

        public void NavigateToScreenTop(string screenName)
        {
            NavigateToScreen(screenName, Direction.Top);
        }

        public void NavigateToScreenBottom(string screenName)
        {
            NavigateToScreen(screenName, Direction.Bottom);
        }

        public void NavigateToNextScreen()
        {
            NavigateToScreen(GetNextScreenName(), Direction.Right);
        }

        public void NavigateToPreviousScreen()
        {
            NavigateToScreen(GetPreviousScreenName(), Direction.Left);
        }

        public void SetCurrentScreen(string screenName)
        {
            LoadScreen(GetScreenByName(screenName), Direction.Right, false);
        }

        public virtual void FocusScreen(ScreenBase screen, Direction direction, bool doTransition)
        {
            if (screen == null) return;
            if (doTransition)
                screen.MoveTo(GetGridLocationByDirection(direction), GridCenter);
            screen.SetFocus();
        }

        public virtual void UnfocusScreen(ScreenBase screen, Direction direction)
        {
            if (screen == null) return;
            screen.MoveTo(GridCenter, GetOppositeGridLocationByDirection(direction));
            screen.UnsetFocus();
        }

        private string GetNextScreenName()
        {
            var nextScreenId = CurrentScreenId + 1;
            if (nextScreenId >= Screens.Count())
            {
                nextScreenId = LoopNavigation ? 0 : CurrentScreenId;
                if (!LoopNavigation && NavigationEndEvent != null)
                    NavigationEndEvent.Invoke();
            }

            return Screens[nextScreenId].ScreenName;
        }

        private string GetPreviousScreenName()
        {
            var nextScreenId = CurrentScreenId - 1;
            if (nextScreenId < 0)
                nextScreenId = LoopNavigation ? Screens.Count() - 1 : CurrentScreenId;

            return Screens[nextScreenId].ScreenName;
        }


        private void NavigateToScreen(string screenName, Direction direction)
        {
            if (screenName.ToLower() == "previous")
                screenName = PreviousScreen.ScreenName;

            if (IsCurrentScreen(screenName)) return;

            LoadScreen(GetScreenByName(screenName), direction);
        }

        private bool IsCurrentScreen(string screenName)
        {
            return (CurrentScreen != null && CurrentScreen.ScreenName == screenName);
        }

        private ScreenBase GetScreenByName(string screenName)
        {
            ScreenBase screen = null;
            for (var i = 0; i < Screens.Count(); i++)
            {
                if (screenName != Screens[i].ScreenName) continue;
                screen = Screens[i];
                CurrentScreenId = i;
                break;
            }
            return screen;
        }

        private void LoadScreen(ScreenBase screen, Direction direction, bool doTransition = true)
        {
            if (screen == null) return;
            if (CurrentScreen != null)
                PreviousScreen = CurrentScreen;

            FocusScreen(screen, direction, doTransition);
            UnfocusScreen(CurrentScreen, direction);
            CurrentScreen = screen;
        }

        private Vector3 GetGridLocationByDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return _transitionR;
                case Direction.Left:
                    return _transitionL;
                case Direction.Top:
                    return _transitionT;
                case Direction.Bottom:
                    return _transitionB;
            }
            return _transitionR;
        }

        private Vector3 GetOppositeGridLocationByDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return _transitionL;
                case Direction.Left:
                    return _transitionR;
                case Direction.Top:
                    return _transitionB;
                case Direction.Bottom:
                    return _transitionT;
            }
            return _transitionL;
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
