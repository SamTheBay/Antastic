
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Antastic
{
    /// <summary>
    /// This class handles all keyboard and gamepad actions in the game.
    /// </summary>
    public static class InputManager
    {
        private static TouchCollection TouchState;
        private static readonly List<GestureSample> Gestures = new List<GestureSample>();
        private static ButtonState PreviousBackState = ButtonState.Released;
        private static bool BackTriggered = false;
        private static bool PreviousSpaceState = false;

        public static void Update()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && PreviousBackState == ButtonState.Released)
            {
                BackTriggered = true;
            }
            else
            {
                BackTriggered = false;
            }
            PreviousBackState = GamePad.GetState(PlayerIndex.One).Buttons.Back;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true && PreviousSpaceState == false)
            {
                ScreenManager.frozen = !ScreenManager.frozen;
            }
            PreviousSpaceState = Keyboard.GetState().IsKeyDown(Keys.Space);

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }


        public static bool IsLocationPressed(Rectangle loc)
        {
            for (int i = 0; i < TouchState.Count; i++)
            {
                if ((TouchState[i].State == TouchLocationState.Pressed || TouchState[i].State == TouchLocationState.Moved) && loc.Contains((int)TouchState[i].Position.X, (int)TouchState[i].Position.Y))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsLocationTapped(Rectangle loc)
        {
            for (int i = 0; i < Gestures.Count; i++)
            {
                // Note: we translate the coordinate plane for the rotation in line here...
                if (Gestures[i].GestureType == GestureType.Tap && loc.Contains((int)Gestures[i].Position.X, (int)Gestures[i].Position.Y))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsLocationHeld(Rectangle loc)
        {
            for (int i = 0; i < Gestures.Count; i++)
            {
                // Note: we translate the coordinate plane for the rotation in line here...
                if (Gestures[i].GestureType == GestureType.Hold && loc.Contains((int)Gestures[i].Position.X, (int)Gestures[i].Position.Y))
                {
                    return true;
                }
            }
            return false;
        }

        public static Direction IsSwipe()
        {
            for (int i = 0; i < Gestures.Count; i++)
            {
                // Note: we translate the coordinate plane for the rotation in line here...
                if (Gestures[i].GestureType == GestureType.Flick)
                {
                    if (Gestures[i].Delta.X > Gestures[i].Delta2.X)
                    {
                        return Direction.Left;
                    }
                    if (Gestures[i].Delta.X < Gestures[i].Delta2.X)
                    {
                        return Direction.Right;
                    }
                }
            }
            return Direction.Up;
        }

        public static bool GetTouchPoint(ref Vector2 touchPoint)
        {
            for (int i = 0; i < TouchState.Count; i++)
            {
                if ((TouchState[i].State == TouchLocationState.Pressed || TouchState[i].State == TouchLocationState.Moved))
                {
                    touchPoint = TouchState[i].Position;
                    return true;
                }
            }
            return false;
        }

        public static bool GetTapPoint(ref Vector2 touchPoint)
        {
            for (int i = 0; i < Gestures.Count; i++)
            {
                // Note: we translate the coordinate plane for the rotation in line here...
                if (Gestures[i].GestureType == GestureType.Tap)
                {
                    touchPoint = Gestures[i].Position;
                    return true;
                }
            }
            return false;
        }


        public static bool GetPress(ref Vector2 pressLoc)
        {
            if (TouchState.Count == 1 && TouchState[0].State == TouchLocationState.Pressed)
            {
                pressLoc = TouchState[0].Position;
                return true;
            }
            return false;
        }


        public static bool IsBackTriggered()
        {
            return BackTriggered;
        }



        static public TouchCollection GetTouchCollection()
        {
            return TouchState;
        }

        static public List<GestureSample> GetGestures()
        {
            return Gestures;
        }




        static bool currentlyDragging = false;
        static int currentDragId = 0;
        static Vector2 currentDragPosition;
        static Vector2 previousDragPosition;
        public static bool GetDrag(ref Vector2 position, ref bool isStart, ref bool isFinished, ref Vector2 direction, ref float magnitude)
        {
            isStart = false;
            isFinished = false;

            if (!currentlyDragging && TouchState.Count > 0)
            {
                // we are starting a drag
                for (int i = 0; i < TouchState.Count; i++)
                {
                    if (TouchState[i].State == TouchLocationState.Pressed)
                    {
                        isStart = true;
                        currentDragId = TouchState[i].Id;
                        currentDragPosition = TouchState[i].Position;
                        previousDragPosition = TouchState[i].Position;
                        currentlyDragging = true;
                        break;
                    }
                }
            }
            else if (currentlyDragging)
            {
                // find the touch point for our current drag
                bool dragFound = false;
                for (int i = 0; i < TouchState.Count; i++)
                {
                    if (TouchState[i].Id == currentDragId && (TouchState[i].State == TouchLocationState.Pressed || TouchState[i].State == TouchLocationState.Moved))
                    {
                        previousDragPosition = currentDragPosition;
                        currentDragPosition = TouchState[i].Position;
                        dragFound = true;
                        break;
                    }
                }

                if (!dragFound)
                {
                    isFinished = true;
                }
            }

            if (currentlyDragging)
            {
                position = currentDragPosition;
                direction = currentDragPosition - previousDragPosition;
                magnitude = Math.Abs(Vector2.Distance(currentDragPosition, previousDragPosition));
            }



            if (isFinished == true)
            {
                currentlyDragging = false;
            }


            return currentlyDragging;
        }


    }
}
