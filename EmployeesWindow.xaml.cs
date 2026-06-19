using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UfanetApp
{
    public partial class EmployeesWindow : Window
    {
        public EmployeesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                using (var db = new UfanetEntities  ())
                {
                    var employees = await db.employees.ToListAsync();
                    dgEmployees.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEditEmployeeWindow addWindow = new AddEditEmployeeWindow();
            addWindow.ShowDialog();
            LoadData();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Внимание");
                return;
            }

            var selected = dgEmployees.SelectedItem as employees;
            if (selected != null)
            {
                AddEditEmployeeWindow editWindow = new AddEditEmployeeWindow(selected);
                editWindow.ShowDialog();
                LoadData();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Внимание");
                return;
            }

            var selected = dgEmployees.SelectedItem as employees;
            if (selected == null) return;

            var fullName = $"{selected.last_name} {selected.first_name} {selected.patronymic}";
            var result = MessageBox.Show($"Удалить сотрудника {fullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new UfanetEntities())
                    {
                        var item = await db.employees.FindAsync(selected.employee_id);
                        if (item != null)
                        {
                            db.employees.Remove(item);
                            await db.SaveChangesAsync();
                        }
                    }
                    LoadData();
                    MessageBox.Show("Сотрудник удален!", "Успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}