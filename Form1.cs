using System.Diagnostics;
using System.Security.Cryptography;
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
        int score1 = 0;
        int score2 = 0;
        int tokenValue = 0;
        int reto = 0;

        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer spawnTimer = new System.Windows.Forms.Timer();
        List<PictureBox> plataformas = new List<PictureBox>();
        Random rnd = new Random();

        ITree player1Tree;
        ITree player2Tree;

        PictureBox jugador1 = new PictureBox();
        PictureBox jugador2 = new PictureBox();

        Label lblTiempo = new Label();
        Label scoreJ1 = new Label();
        Label scoreJ2 = new Label();
        Label lblreto = new Label();

        Panel panelArbolesJ1;
        Panel panelArbolesJ2;

        bool jugador1Vivo = true;
        bool jugador2Vivo = true;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ClientSize = new Size(1300, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            spawnTimer = new System.Windows.Forms.Timer();
            spawnTimer.Interval = 5000;
            spawnTimer.Tick += (s, e) =>
            {
                SpawnToken();
            };
            spawnTimer.Start();


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
            plataformaSuelo.Size = new Size(this.ClientSize.Width / 2, 50);
            plataformaSuelo.Image = Image.FromFile("isla.png");
            plataformaSuelo.SizeMode = PictureBoxSizeMode.StretchImage;
            plataformaSuelo.Location = new Point((this.ClientSize.Width - plataformaSuelo.Width - 350) / 2, this.ClientSize.Height - plataformaSuelo.Height - 20);
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

            // Score Jugador 1
            scoreJ1.Text = "0";
            scoreJ1.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            scoreJ1.ForeColor = Color.White;
            scoreJ1.BackColor = Color.Transparent;
            scoreJ1.Location = new Point(200, 40);
            scoreJ1.AutoSize = true;
            scoreJ1.BorderStyle = BorderStyle.None;
            this.Controls.Add(scoreJ1);

            // Jugador 2
            jugador2.Size = new Size(70, 70);
            jugador2.Image = Image.FromFile("p2.png");
            jugador2.SizeMode = PictureBoxSizeMode.StretchImage;
            jugador2.BackColor = Color.Transparent;
            jugador2.Location = new Point(plataformaSuelo.Right - 120, plataformaSuelo.Top - jugador2.Height);
            this.Controls.Add(jugador2);

            // Score Jugador 2
            scoreJ2.Text = "0";
            scoreJ2.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            scoreJ2.ForeColor = Color.White;
            scoreJ2.BackColor = Color.Transparent;
            scoreJ2.Location = new Point(740, 40);
            scoreJ2.AutoSize = true;
            scoreJ2.BorderStyle = BorderStyle.None;
            this.Controls.Add(scoreJ2);

            // Label de tiempo
            lblTiempo.Text = "⏰: 90";
            lblTiempo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTiempo.ForeColor = Color.White;
            lblTiempo.BackColor = Color.Transparent;
            lblTiempo.Location = new Point(440, 20);
            lblTiempo.AutoSize = true;
            lblTiempo.BorderStyle = BorderStyle.None;
            this.Controls.Add(lblTiempo);

            // Label de reto
            lblreto.Text = "";
            lblreto.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblreto.ForeColor = Color.White;
            lblreto.BackColor = Color.Transparent;
            lblreto.Location = new Point(340, 70);
            lblreto.AutoSize = true;
            lblreto.BorderStyle = BorderStyle.None;
            this.Controls.Add(lblreto);

            // Panel para árboles Jugador 1
            panelArbolesJ1 = new Panel();
            panelArbolesJ1.Size = new Size(270, 170);
            panelArbolesJ1.Location = new Point(this.ClientSize.Width - 305, 125);
            panelArbolesJ1.BackColor = Color.Gray;
            panelArbolesJ1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelArbolesJ1.Paint += new PaintEventHandler(panelArboles_PaintJ1);
            this.Controls.Add(panelArbolesJ1);

            // Panel para árboles Jugador 2
            panelArbolesJ2 = new Panel();
            panelArbolesJ2.Size = new Size(270, 170);
            panelArbolesJ2.Location = new Point(this.ClientSize.Width - 305, 360);
            panelArbolesJ2.BackColor = Color.Gray;
            panelArbolesJ2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelArbolesJ2.Paint += new PaintEventHandler(panelArboles_PaintJ2);
            this.Controls.Add(panelArbolesJ2);

            GeneratePlatforms();
            elegirReto();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (jugador1Vivo)
                MoverJugador(jugador1, ref verticalSpeed1, goLeft1, goRight1, ref grounded1, ref jugador1Vivo);

            if (jugador2Vivo)
                MoverJugador(jugador2, ref verticalSpeed2, goLeft2, goRight2, ref grounded2, ref jugador2Vivo);

            switch (reto)
            {
                case 1:
                    reto1();
                    break;
                case 2:
                    reto2();
                    break;
                    //case 3:
                    //    reto3();
                    //    break;
                    //case 4:
                    //    reto4();
                    //    break;
                    //case 5:
                    //    reto5();
                    //    break;
            }

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
                        jugador1.Left -= 15;
                        jugador2.Left += 15;
                    }
                    else
                    {
                        jugador1.Left += 15;
                        jugador2.Left -= 15;
                    }

                    // Rebote si saltan uno encima del otro
                    if (jugador1.Bottom <= jugador2.Top + 5 && verticalSpeed1 > 0)
                    {
                        verticalSpeed1 = -10;
                        grounded1 = false;
                        jugador2.Left += jugador1.Left < jugador2.Left ? 10 : -10;
                    }
                    else if (jugador2.Bottom <= jugador1.Top + 5 && verticalSpeed2 > 0)
                    {
                        verticalSpeed2 = -10;
                        grounded2 = false;
                        jugador1.Left += jugador2.Left < jugador1.Left ? 10 : -10;
                    }
                }
            }
        }

        private void GolpeJ1()
        {
            if (jugador1Vivo && jugador2Vivo)
            {
                if (jugador1.Left >= jugador2.Left - 100 && jugador1.Left <= jugador2.Left + 100 && jugador1.Bottom == jugador2.Bottom)
                {
                    if (jugador1.Left < jugador2.Left)
                    {
                        jugador2.Left += 50;
                    }
                    else
                    {
                        jugador2.Left -= 50;
                    }
                }
            }
        }

        private void GolpeJ2()
        {
            if (jugador1Vivo && jugador2Vivo)
            {
                if (jugador2.Left >= jugador1.Left - 100 && jugador2.Left <= jugador1.Left + 100 && jugador1.Bottom == jugador2.Bottom)
                {
                    if (jugador2.Left < jugador1.Left)
                    {
                        jugador1.Left += 50;
                    }
                    else
                    {
                        jugador1.Left -= 50;
                    }
                }
            }
        }

        private void sumarScoreJ1()
        {
            score1 += 10;
            scoreJ1.Text = score1.ToString();
        }

        private void sumarScoreJ2()
        {
            score2 += 10;
            scoreJ2.Text = score2.ToString();
        }

        private void elegirReto()
        {
            reto = rnd.Next(1, 5);
            Debug.WriteLine(reto);

            switch (reto)
            {
                case 1:
                    player1Tree = new AVLTree();
                    player2Tree = new AVLTree();
                    lblreto.Text = "Crea un AVL con 5 tokens";
                    break;

                case 2:
                    player1Tree = new BSTree();
                    player2Tree = new BSTree();
                    lblreto.Text = "Haz un BST con raíz y dos hijos";
                    break;

                case 3:
                    lblreto.Text = "";
                    player1Tree = new BSTree();
                    player2Tree = new BSTree();
                    break;

                case 4:
                    lblreto.Text = "";
                    player1Tree = new BSTree();
                    player2Tree = new BSTree();
                    break;

                case 5:
                    lblreto.Text = "";
                    player1Tree = new BSTree();
                    player2Tree = new BSTree();
                    break;
            }
        }


        private void SpawnToken()
        {
            int thisTokenValue = rnd.Next(1, 99);
            PictureBox token = new PictureBox();
            token.Size = new Size(60, 60);
            token.Image = Image.FromFile("token.png");
            token.SizeMode = PictureBoxSizeMode.StretchImage;
            token.BackColor = Color.Transparent;
            token.Location = new Point(rnd.Next(100, 800), 100);
            this.Controls.Add(token);

            Label tokenValueLabel = new Label();
            tokenValueLabel.Text = thisTokenValue.ToString();
            tokenValueLabel.ForeColor = Color.White;
            tokenValueLabel.BackColor = Color.FromArgb(248, 172, 54);
            tokenValueLabel.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            tokenValueLabel.Location = new Point(token.Left + 15, token.Top + 13);
            tokenValueLabel.AutoSize = true;
            this.Controls.Add(tokenValueLabel);
            tokenValueLabel.BringToFront();

            System.Windows.Forms.Timer tokenTimer = new System.Windows.Forms.Timer();
            tokenTimer.Interval = 20;
            tokenTimer.Tick += (s, e) =>
            {
                token.Top += 5;
                tokenValueLabel.Top += 5;

                if (token.Bounds.IntersectsWith(jugador1.Bounds) && jugador1Vivo)
                {
                    player1Tree.insert(thisTokenValue);
                    Debug.WriteLine(player1Tree.PrintInOrder());

                    token.Dispose();
                    tokenValueLabel.Dispose();
                    tokenTimer.Stop();
                    panelArbolesJ1.Invalidate();
                }
                else if (token.Bounds.IntersectsWith(jugador2.Bounds) && jugador2Vivo)
                {
                    player2Tree.insert(thisTokenValue);
                    Debug.WriteLine(player2Tree.PrintInOrder());

                    token.Dispose();
                    tokenValueLabel.Dispose();
                    tokenTimer.Stop();
                    panelArbolesJ2.Invalidate();
                }
                else if (token.Top > this.ClientSize.Height)
                {
                    token.Dispose();
                    tokenValueLabel.Dispose();
                    tokenTimer.Stop();
                }

                foreach (PictureBox platform in plataformas)
                {
                    Rectangle tokenHitbox = new Rectangle(token.Left, token.Top, token.Width, token.Height);
                    Rectangle plataformaHitbox = new Rectangle(platform.Left, platform.Top, platform.Width, platform.Height);

                    if (tokenHitbox.IntersectsWith(plataformaHitbox))
                    {
                        token.Top = platform.Top - token.Height;
                        tokenValueLabel.Top = token.Top + 13;
                        break;
                    }
                }
            };

            tokenTimer.Start();
        }

        // Verificar que los retos se cumplan
        private void reto1()
        {
            if (player1Tree.GetSize() == 5)
                retoTerminado("1");
            else if (player2Tree.GetSize() == 5)
                retoTerminado("2");
        }

        private void reto2()
        {
            if (player1Tree != null && player1Tree.RootTwoChildren())
                retoTerminado("1");
            else if (player2Tree != null && player2Tree.RootTwoChildren())
                retoTerminado("2");
        }

        private void retoTerminado(string ganador)
        {
            if (ganador == "1")
                sumarScoreJ1();
            if (ganador == "2")
                sumarScoreJ2();

            player1Tree = null;
            player2Tree = null;
            panelArbolesJ1.Invalidate();
            panelArbolesJ2.Invalidate();
            elegirReto();
        }

        // Dibujar arboles para el J1
        private void panelArboles_PaintJ1(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Gray);

            if (player1Tree != null)
            {
                DibujarArbol(g, player1Tree.GetRoot(), 50, 20, 100);
            }

        }

        // Dibujar arboles para el J2
        private void panelArboles_PaintJ2(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Gray);

            if (player2Tree != null)
            {
                DibujarArbol(g, player2Tree.GetRoot(), 50, 20, 100);
            }
        }

        // Método para dibujar el árbol
        private void DibujarArbol(Graphics g, Node node, int x, int y, int dx)
        {
            if (node == null) return;

            // Dibujar nodo
            g.FillEllipse(Brushes.Cyan, x + 65, y - 10, 20, 20);
            g.DrawEllipse(Pens.Black, x + 65, y - 10, 20, 20);
            g.DrawString(node.key.ToString(), new Font("Arial", 8), Brushes.Black, new PointF(x + 68, y - 7));

            if (node.left != null)
            {
                g.DrawLine(Pens.Black, x + 65, y + 8, x + 35, y + 40);
                DibujarArbol(g, node.left, x - 40, y + 40, dx);
            }

            if (node.right != null)
            {
                g.DrawLine(Pens.Black, x + 85, y + 8, x + 120, y + 40);
                DibujarArbol(g, node.right, x + 45, y + 40, dx);
            }
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                goLeft1 = true;
            if (e.KeyCode == Keys.Right)
                goRight1 = true;
            if (e.KeyCode == Keys.NumPad1)
                GolpeJ1();
            if (e.KeyCode == Keys.Up && grounded1)
                verticalSpeed1 = -jumpStrength;

            if (e.KeyCode == Keys.A)
                goLeft2 = true;
            if (e.KeyCode == Keys.D)
                goRight2 = true;
            if (e.KeyCode == Keys.Space)
                GolpeJ2();
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
            lblTiempo.Text = "⏰: " + gameTime;

            if (gameTime <= 0)
            {
                timer1.Stop();
                gameTimer.Stop();
                MessageBox.Show("¡Tiempo terminado!");

                if (score1 > score2)
                    MessageBox.Show("¡Jugador 1 gana!");
                else if (score2 > score1)
                    MessageBox.Show("¡Jugador 2 gana!");
                else
                    MessageBox.Show("¡Empate!");

                Application.Exit();
            }

        }

        private void GeneratePlatforms()
        {
            int cantidad = 8;
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
                    int x = rnd.Next(50, this.ClientSize.Width - ancho - 350);
                    int y = 175 + i * espacioVertical + rnd.Next(-30, 30);
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


        }

    }
}