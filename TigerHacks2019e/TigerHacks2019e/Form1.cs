using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Drawing2D;

namespace TigerHacks2019e
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            gameLoop();
        }
        public void displayBoard(GameState state)
        {
            gameGrid.Controls.Clear();
            gameGrid.ColumnStyles.Clear();
            gameGrid.RowStyles.Clear();
            //gameGrid.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            //gameGrid.Padding = new Padding(0);
            
            //assuming rectangle board, can change later
            gameGrid.RowCount = state.board.Length;
            gameGrid.ColumnCount = state.board[0].Length;
            gameGrid.Height = 50* gameGrid.RowCount;
            gameGrid.Width = 50 * gameGrid.ColumnCount;
            Panel[,] panels = new Panel[gameGrid.RowCount, gameGrid.ColumnCount];
            
            //outer loop to create columns, inner loop to create rows
            for (int i = 0; i < gameGrid.ColumnCount; i++)
            {
                gameGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, gameGrid.Width / gameGrid.ColumnCount));
                for (int j = 0; j < gameGrid.RowCount; j++)
                {
                    if (i ==0)
                    {
                        gameGrid.RowStyles.Add(new RowStyle(SizeType.Absolute,gameGrid.Height/gameGrid.RowCount));
                    }

                    //create panel to fill the rectangle for color
                    Panel p1 = new Panel();
                    //p1.Padding = new Padding(0);
                    //p1.Margin = new Padding (0);
                    p1.BackColor = Color.FromArgb(state.board[i][j].a, state.board[i][j].r, state.board[i][j].g, state.board[i][j].b);
                    p1.BorderStyle = BorderStyle.FixedSingle;
                    gameGrid.Controls.Add(p1, i, j);
                    panels[i, j] = p1;
                }

            }
            foreach(Player p in state.Players)
            {
                foreach( Unit u in p.units)
                {
                    //CustomLabel lbl = new CustomLabel();
                    Label lbl = new Label();
                    lbl.ForeColor = Color.FromArgb(p.color.a, p.color.r, p.color.g, p.color.b);
                    DialogResult dr = MessageBox.Show(p.color.a + " " + p.color.r +" " + p.color.g +" " + p.color.b);
                    lbl.Font = new Font("Arial", 18,FontStyle.Regular);
                    lbl.Text = u.actionType.ToString();
                    lbl.Dock = DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.BackColor = Color.Transparent;
                    panels[u.position.X, u.position.Y].Controls.Add(lbl);
                }
            }

        }
        private TcpClient Connect()
        {
            //TODO: connect to server
            string server = "192.168.255.166";
            Int32 port = 8080;
            TcpClient client = new TcpClient(server, port);
            return client;


        }

        private void gameLoop()
        {
            bool gameActive = true;
            bool myTurn = false;
            TcpClient client = Connect();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.255.166"), 8080);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ip);
            }
            catch (Exception e)
            {

                Console.Write(e.Message);
            }

            while (gameActive)
            {
                //recieve board
                Byte[] data = new Byte[20000];
                int recievedDataLength = server.Receive(data);
                //               
                State state = new State(Encoding.UTF8.GetString(data, 0, recievedDataLength));
                GameState gameState = state.state;
                //display board
                displayBoard(gameState);
                gameActive = false;

                //if players turn
                if (myTurn)
                {
                    //take input

                    //send input
                }

            }

            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }


    }
    public class CustomLabel : Label
    {
        public CustomLabel()
        {
            OutlineForeColor = Color.Black;
            OutlineWidth = 1;
        }
        public Color OutlineForeColor { get; set; }
        public float OutlineWidth { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen outline = new Pen(OutlineForeColor, OutlineWidth)
            { LineJoin = LineJoin.Round })
            using (StringFormat sf = new StringFormat())
            using (Brush foreBrush = new SolidBrush(ForeColor))
            {
                gp.AddString(Text, Font.FontFamily, (int)Font.Style,
                    Font.Size, ClientRectangle, sf);
                e.Graphics.ScaleTransform(1.3f, 1.35f);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawPath(outline, gp);
                e.Graphics.FillPath(foreBrush, gp);
            }
        }
    }
        public class ColorConvert
    {
        public int a { get; set; }
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
    }
    public class GameState
    {
        
        public ColorConvert[][] board { get; set; }
        public List<Player> Players { get; set; }
        public int currentPlayer { get; set; }
    }
    public class State
    {
        public GameState state { get; set; }
        public State()
        {

        }
        public State(string jsonIn)
        {

            
            this.state = new JavaScriptSerializer().Deserialize<GameState>(jsonIn);

           
        }
    }



    public class Player
    {
        public ColorConvert color { get; set; }
        public string name { get; set; }
        public int number { get; set; }
        public int score { get; set; }
        public List<Unit> units { get; set; }
        public Player()
        {

        }

    }
    public class Unit
    {
        public Point position { get; set; }
        public int actionType { get; set; }
        public int range { get; set; }
        public int speed { get; set; }
        public Unit()
        {

        }
    }
    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
        public Position()
        {

        }
    }

    
}
