using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FutureLink.BatchFlow4.Integrator.Info;

namespace FutureLink.BatchFlow4.Integrator.Configurator
{
    public partial class ctrlExportTask : UserControl, IconfigControl
    {
        private Info.IntegrationConfigDetail _taskInfo;


        public ctrlExportTask()
        {
            _taskInfo = new Info.IntegrationConfigDetail();
            NewTask = true;
            InitializeComponent();
            loadTaskInfo(_taskInfo);

        }

        public ctrlExportTask(Info.IntegrationConfigDetail taskInfo)
        {
            _taskInfo = taskInfo;
            NewTask = true;
            InitializeComponent();
            loadTaskInfo(_taskInfo);

        }

        public ctrlExportTask(Info.IntegrationConfigDetail taskInfo, bool newTask)
        {
            _taskInfo = taskInfo;
            NewTask = newTask;
            InitializeComponent();
            loadTaskInfo(_taskInfo);

        }

        private void loadTaskInfo(Info.IntegrationConfigDetail taskInfo)
        {
            serialText.Text = taskInfo.Serial;
            descriptionText.Text = taskInfo.Description;
            destinationFolderText.Text = taskInfo.Destination;
        }

        private Info.IntegrationConfigDetail getTaskInfo()
        {
            _taskInfo.Type = "Export";
            _taskInfo.Format = "xml";
            _taskInfo.Serial = serialText.Text;
            _taskInfo.Description = descriptionText.Text;
            _taskInfo.Destination = destinationFolderText.Text;

            return _taskInfo;
        }

        public Info.IntegrationConfigDetail TaskInfo
        {
            get
            {
                return getTaskInfo();
            }
            set
            {
                _taskInfo = value;
                loadTaskInfo(_taskInfo);
            }
        }

        public bool NewTask { get; set; }
        public bool HasChanges
        {
            get
            {
                if (_taskInfo.Serial != serialText.Text) return true;
                if (_taskInfo.Description != descriptionText.Text) return true;
                if (_taskInfo.Destination != destinationFolderText.Text) return true;
                return false;
            }
        }
    }
}
