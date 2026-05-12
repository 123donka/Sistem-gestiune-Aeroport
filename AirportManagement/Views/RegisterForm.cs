using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;
using AirportManagement.Utils;

namespace AirportManagement.Views
{
    public class RegisterForm : Form
    {
        private TextBox txtNume, txtUsername, txtParola, txtConfirm;
        private ComboBox cmbRol;
        private Label lblError;
        private AuthController _controller = new AuthController();

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Înregistrare - Airport Management";
            Width = 480; Height = 700; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);

            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F8F9FA") };

            var card = new Panel { Width = 380, Height = 480, BackColor = Color.White, Anchor = AnchorStyles.None };
            card.Top = 40;

            var lblNume = new Label { Text = "Nume", Left = 20, Top = 20 };
            txtNume = new TextBox { Left = 20, Top = 40, Width = 340 };
            var lblUser = new Label { Text = "Username", Left = 20, Top = 80 };
            txtUsername = new TextBox { Left = 20, Top = 100, Width = 340 };
            var lblParola = new Label { Text = "Parola", Left = 20, Top = 140 };
            txtParola = new TextBox { Left = 20, Top = 160, Width = 340, UseSystemPasswordChar = true };
            var lblConfirm = new Label { Text = "Confirmare Parola", Left = 20, Top = 200 };
            txtConfirm = new TextBox { Left = 20, Top = 220, Width = 340, UseSystemPasswordChar = true };
            var lblRol = new Label { Text = "Rol", Left = 20, Top = 260 };
            cmbRol = new ComboBox { Left = 20, Top = 280, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.AddRange(new[] { "admin", "operator" }); cmbRol.SelectedIndex = 1;

            var btnRegister = new Button { Text = "Înregistrează", Left = 20, Top = 330, Width = 340, Height = 40, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRegister.FlatAppearance.BorderSize = 0; btnRegister.Click += BtnRegister_Click;

            lblError = new Label { ForeColor = ColorTranslator.FromHtml("#DC2626"), Left = 20, Top = 380, Width = 340, Visible = false };

            card.Controls.AddRange(new Control[] { lblNume, txtNume, lblUser, txtUsername, lblParola, txtParola, lblConfirm, txtConfirm, lblRol, cmbRol, btnRegister, lblError });
            pnl.Controls.Add(card);

            // center card horizontally and vertically (shifted slightly up so buttons are visible)
            void CenterCard()
            {
                var left = Math.Max(10, (pnl.ClientSize.Width - card.Width) / 2);
                var top = (pnl.ClientSize.Height - card.Height) / 2 - 40; // shift up 40px
                if (top < 10) top = 10;
                card.Left = left;
                card.Top = top;
            }

            pnl.Resize += (s, e) => CenterCard();
            this.Load += (s, e) => CenterCard();

            Controls.Add(pnl);
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            lblError.Visible = false;
            if (string.IsNullOrWhiteSpace(txtNume.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtParola.Text))
            {
                lblError.Text = "Toate câmpurile sunt obligatorii."; lblError.Visible = true; return;
            }
            if (txtParola.Text != txtConfirm.Text) { lblError.Text = "Parolele nu coincid."; lblError.Visible = true; return; }

            var u = new Utilizator { Nume = txtNume.Text.Trim(), Username = txtUsername.Text.Trim(), Parola = txtParola.Text, Rol = cmbRol.SelectedItem?.ToString() ?? "operator" };
            var ok = _controller.Register(u);
            if (!ok) { lblError.Text = "Nu s-a putut înregistra (username deja folosit)."; lblError.Visible = true; return; }
            MessageBox.Show("Înregistrare efectuată cu succes.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
