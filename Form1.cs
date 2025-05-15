
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

        // Poderes de los jugadores
        bool[] player1Powers = new bool[3]; // [0] = Force-push, [1] = Shield, [2] = Air-jump
        bool[] player2Powers = new bool[3]; // [0] = Force-push, [1] = Shield, [2] = Air-jump
        bool shieldActiveP1 = false;
        bool shieldActiveP2 = false;
        int shieldDurationP1 = 0;
        int shieldDurationP2 = 0;
        int shieldMaxDuration = 200; // 4 segundos a 50ms por tick
        bool airJumpAvailableP1 = false;
        bool airJumpAvailableP2 = false;
        int forcePushCooldownP1 = 0;
        int forcePushCooldownP2 = 0;
        int forcePushMaxCooldown = 150; // 3 segundos a 50ms por tick
        bool retoActivo = true;

        // Indicadores visuales de poderes
        PictureBox[] player1PowerIcons = new PictureBox[3];
        PictureBox[] player2PowerIcons = new PictureBox[3];
        PictureBox shieldP1;
        PictureBox shieldP2;

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

            // Crear iconos de poderes para jugador 1
            SetupPowerIcons();

            GeneratePlatforms();
            elegirReto();
        }

        private void SetupPowerIcons()
        {
            // Iconos de poderes para jugador 1
            string[] powerImages = { "force_push.png", "shield.png", "air_jump.png" };

            // Intenta cargar las imágenes, si no existen usa colores
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    player1PowerIcons[i] = new PictureBox();
                    player1PowerIcons[i].Size = new Size(30, 30);
                    player1PowerIcons[i].Location = new Point(50 + i * 35, 40);
                    try
                    {
                        player1PowerIcons[i].Image = Image.FromFile(powerImages[i]);
                        player1PowerIcons[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch
                    {
                        player1PowerIcons[i].BackColor = Color.FromArgb(100, 100, 100);
                    }
                    player1PowerIcons[i].Visible = false;
                    this.Controls.Add(player1PowerIcons[i]);
                }

                // Iconos de poderes para jugador 2
                for (int i = 0; i < 3; i++)
                {
                    player2PowerIcons[i] = new PictureBox();
                    player2PowerIcons[i].Size = new Size(30, 30);
                    player2PowerIcons[i].Location = new Point(850 + i * 35, 40);
                    try
                    {
                        player2PowerIcons[i].Image = Image.FromFile(powerImages[i]);
                        player2PowerIcons[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch
                    {
                        player2PowerIcons[i].BackColor = Color.FromArgb(100, 100, 100);
                    }
                    player2PowerIcons[i].Visible = false;
                    this.Controls.Add(player2PowerIcons[i]);
                }

                // Shield para jugador 1
                shieldP1 = new PictureBox();
                shieldP1.Size = new Size(80, 80);
                shieldP1.BackColor = Color.FromArgb(100, 0, 200, 255);
                shieldP1.Location = new Point(jugador1.Left - 5, jugador1.Top - 5);
                shieldP1.Visible = false;
                this.Controls.Add(shieldP1);

                // Shield para jugador 2
                shieldP2 = new PictureBox();
                shieldP2.Size = new Size(80, 80);
                shieldP2.BackColor = Color.FromArgb(100, 0, 200, 255);
                shieldP2.Location = new Point(jugador2.Left - 5, jugador2.Top - 5);
                shieldP2.Visible = false;
                this.Controls.Add(shieldP2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al cargar iconos de poderes: " + ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (jugador1Vivo)
                MoverJugador(jugador1, ref verticalSpeed1, goLeft1, goRight1, ref grounded1, ref jugador1Vivo, ref airJumpAvailableP1);

            if (jugador2Vivo)
                MoverJugador(jugador2, ref verticalSpeed2, goLeft2, goRight2, ref grounded2, ref jugador2Vivo, ref airJumpAvailableP2);

            // Actualizar posición de escudos si están activos
            if (shieldActiveP1)
            {
                shieldP1.Location = new Point(jugador1.Left - 5, jugador1.Top - 5);
                shieldDurationP1--;
                if (shieldDurationP1 <= 0)
                {
                    shieldActiveP1 = false;
                    shieldP1.Visible = false;
                }
            }

            if (shieldActiveP2)
            {
                shieldP2.Location = new Point(jugador2.Left - 5, jugador2.Top - 5);
                shieldDurationP2--;
                if (shieldDurationP2 <= 0)
                {
                    shieldActiveP2 = false;
                    shieldP2.Visible = false;
                }
            }

            // Reducir cooldowns de force push
            if (forcePushCooldownP1 > 0)
                forcePushCooldownP1--;

            if (forcePushCooldownP2 > 0)
                forcePushCooldownP2--;
            if (retoActivo)
            {
                switch (reto)
                {
                    case 1:
                        reto1();
                        break;
                    case 2:
                        reto2();
                        break;
                    case 3:
                        reto3();
                        break;
                    case 4:
                        reto4();
                        break;
                    case 5:
                        reto5();
                        break;
                    case 6:
                        reto6();
                        break;
                    case 7:
                        reto7();
                        break;
                }
            }
      
            

            DetectarColisionEntreJugadores();
        }

        private void MoverJugador(PictureBox jugador, ref int verticalSpeed, bool goLeft, bool goRight, ref bool grounded, ref bool jugadorVivo, ref bool airJumpAvailable)
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
                    airJumpAvailable = true; // Restablecer air-jump cuando está en plataforma
                }
            }

            // Verificar si el jugador cayó fuera de la pantalla
            if (jugador.Top > this.ClientSize.Height)
            {
                if (jugador.Top > this.ClientSize.Height)
                {
                    jugadorVivo = false;
                    jugador.Visible = false;

                    // Determinar qué jugador cayó
                    if (jugador == jugador1)
                    {
                        sumarScoreJ2(); // El jugador 2 gana puntos por eliminar al jugador 1
                        MessageBox.Show("¡Jugador 1 eliminado! Jugador 2 gana 10 puntos.");
                    }
                    else
                    {
                        sumarScoreJ1(); // El jugador 1 gana puntos por eliminar al jugador 2
                        MessageBox.Show("¡Jugador 2 eliminado! Jugador 1 gana 10 puntos.");
                    }
                }
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
                    if (shieldActiveP2)
                        return; // No hacer daño si el escudo está activo

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
                    if (shieldActiveP1)
                        return; // No hacer daño si el escudo está activo

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

        // Force Push del Jugador 1
        private void ForcePushJ1()
        {
            if (!player1Powers[0] || forcePushCooldownP1 > 0)
                return;

            if (jugador1Vivo && jugador2Vivo)
            {
                // Verificar si el jugador 2 tiene escudo activo
                if (shieldActiveP2)
                    return;

                int distanciaX = Math.Abs(jugador1.Left - jugador2.Left);
                int distanciaY = Math.Abs(jugador1.Top - jugador2.Top);

                // Force Push tiene un rango más grande que un golpe normal
                if (distanciaX < 200 && distanciaY < 100)
                {
                    int direccion = jugador1.Left < jugador2.Left ? 1 : -1;
                    jugador2.Left += direccion * 150; // Empuje más fuerte
                    forcePushCooldownP1 = forcePushMaxCooldown;

                    // Agregar efecto visual (opcional)
                    PictureBox forcePushEffect = new PictureBox();
                    forcePushEffect.Size = new Size(50, 50);
                    forcePushEffect.BackColor = Color.FromArgb(150, 255, 255, 0);
                    forcePushEffect.Location = new Point(jugador2.Left - 25, jugador2.Top);
                    this.Controls.Add(forcePushEffect);

                    // Timer para quitar el efecto
                    System.Windows.Forms.Timer effectTimer = new System.Windows.Forms.Timer();
                    effectTimer.Interval = 300;
                    effectTimer.Tick += (s, e) => {
                        this.Controls.Remove(forcePushEffect);
                        forcePushEffect.Dispose();
                        effectTimer.Stop();
                    };
                    effectTimer.Start();
                }
            }
        }

        // Force Push del Jugador 2
        private void ForcePushJ2()
        {
            if (!player2Powers[0] || forcePushCooldownP2 > 0)
                return;

            if (jugador1Vivo && jugador2Vivo)
            {
                // Verificar si el jugador 1 tiene escudo activo
                if (shieldActiveP1)
                    return;

                int distanciaX = Math.Abs(jugador1.Left - jugador2.Left);
                int distanciaY = Math.Abs(jugador1.Top - jugador2.Top);

                // Force Push tiene un rango más grande que un golpe normal
                if (distanciaX < 200 && distanciaY < 100)
                {
                    int direccion = jugador2.Left < jugador1.Left ? 1 : -1;
                    jugador1.Left += direccion * 150; // Empuje más fuerte
                    forcePushCooldownP2 = forcePushMaxCooldown;

                    // Agregar efecto visual (opcional)
                    PictureBox forcePushEffect = new PictureBox();
                    forcePushEffect.Size = new Size(50, 50);
                    forcePushEffect.BackColor = Color.FromArgb(150, 255, 255, 0);
                    forcePushEffect.Location = new Point(jugador1.Left - 25, jugador1.Top);
                    this.Controls.Add(forcePushEffect);

                    // Timer para quitar el efecto
                    System.Windows.Forms.Timer effectTimer = new System.Windows.Forms.Timer();
                    effectTimer.Interval = 300;
                    effectTimer.Tick += (s, e) => {
                        this.Controls.Remove(forcePushEffect);
                        forcePushEffect.Dispose();
                        effectTimer.Stop();
                    };
                    effectTimer.Start();
                }
            }
        }

        // Activar escudo para Jugador 1
        private void ActivateShieldJ1()
        {
            if (!player1Powers[1])
                return;

            shieldActiveP1 = true;
            shieldDurationP1 = shieldMaxDuration;
            shieldP1.Location = new Point(jugador1.Left - 5, jugador1.Top - 5);
            shieldP1.Visible = true;
            shieldP1.BringToFront();
        }

        // Activar escudo para Jugador 2
        private void ActivateShieldJ2()
        {
            if (!player2Powers[1])
                return;

            shieldActiveP2 = true;
            shieldDurationP2 = shieldMaxDuration;
            shieldP2.Location = new Point(jugador2.Left - 5, jugador2.Top - 5);
            shieldP2.Visible = true;
            shieldP2.BringToFront();
        }

        // Activar Air Jump para Jugador 1 (se usa automáticamente al caer)
        private void ActivateAirJumpJ1()
        {
            if (!player1Powers[2] || !airJumpAvailableP1)
                return;

            if (!grounded1)
            {
                verticalSpeed1 = (int)(-jumpStrength * 1.5f);
                airJumpAvailableP1 = false;
            }
        }

        // Activar Air Jump para Jugador 2 (se usa automáticamente al caer)
        private void ActivateAirJumpJ2()
        {
            if (!player2Powers[2] || !airJumpAvailableP2)
                return;

            if (!grounded2)
            {
                verticalSpeed2 = (int)(-jumpStrength * 1.5f);
                airJumpAvailableP2 = false;
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
            reto = rnd.Next(1, 8); // 7 retos en total
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
                    player1Tree = new AVLTree();
                    player2Tree = new AVLTree();
                    lblreto.Text = "Crea un AVL balanceado con altura 3";
                    break;

                case 4:
                    lblreto.Text = "Crea un BST con 7 nodos en orden ascendente";
                    player1Tree = new BSTree();
                    player2Tree = new BSTree();
                    break;

                case 5:
                    lblreto.Text = "Crea un BST con altura máxima de profundidad 4";
                    player1Tree = new BSTree();
                    player2Tree = new BSTree();
                    
                    break;

                case 6:
                    player1Tree = new BTree(3); // Orden 3 para el B-Tree
                    player2Tree = new BTree(3);
                    lblreto.Text = "Crea un B-Tree con al menos 2 nodos internos";
                    break;

                case 7:
                    player1Tree = new BTree(3);
                    player2Tree = new BTree(3);
                    lblreto.Text = "Crea un B-Tree con 6 claves en total";
                    break;
            }
            retoActivo = true;
        }

        // Otorgar un poder aleatorio al jugador
        private void OtorgarPoderAleatorio(string jugador)
        {
            int poderIndex = rnd.Next(0, 3); // 0 = Force-push, 1 = Shield, 2 = Air-jump

            if (jugador == "1")
            {
                // Desactivar todos los poderes anteriores
                for (int i = 0; i < player1Powers.Length; i++)
                {
                    player1Powers[i] = false;
                    player1PowerIcons[i].Visible = false;
                }
                // Activar solo el nuevo poder
                player1Powers[poderIndex] = true;
                player1PowerIcons[poderIndex].Visible = true;

                string nombrePoder = GetNombrePoder(poderIndex);
                MessageBox.Show($"¡Jugador 1 obtiene el poder: {nombrePoder}!");
            }
            else
            {
                // Desactivar todos los poderes anteriores
                for (int i = 0; i < player2Powers.Length; i++)
                {
                    player2Powers[i] = false;
                    player2PowerIcons[i].Visible = false;
                }
                // Activar solo el nuevo poder
                player2Powers[poderIndex] = true;
                player2PowerIcons[poderIndex].Visible = true;

                string nombrePoder = GetNombrePoder(poderIndex);
                MessageBox.Show($"¡Jugador 2 obtiene el poder: {nombrePoder}!");
            }
        }

        private string GetNombrePoder(int index)
        {
            switch (index)
            {
                case 0: return "Force Push (Empuje)";
                case 1: return "Shield (Escudo)";
                case 2: return "Air Jump (Salto en Aire)";
                default: return "Desconocido";
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
        // Reto 3: AVL balanceado con altura 3
        private void reto3()
        {
            if (IsAVLWithHeight3(player1Tree))
                retoTerminado("1");
            else if (IsAVLWithHeight3(player2Tree))
                retoTerminado("2");
        }

        private bool IsAVLWithHeight3(ITree tree)
        {
            if (tree == null || tree.GetRoot() == null)
                return false;

            AVLTree avlTree = tree as AVLTree;
            if (avlTree == null)
                return false;

            Node root = tree.GetRoot();

            // Verificar que la altura sea exactamente 3
            if (root.height != 3)
                return false;

            // Verificar que el árbol esté balanceado (esto ya está garantizado por AVL)
            return true;
        }

        // Reto 4: BST con 7 nodos en orden ascendente
        private void reto4()
        {
            if (HasSevenNodesInAscendingOrder(player1Tree))
                retoTerminado("1");
            else if (HasSevenNodesInAscendingOrder(player2Tree))
                retoTerminado("2");
        }

        private bool HasSevenNodesInAscendingOrder(ITree tree)
        {
            if (tree == null || tree.GetSize() != 7)
                return false;

            string inOrder = tree.PrintInOrder();
            string[] values = inOrder.Trim().Split(' ');

            if (values.Length != 7)
                return false;

            // Verificar que los valores estén en orden ascendente
            for (int i = 1; i < values.Length; i++)
            {
                if (int.Parse(values[i]) <= int.Parse(values[i - 1]))
                    return false;
            }

            return true;
        }

        // Reto 5: BST con altura máxima de profundidad 4
        private void reto5()
        {
            if (HasMaxDepthFour(player1Tree))
                retoTerminado("1");
            else if (HasMaxDepthFour(player2Tree))
                retoTerminado("2");
        }

        private bool HasMaxDepthFour(ITree tree)
        {
            if (tree == null || tree.GetRoot() == null)
                return false;

            return GetMaxDepth(tree.GetRoot()) <= 4 && tree.GetSize() >= 5;
        }

        private int GetMaxDepth(Node node)
        {
            if (node == null)
                return 0;

            int leftDepth = GetMaxDepth(node.left);
            int rightDepth = GetMaxDepth(node.right);

            return Math.Max(leftDepth, rightDepth) + 1;
        }

        // Reto 6: B-Tree con al menos 2 nodos internos
        private void reto6()
        {
            if (HasTwoInternalNodes(player1Tree))
                retoTerminado("1");
            else if (HasTwoInternalNodes(player2Tree))
                retoTerminado("2");
        }

        private bool HasTwoInternalNodes(ITree tree)
        {
            if (tree == null || tree.GetRoot() == null)
                return false;

            BTree bTree = tree as BTree;
            if (bTree == null)
                return false;

            return bTree.CountInternalNodes() >= 2;
        }

        // Reto 7: B-Tree con 6 claves en total
        private void reto7()
        {
            if (HasSixKeys(player1Tree))
                retoTerminado("1");
            else if (HasSixKeys(player2Tree))
                retoTerminado("2");
        }

        private bool HasSixKeys(ITree tree)
        {
            if (tree == null)
                return false;

            BTree bTree = tree as BTree;
            if (bTree == null)
                return false;

            return bTree.CountKeys() >= 6;
        }

        private void retoTerminado(string ganador)
        {
            retoActivo = false;

            // Reiniciar movimiento y velocidad de ambos jugadores
            goLeft1 = goRight1 = grounded1 = false;
            goLeft2 = goRight2 = grounded2 = false;
            verticalSpeed1 = 0;
            verticalSpeed2 = 0;

            if (ganador == "1")
    {
                sumarScoreJ1();
                OtorgarPoderAleatorio("1"); // Otorga poder a Jugador 1
            }
            if (ganador == "2")
            {
                sumarScoreJ2();
                OtorgarPoderAleatorio("2"); // Otorga poder a Jugador 2
            }

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

            // Poderes Jugador 1
            if (e.KeyCode == Keys.NumPad9)
                ForcePushJ1();
            if (e.KeyCode == Keys.NumPad8)
                ActivateShieldJ1();
            if (e.KeyCode == Keys.NumPad7)
                ActivateAirJumpJ1();

            if (e.KeyCode == Keys.A)
                goLeft2 = true;
            if (e.KeyCode == Keys.D)
                goRight2 = true;
            if (e.KeyCode == Keys.Space)
                GolpeJ2();
            if (e.KeyCode == Keys.W && grounded2)
                verticalSpeed2 = -jumpStrength;

            // Poderes Jugador 2
            if (e.KeyCode == Keys.Q)
                ForcePushJ2();
            if (e.KeyCode == Keys.E)
                ActivateShieldJ2();
            if (e.KeyCode == Keys.R)
                ActivateAirJumpJ2();
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