using System.Data.Common;
using System.Data;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System;

namespace ADONETHomeWork3;

public partial class MainWindow : Window
{

    DbConnection? connection = null;
    DbDataAdapter? adapter = null;
    DbProviderFactory? providerFactory = null;
    DataSet? dataSet = null;
    string? providerName = null;

    public MainWindow()
    {
        InitializeComponent();

        DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));
        providerName = "System.Data.SqlClient";

        providerFactory = DbProviderFactories.GetFactory(providerName);


        dataSet = new DataSet();
        connection = providerFactory.CreateConnection();
        connection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        adapter = providerFactory.CreateDataAdapter();
    }

    private void btn_Execute_Click(object sender, RoutedEventArgs e)
    {
        var command = providerFactory.CreateCommand();

        command.CommandText = tBox_Command.Text;
        command.Connection = connection;

        adapter.SelectCommand = command;

        if (Tabs.Items.Count > 1)
            for (int i = Tabs.Items.Count - 1; i > 0; i--)
                Tabs.Items.RemoveAt(i);


        dataSet.Tables.Clear();

        try
        {
            adapter.Fill(dataSet);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        GenerateTabs();
    }

    private void GenerateTabs()
    {
        foreach (DataTable table in dataSet.Tables)
        {
            var tab = new TabItem();
            tab.Header = table.TableName;

            var dataGrid = new DataGrid();


            dataGrid.IsReadOnly = true;

            dataGrid.ItemsSource = table.AsDataView();

            tab.Content = dataGrid;

            Tabs.Items.Add(tab);
        }
    }
}
