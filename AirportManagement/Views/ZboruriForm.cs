using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ZboruriForm : Form
    {
        private DataGridView dgv;
        private ZboruriController _controller = new ZboruriController();

        public ZboruriForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Zboruri"; Width = 900; Height = 600; StartPosition = FormStartPosition.CenterParent;
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

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

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new ZborEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var z = dlg.ZborResult!;
                _controller.Create(z);
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = dgv.CurrentRow;
            var z = new Zbor
            {
                Id = Convert.ToInt32(row.Cells["id"].Value),
                Cod = row.Cells["cod"].Value?.ToString() ?? string.Empty,
                Sursa = row.Cells["sursa"].Value?.ToString() ?? string.Empty,
                Destinatie = row.Cells["destinatie"].Value?.ToString() ?? string.Empty,
                Plecare = Convert.ToDateTime(row.Cells["plecare"].Value),
                Sosire = Convert.ToDateTime(row.Cells["sosire"].Value),
                Status = row.Cells["status"].Value?.ToString() ?? string.Empty
            };

            var dlg = new ZborEditForm(z);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var updated = dlg.ZborResult!;
                _controller.Update(updated);
                LoadData();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            _controller.Delete(id);
            LoadData();
        }
    }
}
