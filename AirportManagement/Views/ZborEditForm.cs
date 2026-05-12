using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ZborEditForm : Form
    {
        private TextBox txtCod, txtSursa, txtDest;
        private DateTimePicker dtPlecare, dtSosire;
        private TextBox txtStatus;
        private Button btnOk, btnCancel;
        private Zbor? _edit;
        public Zbor? ZborResult { get; private set; }

        public ZborEditForm(Zbor? z = null)
        {
            _edit = z;
            InitializeComponent();
            if (z != null) Populate(z);
        }

        private void InitializeComponent()
        {
            Text = _edit == null ? "Adaugă zbor" : "Editează zbor";
            Width = 480; Height = 420; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);

            var lblCod = new Label { Text = "Cod", Left = 20, Top = 20 };
            txtCod = new TextBox { Left = 20, Top = 40, Width = 420 };

            var lblSursa = new Label { Text = "Sursa", Left = 20, Top = 80 };
            txtSursa = new TextBox { Left = 20, Top = 100, Width = 420 };

            var lblDest = new Label { Text = "Destinație", Left = 20, Top = 140 };
            txtDest = new TextBox { Left = 20, Top = 160, Width = 420 };

            var lblPlecare = new Label { Text = "Plecare", Left = 20, Top = 200 };
            dtPlecare = new DateTimePicker { Left = 20, Top = 220, Width = 420, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

            var lblSosire = new Label { Text = "Sosire", Left = 20, Top = 260 };
            dtSosire = new DateTimePicker { Left = 20, Top = 280, Width = 420, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

            var lblStatus = new Label { Text = "Status", Left = 20, Top = 320 };
            txtStatus = new TextBox { Left = 20, Top = 340, Width = 420 };

            btnOk = new Button { Text = "OK", Left = 80, Top = 380, Width = 120, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0; btnOk.Click += BtnOk_Click;
            btnCancel = new Button { Text = "Cancel", Left = 220, Top = 380, Width = 120 }; btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblCod, txtCod, lblSursa, txtSursa, lblDest, txtDest, lblPlecare, dtPlecare, lblSosire, dtSosire, lblStatus, txtStatus, btnOk, btnCancel });
        }

        private void Populate(Zbor z)
        {
            txtCod.Text = z.Cod;
            txtSursa.Text = z.Sursa;
            txtDest.Text = z.Destinatie;
            dtPlecare.Value = z.Plecare == default ? DateTime.Now : z.Plecare;
            dtSosire.Value = z.Sosire == default ? DateTime.Now : z.Sosire;
            txtStatus.Text = z.Status;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCod.Text)) { MessageBox.Show("Cod zbor obligatoriu"); return; }
            ZborResult = new Zbor
            {
                Id = _edit?.Id ?? 0,
                Cod = txtCod.Text.Trim(),
                Sursa = txtSursa.Text.Trim(),
                Destinatie = txtDest.Text.Trim(),
                Plecare = dtPlecare.Value,
                Sosire = dtSosire.Value,
                Status = txtStatus.Text.Trim()
            };
            DialogResult = DialogResult.OK;
        }
    }
}
