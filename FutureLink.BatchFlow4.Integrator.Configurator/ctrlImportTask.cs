using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.About;

namespace FutureLink.BatchFlow4.Integrator.Configurator
{
    public partial class ctrlImportTask : UserControl, IconfigControl
    {
        private Info.IntegrationConfigDetail _taskInfo;

        public ctrlImportTask()
        {
            _taskInfo = new Info.IntegrationConfigDetail();
            NewTask = true;
            InitializeComponent();
            loadTaskInfo(_taskInfo);

        }

        public ctrlImportTask(Info.IntegrationConfigDetail taskInfo)
        {
            _taskInfo = taskInfo;            
            NewTask = true;
            InitializeComponent();
            loadTaskInfo(_taskInfo);

        }

        public ctrlImportTask(Info.IntegrationConfigDetail taskInfo, bool newTask)
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
            sourceFolderText.Text = taskInfo.Source;
            successFolderText.Text = taskInfo.Succeeded;
            failedFolderText.Text = taskInfo.Failed;
        }

        private Info.IntegrationConfigDetail getTaskInfo()
        {
            _taskInfo.Type = "Import";
            _taskInfo.Format = "xml";
            _taskInfo.Serial = serialText.Text;
            _taskInfo.Description = descriptionText.Text;
            _taskInfo.Source = sourceFolderText.Text;
            _taskInfo.Succeeded = successFolderText.Text;
            _taskInfo.Failed = failedFolderText.Text;
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
        public bool HasChanges { 
            get
            {
                if (_taskInfo.Serial != serialText.Text) return true;
                if (_taskInfo.Description != descriptionText.Text) return true;
                if (_taskInfo.Source != sourceFolderText.Text) return true;
                if (_taskInfo.Succeeded != successFolderText.Text) return true;
                if (_taskInfo.Failed != failedFolderText.Text) return true;
                return false;
            }
        }

        private void labelControl3_Click(object sender, EventArgs e)
        {

        }
    }
}
