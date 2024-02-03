using System.Drawing;

namespace cFlowForms
{
    internal class MoveMapByMouse
    {
        private bool isRightButtonDown = false;
        private System.Drawing.Point mouseOldPos = new();

        //curried updater for current movement
        private Action<System.Drawing.Point> updatePositionBy = delta => { };

        //provided from outside
        private Action<System.Drawing.Point> setPosition;
        private Func<System.Drawing.Point> getPosition;

        public MoveMapByMouse(Control form, Action<System.Drawing.Point> setPosition, Func<System.Drawing.Point> getPosition)
        {
            this.getPosition = getPosition;
            this.setPosition = setPosition;

            form.MouseDown += MainForm_MouseDown;
            form.MouseUp += MainForm_MouseUp;
            form.MouseMove += MainForm_MouseMove;
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(e.Button == MouseButtons.Right))
                return;

            isRightButtonDown = false;

        }


        private void MainForm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var currentCenter = getPosition();
                updatePositionBy = delta => {
                    setPosition(new System.Drawing.Point(currentCenter.X + delta.X, currentCenter.Y + delta.Y));
                };
                isRightButtonDown = true;
                mouseOldPos = new System.Drawing.Point(e.X, e.Y);

            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightButtonDown)
            {
                // Code to run while the right mouse button is down and the mouse is moving
                var delta = new System.Drawing.Point(mouseOldPos.X - e.X, mouseOldPos.Y - e.Y);
                updatePositionBy(delta);
            }
        }

    }
}
