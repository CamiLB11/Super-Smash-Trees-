using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace P2_SuperSmashTrees
{
    public partial class Form1 : Form
    {
        bool goLeft1, goRight1, grounded1;
        bool goLeft2, goRight2, grounded2;
        int verticalSpeed1 = 0;
        int verticalSpeed2 = 0;
        int playerSpeed = 8;
        int gravity = 2;
        int jumpStrength = 20;
        int gameTime = 90;

        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        List<PictureBox> plataformas = new List<PictureBox>();
        Random rnd = new Random();

        PictureBox jugador1 = new PictureBox();
        PictureBox jugador2 = new PictureBox();
        Label lblTiempo = new Label();

        bool jugador1Vivo = true;
        bool jugador2Vivo = true;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ClientSize = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Image.FromFile("fondo.png");
                this.BackgroundImageLayout = ImageLayout.Stretch;

                SetupGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando imágenes: " + ex.Message);
                Application.Exit();
            }
        }

        private void SetupGame()
        {
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            timer1.Interval = 20;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            gameTimer.Interval = 1000;
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Start();

            // Plataforma inicial
            PictureBox plataformaSuelo = new PictureBox();
            plataformaSuelo.Size = new Size(this.ClientSize.Width / 2, 80);
            plataformaSuelo.Image = Image.FromFile("isla.png");
            plataformaSuelo.SizeMode = PictureBoxSizeMode.StretchImage;
            plataformaSuelo.Location = new Point((this.ClientSize.Width - plataformaSuelo.Width) / 2, this.ClientSize.Height - plataformaSuelo.Height - 20);
            plataformaSuelo.BackColor = Color.Transparent;
            plataformas.Add(plataformaSuelo);
            this.Controls.Add(plataformaSuelo);

            // Jugador 1
            jugador1.Size = new Size(70, 70);
            jugador1.Image = Image.FromFile("p1.png");
            jugador1.SizeMode = PictureBoxSizeMode.StretchImage;
            jugador1.BackColor = Color.Transparent;
            jugador1.Location = new Point(plataformaSuelo.Left + 50, plataformaSuelo.Top - jugador1.Height);
            this.Controls.Add(jugador1);

            // Jugador 2
            jugador2.Size = new Size(70, 70);
            jugador2.Image = Image.FromFile("p2.png");
            jugador2.SizeMode = PictureBoxSizeMode.StretchImage;
            jugador2.BackColor = Color.Transparent;
            jugador2.Location = new Point(plataformaSuelo.Right - 120, plataformaSuelo.Top - jugador2.Height);
            this.Controls.Add(jugador2);

            // Label de tiempo
            lblTiempo.Text = "⏰ Tiempo: 90";
            lblTiempo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTiempo.ForeColor = Color.White;
            lblTiempo.BackColor = Color.FromArgb(160, 0, 0, 0);
            lblTiempo.Location = new Point(20, 20);
            lblTiempo.AutoSize = true;
            lblTiempo.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(lblTiempo);

            GeneratePlatforms();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (jugador1Vivo)
                MoverJugador(jugador1, ref verticalSpeed1, goLeft1, goRight1, ref grounded1, ref jugador1Vivo);

            if (jugador2Vivo)
                MoverJugador(jugador2, ref verticalSpeed2, goLeft2, goRight2, ref grounded2, ref jugador2Vivo);

            DetectarColisionEntreJugadores();
        }

        private void MoverJugador(PictureBox jugador, ref int verticalSpeed, bool goLeft, bool goRight, ref bool grounded, ref bool jugadorVivo)
        {
            jugador.Top += verticalSpeed;

            if (goLeft)
                jugador.Left -= playerSpeed;
            if (goRight)
                jugador.Left += playerSpeed;

            verticalSpeed += gravity;

            grounded = false;
            foreach (PictureBox platform in plataformas)
            {
                Rectangle jugadorHitbox = new Rectangle(jugador.Left, jugador.Top, jugador.Width, jugador.Height);
                Rectangle plataformaHitbox = new Rectangle(platform.Left, platform.Top, platform.Width, platform.Height);

                if (jugadorHitbox.IntersectsWith(plataformaHitbox) && verticalSpeed >= 0)
                {
                    grounded = true;
                    jugador.Top = platform.Top - jugador.Height;
                    verticalSpeed = 0;
                }
            }

            if (jugador.Top > this.ClientSize.Height)
            {
                jugadorVivo = false;
                jugador.Dispose();
                MessageBox.Show("¡Jugador eliminado!");
            }
        }

        private void DetectarColisionEntreJugadores()
        {
            if (jugador1Vivo && jugador2Vivo)
            {
                if (jugador1.Bounds.IntersectsWith(jugador2.Bounds))
                {
                    if (jugador1.Left < jugador2.Left)
                    {
                        jugador1.Left -= 5;
                        jugador2.Left += 5;
                    }
                    else
                    {
                        jugador1.Left += 5;
                        jugador2.Left -= 5;
                    }

                    // Rebote si saltan uno encima del otro
                    if (jugador1.Top + jugador1.Height / 2 < jugador2.Top + jugador2.Height / 2)
                    {
                        verticalSpeed2 = -10;
                        grounded2 = false;
                        jugador2.Left += jugador1.Left < jugador2.Left ? 10 : -10; // Empujón lateral
                    }
                    else
                    {
                        verticalSpeed1 = -10;
                        grounded1 = false;
                        jugador1.Left += jugador2.Left < jugador1.Left ? 10 : -10; // Empujón lateral
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                goLeft1 = true;
            if (e.KeyCode == Keys.Right)
                goRight1 = true;
            if (e.KeyCode == Keys.Up && grounded1)
                verticalSpeed1 = -jumpStrength;

            if (e.KeyCode == Keys.A)
                goLeft2 = true;
            if (e.KeyCode == Keys.D)
                goRight2 = true;
            if (e.KeyCode == Keys.W && grounded2)
                verticalSpeed2 = -jumpStrength;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                goLeft1 = false;
            if (e.KeyCode == Keys.Right)
                goRight1 = false;
            if (e.KeyCode == Keys.A)
                goLeft2 = false;
            if (e.KeyCode == Keys.D)
                goRight2 = false;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            gameTime--;
            lblTiempo.Text = "⏰ Tiempo: " + gameTime;

            if (gameTime <= 0)
            {
                timer1.Stop();
                gameTimer.Stop();
                MessageBox.Show("¡Tiempo terminado!");
            }
        }

        private void GeneratePlatforms()
        {
            int cantidad = 10;
            int espacioVertical = (this.ClientSize.Height - 300) / cantidad;
            int minSeparationX = 180;
            int minSeparationY = 100; 

            for (int i = 0; i < cantidad; i++)
            {
                PictureBox newPlatform = new PictureBox();
                int ancho = rnd.Next(100, 200);
                newPlatform.Size = new Size(ancho, 20);
                newPlatform.Image = Image.FromFile("isla.png");
                newPlatform.SizeMode = PictureBoxSizeMode.StretchImage;
                newPlatform.BackColor = Color.Transparent;

                bool overlap;
                int intentos = 0;
                do
                {
                    overlap = false;
                    int x = rnd.Next(50, this.ClientSize.Width - ancho - 50);
                    int y = 100 + i * espacioVertical + rnd.Next(-30, 30);
                    newPlatform.Location = new Point(x, y);

                    foreach (PictureBox plat in plataformas)
                    {
                        int distanciaX = Math.Abs((newPlatform.Left + newPlatform.Width / 2) - (plat.Left + plat.Width / 2));
                        int distanciaY = Math.Abs((newPlatform.Top + newPlatform.Height / 2) - (plat.Top + plat.Height / 2));

                        if (distanciaX < minSeparationX && distanciaY < minSeparationY)
                        {
                            overlap = true;
                            break;
                        }
                    }

                    intentos++;
                    if (intentos > 100) break; // Evitando bucle infinito

                } while (overlap);

                plataformas.Add(newPlatform);
                this.Controls.Add(newPlatform);
                newPlatform.BringToFront();
            }

            jugador1.BringToFront();
            jugador2.BringToFront();
            lblTiempo.BringToFront();
        }


    }
}
