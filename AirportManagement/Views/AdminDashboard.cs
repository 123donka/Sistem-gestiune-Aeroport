using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class AdminDashboard : Form
    {
        private Utilizator _user;
        private AdminForm _adminForm;
        public AdminDashboard(Utilizator user) { _user = user; InitializeComponent(); }

        private void InitializeComponent()
        {
            Text = "Admin Dashboard"; Width = 1200; Height = 800; StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 9);

            var sidebar = new Panel { Width = 220, Dock = DockStyle.Left, BackColor = ColorTranslator.FromHtml("#2563EB") };
            var btnDashboard = CreateBtn("Dashboard", 20);
            var btnZboruri = CreateBtn("Zboruri", 80);
            var btnPasageri = CreateBtn("Pasageri", 140);
            var btnResurse = CreateBtn("Resurse", 200);
            var btnAlerte = CreateBtn("Alerte", 260);
            var btnRapoarte = CreateBtn("Rapoarte", 320);
            var btnProfil = CreateBtn("Profil", 380);
            var btnAdmin = CreateBtn("Administrare Utilizatori", 440);
            sidebar.Controls.AddRange(new Control[] { btnDashboard, btnZboruri, btnPasageri, btnResurse, btnAlerte, btnRapoarte, btnProfil, btnAdmin });
            Controls.Add(sidebar);


            // Top bar (matches mockup style)
            var header = new Panel { Height = 64, Dock = DockStyle.Top, BackColor = ColorTranslator.FromHtml("#1E3A8A") };
            var lblTitle = new Label { Text = "Aeroport Management", Left = 20, Top = 18, AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White };
            header.Controls.Add(lblTitle);

            var rightFlow = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Padding = new Padding(0, 18, 20, 0), BackColor = Color.Transparent };
            var btnLogout = new Button { Text = "Deconectare", AutoSize = true, Height = 28, FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.White }; btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => { this.Close(); };
            var sep = new Label { Text = "|", ForeColor = Color.White, AutoSize = true, Padding = new Padding(8, 12, 8, 0) };
            var lblRole = new Label { Text = _user.Rol ?? string.Empty, ForeColor = Color.White, AutoSize = true, Padding = new Padding(8, 12, 0, 0) };
            var lblUser = new Label { Text = _user.Nume, ForeColor = Color.White, AutoSize = true, Padding = new Padding(8, 12, 0, 0) };
            rightFlow.Controls.Add(btnLogout);
            rightFlow.Controls.Add(sep);
            rightFlow.Controls.Add(lblRole);
            rightFlow.Controls.Add(new Label { Text = "|", ForeColor = Color.White, AutoSize = true, Padding = new Padding(8, 12, 8, 0) });
            rightFlow.Controls.Add(lblUser);
            header.Controls.Add(rightFlow);

            Controls.Add(header);

            var main = new Panel { Dock = DockStyle.Fill, BackColor = ColorTranslator.FromHtml("#F3F4F6"), Padding = new Padding(24) };
            Controls.Add(main);

            // White rounded card centered in the main area
            var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            card.Padding = new Padding(16);
            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using var p = new Pen(ColorTranslator.FromHtml("#E6E9EE"), 1);
                e.Graphics.DrawPath(p, GetRoundedPath(rect, 10));
                card.Region = new Region(GetRoundedPath(rect, 10));
            };

            // Title inside card
            var cardTitle = new Label { Text = "Administrare", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#0F172A"), AutoSize = true, Left = 12, Top = 12 };
            card.Controls.Add(cardTitle);

            // area to host the admin management UI
            var contentHost = new Panel { Left = 12, Top = 56, Dock = DockStyle.Fill, BackColor = Color.White };
            card.Controls.Add(contentHost);

            // Embed AdminForm into the content host so its tabs and table appear inside the card
            _adminForm = new AdminForm(_user);
            _adminForm.TopLevel = false;
            _adminForm.FormBorderStyle = FormBorderStyle.None;
            _adminForm.Dock = DockStyle.Fill;
            contentHost.Controls.Add(_adminForm);
            _adminForm.SetEmbedded(true);
            _adminForm.Show();

            btnZboruri.Click += (s, e) => { var f = new ZboruriForm(); f.ShowDialog(); };
            btnPasageri.Click += (s, e) => { var f = new PasageriForm(); f.ShowDialog(); };
            btnResurse.Click += (s, e) => { var f = new ResurseForm(); f.ShowDialog(); };
            btnAlerte.Click += (s, e) => { var f = new AlerteForm(); f.ShowDialog(); };
            btnRapoarte.Click += (s, e) => { var f = new RapoarteForm(); f.ShowDialog(); };
            btnProfil.Click += (s, e) => { var f = new ProfilForm(_user); f.ShowDialog(); };
            btnAdmin.Click += (s, e) => { _adminForm.BringToFront(); };
        }

        private Button CreateBtn(string text, int top)
        {
            return new Button { Text = text, Left = 0, Top = top, Width = 220, Height = 48, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = ColorTranslator.FromHtml("#2563EB") };
        }

        private GraphicsPath GetRoundedPath(Rectangle r, int radius)
        {
            var d = radius * 2;
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
