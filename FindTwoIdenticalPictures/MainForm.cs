using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindTwoIdenticalPictures
{
    public partial class MainForm : Form
    {
        private List<Card> cards = new List<Card>();
        private Card firstFlipped = null;
        private PictureBox firstPb = null;
        private bool isFlipping = false;
        private List<Image> images;
        private Image cardBackImage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeCards();
            InitializePictureBoxes();
        }

        private void InitializeImages()
        {
            cardBackImage = Properties.Resources.CardBack; // Directly assign the Bitmap object
            List<Image> allImages = new List<Image>
            {
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture1)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture2)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture3)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture4)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture5)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture6)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture7)),
                Image.FromStream(new System.IO.MemoryStream(Properties.Resources.picture8)),
            };

            Random rng = new Random();
            images = allImages.OrderBy(x => rng.Next()).Take(8).ToList();
        }

        private void InitializeCards()
        {
            cards.Clear();
            InitializeImages();

            for (int i = 0; i < 8; i++)
            {
                Card card1 = new Card { Image = images[i], Id = i };
                Card card2 = new Card { Image = images[i], Id = i };
                cards.Add(card1);
                cards.Add(card2);
            }

            Random rng = new Random();
            cards = cards.OrderBy(c => rng.Next()).ToList();
        }

        private void InitializePictureBoxes()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            for (int i = 0; i < 4; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    PictureBox pb = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Image = cardBackImage
                    };
                    pb.Click += PictureBox_Click;
                    pb.Tag = cards[row * 4 + col];
                    tableLayoutPanel1.Controls.Add(pb, col, row);
                }
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (isFlipping) return;
            PictureBox pb = (PictureBox)sender;
            Card card = (Card)pb.Tag;
            if (card.IsFlipped) return;

            card.IsFlipped = true;
            pb.Image = card.Image;

            if (firstFlipped == null)
            {
                firstFlipped = card;
                firstPb = pb;
            }
            else
            {
                isFlipping = true;
                if (firstFlipped.Id == card.Id)
                {
                    firstFlipped = null;
                    firstPb = null;
                    CheckForWin();
                    isFlipping = false;
                }
                else
                {
                    Timer timer = new Timer { Interval = 1000 };
                    timer.Tick += (s, ev) =>
                    {
                        timer.Stop();
                        firstFlipped.IsFlipped = false;
                        card.IsFlipped = false;
                        firstPb.Image = cardBackImage;
                        pb.Image = cardBackImage;
                        firstFlipped = null;
                        firstPb = null;
                        isFlipping = false;
                    };
                    timer.Start();
                }
            }
        }

        private void CheckForWin()
        {
            if (cards.All(c => c.IsFlipped))
            {
                MessageBox.Show("Поздравляем! Вы нашли все пары!");
                ResetGame();
            }
        }

        private void ResetGame()
        {
            // Сбрасываем состояние карт
            foreach (var card in cards)
            {
                card.IsFlipped = false;
            }
            // Перемешиваем карты заново
            Random rng = new Random();
            cards = cards.OrderBy(c => rng.Next()).ToList();
            // Обновляем PictureBox
            int index = 0;
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is PictureBox pb)
                {
                    pb.Image = cardBackImage;
                    pb.Tag = cards[index];
                    index++;
                }
            }
            // Сбрасываем переменные
            firstFlipped = null;
            firstPb = null;
            isFlipping = false;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void pictureBox3_Click(object sender, EventArgs e) { }
        private void pictureBox4_Click(object sender, EventArgs e) { }
        private void pictureBox8_Click(object sender, EventArgs e) { }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit the game?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            string rules = "Rules of the game \"Find two identical pictures\":\n\n" +
                           "1. The playing field is laid out with cards face down..\n" +
                           "2. The player opens two cards in turn.\n" +
                           "3. If the images on the cards match, they remain open.\n" +
                           "4. If the images are different, the cards are turned back over.\n" +
                           "5. The goal of the game is to find all pairs of identical pictures.\n\n" +
                           "Good luck!";
            MessageBox.Show(rules, "Rules of the game", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}