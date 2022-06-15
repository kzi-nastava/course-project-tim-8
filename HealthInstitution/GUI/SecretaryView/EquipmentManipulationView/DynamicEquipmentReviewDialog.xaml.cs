﻿using HealthInstitution.Core.DIContainer;
using HealthInstitution.Core.Equipments;
using HealthInstitution.Core.Equipments.Model;
using HealthInstitution.Core.Equipments.Repository;
using HealthInstitution.Core.EquipmentTransfers;
using HealthInstitution.Core.Rooms;
using HealthInstitution.Core.Rooms.Model;
using HealthInstitution.Core.Rooms.Repository;
using System.Dynamic;
using System.Windows;

namespace HealthInstitution.GUI.SecretaryView
{
    /// <summary>
    /// Interaction logic for DynamicEquipmentReviewDialog.xaml
    /// </summary>
    public partial class DynamicEquipmentReviewDialog : Window
    {
        IEquipmentService _equipmentService;
        public DynamicEquipmentReviewDialog(IEquipmentService equipmentService)
        {
            InitializeComponent();
            _equipmentService = equipmentService;
            LoadRows();
        }
        private void ProcessDialog()
        {
            dataGrid.SelectedItem = null;
            LoadRows();
        }
        private void LoadRows()
        {
            List<dynamic> rows = _equipmentService.GetMissingEquipment();
            foreach(dynamic row in rows)
            {
                dataGrid.Items.Add(row);
            }
            dataGrid.Items.Refresh();
        }       
        private void selectEquipment_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedEquipment = (dynamic)dataGrid.SelectedItem;
            if (selectedEquipment != null)
            {
                DynamicEquipmentTransferDialog dynamicEquipmentTransferDialog = new DynamicEquipmentTransferDialog(selectedEquipment.Room, selectedEquipment.Equipment, DIContainer.GetService<IEquipmentService>(), DIContainer.GetService<IRoomService>(), DIContainer.GetService<IEquipmentTransferService>());
                dynamicEquipmentTransferDialog.ShowDialog();
                ProcessDialog();
            }
        }
    }
}
