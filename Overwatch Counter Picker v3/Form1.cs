using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Resources;

namespace Overwatch_Counter_Picker_v3
{
    public partial class frmCounter : Form
    {

        List<PictureBox> lstGeneratedPics = new List<PictureBox>();
        List<PictureBox> lstSelcted = new List<PictureBox>();
        List<Hero> lstHeroes = new List<Hero>();

        byte Selected;

        public frmCounter()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            vGetHeroes();
            vGenerateHeroPictures();
        }

        private void vGenerateHeroPictures()
        {
            //Generates some Flowlayoutpanels, in which pictureboxes are added, labels are also generated
            //specifying the Group

            Point LocationGroup = new Point(5, 25);

            for (int i = 0; i < 5; i++)
            {      
                //Generate flp
                FlowLayoutPanel flp = new FlowLayoutPanel();
                flp.Location = LocationGroup;
                flp.Height = 80;
                flp.Width = 0;
                flp.BorderStyle = BorderStyle.FixedSingle;
                LocationGroup.Y += flp.Height + 5;

                //Generate Label
                Label lbl = new Label();
                lbl.Location = flp.Location;
                lbl.BackColor = Color.Black;
                lbl.ForeColor = Color.White;
                lbl.AutoSize = true;

                this.Controls.Add(lbl);

                //Depending on i, set label text to the group
                switch (i)
                {
                    case 0:
                        lbl.Text = "Offense";
                        break;
                    case 1:
                        lbl.Text = "Defense";
                        break;
                    case 2:
                        lbl.Text = "Tank";
                        break;
                    case 3:
                        lbl.Text = "Support";
                        break;
                    case 4:
                        lbl.Text = "Enemy Team";
                        for (int b = 0; b < 6; b++)
                        {
                            //Add the enemy team pictureboxes
                            PictureBox picSelect = new PictureBox();
                            picSelect.Height = flp.Height;
                            picSelect.Width = flp.Height;
                            picSelect.SizeMode = PictureBoxSizeMode.StretchImage;
                            picSelect.Image = Properties.Resources.Overwatch_circle_logo_svg;
                            picSelect.BorderStyle = BorderStyle.FixedSingle;
                            //Add handler to the pictureboxes
                            picSelect.Click += PicSelect_Click;
                            //Add the pictureboxes to the flp, and change the width
                            flp.Width += flp.Height + 7;
                            flp.Controls.Add(picSelect);

                            lstSelcted.Add(picSelect);
                            lstSelcted[Selected].BorderStyle = BorderStyle.Fixed3D;
                            lstSelcted[Selected].BackColor = Color.Yellow;

                            picSelect.Name = "picSelect" + b;
                        }
                        break;

                    default:
                        break;
                }

                if (i != 4)
                {
                    for (int b = 0; b < lstHeroes.Count; b++)
                    {
                        if (lstHeroes[b].Group == lbl.Text.ToLower())
                        {
                            //Generate new picturebox, and add it to the flp
                            PictureBox picHero = new PictureBox();
                            picHero.Height = flp.Height;
                            picHero.Width = flp.Height;
                            picHero.SizeMode = PictureBoxSizeMode.StretchImage;
                            picHero.Image = lstHeroes[b].Icon;
                            picHero.BorderStyle = BorderStyle.FixedSingle;
                            //Add handler to picturebox
                            picHero.Click += PicHero_Click;

                            flp.Width += flp.Height + 7;
                            flp.Controls.Add(picHero);

                            lstGeneratedPics.Add(picHero);

                            picHero.Name = "pic" + lstHeroes[b].Name;
                        }
                    }
                }
                //If the width of the form is less then the width of the current flp,
                //increase the width of the form
                if (this.Width < flp.Width)
                    this.Width = flp.Width + 25;

                this.Controls.Add(flp);
                flp.Name = "flp" + lbl.Text;
                lbl.Name = "lbl" + lbl.Text;
            }
            //Make it so the form cant be resized
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void PicSelect_Click(object sender, EventArgs e)
        {
            PictureBox ClickedPic = (PictureBox)sender;

            Selected = (byte)lstSelcted.IndexOf(ClickedPic);

            foreach (var Pic in lstSelcted)
            {
                Pic.BorderStyle = BorderStyle.FixedSingle;
                Pic.BackColor = DefaultBackColor;
            }
                
            lstSelcted[Selected].BorderStyle = BorderStyle.Fixed3D;
            lstSelcted[Selected].BackColor = Color.Yellow;
        }

        private void PicHero_Click(object sender, EventArgs e)
        {
            PictureBox ClickedPic = (PictureBox)sender;
            //Set the selected image to the clicked image, and set the border and backcolour to their default
            if (Selected < lstSelcted.Count)
            {
                lstSelcted[Selected].Image = ClickedPic.Image;
                lstSelcted[Selected].BackColor = DefaultBackColor;
                lstSelcted[Selected].BorderStyle = BorderStyle.FixedSingle;
            }
            
            Selected++;
            //Select the next enemy in line
            if (Selected < lstSelcted.Count)
            {
                lstSelcted[Selected].BorderStyle = BorderStyle.Fixed3D;
                lstSelcted[Selected].BackColor = Color.Yellow;
            }

            vCalculateCounter();
        }

        private void vGetHeroes()
        {
            //Go through the resource file of Heroes and their counters, and add them to list
            using (var sr = new System.IO.StringReader(Properties.Resources.CounterPickerHeroes))
            {
                string line;
                byte Counter = 0;

                string Default = string.Empty;

                //Variables used in creation of a new Hero
                string Name = Default;
                string Group = Default;
                string HeroCounters = Default;
                string CounterHeroes = Default;
                Image Icon = Properties.Resources.Overwatch_circle_logo_svg;

                ResourceManager rm = Properties.Resources.ResourceManager;
                //Read each line
                while ((line = sr.ReadLine()) != null)
                {
                    //If the line is = "{" it is the start of a hero, "}" is the end
                    if (line == "{")
                        Counter = 1;
                    else if (line == "}")
                    { //If the end is reached, add new hero to list
                        Counter = 0;

                        string IconName = "Icon_" + Name;
                        Icon = (Image)rm.GetObject(IconName);

                        lstHeroes.Add(new Hero(Name, Group, HeroCounters, CounterHeroes, Icon, 100));

                        Name = Default;
                        Group = Default;
                        HeroCounters = Default;
                        CounterHeroes = Default;
                        Icon = Properties.Resources.Overwatch_circle_logo_svg;
                    }
                    
                    switch (Counter)
                    { //Each hero has 6 lines, the start and end, then the following
                        //Set each variable according to the line number / Counter
                        case 2:
                            Name = line.ToLower();
                            break;
                        case 3:
                            Group = line.ToLower();
                            break;
                        case 4:
                            HeroCounters = line.ToLower();
                            break;
                        case 5:
                            CounterHeroes = line.ToLower();
                            break;

                        default:
                            break;
                    }

                    if (Counter != 0)
                        Counter++;
                }
            }
        }

        private void vCalculateCounter()
        {
            vResetCounters();
            //Go through each picturebox in lstSelected, if the image is the same as the icon of a Hero in lstHeroes
            //Extract the HeroCounters and CounterHeroes to lists, and go through each hero, if their name
            //is on the list, either add or retract X from their counter values
            foreach (var pic in lstSelcted)
                foreach (var hero in lstHeroes)
                    if (pic.Image == hero.Icon)
                    {
                        List<string> lstHeroCounters = (hero.HeroCounters.ToLower()).Split(',').ToList();
                        List<string> lstCounterHeroes = (hero.CounterHeroes.ToLower()).Split(',').ToList();

                        foreach (string name in lstCounterHeroes)
                            foreach (var bHero in lstHeroes)
                                if (name.Trim() == bHero.Name.ToLower())
                                    bHero.CounterValue += 5;

                        foreach (string name in lstHeroCounters)
                            foreach (var bHero in lstHeroes)
                                if (name.Trim() == bHero.Name.ToLower())
                                    bHero.CounterValue -= 5;
                    }

            vSetColours();
        }

        private void vResetCounters()
        {
            //Go through each hero and reset their countervalue back to the deafult of 100
            foreach (var Hero in lstHeroes)
                Hero.CounterValue = 100;
            //Also change their backcolour to the default
            foreach (var pic in lstGeneratedPics)
                pic.BackColor = DefaultBackColor;
        }

        private void vSetColours()
        { 
            //Set the colours of each Heroes back to Green, orangeRed, or Blue depending on their countervalue
            foreach (var pic in lstGeneratedPics)
                foreach (var hero in lstHeroes)
                    if (pic.Image == hero.Icon)
                        if (hero.CounterValue > 100)
                            pic.BackColor = Color.Green;
                        else if (hero.CounterValue < 100)
                            pic.BackColor = Color.OrangeRed;
                        else if (hero.CounterValue == 100)
                            pic.BackColor = Color.Blue;
        }

        private void frmCounter_MouseClick(object sender, MouseEventArgs e)
        {
            vClearSelection();
        }

        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vCalculateCounter();
        }

        private void clearResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Reset everything, and change the selected heroes back to the default image
            vResetCounters();
            foreach (var pic in lstSelcted)
                pic.Image = Properties.Resources.Overwatch_circle_logo_svg;

            vClearSelection();
        }

        private void vClearSelection()
        {
            //Set the selcted backcolour to the default, along with the borderstyle, and set selected to 6
            if (Selected < lstSelcted.Count)
            {
                lstSelcted[Selected].BackColor = DefaultBackColor;
                lstSelcted[Selected].BorderStyle = BorderStyle.FixedSingle;
                Selected = 6;
            }      
        }
    }
}