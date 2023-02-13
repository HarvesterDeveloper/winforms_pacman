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

namespace PacMan
{
    #region Enums
    public enum ePages { Menu, Game }
    public enum eDirections { Up, Right, Down, Left, Idle }
    #endregion Enums

    public class PacManComponent : Control
    {
        #region Variabes and constatns

        private const Double _Tickrate = 16;

        private static System.Timers.Timer _Timer = new System.Timers.Timer(_Tickrate);

        private Random _Random = new Random();

        private static ePages _CurrentPage = ePages.Menu;

        private Int32 _Score = 0;

        private Int32 _MaxScore = -1;

        private Int32 _Lives = 3; 

        private String _MapPath = "";

        private Int32[,] _NavMap = new Int32[31, 28];

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
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle = 0x02000000;
                return createParams;
            }
        }

        public Int32 Score { get => _Score; }

        public Int32 Lives { get => _Lives; }

        public String MapPath
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
                            StartMatch();
                            LoadMap(_MapPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                } 
            } 
        }

        public ePages CurrentPage { get => _CurrentPage; }

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

            // background
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

            if (_CurrentPage == ePages.Menu)
            {
                e.Graphics.DrawImage((Bitmap)Properties.Resources.Menu, new RectangleF(0, 0, 392, 434));
            }
            else if (_CurrentPage == ePages.Game)
            {
                // player
                if (_Player != null)
                {
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(_Player.Image), new RectangleF(_Player.Transform.X, _Player.Transform.Y, _Player.Transform.Width, _Player.Transform.Height));
                }

                // walls
                foreach (Wall _CurrentWall in _Walls)
                {
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.Wall, new RectangleF(_CurrentWall.Transform.X, _CurrentWall.Transform.Y, _CurrentWall.Transform.Width, _CurrentWall.Transform.Height));
                }
                
                // ghosts
                for(Int32 i=0;i<_Ghosts.Length;i++)
                {
                    if (_Ghosts[i]!=null)
                    {
                        e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject(_Ghosts[i].Image), new RectangleF(_Ghosts[i].Transform.X, _Ghosts[i].Transform.Y, _Ghosts[i].Transform.Width, _Ghosts[i].Transform.Height));

                    }
                }
                
                // pickups
                for (Int32 _CurrentPickup=0; _CurrentPickup< _Pickups.Length; _CurrentPickup++)
                {
                    if (_Pickups[_CurrentPickup] != null && _Pickups != null)
                    {
                        e.Graphics.FillEllipse(new SolidBrush(Color.Yellow), new Rectangle(_Pickups[_CurrentPickup].Transform.X, _Pickups[_CurrentPickup].Transform.Y, _Pickups[_CurrentPickup].Transform.Width, _Pickups[_CurrentPickup].Transform.Height));
                    }
                }
                
                // hud
                try
                {
                    e.Graphics.DrawString("Score:" + _Score.ToString() + "/" + _MaxScore.ToString() + " Energized:" + (Int32)(_Player?.EnergizedTicks / _Tickrate) + "s" + " Lives:" + _Lives, this.Font, Brushes.White, 0, 0);
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
                // Это условие срабатывает при клике на кнопку "новой игры"
                if (isOverlaping(new Transform(e.X, e.Y, 0, 0), new Transform(38, 0, 310, 68)))
                {
                    StartMatch();
                    NewMap();
                    LoadMap("NewMap.txt");
                    _Timer.Enabled = true;
                }

                // Это условие срабатывает при клике на кнопку "загрузка"
                if (isOverlaping(new Transform(e.X, e.Y, 0, 0), new Transform(38, 92, 310, 68)))
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = false;
                    openFileDialog.Filter = "txt files (*.txt)|*.txt";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        StartMatch();
                        LoadMap(openFileDialog.FileNames[0]);
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

        // Главный метод, который отвечает за одну временную итерацию игры.
        private void Tick(object source, ElapsedEventArgs e)
        {
            if (_CurrentPage == ePages.Game)
            {
                _Player.Tick();

                // collision player & ghosts
                for(Int32 i = 0; i < _Ghosts.Length;i++)
                {
                    _Ghosts[i].Tick();
                    if (isOverlaping(_Player.Transform, _Ghosts[i].Transform) && _Player.EnergizedTicks == 0)
                    {
                        _Lives--;
                        Point rndRespawnPoint = GetRandomNonWallPoint();
                        _Player.Transform.X = rndRespawnPoint.X * 14;
                        _Player.Transform.Y = rndRespawnPoint.Y * 14;
                        _Ghosts[0].Alertness = 0;
                        _Ghosts[1].Alertness = 0;
                        _Ghosts[2].Alertness = 0;
                        _Ghosts[3].Alertness = 0;
                    }
                }

                // collision player & pickups
                for (Int32 i = 0; i < _Pickups.Length; i++)
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

                // other game managment stuff
                if (_Lives <= 0)
                {
                    _CurrentPage = ePages.Menu;
                    _Timer.Enabled = false;
                    OnMatchLoose();
                    CloseMatch();
                }

                // other game managment stuff
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

        // Функция возвращающая ложь или истину в зависимости от результата столкновения.
        private static Boolean isOverlaping(Transform firstObj, Transform secondObj)
        {
            if (
                (firstObj.X + firstObj.Width >= secondObj.X)
                && (firstObj.X <= secondObj.X + secondObj.Width)
                && (firstObj.Y + firstObj.Height >= secondObj.Y)
                && (firstObj.Y <= secondObj.Y + secondObj.Height)
               )
            return true; else return false;
        }

        // Метод создающий текстовый файл с данными карты в директории исполняемого файла.
        private void NewMap()
        {
            char[,] map =
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
            String mapToText = "";
            Int32 topPreset = _Random.Next(1, 4);
            Int32 sidePreset = _Random.Next(1, 4);
            Int32 bottomPreset = _Random.Next(1, 4);
            Int32 row = 0;
            Int32 col = 0;
            Int32 rndRow = 0;
            Int32 rndCol = 0;
            Int32 energizerCount = 0;

            // map "generating" (wall spawn)
            switch (topPreset)
            {
                case 1:
                    map[3, 3] = 'w';
                    map[11, 3] = 'w';
                    map[3, 4] = 'w';
                    map[11, 4] = 'w';
                    map[3, 5] = 'w';
                    map[4, 5] = 'w';
                    map[5, 5] = 'w';
                    map[6, 5] = 'w';
                    map[7, 5] = 'w';
                    map[11, 5] = 'w';
                    map[11, 6] = 'w';
                    map[3, 7] = 'w';
                    map[4, 7] = 'w';
                    map[5, 7] = 'w';
                    map[6, 7] = 'w';
                    map[7, 7] = 'w';
                    map[10, 7] = 'w';
                    map[11, 7] = 'w';
                    map[3, 10] = 'w';
                    map[10, 10] = 'w';
                    map[3, 11] = 'w';
                    map[10, 11] = 'w';
                    map[3, 12] = 'w';
                    map[10, 12] = 'w';
                    map[3, 13] = 'w';
                    map[6, 13] = 'w';
                    map[7, 13] = 'w';
                    map[8, 13] = 'w';
                    map[9, 13] = 'w';
                    map[10, 13] = 'w';
                    map[3, 14] = 'w';
                    map[6, 14] = 'w';
                    map[7, 14] = 'w';
                    map[8, 14] = 'w';
                    map[9, 14] = 'w';
                    map[10, 14] = 'w';
                    map[3, 15] = 'w';
                    map[10, 15] = 'w';
                    map[3, 16] = 'w';
                    map[10, 16] = 'w';
                    map[3, 17] = 'w';
                    map[10, 17] = 'w';
                    map[3, 20] = 'w';
                    map[4, 20] = 'w';
                    map[5, 20] = 'w';
                    map[6, 20] = 'w';
                    map[7, 20] = 'w';
                    map[10, 20] = 'w';
                    map[11, 20] = 'w';
                    map[11, 21] = 'w';
                    map[3, 22] = 'w';
                    map[4, 22] = 'w';
                    map[5, 22] = 'w';
                    map[6, 22] = 'w';
                    map[7, 22] = 'w';
                    map[11, 22] = 'w';
                    map[3, 23] = 'w';
                    map[11, 23] = 'w';
                    map[3, 24] = 'w';
                    map[11, 24] = 'w';
                    break;
                case 2:
                    map[3, 3] = 'w';
                    map[4, 3] = 'w';
                    map[5, 3] = 'w';
                    map[6, 3] = 'w';
                    map[7, 3] = 'w';
                    map[10, 3] = 'w';
                    map[11, 3] = 'w';
                    map[3, 4] = 'w';
                    map[10, 4] = 'w';
                    map[11, 4] = 'w';
                    map[3, 5] = 'w';
                    map[10, 5] = 'w';
                    map[11, 5] = 'w';
                    map[6, 7] = 'w';
                    map[6, 8] = 'w';
                    map[7, 8] = 'w';
                    map[8, 8] = 'w';
                    map[9, 8] = 'w';
                    map[1, 13] = 'w';
                    map[2, 13] = 'w';
                    map[3, 13] = 'w';
                    map[4, 13] = 'w';
                    map[5, 13] = 'w';
                    map[6, 13] = 'w';
                    map[7, 13] = 'w';
                    map[8, 13] = 'w';
                    map[9, 13] = 'w';
                    map[1, 14] = 'w';
                    map[2, 14] = 'w';
                    map[3, 14] = 'w';
                    map[4, 14] = 'w';
                    map[5, 14] = 'w';
                    map[6, 14] = 'w';
                    map[7, 14] = 'w';
                    map[8, 14] = 'w';
                    map[9, 14] = 'w';
                    map[6, 19] = 'w';
                    map[7, 19] = 'w';
                    map[8, 19] = 'w';
                    map[9, 19] = 'w';
                    map[6, 20] = 'w';
                    map[3, 22] = 'w';
                    map[10, 22] = 'w';
                    map[11, 22] = 'w';
                    map[3, 23] = 'w';
                    map[10, 23] = 'w';
                    map[11, 23] = 'w';
                    map[3, 24] = 'w';
                    map[4, 24] = 'w';
                    map[5, 24] = 'w';
                    map[6, 24] = 'w';
                    map[7, 24] = 'w';
                    map[10, 24] = 'w';
                    map[11, 24] = 'w';
                    break;
                case 3:
                    map[4, 3] = 'w';
                    map[5, 3] = 'w';
                    map[6, 3] = 'w';
                    map[7, 3] = 'w';
                    map[8, 3] = 'w';
                    map[9, 3] = 'w';
                    map[4, 4] = 'w';
                    map[5, 4] = 'w';
                    map[6, 4] = 'w';
                    map[7, 4] = 'w';
                    map[8, 4] = 'w';
                    map[9, 4] = 'w';
                    map[4, 5] = 'w';
                    map[5, 5] = 'w';
                    map[6, 5] = 'w';
                    map[7, 5] = 'w';
                    map[8, 5] = 'w';
                    map[9, 5] = 'w';
                    map[4, 6] = 'w';
                    map[5, 6] = 'w';
                    map[6, 6] = 'w';
                    map[7, 6] = 'w';
                    map[8, 6] = 'w';
                    map[9, 6] = 'w';
                    map[3, 9] = 'w';
                    map[4, 9] = 'w';
                    map[5, 9] = 'w';
                    map[6, 9] = 'w';
                    map[7, 9] = 'w';
                    map[8, 9] = 'w';
                    map[9, 9] = 'w';
                    map[10, 9] = 'w';
                    map[3, 12] = 'w';
                    map[10, 12] = 'w';
                    map[3, 13] = 'w';
                    map[10, 13] = 'w';
                    map[3, 14] = 'w';
                    map[10, 14] = 'w';
                    map[3, 15] = 'w';
                    map[10, 15] = 'w';
                    map[3, 18] = 'w';
                    map[4, 18] = 'w';
                    map[5, 18] = 'w';
                    map[6, 18] = 'w';
                    map[7, 18] = 'w';
                    map[8, 18] = 'w';
                    map[9, 18] = 'w';
                    map[10, 18] = 'w';
                    map[4, 21] = 'w';
                    map[5, 21] = 'w';
                    map[6, 21] = 'w';
                    map[7, 21] = 'w';
                    map[8, 21] = 'w';
                    map[9, 21] = 'w';
                    map[4, 22] = 'w';
                    map[5, 22] = 'w';
                    map[6, 22] = 'w';
                    map[7, 22] = 'w';
                    map[8, 22] = 'w';
                    map[9, 22] = 'w';
                    map[4, 23] = 'w';
                    map[5, 23] = 'w';
                    map[6, 23] = 'w';
                    map[7, 23] = 'w';
                    map[8, 23] = 'w';
                    map[9, 23] = 'w';
                    map[4, 24] = 'w';
                    map[5, 24] = 'w';
                    map[6, 24] = 'w';
                    map[7, 24] = 'w';
                    map[8, 24] = 'w';
                    map[9, 24] = 'w';
                    break;
            }
            switch (sidePreset)
            {
                case 1:
                    map[16, 3] = 'w';
                    map[16, 4] = 'w';
                    map[14, 5] = 'w';
                    map[15, 5] = 'w';
                    map[16, 5] = 'w';
                    map[16, 6] = 'w';
                    map[16, 7] = 'w';
                    map[16, 20] = 'w';
                    map[16, 21] = 'w';
                    map[14, 22] = 'w';
                    map[15, 22] = 'w';
                    map[16, 22] = 'w';
                    map[16, 23] = 'w';
                    map[16, 24] = 'w';
                    break;
                case 2:
                    map[13, 3] = 'w';
                    map[14, 3] = 'w';
                    map[14, 4] = 'w';
                    map[15, 4] = 'w';
                    map[15, 6] = 'w';
                    map[16, 6] = 'w';
                    map[16, 7] = 'w';
                    map[17, 7] = 'w';
                    map[16, 20] = 'w';
                    map[17, 20] = 'w';
                    map[15, 21] = 'w';
                    map[16, 21] = 'w';
                    map[14, 23] = 'w';
                    map[15, 23] = 'w';
                    map[13, 24] = 'w';
                    map[14, 24] = 'w';
                    break;
                case 3:
                    map[13, 2] = 'w';
                    map[13, 3] = 'w';
                    map[13, 4] = 'w';
                    map[13, 5] = 'w';
                    map[17, 5] = 'w';
                    map[17, 6] = 'w';
                    map[17, 7] = 'w';
                    map[17, 8] = 'w';
                    map[17, 19] = 'w';
                    map[17, 20] = 'w';
                    map[17, 21] = 'w';
                    map[13, 22] = 'w';
                    map[17, 22] = 'w';
                    map[13, 23] = 'w';
                    map[13, 24] = 'w';
                    map[13, 25] = 'w';
                    break;
            }
            switch (bottomPreset)
            {
                case 1:
                    map[19, 2] = 'w';
                    map[20, 2] = 'w';
                    map[23, 2] = 'w';
                    map[20, 3] = 'w';
                    map[23, 3] = 'w';
                    map[26, 3] = 'w';
                    map[23, 4] = 'w';
                    map[26, 4] = 'w';
                    map[23, 5] = 'w';
                    map[26, 5] = 'w';
                    map[23, 6] = 'w';
                    map[21, 7] = 'w';
                    map[22, 7] = 'w';
                    map[23, 7] = 'w';
                    map[23, 8] = 'w';
                    map[27, 8] = 'w';
                    map[28, 8] = 'w';
                    map[29, 8] = 'w';
                    map[23, 9] = 'w';
                    map[25, 12] = 'w';
                    map[21, 13] = 'w';
                    map[22, 13] = 'w';
                    map[23, 13] = 'w';
                    map[24, 13] = 'w';
                    map[25, 13] = 'w';
                    map[21, 14] = 'w';
                    map[22, 14] = 'w';
                    map[23, 14] = 'w';
                    map[24, 14] = 'w';
                    map[25, 14] = 'w';
                    map[25, 15] = 'w';
                    map[23, 18] = 'w';
                    map[23, 19] = 'w';
                    map[27, 19] = 'w';
                    map[28, 19] = 'w';
                    map[29, 19] = 'w';
                    map[21, 20] = 'w';
                    map[22, 20] = 'w';
                    map[23, 20] = 'w';
                    map[23, 21] = 'w';
                    map[23, 22] = 'w';
                    map[26, 22] = 'w';
                    map[23, 23] = 'w';
                    map[26, 23] = 'w';
                    map[20, 24] = 'w';
                    map[23, 24] = 'w';
                    map[26, 24] = 'w';
                    map[19, 25] = 'w';
                    map[20, 25] = 'w';
                    map[23, 25] = 'w';
                    break;
                case 2:
                    map[24, 3] = 'w';
                    map[25, 3] = 'w';
                    map[26, 3] = 'w';
                    map[27, 3] = 'w';
                    map[27, 4] = 'w';
                    map[27, 5] = 'w';
                    map[23, 6] = 'w';
                    map[27, 6] = 'w';
                    map[23, 7] = 'w';
                    map[23, 8] = 'w';
                    map[23, 9] = 'w';
                    map[20, 10] = 'w';
                    map[23, 10] = 'w';
                    map[24, 10] = 'w';
                    map[25, 10] = 'w';
                    map[26, 10] = 'w';
                    map[27, 10] = 'w';
                    map[28, 10] = 'w';
                    map[29, 10] = 'w';
                    map[20, 11] = 'w';
                    map[20, 12] = 'w';
                    map[20, 13] = 'w';
                    map[27, 13] = 'w';
                    map[28, 13] = 'w';
                    map[29, 13] = 'w';
                    map[20, 14] = 'w';
                    map[27, 14] = 'w';
                    map[28, 14] = 'w';
                    map[29, 14] = 'w';
                    map[20, 15] = 'w';
                    map[20, 16] = 'w';
                    map[20, 17] = 'w';
                    map[23, 17] = 'w';
                    map[24, 17] = 'w';
                    map[25, 17] = 'w';
                    map[26, 17] = 'w';
                    map[27, 17] = 'w';
                    map[28, 17] = 'w';
                    map[29, 17] = 'w';
                    map[23, 18] = 'w';
                    map[23, 19] = 'w';
                    map[23, 20] = 'w';
                    map[23, 21] = 'w';
                    map[27, 21] = 'w';
                    map[27, 22] = 'w';
                    map[27, 23] = 'w';
                    map[24, 24] = 'w';
                    map[25, 24] = 'w';
                    map[26, 24] = 'w';
                    map[27, 24] = 'w';
                    break;
                case 3:
                    map[19, 3] = 'w';
                    map[20, 3] = 'w';
                    map[21, 3] = 'w';
                    map[22, 3] = 'w';
                    map[25, 3] = 'w';
                    map[26, 3] = 'w';
                    map[27, 3] = 'w';
                    map[19, 4] = 'w';
                    map[27, 4] = 'w';
                    map[19, 5] = 'w';
                    map[27, 5] = 'w';
                    map[23, 7] = 'w';
                    map[24, 7] = 'w';
                    map[25, 7] = 'w';
                    map[20, 8] = 'w';
                    map[25, 8] = 'w';
                    map[20, 9] = 'w';
                    map[25, 9] = 'w';
                    map[20, 10] = 'w';
                    map[20, 11] = 'w';
                    map[23, 12] = 'w';
                    map[23, 13] = 'w';
                    map[24, 13] = 'w';
                    map[25, 13] = 'w';
                    map[26, 13] = 'w';
                    map[23, 14] = 'w';
                    map[24, 14] = 'w';
                    map[25, 14] = 'w';
                    map[26, 14] = 'w';
                    map[23, 15] = 'w';
                    map[20, 16] = 'w';
                    map[20, 17] = 'w';
                    map[20, 18] = 'w';
                    map[25, 18] = 'w';
                    map[20, 19] = 'w';
                    map[25, 19] = 'w';
                    map[23, 20] = 'w';
                    map[24, 20] = 'w';
                    map[25, 20] = 'w';
                    map[19, 22] = 'w';
                    map[27, 22] = 'w';
                    map[19, 23] = 'w';
                    map[27, 23] = 'w';
                    map[19, 24] = 'w';
                    map[20, 24] = 'w';
                    map[21, 24] = 'w';
                    map[22, 24] = 'w';
                    map[25, 24] = 'w';
                    map[26, 24] = 'w';
                    map[27, 24] = 'w';
                    break;
            }

            // energizer spawn
            for (energizerCount = 0; energizerCount < 4; energizerCount++)
            {
                while(map[rndRow, rndCol] != ' ')
                {
                    rndRow = _Random.Next(0, 31);
                    rndCol = _Random.Next(0, 28);
                }

                map[rndRow, rndCol] = 'e';
            }

            // pickup spawn
            for (row = 0; row < 31; row++)
            {
                for (col = 0; col < 28; col++)
                {
                    if (map[row, col] == ' ') { map[row, col] = 'i'; }
                }
            }

            // output entire map to text file
            for (row = 0; row < 31; row++)
            {
                for (col = 0; col < 28; col++)
                {
                    mapToText += map[row, col];
                }
                mapToText += "\n";
            }

            File.WriteAllText("NewMap.txt", mapToText);
        }

        // Метод спавнящий объекты из текстового файла по указанному пути.
        private void LoadMap(String path)
        {
            StreamReader streamreader = new StreamReader(path);
            String readedLine = "";
            Int32 posX = 0;
            Int32 posY = 0;
            Int32 i = 0;
            Int32 curPickupIndex = 0;
            Int32 ghostsCreated = 0;

            while (streamreader.EndOfStream == false)
            {
                readedLine = streamreader.ReadLine();

                for (i = 0; i < readedLine.Length; i++)
                {
                    switch (readedLine[i])
                    {
                        case 'w':
                            _Walls.Add(new Wall(new Transform(posX * 14, posY * 14, 14, 14)));
                            _NavMap[posY, posX] = 2;
                            break;
                        case 'p':
                            _Player = new Player(new Transform(posX * 14, posY * 14, 10 ,10), this);
                            break;
                        case 'g':
                            _Ghosts[ghostsCreated] = new Ghost(new Transform(posX * 14, posY * 14, 14, 14), this);
                            if(ghostsCreated < 3) { ghostsCreated++; }
                            break;
                        case 'e':
                            curPickupIndex = 0;
                            while (_Pickups[curPickupIndex] != null && curPickupIndex < _Pickups.Length) { curPickupIndex++; }
                            _Pickups[curPickupIndex] = new Pickup(new Transform(posX * 14, posY * 14, 8, 8), true);
                            break;
                        case 'i':
                            curPickupIndex = 0;
                            while (_Pickups[curPickupIndex] != null && curPickupIndex < _Pickups.Length) { curPickupIndex++; }
                            _Pickups[curPickupIndex] = new Pickup(new Transform(posX * 14+4, posY * 14+4, 4, 4), false);
                            _MaxScore++;
                            break;
                    }

                    posX++;
                }

                posY++;
                posX = 0;
            }
            streamreader.Close();
        }

        // Сменяет сцену на матч.
        private void StartMatch()
        {
            _CurrentPage = ePages.Game;
        }

        // Сменяет сцену на меню и обнуляет матчевые переменные в начальные значения.
        private void CloseMatch()
        {
            _CurrentPage = ePages.Menu;
            _Score = 0;
            _MaxScore = -1;
            _Lives = 3;
            _MapPath = "";
            _Player = null;
            _Walls = new List<Wall>();
            for (Int32 i = 0; i < _Ghosts.Length; i++)
            {
                _Ghosts[i] = null;
            }
            _Pickups = new Pickup[868];
        }

        // Функция, возвращающая значение в зависимости от пересечения луча со стеной. Если луч не встретился с стеной, то возвращается true, но при пресечении стены вернёт false.
        private Boolean RayHit(Int32 startX, Int32 startY, Int32 destX, Int32 destY, Int32 rayThickness)
        {
            Boolean res = true;
            Single times = 0.00f;
            Transform curRayPos = new Transform(startX, startY, rayThickness, rayThickness);

            while(times <= 1.0f)
            {
                curRayPos = new Transform((Int32)(startX + (destX - startX) * times), (Int32)(startY + (destY - startY) * times), rayThickness, rayThickness);
                foreach (Wall _CurrentWall in _Walls)
                {
                    if (isOverlaping(curRayPos, _CurrentWall.Transform) == true)
                    {
                        res = false;
                    }
                }
                times += 0.01f;
            }

            return res;
        }

        // Функция для получения случайной координаты свободного блока (не случайный пиксель, а именно позиция блока карты.)
        private Point GetRandomNonWallPoint()
        {
            
            Point rndPoint = new Point(_Random.Next(1, 28), _Random.Next(1, 31));
            
            while(this._NavMap[rndPoint.Y, rndPoint.X] == 2)
            {
                rndPoint = new Point(_Random.Next(1, 28), _Random.Next(1, 31));
            }

            return rndPoint;
        }

        #endregion

        #region Game Classes

        private class Player
        {
            private Transform _Transform = null;
            private Int32 _PreviousX = -1;
            private Int32 _PreviousY = -1;
            private eDirections _CurrentDirection = eDirections.Idle;
            private eDirections _PlannedDirection = eDirections.Idle;
            private String _Image = "";
            private Int32 _AnimationTick = 0; 
            private Int32 _EnergizedTicks = 0;
            private PacManComponent _Context = null;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }
            public Int32 PreviousX { get => _PreviousX; set { if (_PreviousX != value) { _PreviousX = value; } } }
            public Int32 PreviousY { get => _PreviousY; set { if (_PreviousY != value) { _PreviousY = value; } } }
            public Int32 EnergizedTicks { get => _EnergizedTicks; set { if (_EnergizedTicks != value) { _EnergizedTicks = value; } } }
            public eDirections CurrentDirection { get => _CurrentDirection; set { if (_CurrentDirection != value) { _CurrentDirection = value; } } }
            public eDirections PlannedDirection { get => _PlannedDirection; set { if (_PlannedDirection != value) { _PlannedDirection = value; } } }
            public String Image { get => _Image; set { if (_Image != value) { _Image = value; } } }
            public PacManComponent Context { get; set; }

            public Player()
            {
                _Transform = new Transform(0, 0, 12, 12);
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

                Boolean _WallOnWay = false;
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

        private class Wall
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

        private class Ghost
        {
            private Transform _Transform = null;
            private Int32 _TicksBeforeNextStep = 0;
            private const Int32 _TicksToStep = 2;
            private String _Image = "GreenGhost_Right_1";
            private String _CurrentColor = "Green";
            private Int32 _AnimationTick = 0;
            private List<Point> _PathToTarget = null;
            private Int32 _PathStepsWalked = 0;
            private Int32 _Alertness = 0;
            private PacManComponent _Context = null;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }
            public Int32 Alertness { get => _Alertness; set { if (_Alertness != value) { _Alertness = value; } } }
            public List<Point> PathToTarget { get => _PathToTarget; set { if (_PathToTarget != value) { _PathToTarget = value; } } }
            public Int32 TicksBeforeNextStep { get => _TicksBeforeNextStep; set { if (_TicksBeforeNextStep != value) { _TicksBeforeNextStep = value; } } }
            public Int32 PathStepsWalked { get => _PathStepsWalked; set { if (_PathStepsWalked != value) { _PathStepsWalked = value; } } }
            public String Image { get => _Image; set { if (_Image != value) { _Image = value; } } }
            public String CurrentColor { get => _CurrentColor; set { if (_CurrentColor != value) { _CurrentColor = value; } } }
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

                if (_Context.RayHit(_Transform.X+7, _Transform.Y+7, _Context._Player.Transform.X+7, _Context._Player.Transform.Y+7, 2) == true)
                {
                    _Alertness = 600;
                    _PathToTarget = null;
                }

                if(_PathToTarget == null)
                {
                    if(_Alertness==0)
                    {
                        _PathStepsWalked = 1;
                        _PathToTarget = PathFind.FindPath(_Context._NavMap, new Point(this.Transform.Y / 14, this.Transform.X / 14), this._Context.GetRandomNonWallPoint());
                    }
                    else if(_Alertness>0)
                    {
                        _PathStepsWalked = 1;
                        _PathToTarget = PathFind.FindPath(_Context._NavMap, new Point((Int32)this.Transform.Y / 14, (Int32)this.Transform.X / 14), new Point((Int32)_Context._Player.Transform.Y / 14, (Int32)_Context._Player.Transform.X / 14));
                    }
                }

                if (_PathToTarget != null)
                {
                    if (_PathToTarget.Count > 0 && _TicksBeforeNextStep == 0 && _PathStepsWalked < _PathToTarget.Count)
                    {
                        if (_PathToTarget[_PathStepsWalked].X * 14 > _Transform.Y) { _Transform.Y++; _Image = _CurrentColor + "Ghost_Down_1"; }
                        else if (_PathToTarget[_PathStepsWalked].X * 14 < _Transform.Y) { _Transform.Y--; _Image = _CurrentColor + "Ghost_Up_1"; }

                        if (_PathToTarget[_PathStepsWalked].Y * 14 > _Transform.X) { _Transform.X++; _Image = _CurrentColor + "Ghost_Right_1"; }
                        else if (_PathToTarget[_PathStepsWalked].Y * 14 < _Transform.X) { _Transform.X--; _Image = _CurrentColor + "Ghost_Left_1"; }

                        if (_PathToTarget[_PathStepsWalked].X * 14 == _Transform.Y && _PathToTarget[_PathStepsWalked].Y * 14 == _Transform.X) { this.PathStepsWalked++; }

                    }
                    else if (_PathStepsWalked == _PathToTarget.Count)
                    {
                        this.PathToTarget = null;
                    }
                }
            }

        }

        private class Pickup
        {
            private Transform _Transform = null;
            private Boolean _Energized = false;

            public Transform Transform { get => _Transform; set { if (_Transform != value) { _Transform = value; } } }
            public Boolean Energized { get => _Energized; set { if (_Energized != value) { _Energized = value; } } }

            public Pickup()
            {
                _Transform = new Transform(0, 0, 0, 0);
                _Energized = false;
            }

            public Pickup(Transform in_SpawnPoint, Boolean in_Energized)
            {
                _Transform = in_SpawnPoint;
                _Energized = in_Energized;
            }
        }

        private static class PathFind
        {
            public static List<Point> FindPath(Int32[,] field, Point start, Point goal)
            {
                var closedSet = new Collection<PathNode>();
                var openSet = new Collection<PathNode>();
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
                    var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
                    if (currentNode.Position == goal)
                        return GetPathForNode(currentNode);
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);
                    foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
                    {
                        if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                            continue;
                        var openNode = openSet.FirstOrDefault(node =>
                          node.Position == neighbourNode.Position);
                        if (openNode == null)
                            openSet.Add(neighbourNode);
                        else
                          if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                        {
                            openNode.CameFrom = currentNode;
                            openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                        }
                    }
                }
                return null;
            }

            private static Int32 GetDistanceBetweenNeighbours()
            {
                return 1;
            }

            private static Int32 GetHeuristicPathLength(Point from, Point to)
            {
                return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
            }

            private static Collection<PathNode> GetNeighbours(PathNode pathNode, Point goal, Int32[,] field)
            {
                var result = new Collection<PathNode>();

                Point[] neighbourPoints = new Point[4];
                neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
                neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
                neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
                neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

                foreach (var point in neighbourPoints)
                {
                    if (point.X < 0 || point.X >= field.GetLength(0))
                        continue;
                    if (point.Y < 0 || point.Y >= field.GetLength(1))
                        continue;
                    if ((field[point.X, point.Y] != 0) && (field[point.X, point.Y] != 1))
                        continue;
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

        private class PathNode
        {

            public Point Position { get; set; }

            public PathNode CameFrom { get; set; }

            public Int32 PathLengthFromStart { get; set; }

            public Int32 HeuristicEstimatePathLength { get; set; } 

            public Int32 EstimateFullPathLength
            {
                get
                {
                    return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
                }
            }
        }

        private class Transform
        {
            private Int32 _X = -1;
            private Int32 _Y = -1;
            private Int32 _Width = -1;
            private Int32 _Height = -1;

            public Int32 X { get => _X; set { if (_X != value) { _X = value; } } }
            public Int32 Y { get => _Y; set { if (_Y != value) { _Y = value; } } }
            public Int32 Width { get => _Width; set { if (_Width != value) { _Width = value; } } }
            public Int32 Height { get => _Height; set { if (_Height != value) { _Height = value; } } }

            public Transform()
            {
                _X = -1;
                _Y = -1;
                _Width = -1;
                _Height = -1;
            }

            public Transform(Int32 in_X, Int32 in_Y, Int32 in_Width, Int32 in_Height)
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