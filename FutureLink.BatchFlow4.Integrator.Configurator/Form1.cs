using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Layout;
using System.Runtime.InteropServices;
using DevExpress.XtraLayout.Utils;

namespace FutureLink.BatchFlow4.Integrator.Configurator
{
    public partial class Form1 : Form
    {
        IconfigControl _taskObject;
        private Info.ConfigurationInfo _config;

        public Form1()
        {
            InitializeComponent();
            _config = loadConfiguration();
            displayConfig();
        }

        private void displayConfig()
        {
            if (_config.ExecutionInterval < 5)
                _config.ExecutionInterval = 5;
            if (_config.ApiUrl.Length == 0)
                _config.ApiUrl = "https://api.batchflow.dk";
            apiUrlText.EditValue = _config.ApiUrl;
            IntervalNum.EditValue = _config.ExecutionInterval;
            foreach (Info.IntegrationConfigDetail configDetail in _config.Items)
            {
                NavBarItemLink navItem = null;
                NavBarItem item = null;
                switch (configDetail.Type.ToLower())
                {
                    case "import":
                        navItem = navBarGroup1.AddItem();
                        break;
                    case "export":
                        navItem = navBarGroup2.AddItem();
                        break;

                }
                if (navItem != null)
                {
                    item = navItem.Item;
                    item.Tag = configDetail;
                    item.Caption = configDetail.Description + " (" + configDetail.Serial + ")";
                }

            }
            navBarGroup1.Expanded = true;
            navBarGroup2.Expanded = true;
        }

        private bool saveConfiguration()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Info.ConfigurationInfo));
                string fileName = getConfigurationFile();
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    serializer.Serialize(stream, _config);
                }
                return true;

            }
            catch (Exception ex)
            {

                string test = ex.Message;
                return false;
            }
        }

        private Info.ConfigurationInfo loadConfiguration()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Info.ConfigurationInfo));
                string fileName = getConfigurationFile();
                if (File.Exists(fileName))
                    {
                    using (FileStream stream = new FileStream(fileName, FileMode.Open))
                    {
                        Info.ConfigurationInfo config = (Info.ConfigurationInfo)serializer.Deserialize(stream);
                        return config;
                    }
                } else
                    return new Info.ConfigurationInfo();
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        private string getConfigurationFile()
        {
            string curPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(curPath, "configuration.xml");
        }

        private void btnAddImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (trySave())
            {
                splitContainerControl1.Panel2.Controls.Clear();
                _taskObject = null;
                ctrlImportTask importTaskControl = new ctrlImportTask();
                splitContainerControl1.Panel2.Controls.Add(importTaskControl);
                importTaskControl.Dock = DockStyle.Fill;
                ribbonPageGroup3.Visible = true;
                _taskObject = importTaskControl;
            }
        }

        private void btnAddExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (trySave())
            {
                splitContainerControl1.Panel2.Controls.Clear();
                _taskObject = null;
                ctrlExportTask taskControl = new ctrlExportTask();
                splitContainerControl1.Panel2.Controls.Add(taskControl);
                taskControl.Dock = DockStyle.Fill;
                ribbonPageGroup3.Visible = true;
                _taskObject = taskControl;
            }
        }

        private bool saveTask()
        {
            if (_taskObject != null)
            {
                Info.IntegrationConfigDetail taskInfo = _taskObject.TaskInfo;
                if (_taskObject.NewTask)
                {
                    _config.Items.Add(taskInfo);
                    if (!saveConfiguration())
                    {
                        _config.Items.Remove(taskInfo);
                        return false;
                    }

                    NavBarItemLink navItem = null;
                    switch (taskInfo.Type.ToLower())
                    {
                        case "import":
                            navItem = navBarGroup1.AddItem();
                            break;
                        case "export":
                            navItem = navBarGroup2.AddItem();
                            break;
                            default:
                            navItem = null;
                            break;
                    }
                    NavBarItem item = navItem.Item;
                    item.Tag = taskInfo;
                    item.Caption = taskInfo.Description + "(" + taskInfo.Serial + ")";
                    _taskObject.NewTask = false;
                }
            } else
            {
                saveConfiguration();
            }
            return true;
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            saveTask();
            if (navBarControl1.SelectedLink != null)
            {
                Info.IntegrationConfigDetail taskInfo = (Info.IntegrationConfigDetail)navBarControl1.SelectedLink.Item.Tag;
                navBarControl1.SelectedLink.Item.Caption = taskInfo.Description + " (" + taskInfo.Serial + ")";
            }
            
        }



        private void navBarControl1_SelectedLinkChanged(object sender, DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventArgs e)
        {

        }

        private bool trySave()
        {
            if (_taskObject != null)
            {
                if (_taskObject.HasChanges )
                {
                    return saveTask();
                }
            }
            saveConfiguration();
            return true;
        }

        private void displayEditor(Info.IntegrationConfigDetail taskInfo)
        {
            if (trySave())
            {
                splitContainerControl1.Panel2.Controls.Clear();
                _taskObject = null;
                switch (taskInfo.Type.ToLower())
                {
                    case "import":
                        ctrlImportTask importConfigObject = new ctrlImportTask(taskInfo,false);
                        splitContainerControl1.Panel2.Controls.Add((importConfigObject));
                        importConfigObject.Dock = DockStyle.Fill;
                        _taskObject = importConfigObject;
                        break;
                    case "export":
                        ctrlExportTask exportConfigObject = new ctrlExportTask(taskInfo, false);
                        splitContainerControl1.Panel2.Controls.Add((exportConfigObject));
                        exportConfigObject.Dock = DockStyle.Fill;
                        _taskObject = exportConfigObject;
                        break;
                }
                if (_taskObject != null)
                    ribbonPageGroup3.Visible = true;
                else
                    ribbonPageGroup3.Visible = false;
            }
        }

        private void navBarControl1_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (e.Link != null)
            {
                Info.IntegrationConfigDetail taskInfo = (Info.IntegrationConfigDetail)e.Link.Item.Tag;
                displayEditor(taskInfo);

            }
        }

        private void btnRemove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (navBarControl1.SelectedLink != null)
            {
                Info.IntegrationConfigDetail taskInfo = (Info.IntegrationConfigDetail)navBarControl1.SelectedLink.Item.Tag;
                _config.Items.Remove(taskInfo);
                if (saveConfiguration())
                    navBarControl1.Items.Remove(navBarControl1.SelectedLink.Item);
            }
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            _config.ExecutionInterval = Convert.ToInt32(IntervalNum.EditValue);
        }

        private void apiUrlText_EditValueChanged(object sender, EventArgs e)
        {
            _config.ApiUrl = apiUrlText.EditValue.ToString();
        }

        private void IntervalNum_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            trySave();

        }
    }
}
