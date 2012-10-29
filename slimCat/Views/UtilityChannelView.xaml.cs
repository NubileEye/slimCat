﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModels;

namespace Views
{
    /// <summary>
    /// Interaction logic for UtilityChannelView.xaml
    /// </summary>
    public partial class UtilityChannelView : UserControl
    {
        private ViewModelBase _vm;

        public UtilityChannelView(UtilityChannelViewModel vm)
        {
            InitializeComponent();
            _vm = vm;

            this.DataContext = _vm;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}