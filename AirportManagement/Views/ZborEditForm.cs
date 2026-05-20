using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ZborEditForm : Form
    {
        private (Label Label, TextBox Box) txtNumarZbor;
        private (Label Label, TextBox Box) txtCompanie;
        private (Label Label, TextBox Box) txtOrigine;
        private (Label Label, TextBox Box) txtDestinatie;
        private (Label Label, DateTimePicker Box) dtPlecare;
        private (Label Label, DateTimePicker Box) dtAterizare;
        private ComboBox cmbStatus = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        private readonly Zbor? _edit;
        public Zbor? ZborResult { get; private set; }

        public ZborEditForm(Zbor? z = null)
        {
            _edit = z;
            InitializeComponent();
            if (z != null) Populate(z);
        }

        private void InitializeComponent()
        {
            Text = _edit == null ? "Adauga zbor" : "Editeaza zbor";
            Width = 620;
            Height = 560;
            MinimumSize = new Size(620, 560);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);
            BackColor = ColorTranslator.FromHtml("#F3F6FB");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var card = new Panel
            {
                Left = 18,
                Top = 18,
                Width = 570,
                Height = 430,
                BackColor = Color.White,
                Padding = new Padding(18)
            };
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using var path = RoundedRect(rect, 12);
                using var border = new Pen(ColorTranslator.FromHtml("#E5E7EB"), 1);
                e.Graphics.DrawPath(border, path);
                card.Region = new Region(path);
            };

            var title = new Label
            {
                Text = _edit == null ? "Adauga zbor" : "Editeaza zbor",
                Left = 22,
                Top = 14,
                AutoSize = true,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };

            var subtitle = new Label
            {
                Text = "Completeaza doar campurile esentiale. Restul este generat automat pentru schema aplicatiei.",
                Left = 24,
                Top = 46,
                Width = 500,
                Height = 34,
                Font = new Font("Segoe UI", 9F),
                ForeColor = ColorTranslator.FromHtml("#64748B")
            };

            txtNumarZbor = CreateBox("Numar zbor", 24, 88, 240);
            txtCompanie = CreateBox("Companie aeriana", 286, 88, 240);
            txtOrigine = CreateBox("Origine", 24, 164, 240);
            txtDestinatie = CreateBox("Destinatie", 286, 164, 240);

            dtPlecare = CreateDateBox("Ora decolare", 24, 240, 240);
            dtAterizare = CreateDateBox("Ora aterizare", 286, 240, 240);

            cmbStatus = new ComboBox
            {
                Left = 24,
                Top = 326,
                Width = 502,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            cmbStatus.Items.AddRange(new object[] { "Programat", "Live", "La timp", "Intarziat", "Imbarcare" });
            cmbStatus.SelectedIndex = 0;

            var lblStatus = new Label
            {
                Text = "Status",
                Left = 24,
                Top = 304,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#475569")
            };

            btnSave = new Button
            {
                Text = _edit == null ? "Creeaza" : "Salveaza",
                Left = 274,
                Top = 474,
                Width = 150,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Renunta",
                Left = 436,
                Top = 474,
                Width = 150,
                Height = 40,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#334155"),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#CBD5E1");
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.Add(title);
            Controls.Add(subtitle);
            Controls.Add(card);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            card.Controls.Add(txtNumarZbor.Label);
            card.Controls.Add(txtNumarZbor.Box);
            card.Controls.Add(txtCompanie.Label);
            card.Controls.Add(txtCompanie.Box);
            card.Controls.Add(txtOrigine.Label);
            card.Controls.Add(txtOrigine.Box);
            card.Controls.Add(txtDestinatie.Label);
            card.Controls.Add(txtDestinatie.Box);
            card.Controls.Add(dtPlecare.Label);
            card.Controls.Add(dtPlecare.Box);
            card.Controls.Add(dtAterizare.Label);
            card.Controls.Add(dtAterizare.Box);
            card.Controls.Add(lblStatus);
            card.Controls.Add(cmbStatus);

            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private void Populate(Zbor z)
        {
            txtNumarZbor.Box.Text = z.Cod;
            txtCompanie.Box.Text = z.CompanieAeriana;
            txtOrigine.Box.Text = z.Sursa;
            txtDestinatie.Box.Text = z.Destinatie;
            dtPlecare.Box.Value = z.Plecare == default ? DateTime.Now : z.Plecare;
            dtAterizare.Box.Value = z.Sosire == default ? DateTime.Now : z.Sosire;

            var idx = cmbStatus.Items.IndexOf(string.IsNullOrWhiteSpace(z.Status) ? "Programat" : z.Status);
            cmbStatus.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumarZbor.Box.Text))
            {
                MessageBox.Show("Numarul zborului este obligatoriu.", "Zbor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCompanie.Box.Text))
            {
                MessageBox.Show("Compania aeriana este obligatorie.", "Zbor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOrigine.Box.Text) || string.IsNullOrWhiteSpace(txtDestinatie.Box.Text))
            {
                MessageBox.Show("Originea si destinatia sunt obligatorii.", "Zbor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ZborResult = new Zbor
            {
                Id = _edit?.Id ?? 0,
                Cod = txtNumarZbor.Box.Text.Trim(),
                CompanieAeriana = txtCompanie.Box.Text.Trim(),
                TipZbor = ResolveTipZbor(txtOrigine.Box.Text.Trim(), txtDestinatie.Box.Text.Trim()),
                Sursa = txtOrigine.Box.Text.Trim(),
                Destinatie = txtDestinatie.Box.Text.Trim(),
                Plecare = dtPlecare.Box.Value,
                Sosire = dtAterizare.Box.Value,
                Status = cmbStatus.SelectedItem?.ToString() ?? "Programat"
            };

            DialogResult = DialogResult.OK;
        }

        private static string ResolveTipZbor(string origine, string destinatie)
        {
            var src = origine.Trim().ToLowerInvariant();
            if (src.Contains("cluj"))
            {
                return "plecare";
            }

            return "sosire";
        }

        private static (Label Label, TextBox Box) CreateBox(string label, int left, int top, int width)
        {
            var lbl = new Label
            {
                Text = label,
                Left = left,
                Top = top,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#475569")
            };

            var box = new TextBox
            {
                Left = left,
                Top = top + 22,
                Width = width,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            return (lbl, box);
        }

        private static (Label Label, DateTimePicker Box) CreateDateBox(string label, int left, int top, int width)
        {
            var lbl = new Label
            {
                Text = label,
                Left = left,
                Top = top,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#475569")
            };

            var box = new DateTimePicker
            {
                Left = left,
                Top = top + 22,
                Width = width,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm",
                Font = new Font("Segoe UI", 10F)
            };

            return (lbl, box);
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
