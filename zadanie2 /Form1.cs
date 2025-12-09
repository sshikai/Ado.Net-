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
        private bool _newUserEdit;

        public Form1()
        {
            InitializeComponent();

            _list = new List<DAL.User>();
            bsUser.DataSource = _list;
            dataGridView1.DataSource = bsUser;
            dataGridView1.AutoGenerateColumns = true;
            ucUserEdit1.User = new DAL.User { Date = DateTime.Now.Date };
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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



        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var user = (DAL.User)bsUser.Current;
            ucUserEdit1.User = user;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. Берем данные из ucUserEdit (которые ты ввел в правой части)
            var newUser = ucUserEdit1.User;

            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Name))
            {
                MessageBox.Show("Введите имя пользователя", "Внимание",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Добавляем в базу
            bool success = DAL.SqliteHelper.AddUser(newUser);

            if (success)
            {
                MessageBox.Show("Пользователь добавлен!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3. Очищаем форму для ввода
                ucUserEdit1.User = new DAL.User { Date = DateTime.Now.Date };

                // 4. Обновляем таблицу
                RefreshUserList();
            }
            else
            {
                MessageBox.Show("Не удалось добавить пользователя", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Проверяем, выбран ли пользователь в таблице
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования (кликните по строке)",
                               "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Получаем выбранного пользователя из таблицы
            var selectedUser = dataGridView1.CurrentRow.DataBoundItem as DAL.User;

            if (selectedUser == null)
            {
                MessageBox.Show("Ошибка: не удалось получить данные пользователя",
                               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Берем данные из ucUserEdit (которые ты ввел в правой части формы)
            var editedUser = ucUserEdit1.User;

            if (editedUser == null)
            {
                MessageBox.Show("Нет данных для сохранения", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Копируем ID от выбранного пользователя (чтобы знать кого обновлять)
            editedUser.Id = selectedUser.Id;

            // 5. Сохраняем в базу
            bool success = DAL.SqliteHelper.SaveUser(editedUser);

            if (success)
            {
                MessageBox.Show("Изменения сохранены!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshUserList(); // Обновляем таблицу
            }
            else
            {
                MessageBox.Show("Не удалось сохранить изменения", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли выбранная строка в таблице
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя в таблице (кликните по строке)",
                               "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем пользователя из выбранной строки
            var user = dataGridView1.CurrentRow.DataBoundItem as DAL.User;

            if (user == null)
            {
                MessageBox.Show("Ошибка: не удалось получить данные пользователя",
                               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Спрашиваем подтверждение
            DialogResult result = MessageBox.Show(
                $"Вы точно хотите удалить пользователя:\n" +
                $"Имя: {user.Name}\n" +
                $"Логин: {user.UserName}",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Удаляем из базы
                bool success = DAL.SqliteHelper.DeleteUser(user.Id);

                if (success)
                {
                    MessageBox.Show("Пользователь успешно удалён",
                                  "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ОБНОВЛЯЕМ ТАБЛИЦУ
                    RefreshUserList();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить пользователя",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshUserList()
        {
            List<DAL.User> list = DAL.SqliteHelper.GetUsers();

            if (list != null && list.Count > 0)
            {
                _list.Clear();
                _list.AddRange(list);
                bsUser.ResetBindings(false);

                // Автоматически выбираем первый элемент
                if (_list.Count > 0)
                {
                    bsUser.Position = 0;
                }
            }
            else
            {
                _list.Clear();
                bsUser.ResetBindings(false);
                ucUserEdit1.User = new DAL.User { Date = DateTime.Now.Date };
            }
        }
    }

       
 }
 


