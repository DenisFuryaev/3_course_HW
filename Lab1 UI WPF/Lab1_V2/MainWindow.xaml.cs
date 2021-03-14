﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;

using MyLibrary;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.IO;

namespace Lab1_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class DataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var result = value as V2DataCollection;
            //ObservableCollection<string> output = new ObservableCollection<string>();
            //foreach (V2DataCollection data in result)
            //{
            //    foreach(DataItem item in data.EM_list)
            //    {
            //        output.Add(item.ToString());
            //    }
            //}

            return result;

            //V2DataCollection data = (V2DataCollection)value;
            //string coords = data.grid_coord.X.ToString() + " " + data.grid_coord.Y.ToString();
            //return data.EM_frequency;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public partial class MainWindow : Window
    {

        public V2MainCollection main_collection { get; set; }
        public ICollectionView main_view { get; set; }
        public ICollectionView collection_view { get; set; }
        public ICollectionView grid_view { get; set; }
        public V2DataCollection selected_items { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result;
            if (main_collection.isModified)
            {
                result = MessageBox.Show("You have modified collection but didnt saved it.\n Do you wont to save it?", "Save or not?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveMenuItemClicked(null, null);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

        }

        private void UpdateBindings()
        {
            this.DataContext = null;

            listBox_details.ItemsSource = ListBox_DataCollection.SelectedItems;

            main_view = new CollectionViewSource() { Source = main_collection }.View;
            collection_view = new CollectionViewSource() { Source = main_collection }.View;
            collection_view.Filter = CollectionFilter;
            grid_view = new CollectionViewSource() { Source = main_collection }.View;
            grid_view.Filter = GridFilter;

            this.DataContext = this;
        }

        private void NewMenuItemClicked(object sender, RoutedEventArgs e)
        {
            main_collection = new V2MainCollection();
            UpdateBindings();
        }
        private void OpenMenuItemClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open_dialoge = new Microsoft.Win32.OpenFileDialog();
            open_dialoge.ShowDialog();
            string filename = open_dialoge.FileName;
            main_collection.Load(filename);

            UpdateBindings();
        }
        private void SaveMenuItemClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog save_dialoge = new Microsoft.Win32.SaveFileDialog();
            save_dialoge.ShowDialog();
            string filename = save_dialoge.FileName;
            main_collection.Save(filename);
        }

        private bool CollectionFilter(object item)
        {
            V2Data data = item as V2Data;
            return (data.GetType() == typeof(V2DataCollection));
        }
        private bool GridFilter(object item)
        {
            V2Data data = item as V2Data;
            return (data.GetType() == typeof(V2DataOnGrid));
        }

        private void AddDefaultsClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaults();
        }
        private void AddDefaultV2DataCollectionClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaultV2DataCollection();
        }
        private void AddDefaultV2DataOnGridClicked(object sender, RoutedEventArgs e)
        {
            main_collection.AddDefaultV2DataOnGrid();
        }

        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            main_collection.Remove(ListBox_Main.SelectedIndex);
        }

    }
}