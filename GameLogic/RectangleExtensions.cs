using Microsoft.Xna.Framework;

namespace GameLogic
{
    public static class RectangleExtensions
    {
        public static Rectangle GetAlignedToCenterBorder(this Rectangle container, int width, int height)
        {
            var leftOffset = container.X + container.Width / 2 - width / 2;
            var topOffset = container.Y + container.Height / 2 - height / 2;

            return new Rectangle(leftOffset, topOffset, width, height);
        }

        public static Rectangle GetTopLeftPart(this Rectangle container, int width, int height)
        {
            return new Rectangle(container.X, container.Y, width, height);
        }

        public static Rectangle GetTopRightPart(this Rectangle container, int width, int height)
        {
            return new Rectangle(container.X + container.Width - width, container.Y, width, height);
        }

        public static Rectangle GetBotLeftPart(this Rectangle container, int width, int height)
        {
            return new Rectangle(container.X, container.Y + container.Height - height, width, height);
        }

        public static Rectangle GetInnerBorder(this Rectangle container, int leftRightPadding, int topBottomPadding)
        {
            return new Rectangle(container.X + leftRightPadding,
                container.Y + topBottomPadding,
                container.Width - leftRightPadding * 2,
                container.Height - topBottomPadding * 2);
        }
    }
}