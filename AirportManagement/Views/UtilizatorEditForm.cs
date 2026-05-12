using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class UtilizatorEditForm : Form
    {
        private TextBox txtNume, txtUsername, txtParola;
        private ComboBox cmbRol;
        private Button btnOk, btnCancel;
        private Utilizator? _edit;
        public Utilizator? UtilizatorResult { get; private set; }

        public UtilizatorEditForm(Utilizator? u = null)
        {
            _edit = u;
            InitializeComponent();
            if (u != null) Populate(u);
        }

        private void InitializeComponent()
        {
            Text = _edit == null ? "Adaugă utilizator" : "Editează utilizator";
            Width = 480; Height = 340; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);

            var lblNume = new Label { Text = "Nume", Left = 20, Top = 20 };
            txtNume = new TextBox { Left = 20, Top = 40, Width = 420 };

            var lblUsername = new Label { Text = "Username", Left = 20, Top = 80 };
            txtUsername = new TextBox { Left = 20, Top = 100, Width = 420 };

            var lblParola = new Label { Text = "Parola (lăsați gol pentru a nu schimba)", Left = 20, Top = 140 };
            txtParola = new TextBox { Left = 20, Top = 160, Width = 420, UseSystemPasswordChar = true };

            var lblRol = new Label { Text = "Rol", Left = 20, Top = 200 };
            cmbRol = new ComboBox { Left = 20, Top = 220, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.AddRange(new object[] { "admin", "operator" });
            cmbRol.SelectedIndex = 1;

            btnOk = new Button { Text = "OK", Left = 80, Top = 260, Width = 120, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0; btnOk.Click += BtnOk_Click;
            btnCancel = new Button { Text = "Cancel", Left = 220, Top = 260, Width = 120 }; btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblNume, txtNume, lblUsername, txtUsername, lblParola, txtParola, lblRol, cmbRol, btnOk, btnCancel });
        }

        private void Populate(Utilizator u)
        {
            txtNume.Text = u.Nume;
            txtUsername.Text = u.Username;
            cmbRol.SelectedItem = u.Rol.ToLower();
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text) || string.IsNullOrWhiteSpace(txtUsername.Text)) { MessageBox.Show("Nume și username obligatorii"); return; }
            UtilizatorResult = new Utilizator
            {
                Id = _edit?.Id ?? 0,
                Nume = txtNume.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Parola = txtParola.Text,
                Rol = cmbRol.SelectedItem?.ToString() ?? "operator"
            };
            DialogResult = DialogResult.OK;
        }
    }
}
