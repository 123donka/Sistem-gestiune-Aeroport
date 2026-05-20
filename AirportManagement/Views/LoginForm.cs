using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class LoginForm : Form
    {
        public static bool SuppressChildReturn { get; set; }

        private TextBox txtUsername;
        private TextBox txtParola;
        private Button btnConectare;
        private Label lblError;
        private Label lblTitle;
        private CheckBox chkShowPassword;
        private Panel card;

        private AuthController authController = new AuthController();

        private const string PlaceholderUser = "Nume utilizator";
        private const string PlaceholderPass = "Parolă";

        public LoginForm()
        {
            InitializeComponent();
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

        private void InitializeComponent()
        {
            Text = "Airport Management - Login";
            Width = 1000;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9);
            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();

            // draw gradient background in Paint
            this.Paint += LoginForm_Paint;

            var cardWidth = 420;
            var cardHeight = 500;

            // shadow drawing removed (was using transparent Panel which caused repaint artefacts)

            // accent stripes removed per user request

            card = new Panel { Width = cardWidth, Height = cardHeight, BackColor = Color.White };
            card.Anchor = AnchorStyles.None;
            card.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (var p = new Pen(ColorTranslator.FromHtml("#E6E9EE"), 1)) { var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1); e.Graphics.DrawPath(p, GetRoundedPath(rect, 14)); } };
            card.Region = new Region(GetRoundedPath(new Rectangle(0, 0, card.Width, card.Height), 14));
            Controls.Add(card);

            lblTitle = new Label
            {
                Text = "Aeroport Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 24,
                Top = 18,
                Width = card.Width - 48,
                Height = 48,
                ForeColor = ColorTranslator.FromHtml("#0F172A"),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblTitle);

            // logo removed per user request: no icon is shown on the login card

            // default border color for inputs
            var inpDefaultBorder = ColorTranslator.FromHtml("#E5E7EB");

            // Username input container
            var inpUser = new Panel { Left = 40, Top = 130, Width = card.Width - 80, Height = 44, BackColor = Color.White, Tag = inpDefaultBorder };
            inpUser.Paint += (s, e) => {
                var p = (Panel)s;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var clr = p.Tag is Color c ? c : inpDefaultBorder;
                using var pb = new Pen(clr, 1.5f) { LineJoin = LineJoin.Round };
                e.Graphics.DrawPath(pb, GetRoundedPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 8));
            };
            inpUser.Region = new Region(GetRoundedPath(new Rectangle(0, 0, inpUser.Width, inpUser.Height), 8));

            // no icon shown — make full-width text input
            txtUsername = new TextBox { Left = 12, Top = 8, Width = inpUser.Width - 24, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
            txtUsername.Text = PlaceholderUser; txtUsername.ForeColor = Color.Gray;
            txtUsername.GotFocus += (s, e) => { if (txtUsername.Text == PlaceholderUser) { txtUsername.Text = ""; txtUsername.ForeColor = Color.Black; } var p = txtUsername.Parent as Panel; p.Tag = ColorTranslator.FromHtml("#93C5FD"); p.Invalidate(); };
            txtUsername.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtUsername.Text)) { txtUsername.Text = PlaceholderUser; txtUsername.ForeColor = Color.Gray; } var p = txtUsername.Parent as Panel; p.Tag = inpDefaultBorder; p.Invalidate(); };
            inpUser.Controls.Add(txtUsername);
            card.Controls.Add(inpUser);

            // Password input container
            var inpPass = new Panel { Left = 40, Top = 190, Width = card.Width - 80, Height = 44, BackColor = Color.White };
            inpPass.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using var pb = new Pen(ColorTranslator.FromHtml("#E5E7EB")); e.Graphics.DrawPath(pb, GetRoundedPath(new Rectangle(0, 0, inpPass.Width - 1, inpPass.Height - 1), 8)); };
            inpPass.Region = new Region(GetRoundedPath(new Rectangle(0, 0, inpPass.Width, inpPass.Height), 8));

            // no icon shown — make full-width password input
            txtParola = new TextBox { Left = 12, Top = 8, Width = inpPass.Width - 24, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
            txtParola.Text = PlaceholderPass; txtParola.ForeColor = Color.Gray; txtParola.UseSystemPasswordChar = false;
            txtParola.GotFocus += (s, e) => { if (txtParola.Text == PlaceholderPass) { txtParola.Text = ""; txtParola.ForeColor = Color.Black; txtParola.UseSystemPasswordChar = true; } var p = txtParola.Parent as Panel; p.Tag = ColorTranslator.FromHtml("#93C5FD"); p.Invalidate(); };
            txtParola.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtParola.Text)) { txtParola.Text = PlaceholderPass; txtParola.ForeColor = Color.Gray; txtParola.UseSystemPasswordChar = false; } var p = txtParola.Parent as Panel; p.Tag = inpDefaultBorder; p.Invalidate(); };
            inpPass.Controls.Add(txtParola);

            chkShowPassword = new CheckBox { Text = "", Left = inpPass.Width - 36, Top = 12, Width = 24, Height = 20, Appearance = Appearance.Button, FlatStyle = FlatStyle.Flat }; chkShowPassword.FlatAppearance.BorderSize = 0;
            chkShowPassword.CheckedChanged += (s, e) => { if (txtParola.Text != PlaceholderPass) txtParola.UseSystemPasswordChar = !chkShowPassword.Checked; };
            inpPass.Controls.Add(chkShowPassword);

            card.Controls.Add(inpPass);

            btnConectare = new Button { Text = "Conectare", Left = 40, Top = 260, Width = card.Width - 80, Height = 46, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConectare.FlatAppearance.BorderSize = 0; btnConectare.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnConectare.Click += BtnConectare_Click;
            btnConectare.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnConectare.Width, btnConectare.Height), 12));
            btnConectare.Cursor = Cursors.Hand; btnConectare.UseVisualStyleBackColor = false;
            // keep the text in Tag and clear Text so we can custom-paint gradient
            btnConectare.Tag = btnConectare.Text; btnConectare.Text = string.Empty;
            btnConectare.Paint += (s, e) =>
            {
                var b = (Button)s;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new LinearGradientBrush(new Rectangle(0, 0, b.Width, b.Height), ColorTranslator.FromHtml("#3B82F6"), ColorTranslator.FromHtml("#2563EB"), LinearGradientMode.Vertical);
                e.Graphics.FillPath(brush, GetRoundedPath(new Rectangle(0, 0, b.Width, b.Height), 12));
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var textBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(b.Tag?.ToString() ?? "Conectare", b.Font, textBrush, new RectangleF(0, 0, b.Width, b.Height), sf);
            };
            card.Controls.Add(btnConectare);

            lblError = new Label { ForeColor = ColorTranslator.FromHtml("#DC2626"), Left = 40, Top = 320, Width = card.Width - 80, Visible = false, TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.Add(lblError);

            var lnkRegister = new LinkLabel { Left = 40, Top = 350, Width = card.Width - 80, Text = "Nu ai cont? Înregistrează-te", TextAlign = ContentAlignment.MiddleCenter };
            lnkRegister.Click += (s, e) => { var reg = new RegisterForm(); reg.ShowDialog(); };
            card.Controls.Add(lnkRegister);

            var ver = new Label { Text = "Versiune aplicație 1.0", Left = 0, Top = card.Height - 28, Width = card.Width, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray };
            card.Controls.Add(ver);

            // center controls on resize and at load
            void CenterCard()
            {
                var centerX = (ClientSize.Width - card.Width) / 2;
                var top = Math.Max(40, (ClientSize.Height - card.Height) / 2);

                // card sits above the background shadow (shadow now drawn in form Paint)
                card.Left = centerX;
                card.Top = top;
                card.BringToFront();
                Invalidate();
            }

            this.Load += (s, e) => CenterCard();
            this.Resize += (s, e) => CenterCard();
        }

        private void LoginForm_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var brush = new LinearGradientBrush(ClientRectangle, ColorTranslator.FromHtml("#0F172A"), ColorTranslator.FromHtml("#1E3A8A"), LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);

            // draw soft shadow behind centered card (drawn on form to avoid transparent child artifacts)
            if (card != null)
            {
                var cardWidth = card.Width;
                var cardHeight = card.Height;
                var centerX = (ClientSize.Width - cardWidth) / 2;
                var top = Math.Max(40, (ClientSize.Height - cardHeight) / 2);
                var shadowOffset = 16;

                var shadowLeft = centerX + shadowOffset - 20;
                var shadowTop = top + shadowOffset - 20;
                var shadowWidth = cardWidth + 40;
                var shadowHeight = cardHeight + 40;

                for (int i = 0; i < 10; i++)
                {
                    int alpha = 36 - i * 3;
                    if (alpha <= 0) break;
                    using var sbrush = new SolidBrush(Color.FromArgb(alpha, 8, 20, 48));
                    var r = new Rectangle(shadowLeft + i * 2, shadowTop + i * 2, shadowWidth - i * 4, shadowHeight - i * 4);
                    e.Graphics.FillPath(sbrush, GetRoundedPath(r, 20));
                }
            }
        }

        private void BtnConectare_Click(object? sender, EventArgs e)
        {
            lblError.Visible = false;
            var username = txtUsername.Text.Trim();
            var parola = txtParola.Text;

            if (username == PlaceholderUser) username = string.Empty;
            if (parola == PlaceholderPass) parola = string.Empty;

            var user = authController.Login(username, parola);
            if (user == null)
            {
                lblError.Text = "Date de autentificare incorecte.";
                lblError.Visible = true;
                return;
            }

            Hide();
            var role = (user.Rol ?? string.Empty).ToLower();
            if (role.Contains("admin"))
            {
                var adminForm = new AdminForm(user);
                adminForm.FormClosed += (s, e) =>
                {
                    if (!SuppressChildReturn) this.Show();
                };
                adminForm.Show();
            }
            else
            {
                var op = new OperatorDashboard(user);
                op.FormClosed += (s, e) =>
                {
                    if (!SuppressChildReturn) this.Show();
                };
                op.Show();
            }
        }
    }
}
