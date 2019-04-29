using UnityEngine;

namespace BToolkit
{
    public abstract class CardCurler_Method
    {
        protected CardCurler cardCurler;
        internal Vector2 dragPos, leftPoint, rightPoint;
        public enum AnimState
        {
            Drag,
            Resume,
            TurnOver
        }
        public enum Side
        {
            Left,
            Right,
            Top,
            Bottom
        }
        public AnimState currAnimState = AnimState.Drag;
        public Side currSide = Side.Left;
        protected float turnOverSpeed = 0.1f;

        public CardCurler_Method(CardCurler cardCurler,Side side)
        {
            this.cardCurler = cardCurler;
            currSide = side;
        }

        public abstract void SetTargetsPivot(Vector2 touchPos);

        public abstract void Update();

    }
}