using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Numerics;
using System.Collections.ObjectModel;
//using static PacMan.PacManComponent;

namespace PacMan
{
    #region Enums
    public enum ePages { Menu, Game }
    public enum eDirections { Up, Right, Down, Left, Idle }
    #endregion Enums

    public class PacManComponent : Control
    {
        #region Variabes and constatns

        private const double _Tickrate = 16;

        private static System.Timers.Timer _Timer = new System.Timers.Timer(_Tickrate);

        Random _Random = new Random();

        private static ePages _CurrentPage = ePages.Menu;

        private int _Score = 0;

        private int _MaxScore = -1;

        private int _Lives = 3; 

        private string _MapPath = "";

        private int[,] _NavMap =
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

        private Player _Player = null;

        private List<Wall> _Walls = new List<Wall>();

        private Ghost[] _Ghosts = new Ghost[4];

        private Pickup[] _Pickups = new Pickup[868];

        public delegate void MethodContainer();
        public event MethodContainer OnMatchLoose;
        public event MethodContainer OnMatchWin;
        #endregion

        #region Properties
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams _CreateParams = base.CreateParams;
                _CreateParams.ExStyle = 0x02000000;
                return _CreateParams;
            }
        }
        public int Score { get => _Score; }
        public int Lives { get => _Lives; }
        public string MapPath
        {
            get => _MapPath; 
            set
            {
                if (_MapPath != value)
                {
                    _MapPath = value;
                    if (_MapPath != "")
                    {
                        try
                        {
                            _CurrentPage = ePages.Game;
                            LoadMap(_MapPath);
                        }
                        catch (Exception in_Exception)
                        {
                            MessageBox.Show(in_Exception.Message);
                        }
                    }
                } 
            } 
        }

        #endregion

        #region Constructors

        public PacManComponent()
        {
            _Timer.Elapsed += new ElapsedEventHandler(Tick);
            _Timer.Enabled = false;
        }

        #endregion

        #region Control class override voids

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

            if (_CurrentPage == ePages.Menu)
            {
                e.Graphics.DrawImage((Bitmap)Properties.Resources.Menu, new RectangleF(0, 0, 392, 434));
            }
            else if (_CurrentPage == ePages.Game)
            {
                if (_Player != null)
                {
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(_Player.Image), new RectangleF(_Player.Transform.X, _Player.Transform.Y, _Player.Transform.Width, _Player.Transform.Height));
                }
                foreach (Wall _CurrentWall in _Walls){e.Graphics.DrawImage((Bitmap)Properties.Resources.Wall, new RectangleF(_CurrentWall.Transform.X, _CurrentWall.Transform.Y, _CurrentWall.Transform.Width, _CurrentWall.Transform.Height));}
                for(int i=0;i<_Ghosts.Length;i++)
                {
                    if (_Ghosts[i]!=null)
                    {
                        e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(_Ghosts[i].Image), new RectangleF(_Ghosts[i].Transform.X, _Ghosts[i].Transform.Y, _Ghosts[i].Transform.Width, _Ghosts[i].Transform.Height));

                    }
                }
                for (int _CurrentPickup=0; _CurrentPickup< _Pickups.Length; _CurrentPickup++)
                {
                    if (_Pickups[_CurrentPickup] != null && _Pickups != null)
                    {
                        e.Graphics.FillEllipse(new SolidBrush(Color.Yellow), new Rectangle(_Pickups[_CurrentPickup].Transform.X, _Pickups[_CurrentPickup].Transform.Y, _Pickups[_CurrentPickup].Transform.Width, _Pickups[_CurrentPickup].Transform.Height));
                    }
                }
                try
                {
                    e.Graphics.DrawString("Score:" + _Score.ToString() + "/" + _MaxScore.ToString() + " Energized:" + (int)(_Player?.EnergizedTicks / _Tickrate) + "s" + " Lives:" + _Lives, this.Font, Brushes.White, 0, 0);
                }
                catch (Exception in_Exception)
                {
                    Console.WriteLine(in_Exception);
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_CurrentPage == ePages.Menu)
            {
                // This IF provide click over "New" button in menu
                if (MouseOverZone(e.X, e.Y, new Rectangle(38, 0, 310, 68)))
                {
                    StartMatch();
                    NewMap();
                    LoadMap("NewMap.txt");
                    _Timer.Enabled = true;
                }

                // This IF provide click over "Load" button in menu
                if (MouseOverZone(e.X, e.Y, new Rectangle(38, 92, 310, 68)))
                {
                    OpenFileDialog _OpenFileDialog = new OpenFileDialog();
                    _OpenFileDialog.Multiselect = false;
                    if (_OpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        StartMatch();
                        LoadMap(_OpenFileDialog.FileNames[0]);
                        _Timer.Enabled = true;
                    }
                }

            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_CurrentPage == ePages.Game)
            {
                switch (e.KeyValue)
                {
                    case 87: _Player.PlannedDirection = eDirections.Up; break;
                    case 65: _Player.PlannedDirection = eDirections.Left; break;
                    case 83: _Player.PlannedDirection = eDirections.Down; break;
                    case 68: _Player.PlannedDirection = eDirections.Right; break;
                }
            }
            if(_CurrentPage == ePages.Game && _Timer.Enabled == false)
            {
                _Timer.Enabled = true;
            }

        }

        #endregion

        #region Game related voids and functions

        private static bool MouseOverZone(int in_MouseX, int in_MouseY, Rectangle in_Zone)
        {
            if (in_MouseX > in_Zone.X && in_MouseX < in_Zone.X + in_Zone.Width && in_MouseY > in_Zone.Y && in_MouseY < in_Zone.Y + in_Zone.Height)
            { return true; }
            else { return false; }
        }

        private void Tick(object in_source, ElapsedEventArgs in_e)
        {
            if (_CurrentPage == ePages.Game)
            {
                _Player.Tick();

                for(int i = 0; i < _Ghosts.Length;i++)
                {
                    _Ghosts[i].Tick();
                    if (isOverlaping(_Player.Transform, _Ghosts[i].Transform) && _Player.EnergizedTicks == 0)
                    {
                        _Lives--;
                        Point _RandomRespawnPoint = GetRandomNonWallPoint();
                        _Player.Transform.X = _RandomRespawnPoint.X * 14;
                        _Player.Transform.Y = _RandomRespawnPoint.Y * 14;
                    }
                }

                for (int i = 0; i < _Pickups.Length; i++)
                {
                    if (_Pickups[i] != null)
                    {
                        if (isOverlaping(_Player.Transform, _Pickups[i].Transform))
                        {
                            if (_Pickups[i].Energized)
                            {
                                _Player.EnergizedTicks += 600;
                            }
                            else
                            {
                                _Score++;
                            }
                            _Pickups[i] = null;
                        }
                    }

                }

                if (_Lives <= 0)
                {
                    _CurrentPage = ePages.Menu;
                    _Timer.Enabled = false;
                    OnMatchLoose();
                    CloseMatch();
                }

                if (_Score >=_MaxScore)
                {
                    _CurrentPage = ePages.Menu;
                    _Timer.Enabled = false;
                    OnMatchWin();
                    CloseMatch();
                }

            }

            this.Invalidate();
        }

        private static bool isOverlaping(Transform in_FirstObject, Transform in_SecondObject)
        {
            if ((in_FirstObject.X + in_FirstObject.Width >= in_SecondObject.X) && (in_FirstObject.X <= in_SecondObject.X + in_SecondObject.Width) && (in_FirstObject.Y + in_FirstObject.Height >= in_SecondObject.Y) && (in_FirstObject.Y <= in_SecondObject.Y + in_SecondObject.Height))
            { return true; }
            else { return false; }
        }

        private static void NewMap()
        {
            char[,] _Map =
            {
                { 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w', 'w', 'w', ' ', ' ', 'w', 'w', 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w', ' ', ' ', ' ', ' ', ' ', ' ', 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w', ' ', 'g', 'g', 'g', 'g', ' ', 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w', ' ', ' ', ' ', ' ', ' ', ' ', 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', 'p', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'w' },
                { 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w', 'w' }
            };
            string _MapText = "";
            Random _Rnd = new Random();
            int _Top = _Rnd.Next(1, 4);
            int _Side = _Rnd.Next(1, 4);
            int _Bottom = _Rnd.Next(1, 4);
            int _Row = 0;
            int _Col = 0;
            int _RndRow = 0;
            int _RndCol = 0;
            int _EnergizerCount = 0;

            switch (_Top)
            {
                case 1:
                    _Map[3, 3] = 'w';
                    _Map[11, 3] = 'w';
                    _Map[3, 4] = 'w';
                    _Map[11, 4] = 'w';
                    _Map[3, 5] = 'w';
                    _Map[4, 5] = 'w';
                    _Map[5, 5] = 'w';
                    _Map[6, 5] = 'w';
                    _Map[7, 5] = 'w';
                    _Map[11, 5] = 'w';
                    _Map[11, 6] = 'w';
                    _Map[3, 7] = 'w';
                    _Map[4, 7] = 'w';
                    _Map[5, 7] = 'w';
                    _Map[6, 7] = 'w';
                    _Map[7, 7] = 'w';
                    _Map[10, 7] = 'w';
                    _Map[11, 7] = 'w';
                    _Map[3, 10] = 'w';
                    _Map[10, 10] = 'w';
                    _Map[3, 11] = 'w';
                    _Map[10, 11] = 'w';
                    _Map[3, 12] = 'w';
                    _Map[10, 12] = 'w';
                    _Map[3, 13] = 'w';
                    _Map[6, 13] = 'w';
                    _Map[7, 13] = 'w';
                    _Map[8, 13] = 'w';
                    _Map[9, 13] = 'w';
                    _Map[10, 13] = 'w';
                    _Map[3, 14] = 'w';
                    _Map[6, 14] = 'w';
                    _Map[7, 14] = 'w';
                    _Map[8, 14] = 'w';
                    _Map[9, 14] = 'w';
                    _Map[10, 14] = 'w';
                    _Map[3, 15] = 'w';
                    _Map[10, 15] = 'w';
                    _Map[3, 16] = 'w';
                    _Map[10, 16] = 'w';
                    _Map[3, 17] = 'w';
                    _Map[10, 17] = 'w';
                    _Map[3, 20] = 'w';
                    _Map[4, 20] = 'w';
                    _Map[5, 20] = 'w';
                    _Map[6, 20] = 'w';
                    _Map[7, 20] = 'w';
                    _Map[10, 20] = 'w';
                    _Map[11, 20] = 'w';
                    _Map[11, 21] = 'w';
                    _Map[3, 22] = 'w';
                    _Map[4, 22] = 'w';
                    _Map[5, 22] = 'w';
                    _Map[6, 22] = 'w';
                    _Map[7, 22] = 'w';
                    _Map[11, 22] = 'w';
                    _Map[3, 23] = 'w';
                    _Map[11, 23] = 'w';
                    _Map[3, 24] = 'w';
                    _Map[11, 24] = 'w';
                    break;
                case 2:
                    _Map[3, 3] = 'w';
                    _Map[4, 3] = 'w';
                    _Map[5, 3] = 'w';
                    _Map[6, 3] = 'w';
                    _Map[7, 3] = 'w';
                    _Map[10, 3] = 'w';
                    _Map[11, 3] = 'w';
                    _Map[3, 4] = 'w';
                    _Map[10, 4] = 'w';
                    _Map[11, 4] = 'w';
                    _Map[3, 5] = 'w';
                    _Map[10, 5] = 'w';
                    _Map[11, 5] = 'w';
                    _Map[6, 7] = 'w';
                    _Map[6, 8] = 'w';
                    _Map[7, 8] = 'w';
                    _Map[8, 8] = 'w';
                    _Map[9, 8] = 'w';
                    _Map[1, 13] = 'w';
                    _Map[2, 13] = 'w';
                    _Map[3, 13] = 'w';
                    _Map[4, 13] = 'w';
                    _Map[5, 13] = 'w';
                    _Map[6, 13] = 'w';
                    _Map[7, 13] = 'w';
                    _Map[8, 13] = 'w';
                    _Map[9, 13] = 'w';
                    _Map[1, 14] = 'w';
                    _Map[2, 14] = 'w';
                    _Map[3, 14] = 'w';
                    _Map[4, 14] = 'w';
                    _Map[5, 14] = 'w';
                    _Map[6, 14] = 'w';
                    _Map[7, 14] = 'w';
                    _Map[8, 14] = 'w';
                    _Map[9, 14] = 'w';
                    _Map[6, 19] = 'w';
                    _Map[7, 19] = 'w';
                    _Map[8, 19] = 'w';
                    _Map[9, 19] = 'w';
                    _Map[6, 20] = 'w';
                    _Map[3, 22] = 'w';
                    _Map[10, 22] = 'w';
                    _Map[11, 22] = 'w';
                    _Map[3, 23] = 'w';
                    _Map[10, 23] = 'w';
                    _Map[11, 23] = 'w';
                    _Map[3, 24] = 'w';
                    _Map[4, 24] = 'w';
                    _Map[5, 24] = 'w';
                    _Map[6, 24] = 'w';
                    _Map[7, 24] = 'w';
                    _Map[10, 24] = 'w';
                    _Map[11, 24] = 'w';
                    break;
                case 3:
                    _Map[4, 3] = 'w';
                    _Map[5, 3] = 'w';
                    _Map[6, 3] = 'w';
                    _Map[7, 3] = 'w';
                    _Map[8, 3] = 'w';
                    _Map[9, 3] = 'w';
                    _Map[4, 4] = 'w';
                    _Map[5, 4] = 'w';
                    _Map[6, 4] = 'w';
                    _Map[7, 4] = 'w';
                    _Map[8, 4] = 'w';
                    _Map[9, 4] = 'w';
                    _Map[4, 5] = 'w';
                    _Map[5, 5] = 'w';
                    _Map[6, 5] = 'w';
                    _Map[7, 5] = 'w';
                    _Map[8, 5] = 'w';
                    _Map[9, 5] = 'w';
                    _Map[4, 6] = 'w';
                    _Map[5, 6] = 'w';
                    _Map[6, 6] = 'w';
                    _Map[7, 6] = 'w';
                    _Map[8, 6] = 'w';
                    _Map[9, 6] = 'w';
                    _Map[3, 9] = 'w';
                    _Map[4, 9] = 'w';
                    _Map[5, 9] = 'w';
                    _Map[6, 9] = 'w';
                    _Map[7, 9] = 'w';
                    _Map[8, 9] = 'w';
                    _Map[9, 9] = 'w';
                    _Map[10, 9] = 'w';
                    _Map[3, 12] = 'w';
                    _Map[10, 12] = 'w';
                    _Map[3, 13] = 'w';
                    _Map[10, 13] = 'w';
                    _Map[3, 14] = 'w';
                    _Map[10, 14] = 'w';
                    _Map[3, 15] = 'w';
                    _Map[10, 15] = 'w';
                    _Map[3, 18] = 'w';
                    _Map[4, 18] = 'w';
                    _Map[5, 18] = 'w';
                    _Map[6, 18] = 'w';
                    _Map[7, 18] = 'w';
                    _Map[8, 18] = 'w';
                    _Map[9, 18] = 'w';
                    _Map[10, 18] = 'w';
                    _Map[4, 21] = 'w';
                    _Map[5, 21] = 'w';
                    _Map[6, 21] = 'w';
                    _Map[7, 21] = 'w';
                    _Map[8, 21] = 'w';
                    _Map[9, 21] = 'w';
                    _Map[4, 22] = 'w';
                    _Map[5, 22] = 'w';
                    _Map[6, 22] = 'w';
                    _Map[7, 22] = 'w';
                    _Map[8, 22] = 'w';
                    _Map[9, 22] = 'w';
                    _Map[4, 23] = 'w';
                    _Map[5, 23] = 'w';
                    _Map[6, 23] = 'w';
                    _Map[7, 23] = 'w';
                    _Map[8, 23] = 'w';
                    _Map[9, 23] = 'w';
                    _Map[4, 24] = 'w';
                    _Map[5, 24] = 'w';
                    _Map[6, 24] = 'w';
                    _Map[7, 24] = 'w';
                    _Map[8, 24] = 'w';
                    _Map[9, 24] = 'w';
                    break;
            }
            switch (_Side)
            {
                case 1:
                    _Map[16, 3] = 'w';
                    _Map[16, 4] = 'w';
                    _Map[14, 5] = 'w';
                    _Map[15, 5] = 'w';
                    _Map[16, 5] = 'w';
                    _Map[16, 6] = 'w';
                    _Map[16, 7] = 'w';
                    _Map[16, 20] = 'w';
                    _Map[16, 21] = 'w';
                    _Map[14, 22] = 'w';
                    _Map[15, 22] = 'w';
                    _Map[16, 22] = 'w';
                    _Map[16, 23] = 'w';
                    _Map[16, 24] = 'w';
                    break;
                case 2:
                    _Map[13, 3] = 'w';
                    _Map[14, 3] = 'w';
                    _Map[14, 4] = 'w';
                    _Map[15, 4] = 'w';
                    _Map[15, 6] = 'w';
                    _Map[16, 6] = 'w';
                    _Map[16, 7] = 'w';
                    _Map[17, 7] = 'w';
                    _Map[16, 20] = 'w';
                    _Map[17, 20] = 'w';
                    _Map[15, 21] = 'w';
                    _Map[16, 21] = 'w';
                    _Map[14, 23] = 'w';
                    _Map[15, 23] = 'w';
                    _Map[13, 24] = 'w';
                    _Map[14, 24] = 'w';
                    break;
                case 3:
                    _Map[13, 2] = 'w';
                    _Map[13, 3] = 'w';
                    _Map[13, 4] = 'w';
                    _Map[13, 5] = 'w';
                    _Map[17, 5] = 'w';
                    _Map[17, 6] = 'w';
                    _Map[17, 7] = 'w';
                    _Map[17, 8] = 'w';
                    _Map[17, 19] = 'w';
                    _Map[17, 20] = 'w';
                    _Map[17, 21] = 'w';
                    _Map[13, 22] = 'w';
                    _Map[17, 22] = 'w';
                    _Map[13, 23] = 'w';
                    _Map[13, 24] = 'w';
                    _Map[13, 25] = 'w';
                    break;
            }
            switch (_Bottom)
            {
                case 1:
                    _Map[19, 2] = 'w';
                    _Map[20, 2] = 'w';
                    _Map[23, 2] = 'w';
                    _Map[20, 3] = 'w';
                    _Map[23, 3] = 'w';
                    _Map[26, 3] = 'w';
                    _Map[23, 4] = 'w';
                    _Map[26, 4] = 'w';
                    _Map[23, 5] = 'w';
                    _Map[26, 5] = 'w';
                    _Map[23, 6] = 'w';
                    _Map[21, 7] = 'w';
                    _Map[22, 7] = 'w';
                    _Map[23, 7] = 'w';
                    _Map[23, 8] = 'w';
                    _Map[27, 8] = 'w';
                    _Map[28, 8] = 'w';
                    _Map[29, 8] = 'w';
                    _Map[23, 9] = 'w';
                    _Map[25, 12] = 'w';
                    _Map[21, 13] = 'w';
                    _Map[22, 13] = 'w';
                    _Map[23, 13] = 'w';
                    _Map[24, 13] = 'w';
                    _Map[25, 13] = 'w';
                    _Map[21, 14] = 'w';
                    _Map[22, 14] = 'w';
                    _Map[23, 14] = 'w';
                    _Map[24, 14] = 'w';
                    _Map[25, 14] = 'w';
                    _Map[25, 15] = 'w';
                    _Map[23, 18] = 'w';
                    _Map[23, 19] = 'w';
                    _Map[27, 19] = 'w';
                    _Map[28, 19] = 'w';
                    _Map[29, 19] = 'w';
                    _Map[21, 20] = 'w';
                    _Map[22, 20] = 'w';
                    _Map[23, 20] = 'w';
                    _Map[23, 21] = 'w';
                    _Map[23, 22] = 'w';
                    _Map[26, 22] = 'w';
                    _Map[23, 23] = 'w';
                    _Map[26, 23] = 'w';
                    _Map[20, 24] = 'w';
                    _Map[23, 24] = 'w';
                    _Map[26, 24] = 'w';
                    _Map[19, 25] = 'w';
                    _Map[20, 25] = 'w';
                    _Map[23, 25] = 'w';
                    break;
                case 2:
                    _Map[24, 3] = 'w';
                    _Map[25, 3] = 'w';
                    _Map[26, 3] = 'w';
                    _Map[27, 3] = 'w';
                    _Map[27, 4] = 'w';
                    _Map[27, 5] = 'w';
                    _Map[23, 6] = 'w';
                    _Map[27, 6] = 'w';
                    _Map[23, 7] = 'w';
                    _Map[23, 8] = 'w';
                    _Map[23, 9] = 'w';
                    _Map[20, 10] = 'w';
                    _Map[23, 10] = 'w';
                    _Map[24, 10] = 'w';
                    _Map[25, 10] = 'w';
                    _Map[26, 10] = 'w';
                    _Map[27, 10] = 'w';
                    _Map[28, 10] = 'w';
                    _Map[29, 10] = 'w';
                    _Map[20, 11] = 'w';
                    _Map[20, 12] = 'w';
                    _Map[20, 13] = 'w';
                    _Map[27, 13] = 'w';
                    _Map[28, 13] = 'w';
                    _Map[29, 13] = 'w';
                    _Map[20, 14] = 'w';
                    _Map[27, 14] = 'w';
                    _Map[28, 14] = 'w';
                    _Map[29, 14] = 'w';
                    _Map[20, 15] = 'w';
                    _Map[20, 16] = 'w';
                    _Map[20, 17] = 'w';
                    _Map[23, 17] = 'w';
                    _Map[24, 17] = 'w';
                    _Map[25, 17] = 'w';
                    _Map[26, 17] = 'w';
                    _Map[27, 17] = 'w';
                    _Map[28, 17] = 'w';
                    _Map[29, 17] = 'w';
                    _Map[23, 18] = 'w';
                    _Map[23, 19] = 'w';
                    _Map[23, 20] = 'w';
                    _Map[23, 21] = 'w';
                    _Map[27, 21] = 'w';
                    _Map[27, 22] = 'w';
                    _Map[27, 23] = 'w';
                    _Map[24, 24] = 'w';
                    _Map[25, 24] = 'w';
                    _Map[26, 24] = 'w';
                    _Map[27, 24] = 'w';
                    break;
                case 3:
                    _Map[19, 3] = 'w';
                    _Map[20, 3] = 'w';
                    _Map[21, 3] = 'w';
                    _Map[22, 3] = 'w';
                    _Map[25, 3] = 'w';
                    _Map[26, 3] = 'w';
                    _Map[27, 3] = 'w';
                    _Map[19, 4] = 'w';
                    _Map[27, 4] = 'w';
                    _Map[19, 5] = 'w';
                    _Map[27, 5] = 'w';
                    _Map[23, 7] = 'w';
                    _Map[24, 7] = 'w';
                    _Map[25, 7] = 'w';
                    _Map[20, 8] = 'w';
                    _Map[25, 8] = 'w';
                    _Map[20, 9] = 'w';
                    _Map[25, 9] = 'w';
                    _Map[20, 10] = 'w';
                    _Map[20, 11] = 'w';
                    _Map[23, 12] = 'w';
                    _Map[23, 13] = 'w';
                    _Map[24, 13] = 'w';
                    _Map[25, 13] = 'w';
                    _Map[26, 13] = 'w';
                    _Map[23, 14] = 'w';
                    _Map[24, 14] = 'w';
                    _Map[25, 14] = 'w';
                    _Map[26, 14] = 'w';
                    _Map[23, 15] = 'w';
                    _Map[20, 16] = 'w';
                    _Map[20, 17] = 'w';
                    _Map[20, 18] = 'w';
                    _Map[25, 18] = 'w';
                    _Map[20, 19] = 'w';
                    _Map[25, 19] = 'w';
                    _Map[23, 20] = 'w';
                    _Map[24, 20] = 'w';
                    _Map[25, 20] = 'w';
                    _Map[19, 22] = 'w';
                    _Map[27, 22] = 'w';
                    _Map[19, 23] = 'w';
                    _Map[27, 23] = 'w';
                    _Map[19, 24] = 'w';
                    _Map[20, 24] = 'w';
                    _Map[21, 24] = 'w';
                    _Map[22, 24] = 'w';
                    _Map[25, 24] = 'w';
                    _Map[26, 24] = 'w';
                    _Map[27, 24] = 'w';
                    break;
            }

            for (_EnergizerCount = 0; _EnergizerCount < 4; _EnergizerCount++)
            {
                do
                {
                    _RndRow = _Rnd.Next(0, 31);
                    _RndCol = _Rnd.Next(0, 28);
                }
                while (_Map[_RndRow, _RndCol] != ' ');
                _Map[_RndRow, _RndCol] = 'e';
            }

            for (_Row = 0; _Row < 31; _Row++)
            {
                for (_Col = 0; _Col < 28; _Col++)
                {
                    if (_Map[_Row, _Col] == ' ') { _Map[_Row, _Col] = 'i'; }
                }
            }

            for (_Row = 0; _Row < 31; _Row++)
            {
                for (_Col = 0; _Col < 28; _Col++)
                {
                    _MapText += _Map[_Row, _Col];
                }
                _MapText += "\n";
            }

            File.WriteAllText("NewMap.txt", _MapText);
        }

        private void LoadMap(string in_Path)
        {
            StreamReader _StreamReader = new StreamReader(in_Path);
            string _ReadedLine = "";
            int _PositionX = 0;
            int _PositionY = 0;
            int _SymbolInLineIndex = 0;
            int _CurPickupIndex = 0;
            int _GhostsCreated = 0;

            while (_StreamReader.EndOfStream == false)
            {
                _ReadedLine = _StreamReader.ReadLine();
                for (_SymbolInLineIndex = 0; _SymbolInLineIndex < _ReadedLine.Length; _SymbolInLineIndex++)
                {
                    switch (_ReadedLine[_SymbolInLineIndex])
                    {
                        case 'w':
                            _Walls.Add(new Wall(new Transform(_PositionX * 14, _PositionY * 14, 14, 14)));
                            _NavMap[_PositionY, _PositionX] = 2;
                            break;
                        case 'p':
                            _Player = new Player(new Transform(_PositionX * 14, _PositionY * 14, 10 ,10), this);
                            break;
                        case 'g':
                            _Ghosts[_GhostsCreated] = new Ghost(new Transform(_PositionX * 14, _PositionY * 14, 14, 14), this);
                            if(_GhostsCreated<3) { _GhostsCreated++; }
                            break;
                        case 'e':
                            _CurPickupIndex = 0;
                            do { _CurPickupIndex++; } while (_Pickups[_CurPickupIndex] != null && _CurPickupIndex < _Pickups.Length);
                            _Pickups[_CurPickupIndex] = new Pickup(new Transform(_PositionX * 14, _PositionY * 14, 8, 8), true);
                            break;
                        case 'i':
                            _CurPickupIndex = 0;
                            do { _CurPickupIndex++; } while (_Pickups[_CurPickupIndex] != null && _CurPickupIndex < _Pickups.Length);
                            _Pickups[_CurPickupIndex] = new Pickup(new Transform(_PositionX * 14, _PositionY * 14, 4, 4), false);
                            _MaxScore++;
                            break;
                    }
                    _PositionX++;
                }
                _PositionY++;
                _PositionX = 0;
            }
            _StreamReader.Close();
        }

        private void StartMatch()
        {
            _CurrentPage = ePages.Game;
        }

        private void CloseMatch()
        {
         _CurrentPage = ePages.Menu;
         _Score = 0; 
         _MaxScore = -1;
         _Lives = 3; 
         _MapPath = "";
         _Player = null;
         _Walls = new List<Wall>(); 
         for(int i=0;i<_Ghosts.Length;i++)
         {
                _Ghosts[i] = null;
         }
         _Pickups = new Pickup[868];
        }

        private static bool RayHit(Transform in_Start, Transform in_Destination, List<Wall> IncomeWallList)
        {
            bool _Result = true;
            float _Times = 0.00f;
            var _CurrentRayPos = new Transform(in_Start.X, in_Start.Y, 14, 14);
            do
            {
                _CurrentRayPos = new Transform((int)(in_Start.X + (in_Destination.X - in_Start.X) * _Times), (int)(in_Start.Y + (in_Destination.Y - in_Start.Y) * _Times), 14, 14);
                foreach(Wall _CurrentWall in IncomeWallList)
                {
                    if(isOverlaping(_CurrentRayPos, _CurrentWall.Transform) == true)
                    {
                        _Result = false;
                        //Console.WriteLine(_CurrentRayPos.X + "x" + _CurrentRayPos.Y +" vs "+_CurrentWall.Transform.X+"x"+_CurrentWall.Transform.Y);
                    }
                    
                }
                _Times += 0.01f;
            }
            while (_Times <= 1.0f);
            return _Result;
        }

        private Point GetRandomNonWallPoint()
        {
            
            Point _RandomPatrolPoint = new Point(_Random.Next(1, 28), _Random.Next(1, 31));
            
            do { _RandomPatrolPoint = new Point(_Random.Next(1, 28), _Random.Next(1, 31)); }
            while (this._NavMap[_RandomPatrolPoint.Y, _RandomPatrolPoint.X] == 2);

            Console.WriteLine(_RandomPatrolPoint);
            return _RandomPatrolPoint;
        }

        #endregion

        #region Game Classes

        public class Player
        {
            private Transform _Transform = null;
            private int _PreviousX = -1;
            private int _PreviousY = -1;
            private eDirections _CurrentDirection = eDirections.Idle;
            private eDirections _PlannedDirection = eDirections.Idle;
            private string _Image = "";
            private int _AnimationTick = 0; 
            private int _EnergizedTicks = 0;
            private PacManComponent _Context = null;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }
            public int PreviousX { get => _PreviousX; set { if (_PreviousX != value) { _PreviousX = value; } } }
            public int PreviousY { get => _PreviousY; set { if (_PreviousY != value) { _PreviousY = value; } } }
            public int EnergizedTicks { get => _EnergizedTicks; set { if (_EnergizedTicks != value) { _EnergizedTicks = value; } } }
            public eDirections CurrentDirection { get => _CurrentDirection; set { if (_CurrentDirection != value) { _CurrentDirection = value; } } }
            public eDirections PlannedDirection { get => _PlannedDirection; set { if (_PlannedDirection != value) { _PlannedDirection = value; } } }
            public string Image { get => _Image; set { if (_Image != value) { _Image = value; } } }
            public PacManComponent Context { get; set; }

            public Player()
            {
                _Transform = new Transform(0, 0, 14, 14);
                _CurrentDirection = eDirections.Idle;
                _PlannedDirection = eDirections.Idle;
                _Image = "PacMan_Right_1";
                _AnimationTick = 0;
                _EnergizedTicks = 0;
                _Context = null;
            }

            public Player(Transform in_SpawnPosition)
            {
                _Transform = in_SpawnPosition;
                _CurrentDirection = eDirections.Idle;
                _PlannedDirection = eDirections.Idle;
                _Image = "PacMan_Right_1";
                _AnimationTick = 0;
                _EnergizedTicks = 0;
                _Context = null;
            }

            public Player(Transform in_SpawnPosition, PacManComponent IncomeContext)
            {
                _Transform = in_SpawnPosition;
                _CurrentDirection = eDirections.Idle;
                _PlannedDirection = eDirections.Idle;
                _Image = "PacMan_Right_1";
                _AnimationTick = 0;
                _EnergizedTicks = 0;
                _Context = IncomeContext;
            }

            public void Tick()
            {
                if (_EnergizedTicks > 0) { _EnergizedTicks--; }

                _AnimationTick++;
                if (_AnimationTick > 3)
                {
                    _AnimationTick = 0;
                    switch (_Image)
                    {
                        case "PacMan_Right_1": _Image = "PacMan_Right_2"; break;
                        case "PacMan_Right_2": _Image = "PacMan_Mouthed"; break;
                        case "PacMan_Left_1": _Image = "PacMan_Left_2"; break;
                        case "PacMan_Left_2": _Image = "PacMan_Mouthed"; break;
                        case "PacMan_Up_1": _Image = "PacMan_Up_2"; break;
                        case "PacMan_Up_2": _Image = "PacMan_Mouthed"; break;
                        case "PacMan_Down_1": _Image = "PacMan_Down_2"; break;
                        case "PacMan_Down_2": _Image = "PacMan_Mouthed"; break;
                        case "PacMan_Mouthed":
                            switch (_CurrentDirection)
                            {
                                case eDirections.Up: _Image = "PacMan_Up_1"; break;
                                case eDirections.Left: _Image = "PacMan_Left_1"; break;
                                case eDirections.Down: _Image = "PacMan_Down_1"; break;
                                case eDirections.Right: _Image = "PacMan_Right_1"; break;
                            }
                            break;
                    }
                }

                _PreviousX = _Transform.X;
                _PreviousY = _Transform.Y;

                switch (_CurrentDirection)
                {
                    case eDirections.Up: _Transform.Y -= 2; ; break;
                    case eDirections.Left: _Transform.X -= 2; ; break;
                    case eDirections.Down: _Transform.Y += 2; ; break;
                    case eDirections.Right: _Transform.X += 2; break;
                }

                foreach (Wall _CurrentWall in _Context._Walls)
                {
                    if (isOverlaping(_Context._Player.Transform, _CurrentWall.Transform))
                    {
                        _Context._Player.Transform.X = _Context._Player.PreviousX;
                        _Context._Player.Transform.Y = _Context._Player.PreviousY;
                    }
                }

                bool _WallOnWay = false;
                switch (this.PlannedDirection)
                {
                    case eDirections.Up:
                        ;
                        foreach (Wall _CurrentWall in _Context._Walls)
                        {
                            if (isOverlaping(new Transform(this.Transform.X, this.Transform.Y - 14, this.Transform.Width, this.Transform.Height), _CurrentWall.Transform))
                            { _WallOnWay = true; }
                        }
                        break;
                    case eDirections.Left:
                        foreach (Wall _CurrentWall in _Context._Walls)
                        {
                            if (isOverlaping(new Transform(this.Transform.X - 14, this.Transform.Y, this.Transform.Width, this.Transform.Height), _CurrentWall.Transform))
                            { _WallOnWay = true; }
                        }
                        break;
                    case eDirections.Down:
                        foreach (Wall _CurrentWall in _Context._Walls)
                        {
                            if (isOverlaping(new Transform(this.Transform.X, this.Transform.Y + 14, this.Transform.Width, this.Transform.Height), _CurrentWall.Transform))
                            { _WallOnWay = true; }
                        }
                        break;
                    case eDirections.Right:
                        foreach (Wall _CurrentWall in _Context._Walls)
                        {
                            if (isOverlaping(new Transform(this.Transform.X + 14, this.Transform.Y, this.Transform.Width, this.Transform.Height), _CurrentWall.Transform))
                            { _WallOnWay = true; }
                        }
                        break;
                }
                if (_WallOnWay == false) { this.CurrentDirection = this.PlannedDirection; }
            }
        }

        public class Wall
        {
            private Transform _Transform = null;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }

            public Wall()
            {
                _Transform = new Transform();
            }

            public Wall(Transform in_SpawnPosition)
            {
                _Transform = in_SpawnPosition;
            }
        }

        public class Ghost
        {
            private Transform _Transform = null;
            private int _TicksBeforeNextStep = 0;
            private const int _TicksToStep = 2;
            private string _Image = "GreenGhost_Right_1";
            private string _CurrentColor = "Green";
            private int _AnimationTick = 0;
            private List<Point> _PathToTarget = null;
            private int _PathStepsWalked = 0;
            private int _Alertness = 0;
            private PacManComponent _Context = null;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }
            public int Alertness { get => _Alertness; set { if (_Alertness != value) { _Alertness = value; } } }
            public List<Point> PathToTarget { get => _PathToTarget; set { if (_PathToTarget != value) { _PathToTarget = value; } } }
            public int TicksBeforeNextStep { get => _TicksBeforeNextStep; set { if (_TicksBeforeNextStep != value) { _TicksBeforeNextStep = value; } } }
            public int PathStepsWalked { get => _PathStepsWalked; set { if (_PathStepsWalked != value) { _PathStepsWalked = value; } } }
            public string Image { get => _Image; set { if (_Image != value) { _Image = value; } } }
            public string CurrentColor { get => _CurrentColor; set { if (_CurrentColor != value) { _CurrentColor = value; } } }
            public PacManComponent Context { get; set; }

            public Ghost(Transform in_SpawnPosition)
            {
                _Transform = in_SpawnPosition;
                _TicksBeforeNextStep = 1;
                _Image = "GreenGhost_Right_1";
                _CurrentColor = "Green";
                _AnimationTick = 0;
                _PathToTarget = null;
                _PathStepsWalked = 0;
                _Alertness = 0;
                _Context = null;
            }

            public Ghost(Transform in_SpawnPosition, PacManComponent IncomeContext)
            {
                _Transform = in_SpawnPosition;
                _TicksBeforeNextStep = 1;
                _Image = "GreenGhost_Right_1";
                _CurrentColor = "Green";
                _AnimationTick = 0;
                _PathToTarget = null;
                _PathStepsWalked = 0;
                _Alertness = 0;
                _Context = IncomeContext;
            }

            public void Tick()
            {
                _TicksBeforeNextStep--;
                if (_TicksBeforeNextStep < 0) { _TicksBeforeNextStep = _TicksToStep; }

                if (_Alertness > 0) { _Alertness--; }

                if (_Alertness == 0) { _CurrentColor = "Green"; }
                else if (_Alertness > 0 && _Alertness < 150) { _CurrentColor = "Orange"; }
                else if (_Alertness >= 150) { _CurrentColor = "Red"; }

                _AnimationTick++;
                if (_AnimationTick > 3)
                {
                    _AnimationTick = 0;
                    if (_Image == _CurrentColor + "Ghost_Right_1") { _Image = _CurrentColor + "Ghost_Right_2"; }
                    else if (_Image == _CurrentColor + "Ghost_Right_2") { _Image = _CurrentColor + "Ghost_Right_1"; }
                    else if (_Image == _CurrentColor + "Ghost_Left_1") { _Image = _CurrentColor + "Ghost_Left_2"; }
                    else if (_Image == _CurrentColor + "Ghost_Left_2") { _Image = _CurrentColor + "Ghost_Left_1"; }
                    else if (_Image == _CurrentColor + "Ghost_Up_1") { _Image = _CurrentColor + "Ghost_Up_2"; }
                    else if (_Image == _CurrentColor + "Ghost_Up_2") { _Image = _CurrentColor + "Ghost_Up_1"; }
                    else if (_Image == _CurrentColor + "Ghost_Down_1") { _Image = _CurrentColor + "Ghost_Down_2"; }
                    else if (_Image == _CurrentColor + "Ghost_Down_2") { _Image = _CurrentColor + "Ghost_Down_1"; }
                }

                if (RayHit(this.Transform, _Context._Player.Transform, _Context._Walls) == true)
                {
                    this.Alertness = 300;
                    this.PathToTarget = null;
                }

                if(this._PathToTarget == null)
                {
                    if(this._Alertness==0)
                    {
                        this._PathStepsWalked = 0;
                        this._PathToTarget = PathFind.FindPath(this._Context._NavMap, new Point(this.Transform.Y / 14, this.Transform.X / 14), this._Context.GetRandomNonWallPoint());
                    }
                    else if(this._Alertness>0)
                    {
                        this._PathStepsWalked = 0;
                        this._PathToTarget = PathFind.FindPath(_Context._NavMap, new Point((int)this.Transform.Y / 14, (int)this.Transform.X / 14), new Point((int)_Context._Player.Transform.Y / 14, (int)_Context._Player.Transform.X / 14));
                    }
                }

                if (this._PathToTarget != null)
                {
                    if (this._PathToTarget.Count > 1 && this.TicksBeforeNextStep == 0 && this.PathStepsWalked < this.PathToTarget.Count)
                    {
                        if (this.PathToTarget[this.PathStepsWalked].X * 14 > this.Transform.Y) { this.Transform.Y++; this.Image = this.CurrentColor + "Ghost_Down_1"; }
                        else if (this.PathToTarget[this.PathStepsWalked].X * 14 < this.Transform.Y) { this.Transform.Y--; this.Image = this.CurrentColor + "Ghost_Up_1"; }

                        if (this.PathToTarget[this.PathStepsWalked].Y * 14 > this.Transform.X) { this.Transform.X++; this.Image = this.CurrentColor + "Ghost_Right_1"; }
                        else if (this.PathToTarget[this.PathStepsWalked].Y * 14 < this.Transform.X) { this.Transform.X--; this.Image = this.CurrentColor + "Ghost_Left_1"; }

                        if (this.PathToTarget[this.PathStepsWalked].X * 14 == this.Transform.Y && this.PathToTarget[this.PathStepsWalked].Y * 14 == this.Transform.X) { this.PathStepsWalked++; }

                    }
                    else if (this.PathStepsWalked == this.PathToTarget.Count)
                    {
                        this.PathToTarget = null;
                    }
                }
            }

        }

        public class Pickup
        {
            private Transform _Transform = null;
            private bool _Energized = false;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }
            public bool Energized { get => _Energized; set { if (_Energized != value) { _Energized = value; } } }

            public Pickup()
            {
                _Transform = new Transform(0, 0, 0, 0);
                _Energized = false;
            }

            public Pickup(Transform in_SpawnPoint, bool in_Energized)
            {
                _Transform = in_SpawnPoint;
                _Energized = in_Energized;
            }
        }

        static class PathFind
        {
            public static List<Point> FindPath(int[,] field, Point start, Point goal)
            {
                // Шаг 1. Создается 2 списка вершин — ожидающие рассмотрения и уже рассмотренные. В ожидающие добавляется точка старта, список рассмотренных пока пуст.
                var closedSet = new Collection<PathNode>();
                var openSet = new Collection<PathNode>();
                // Шаг 2. Для каждой точки рассчитывается F = G + H. G — расстояние от старта до точки, H — примерное расстояние от точки до цели. О расчете этой величины я расскажу позднее. Так же каждая точка хранит ссылку на точку, из которой в нее пришли.
                PathNode startNode = new PathNode()
                {
                    Position = start,
                    CameFrom = null,
                    PathLengthFromStart = 0,
                    HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
                };
                openSet.Add(startNode);
                while (openSet.Count > 0)
                {
                    // Шаг 3. Из списка точек на рассмотрение выбирается точка с наименьшим F. Обозначим ее X.
                    var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
                    // Шаг 4. Если X — цель, то мы нашли маршрут.
                    if (currentNode.Position == goal)
                        return GetPathForNode(currentNode);
                    // Шаг 5. Переносим X из списка ожидающих рассмотрения в список уже рассмотренных.
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);
                    // Шаг 6. Для каждой из точек, соседних для X (обозначим эту соседнюю точку Y), делаем следующее:
                    foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
                    {
                        // Шаг 7. Если Y уже находится в рассмотренных — пропускаем ее.
                        if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                            continue;
                        var openNode = openSet.FirstOrDefault(node =>
                          node.Position == neighbourNode.Position);
                        // Шаг 8. Если Y еще нет в списке на ожидание — добавляем ее туда, запомнив ссылку на X и рассчитав Y.G (это X.G + расстояние от X до Y) и Y.H.
                        if (openNode == null)
                            openSet.Add(neighbourNode);
                        else
                          if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                        {
                            // Шаг 9. Если же Y в списке на рассмотрение — проверяем, если X.G + расстояние от X до Y < Y.G, значит мы пришли в точку Y более коротким путем, заменяем Y.G на X.G + расстояние от X до Y, а точку, из которой пришли в Y на X.
                            openNode.CameFrom = currentNode;
                            openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                        }
                    }
                }
                // Шаг 10. Если список точек на рассмотрение пуст, а до цели мы так и не дошли — значит маршрут не существует.
                return null;
            }

            private static int GetDistanceBetweenNeighbours()
            {
                return 1;
            }

            private static int GetHeuristicPathLength(Point from, Point to)
            {
                return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
            }

            private static Collection<PathNode> GetNeighbours(PathNode pathNode, Point goal, int[,] field)
            {
                var result = new Collection<PathNode>();

                // Соседними точками являются соседние по стороне клетки.
                Point[] neighbourPoints = new Point[4];
                neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
                neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
                neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
                neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

                foreach (var point in neighbourPoints)
                {
                    // Проверяем, что не вышли за границы карты.
                    if (point.X < 0 || point.X >= field.GetLength(0))
                        continue;
                    if (point.Y < 0 || point.Y >= field.GetLength(1))
                        continue;
                    // Проверяем, что по клетке можно ходить.
                    if ((field[point.X, point.Y] != 0) && (field[point.X, point.Y] != 1))
                        continue;
                    // Заполняем данные для точки маршрута.
                    var neighbourNode = new PathNode()
                    {
                        Position = point,
                        CameFrom = pathNode,
                        PathLengthFromStart = pathNode.PathLengthFromStart +
                        GetDistanceBetweenNeighbours(),
                        HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                    };
                    result.Add(neighbourNode);
                }
                return result;
            }

            private static List<Point> GetPathForNode(PathNode pathNode)
            {
                var result = new List<Point>();
                var currentNode = pathNode;
                while (currentNode != null)
                {
                    result.Add(currentNode.Position);
                    currentNode = currentNode.CameFrom;
                }
                result.Reverse();
                return result;
            }
        }

        public class PathNode
        {

            public Point Position { get; set; }

            public PathNode CameFrom { get; set; }

            public int PathLengthFromStart { get; set; }

            public int HeuristicEstimatePathLength { get; set; } 

            public int EstimateFullPathLength
            {
                get
                {
                    return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
                }
            }
        }

        public class Transform
        {
            private int _X = -1;
            private int _Y = -1;
            private int _Width = -1;
            private int _Height = -1;

            public int X { get => _X; set { if (_X != value) { _X = value; } } }
            public int Y { get => _Y; set { if (_Y != value) { _Y = value; } } }
            public int Width { get => _Width; set { if (_Width != value) { _Width = value; } } }
            public int Height { get => _Height; set { if (_Height != value) { _Height = value; } } }

            public Transform()
            {
                _X = -1;
                _Y = -1;
                _Width = -1;
                _Height = -1;
            }

            public Transform(int in_X, int in_Y, int in_Width, int in_Height)
            {
                _X = in_X;
                _Y = in_Y;
                _Width = in_Width;
                _Height = in_Height;
            }
        }

        #endregion Game Classes
    }
}
