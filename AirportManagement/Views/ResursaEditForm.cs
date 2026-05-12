using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ResursaEditForm : Form
    {
        private TextBox txtNume, txtTip;
        private CheckBox chkDisponibila;
        private Button btnOk, btnCancel;
        private Resursa? _edit;
        public Resursa? ResursaResult { get; private set; }

        public ResursaEditForm(Resursa? r = null)
        {
            _edit = r;
            InitializeComponent();
            if (r != null) Populate(r);
        }

        private void InitializeComponent()
        {
            Text = _edit == null ? "Adaugă resursă" : "Editează resursă";
            Width = 420; Height = 260; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);

            var lblNume = new Label { Text = "Nume", Left = 20, Top = 20 };
            txtNume = new TextBox { Left = 20, Top = 40, Width = 360 };

            var lblTip = new Label { Text = "Tip", Left = 20, Top = 80 };
            txtTip = new TextBox { Left = 20, Top = 100, Width = 360 };

            chkDisponibila = new CheckBox { Text = "Disponibilă", Left = 20, Top = 140 };

            btnOk = new Button { Text = "OK", Left = 80, Top = 180, Width = 120, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0; btnOk.Click += BtnOk_Click;
            btnCancel = new Button { Text = "Cancel", Left = 220, Top = 180, Width = 120 }; btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblNume, txtNume, lblTip, txtTip, chkDisponibila, btnOk, btnCancel });
        }

        private void Populate(Resursa r)
        {
            txtNume.Text = r.Nume;
            txtTip.Text = r.Tip;
            chkDisponibila.Checked = r.Disponibila;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text)) { MessageBox.Show("Nume obligatoriu"); return; }
            ResursaResult = new Resursa
            {
                Id = _edit?.Id ?? 0,
                Nume = txtNume.Text.Trim(),
                Tip = txtTip.Text.Trim(),
                Disponibila = chkDisponibila.Checked
            };
            DialogResult = DialogResult.OK;
        }
    }
}
