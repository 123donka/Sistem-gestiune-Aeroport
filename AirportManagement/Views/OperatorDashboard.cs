using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class OperatorDashboard : Form
    {
        private Utilizator _user;
        public OperatorDashboard(Utilizator user) { _user = user; InitializeComponent(); }

        private void InitializeComponent()
        {
            Text = "Operator Dashboard"; Width = 1100; Height = 700; StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 9);
            var sidebar = new Panel { Width = 220, Dock = DockStyle.Left, BackColor = ColorTranslator.FromHtml("#2563EB") };
            var btnDashboard = CreateBtn("Dashboard", 20);
            var btnZboruri = CreateBtn("Zboruri", 80);
            var btnPasageri = CreateBtn("Pasageri", 140);
            var btnResurse = CreateBtn("Resurse", 200);
            var btnAlerte = CreateBtn("Alerte", 260);
            var btnRapoarte = CreateBtn("Rapoarte", 320);
            var btnProfil = CreateBtn("Profil", 380);
            sidebar.Controls.AddRange(new Control[] { btnDashboard, btnZboruri, btnPasageri, btnResurse, btnAlerte, btnRapoarte, btnProfil });
            Controls.Add(sidebar);

            var header = new Panel { Height = 60, Dock = DockStyle.Top, BackColor = Color.White };
            var lblUser = new Label { Text = _user.Nume, Dock = DockStyle.Right, Padding = new Padding(0, 18, 20, 0) };
            header.Controls.Add(lblUser);
            Controls.Add(header);

            var main = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F8F9FA") };
            var welcome = new Label { Text = "Bun venit, " + _user.Nume, Left = 260, Top = 80, Font = new Font("Segoe UI", 14, FontStyle.Bold) };
            main.Controls.Add(welcome);
            Controls.Add(main);

            btnZboruri.Click += (s, e) => { var f = new ZboruriForm(); f.ShowDialog(); };
            btnPasageri.Click += (s, e) => { var f = new PasageriForm(); f.ShowDialog(); };
            btnResurse.Click += (s, e) => { var f = new ResurseForm(); f.ShowDialog(); };
            btnAlerte.Click += (s, e) => { var f = new AlerteForm(); f.ShowDialog(); };
            btnRapoarte.Click += (s, e) => { var f = new RapoarteForm(); f.ShowDialog(); };
            btnProfil.Click += (s, e) => { var f = new ProfilForm(_user); f.ShowDialog(); };
        }

        private Button CreateBtn(string text, int top)
        {
            return new Button { Text = text, Left = 0, Top = top, Width = 220, Height = 48, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = ColorTranslator.FromHtml("#2563EB") };
        }
    }
}
