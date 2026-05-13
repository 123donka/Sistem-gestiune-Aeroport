using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private const string PlaceholderNume = "Nume complet";
        private const string PlaceholderUser = "Nume utilizator";
        private const string PlaceholderPass = "Parolă";

        public RegisterForm()
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
            Text = "Înregistrare - Airport Management";
            Width = 920; Height = 760; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);
            DoubleBuffered = true;

            this.Paint += (s, e) =>
            {
                using var brush = new LinearGradientBrush(ClientRectangle, ColorTranslator.FromHtml("#0F172A"), ColorTranslator.FromHtml("#1E3A8A"), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, ClientRectangle);
            };

            var cardWidth = 520; var cardHeight = 560;

            // soft shadow panel
            var shadow = new Panel { Width = cardWidth + 40, Height = cardHeight + 40, BackColor = Color.Transparent };
            shadow.Anchor = AnchorStyles.None;
            shadow.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                for (int i = 0; i < 10; i++)
                {
                    int alpha = 36 - i * 3;
                    if (alpha <= 0) break;
                    using var brush = new SolidBrush(Color.FromArgb(alpha, 8, 20, 48));
                    var r = new Rectangle(i * 2, i * 2, shadow.Width - i * 4, shadow.Height - i * 4);
                    e.Graphics.FillPath(brush, GetRoundedPath(r, 20));
                }
            };
            Controls.Add(shadow);

            var card = new Panel { Width = cardWidth, Height = cardHeight, BackColor = Color.White };
            card.Anchor = AnchorStyles.None;
            card.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using var p = new Pen(ColorTranslator.FromHtml("#E6E9EE"), 1) { LineJoin = LineJoin.Round }; e.Graphics.DrawPath(p, GetRoundedPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14)); };
            card.Region = new Region(GetRoundedPath(new Rectangle(0, 0, card.Width, card.Height), 14));
            Controls.Add(card);

            var inpDefaultBorder = ColorTranslator.FromHtml("#E5E7EB");

            var lblTitle = new Label
            {
                Text = "Înregistrare",
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

            // Nume input
            var inpNume = new Panel { Left = 40, Top = 96, Width = card.Width - 80, Height = 44, BackColor = Color.White, Tag = inpDefaultBorder };
            inpNume.Paint += (s, e) => { var p = (Panel)s; e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; var clr = p.Tag is Color c ? c : inpDefaultBorder; using var pb = new Pen(clr, 1.5f) { LineJoin = LineJoin.Round }; e.Graphics.DrawPath(pb, GetRoundedPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 8)); };
            inpNume.Region = new Region(GetRoundedPath(new Rectangle(0, 0, inpNume.Width, inpNume.Height), 8));
            txtNume = new TextBox { Left = 12, Top = 8, Width = inpNume.Width - 24, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
            txtNume.Text = PlaceholderNume; txtNume.ForeColor = Color.Gray;
            txtNume.GotFocus += (s, e) => { if (txtNume.Text == PlaceholderNume) { txtNume.Text = ""; txtNume.ForeColor = Color.Black; } var p = txtNume.Parent as Panel; p.Tag = ColorTranslator.FromHtml("#93C5FD"); p.Invalidate(); };
            txtNume.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtNume.Text)) { txtNume.Text = PlaceholderNume; txtNume.ForeColor = Color.Gray; } var p = txtNume.Parent as Panel; p.Tag = inpDefaultBorder; p.Invalidate(); };
            inpNume.Controls.Add(txtNume);
            card.Controls.Add(inpNume);

            // Username input
            var inpUser = new Panel { Left = 40, Top = 156, Width = card.Width - 80, Height = 44, BackColor = Color.White, Tag = inpDefaultBorder };
            inpUser.Paint += (s, e) => { var p = (Panel)s; e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; var clr = p.Tag is Color c ? c : inpDefaultBorder; using var pb = new Pen(clr, 1.5f) { LineJoin = LineJoin.Round }; e.Graphics.DrawPath(pb, GetRoundedPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 8)); };
            inpUser.Region = new Region(GetRoundedPath(new Rectangle(0, 0, inpUser.Width, inpUser.Height), 8));
            txtUsername = new TextBox { Left = 12, Top = 8, Width = inpUser.Width - 24, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
            txtUsername.Text = PlaceholderUser; txtUsername.ForeColor = Color.Gray;
            txtUsername.GotFocus += (s, e) => { if (txtUsername.Text == PlaceholderUser) { txtUsername.Text = ""; txtUsername.ForeColor = Color.Black; } var p = txtUsername.Parent as Panel; p.Tag = ColorTranslator.FromHtml("#93C5FD"); p.Invalidate(); };
            txtUsername.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtUsername.Text)) { txtUsername.Text = PlaceholderUser; txtUsername.ForeColor = Color.Gray; } var p = txtUsername.Parent as Panel; p.Tag = inpDefaultBorder; p.Invalidate(); };
            inpUser.Controls.Add(txtUsername);
            card.Controls.Add(inpUser);

            // Parola
            var inpParola = new Panel { Left = 40, Top = 216, Width = card.Width - 80, Height = 44, BackColor = Color.White, Tag = inpDefaultBorder };
            inpParola.Paint += (s, e) => { var p = (Panel)s; e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; var clr = p.Tag is Color c ? c : inpDefaultBorder; using var pb = new Pen(clr, 1.5f) { LineJoin = LineJoin.Round }; e.Graphics.DrawPath(pb, GetRoundedPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 8)); };
            inpParola.Region = new Region(GetRoundedPath(new Rectangle(0, 0, inpParola.Width, inpParola.Height), 8));
            txtParola = new TextBox { Left = 12, Top = 8, Width = inpParola.Width - 24 - 36, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
            txtParola.Text = PlaceholderPass; txtParola.ForeColor = Color.Gray; txtParola.UseSystemPasswordChar = false;
            txtParola.GotFocus += (s, e) => { if (txtParola.Text == PlaceholderPass) { txtParola.Text = ""; txtParola.ForeColor = Color.Black; txtParola.UseSystemPasswordChar = true; } var p = txtParola.Parent as Panel; p.Tag = ColorTranslator.FromHtml("#93C5FD"); p.Invalidate(); };
            txtParola.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtParola.Text)) { txtParola.Text = PlaceholderPass; txtParola.ForeColor = Color.Gray; txtParola.UseSystemPasswordChar = false; } var p = txtParola.Parent as Panel; p.Tag = inpDefaultBorder; p.Invalidate(); };
            inpParola.Controls.Add(txtParola);

            var chkShowPassword = new CheckBox { Text = "", Left = inpParola.Width - 36, Top = 8, Width = 28, Height = 24, Appearance = Appearance.Button, FlatStyle = FlatStyle.Flat };
            chkShowPassword.FlatAppearance.BorderSize = 0;
            chkShowPassword.CheckedChanged += (s, e) => { if (txtParola.Text != PlaceholderPass) txtParola.UseSystemPasswordChar = !chkShowPassword.Checked; };
            inpParola.Controls.Add(chkShowPassword);
            card.Controls.Add(inpParola);

            // Confirm
            var inpConfirm = new Panel { Left = 40, Top = 276, Width = card.Width - 80, Height = 44, BackColor = Color.White, Tag = inpDefaultBorder };
            inpConfirm.Paint += (s, e) => { var p = (Panel)s; e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; var clr = p.Tag is Color c ? c : inpDefaultBorder; using var pb = new Pen(clr, 1.5f) { LineJoin = LineJoin.Round }; e.Graphics.DrawPath(pb, GetRoundedPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 8)); };
            inpConfirm.Region = new Region(GetRoundedPath(new Rectangle(0, 0, inpConfirm.Width, inpConfirm.Height), 8));
            txtConfirm = new TextBox { Left = 12, Top = 8, Width = inpConfirm.Width - 24 - 36, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
            txtConfirm.Text = PlaceholderPass; txtConfirm.ForeColor = Color.Gray; txtConfirm.UseSystemPasswordChar = false;
            txtConfirm.GotFocus += (s, e) => { if (txtConfirm.Text == PlaceholderPass) { txtConfirm.Text = ""; txtConfirm.ForeColor = Color.Black; txtConfirm.UseSystemPasswordChar = true; } var p = txtConfirm.Parent as Panel; p.Tag = ColorTranslator.FromHtml("#93C5FD"); p.Invalidate(); };
            txtConfirm.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtConfirm.Text)) { txtConfirm.Text = PlaceholderPass; txtConfirm.ForeColor = Color.Gray; txtConfirm.UseSystemPasswordChar = false; } var p = txtConfirm.Parent as Panel; p.Tag = inpDefaultBorder; p.Invalidate(); };
            inpConfirm.Controls.Add(txtConfirm);

            var chkShowConfirm = new CheckBox { Text = "", Left = inpConfirm.Width - 36, Top = 8, Width = 28, Height = 24, Appearance = Appearance.Button, FlatStyle = FlatStyle.Flat };
            chkShowConfirm.FlatAppearance.BorderSize = 0;
            chkShowConfirm.CheckedChanged += (s, e) => { if (txtConfirm.Text != PlaceholderPass) txtConfirm.UseSystemPasswordChar = !chkShowConfirm.Checked; };
            inpConfirm.Controls.Add(chkShowConfirm);
            card.Controls.Add(inpConfirm);

            // Rol
            cmbRol = new ComboBox { Left = 40, Top = 336, Width = card.Width - 80, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cmbRol.Items.AddRange(new[] { "admin", "operator" }); cmbRol.SelectedIndex = 1;
            card.Controls.Add(cmbRol);

            var btnRegister = new Button { Text = "Înregistrează", Left = 40, Top = 396, Width = card.Width - 80, Height = 46, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRegister.FlatAppearance.BorderSize = 0; btnRegister.Font = new Font("Segoe UI", 10, FontStyle.Bold); btnRegister.Click += BtnRegister_Click;
            btnRegister.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnRegister.Width, btnRegister.Height), 12));
            btnRegister.Tag = btnRegister.Text; btnRegister.Text = string.Empty;
            btnRegister.Paint += (s, e) =>
            {
                var b = (Button)s;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new LinearGradientBrush(new Rectangle(0, 0, b.Width, b.Height), ColorTranslator.FromHtml("#3B82F6"), ColorTranslator.FromHtml("#2563EB"), LinearGradientMode.Vertical);
                e.Graphics.FillPath(brush, GetRoundedPath(new Rectangle(0, 0, b.Width, b.Height), 12));
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var textBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(b.Tag?.ToString() ?? "Înregistrează", b.Font, textBrush, new RectangleF(0, 0, b.Width, b.Height), sf);
            };
            card.Controls.Add(btnRegister);

            lblError = new Label { ForeColor = ColorTranslator.FromHtml("#DC2626"), Left = 40, Top = 458, Width = card.Width - 80, Visible = false, TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.Add(lblError);

            var linkLogin = new LinkLabel { Left = 40, Top = 498, Width = card.Width - 80, Text = "Ai deja cont? Conectează-te", TextAlign = ContentAlignment.MiddleCenter };
            linkLogin.Click += (s, e) => { Close(); };
            card.Controls.Add(linkLogin);

            var ver = new Label { Text = "Versiune aplicație 1.0", Left = 0, Top = card.Height - 28, Width = card.Width, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray };
            card.Controls.Add(ver);

            void CenterCard()
            {
                var centerX = (ClientSize.Width - card.Width) / 2;
                var top = Math.Max(40, (ClientSize.Height - card.Height) / 2);
                var shadowOffset = 16;

                shadow.Left = centerX + shadowOffset - 20;
                shadow.Top = top + shadowOffset - 20;

                card.Left = centerX;
                card.Top = top;

                shadow.SendToBack();
                card.BringToFront();
            }

            this.Load += (s, e) => CenterCard();
            this.Resize += (s, e) => CenterCard();
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            lblError.Visible = false;
            var nume = txtNume.Text == PlaceholderNume ? string.Empty : txtNume.Text.Trim();
            var user = txtUsername.Text == PlaceholderUser ? string.Empty : txtUsername.Text.Trim();
            var pass = txtParola.Text == PlaceholderPass ? string.Empty : txtParola.Text;
            var conf = txtConfirm.Text == PlaceholderPass ? string.Empty : txtConfirm.Text;

            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                lblError.Text = "Toate câmpurile sunt obligatorii."; lblError.Visible = true; return;
            }
            if (pass != conf) { lblError.Text = "Parolele nu coincid."; lblError.Visible = true; return; }

            var u = new Utilizator { Nume = nume, Username = user, Parola = pass, Rol = cmbRol.SelectedItem?.ToString() ?? "operator" };
            var ok = _controller.Register(u);
            if (!ok) { lblError.Text = "Nu s-a putut înregistra (username deja folosit)."; lblError.Visible = true; return; }
            MessageBox.Show("Înregistrare efectuată cu succes.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
