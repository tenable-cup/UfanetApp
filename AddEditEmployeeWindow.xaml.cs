using System;
using System.Windows;

namespace UfanetApp
{
    public partial class AddEditEmployeeWindow : Window
    {
        private employees _editingEmployee;
        private bool _isEditMode = false;

        public AddEditEmployeeWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            tbTitle.Text = "Добавление сотрудника";
        }

        public AddEditEmployeeWindow(employees employee)
        {
            InitializeComponent();
            _isEditMode = true;
            _editingEmployee = employee;
            tbTitle.Text = "Редактирование сотрудника";
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            tbLastName.Text = _editingEmployee.last_name;
            tbFirstName.Text = _editingEmployee.first_name;
            tbPatronymic.Text = _editingEmployee.patronymic;
            tbPosition.Text = _editingEmployee.position;
            tbDepartment.Text = _editingEmployee.department;
            tbPhone.Text = _editingEmployee.phone;
            tbEmail.Text = _editingEmployee.email;
            tbSalary.Text = _editingEmployee.salary.ToString();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbLastName.Text))
            {
                MessageBox.Show("Введите фамилию!", "Внимание");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbFirstName.Text))
            {
                MessageBox.Show("Введите имя!", "Внимание");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbPosition.Text))
            {
                MessageBox.Show("Введите должность!", "Внимание");
                return;
            }

            decimal salary = 0;
            if (!string.IsNullOrWhiteSpace(tbSalary.Text))
                decimal.TryParse(tbSalary.Text, out salary);

            try
            {
                using (var db = new UfanetEntities())
                {
                    if (_isEditMode)
                    {
                        var emp = await db.employees.FindAsync(_editingEmployee.employee_id);
                        if (emp != null)
                        {
                            emp.last_name = tbLastName.Text.Trim();
                            emp.first_name = tbFirstName.Text.Trim();
                            emp.patronymic = tbPatronymic.Text.Trim();
                            emp.position = tbPosition.Text.Trim();
                            emp.department = tbDepartment.Text.Trim();
                            emp.phone = tbPhone.Text.Trim();
                            emp.email = tbEmail.Text.Trim();
                            emp.salary = salary;
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var newEmp = new employees
                        {
                            last_name = tbLastName.Text.Trim(),
                            first_name = tbFirstName.Text.Trim(),
                            patronymic = tbPatronymic.Text.Trim(),
                            position = tbPosition.Text.Trim(),
                            department = tbDepartment.Text.Trim(),
                            phone = tbPhone.Text.Trim(),
                            email = tbEmail.Text.Trim(),
                            salary = salary,
                            hire_date = DateTime.Now,
                            is_active = true
                        };
                        db.employees.Add(newEmp);
                        await db.SaveChangesAsync();
                    }
                }

                MessageBox.Show("Данные сохранены!", "Успешно");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}