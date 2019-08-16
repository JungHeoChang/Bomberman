using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gameprog_v2
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();

            label1.Text = "키보드 방향키를 이용하여 상하좌우로 이동할 수 있습니다.";
            label2.Text = "키보드 스페이스바를 이용하여 폭탄을 설치할 수 있습니다.";
            label8.Text = "이용하여 모든 적을 섬멸하면 됩니다.";
            label11.Text = "플레이어의 폭탄 개수를 늘려줍니다.";
            label12.Text = "폭탄의 화염 범위를 늘려줍니다.";
            label13.Text = "폭탄으로 파괴할 수 없는 블록이며 플레이어 또는 적이 이동할 수 없습니다.";
            label14.Text = "폭탄으로 파괴할 수 있으며 파괴하기 전까지는 이동할 수 없습니다. \r\n파괴했을 경우 랜덤으로 아이템이 나오며 이동또한 가능합니다.";
        }
    }
}
