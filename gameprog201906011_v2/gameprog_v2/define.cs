using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gameprog_v2
{
    class define
    {
        /* 타일 사이즈 */
        public const int tile_size = 40;
        public const int left_tile_size_x = 20;
        public const int left_tile_size_y = 40;
        public const int right_tile_size_x = 20;
        public const int right_tile_size_y = 40;
        public const int top_tile_size_x = 40;
        public const int top_tile_size_y = 20;
        public const int bottom_tile_size_x = 40;
        public const int bottom_tile_size_y = 20;

        /* 캐릭터 사이즈 */
        public const int character_size = 40;

        /* 폭탄 사이즈 */
        public const int bomb_size = 40;
        public const int left_bomb_size_x = 20;
        public const int left_bomb_size_y = 40;
        public const int right_bomb_size_x = 20;
        public const int right_bomb_size_y = 40;
        public const int top_bomb_size_x = 40;
        public const int top_bomb_size_y = 20;
        public const int bottom_bomb_size_x = 40;
        public const int bottom_bomb_size_y = 20;

        /* 폭발 사이즈 */
        public const int explosion_size_x = 40;
        public const int explosion_size_y = 40;

        /* 파괴되는 블럭 사이즈 */
        public const int non_block_size_x = 40;
        public const int non_block_size_y = 40;

        /* 파괴되지 않는 블럭 사이즈 */
        public const int block_size_x = 40;
        public const int block_size_y = 40;

        /* 적 사이즈 */
        public const int enemy_size_x = 40;
        public const int enemy_size_y = 40;

        /* 아이템 사이즈 */
        public const int item_size_x = 40;
        public const int item_size_y = 40;


        /* 타일 사진 */
        public const string tile = @"\gamefile\cg\tile.jpg";
        public const string top_tile = @"\gamefile\cg\top_tile.jpg";
        public const string bottom_tile = @"\gamefile\cg\bottom_tile.jpg";
        public const string left_tile = @"\gamefile\cg\left_tile.jpg";
        public const string right_tile = @"\gamefile\cg\right_tile.jpg";

        /* 폭탄 사진 */
        public const string bomb = @"\gamefile\cg\bomb.jpg";
        public const string left_bomb = @"\gamefile\cg\left_bomb.jpg";
        public const string right_bomb = @"\gamefile\cg\right_bomb.jpg";
        public const string top_bomb = @"\gamefile\cg\top_bomb.jpg";
        public const string bottom_bomb = @"\gamefile\cg\bottom_bomb.jpg";

        /* 폭발 사진 */
        public const string center_explosion = @"\gamefile\cg\center_explosion.jpg";
        public const string left_explosion = @"\gamefile\cg\left_explosion.jpg";
        public const string right_explosion = @"\gamefile\cg\right_explosion.jpg";
        public const string top_explosion = @"\gamefile\cg\top_explosion.jpg";
        public const string bottom_explosion = @"\gamefile\cg\bottom_explosion.jpg";
        public const string right_colunm = @"\gamefile\cg\right_colunm.jpg";
        public const string left_colunm = @"\gamefile\cg\left_colunm.jpg";
        public const string top_colunm = @"\gamefile\cg\top_colunm.jpg";
        public const string bottom_colunm = @"\gamefile\cg\bottom_colunm.jpg";

        /* 파괴되는 블럭 사진 */
        public const string non_block = @"\gamefile\cg\destroyed_block.jpg";

        /* 파괴되지 않는 블럭 사진 */
        public const string block = @"\gamefile\cg\block.jpg";

        /* 적 사진 */
        public const string enemy = @"\gamefile\cg\enemy.jpg";
        public const string mushroom = @"\gamefile\cg\mushroom.jpg";

        /* 폭탄 아이템 사진 */
        public const string bomb_item = @"\gamefile\cg\bomb_item.jpg";

        /* 폭발 아이템 사진 */
        public const string explosion_item = @"\gamefile\cg\explosion_item.jpg";

        /* 폭발 사운드 */
        public const string bomb_sound = @"gamefile\sound\bomb_sound.wav";

        /* 배경 사운드 */
        public const string bgm_sound = @"gamefile\sound\bgm_sound.wav";

        /* 스테이지 클리어 */
        public const string stage_clear = @"\gamefile\cg\stage_clear.jpg";

        /* 1p 사진 */
        public const string _1p = @"\gamefile\cg\1p.jpg";

        /* 게임 오버 사진 */
        public const string game_over = @"\gamefile\cg\game_over.jpg";

        /* hide 사진 */
        public const string hide = @"\gamefile\cg\hide.jpg";

        /* 보스 사진 */
        public const string boss = @"\gamefile\cg\boss.jpg";

        /* 플레이어 2P */
        public const string player2 = @"\gamefile\cg\player2.jpg";

        /* 승리 사진 */
        public const string you_win = @"\gamefile\cg\you_win.jpg";

        /* 패배 사진 */
        public const string you_lose = @"\gamefile\cg\you_lose.jpg";
    }
}

