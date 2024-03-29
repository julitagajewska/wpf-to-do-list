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
using System.Windows.Shapes;
using ToDoListApp.MVVM.Model;
using ToDoListApp.MVVM.ViewModel;
namespace ToDoListApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for CategoryPanelView.xaml
    /// </summary>
    public partial class CategoryPanelView : UserControl
    {
        public CategoryPanelView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(newCategoryInput.Text))
            {
                placeholder.Visibility = Visibility.Visible;

            }
            else
            {
                placeholder.Visibility = Visibility.Hidden;
            }
        }
    }
}
