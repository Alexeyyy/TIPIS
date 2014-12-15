using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Course_work_TIPIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int count_InitialVariables;
        private int count_InitialConditions;
        
        public bool XmlLoad
        { 
            get; set; 
        }

        public int GetInitialConditionsCount()
        {
            return count_InitialConditions;
        }

        public int GetInitialVariablesCount()
        {
            return count_InitialVariables;
        }


        
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Обработчики событий окна
        //if CreateVariant Radiobutton is checked than provide an ability for calculation of existing variant 
        private void RDBTN_ChooseVariant_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true) {
                //set a red color for highlighting of a enable container 
                BRD_ChooseVariant.BorderBrush = new SolidColorBrush(Colors.Red);
                //set an "Enable" property of all container's children in value "true"
                TB_WriteVariant.IsEnabled = true;

                //set a blue color for highlighting of a not enable container
                BRD_CreateOwnVariant.BorderBrush = new SolidColorBrush(Colors.CadetBlue);
                CMBB_ConditionsNumber.IsEnabled = false;
                CMBB_ElementsNumber.IsEnabled = false;
            }
        }

        //if ChooseVariant Radiobutton is checked than provide an ability of hand-input values of coefficieints and signs
        private void RDBTN_CreateVariant_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                //set a blue color for highlighting of a not enable container 
                BRD_ChooseVariant.BorderBrush = new SolidColorBrush(Colors.CadetBlue);
                TB_WriteVariant.IsEnabled = false;

                //set a red color for highlighting of a enable container
                BRD_CreateOwnVariant.BorderBrush = new SolidColorBrush(Colors.Red);
                CMBB_ConditionsNumber.IsEnabled = true;
                CMBB_ElementsNumber.IsEnabled = true;
            }
        }
        
        //If the button  was pressed then save a count of conditions and variables    
        private void Button_CreateConditions(object sender, RoutedEventArgs e)
        {
            if (RDB_ChooseVariant.IsChecked == true)
            {
                //Take a content of XML-file 
                XmlLoad = true;
                string xmlVars = Course_work_TIPIS.Properties.Resources.variants.ToString();
                try
                {
                    if (XML_Loader.WorkWithXml(xmlVars, Convert.ToInt32(TB_WriteVariant.Text), ref count_InitialConditions, ref count_InitialVariables)) {
                        LaunchChildForm(this, new DecisionWindow());
                    }
                }
                catch {
                    MessageBox.Show("Номер варианта указан неверно!");
                }
                finally {
                    //...
                }
            }

            if (RDB_CreateVariant.IsChecked == true)
            {
                count_InitialConditions = Convert.ToInt32(CMBB_ConditionsNumber.SelectionBoxItem.ToString());
                count_InitialVariables = Convert.ToInt32(CMBB_ElementsNumber.SelectionBoxItem.ToString());
                               
                //Launch a solution-window
                LaunchChildForm(this, new DecisionWindow());
            }
        }

        #endregion

        #region A child window (An input form window)
        //Launch a child window
        private void LaunchChildForm(Window _owner, Window _child)
        {
            _child.Owner = _owner;
            _child.ShowDialog();
        }
        #endregion
    }
}
