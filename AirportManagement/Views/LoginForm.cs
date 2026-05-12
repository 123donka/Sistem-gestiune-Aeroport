using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtParola;
        private Button btnConectare;
        private Label lblError;

        private AuthController authController = new AuthController();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Airport Management - Login";
            Width = 480;
            Height = 640;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9);
            BackColor = ColorTranslator.FromHtml("#F8F9FA");

            var card = new Panel { Width = 360, Height = 420, BackColor = Color.White };
            card.Left = (ClientSize.Width - card.Width) / 2;
            card.Top = 60;
            card.Anchor = AnchorStyles.None;
            card.BorderStyle = BorderStyle.None;

            var picLogo = new PictureBox { Width = 80, Height = 80, Top = 20, Left = (card.Width - 80) / 2, SizeMode = PictureBoxSizeMode.Zoom };
            try { picLogo.ImageLocation = System.IO.Path.Combine(AppContext.BaseDirectory, "Resources", "logo.png"); } catch { }
            card.Controls.Add(picLogo);

            var lblUsername = new Label { Text = "Username", Left = 30, Top = 120, Width = 300 };
            txtUsername = new TextBox { Left = 30, Top = 140, Width = 300, Height = 30 };
            var lblParola = new Label { Text = "Parola", Left = 30, Top = 190, Width = 300 };
            txtParola = new TextBox { Left = 30, Top = 210, Width = 300, Height = 30, UseSystemPasswordChar = true };

            btnConectare = new Button { Text = "Conectare", Left = 30, Top = 260, Width = 300, Height = 40, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConectare.FlatAppearance.BorderSize = 0;
            btnConectare.Click += BtnConectare_Click;

            lblError = new Label { ForeColor = ColorTranslator.FromHtml("#DC2626"), Left = 30, Top = 310, Width = 300, Visible = false };

            var lnkRegister = new LinkLabel { Left = 30, Top = 340, Width = 300, Text = "Nu ai cont? Înregistrează-te" };
            lnkRegister.Click += (s, e) => { var reg = new RegisterForm(); reg.ShowDialog(); };

            card.Controls.Add(lblUsername);
            card.Controls.Add(txtUsername);
            card.Controls.Add(lblParola);
            card.Controls.Add(txtParola);
            card.Controls.Add(btnConectare);
            card.Controls.Add(lblError);
            card.Controls.Add(lnkRegister);

            Controls.Add(card);
        }

        private void BtnConectare_Click(object? sender, EventArgs e)
        {
            lblError.Visible = false;
            var username = txtUsername.Text.Trim();
            var parola = txtParola.Text;

            var user = authController.Login(username, parola);
            if (user == null)
            {
                lblError.Text = "Date de autentificare incorecte.";
                lblError.Visible = true;
                return;
            }

            Hide();
            if (user.Rol?.ToLower() == "admin")
            {
                var admin = new AdminDashboard(user);
                admin.FormClosed += (s, e) => this.Show();
                admin.Show();
            }
            else
            {
                var op = new OperatorDashboard(user);
                op.FormClosed += (s, e) => this.Show();
                op.Show();
            }
        }
    }
}
