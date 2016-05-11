using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using Windows.Storage;
using System.Diagnostics;

namespace SQLiteMerge
{ 

    public sealed partial class MainPage : Page
    {
        private SQLiteAsyncConnection dbCon;

        public MainPage()
        {
            this.InitializeComponent();

        }

        [Table("Students")]
        public sealed class Student
        {
            public string Name { get; set; }
            public int RollNo { get; set; }
            public string Faculty { get; set; }
        }

        //Creates First Database
        private async void btnCreateFirstDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbCon = new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\FirstDatabase.sqlite");
                await dbCon.CreateTableAsync<Student>();

                var student = new Student
                {
                    Name = "Ram",
                    RollNo = 1,
                    Faculty = "Physics"
                };
                await dbCon.InsertAsync(student);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        //Creates Second Database
        private async void btnCreateSecondDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SQLiteAsyncConnection dbCon = new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\SecondDatabase.sqlite");
                await dbCon.CreateTableAsync<Student>();

                var student = new Student
                {
                    Name = "Shyam",
                    RollNo = 2,
                    Faculty = "Biology"
                };
                await dbCon.InsertAsync(student);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        //Merging two databases. Inserts values from Second database to First Database
        private async void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstDbPath = ApplicationData.Current.LocalFolder.Path + "\\FirstDatabase.sqlite";
                string secondDbPath = ApplicationData.Current.LocalFolder.Path + "\\SecondDatabase.sqlite";
                await dbCon.ExecuteAsync("ATTACH DATABASE '" + firstDbPath + "' AS firstDB;");
                await dbCon.ExecuteAsync("ATTACH DATABASE '" + secondDbPath + "' AS secondDB");

                string query = "INSERT OR REPLACE INTO firstDB.Students ("
                + "Name, RollNo, Faculty) "
                + "SELECT Name, RollNo, Faculty "
                + "FROM secondDB.Students";
                await dbCon.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
