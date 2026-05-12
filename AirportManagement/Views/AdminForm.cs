using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class AdminForm : Form
    {
        private DataGridView dgv;
        private UtilizatoriController _controller = new UtilizatoriController();

        public AdminForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Administrare Utilizatori"; Width = 900; Height = 600; StartPosition = FormStartPosition.CenterParent;
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            var pnlTop = new Panel { Height = 50, Dock = DockStyle.Top };
            var btnAdd = new Button { Text = "Add", Left = 10, Width = 100, Top = 10 };
            var btnEdit = new Button { Text = "Edit", Left = 120, Width = 100, Top = 10 };
            var btnDelete = new Button { Text = "Delete", Left = 230, Width = 100, Top = 10 };
            btnAdd.Click += BtnAdd_Click; btnEdit.Click += BtnEdit_Click; btnDelete.Click += BtnDelete_Click;
            pnlTop.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            Controls.Add(dgv); Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var dt = _controller.GetAll();
            dgv.DataSource = dt;
        }

        private Utilizator? GetSelected()
        {
            if (dgv.CurrentRow == null) return null;
            var row = dgv.CurrentRow;
            return new Utilizator
            {
                Id = Convert.ToInt32(row.Cells["id"].Value),
                Nume = row.Cells["nume"].Value?.ToString() ?? string.Empty,
                Username = row.Cells["username"].Value?.ToString() ?? string.Empty,
                Rol = row.Cells["rol"].Value?.ToString() ?? string.Empty
            };
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new UtilizatorEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var u = dlg.UtilizatorResult!;
                _controller.Create(u);
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var sel = GetSelected(); if (sel == null) return;
            var full = _controller.GetById(sel.Id);
            if (full == null) return;
            var dlg = new UtilizatorEditForm(full);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var u = dlg.UtilizatorResult!;
                _controller.Update(u);
                LoadData();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var sel = GetSelected(); if (sel == null) return;
            if (MessageBox.Show("Ștergi utilizatorul selectat?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _controller.Delete(sel.Id);
                LoadData();
            }
        }
    }
}
