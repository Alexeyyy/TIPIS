using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace Course_work_TIPIS
{
    /// <summary>
    /// Interection logic for DecisionWindow.xaml
    /// </summary>
    public partial class DecisionWindow : Window
    {
        private object[,] targetFunction; //target function
        private List<object[,]> conditionsSimplex = new List<object[,]>(); //conditions for simplex-algorithm (They've signs like <= or = or =>)
        //unused
        private List<object[,]> conditionsGraphic = new List<object[,]>(); //conditions for graphic-building (They've signs like < or >)
        //unused
        private object[,] artificialFunction; //artificial function
        private object[,] simplexTable;  //solution-table for simplex-algorithm
        private object[,] solutionMatrix; //received solution
        private double minValue; //a value of target function which has a place to be in case of received solution

        public DecisionWindow()
        {
            InitializeComponent();
        }
        
        #region Form input window
        
        //Create a template of target function
        private void CreateTargetFunctionTemplate()
        {
            DockPanel DP_targetFunctionTemplate = new DockPanel();
            DP_targetFunctionTemplate.Name = "TargetFunction";
            
            //get a number of variables
            int count_Variables = ((this.Owner as MainWindow).GetInitialVariablesCount());  
            
            //create a template under target function
            Label LB_functionName = new Label(); //the name of the function
            TextBox[] TBs_variablesCoefficients = new TextBox[count_Variables]; //variables coefficients
            Label[] LBs_signs = new Label[count_Variables - 1]; //signs
            Label[] LBs_variablesNames = new Label[count_Variables]; //titles

            LB_functionName.Content = "Целевая функция: F[x] = ";
            DP_targetFunctionTemplate.Children.Add(LB_functionName);

            for (int i = 0; i < count_Variables; i++)
            {
                //textbox under coefficient
                TBs_variablesCoefficients[i] = new TextBox();
                //following this event code we could enter only numbers and "," for float members
                TBs_variablesCoefficients[i].PreviewKeyDown += DecisionWindow_PreviewKeyDown;
                TBs_variablesCoefficients[i].MinWidth = 30;
                TBs_variablesCoefficients[i].Width = 30;
                TBs_variablesCoefficients[i].MaxWidth = 30;
                TBs_variablesCoefficients[i].MaxWidth = 30;
                TBs_variablesCoefficients[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                TBs_variablesCoefficients[i].VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //every textbox's value is 0 by default
                TBs_variablesCoefficients[i].Text = "0";
                //the names of variables
                LBs_variablesNames[i] = new Label();
                LBs_variablesNames[i].Content = "x" + (i + 1);

                DP_targetFunctionTemplate.Children.Add(TBs_variablesCoefficients[i]);
                DP_targetFunctionTemplate.Children.Add(LBs_variablesNames[i]);
                //avoid OutOfRangeException for signs (because the amount of last ones is less on 1 then amount of variables)
                if(i != count_Variables - 1) 
                {
                    //set a sign as a label's content (always take a sign "+" because "-" will take from coefficient)
                    LBs_signs[i] = new Label();
                    LBs_signs[i].Content = "+";
                    DP_targetFunctionTemplate.Children.Add(LBs_signs[i]);
                }
            }

            //add our template on the form
            STPAN_InitialTemplatesPanel.Children.Add(DP_targetFunctionTemplate);
        }
                            
        //Create a condition's template
        private void CreateConditionTemplates(int _conditionNumber)
        {
            DockPanel DP_conditionTemplate = new DockPanel();
            DP_conditionTemplate.Name = "Condition";

            int count_Variables = ((this.Owner as MainWindow).GetInitialVariablesCount());

            Label LB_conditionName = new Label();
            
            TextBox[] TBs_variablesCoefficients = new TextBox[count_Variables]; //коэффициенты перед переменными
            Label[] LBs_signs = new Label[count_Variables - 1]; //знаки
            Label[] LBs_variablesNames = new Label[count_Variables]; //названия
            TextBox TB_freeItem = new TextBox();
            ComboBox CB_comparativeSigns = new ComboBox();

            TB_freeItem.PreviewKeyDown += DecisionWindow_PreviewKeyDown;
            TB_freeItem.MinWidth = 30;
            TB_freeItem.Width = 30;
            TB_freeItem.MaxWidth = 30;
            TB_freeItem.Margin = new Thickness(5,0,0,0);
            TB_freeItem.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            TB_freeItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            TB_freeItem.Text = "0";
            TB_freeItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
           
            CB_comparativeSigns.SelectedIndex = 0; //пусть выделен по умолчанию 0 элемент списка
            CB_comparativeSigns.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            CB_comparativeSigns.Items.Add("=");
            CB_comparativeSigns.Items.Add("<=");
            CB_comparativeSigns.Items.Add("=>");
            
            LB_conditionName.Content = "Условие " + _conditionNumber + ":";
            DP_conditionTemplate.Children.Add(LB_conditionName);

            for (int i = 0; i < count_Variables; i++)
            {
                TBs_variablesCoefficients[i] = new TextBox();
                TBs_variablesCoefficients[i].PreviewKeyDown += DecisionWindow_PreviewKeyDown;
                TBs_variablesCoefficients[i].MinWidth = 30;
                TBs_variablesCoefficients[i].Width = 30;
                TBs_variablesCoefficients[i].MaxWidth = 30;
                TBs_variablesCoefficients[i].Margin = new Thickness(0);
                TBs_variablesCoefficients[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                TBs_variablesCoefficients[i].VerticalAlignment = System.Windows.VerticalAlignment.Center;
                TBs_variablesCoefficients[i].Text = "0";

                LBs_variablesNames[i] = new Label();
                LBs_variablesNames[i].Content = "x" + (i + 1);
                
                DP_conditionTemplate.Children.Add(TBs_variablesCoefficients[i]);
                DP_conditionTemplate.Children.Add(LBs_variablesNames[i]);

                if (i != count_Variables - 1)
                {
                    LBs_signs[i] = new Label();
                    LBs_signs[i].Content = "+";
                    DP_conditionTemplate.Children.Add(LBs_signs[i]);
                }
            }

            DP_conditionTemplate.Children.Add(CB_comparativeSigns);
            DP_conditionTemplate.Children.Add(TB_freeItem);

            STPAN_InitialTemplatesPanel.Children.Add(DP_conditionTemplate);
        }

        #region An event handlers of created controllers
        //Input only "," and numbers in the textbox field
        private void DecisionWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Back || (e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.Decimal))
            {
                e.Handled = true;
            }
        }
        #endregion

        #endregion

        #region A handling of window's events and controllers which initially lie on the form 
        
        //When the form loads we form a fields for input of conditions and target function
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateTargetFunctionTemplate();
            for (int i = 0; i < (this.Owner as MainWindow).GetInitialConditionsCount(); i++)
                CreateConditionTemplates(i + 1);
            
            //if data load from xml-file than set data into controllers (TextBoxes and etc)
            if ((this.Owner as MainWindow).XmlLoad)
            {
                (this.Owner as MainWindow).XmlLoad = false;
                UploadXmlData(XML_Loader.dataArray);
            }
        }

        //Upload our xml-data into contollers
        private void UploadXmlData(object[,] _dataArray)
        {
            //Trace.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //Service.TracingObjectMatrix(_dataArray);

            for (int i = 0; i < STPAN_InitialTemplatesPanel.Children.Count; i++)
            {
                for (int j = 0, k = 0; j < (STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children.Count; j++)
                {
                    if (((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j] is TextBox) || ((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j] is ComboBox))
                    {
                        Trace.WriteLine(i + " " + k);
                        Trace.WriteLine((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j].GetType().ToString());
                        try
                        {
                            ((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j] as TextBox).Text = _dataArray[i, k].ToString();
                        }
                        catch
                        {
                            //in case of "NOTHING" value in dataArray 
                            if (_dataArray[i, k].ToString() == "nothing")
                            {
                                //j++;
                                k++;
                                continue;
                            }
                            //in case of "EQUALITY" condition
                            if (_dataArray[i, k].ToString() == "equal")
                            {
                                ((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j] as ComboBox).SelectedIndex = 0;
                            }
                            //in case of "LESS" condition
                            if (_dataArray[i, k].ToString() == "less")
                            {
                                ((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j] as ComboBox).SelectedIndex = 1;
                            }
                            //in case of "MORE" condition
                            if (_dataArray[i, k].ToString() == "more")
                            {
                                ((STPAN_InitialTemplatesPanel.Children[i] as DockPanel).Children[j] as ComboBox).SelectedIndex = 2;
                            }
                        }
                        finally
                        {
                            k++;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        #region Button "To get a solution"

        private void ShowSolution(StackPanel _STPAN_solutionContainer, TextBlock _TBL_solution, object[,] _solutionMatrix, double _minValue)
        {
            Service.RecordSolution(_solutionMatrix, _minValue);

            _TBL_solution.Text = Service.solutionString;
            _STPAN_solutionContainer.Visibility = System.Windows.Visibility.Visible;
        }

        //Solve input system via simplex-method
        private void BTN_Solve_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                targetFunction = DataHandling.CreateTargetFunctionMatrix(STPAN_InitialTemplatesPanel, (this.Owner as MainWindow).GetInitialConditionsCount(), (this.Owner as MainWindow).GetInitialVariablesCount());

                //Trace.WriteLine("CREATE MATRIXES");
                DataHandling.CreateEquationMatrix(STPAN_InitialTemplatesPanel, (this.Owner as MainWindow).GetInitialConditionsCount(), (this.Owner as MainWindow).GetInitialVariablesCount(), ref conditionsSimplex, ref conditionsGraphic);
                //Trace.WriteLine("FOR SIMPLEX METHOD!!!");
                //Service.TracingConditionsList(conditionsSimplex);

                //Trace.WriteLine("FOR GRAPHIC METHOD!!!");
                //Service.TracingConditionsList(conditionsGraphic);

                //Trace.WriteLine("ARTIFICIAL VARIABLES!!!!");
                DataHandling.AddArtificialVariables(ref conditionsSimplex, (this.Owner as MainWindow).GetInitialVariablesCount());
                //Service.TracingConditionsList(conditionsSimplex);

                artificialFunction = DataHandling.GetArtificialFunctionMatrix(conditionsSimplex, (this.Owner as MainWindow).GetInitialVariablesCount());
                //Trace.WriteLine("Artificial Function!!!!!!!!!!!!!!");
                //Service.TracingObjectMatrix(artificialFunction);

                simplexTable = DataHandling.FormTableForSimplexMethod(targetFunction, conditionsSimplex, artificialFunction, conditionsSimplex.Count, (this.Owner as MainWindow).GetInitialVariablesCount());
                //Сохраним матрицу на итерации 0 
                Service.RecordMatrix(simplexTable);
                
                //Trace.WriteLine("\nSimplex table!!!!!!!!!!!!!!!!!!!");
                //Service.TracingObjectMatrix(simplexTable);

                //Trace.WriteLine("Simplex algorithm!!!!!!!!!!!!!");
                //Service.TracingObjectMatrix(simplexTable);
                //Trace.WriteLine("");
                SimplexAlgorithm.Simplex(ref simplexTable);

                //Trace.WriteLine("\n\n Conditions");
                //Service.TracingConditionsList(conditionsSimplex);

                //Trace.WriteLine("\n\n Looking for values");
                solutionMatrix = SolutionFinder.GetSolution(simplexTable, targetFunction, ref minValue);
                //Service.TracingObjectMatrix(solutionMatrix);
                //Trace.WriteLine("\n\nF(x) = " + minValue);

                //Show our solution
                ShowSolution(STPAN_solutionContainer, TBL_Solution, solutionMatrix, minValue);
                BTN_Solve.IsEnabled = false;
                BTN_AdvanceSolution.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("При вводе значений коэффициентов была допущена ошибка. Проверьте введенные данные!");
            }
            finally
            {
                //...
            }
        }
        #endregion

        #region The button "Show solution's steps"
        
        //An event handler of button "Show all solution's steps"
        private void BTN_AdvanceSolution_Click(object sender, RoutedEventArgs e)
        {
            AdvanceDicisionWindow WINDOW_advanceDecision = new AdvanceDicisionWindow();
            WINDOW_advanceDecision.Owner = (this as DecisionWindow);
            WINDOW_advanceDecision.ShowDialog();
        }
        #endregion

        #region Reset all input values into fields

        //Set a values by default
        private void SetInitialValues(StackPanel _stackpanel)
        {
            foreach (DockPanel docPan in _stackpanel.Children)
            {
                foreach (UIElement item in docPan.Children)
                {
                    if (item is TextBox)
                        (item as TextBox).Text = "0";
                    if (item is ComboBox)
                        (item as ComboBox).SelectedIndex = 0;
                }
            }
        }

        //Set an initial values of variables
        private void ResetVariables()
        {
            targetFunction = null;
            conditionsSimplex.Clear();
            conditionsGraphic.Clear();
            artificialFunction = null;
            simplexTable = null;
            solutionMatrix = null;
            minValue = new double();

            //Also reset a variable which contains solution-table
            Service.solutionTables.Clear();
        }

        //An event handler of button "Reset"
        private void BTN_Reset_Click(object sender, RoutedEventArgs e)
        {
            //Produce a reset
            ResetVariables();
            //Fill all gaps by default
            SetInitialValues(STPAN_InitialTemplatesPanel);

            STPAN_solutionContainer.Visibility = System.Windows.Visibility.Collapsed;
            BTN_AdvanceSolution.IsEnabled = false;
            BTN_Solve.IsEnabled = true;
        }

        //An event handler of closed window button (a panel's button)
        private void DecisionWindow_Closed(object sender, EventArgs e)
        {
            //Produce a reset
            ResetVariables();
            //Set all gaps by default
            SetInitialValues(STPAN_InitialTemplatesPanel);
        }

        #endregion

               
        #endregion
    }
}
