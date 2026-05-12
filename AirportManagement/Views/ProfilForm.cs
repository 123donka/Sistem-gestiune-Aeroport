using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ProfilForm : Form
    {
        private Utilizator _user;
        private TextBox txtNume, txtUsername, txtParola;
        private Label lblRol;
        private Button btnSave;
        private UtilizatoriController _controller = new UtilizatoriController();

        public ProfilForm(Utilizator user)
        {
            _user = user;
            InitializeComponent();
            Populate();
        }

        private void InitializeComponent()
        {
            Text = "Profil"; Width = 500; Height = 360; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);

            var lblNume = new Label { Text = "Nume", Left = 20, Top = 20 };
            txtNume = new TextBox { Left = 20, Top = 40, Width = 420 };

            var lblUsername = new Label { Text = "Username", Left = 20, Top = 80 };
            txtUsername = new TextBox { Left = 20, Top = 100, Width = 420 };

            var lblParola = new Label { Text = "Schimbă parola (lăsați gol pentru a păstra)", Left = 20, Top = 140 };
            txtParola = new TextBox { Left = 20, Top = 160, Width = 420, UseSystemPasswordChar = true };

            var lblRolTitle = new Label { Text = "Rol", Left = 20, Top = 200 };
            lblRol = new Label { Left = 20, Top = 220, Width = 420 };

            btnSave = new Button { Text = "Salvează", Left = 160, Top = 260, Width = 160, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] { lblNume, txtNume, lblUsername, txtUsername, lblParola, txtParola, lblRolTitle, lblRol, btnSave });
        }

        private void Populate()
        {
            txtNume.Text = _user.Nume;
            txtUsername.Text = _user.Username;
            lblRol.Text = _user.Rol;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            _user.Nume = txtNume.Text.Trim();
            _user.Username = txtUsername.Text.Trim();
            _user.Parola = txtParola.Text; // empty means no change in service
            var ok = _controller.Update(_user);
            if (ok) MessageBox.Show("Profil actualizat."); else MessageBox.Show("Eroare la actualizare.");
            DialogResult = ok ? DialogResult.OK : DialogResult.None;
        }
    }
}
