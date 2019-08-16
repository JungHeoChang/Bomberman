using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;

namespace gameprog_v2
{
    public partial class main : Form
    {
        bool right_key, left_key, up_key, down_key;
        bool bomb_flag = false;
        bool game_over = false;
        bool enemy1, enemy2, enemy3 = false;
        bool game_win = false;
        bool player_move = true;
        Map[] field = new Map[200];
        int[,] map = new int[13, 15];
        int[,] enemy_map = new int[13, 15];
        byte[] reci_map = new byte[195];
        Character[] character = new Character[2]; // character[0] = 본인, character[1] = 상대방
        Enemy[] enemy = new Enemy[3];

        int my_bomb = 1;
        int bomb_num = 0;
        int explosion_num = 1;
        int time_left = 120;
        int player_score = 0;
        int stage_num = 0;
        int boss_explosion_num = 3;
        int character_select=1;

        bool pvp_flag = false;

        static TcpClient tc = null;
        static NetworkStream stream = null;
        static int flag = 0;

        static int x2 = 0;
        static int y2 = 0;


        //좌표정보 구조체
        public struct Game_data
        {
            public int x;
            public int y;
            public int w; //폭탄, 캐릭터, 아이템 판별
            public int d;

            public Game_data(int x, int y, int w, int d)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.d = d;
            }
        }

        byte[] buff = new byte[Marshal.SizeOf(typeof(Game_data))];

        private SoundPlayer Player = new SoundPlayer();

        void Main_BGM()
        {
            SoundPlayer bgm_sound = new SoundPlayer(define.bgm_sound);
            bgm_sound.PlayLooping();
            //bgm_sound.Play();
        }

    

        public main()
        {
            InitializeComponent();
           
            Size = new Size(600, 550);

            Main_Mode();
            PVP_Mode();
            Stage_Mode();
            Thread bgm = new Thread(() => Main_BGM());
            bgm.Start();
            right_key = left_key = up_key = down_key = true;

            for (int i = 0; i < 200; i++)
            {
                field[i] = new Map();
            }

            for (int i = 0; i < 2; i++)
            {
                character[i] = new Character();
            }
            
            for(int i=0; i<3; i++)
            {
                enemy[i] = new Enemy();
            }

            player1_score.Text = "0";
            time.Visible = false;
            time.Location = new Point(240, 3);

            player1_score.Visible = false;
            player1_score.Location = new Point(100, 3);

            picture1p.Location = new Point(40, 3);
            picture1p.Visible = false;
            //back_button.Visible = false;
            back_button.Enabled = false;
            back_button.Location = new Point(500, 480);

            next_button.Enabled = false;
            next_button.Location = new Point(400, 480);

        }

        private void main_Load(object sender, EventArgs e)
        {
        }

        public void Main_Mode()
        {
            Graphics g = this.CreateGraphics();
            Image mains = Image.FromFile(Application.StartupPath + @"\gamefile\cg\main.jpg");
            this.BackgroundImage = mains;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public void PVP_Mode()
        {
            pvp.Load(Application.StartupPath + @"\gamefile\cg\battle.jpg");
            pvp.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void Stage_Mode()
        {
            stage.Load(Application.StartupPath + @"\gamefile\cg\story.jpg");
            stage.SizeMode = PictureBoxSizeMode.StretchImage;            
        }

        private void stage_Click(object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            pvp.Visible = false;
            stage.Visible = false;
            stage_num = 1;
            next_button.Enabled = false;
            player1_score.Visible = true;
            picture1p.Visible = true;
            time_left = 120;
            //if (stage_num == 1)
            //{
            Stage1();
            //            
        }

        private void pvp_Click(object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            pvp.Visible = false;
            stage.Visible = false;
            player1_score.Visible = false;
            picture1p.Visible = false;
            // 이제 켜질때 서버에 접속을 시도
            tc = new TcpClient("192.168.155.17", 7000);
            if (tc.Connected == false)
            {
                
                //Console.WriteLine("서버 연결 실패.");
                return;
            }
            pvp_flag = true;
            
            Thread thread = new Thread(new ParameterizedThreadStart(Data_Reci));
            thread.Start();


            // 이제 그 클라이언트 스트림도 저장해놓고
            stream = tc.GetStream();
            
            Map_Batch();
            Map_Draw();
            Block_Batch();
            Non_Block_Batch();
            Map_Obstacles();
            Multimap();
            
        }

        private void Stage1()
        {
            player_move = true;
            time.Visible = true;
            player1_score.Visible = true;
            time.Enabled = false;
            back_button.Enabled = false;
            Map_init();
            Map_Batch();
            Map_Draw();
            Block_Batch();
            Non_Block_Batch();
            Character_Batch();
            Map_Obstacles();
            Item_Batch();
            Player_Picture();
            Enemy_Map_init();
            Enemy_Map();
            Enemy_Batch();

            Thread pt_enemy = new Thread(() => Enemy_Move());
            pt_enemy.Start();
            timer.Start();
        }

        private void Stage2()
        {
            time_left = 120;
            player_move = true;
            game_win = false;
            enemy1 = false;
            enemy2 = false;
            game_over = false;
            next_button.Enabled = false;
            back_button.Enabled = false;

            Map_init();
            Map_Batch();
            Map_Draw();
            Stage2_Block_Batch();
            Stage2_Non_Block_Batch();
            Character_Batch();
            Map_Obstacles();
            Item_Batch();
            Enemy_Map_init();
            Enemy_Map();
            Enemy_Batch();


            Thread pt_enemy2 = new Thread(() => Enemy_Move());
            pt_enemy2.Start();

            timer.Start();
        }

        private void Stage_Boss()
        {
            time_left = 120;
            player_move = true;
            game_win = false;
            enemy1 = false;
            enemy2 = false;
            game_over = false;
            next_button.Enabled = false;
            back_button.Enabled = false;

            Map_init();
            Map_Batch();
            Map_Draw();
            Stage_Boss_Block_Batch();
            Character_Batch();
            Map_Obstacles();
            Enemy_Map_init();
            Enemy_Map();
            Enemy_Batch();

            Thread boss = new Thread(() => Enemy_Move());
            boss.Start();
        }

        private void Player_Picture()
        {
            picture1p.Load(Application.StartupPath + define._1p);
            picture1p.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void Map_init()
        {
            for(int i=0; i<13; i++)
            {
                for(int j=0; j<15; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        private void Map_Batch()
        {
            field[0].local.X = 40;
            field[0].local.Y = 40;
            int j = 0;

            for (int i = 1; i < 143; i++)
            {
                field[i].local.X = field[i - 1].local.X + define.tile_size;
                field[i].local.Y = field[i - 1].local.Y;
                j++;
                if (j == 13)
                {
                    field[i].local.X = 40;
                    field[i].local.Y += define.tile_size;
                    j = 0;
                }
            }
        }
        private void Enemy_Map_init()
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    enemy_map[i, j] = 0;
                }
            }
        }
        private void Enemy_Map()
        {
            for(int i=0; i<13; i++)
            {
                for(int j=0; j<15; j++)
                {
                    if (map[i, j] == 1 || map[i, j] == 2 || map[i, j] == 3 || map[i, j] == 4 || map[i, j] == 6)
                        enemy_map[i, j] = 999;
                    else
                        enemy_map[i, j] = 0;
                }
            }
        }

        public void Map_Draw()
        {
            Graphics g = this.CreateGraphics();
            Image tile = Image.FromFile(Application.StartupPath + @"\gamefile\cg\tile.jpg");
            
            for (int i = 0; i < 143; i++)
            {
                g.DrawImage(tile, field[i].local.X, field[i].local.Y, define.tile_size, define.tile_size);
            }
        }

        private void Block_Batch_Num(int y, params int[] args)
        {
            foreach (int x in args)
            {
                Block_Draw(define.block, x, y, define.block_size_x, define.block_size_y);
                Block_Obstacles(x, y);
            }
        }

        private void Block_Batch()
        {
            Block_Batch_Num(80, 80, 120, 160, 240, 320, 400, 440, 480);
            Block_Batch_Num(120, 80, 480);
            Block_Batch_Num(160, 80, 160, 200, 240, 320, 360, 400, 480);
            Block_Batch_Num(200, 160, 400);
            Block_Batch_Num(240, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(280, 80);
            Block_Batch_Num(320, 80, 160, 240, 320, 400, 440, 480, 520);
            Block_Batch_Num(360, 80);
            Block_Batch_Num(400, 80, 120, 160, 240, 320, 400, 480);
        }

        private void Stage2_Block_Batch()
        {
            Block_Batch_Num(40, 280);
            Block_Batch_Num(80, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(120, 160, 400);
            Block_Batch_Num(160, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(240, 40, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(320, 80, 160, 240, 320, 400, 480, 520);
            Block_Batch_Num(400, 80, 160, 200, 240, 280, 320, 360, 400, 480);
            Block_Batch_Num(440, 80, 480);
        }

        private void Stage_Boss_Block_Batch()
        {
            Block_Batch_Num(40, 160, 400);
            Block_Batch_Num(80, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(120, 160, 400);
            Block_Batch_Num(160, 80, 160, 240, 320, 400, 480);

            Block_Batch_Num(240, 40, 80, 160, 240, 320, 400, 480, 520);

            Block_Batch_Num(320, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(360, 160, 400);
            Block_Batch_Num(400, 80, 160, 240, 320, 400, 480);
            Block_Batch_Num(440, 160, 400);           
        }

        public void Block_Draw(string img, int X, int Y, int size_x, int size_y)
        {
            Graphics g = this.CreateGraphics();
            Image block = Image.FromFile(Application.StartupPath + img);
            g.DrawImage(block, X, Y, define.block_size_x, define.block_size_y);
        }

        public void Block_Obstacles(int x, int y)
        {
            int Ox, Oy;

            Ox = x / 40;
            Oy = y / 40;

            map[Oy, Ox] = 1;
        }

        public void Map_Obstacles()
        {
            int i;
            
            for (i = 0; i < 15; i++)
            {
                map[0, i] = 1;
                map[12, i] = 1;
            }
 
            for (i=0; i<13; i++)
            {
                map[i, 0] = 1;
                map[i, 14] = 1;
            }
            
            for (i=0; i<13; i++)
            {
                for(int j=0; j<15; j++)
                {
                    if (map[i, j] == 0)
                        map[i, j] = 0;
                }
            }
        }

        public void Non_Block_Obstacles(int x, int y)
        {
            int Ox, Oy;

            Ox = x / 40;
            Oy = y / 40;

            map[Oy, Ox] = 2;
        }

        private void Non_Block_Batch()
        {
            Non_Block_Num(40, 160, 240, 280); // y, x1, x2
            Non_Block_Num(80, 200, 280);
            Non_Block_Num(120, 520);
            Non_Block_Num(160, 40, 520);
            Non_Block_Num(200, 240, 320, 440, 480);
            Non_Block_Num(240, 40, 120, 200, 520);
            Non_Block_Num(280, 40, 120, 160, 240, 320);
            Non_Block_Num(320, 120, 360);
            Non_Block_Num(360, 240, 320, 400, 480);
            Non_Block_Num(400, 200, 360, 440);
        }        

        private void Stage2_Non_Block_Batch()
        {
            Non_Block_Num(40, 160, 360, 400); // y, x1, x2
            Non_Block_Num(80, 120, 280);
            Non_Block_Num(120, 240, 440, 480);
            Non_Block_Num(160, 200, 360, 520);
            Non_Block_Num(200, 40, 80, 160, 400, 520);
            Non_Block_Num(240, 120, 440);
            Non_Block_Num(280, 80, 160, 320, 400, 480);
            Non_Block_Num(320, 200, 360, 440);
            Non_Block_Num(360, 160, 240, 280, 400);
            Non_Block_Num(400, 120, 440);
            Non_Block_Num(440, 160, 200, 360, 400);
        }

        public void Non_Block_Num(int y, params int[] args)
        {
            foreach (int x in args)
            {
                Non_Block_Draw(define.non_block, x, y, define.non_block_size_x, define.non_block_size_y);
                Non_Block_Obstacles(x, y);
            }
        }

        public void Non_Block_Draw(string img, int X, int Y, int size_x, int size_y)
        {
            Graphics g = this.CreateGraphics();
            Image non_block = Image.FromFile(Application.StartupPath + img);
            g.DrawImage(non_block, X, Y, define.non_block_size_x, define.non_block_size_y);
        }

        public void Character_Draw(int X, int Y)
        {
            Graphics g = this.CreateGraphics();
            Image user;
            if (character_select == 1)
            {
                user = Image.FromFile(Application.StartupPath + @"\gamefile\cg\bomberman.jpg");
            }
            else
            {
                user = Image.FromFile(Application.StartupPath + define.player2);
            }
            character[0].local.X = X;
            character[0].local.Y = Y;

            g.DrawImage(user, character[0].local.X, character[0].local.Y, define.character_size, define.character_size);

            if(pvp_flag==true)
            {
                Game_data temp = new Game_data(X, Y, 0, explosion_num);
                buff = StructureToByte(temp);
                stream.Write(buff, 0, Marshal.SizeOf(temp));
                stream.Flush();
            }          
        }

        public void Tile_Draw(string img, int X, int Y, int size_x, int size_y)
        {
            Graphics g = this.CreateGraphics();
            Image tile = Image.FromFile(Application.StartupPath + img);
            
            g.DrawImage(tile, X, Y, size_x, size_y);
        }

        public void Clash()
        {
            Graphics g = this.CreateGraphics();
            Image over = Image.FromFile(Application.StartupPath + @"\gamefile\cg\game_over.jpg");
            int X = character[0].local.X;
            int Y = character[0].local.Y;
            int div = 40;

            /* 캐릭터 & 맵 충돌 */
            if (character[0].local.X == 40)
            {
                left_key = false;
            }
            if (character[0].local.Y == 40)
            {
                up_key = false;
            }
            if (character[0].local.X == 520)
            {
                right_key = false;
            }
            if (character[0].local.Y == 440)
            {
                down_key = false;
            }

            /* 캐릭터 & 폭탄 아이템 충돌*/
            if (map[character[0].local.Y / 40, character[0].local.X / 40] == 5)
            {
                my_bomb++;
                map[character[0].local.Y / 40, character[0].local.X / 40] = 0;
            }

            /* 캐릭터 & 폭발 아이템 충돌*/
            if(map[character[0].local.Y / 40, character[0].local.X / 40] == 7)
            {
                if(explosion_num < 4)
                    explosion_num++;
                map[Y / div, X / div] = 0;
            }

            /* 캐릭터 & 적 충돌*/
            for (int i = 0; i < 2; i++)
            {
                if (character[0].local.X == enemy[i].local.X && character[0].local.Y == enemy[i].local.Y)
                {
                    g.DrawImage(over, 60, 100);
                    Thread.Sleep(300);
                    g.DrawImage(over, 60, 100);
                    Thread.Sleep(300);
                    g.DrawImage(over, 60, 100);
                    Thread.Sleep(300);
                    game_over = true;
                    Back();                    
                }
            }              
        }

        /* 캐릭터 1p, 2p & 폭발 충돌 */
        public void Character_Explosion_Clash(int X, int Y)
        {
            int j=0;
            Graphics g = this.CreateGraphics();
            Image game = Image.FromFile(Application.StartupPath + @"\gamefile\cg\you_win.jpg");

            if (stage_num > 0)
            {
                Image over = Image.FromFile(Application.StartupPath + @"\gamefile\cg\game_over.jpg");

                if (character[j].local.X == X && character[j].local.Y == Y)
                {
                    g.DrawImage(over, 60, 100);
                    Thread.Sleep(300);
                    g.DrawImage(over, 60, 100);
                    Thread.Sleep(300);
                    g.DrawImage(over, 60, 100);
                    Thread.Sleep(300);
                    game_over = true;
                    Back();
                }
            }
            else
            {
                for (j = 0; j < 2; j++)
                {
                    if (character[j].local.X == X && character[j].local.Y == Y)
                    {
                        if (j == 0)
                        {
                            Image lose = Image.FromFile(Application.StartupPath + define.you_lose);
                            g.DrawImage(lose, 60, 50, 400, 200);
                        }
                        else if (j == 1)
                        {
                            Thread.Sleep(400);
                            g.DrawImage(game, 60, 50);
                            Thread.Sleep(400);
                            g.DrawImage(game, 60, 50);
                            Thread.Sleep(400);
                            g.DrawImage(game, 60, 50);
                        }
                    }
                }
            }
        }

        public void Bomb_Draw(string img, int X, int Y, int size_x, int size_y)
        {
            Graphics g = this.CreateGraphics();
            Image bomb = Image.FromFile(Application.StartupPath + img);
          
            g.DrawImage(bomb, X, Y, size_x, size_y);

            bomb_flag = false;
        }

        public void Bomb_Execute(string img, int X, int Y, int size_x, int size_y)
        {
            Graphics g = this.CreateGraphics();
            Image bomb = Image.FromFile(Application.StartupPath + img);

            g.DrawImage(bomb, X, Y, size_x, size_y);

            bomb_flag = false;

            if (pvp_flag == true)
            {
                Game_data temp = new Game_data(X, Y, 1, explosion_num);
                buff = StructureToByte(temp);
                stream.Write(buff, 0, Marshal.SizeOf(temp));
                stream.Flush();
            }

            Thread pt_bomb = new Thread(() => Run_Bomb(X, Y, define.explosion_size_x, define.explosion_size_y));
            pt_bomb.Start();
        }

        public async void Run_Bomb(int X, int Y, int size_x, int size_y)
        {
            enemy_map[Y / 40, X / 40] = 2;
            Thread.Sleep(1500);
            int a = 1;
;
            Explosion_Clash(X, Y, size_x, size_y);
            Thread.Sleep(300);
            enemy_map[Y / 40, X / 40] = 0;
            map[Y / 40, X / 40] = 0;
            bomb_num--;
            a--;
            bomb_num = a;           
        }        

        public void Explosion_Draw(string img, int X, int Y, int size_x, int size_y)
        {
            Graphics g = this.CreateGraphics();
            Image explosion = Image.FromFile(Application.StartupPath + img);

            g.DrawImage(explosion, X, Y, size_x, size_y);
        }

        public void Explosion_Clash(int X, int Y, int size_x, int size_y)
        {
            int i = 0;
            int j = 0;
            Graphics g = this.CreateGraphics();
            Image over = Image.FromFile(Application.StartupPath + @"\gamefile\cg\game_over.jpg");
            /* 숫자 의미 */
            /*
             * 0 = 길
             * 1 = 블록
             * 2 = 논블록
             * 3 = 폭탄
             * 4 = 논블록 안 폭탄 아이템
             * 5 = 논블록 밖 폭탄 아이템
             * 6 = 논블록 안 폭발 아이템
             * 7 = 논블록 밖 폭발 아이템
             * /
            
            /* 가운데 폭발 */
            Explosion_Draw(define.center_explosion, X, Y, size_x, size_y);

            /* 본인 플레이어 & 가운데 폭발 충돌 */
            for (i = 0; i < 2; i++)
            {
                if (X == character[i].local.X && Y == character[i].local.Y)
                {
                    if (i == 0)
                    {
                        g.DrawImage(over, 60, 100);
                        Thread.Sleep(300);
                        g.DrawImage(over, 60, 100);
                        Thread.Sleep(300);
                        g.DrawImage(over, 60, 100);
                        Thread.Sleep(300);

                        game_over = true;
                        player_move = false;
                        Back();
                    }
                    else
                    {
                        MessageBox.Show("승리");
                    }
                }
            }

            /* 오른쪽 폭발 */
            for(i = X + 40; i <= X + (explosion_num * 40); i += 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == X + (explosion_num * 40))
                        {
                            Explosion_Draw(define.right_explosion, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                            Enemy_Explosion_Clash(i, Y);
                        }
                        else
                        {
                            Explosion_Draw(define.right_colunm, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                            Enemy_Explosion_Clash(i, Y);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.right_explosion, i, Y, size_x, size_y);
                        Character_Explosion_Clash(i, Y);
                        Enemy_Explosion_Clash(i, Y);
                    }
                    
                    if (map[Y / 40, i / 40] == 2 || map[Y / 40, i / 40] == 4 || map[Y / 40, i /40] == 6)
                    {
                        //map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if(map[Y/40, i/40] == 5 || map[Y/40, i/40] == 7)
                    {
                        map[Y / 40, i / 40] = 0;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 왼쪽 폭발 */
            for (i = X - 40; i >= X - (explosion_num * 40); i -= 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == X + (explosion_num * 40))
                        {
                            Explosion_Draw(define.left_explosion, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                            Enemy_Explosion_Clash(i, Y);
                        }
                        else
                        {
                            Explosion_Draw(define.left_colunm, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                            Enemy_Explosion_Clash(i, Y);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.left_explosion, i, Y, size_x, size_y);
                        Character_Explosion_Clash(i, Y);
                        Enemy_Explosion_Clash(i, Y);
                    }

                    if (map[Y / 40, i / 40] == 2 || map[Y/40, i/40] == 4 || map[Y/40, i/40] == 6)
                    {
                        //map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if(map[Y/40, i/40] == 5 || map[Y/40, i/40] == 7)
                    {
                        map[Y / 40, i / 40] = 0;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }
            Bomb_Sound();
            /* 위쪽 폭발 */
            for (i = Y - 40; i >= Y - (explosion_num * 40); i -= 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == Y + (explosion_num * 40))
                        {
                            Explosion_Draw(define.top_explosion, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                            Enemy_Explosion_Clash(X, i);
                        }
                        else
                        {
                            Explosion_Draw(define.top_colunm, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                            Enemy_Explosion_Clash(X, i);
                        }
                        }
                    else
                    {
                        Explosion_Draw(define.top_explosion, X, i, size_x, size_y);
                        Character_Explosion_Clash(X, i);
                        Enemy_Explosion_Clash(X, i);
                    }

                    if (map[i / 40, X / 40] == 2 || map[i / 40, X / 40] == 4 || map[i / 40, X / 40] == 6)
                        break;
                    else if (map[i / 40, X / 40] == 5 || map[i / 40, X / 40] == 7)
                        map[Y / 40, i / 40] = 0;
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            /* 아래쪽 폭발 */
            for(i = Y + 40; i <= Y + (explosion_num * 40); i += 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == Y + (explosion_num * 40))
                        {
                            Explosion_Draw(define.bottom_explosion, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                            Enemy_Explosion_Clash(X, i);
                        }
                        else
                        {
                            Explosion_Draw(define.bottom_colunm, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                            Enemy_Explosion_Clash(X, i);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.bottom_explosion, X, i, size_x, size_y);
                        Character_Explosion_Clash(X, i);
                        Enemy_Explosion_Clash(X, i);
                    }

                    if (map[i / 40, X / 40] == 2 || map[i / 40, X / 40] == 4 || map[i / 40, X / 40] == 6)
                        break;
                    else if (map[i / 40, X / 40] == 5 || map[i / 40, X / 40] == 7)
                        map[Y / 40, i / 40] = 0;
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }
            
            Thread.Sleep(500);
            
            /* 가운데 그림 */
            Tile_Draw(define.tile, X, Y, size_x, size_y);

            /* 오른쪽 그림 */
            for (i = X + 40; i <= X + (explosion_num * 40); i += 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == X + (explosion_num * 40))
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                        else
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    
                    if (map[Y / 40, i / 40] == 2)
                    {
                        map[Y / 40, i / 40] = 0;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if(map[Y/40, i/40] == 4)
                    {
                        Bomb_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 5;
                        enemy_map[Y / 40, i / 40] = 0;

                        break;
                    }
                    else if(map[Y/40, i/40] == 6)
                    {
                        Explosion_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 7;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 왼쪽 그림 */
            for (i = X - 40; i >= X - (explosion_num * 40); i -= 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == X + (explosion_num * 40))
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                        else
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }

                    if (map[Y / 40, i / 40] == 2)
                    {
                        map[Y / 40, i / 40] = 0;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if(map[Y/40, i/40] == 4)
                    {
                        Bomb_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 5;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if(map[Y/40, i/40] == 6)
                    {
                        Explosion_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 7;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 위쪽 그림 */
            for (i = Y - 40; i >= Y - (explosion_num * 40); i -= 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == Y + (explosion_num * 40))
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                        else
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, X, i, size_x, size_y);
                    }

                    if (map[i / 40, X / 40] == 2)
                    {
                        map[i / 40, X / 40] = 0;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if(map[i/40, X/40] == 4)
                    {
                        Bomb_Item_Draw(X, i);
                        map[i / 40, X / 40] = 5;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if(map[i/40, X/40] == 6)
                    {
                        Explosion_Item_Draw(X, i);
                        map[i / 40, X / 40] = 7;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            /* 아래쪽 그림 */
            for (i = Y + 40; i <= Y + (explosion_num * 40); i += 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (explosion_num > 1)
                    {
                        if (i == Y + (explosion_num * 40))
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                        else
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, X, i, size_x, size_y);
                    }

                    if (map[i / 40, X / 40] == 2)
                    {
                        map[i / 40, X / 40] = 0;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if(map[i/40, X/40] == 4)
                    {
                        Bomb_Item_Draw(X, i);
                        map[i / 40, X / 40] = 5;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if(map[i/40, X/40] == 6)
                    {
                        Explosion_Item_Draw(X, i);
                        map[i / 40, X / 40] = 7;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }          
        }

        public void Boss_Bomb(int X, int Y)
        {
            Graphics g = this.CreateGraphics();
            Image bomb = Image.FromFile(Application.StartupPath + define.bomb);

            g.DrawImage(bomb, Y*40, X*40, 40, 40);

            Thread boss_bomb = new Thread(() => Boss_Run_Bomb(X, Y));
            boss_bomb.Start();
        }


        public async void Boss_Run_Bomb(int X, int Y)
        {
            enemy_map[X, Y] = 2;
            map[X, Y] = 2;
            Thread.Sleep(1500);

            Boss_Bomb_Explosion(Y*40, X*40);
            Thread.Sleep(300);
            enemy_map[X, Y] = 0;
            map[X, Y] = 0;
        }

        public void Boss_Bomb_Explosion(int X, int Y)
        {
            int i = 0;
            int j = 0;
            int size_x = 40, size_y = 40;
            Graphics g = this.CreateGraphics();
            Image over = Image.FromFile(Application.StartupPath + @"\gamefile\cg\game_over.jpg");
            
            
            /* 가운데 폭발 */
            Explosion_Draw(define.center_explosion, X, Y, size_x, size_y);

            /* 본인 플레이어 & 가운데 폭발 충돌 */
            for (i = 0; i < 2; i++)
            {
                if (X == character[i].local.X && Y == character[i].local.Y)
                {
                    if (i == 0)
                    {
                        g.DrawImage(over, 60, 100);
                        Thread.Sleep(300);
                        g.DrawImage(over, 60, 100);
                        Thread.Sleep(300);
                        g.DrawImage(over, 60, 100);
                        Thread.Sleep(300);

                        game_over = true;
                        player_move = false;
                        Back();
                    }
                }
            }

            /* 오른쪽 폭발 */
            for (i = X + 40; i <= X + (boss_explosion_num * 40); i += 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == X + (boss_explosion_num * 40))
                        {
                            Explosion_Draw(define.right_explosion, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                        }
                        else
                        {
                            Explosion_Draw(define.right_colunm, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.right_explosion, i, Y, size_x, size_y);
                        Character_Explosion_Clash(i, Y);
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 왼쪽 폭발 */
            for (i = X - 40; i >= X - (boss_explosion_num * 40); i -= 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == X + (boss_explosion_num * 40))
                        {
                            Explosion_Draw(define.left_explosion, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                        }
                        else
                        {
                            Explosion_Draw(define.left_colunm, i, Y, size_x, size_y);
                            Character_Explosion_Clash(i, Y);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.left_explosion, i, Y, size_x, size_y);
                        Character_Explosion_Clash(i, Y);
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }
            Bomb_Sound();
            /* 위쪽 폭발 */
            for (i = Y - 40; i >= Y - (boss_explosion_num * 40); i -= 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == Y + (boss_explosion_num * 40))
                        {
                            Explosion_Draw(define.top_explosion, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                        }
                        else
                        {
                            Explosion_Draw(define.top_colunm, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.top_explosion, X, i, size_x, size_y);
                        Character_Explosion_Clash(X, i);
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            /* 아래쪽 폭발 */
            for (i = Y + 40; i <= Y + (boss_explosion_num * 40); i += 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == Y + (boss_explosion_num * 40))
                        {
                            Explosion_Draw(define.bottom_explosion, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                        }
                        else
                        {
                            Explosion_Draw(define.bottom_colunm, X, i, size_x, size_y);
                            Character_Explosion_Clash(X, i);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.bottom_explosion, X, i, size_x, size_y);
                        Character_Explosion_Clash(X, i);
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            Thread.Sleep(500);

            /* 가운데 그림 */
            Tile_Draw(define.tile, X, Y, size_x, size_y);

            /* 오른쪽 그림 */
            for (i = X + 40; i <= X + (boss_explosion_num * 40); i += 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == X + (boss_explosion_num * 40))
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                        else
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 왼쪽 그림 */
            for (i = X - 40; i >= X - (boss_explosion_num * 40); i -= 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == X + (boss_explosion_num * 40))
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                        else
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 위쪽 그림 */
            for (i = Y - 40; i >= Y - (boss_explosion_num * 40); i -= 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == Y + (boss_explosion_num * 40))
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                        else
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            /* 아래쪽 그림 */
            for (i = Y + 40; i <= Y + (boss_explosion_num * 40); i += 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (boss_explosion_num > 1)
                    {
                        if (i == Y + (boss_explosion_num * 40))
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                        else
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }
        }
        class Map
        {
            public Point local;
        }

        class Character
        {
            public Point local;
        }

        class Explosion
        {
            public Point local;
        }
        
        class Enemy
        {
            public Point local;
            public int life;
        }
        

        private void Character_Batch()
        {
            Character_Draw(40, 40);
        }
        
        private void Enemy_Batch()
        {
            if (stage_num == 1)
            {
                enemy[0].local.X = 80;
                enemy[0].local.Y = 440;
                enemy[1].local.X = 520;
                enemy[1].local.Y = 280;
                enemy[0].life = 1;
                enemy[1].life = 1;
            }
            else if(stage_num == 2)
            {
                enemy[0].local.X = 280;
                enemy[0].local.Y = 200;
                enemy[1].local.X = 40;
                enemy[1].local.Y = 440;

                enemy[0].life = 1;
                enemy[1].life = 1;
            }
            else if(stage_num == 3)
            {
                enemy[0].local.X = 320;
                enemy[0].local.Y = 280;
            }

            if (stage_num == 3)
            {
                Enemy_Draw(enemy[0].local.X, enemy[0].local.Y, 0);
                enemy[0].life = 5;
            }
            else
            {
                Enemy_Draw(enemy[0].local.X, enemy[0].local.Y, 0);
                Enemy_Draw(enemy[1].local.X, enemy[1].local.Y, 1);
            }
        }

        public void Enemy_Draw(int X, int Y, int i)
        {
            Graphics g = this.CreateGraphics();
            string img;
            int num;

            if (stage_num == 1)
                img = define.enemy;
            else if (stage_num == 2)
            {
                Random r = new Random();
                num = r.Next(1, 10);

                if (num == 1 || num == 2 || num == 3)
                {
                    img = define.hide;
                }
                else
                {
                    img = define.mushroom;
                }

            }
            else if(stage_num == 3)
            {
                img = define.boss;
            }
            else
                img = define.enemy;

            Image ienemy = Image.FromFile(Application.StartupPath + img);
            

            enemy[i].local.X = X;
            enemy[i].local.Y = Y;

            g.DrawImage(ienemy, enemy[i].local.X, enemy[i].local.Y, define.enemy_size_x, define.enemy_size_y);
        }

        public void Enemy_Move()
        {
            Graphics g = this.CreateGraphics();

            while (game_over == false)
            {
                if (enemy1 == false)
                {                 
                    Thread search_0 = new Thread(() => Search(0));
                    search_0.Start();
                }

                if (enemy2 == false && stage_num != 3)
                {
                    Thread search_1 = new Thread(() => Search(1));
                    search_1.Start();
                }
                /*
                if(stage_num == 2)
                {
                    if(enemy3 == false)
                    {
                        Thread search_3 = new Thread(() => Search(2));
                        search_3.Start();
                    }
                }
                */
                if(enemy1 == true && enemy2 == true)
                {
                    Image clear = Image.FromFile(Application.StartupPath + @"\gamefile\cg\stage_clear.jpg");

                    
                    game_over = true;
                    game_win = true;
                    player_move = false;
                    
                    g.DrawImage(clear, 100, 150);
                    Thread.Sleep(300);
                    g.DrawImage(clear, 100, 150);
                    Thread.Sleep(300);
                    g.DrawImage(clear, 100, 150);
                    Thread.Sleep(300);
                    g.DrawImage(clear, 100, 150);

                    Thread.Sleep(1000);
                    stage_num++;                                      
                }
                else if(enemy1 == true && stage_num == 3)
                {
                    Image clear = Image.FromFile(Application.StartupPath + @"\gamefile\cg\stage_clear.jpg");


                    game_over = true;
                    game_win = true;
                    player_move = false;
                    Back();

                    g.DrawImage(clear, 100, 150);
                    Thread.Sleep(300);
                    g.DrawImage(clear, 100, 150);
                    Thread.Sleep(300);
                    g.DrawImage(clear, 100, 150);
                    Thread.Sleep(300);
                    g.DrawImage(clear, 100, 150);

                    Thread.Sleep(1000);
                }

                if(stage_num == 3)
                {
                    Thread.Sleep(500);
                }
                else
                    Thread.Sleep(700); 

                if (stage_num == 3 && game_over == false)
                {
                    Random r = new Random();
                    int n;

                    n = r.Next(1, 10);

                    if (n == 1)
                    {
                        Thread bomb = new Thread(() => Boss_Skill());
                        bomb.Start();
                    }
                }
            }
            if (stage_num == 2)
            {
                Invoke(new Action(delegate ()
                {
                    next_button.Enabled = true;
                }
                            ));
            }
            else if(stage_num == 3)
            {
                Invoke(new Action(delegate ()
                {
                    next_button.Enabled = true;
                }
                            ));
            }
        }
        
        public void Boss_Move()
        {
            int num;

            while(game_over == false)
            {
                if(enemy1 == false)
                {
                    Thread boss_move = new Thread(() => Search(0));
                    boss_move.Start();
                }
            }
        }
        
        public void Boss_Skill()
        {
            Random r = new Random();
            int x, y;
            int num=0;
            
            
            while (num < 1)
            {
                x = r.Next(1, 12);
                y = r.Next(1, 14);
                if (map[x, y] == 0)
                {
                    Boss_Bomb(x, y);
                    num += 1;
                }
            }
            
        }

        public void Enemy_Clash(int X, int Y)
        {
            Graphics g = this.CreateGraphics();
            Image over = Image.FromFile(Application.StartupPath + @"\gamefile\cg\game_over.jpg");
            int i = 0;
            if (X == character[i].local.X && Y == character[i].local.Y)
            {
                g.DrawImage(over, 60, 100);
                Thread.Sleep(300);
                g.DrawImage(over, 60, 100);
                Thread.Sleep(300);
                g.DrawImage(over, 60, 100);
                Thread.Sleep(300);
                Back();
                game_over = true;
            }

            if(map[enemy[i].local.Y/40, enemy[i].local.X/40] == 5 || map[enemy[i].local.Y / 40, enemy[i].local.X / 40] == 7)
            {
                map[enemy[i].local.Y / 40, enemy[i].local.X / 40] = 0;
            }
        }

        public void Enemy_Explosion_Clash(int X, int Y)
        {
            int j;

            if (stage_num > 0)
            {
                for (j = 0; j < 2; j++)
                {
                    if (enemy[j].local.X == X && enemy[j].local.Y == Y)
                    {
                        if (j == 0)
                        {                          
                            enemy[j].life -= 1;

                            if (enemy[j].life <= 0)
                            {
                                enemy1 = true;
                                enemy[j].local.X = 0;
                                enemy[j].local.Y = 0;
                            }

                            player_score += 100;
                            Invoke(new Action(delegate ()
                            {
                                player1_score.Text = string.Format("{0}", player_score);
                            }
                            ));
                        }
                        else if (j == 1)
                        {
                            enemy[j].life -= 1;

                            if (enemy[j].life <= 0)
                            {
                                enemy2 = true;
                                enemy[j].local.X = 0;
                                enemy[j].local.Y = 0;
                            }

                            player_score += 100;
                            Invoke(new Action(delegate ()
                            {
                                player1_score.Text = string.Format("{0}", player_score);
                            }
                            ));
                        }
                    }
                }
            }
        }

        public void Enemy_Clash_List()
        {
            Thread clash_0 = new Thread(() => Enemy_Clash(enemy[0].local.X, enemy[0].local.Y));
            clash_0.Start();

            Thread clash_1 = new Thread(() => Enemy_Clash(enemy[1].local.X, enemy[1].local.Y));
            clash_1.Start();
        }
        
        private void Item_Batch()
        {
            Random r = new Random();
            int num;

            for(int i=0; i<13; i++)
            {
                for(int j=0; j<15; j++)
                {
                    if(map[i,j] == 2)
                    {
                        num = r.Next(1, 7);
                        
                        if(num == 1)
                        {
                            map[i, j] = 4;
                        }
                        else if(num == 2)
                        {
                            map[i, j] = 6;
                        }
                    }
                }
            }           
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            game_over = true;
        }

        public void Bomb_Item_Draw(int X, int Y)
        {
            Graphics g = CreateGraphics();
            Image bomb_item = Image.FromFile(Application.StartupPath + define.bomb_item);

            g.DrawImage(bomb_item, X, Y, define.item_size_x, define.item_size_y);
        }       

        public void Explosion_Item_Draw(int X, int Y)
        {
            Graphics g = CreateGraphics();
            Image explosion_item = Image.FromFile(Application.StartupPath + define.explosion_item);

            g.DrawImage(explosion_item, X, Y, define.item_size_x, define.item_size_y);
        }

        private void back_button_Click(object sender, EventArgs e)
        {
            Main_Mode();
            stage.Visible = true;
            pvp.Visible = true;

            bomb_flag = false;
            game_over = false;
            enemy1 = false;
            enemy2 = false;
            game_win = false;
            player_move = true;
            player1_score.Text = "0";
            player_score = 0;
            my_bomb = 1;
            explosion_num = 1;
            time_left = 120;
        }

        private void next_Click(object sender, EventArgs e)
        {
            if(stage_num == 2)
            {
                Stage2();
            }
            else if(stage_num == 3)
            {
                Stage_Boss();
            }
        }

        private void 게임방법ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
        }

        private void Bomb_Sound()
        {
            SoundPlayer bomb_sound = new SoundPlayer(define.bomb_sound);
            bomb_sound.Play();
        }

        

        public void Search(int k)
        {
            bool right, left, top, bottom;
            right = left = top = bottom = false;

            Random r = new Random();
            int num=0;
            int n = 0;
            int X = enemy[k].local.X;
            int Y = enemy[k].local.Y;

            if (enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X + 40) / 40] == 0)
            {
                right = true;
                n++;
            }
            else
                right = false;

            if (enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X - 40) / 40] == 0)
            {
                left = true;
                n++;
            }
            else
                left = false;

            if (enemy_map[(enemy[k].local.Y - 40) / 40, enemy[k].local.X / 40] == 0)
            {
                top = true;
                n++;
            }
            else
                top = false;

            if (enemy_map[(enemy[k].local.Y + 40) / 40, enemy[k].local.X / 40] == 0)
            {
                bottom = true;
                n++;
            }
            else
                bottom = false;

            if(n == 0)
            {
                for(int i=0; i<13; i++)
                {
                    for(int j=0; j<15; j++)
                    {
                        if (enemy_map[i, j] == 1)
                            enemy_map[i, j] = 0;
                    }
                }
                enemy_map[enemy[k].local.Y / 40, enemy[k].local.X / 40] = 1;
            }

            if(n == 1)
            {
                if(right == true)
                {
                    Enemy_Draw(enemy[k].local.X += 40, enemy[k].local.Y, k);
                    Tile_Draw(define.tile, enemy[k].local.X-40, enemy[k].local.Y, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
                else if(left == true)
                {
                    Enemy_Draw(enemy[k].local.X -= 40, enemy[k].local.Y, k);
                    Tile_Draw(define.tile, enemy[k].local.X+40, enemy[k].local.Y, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
                else if(top == true)
                {
                    Enemy_Draw(enemy[k].local.X, enemy[k].local.Y -= 40, k);
                    Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y+40, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
                else if(bottom == true)
                {
                    Enemy_Draw(enemy[k].local.X, enemy[k].local.Y += 40, k);
                    Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y-40, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
            }

            while(n >= 2)
            {
                if(right == true)
                {
                    num = r.Next(1, 5);

                    if(num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X += 40, enemy[k].local.Y, k);
                        Tile_Draw(define.tile, enemy[k].local.X-40, enemy[k].local.Y, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                    else
                    {
                        //right = false;
                    }
                }
                if(left == true)
                {
                    num = r.Next(1, 4);

                    if(num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X -= 40, enemy[k].local.Y, k);
                        Tile_Draw(define.tile, enemy[k].local.X+40, enemy[k].local.Y, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                    else
                    {
                        //left = false;
                    }
                }
                if(top == true)
                {
                    num = r.Next(1, 3);

                    if (num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X, enemy[k].local.Y -= 40, k);
                        Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y+40, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                    else
                    {
                        //top = false;
                    }
                }
                if(bottom == true)
                {
                    num = r.Next(1, 3);

                    if (num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X, enemy[k].local.Y += 40, k);
                        Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y-40, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                    else
                    {
                        //bottom = false;
                    }
                }
            }
            Enemy_Clash(enemy[k].local.X, enemy[k].local.Y);
        }

        public void Boss_Search(int k)
        {
            bool right, left, top, bottom;
            right = left = top = bottom = false;

            Random r = new Random();
            int num = 0;
            int n = 0;
            int X = enemy[k].local.X;
            int Y = enemy[k].local.Y;

            if (enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X + 40) / 40] == 0)
            {
                right = true;
                n++;
            }
            else
                right = false;

            if (enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X - 40) / 40] == 0)
            {
                left = true;
                n++;
            }
            else
                left = false;

            if (enemy_map[(enemy[k].local.Y - 40) / 40, enemy[k].local.X / 40] == 0)
            {
                top = true;
                n++;
            }
            else
                top = false;

            if (enemy_map[(enemy[k].local.Y + 40) / 40, enemy[k].local.X / 40] == 0)
            {
                bottom = true;
                n++;
            }
            else
                bottom = false;

            if (n == 0)
            {
                for (int i = 0; i < 13; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (enemy_map[i, j] == 1)
                            enemy_map[i, j] = 0;
                    }
                }
                enemy_map[enemy[k].local.Y / 40, enemy[k].local.X / 40] = 1;
            }

            if (n == 1)
            {
                if (right == true)
                {
                    Enemy_Draw(enemy[k].local.X += 40, enemy[k].local.Y, k);
                    Tile_Draw(define.tile, enemy[k].local.X - 40, enemy[k].local.Y, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
                else if (left == true)
                {
                    Enemy_Draw(enemy[k].local.X -= 40, enemy[k].local.Y, k);
                    Tile_Draw(define.tile, enemy[k].local.X + 40, enemy[k].local.Y, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
                else if (top == true)
                {
                    Enemy_Draw(enemy[k].local.X, enemy[k].local.Y -= 40, k);
                    Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y + 40, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
                else if (bottom == true)
                {
                    Enemy_Draw(enemy[k].local.X, enemy[k].local.Y += 40, k);
                    Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y - 40, define.tile_size, define.tile_size);
                    enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                }
            }

            while (n >= 2)
            {
                if (right == true)
                {
                    num = r.Next(1, 5);

                    if (num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X += 40, enemy[k].local.Y, k);
                        Tile_Draw(define.tile, enemy[k].local.X - 40, enemy[k].local.Y, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                }
                if (left == true)
                {
                    num = r.Next(1, 4);

                    if (num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X -= 40, enemy[k].local.Y, k);
                        Tile_Draw(define.tile, enemy[k].local.X + 40, enemy[k].local.Y, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                }
                if (top == true)
                {
                    num = r.Next(1, 3);

                    if (num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X, enemy[k].local.Y -= 40, k);
                        Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y + 40, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                }
                if (bottom == true)
                {
                    num = r.Next(1, 3);

                    if (num == 1)
                    {
                        Enemy_Draw(enemy[k].local.X, enemy[k].local.Y += 40, k);
                        Tile_Draw(define.tile, enemy[k].local.X, enemy[k].local.Y - 40, define.tile_size, define.tile_size);
                        enemy_map[enemy[k].local.Y / 40, (enemy[k].local.X / 40)] = 1;
                        n = 1;
                        break;
                    }
                }
            }
            Enemy_Clash(enemy[k].local.X, enemy[k].local.Y);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (time_left > 0)
            {
                time_left = time_left - 1;
                time.Text = string.Format("{0}", time_left);

                if (game_win == true)
                {
                    timer.Stop();
                    player_score += time_left * 10;
                    Invoke(new Action(delegate ()
                    {
                        player1_score.Text = string.Format("{0}", player_score);
                    }
                        ));
                    game_win = false;
                    timer.Start();
                }
            }
            else
            {
                timer.Stop();
                game_over = true;
                Graphics g = this.CreateGraphics();
                Image over = Image.FromFile(Application.StartupPath + @"\gamefile\cg\game_over.jpg");
                g.DrawImage(over, 100, 150);
                Back();
            }
        }

        public void Back()
        {
            Invoke(new Action(delegate ()
            {
                back_button.Enabled = true;
            }
                            ));
        }

        public void Thread_send(object o)
        {
            //NetworkStream stream = tc.GetStream();
            Game_data data = new Game_data();

            data = (Game_data)o;
            buff = StructureToByte(data);
            stream.Write(buff, 0, Marshal.SizeOf(data));
            stream.Flush();

            // tc객체에서 오는 리비서 등록 및 콜백 함수 등록 끝
            ThreadPool.QueueUserWorkItem(Data_Reci, tc);
            //stream.Close();
            //clientSocket.Close();

        }


        public void Data_Reci(object o)
        {

            TcpClient client = o as TcpClient;
            NetworkStream sr = (client.GetStream());


            while (reci_map[0] == 0)
            {
                stream.Read(reci_map, 0, reci_map.Length);
            }

            for (int i = 0; i < reci_map.Length; i++)
            {
                if (i % 15 == 0)
                {
                    Console.WriteLine();
                }
                Console.Write(reci_map[i] + " ");

            }

            byte[] jong = new byte[1];

            while (true)
            {
                stream.Read(jong, 0, jong.Length);

                if (jong[0] == 1)
                {
                    character_select = jong[0];
                    Character_Draw_pvp(40, 40);
                    break;
                }
                else if (jong[0] == 2)
                {
                    character_select = jong[0];
                    Character_Draw_pvp(520, 40);
                    break;

                }
            }



            // 계속 받기
            while (true) //520 40
            {
                byte[] outbuf = new byte[Marshal.SizeOf(typeof(Game_data))];

                // 데이터 받기
                stream.Read(outbuf, 0, outbuf.Length);
                object obj = ByteToStructure(outbuf, typeof(Game_data));

                Game_data data = (Game_data)obj;

                if (data.w == 0)
                {
                    if (data.x > 600 || data.y > 600)
                    {

                    }
                    else
                    {
                        Character_Draw_test(data.x, data.y);
                        x2 = data.x;
                        y2 = data.y;
                    }

                }

                else if (data.w == 1)
                {
                    if (data.x > 600 || data.y > 600)
                    {

                    }
                    else
                    {
                        Bomb_Execute_test_pvp(define.bomb, data.x, data.y, define.bomb_size, define.bomb_size, data.d);
                    }

                }

            }
        }

        public void Character_Draw_pvp(int X, int Y)
        {

            Graphics g = this.CreateGraphics();
            Image user;
            //Image other = Image.FromFile(Application.StartupPath + @"\gamefile\cg\bomberman.jpg");
            if (character_select == 1)
                user = Image.FromFile(Application.StartupPath + @"\gamefile\cg\bomberman.jpg");
            else
                user = Image.FromFile(Application.StartupPath + define.player2);

            character[0].local.X = X;
            character[0].local.Y = Y;
            //character[1].local.X = 40;
            //character[1].local.Y = 40;


            g.DrawImage(user, character[0].local.X, character[0].local.Y, define.character_size, define.character_size);
            //g.DrawImage(other, character[1].local.X, character[1].local.Y, define.character_size, define.character_size);

            Game_data temp = new Game_data(X, Y, 0, explosion_num);
            buff = StructureToByte(temp);
            stream.Write(buff, 0, Marshal.SizeOf(temp));
            stream.Flush();

        }

        public void Character_Draw_test(int X, int Y)
        {

            if (map[y2 / 40, x2 / 40] == 3)
            {
                Bomb_Draw(define.bomb, x2, y2, define.bomb_size, define.bomb_size);
            }

            else if ((character[1].local.X != X) || (character[1].local.Y != Y))
            {
                Tile_Draw(define.tile, character[1].local.X, character[1].local.Y, define.tile_size, define.tile_size);
            }


            character[1].local.X = X;
            character[1].local.Y = Y;

            Graphics g = this.CreateGraphics();
            Image user;
            
            if(character_select == 2)
                user = Image.FromFile(Application.StartupPath + @"\gamefile\cg\bomberman.jpg");
            else
                user = Image.FromFile(Application.StartupPath + define.player2);

            g.DrawImage(user, character[1].local.X, character[1].local.Y, define.character_size, define.character_size);

        }

        public void Character_Explosion_Clash_pvp(int X, int Y)
        {
            int j;
            Graphics g = this.CreateGraphics();
            Image game;

            for (j = 0; j < 2; j++)
            {
                if (character[j].local.X == X && character[j].local.Y == Y)
                {
                    if (j == 0)
                    {
                        game = Image.FromFile(Application.StartupPath + @"\gamefile\cg\you_lose.jpg");
                        Thread.Sleep(400);
                        g.DrawImage(game, 60, 50, 400, 200);
                        Thread.Sleep(400);
                        g.DrawImage(game, 60, 50, 400, 200);
                        Thread.Sleep(400);
                        g.DrawImage(game, 60, 50, 400, 200);
                    }
                }
            }
        }

        public void Bomb_Execute_pvp(string img, int X, int Y, int size_x, int size_y)
        {
            int[,] map = new int[13, 15];

            Graphics g = this.CreateGraphics();
            Image bomb = Image.FromFile(Application.StartupPath + img);

            g.DrawImage(bomb, X, Y, size_x, size_y);

            Game_data temp = new Game_data(X, Y, 1, explosion_num);
            buff = StructureToByte(temp);
            stream.Write(buff, 0, Marshal.SizeOf(temp));
            stream.Flush();

            Thread pt_bomb = new Thread(() => Run_Bomb(X, Y, define.explosion_size_x, define.explosion_size_y));
            pt_bomb.Start();
        }

        public void Bomb_Execute_test_pvp(string img, int X, int Y, int size_x, int size_y, int d)
        {
            Graphics g = this.CreateGraphics();
            Image bomb = Image.FromFile(Application.StartupPath + img);

            g.DrawImage(bomb, X, Y, size_x, size_y);

            map[Y / 40, X / 40] = 3;

            //bomb_flag = false;
            Thread pt_tbomb = new Thread(() => Run_test(X, Y, define.explosion_size_x, define.explosion_size_y, d));
            pt_tbomb.Start();

        }

        public async void Run_test(int X, int Y, int size_x, int size_y, int d)
        {
            enemy_map[Y / 40, X / 40] = 2;
            Thread.Sleep(1500);
            //int a = 1;

            Explosion_Clash_test_pvp(X, Y, size_x, size_y, d);
            Thread.Sleep(300);
            enemy_map[Y / 40, X / 40] = 0;
            map[Y / 40, X / 40] = 0;
        }

       

        public void Explosion_Clash_test_pvp(int X, int Y, int size_x, int size_y, int d)
        {
            int i = 0;
            int j = 0;
            
            /* 가운데 폭발 */
            Explosion_Draw(define.center_explosion, X, Y, size_x, size_y);

            /* 본인 플레이어 & 가운데 폭발 충돌 */
            for (i = 0; i < 2; i++)
            {
                if (X == character[i].local.X && Y == character[i].local.Y)
                {
                    Character_Explosion_Clash_pvp(X, Y);
                }
            }

            /* 오른쪽 폭발 */
            for (i = X + 40; i <= X + (d * 40); i += 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == X + (d * 40))
                        {
                            Explosion_Draw(define.right_explosion, i, Y, size_x, size_y);
                            Character_Explosion_Clash_pvp(i, Y);
                        }
                        else
                        {
                            Explosion_Draw(define.right_colunm, i, Y, size_x, size_y);
                            Character_Explosion_Clash_pvp(i, Y);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.right_explosion, i, Y, size_x, size_y);
                        Character_Explosion_Clash_pvp(i, Y);
                    }

                    if (map[Y / 40, i / 40] == 2 || map[Y / 40, i / 40] == 4 || map[Y / 40, i / 40] == 6)
                    {
                        //map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if (map[Y / 40, i / 40] == 5 || map[Y / 40, i / 40] == 7)
                    {
                        map[Y / 40, i / 40] = 0;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 왼쪽 폭발 */
            for (i = X - 40; i >= X - (d * 40); i -= 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == X + (d * 40))
                        {
                            Explosion_Draw(define.left_explosion, i, Y, size_x, size_y);
                            Character_Explosion_Clash_pvp(i, Y);
                        }
                        else
                        {
                            Explosion_Draw(define.left_colunm, i, Y, size_x, size_y);
                            Character_Explosion_Clash_pvp(i, Y);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.left_explosion, i, Y, size_x, size_y);
                        Character_Explosion_Clash_pvp(i, Y);
                    }

                    if (map[Y / 40, i / 40] == 2 || map[Y / 40, i / 40] == 4 || map[Y / 40, i / 40] == 6)
                    {
                        //map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if (map[Y / 40, i / 40] == 5 || map[Y / 40, i / 40] == 7)
                    {
                        map[Y / 40, i / 40] = 0;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }
            Bomb_Sound();
            /* 위쪽 폭발 */
            for (i = Y - 40; i >= Y - (d * 40); i -= 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == Y + (d * 40))
                        {
                            Explosion_Draw(define.top_explosion, X, i, size_x, size_y);
                            Character_Explosion_Clash_pvp(X, i);
                        }
                        else
                        {
                            Explosion_Draw(define.top_colunm, X, i, size_x, size_y);
                            Character_Explosion_Clash_pvp(X, i);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.top_explosion, X, i, size_x, size_y);
                        Character_Explosion_Clash_pvp(X, i);
                    }

                    if (map[i / 40, X / 40] == 2 || map[i / 40, X / 40] == 4 || map[i / 40, X / 40] == 6)
                        break;
                    else if (map[i / 40, X / 40] == 5 || map[i / 40, X / 40] == 7)
                        map[Y / 40, i / 40] = 0;
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            /* 아래쪽 폭발 */
            for (i = Y + 40; i <= Y + (d * 40); i += 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == Y + (d * 40))
                        {
                            Explosion_Draw(define.bottom_explosion, X, i, size_x, size_y);
                            Character_Explosion_Clash_pvp(X, i);
                        }
                        else
                        {
                            Explosion_Draw(define.bottom_colunm, X, i, size_x, size_y);
                            Character_Explosion_Clash_pvp(X, i);
                        }
                    }
                    else
                    {
                        Explosion_Draw(define.bottom_explosion, X, i, size_x, size_y);
                        Character_Explosion_Clash_pvp(X, i);
                    }

                    if (map[i / 40, X / 40] == 2 || map[i / 40, X / 40] == 4 || map[i / 40, X / 40] == 6)
                        break;
                    else if (map[i / 40, X / 40] == 5 || map[i / 40, X / 40] == 7)
                        map[Y / 40, i / 40] = 0;
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            Thread.Sleep(500);

            /* 가운데 그림 */
            Tile_Draw(define.tile, X, Y, size_x, size_y);

            /* 오른쪽 그림 */
            for (i = X + 40; i <= X + (d * 40); i += 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == X + (d * 40))
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                        else
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }

                    if (map[Y / 40, i / 40] == 2)
                    {
                        map[Y / 40, i / 40] = 0;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if (map[Y / 40, i / 40] == 4)
                    {
                        Bomb_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 5;
                        enemy_map[Y / 40, i / 40] = 0;

                        break;
                    }
                    else if (map[Y / 40, i / 40] == 6)
                    {
                        Explosion_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 7;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 왼쪽 그림 */
            for (i = X - 40; i >= X - (d * 40); i -= 40)
            {
                if (map[Y / 40, i / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == X + (d * 40))
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                        else
                            Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, i, Y, size_x, size_y);
                    }

                    if (map[Y / 40, i / 40] == 2)
                    {
                        map[Y / 40, i / 40] = 0;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if (map[Y / 40, i / 40] == 4)
                    {
                        Bomb_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 5;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                    else if (map[Y / 40, i / 40] == 6)
                    {
                        Explosion_Item_Draw(i, Y);
                        map[Y / 40, i / 40] = 7;
                        enemy_map[Y / 40, i / 40] = 0;
                        break;
                    }
                }
                else if (map[Y / 40, i / 40] == 1)
                    break;
            }

            /* 위쪽 폭발 */
            for (i = Y - 40; i >= Y - (d * 40); i -= 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == Y + (d * 40))
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                        else
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, X, i, size_x, size_y);
                    }

                    if (map[i / 40, X / 40] == 2)
                    {
                        map[i / 40, X / 40] = 0;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if (map[i / 40, X / 40] == 4)
                    {
                        Bomb_Item_Draw(X, i);
                        map[i / 40, X / 40] = 5;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if (map[i / 40, X / 40] == 6)
                    {
                        Explosion_Item_Draw(X, i);
                        map[i / 40, X / 40] = 7;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }

            /* 아래쪽 폭발 */
            for (i = Y + 40; i <= Y + (d * 40); i += 40)
            {
                if (map[i / 40, X / 40] != 1)
                {
                    if (d > 1)
                    {
                        if (i == Y + (d * 40))
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                        else
                            Tile_Draw(define.tile, X, i, size_x, size_y);
                    }
                    else
                    {
                        Tile_Draw(define.tile, X, i, size_x, size_y);
                    }

                    if (map[i / 40, X / 40] == 2)
                    {
                        map[i / 40, X / 40] = 0;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if (map[i / 40, X / 40] == 4)
                    {
                        Bomb_Item_Draw(X, i);
                        map[i / 40, X / 40] = 5;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                    else if (map[i / 40, X / 40] == 6)
                    {
                        Explosion_Item_Draw(X, i);
                        map[i / 40, X / 40] = 7;
                        enemy_map[i / 40, X / 40] = 0;
                        break;
                    }
                }
                else if (map[i / 40, X / 40] == 1)
                    break;
            }
        }

        public void Multimap()
        {
            int k = 0;
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    map[i, j] = reci_map[k];
                    k++;
                }
            }
        }

        //바이트배열를 구조체로 변환 메소드
        public static object ByteToStructure(byte[] data, Type type)
        {
            /*
            관리되지 않는 메모리 블록을 복사하고, 
            관리되는 형식을 관리되지 않는 형식으로 변환하는 메서드의 컬렉션 및 비관리 코드와, 
            상호 작용할 때 사용되는 기타 메서드의 컬렉션을 제공합니다.
            */
            IntPtr buff = Marshal.AllocHGlobal(data.Length); //배열의 크기만큼 비관리 영역에 매모리를 할당한다. 
            Marshal.Copy(data, 0, buff, data.Length); //배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
            object obj = Marshal.PtrToStructure(buff, type); //복사된 데이터를 구조체 객체로 변환한다.
            Marshal.FreeHGlobal(buff); //비관리 메모리 영역에 할당했던 메모리를 해제함.

            if (Marshal.SizeOf(obj) != data.Length) //구조체와 원래의 데이터 크기 비교.
            {
                return null; //크기가 다르면 null 리턴.
            }
            return obj; //성공이면 object 형식으로 리턴.
        }


        //구조체를 바이트배열로 변환 베소드
        public static byte[] StructureToByte(object obj)
        {
            int datasize = Marshal.SizeOf(obj); // 구조체에 할당된 메모리의 크기를 구한다.
            IntPtr buff = Marshal.AllocHGlobal(datasize); // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
            Marshal.StructureToPtr(obj, buff, false); // 할당된 구조체 객체의 주소를 구한다.
            byte[] data = new byte[datasize]; // 구조체가 복사될 배열.
            Marshal.Copy(buff, data, 0, datasize); // 구조체 객체를 배열에 복사.
            Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함.
            return data; // 바이트 배열을 리턴.
        }

        private void main_KeyDown(object sender, KeyEventArgs e)
        {
            int X = character[0].local.X;
            int Y = character[0].local.Y;
            int div = 40;

            switch (e.KeyCode)
            {               
                case Keys.Left:
                    Clash();
                    if (left_key == true && player_move == true)
                    {
                        if (map[(character[0].local.Y / 40), ((character[0].local.X - 40) / 40)] == 0 || map[(character[0].local.Y / 40), ((character[0].local.X - 40) / 40)] == 5 || map[(character[0].local.Y / 40), ((character[0].local.X - 40) / 40)] == 7)
                        {
                            Character_Draw(character[0].local.X -= 40, character[0].local.Y);
                            if (bomb_flag == false)
                                Tile_Draw(define.tile, character[0].local.X + 40, character[0].local.Y, define.tile_size, define.tile_size);
                            else
                                Bomb_Draw(define.bomb, character[0].local.X + 40, character[0].local.Y, define.bomb_size, define.bomb_size);
                        }
                    }
                    left_key = true;
                    break;

                case Keys.Right:
                    Clash();
                    if (right_key == true && player_move == true)
                    {
                        if (map[(character[0].local.Y / 40), ((character[0].local.X + 40) / 40)] == 0 || map[(character[0].local.Y / 40), ((character[0].local.X + 40) / 40)] == 5 || map[(character[0].local.Y / 40), ((character[0].local.X + 40) / 40)] == 7)
                        {
                            Character_Draw(character[0].local.X += 40, character[0].local.Y);
                            if (bomb_flag == false)
                                Tile_Draw(define.tile, character[0].local.X - 40, character[0].local.Y, define.tile_size, define.tile_size);
                            else
                                Bomb_Draw(define.bomb, character[0].local.X - 40, character[0].local.Y, define.bomb_size, define.bomb_size);
                        }
                    }
                    right_key = true;
                    break;

                case Keys.Up:
                    Clash();
                    if (up_key == true && player_move == true)
                    {
                        if (map[((character[0].local.Y - 40) / 40), (character[0].local.X / 40)] == 0 || map[((character[0].local.Y - 40) / 40), (character[0].local.X / 40)] == 5 || map[((character[0].local.Y - 40) / 40), (character[0].local.X / 40)] == 7)
                        {
                            Character_Draw(character[0].local.X, character[0].local.Y -= 40);
                            if (bomb_flag == false)
                                Tile_Draw(define.tile, character[0].local.X, character[0].local.Y + 40, define.tile_size, define.tile_size);
                            else
                                Bomb_Draw(define.bomb, character[0].local.X, character[0].local.Y + 40, define.bomb_size, define.bomb_size);
                        }
                    }
                    up_key = true;
                    break;

                case Keys.Down:
                    Clash();
                    if (down_key == true && player_move == true)
                    {
                        if (map[((character[0].local.Y + 40) / 40), (character[0].local.X / 40)] == 0 || map[((character[0].local.Y + 40) / 40), (character[0].local.X / 40)] == 5 || map[((character[0].local.Y + 40) / 40), (character[0].local.X / 40)] == 7)
                        {
                            Character_Draw(character[0].local.X, character[0].local.Y += 40);
                            if (bomb_flag == false)
                                Tile_Draw(define.tile, character[0].local.X, character[0].local.Y - 40, define.tile_size, define.tile_size);
                            else
                                Bomb_Draw(define.bomb, character[0].local.X, character[0].local.Y - 40, define.bomb_size, define.bomb_size);
                        }
                    }
                    down_key = true;
                    break;

                case Keys.Space:
                    Clash();
                    if (player_move == true)
                    {
                        if (bomb_num < my_bomb)
                        {
                            Bomb_Execute(define.bomb, character[0].local.X, character[0].local.Y, define.bomb_size, define.bomb_size);
                            bomb_num++;
                            map[(character[0].local.Y / 40), (character[0].local.X / 40)] = 3;
                            bomb_flag = true;
                        }
                    }
                    break;
            }
        }
    } 
}
