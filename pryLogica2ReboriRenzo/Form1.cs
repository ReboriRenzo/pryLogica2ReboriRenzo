using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryLogica2ReboriRenzo
{
    public partial class Form1 : Form
    {
        private DateTime[,] Rc = new DateTime[25, 3];
        private DateTime[,] Fideo = new DateTime[10, 2];
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\TuUsuario\Desktop\TuBaseDeDatos.accdb;";

        public Form1()
        {
            InitializeComponent();
            CargarGrilla();
            CargarComboBoxLaboratorios();
            dateTimePicker1.ValueChanged += new EventHandler(dateTimePicker1_ValueChanged);
        }

        // Actividad 1: Cargar datos en una grilla
        private void CargarGrilla()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("FechaVencimiento", "Fecha Vencimiento");
            dataGridView1.Columns.Add("Nombre", "Nombre Medicamento");
            dataGridView1.Columns.Add("Laboratorio", "Nombre Laboratorio");
            dataGridView1.Columns.Add("StockMinimo", "Stock Mínimo");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT FechaVencimiento, Nombre, Laboratorio, StockMinimo FROM Medicamentos";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(
                                reader["FechaVencimiento"].ToString(),
                                reader["Nombre"].ToString(),
                                reader["Laboratorio"].ToString(),
                                reader["StockMinimo"].ToString());
                        }
                    }
                }
            }
        }

        // Actividad 2: Filtrar y mostrar medicamentos por laboratorio
        private void CargarComboBoxLaboratorios()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT DISTINCT Laboratorio FROM Medicamentos";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader["Laboratorio"].ToString());
                        }
                    }
                }
            }
            comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string laboratorioSeleccionado = comboBox1.SelectedItem.ToString();
            CargarGrillaPorLaboratorio(laboratorioSeleccionado);
        }

        private void CargarGrillaPorLaboratorio(string laboratorio)
        {
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();

            dataGridView2.Columns.Add("Nombre", "Nombre Medicamento");
            dataGridView2.Columns.Add("Cantidad", "Cantidad Provista");
            dataGridView2.Columns.Add("Total", "Total en Dinero");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Nombre, StockMinimo, Precio FROM Medicamentos WHERE Laboratorio = @Laboratorio";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Laboratorio", laboratorio);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int cantidadProvista = Convert.ToInt32(reader["StockMinimo"]);
                            decimal precio = Convert.ToDecimal(reader["Precio"]);
                            decimal totalDinero = cantidadProvista * precio;
                            dataGridView2.Rows.Add(reader["Nombre"].ToString(), cantidadProvista, totalDinero);
                        }
                    }
                }
            }
        }

        // Actividad 3: Filtrar y mostrar medicamentos por fecha de vencimiento
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = dateTimePicker1.Value;
            CargarGrillaPorFecha(fechaSeleccionada);
        }

        private void CargarGrillaPorFecha(DateTime fecha)
        {
            dataGridView3.Columns.Clear();
            dataGridView3.Rows.Clear();

            dataGridView3.Columns.Add("Laboratorio", "Nombre Laboratorio");
            dataGridView3.Columns.Add("Nombre", "Nombre Medicamento");
            dataGridView3.Columns.Add("Cantidad", "Cantidad");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Laboratorio, Nombre, StockMinimo FROM Medicamentos WHERE FechaVencimiento = @FechaVencimiento";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FechaVencimiento", fecha);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView3.Rows.Add(
                                reader["Laboratorio"].ToString(),
                                reader["Nombre"].ToString(),
                                reader["StockMinimo"].ToString());
                        }
                    }
                }
            }
        }

        // Actividad 4: Cargar matriz de medicamentos vencidos
        private void CargarFideoConMedicamentosVencidos()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, FechaVencimiento FROM Medicamentos WHERE FechaVencimiento < @Hoy";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Hoy", DateTime.Now);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        int i = 0;
                        while (reader.Read() && i < Fideo.GetLength(0))
                        {
                            Fideo[i, 0] = Convert.ToDateTime(reader["FechaVencimiento"]);
                            Fideo[i, 1] = new DateTime(Convert.ToInt32(reader["Id"]), 1, 1); // Usando el ID del medicamento en la fecha
                            i++;
                        }
                    }
                }
            }
        }

        // Actividad 5: Mostrar medicamentos con opciones de pago
        private void CargarGrillaConOpcionesDePago()
        {
            dataGridView4.Columns.Clear();
            dataGridView4.Rows.Clear();

            dataGridView4.Columns.Add("IdMedicamento", "ID Medicamento");
            dataGridView4.Columns.Add("Nombre", "Nombre Medicamento");
            dataGridView4.Columns.Add("Precio", "Precio");
            dataGridView4.Columns.Add("Credito", "Crédito (+40%)");
            dataGridView4.Columns.Add("Debito", "Débito (+5%)");
            dataGridView4.Columns.Add("Transferencia", "Transferencia (+7%)");
            dataGridView4.Columns.Add("Contado", "Contado (-10%)");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Nombre, Precio FROM Medicamentos WHERE Precio > 15000";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["Id"]);
                            string nombre = reader["Nombre"].ToString();
                            decimal precio = Convert.ToDecimal(reader["Precio"]);

                            decimal precioCredito = precio * 1.40m;
                            decimal precioDebito = precio * 1.05m;
                            decimal precioTransferencia = precio * 1.07m;
                            decimal precioContado = precio * 0.90m;

                            dataGridView4.Rows.Add(
                                id,
                                nombre,
                                precio,
                                precioCredito,
                                precioDebito,
                                precioTransferencia,
                                precioContado);
                        }
                    }
                }
            }
        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}



