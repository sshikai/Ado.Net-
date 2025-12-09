using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoSqlite
{
    public partial class Form1 : Form
    {
        private List<DAL.User> _list;

        public Form1()
        {
            InitializeComponent();

            _list = new List<DAL.User>();
            bsUser.DataSource = _list;
            dataGridView1.DataSource = bsUser;
            dataGridView1.AutoGenerateColumns = true;
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            List<DAL.User> list = DAL.SqliteHelper.GetUsers();

            if (list != null && list.Count > 0)
            {
                _list.Clear();
                _list.AddRange(list);
                bsUser.ResetBindings(false);

                // Опционально: показываем количество загруженных записей
                MessageBox.Show($"Загружено {list.Count} записей", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Нет данных для отображения", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Оставь пустым или добавь свой код
        }
    }

       
 }
 

