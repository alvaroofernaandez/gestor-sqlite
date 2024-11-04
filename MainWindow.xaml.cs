using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace UA3Tarea1
{
    public partial class MainWindow : Window
    {
        private string dbConnectionString = "Data Source=database.sqlite;Version=3;";

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(dbConnectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS trabajadores (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre TEXT NOT NULL,
                            Edad INTEGER NOT NULL,
                            Profesion TEXT NOT NULL
                        );";
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    string checkDataQuery = "SELECT COUNT(*) FROM trabajadores;";
                    using (var command = new SQLiteCommand(checkDataQuery, connection))
                    {
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count == 0)
                        {
                            string insertDataQuery = @"
                                INSERT INTO trabajadores (Nombre, Edad, Profesion) VALUES
                                ('Juan', 25, 'Ingeniero'),
                                ('Ana', 30, 'Doctora'),
                                ('Luis', 28, 'Maestro'),
                                ('Pedro', 35, 'Abogado'),
                                ('Elena', 22, 'Estudiante');";
                            using (var insertCommand = new SQLiteCommand(insertDataQuery, connection))
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inicializando la base de datos: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                using (var connection = new SQLiteConnection(dbConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM trabajadores";
                    var adapter = new SQLiteDataAdapter(query, connection);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}");
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView row)
            {
                long id = Convert.ToInt64(row["Id"]);
                try
                {
                    using (var connection = new SQLiteConnection(dbConnectionString))
                    {
                        connection.Open();
                        var command = new SQLiteCommand($"DELETE FROM trabajadores WHERE Id = {id}", connection);
                        command.ExecuteNonQuery();
                    }
                    LoadData();
                    MessageBox.Show("Registro eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el registro: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Seleccione un registro para eliminar.");
            }
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView row)
            {
                long id = Convert.ToInt64(row["Id"]);
                if (int.TryParse(txtEdad.Text, out int edad))
                {
                    string nombre = txtNombre.Text;
                    string profesion = txtProfesion.Text;

                    try
                    {
                        using (var connection = new SQLiteConnection(dbConnectionString))
                        {
                            connection.Open();
                            var command = new SQLiteCommand(
                                "UPDATE trabajadores SET Nombre = @nombre, Edad = @edad, Profesion = @profesion WHERE Id = @id", connection);
                            command.Parameters.AddWithValue("@nombre", nombre);
                            command.Parameters.AddWithValue("@edad", edad);
                            command.Parameters.AddWithValue("@profesion", profesion);
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                        }
                        LoadData();
                        MessageBox.Show("Registro modificado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al modificar el registro: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("La edad debe ser un número entero válido.");
                }
            }
            else
            {
                MessageBox.Show("Seleccione un registro para modificar.");
            }
        }

        private void BtnAñadir_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtEdad.Text, out int edad))
            {
                string nombre = txtNombre.Text;
                string profesion = txtProfesion.Text;

                try
                {
                    using (var connection = new SQLiteConnection(dbConnectionString))
                    {
                        connection.Open();
                        var command = new SQLiteCommand(
                            "INSERT INTO trabajadores (Nombre, Edad, Profesion) VALUES (@nombre, @edad, @profesion)", connection);
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@edad", edad);
                        command.Parameters.AddWithValue("@profesion", profesion);
                        command.ExecuteNonQuery();
                    }
                    LoadData();
                    MessageBox.Show("Registro añadido correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al añadir el registro: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("La edad debe ser un número entero válido.");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView row)
            {
                txtNombre.Text = row["Nombre"].ToString();
                txtEdad.Text = row["Edad"].ToString();
                txtProfesion.Text = row["Profesion"].ToString();
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {  
            if (MessageBox.Show("¿Estás seguro que quieres salir?", "Responda", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes){
                this.Close();
            }
        }
    }
}
